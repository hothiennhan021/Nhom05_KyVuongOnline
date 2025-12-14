using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChessLogic; // Namespace chứa các file bạn vừa gửi

namespace MyTcpServer
{
    public class GameSession
    {
        public string SessionId { get; }
        public ConnectedClient PlayerWhite { get; }
        public ConnectedClient PlayerBlack { get; }

        public event Action<GameSession, Player, string> OnGameEnded;

        private GameState _gameState;
        private readonly ChessTimer _gameTimer;
        private readonly ChatRoom _chatRoom;
        private readonly string _connectionString = "Server=.;Database=ChessDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Lưu lịch sử nước đi dạng UCI (vd: "e2e4") để gửi cho Client phân tích sau trận
        private readonly List<string> _moveHistory = new List<string>();

        // Trạng thái yêu cầu tái đấu
        private bool _whiteWantsRematch = false;
        private bool _blackWantsRematch = false;

        public GameSession(ConnectedClient p1, ConnectedClient p2)
        {
            SessionId = Guid.NewGuid().ToString();

            // Random màu quân ngẫu nhiên cho công bằng
            if (new Random().Next(2) == 0)
            {
                PlayerWhite = p1;
                PlayerBlack = p2;
            }
            else
            {
                PlayerWhite = p2;
                PlayerBlack = p1;
            }

            // Khởi tạo bàn cờ mới
            _gameState = new GameState(Player.White, Board.Initial());

            // Khởi tạo đồng hồ (10 phút = 600 giây)
            // ChessTimer của bạn nhận tham số là 'minutes' -> OK
            _gameTimer = new ChessTimer(10);
            _gameTimer.TimeExpired += HandleTimeExpired;

            _chatRoom = new ChatRoom(PlayerWhite, PlayerBlack);
        }

        // Kiểm tra xem game đã kết thúc chưa (để GameManager dọn dẹp)
        public bool IsGameOver() => _gameState.IsGameOver();

        // --- BẮT ĐẦU VÁN CỜ ---
        public async Task StartGame()
        {
            _gameTimer.Start(Player.White);

            string board = Serialization.BoardToString(_gameState.Board);
            int wTime = _gameTimer.WhiteRemaining;
            int bTime = _gameTimer.BlackRemaining;

            // Ban đầu không có En Passant -> gửi "null"
            string epString = "null";

            int whiteElo = 1200;
            int blackElo = 1200;

            // Dùng connectionString đã khai báo ở các bước trước
            var userRepo = new ChessData.UserRepository(_connectionString);

            var wStats = await userRepo.GetUserStatsAsync(PlayerWhite.Username);
            if (wStats != null) whiteElo = wStats.Elo;

            var bStats = await userRepo.GetUserStatsAsync(PlayerBlack.Username);
            if (bStats != null) blackElo = bStats.Elo;

            // Format gói tin: GAME_START | MÀU | BÀN CỜ | TIME_W | TIME_B | EN_PASSANT_POS
            Console.WriteLine($"[SEND] GAME_START ELO: W={whiteElo}, B={blackElo}");

            await PlayerWhite.SendMessageAsync($"GAME_START|WHITE|{board}|{wTime}|{bTime}|{epString}|{whiteElo}|{blackElo}");
            await PlayerBlack.SendMessageAsync($"GAME_START|BLACK|{board}|{wTime}|{bTime}|{epString}|{whiteElo}|{blackElo}");

            Console.WriteLine($"[Game Started] {PlayerWhite.Username} vs {PlayerBlack.Username}");
        }

        // --- XỬ LÝ NƯỚC ĐI (CORE LOGIC) ---
        public async Task HandleMove(ConnectedClient client, string moveString)
        {
            try
            {
                Player player = (client == PlayerWhite) ? Player.White : Player.Black;
                if (player != _gameState.CurrentPlayer) return;

                var parts = moveString.Split('|');
                if (parts.Length < 5) return;

                int r1 = int.Parse(parts[1]); int c1 = int.Parse(parts[2]);
                int r2 = int.Parse(parts[3]); int c2 = int.Parse(parts[4]);
                Position from = new Position(r1, c1);
                Position to = new Position(r2, c2);

                Pieces piece = _gameState.Board[from];
                if (piece == null || piece.Color != player) return;

                IEnumerable<Move> moves = _gameState.MovesForPiece(from);
                Move move = null;

                if (parts.Length == 6) // Phong cấp
                {
                    PieceType promoType = (PieceType)int.Parse(parts[5]);
                    move = moves.OfType<PawnPromotion>().FirstOrDefault(m => m.ToPos == to && m.newType == promoType);
                }
                else // Nước đi thường (bao gồm En Passant)
                {
                    // Lọc bỏ PawnPromotion để tránh nhầm lẫn
                    move = moves.FirstOrDefault(m => m.ToPos == to && !(m is PawnPromotion));
                }

                if (move == null || !move.IsLegal(_gameState.Board))
                {
                    Console.WriteLine($"[Illegal] {client.Username}: {moveString}");
                    return;
                }

                // Thực hiện nước đi
                _gameState.MakeMove(move);
                _gameTimer.SwitchTurn();
                _moveHistory.Add(UciConverter.MoveToUci(move));

                // --- LẤY TỌA ĐỘ EN PASSANT ĐỂ GỬI CLIENT ---
                // (Nếu Đen vừa đi 2 bước, thì Board sẽ lưu SkipPosition cho quân Đen)
                // Chúng ta lấy SkipPosition của đối thủ (người vừa đi xong)
                Position? epPos = _gameState.Board.GetPawnSkipPosition(_gameState.CurrentPlayer.Opponent());

                string epString = "null";
                if (epPos != null)
                {
                    epString = $"{epPos.Row},{epPos.Column}"; // Ví dụ: "2,3"
                }

                string boardStr = Serialization.BoardToString(_gameState.Board);
                string curPlayer = _gameState.CurrentPlayer.ToString().ToUpper();

                // Gửi: UPDATE | BOARD | TURN | W_TIME | B_TIME | EP_POS
                await Broadcast($"UPDATE|{boardStr}|{curPlayer}|{_gameTimer.WhiteRemaining}|{_gameTimer.BlackRemaining}|{epString}");

                CheckGameOver();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
        }

        private async void CheckGameOver()
        {
            if (_gameState.IsGameOver())
            {
                _gameTimer.Stop();
                Player winner = _gameState.Result.Winner;

                string wStr = (winner == Player.White) ? "White" :
                              (winner == Player.Black) ? "Black" : "Draw";

                string reason = _gameState.Result.Reason.ToString();
                OnGameEnded?.Invoke(this, winner, reason);

                await Broadcast($"GAME_OVER_FULL|{wStr}|{reason}");
            }
        }

        // --- CÁC LỆNH HỆ THỐNG ---
        public async Task HandleGameCommand(ConnectedClient client, string command)
        {
            ConnectedClient opp = (client == PlayerWhite) ? PlayerBlack : PlayerWhite;

            switch (command)
            {
                case "RESIGN":
                    _gameTimer.Stop();
                   
                    Player winnerPlayer = (client == PlayerWhite) ? Player.Black : Player.White;
                    OnGameEnded?.Invoke(this, winnerPlayer, "Resignation");

                    string winnerColor = (client == PlayerWhite) ? "Black" : "White";

                    await Broadcast($"GAME_OVER_FULL|{winnerColor}|Opponent Resigned");
                    break;

                case "DRAW_OFFER":
                    await opp.SendMessageAsync("DRAW_OFFER");
                    break;

                case "DRAW_ACCEPT":
                    _gameTimer.Stop();

                    OnGameEnded?.Invoke(this, Player.None, "Mutual Agreement"); 

                    await Broadcast("GAME_OVER_FULL|Draw|Mutual Agreement");
                    break;

                case "REQUEST_RESTART":
                    if (client == PlayerWhite) _whiteWantsRematch = true;
                    else _blackWantsRematch = true;

                    if (_whiteWantsRematch && _blackWantsRematch)
                    {
                        await RestartGame();
                    }
                    else
                    {
                        await opp.SendMessageAsync("ASK_RESTART");
                    }
                    break;

                case "RESTART_NO":
                    _whiteWantsRematch = false;
                    _blackWantsRematch = false;
                    await opp.SendMessageAsync("RESTART_DENIED");
                    break;

                case "LEAVE_GAME":
                    await opp.SendMessageAsync("OPPONENT_LEFT");
                    break;
            }
        }

        private async Task RestartGame()
        {
            Console.WriteLine($"[Rematch] {PlayerWhite.Username} vs {PlayerBlack.Username}");

            _gameState = new GameState(Player.White, Board.Initial());
            _moveHistory.Clear();

            _gameTimer.Stop();
            _gameTimer.Sync(600, 600); // Reset 10 phút

            _whiteWantsRematch = false;
            _blackWantsRematch = false;

            await StartGame();
        }

        private void HandleTimeExpired(Player loser)
        {
            Player winnerPlayer = (loser == Player.White) ? Player.Black : Player.White;
            OnGameEnded?.Invoke(this, winnerPlayer, "TimeOut");

            string winner = (loser == Player.White) ? "Black" : "White";
            
            // Fire-and-forget task
            _ = Broadcast($"GAME_OVER_FULL|{winner}|TimeOut");
        }

        public async Task BroadcastChat(ConnectedClient sender, string msg)
        {
            await _chatRoom.SendMessage(sender, msg);
        }

        public async Task HandleAnalysisRequest(ConnectedClient client)
        {
            if (client != PlayerWhite && client != PlayerBlack) return;
            string data = string.Join(" ", _moveHistory);
            await client.SendMessageAsync($"ANALYSIS_DATA|{data}");
        }

        private async Task Broadcast(string msg)
        {
            var tasks = new List<Task>();
            if (PlayerWhite.Client.Connected) tasks.Add(PlayerWhite.SendMessageAsync(msg));
            if (PlayerBlack.Client.Connected) tasks.Add(PlayerBlack.SendMessageAsync(msg));
            await Task.WhenAll(tasks);
        }
    }
}
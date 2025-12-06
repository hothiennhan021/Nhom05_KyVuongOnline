using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChessLogic;

namespace MyTcpServer
{
    public class GameSession
    {
        public string SessionId { get; }
        public ConnectedClient PlayerWhite { get; }
        public ConnectedClient PlayerBlack { get; }

        private GameState _gameState;
        private readonly ChessTimer _gameTimer;   // Dùng ChessTimer (từ ChessLogic)
        private readonly ChatRoom _chatRoom;
        private readonly List<string> _moveHistory = new List<string>();

        private bool _whiteWantsRematch = false;
        private bool _blackWantsRematch = false;

        public GameSession(ConnectedClient p1, ConnectedClient p2)
        {
            SessionId = Guid.NewGuid().ToString();

            // Random màu quân (logic giống file gốc)
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

            _gameState = new GameState(Player.White, Board.Initial());
            _gameTimer = new ChessTimer(10); // 10 phút mỗi bên
            _gameTimer.TimeExpired += HandleTimeExpired;

            _chatRoom = new ChatRoom(PlayerWhite, PlayerBlack);
        }

        // Cho GameManager kiểm tra
        public bool IsGameOver() => _gameState.IsGameOver();

        // Bắt đầu ván cờ – giống file gốc
        public async Task StartGame()
        {
            _gameTimer.Start(Player.White);

            string board = Serialization.BoardToString(_gameState.Board);
            int wTime = _gameTimer.WhiteRemaining;
            int bTime = _gameTimer.BlackRemaining;

            await PlayerWhite.SendMessageAsync($"GAME_START|WHITE|{board}|{wTime}|{bTime}");
            await PlayerBlack.SendMessageAsync($"GAME_START|BLACK|{board}|{wTime}|{bTime}");

            Console.WriteLine($"[Game Started] {PlayerWhite.Username} vs {PlayerBlack.Username}");
        }

        // =======================
        //  HANDLE MOVE – LOGIC GỐC
        // =======================
        public async Task HandleMove(ConnectedClient client, string moveString)
        {
            try
            {
                // 1. Xác định player đang đi
                Player player = (client == PlayerWhite) ? Player.White : Player.Black;

                // Kiểm tra đúng lượt
                if (player != _gameState.CurrentPlayer)
                {
                    Console.WriteLine($"[Block] {client.Username} đi sai lượt.");
                    return;
                }

                // 2. Parse chuỗi lệnh
                // Format: MOVE|r1|c1|r2|c2|[promotionType]
                var parts = moveString.Split('|');
                if (parts.Length < 5) return;

                int r1 = int.Parse(parts[1]);
                int c1 = int.Parse(parts[2]);
                int r2 = int.Parse(parts[3]);
                int c2 = int.Parse(parts[4]);

                Position from = new Position(r1, c1);
                Position to = new Position(r2, c2);

                // 3. Lấy quân và toàn bộ nước đi hợp lệ của quân đó
                Pieces piece = _gameState.Board[from];
                if (piece == null || piece.Color != player) return;

                IEnumerable<Move> moves = _gameState.MovesForPiece(from);

                Move move = null;

                // --- XỬ LÝ PHONG CẤP (PROMOTION) ---
                // Nếu client gửi thêm type (VD: 4 = Queen)
                if (parts.Length == 6)
                {
                    int typeId = int.Parse(parts[5]);
                    PieceType promoType = (PieceType)typeId;

                    // Tìm đúng PawnPromotion (đúng đích + đúng loại quân muốn phong)
                    move = moves
                        .OfType<PawnPromotion>()
                        .FirstOrDefault(m => m.ToPos == to && m.newType == promoType);
                    // Lưu ý: thuộc tính 'newType' trong PawnPromotion phải là public/protected như file gốc.
                }
                else
                {
                    // Nước đi thường (bao gồm cả nhập thành, bắt tốt qua đường...)
                    // Loại trừ PawnPromotion để tránh phong sai loại
                    move = moves.FirstOrDefault(m => m.ToPos == to && !(m is PawnPromotion));
                }

                // Không tìm được move hợp lệ
                if (move == null || !move.IsLegal(_gameState.Board))
                {
                    Console.WriteLine("Nước đi không hợp lệ hoặc không tìm thấy trên Server.");
                    return;
                }

                // 4. Thực hiện nước đi & đổi lượt
                _gameState.MakeMove(move);
                _gameTimer.SwitchTurn();

                // Ghi lịch sử ván cờ bằng UCI (cho phân tích trận)
                string uciString = UciConverter.MoveToUci(move);
                _moveHistory.Add(uciString);

                // 5. Gửi UPDATE cho cả 2 bên
                string boardStr = Serialization.BoardToString(_gameState.Board);
                string curPlayer = _gameState.CurrentPlayer.ToString().ToUpper(); // "WHITE"/"BLACK"

                await Broadcast($"UPDATE|{boardStr}|{curPlayer}|{_gameTimer.WhiteRemaining}|{_gameTimer.BlackRemaining}");

                // 6. Kiểm tra kết thúc ván
                if (_gameState.IsGameOver())
                {
                    _gameTimer.Stop();
                    Player winner = _gameState.Result.Winner;

                    string wStr =
                        (winner == Player.White) ? "White" :
                        (winner == Player.Black) ? "Black" :
                        "Draw";

                    string reason = _gameState.Result.Reason.ToString();

                    await Broadcast($"GAME_OVER_FULL|{wStr}|{reason}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Move Error] {ex.Message}\n{ex.StackTrace}");
            }
        }

        // =======================
        //  PHÂN TÍCH TRẬN – LOGIC GỐC
        // =======================
        public async Task HandleAnalysisRequest(ConnectedClient client)
        {
            // Chỉ cho 2 người trong ván yêu cầu
            if (client != PlayerWhite && client != PlayerBlack) return;

            // Ghép thành chuỗi kiểu: "e2e4 e7e5 g1f3 ..."
            string data = string.Join(" ", _moveHistory);

            await client.SendMessageAsync($"ANALYSIS_DATA|{data}");
        }

        // =======================
        //  LỆNH HỆ THỐNG (giống gốc)
        // =======================
        public async Task HandleGameCommand(ConnectedClient client, string command)
        {
            if (command == "REQUEST_RESTART")
            {
                if (client == PlayerWhite) _whiteWantsRematch = true;
                else _blackWantsRematch = true;

                ConnectedClient opp = (client == PlayerWhite) ? PlayerBlack : PlayerWhite;

                if (_whiteWantsRematch && _blackWantsRematch)
                {
                    await RestartGame();
                }
                else
                {
                    await opp.SendMessageAsync("ASK_RESTART");
                }
            }
            else if (command == "RESTART_NO")
            {
                _whiteWantsRematch = false;
                _blackWantsRematch = false;

                ConnectedClient opp = (client == PlayerWhite) ? PlayerBlack : PlayerWhite;
                await opp.SendMessageAsync("RESTART_DENIED");
            }
            else if (command == "LEAVE_GAME")
            {
                ConnectedClient opp = (client == PlayerWhite) ? PlayerBlack : PlayerWhite;
                // GameManager sẽ lo dọn dẹp _activeGames, ở đây chỉ báo cho đối thủ
                await opp.SendMessageAsync("OPPONENT_LEFT");
            }
        }

        private async Task RestartGame()
        {
            _gameState = new GameState(Player.White, Board.Initial());
            _gameTimer.Stop();
            _gameTimer.Sync(600, 600); // reset 10 phút mỗi bên (600s)
            _whiteWantsRematch = false;
            _blackWantsRematch = false;

            await StartGame();
        }

        private void HandleTimeExpired(Player loser)
        {
            string winner = (loser == Player.White) ? "Black" : "White";
            _ = Broadcast($"GAME_OVER_FULL|{winner}|TimeOut");
        }

        public async Task BroadcastChat(ConnectedClient sender, string msg)
        {
            await _chatRoom.SendMessage(sender, msg);
        }

        private async Task Broadcast(string msg)
        {
            try { await PlayerWhite.SendMessageAsync(msg); } catch { }
            try { await PlayerBlack.SendMessageAsync(msg); } catch { }
        }
    }
}

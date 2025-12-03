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
        private readonly ChessTimer _gameTimer;
        private readonly ChatRoom _chatRoom;
        private readonly List<string> _moveHistory = new List<string>();

        private bool _whiteWantsRematch = false;
        private bool _blackWantsRematch = false;

        public GameSession(ConnectedClient p1, ConnectedClient p2)
        {
            SessionId = Guid.NewGuid().ToString();

            // Random màu quân
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

        public bool IsGameOver() => _gameState.IsGameOver();

        public async Task StartGame()
        {
            _gameTimer.Start(Player.White);

            string board = Serialization.BoardToString(_gameState.Board);

            // Gửi tin nhắn bắt đầu - LƯU Ý: Dùng "WHITE" và "BLACK" in hoa
            await PlayerWhite.SendMessageAsync($"GAME_START|WHITE|{board}|{_gameTimer.WhiteRemaining}|{_gameTimer.BlackRemaining}");
            await PlayerBlack.SendMessageAsync($"GAME_START|BLACK|{board}|{_gameTimer.WhiteRemaining}|{_gameTimer.BlackRemaining}");

            Console.WriteLine($"[Game Started] {PlayerWhite.Username} vs {PlayerBlack.Username}");
        }

        // ============================================================
        // [FIXED] XỬ LÝ NƯỚC ĐI VÀ GỬI UPDATE CHUẨN
        // ============================================================
        public async Task HandleMove(ConnectedClient client, string moveString)
        {
            try
            {
                // 1. Kiểm tra đúng lượt (Server validation)
                Player currentPlayer = _gameState.CurrentPlayer;
                bool isWhiteClient = (client == PlayerWhite);

                // Nếu Client gửi lệnh khi chưa đến lượt -> Bỏ qua
                if ((isWhiteClient && currentPlayer != Player.White) ||
                    (!isWhiteClient && currentPlayer != Player.Black))
                {
                    Console.WriteLine($"[Block] {client.Username} đi sai lượt.");
                    return;
                }

                // 2. Parse tọa độ (Hỗ trợ cả '|' và ',')
                // Format mong đợi: "MOVE|r1|c1|r2|c2" hoặc "MOVE|r1,c1,r2,c2"
                string[] parts = moveString.Split('|');
                int r1, c1, r2, c2;
                int typeId = 0;

                if (parts.Length >= 5) // Dạng MOVE|r1|c1|r2|c2
                {
                    r1 = int.Parse(parts[1]); c1 = int.Parse(parts[2]);
                    r2 = int.Parse(parts[3]); c2 = int.Parse(parts[4]);
                    if (parts.Length > 5) typeId = int.Parse(parts[5]);
                }
                else if (parts.Length == 2 && parts[1].Contains(",")) // Dạng MOVE|r1,c1,r2,c2
                {
                    string[] coords = parts[1].Split(',');
                    r1 = int.Parse(coords[0]); c1 = int.Parse(coords[1]);
                    r2 = int.Parse(coords[2]); c2 = int.Parse(coords[3]);
                }
                else return;

                // 3. Tìm và thực hiện nước đi
                Position from = new Position(r1, c1);
                Position to = new Position(r2, c2);
                IEnumerable<Move> moves = _gameState.MovesForPiece(from);

                Move move = moves.FirstOrDefault(m => m.ToPos == to);

                // Xử lý phong cấp (nếu client không gửi type, mặc định Queen)
                if (move == null) move = moves.OfType<PawnPromotion>().FirstOrDefault(m => m.ToPos == to);

                if (move == null) return; // Nước đi không hợp lệ

                _gameState.MakeMove(move);
                _gameTimer.SwitchTurn();
                _moveHistory.Add($"{r1},{c1}->{r2},{c2}");

                // 4. [QUAN TRỌNG] GỬI UPDATE CHO CẢ 2
                // Client của bạn cần nhận lệnh UPDATE để vẽ lại bàn cờ và mở khóa lượt
                string boardStr = Serialization.BoardToString(_gameState.Board);

                // *** FIX LỖI 2 NƯỚC ***: Chuyển CurrentPlayer thành IN HOA ("WHITE"/"BLACK")
                // Để khớp với lúc StartGame, giúp Client so sánh chuỗi đúng
                string nextTurnStr = _gameState.CurrentPlayer.ToString().ToUpper();

                string updateMsg = $"UPDATE|{boardStr}|{nextTurnStr}|{_gameTimer.WhiteRemaining}|{_gameTimer.BlackRemaining}";
                await Broadcast(updateMsg);

                // (Option) Gửi thêm lệnh MOVE ngắn gọn nếu client cần animation
                // await Broadcast($"MOVE|{r1}|{c1}|{r2}|{c2}");

                Console.WriteLine($"[Move Valid] {client.Username} moved. Next: {nextTurnStr}");

                // 5. Kiểm tra kết thúc
                if (_gameState.IsGameOver())
                {
                    _gameTimer.Stop();
                    string winner = _gameState.Result.Winner == Player.White ? "White" : (_gameState.Result.Winner == Player.Black ? "Black" : "Draw");
                    await Broadcast($"GAME_OVER_FULL|{winner}|{_gameState.Result.Reason}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error HandleMove] " + ex.Message);
            }
        }

        // --- CÁC HÀM KHÁC GIỮ NGUYÊN ---
        public async Task HandleAnalysisRequest(ConnectedClient client)
        {
            if (client != PlayerWhite && client != PlayerBlack) return;
            string data = string.Join(";", _moveHistory);
            await client.SendMessageAsync($"ANALYSIS_DATA|{data}");
        }

        public async Task HandleGameCommand(ConnectedClient client, string command)
        {
            if (command == "REQUEST_RESTART")
            {
                if (client == PlayerWhite) _whiteWantsRematch = true;
                else _blackWantsRematch = true;

                ConnectedClient opp = (client == PlayerWhite) ? PlayerBlack : PlayerWhite;
                if (_whiteWantsRematch && _blackWantsRematch) await RestartGame();
                else await opp.SendMessageAsync("ASK_RESTART");
            }
            else if (command == "RESTART_NO")
            {
                _whiteWantsRematch = false; _blackWantsRematch = false;
                ConnectedClient opp = (client == PlayerWhite) ? PlayerBlack : PlayerWhite;
                await opp.SendMessageAsync("RESTART_DENIED");
            }
            else if (command == "LEAVE_GAME")
            {
                ConnectedClient opp = (client == PlayerWhite) ? PlayerBlack : PlayerWhite;
                await opp.SendMessageAsync("OPPONENT_LEFT");
            }
        }

        private async Task RestartGame()
        {
            _gameState = new GameState(Player.White, Board.Initial());
            _gameTimer.Sync(600, 600);
            _whiteWantsRematch = false; _blackWantsRematch = false;
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
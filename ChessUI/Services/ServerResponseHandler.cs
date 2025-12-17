using ChessLogic;
using System;
using System.Linq;

namespace ChessUI.Services
{
    public class GameStartEventArgs : EventArgs
    {
        public required Player MyColor { get; set; }
        public required Board Board { get; set; }
        public int WhiteTime { get; set; }
        public int BlackTime { get; set; }
        // [MỚI] Vị trí bắt tốt qua đường (nếu có)

        public int WhiteElo { get; set; } = 1200;
        public int BlackElo { get; set; } = 1200;

        public string? WhiteName { get; set; }
        public string? BlackName { get; set; }


        public Position? EnPassantPos { get; set; }
    }

    public class GameUpdateEventArgs : EventArgs
    {
        public required Board Board { get; set; }
        public required Player CurrentPlayer { get; set; }
        public int WhiteTime { get; set; }
        public int BlackTime { get; set; }
        // [MỚI] Vị trí bắt tốt qua đường (nếu có)
        public Position? EnPassantPos { get; set; }
    }

    public class ChatEventArgs : EventArgs
    {
        public required string Sender { get; set; }
        public required string Content { get; set; }
    }

    public class ServerResponseHandler
    {
        // Các sự kiện Game
        public event EventHandler<GameStartEventArgs>? GameStarted;
        public event EventHandler<GameUpdateEventArgs>? GameUpdated;
        public event EventHandler<ChatEventArgs>? ChatReceived;

        // Các sự kiện thông báo
        public event Action<string>? ErrorReceived;
        public event Action? WaitingReceived;
        public event Action<string>? GameOverReceived;
        public event Action<string, string>? GameOverFullReceived; // Winner, Reason

        // Các sự kiện tương tác
        public event Action? AskRestartReceived;
        public event Action? RestartDeniedReceived;
        public event Action? OpponentLeftReceived;
        public event Action<string>? AnalysisDataReceived;
        public event Action? DrawOfferReceived;

        public void ProcessMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            var parts = message.Split('|');
            var command = parts[0];

            try
            {
                switch (command)
                {
                    case "GAME_START":
                        // Format: GAME_START | COLOR | BOARD | W_TIME | B_TIME | EP_POS
                        int whiteElo = 1200;
                        int blackElo = 1200;

                        if (parts.Length >= 8)
                        {
                            int.TryParse(parts[6], out whiteElo);
                            int.TryParse(parts[7], out blackElo);
                        }
                        string? whiteName = null;
                        string? blackName = null;

                        if (parts.Length >= 10)
                        {
                            whiteName = parts[8];
                            blackName = parts[9];
                        }

                        var argsStart = new GameStartEventArgs
                        {
                            MyColor = (parts[1] == "WHITE") ? Player.White : Player.Black,
                            Board = Serialization.ParseBoardString(parts[2]),
                            WhiteTime = (parts.Length >= 5) ? int.Parse(parts[3]) : 0,
                            BlackTime = (parts.Length >= 5) ? int.Parse(parts[4]) : 0,

                            WhiteElo = whiteElo,
                            BlackElo = blackElo,
                            WhiteName = whiteName,
                            BlackName = blackName,
                            // [MỚI] Parse vị trí En Passant
                            EnPassantPos = (parts.Length >= 6) ? ParseEpString(parts[5]) : null

                        };
                        GameStarted?.Invoke(this, argsStart);
                        break;

                    case "UPDATE":
                        // Format: UPDATE | BOARD | TURN | W_TIME | B_TIME | EP_POS
                        var argsUpdate = new GameUpdateEventArgs
                        {
                            Board = Serialization.ParseBoardString(parts[1]),
                            CurrentPlayer = (parts[2] == "WHITE") ? Player.White : Player.Black,
                            WhiteTime = (parts.Length >= 5) ? int.Parse(parts[3]) : 0,
                            BlackTime = (parts.Length >= 5) ? int.Parse(parts[4]) : 0,
                            // [MỚI] Parse vị trí En Passant
                            EnPassantPos = (parts.Length >= 6) ? ParseEpString(parts[5]) : null
                        };
                        GameUpdated?.Invoke(this, argsUpdate);
                        break;

                    case "CHAT_RECEIVE":
                        if (parts.Length >= 3)
                        {
                            ChatReceived?.Invoke(this, new ChatEventArgs
                            {
                                Sender = parts[1],
                                Content = string.Join("|", parts.Skip(2))
                            });
                        }
                        break;

                    // --- CÁC LỆNH KHÁC GIỮ NGUYÊN ---
                    case "DRAW_OFFER": DrawOfferReceived?.Invoke(); break;
                    case "GAME_OVER_FULL":
                        if (parts.Length >= 3) GameOverFullReceived?.Invoke(parts[1], parts[2]);
                        break;
                    case "OPPONENT_LEFT": OpponentLeftReceived?.Invoke(); break;
                    case "ERROR": ErrorReceived?.Invoke(parts[1]); break;
                    case "WAITING": WaitingReceived?.Invoke(); break;
                    case "GAME_OVER": GameOverReceived?.Invoke(parts[1]); break; // Legacy
                    case "ASK_RESTART": AskRestartReceived?.Invoke(); break;
                    case "RESTART_DENIED": RestartDeniedReceived?.Invoke(); break;
                    case "ANALYSIS_DATA":
                        if (parts.Length >= 2) AnalysisDataReceived?.Invoke(parts[1]);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Có thể log lỗi parse ở đây nếu cần
                Console.WriteLine("Parse Error: " + ex.Message);
            }
        }

        // [HÀM HỖ TRỢ] Chuyển đổi chuỗi "2,3" thành đối tượng Position
        private Position? ParseEpString(string epStr)
        {
            if (string.IsNullOrEmpty(epStr) || epStr == "null") return null;

            try
            {
                var p = epStr.Split(',');
                return new Position(int.Parse(p[0]), int.Parse(p[1]));
            }
            catch
            {
                return null;
            }
        }
    }
}
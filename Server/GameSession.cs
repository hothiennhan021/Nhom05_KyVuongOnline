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
            _gameTimer = new ChessTimer(10);
            _gameTimer.TimeExpired += HandleTimeExpired;

            _chatRoom = new ChatRoom(PlayerWhite, PlayerBlack);
        }

        public bool IsGameOver() => _gameState.IsGameOver();

        public async Task StartGame()
        {
            _gameTimer.Start(Player.White);

            string board = Serialization.BoardToString(_gameState.Board);

            await PlayerWhite.SendMessageAsync($"GAME_START|WHITE|{board}|{_gameTimer.WhiteRemaining}|{_gameTimer.BlackRemaining}");
            await PlayerBlack.SendMessageAsync($"GAME_START|BLACK|{board}|{_gameTimer.WhiteRemaining}|{_gameTimer.BlackRemaining}");
        }

  
        public async Task HandleMove(ConnectedClient client, string moveString)
        {
            try
            {
                Player player = (client == PlayerWhite) ? Player.White : Player.Black;
                if (player != _gameState.CurrentPlayer)
                    return;

                var parts = moveString.Split('|');
                if (parts.Length < 5) return;

                int r1 = int.Parse(parts[1]);
                int c1 = int.Parse(parts[2]);
                int r2 = int.Parse(parts[3]);
                int c2 = int.Parse(parts[4]);

                Position from = new Position(r1, c1);
                Position to = new Position(r2, c2);

                Pieces piece = _gameState.Board[from];
                if (piece == null || piece.Color != player)
                    return;

                IEnumerable<Move> moves = _gameState.MovesForPiece(from);

                Move move = null;

                if (parts.Length == 6)
                {
                    int typeId = int.Parse(parts[5]);
                    PieceType promo = (PieceType)typeId;

                    move = moves
                        .OfType<PawnPromotion>()
                        .FirstOrDefault(m => m.ToPos == to && m.newType == promo);
                }
                else
                {
                    move = moves.FirstOrDefault(m => m.ToPos == to);
                }

                if (move == null)
                    return;

                _gameState.MakeMove(move);
                _gameTimer.SwitchTurn();

                _moveHistory.Add(UciConverter.MoveToUci(move));

                string boardStr = Serialization.BoardToString(_gameState.Board);

                await Broadcast($"UPDATE|{boardStr}|{_gameState.CurrentPlayer}|{_gameTimer.WhiteRemaining}|{_gameTimer.BlackRemaining}");

                if (_gameState.IsGameOver())
                {
                    _gameTimer.Stop();

                    Player win = _gameState.Result.Winner;

                    string winner =
                        (win == Player.White) ? "White" :
                        (win == Player.Black) ? "Black" : "Draw";

                    string reason = _gameState.Result.Reason.ToString();

                    await Broadcast($"GAME_OVER_FULL|{winner}|{reason}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Move Error: " + ex.Message);
            }
        }

      
        public async Task HandleAnalysisRequest(ConnectedClient client)
        {
            if (client != PlayerWhite && client != PlayerBlack)
                return;

            string data = string.Join(" ", _moveHistory);
            await client.SendMessageAsync($"ANALYSIS_DATA|{data}");
        }

     
        public async Task HandleGameCommand(ConnectedClient client, string command)
        {
            if (command == "REQUEST_RESTART")
            {
                if (client == PlayerWhite) _whiteWantsRematch = true;
                else _blackWantsRematch = true;

                ConnectedClient opp = (client == PlayerWhite) ? PlayerBlack : PlayerWhite;

                if (_whiteWantsRematch && _blackWantsRematch)
                    await RestartGame();
                else
                    await opp.SendMessageAsync("ASK_RESTART");
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
                await opp.SendMessageAsync("OPPONENT_LEFT");
            }
        }

        private async Task RestartGame()
        {
            _gameState = new GameState(Player.White, Board.Initial());
            _gameTimer.Sync(600, 600);

            _whiteWantsRematch = false;
            _blackWantsRematch = false;

            await StartGame();
        }

        private void HandleTimeExpired(Player loser)
        {
            string winner =
                (loser == Player.White) ? "Black" :
                (loser == Player.Black) ? "White" : "Draw";

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

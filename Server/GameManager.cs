using ChessLogic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTcpServer
{
    public class PendingMatch
    {
        public string MatchId { get; set; }
        public ConnectedClient Player1 { get; set; }
        public ConnectedClient Player2 { get; set; }
        public bool P1Accepted { get; set; } = false;
        public bool P2Accepted { get; set; } = false;
        public object LockObj = new object(); // Khóa để tránh race condition

        public PendingMatch(ConnectedClient p1, ConnectedClient p2)
        {
            MatchId = Guid.NewGuid().ToString().Substring(0, 8);
            Player1 = p1;
            Player2 = p2;
        }
    }

    public static class GameManager
    {
        private static readonly List<ConnectedClient> _waitingLobby = new List<ConnectedClient>();
        private static readonly object _lobbyLock = new object();

        private static readonly ConcurrentDictionary<ConnectedClient, GameSession> _activeGames = new ConcurrentDictionary<ConnectedClient, GameSession>();
        private static readonly ConcurrentDictionary<string, ConnectedClient> _privateRooms = new ConcurrentDictionary<string, ConnectedClient>();
        private static readonly ConcurrentDictionary<string, PendingMatch> _pendingMatches = new ConcurrentDictionary<string, PendingMatch>();

        public static void HandleClientConnect(ConnectedClient client) { }

        public static void HandleClientDisconnect(ConnectedClient client)
        {
            lock (_lobbyLock) { _waitingLobby.Remove(client); }

            var roomKey = _privateRooms.FirstOrDefault(x => x.Value == client).Key;
            if (roomKey != null) _privateRooms.TryRemove(roomKey, out _);

            var pending = _pendingMatches.Values.FirstOrDefault(m => m.Player1 == client || m.Player2 == client);
            if (pending != null)
            {
                CancelPendingMatch(pending, "Đối thủ đã thoát.").Wait();
            }

            if (_activeGames.TryRemove(client, out GameSession session))
            {
                if (!session.IsGameOver())
                {
                    var other = (session.PlayerWhite == client) ? session.PlayerBlack : session.PlayerWhite;
                    other.SendMessageAsync("GAME_OVER_FULL|Đối thủ thoát|Resigned").Wait();
                    _activeGames.TryRemove(other, out _);
                }
            }
        }

        // --- TÌM TRẬN ---
        public static async Task FindGame(ConnectedClient client)
        {
            PendingMatch newMatch = null;

            lock (_lobbyLock)
            {
                _waitingLobby.RemoveAll(c => !c.Client.Connected);
                if (!_waitingLobby.Contains(client)) _waitingLobby.Add(client);

                if (_waitingLobby.Count >= 2)
                {
                    var p1 = _waitingLobby[0];
                    var p2 = _waitingLobby[1];
                    _waitingLobby.RemoveRange(0, 2);

                    newMatch = new PendingMatch(p1, p2);
                    _pendingMatches[newMatch.MatchId] = newMatch;
                }
            }

            if (newMatch != null)
            {
                await newMatch.Player1.SendMessageAsync($"MATCH_FOUND|{newMatch.MatchId}");
                await newMatch.Player2.SendMessageAsync($"MATCH_FOUND|{newMatch.MatchId}");
            }
            else
            {
                await client.SendMessageAsync("WAITING");
            }
        }

        // --- XỬ LÝ ACCEPT / DECLINE ---
        public static async Task HandleMatchResponse(ConnectedClient client, string matchId, string response)
        {
            if (_pendingMatches.TryGetValue(matchId, out PendingMatch match))
            {
                if (response == "DECLINE")
                {
                    await CancelPendingMatch(match, "Đối thủ đã từ chối.");
                }
                else if (response == "ACCEPT")
                {
                    bool startGame = false;
                    lock (match.LockObj)
                    {
                        if (client == match.Player1) match.P1Accepted = true;
                        if (client == match.Player2) match.P2Accepted = true;

                        if (match.P1Accepted && match.P2Accepted)
                        {
                            startGame = true;
                            _pendingMatches.TryRemove(matchId, out _);
                        }
                    }

                    if (startGame)
                    {
                        var session = new GameSession(match.Player1, match.Player2);
                        session.OnGameEnded += HandleGameEndLog;
                        _activeGames[match.Player1] = session;
                        _activeGames[match.Player2] = session;
                        await session.StartGame();
                    }
                }
            }
        }

        private static async Task CancelPendingMatch(PendingMatch match, string reason)
        {
            _pendingMatches.TryRemove(match.MatchId, out _);
            try { await match.Player1.SendMessageAsync($"MATCH_CANCELLED|{reason}"); } catch { }
            try { await match.Player2.SendMessageAsync($"MATCH_CANCELLED|{reason}"); } catch { }
        }

        // --- ROOM RIÊNG ---
        public static async Task CreateRoom(ConnectedClient creator)
        {
            string id = new Random().Next(1000, 9999).ToString();
            while (_privateRooms.ContainsKey(id)) id = new Random().Next(1000, 9999).ToString();

            if (_privateRooms.TryAdd(id, creator))
                await creator.SendMessageAsync($"ROOM_CREATED|{id}");
        }

        public static async Task JoinRoom(ConnectedClient joiner, string id)
        {
            if (_privateRooms.TryRemove(id, out ConnectedClient creator))
            {
                if (!creator.Client.Connected)
                {
                    await joiner.SendMessageAsync("ROOM_ERROR|Phòng đã hủy.");
                    return;
                }
                var session = new GameSession(creator, joiner);
                session.OnGameEnded += HandleGameEndLog;
                _activeGames[creator] = session;
                _activeGames[joiner] = session;
                await session.StartGame();
            }
            else
            {
                await joiner.SendMessageAsync("ROOM_ERROR|Sai ID phòng.");
            }
        }

        public static async Task ProcessGameCommand(ConnectedClient client, string command)
        {
            if (_activeGames.TryGetValue(client, out GameSession session))
            {
                string[] parts = command.Split('|');
                string cmd = parts[0];

                if (cmd == "MOVE") await session.HandleMove(client, command);
                else if (cmd == "CHAT") await session.BroadcastChat(client, parts[1]);
                else if (cmd == "REQUEST_ANALYSIS") await session.HandleAnalysisRequest(client);
                else
                {
                    await session.HandleGameCommand(client, cmd);
                    if (cmd == "LEAVE_GAME")
                    {
                        _activeGames.TryRemove(session.PlayerWhite, out _);
                        _activeGames.TryRemove(session.PlayerBlack, out _);
                    }
                }
            }
        }

        // Thêm hàm này vào cuối class GameManager
        // Đặt hàm này trong GameManager.cs
        private static async void HandleGameEndLog(GameSession session, Player winner, string reason)
        {
            // Chuỗi kết nối (Thay bằng chuỗi kết nối của bạn)
            string connectionString = "Server=.;Database=ChessDB;Trusted_Connection=True;TrustServerCertificate=True;";
            var userRepo = new ChessData.UserRepository(connectionString);

            try
            {
                if (winner != Player.None)
                {
                    var winnerClient = (winner == Player.White) ? session.PlayerWhite : session.PlayerBlack;
                    var loserClient = (winner == Player.White) ? session.PlayerBlack : session.PlayerWhite;

                    // --- SỬA Ở ĐÂY: Dùng .Username thay vì .Client.Username ---
                    await userRepo.UpdateGameResultAsync(winnerClient.Username, 25, 1);
                    await userRepo.UpdateGameResultAsync(loserClient.Username, -25, -1);
                }
                else // Hòa
                {
                    // --- SỬA Ở ĐÂY: Dùng .Username thay vì .Client.Username ---
                    await userRepo.UpdateGameResultAsync(session.PlayerWhite.Username, 0, 0);
                    await userRepo.UpdateGameResultAsync(session.PlayerBlack.Username, 0, 0);
                }

                Console.WriteLine($"[Safe Log] Game {session.SessionId} result saved to DB.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB Error] Failed to save result: {ex.Message}");
            }
        }
    }
}
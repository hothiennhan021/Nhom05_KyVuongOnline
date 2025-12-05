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

        private static readonly ConcurrentDictionary<string, ConnectedClient> _onlineClients
            = new ConcurrentDictionary<string, ConnectedClient>();
        public static void HandleClientConnect(ConnectedClient client)
        {
            if (!string.IsNullOrEmpty(client.Username))
            {
                _onlineClients[client.Username] = client;
            }
        }

        public static void HandleClientDisconnect(ConnectedClient client)
        {
            if (!string.IsNullOrEmpty(client.Username))
            {
                _onlineClients.TryRemove(client.Username, out _);
            }

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



        // Trong GameManager.cs

        // Lưu ý: Đổi từ 'string' sang 'async Task<string>'
        // Trong GameManager.cs

        // Trong GameManager.cs

        public static string StartChallengeGame(string player1Name, string player2Name)
        {
            // Kiểm tra online
            if (_onlineClients.TryGetValue(player1Name, out var p1) &&
                _onlineClients.TryGetValue(player2Name, out var p2))
            {
                var session = new GameSession(p1, p2);
                _activeGames[p1] = session;
                _activeGames[p2] = session;

                // Lấy bàn cờ
                string rawBoard = session.StartGameSilent();

                // Thời gian (nếu GameSession chưa public GameTimer thì hardcode tạm 600)
                string timeW = "600";
                string timeB = "600";

                // [QUAN TRỌNG] Kiểm tra xem trong session, p1 (người mời) là màu gì?
                // Vì Session random, nên phải check thực tế
                string p1Color = (session.PlayerWhite.Username == p1.Username) ? "WHITE" : "BLACK";

                return $"GAME_START|{p1Color}|{rawBoard}|{timeW}|{timeB}";
            }
            return "ERROR|Đối thủ không còn trực tuyến.";
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
                _activeGames[creator] = session;
                _activeGames[joiner] = session;
                await session.StartGame();
            }
            else
            {
                await joiner.SendMessageAsync("ROOM_ERROR|Sai ID phòng.");
            }
        }

        // Trong GameManager.cs

        public static async Task ProcessGameCommand(ConnectedClient client, string command)
        {
            GameSession session = null;

            // =========================================================================
            // CASE 1: LOGIC CHUẨN (Dành cho Ghép Ngẫu Nhiên & Tạo Phòng & User không bị lỗi)
            // =========================================================================
            // Nếu client hiện tại khớp 100% với client lưu trong phòng -> Lấy ra dùng luôn.
            // Hai chế độ kia sẽ LUÔN LUÔN chạy vào dòng này nên không bao giờ bị ảnh hưởng.
            if (_activeGames.TryGetValue(client, out session))
            {
                // Tìm thấy ngon lành, không cần làm gì thêm.
            }

            // =========================================================================
            // CASE 2: LOGIC CỨU HỘ (Chỉ chạy khi CASE 1 thất bại - Dành riêng cho Thách Đấu)
            // =========================================================================
            else
            {
                // Quét toàn bộ server xem có phòng nào chứa Username của người này không
                var entry = _activeGames.FirstOrDefault(x =>
                    (x.Value.PlayerWhite != null && x.Value.PlayerWhite.Username == client.Username) ||
                    (x.Value.PlayerBlack != null && x.Value.PlayerBlack.Username == client.Username)
                );

                if (entry.Value != null) // Tìm thấy phòng!
                {
                    session = entry.Value;
                    Console.WriteLine($"[AUTO-FIX] Phát hiện lệch kết nối của {client.Username}. Đang đồng bộ lại...");

                    // 1. Cập nhật lại Dictionary: Xóa key cũ (ảo), Thêm key mới (thật)
                    // Để các nước đi sau này nó sẽ tự động rơi vào CASE 1 (cho nhanh)
                    GameSession temp;
                    _activeGames.TryRemove(entry.Key, out temp);
                    _activeGames.TryAdd(client, session);

                    // 2. Cập nhật lại Socket trong Session: Để Server biết đường gửi tin nhắn về
                    if (session.PlayerWhite.Username == client.Username)
                    {
                        session.PlayerWhite = client;
                    }
                    else if (session.PlayerBlack.Username == client.Username)
                    {
                        session.PlayerBlack = client;
                    }

                    // 3. [QUAN TRỌNG] GỬI GÓI RESYNC
                    // Vì bị lệch kết nối nên Client có thể đã lỡ mất gói tin UPDATE trước đó.
                    // Gửi lại ngay bàn cờ hiện tại để Client vẽ lại và mở khóa nước đi.
                    string boardStr = Serialization.BoardToString(session.GameState.Board);
                    string turnStr = session.GameState.CurrentPlayer.ToString().ToUpper();

                    await client.SendMessageAsync($"UPDATE|{boardStr}|{turnStr}|600|600");
                    Console.WriteLine($"[RESYNC] Đã gửi lại bàn cờ mới nhất cho {client.Username}");
                }
            }

            // =========================================================================
            // PHẦN XỬ LÝ LỆNH (GIỮ NGUYÊN CODE CŨ CỦA BẠN)
            // =========================================================================
            if (session != null)
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
            else
            {
                // Chỉ khi nào quét cả server mà vẫn không thấy tên thì mới báo lỗi này
                Console.WriteLine($"[CRITICAL] User {client.Username} gửi lệnh nhưng không tìm thấy phòng chơi nào!");
            }
        }
        // Trong GameManager.cs

        // ... (Giữ nguyên các code khác)

        // === THÊM HÀM NÀY VÀO CUỐI CLASS GameManager (Phải là public static) ===
        // Trong GameManager.cs (Thêm vào cuối class nếu chưa có)
        public static async Task SendMessageToUser(string username, string message)
        {
            if (_onlineClients.TryGetValue(username, out ConnectedClient client))
            {
                if (client != null && client.Client != null && client.Client.Connected)
                {
                    try { await client.SendMessageAsync(message); } catch { }
                }
            }
        }

    }
}
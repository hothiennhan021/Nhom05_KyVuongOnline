using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MyTcpServer
{
    // Class hỗ trợ lưu trạng thái chờ xác nhận
    public class PendingMatch
    {
        public string MatchId { get; set; }
        public ConnectedClient Player1 { get; set; }
        public ConnectedClient Player2 { get; set; }
        public bool P1Accepted { get; set; } = false;
        public bool P2Accepted { get; set; } = false;

        public PendingMatch(ConnectedClient p1, ConnectedClient p2)
        {
            MatchId = Guid.NewGuid().ToString().Substring(0, 8); // ID ngắn gọn
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

        // [MỚI]: Danh sách các trận đang chờ bấm Accept
        private static readonly ConcurrentDictionary<string, PendingMatch> _pendingMatches = new ConcurrentDictionary<string, PendingMatch>();

        public static void HandleClientConnect(ConnectedClient client) { }

        public static void HandleClientDisconnect(ConnectedClient client)
        {
            lock (_lobbyLock) { _waitingLobby.Remove(client); }

            var roomKey = _privateRooms.FirstOrDefault(x => x.Value == client).Key;
            if (roomKey != null) _privateRooms.TryRemove(roomKey, out _);

            // Xử lý ngắt kết nối khi đang chờ Accept
            var pending = _pendingMatches.Values.FirstOrDefault(m => m.Player1 == client || m.Player2 == client);
            if (pending != null)
            {
                _ = CancelPendingMatch(pending, "Đối thủ đã thoát.");
            }

            if (_activeGames.TryRemove(client, out GameSession session))
            {
                if (!session.IsGameOver())
                {
                    var other = (session.PlayerWhite == client) ? session.PlayerBlack : session.PlayerWhite;
                    _ = other.SendMessageAsync("GAME_OVER_FULL|Đối thủ thoát|Resigned");
                    _activeGames.TryRemove(other, out _);
                }
            }
        }

        // --- TÌM TRẬN NGẪU NHIÊN ---
        public static async Task FindGame(ConnectedClient client)
        {
            PendingMatch newMatch = null;

            lock (_lobbyLock)
            {
                _waitingLobby.RemoveAll(c => !c.Client.Connected);

                if (!_waitingLobby.Contains(client))
                    _waitingLobby.Add(client);

                // Nếu đủ 2 người => Tạo Pending Match (chưa Start Game ngay)
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
                // Gửi thông báo MATCH_FOUND để client hiện popup
                await newMatch.Player1.SendMessageAsync($"MATCH_FOUND|{newMatch.MatchId}");
                await newMatch.Player2.SendMessageAsync($"MATCH_FOUND|{newMatch.MatchId}");
            }
            else
            {
                await client.SendMessageAsync("WAITING");
            }
        }

        // --- XỬ LÝ CHẤP NHẬN / TỪ CHỐI ---
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
                    if (client == match.Player1) match.P1Accepted = true;
                    if (client == match.Player2) match.P2Accepted = true;

                    // Nếu cả 2 đều đồng ý => BẮT ĐẦU GAME
                    if (match.P1Accepted && match.P2Accepted)
                    {
                        // Xóa khỏi danh sách chờ
                        _pendingMatches.TryRemove(matchId, out _);

                        // Tạo session game thật
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

            // Gửi tin nhắn hủy trận cho cả 2
            await match.Player1.SendMessageAsync($"MATCH_CANCELLED|{reason}");
            await match.Player2.SendMessageAsync($"MATCH_CANCELLED|{reason}");
        }

        // --- TẠO PHÒNG RIÊNG (GIỮ NGUYÊN HOẶC UPDATE TÙY BẠN) ---
        // Hiện tại Tạo phòng riêng sẽ vào game luôn, nếu muốn popup thì sửa giống FindGame
        public static async Task CreateRoom(ConnectedClient creator)
        {
            string id = new Random().Next(1000, 9999).ToString();
            while (_privateRooms.ContainsKey(id)) id = new Random().Next(1000, 9999).ToString();

            if (_privateRooms.TryAdd(id, creator))
            {
                await creator.SendMessageAsync($"ROOM_CREATED|{id}");
            }
            else
            {
                await creator.SendMessageAsync("ROOM_ERROR|Không thể tạo phòng.");
            }
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

                // Với phòng riêng, ta có thể cho vào game luôn (bỏ qua bước Accept)
                // Hoặc nếu muốn Accept thì làm tương tự FindGame
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
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // [MỚI]: Cần để sử dụng FirstOrDefault cho logic xóa phòng

namespace MyTcpServer
{
    public static class GameManager
    {
        private static readonly List<ConnectedClient> _waitingLobby = new List<ConnectedClient>();
        private static readonly object _lobbyLock = new object();
        private static readonly ConcurrentDictionary<ConnectedClient, GameSession> _activeGames = new ConcurrentDictionary<ConnectedClient, GameSession>();

        // [MỚI]: Từ điển để quản lý các phòng riêng
        private static readonly ConcurrentDictionary<string, ConnectedClient> _privateRooms = new ConcurrentDictionary<string, ConnectedClient>();

        public static void HandleClientConnect(ConnectedClient client) { }

        public static void HandleClientDisconnect(ConnectedClient client)
        {
            // Xóa khỏi hàng đợi nếu đang chờ (Logic gốc)
            lock (_lobbyLock) { _waitingLobby.Remove(client); }

            // [MỚI]: Xóa phòng nếu chủ phòng thoát
            var roomKey = _privateRooms.FirstOrDefault(x => x.Value == client).Key;
            if (roomKey != null) _privateRooms.TryRemove(roomKey, out _);

            // Xử lý nếu đang trong game (Logic gốc)
            if (_activeGames.TryRemove(client, out GameSession session))
            {
                if (!session.IsGameOver())
                {
                    // Tìm người còn lại
                    var other = (session.PlayerWhite == client) ? session.PlayerBlack : session.PlayerWhite;

                    // Gửi tin nhắn thắng cuộc cho người còn lại
                    _ = other.SendMessageAsync("GAME_OVER_FULL|Đối thủ thoát|Resigned");

                    // Xóa người còn lại khỏi danh sách active luôn
                    _activeGames.TryRemove(other, out _);
                }
            }
        }

        // [MỚI]: TÍNH NĂNG TẠO PHÒNG (PUBLIC STATIC)
        public static async Task CreateRoom(ConnectedClient creator)
        {
            // Tạo ID ngẫu nhiên (4 chữ số)
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

        // [MỚI]: TÍNH NĂNG VÀO PHÒNG (PUBLIC STATIC)
        public static async Task JoinRoom(ConnectedClient joiner, string id)
        {
            if (_privateRooms.TryRemove(id, out ConnectedClient creator))
            {
                if (!creator.Client.Connected)
                {
                    await joiner.SendMessageAsync("ROOM_ERROR|Phòng đã hủy.");
                    return;
                }

                // Tạo Game Session
                var session = new GameSession(creator, joiner);

                // [FIX LỖI QUAN TRỌNG]: Đăng ký người chơi vào _activeGames
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
            // Logic gốc giữ nguyên
            if (_activeGames.TryGetValue(client, out GameSession session))
            {
                string[] parts = command.Split('|');
                string cmd = parts[0];

                if (cmd == "MOVE")
                    await session.HandleMove(client, command);
                else if (cmd == "CHAT")
                    await session.BroadcastChat(client, parts[1]);
                else if (cmd == "REQUEST_ANALYSIS")
                {
                    await session.HandleAnalysisRequest(client);
                }
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

        private static async Task AddToLobby(ConnectedClient client)
        {
            GameSession sessionToStart = null;

            // BƯỚC 1: Thao tác với hàng đợi (Cần Lock)
            lock (_lobbyLock)
            {
                _waitingLobby.RemoveAll(c => !c.Client.Connected); // Dọn dẹp kết nối chết

                if (!_waitingLobby.Contains(client))
                    _waitingLobby.Add(client);

                // Nếu đủ 2 người thì ghép cặp
                if (_waitingLobby.Count >= 2)
                {
                    var p1 = _waitingLobby[0];
                    var p2 = _waitingLobby[1];
                    _waitingLobby.RemoveRange(0, 2); // Xóa 2 người khỏi hàng đợi

                    // Tạo session mới
                    sessionToStart = new GameSession(p1, p2);
                    _activeGames[p1] = sessionToStart;
                    _activeGames[p2] = sessionToStart;
                }
            }
            // KẾT THÚC LOCK TẠI ĐÂY

            // BƯỚC 2: Gửi tin nhắn mạng (Async - Không được nằm trong Lock)
            if (sessionToStart != null)
            {
                await sessionToStart.StartGame();
            }
            else
            {
                await client.SendMessageAsync("WAITING");
            }
        }
    }
}
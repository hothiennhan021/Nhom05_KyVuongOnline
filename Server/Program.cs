using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using ChessData;

namespace MyTcpServer
{
    class Program
    {
        private static IConfiguration _config;
        private static UserRepository _userRepo;
        private static FriendRepository _friendRepo;

        static async Task Main(string[] args)
        {
            // Load config
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


        static async Task Main(string[] args)
        {
            // 1. Load cấu hình (Logic gốc giữ nguyên)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();

            // 2. Kết nối DB (Logic gốc giữ nguyên)
            string connString = _config.GetConnectionString("DefaultConnection");

            // Khởi tạo Repository
            try
            {
                _userRepo = new UserRepository(connString);
                _friendRepo = new FriendRepository(connString);

                Console.WriteLine("Database connected successfully.");
                Console.WriteLine("Database: OK.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return;
            }

            // Start TCP Server
            TcpListener server = new TcpListener(IPAddress.Any, 8888);
            server.Start();
            Console.WriteLine("Server started on port 8888...");
            // 3. Mở Server (Logic gốc giữ nguyên)
            int port = 8888;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine($"Server started on port {port}...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        // ==============================
        // XỬ LÝ CLIENT
        // ==============================
        static async Task HandleClientAsync(TcpClient tcp)
        {
            ConnectedClient client = new ConnectedClient(tcp);

        static async Task HandleClientAsync(TcpClient client)
        {
            ConnectedClient connectedClient = new ConnectedClient(client);
            try
            {
                GameManager.HandleClientConnect(connectedClient);

                while (true)
                {
                    string requestMessage = await connectedClient.Reader.ReadLineAsync();
                    if (requestMessage == null) break;

                    string response = await ProcessRequest(client, msg);
                    Console.WriteLine($"[RECV] {requestMessage}");

                    string response = await ProcessRequest(connectedClient, requestMessage);
                    if (response != null)
                    {
                        await connectedClient.SendMessageAsync(response);
                        Console.WriteLine($"[SENT] {response}");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Client Error: {ex.Message}"); }
            finally
            {
                GameManager.HandleClientDisconnect(connectedClient);
                try { connectedClient.Client.Close(); } catch { }
            }
        }

        // ==============================
        // ROUTER XỬ LÝ REQUEST
        // ==============================
        static async Task<string> ProcessRequest(ConnectedClient client, string msg)
        static async Task<string> ProcessRequest(ConnectedClient client, string requestMessage)
        {
            string[] parts = requestMessage.Split('|');
            string command = parts[0];

            switch (command)
            {
                // ===========================
                // AUTH
                // ===========================
                case "REGISTER":
                    if (parts.Length != 6)
                        return "ERROR|Format REGISTER sai";

                    return await _userRepo.RegisterUserAsync(
                        parts[1], parts[2], parts[3], parts[4], parts[5]);

                case "LOGIN":
                    if (parts.Length != 3)
                        return "ERROR|Format LOGIN sai";

                    string loginResult = await _userRepo.LoginUserAsync(parts[1], parts[2]);
                    if (loginResult.StartsWith("LOGIN_SUCCESS"))
                    {
                        client.UserId = GetUserId(parts[1]);
                        client.Username = parts[1];
                    }
                    return loginResult;

                // ===========================
                // MATCHMAKING & ROOM
                // ===========================
                    if (parts.Length == 6)
                        return await _userRepo.RegisterUserAsync(parts[1], parts[2], parts[3], parts[4], parts[5]);
                    return "ERROR|Format REGISTER sai.";

                case "LOGIN":
                    if (parts.Length == 3)
                    {
                        string result = await _userRepo.LoginUserAsync(parts[1], parts[2]);
                        if (result.StartsWith("LOGIN_SUCCESS"))
                        {
                            int uid = GetUserIdByUsername(parts[1]);
                            if (uid > 0) client.UserId = uid;
                        }
                        return result;
                    }
                    return "ERROR|Format LOGIN sai.";

                // --- [MỚI] XỬ LÝ LỆNH PHÒNG ---
                case "CREATE_ROOM":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    await GameManager.CreateRoom(client);
                    return null;

                case "JOIN_ROOM":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    if (parts.Length > 1)
                    {
                        await GameManager.JoinRoom(client, parts[1]);
                    }
                    else
                    {
                        return "ERROR|Format JOIN_ROOM sai.";
                    }
                    return null;
                // ------------------------------

                case "FIND_GAME":
                case "MOVE":
                case "CHAT":
                case "REQUEST_RESTART":
                case "RESTART_NO":
                case "LEAVE_GAME":
                case "REQUEST_ANALYSIS":
                    await GameManager.ProcessGameCommand(client, requestMessage);
                    return null;

                case "CREATE_ROOM":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    await GameManager.CreateRoom(client);
                    return null;

                case "JOIN_ROOM":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    if (parts.Length < 2) return "ERROR|Thiếu ID phòng";
                    await GameManager.JoinRoom(client, parts[1]);
                    return null;

                // ===========================
                // FRIEND SYSTEM
                // ===========================
                case "FRIEND_SEARCH":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    return $"FRIEND_RESULT|{_friendRepo.SendFriendRequest(client.UserId, parts[1])}";

                case "FRIEND_GET_LIST":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    var list = _friendRepo.GetListFriends(client.UserId);
                    return "FRIEND_LIST|" + string.Join(";", list);
                    return $"FRIEND_LIST|{string.Join(";", list)}";

                case "FRIEND_GET_REQUESTS":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    var reqs = _friendRepo.GetFriendRequests(client.UserId);
                    return "FRIEND_REQUESTS|" + string.Join(";", reqs);

                case "FRIEND_ACCEPT":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    if (!int.TryParse(parts[1], out int reqId))
                        return "ERROR|Sai ID request!";
                    _friendRepo.AcceptFriend(reqId);
                    return "FRIEND_ACCEPT_OK";

                case "FRIEND_REMOVE":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    bool ok = _friendRepo.RemoveFriend(client.UserId, parts[1]);
                    return ok ? "SUCCESS|Đã xóa bạn." : "ERROR|Không thể xóa bạn.";

                // ===========================
                // UNKNOWN COMMAND
                // ===========================
                    return $"FRIEND_REQUESTS|{string.Join(";", reqs)}";

                case "FRIEND_ACCEPT":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    if (int.TryParse(parts[1], out int reqId))
                    {
                        _friendRepo.AcceptFriend(reqId);
                        return "FRIEND_ACCEPT_OK";
                    }
                    return "ERROR|Sai ID lời mời";

                case "FRIEND_REMOVE":
                    if (client.UserId == 0) return "ERROR|Bạn chưa đăng nhập!";
                    string friendName = parts[1];
                    bool isDeleted = _friendRepo.RemoveFriend(client.UserId, friendName);
                    if (isDeleted) return "SUCCESS|Đã xóa bạn thành công.";
                    else return "ERROR|Lỗi khi xóa bạn (hoặc chưa kết bạn).";

                // --- MẶC ĐỊNH ---
                default:
                    return "ERROR|Lệnh không xác định.";
            }
        }

        // ==============================
        // Lấy UserId theo Username
        // ==============================
        private static int GetUserId(string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    var cmd = new SqlCommand(
                        "SELECT UserId FROM Users WHERE Username=@u", conn);
                    cmd.Parameters.AddWithValue("@u", username);

                    object r = cmd.ExecuteScalar();
                    return r != null ? (int)r : 0;
        private static int GetUserIdByUsername(string username)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT UserId FROM Users WHERE Username = @u", conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    object res = cmd.ExecuteScalar();
                    return res != null ? (int)res : 0;
                }
            }
            catch
            {
                Console.WriteLine("[Lỗi lấy ID]: " + ex.Message);
                return 0;
            }
        }
    }
}
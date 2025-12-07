using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ChessData;
using Microsoft.Data.SqlClient;

namespace MyTcpServer
{
    class Program
    {
        private static IConfiguration _config;
        private static FriendRepository _friendRepo;
        private static UserRepository _userRepo;
        private static MatchRepository _matchRepo;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _config = builder.Build();

            string connString = _config.GetConnectionString("DefaultConnection");

            try
            {
                _userRepo = new UserRepository(connString);
                _friendRepo = new FriendRepository(connString);
                _matchRepo = new MatchRepository(connString);

                Console.WriteLine("Database OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error: " + ex.Message);
                return;
            }

            TcpListener server = new TcpListener(IPAddress.Any, 8888);
            server.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _ = HandleClient(client);
            }
        }

        static async Task HandleClient(TcpClient tcp)
        {
            var client = new ConnectedClient(tcp);

            try
            {
                GameManager.HandleClientConnect(client);

                while (true)
                {
                    string msg = await client.Reader.ReadLineAsync();
                    if (msg == null) break;

                    Console.WriteLine("[RECV] " + msg);

                    string response = await ProcessRequest(client, msg);

                    if (response != null)
                    {
                        await client.SendMessageAsync(response);
                        Console.WriteLine("[SEND] " + response);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client Error: " + ex.Message);
            }
            finally
            {
                if (client.UserId != 0)
                {
                    _userRepo.SetOnline(client.UserId, false);
                }

                GameManager.HandleClientDisconnect(client);
                client.Close();
            }
        }

        // ======================================================
        //                  PROCESS REQUEST
        // ======================================================
        static async Task<string> ProcessRequest(ConnectedClient client, string msg)
        {
            string[] parts = msg.Split('|');
            string cmd = parts[0];

            switch (cmd)
            {
                // ======================
                //  OTP + REGISTER & LOGIN
                // ======================

                case "REQUEST_OTP":
                    {
                        if (parts.Length < 2)
                            return "ERROR|Thiếu email.";

                        string email = parts[1];

                        try
                        {
                            // Kiểm tra email đã tồn tại chưa
                            if (await _userRepo.EmailExistsAsync(email))
                            {
                                return "ERROR|Email đã tồn tại trong hệ thống.";
                            }

                            // Tạo OTP 6 chữ số
                            var rnd = new Random();
                            string otp = rnd.Next(100000, 999999).ToString();

                            client.TempOtp = otp;
                            client.PendingEmail = email;
                            client.OtpExpire = DateTime.UtcNow.AddMinutes(5);
                            client.IsOtpVerified = false;

                            bool sent = await EmailService.SendOtpAsync(email, otp);
                            if (!sent)
                            {
                                return "ERROR|Gửi OTP thất bại. Vui lòng thử lại.";
                            }

                            return "OTP_SENT|Mã OTP đã được gửi đến email của bạn.";
                        }
                        catch (Exception ex)
                        {
                            return "ERROR|" + ex.Message;
                        }
                    }

                case "VERIFY_OTP":
                    {
                        if (parts.Length < 3)
                            return "ERROR|Thiếu email hoặc mã OTP.";

                        string email = parts[1];
                        string otp = parts[2];

                        if (client.PendingEmail == null || client.TempOtp == null)
                        {
                            return "ERROR|Bạn chưa yêu cầu mã OTP.";
                        }

                        if (!string.Equals(client.PendingEmail, email, StringComparison.OrdinalIgnoreCase))
                        {
                            return "ERROR|Email không khớp với email đã gửi OTP.";
                        }

                        if (client.OtpExpire != default && client.OtpExpire < DateTime.UtcNow)
                        {
                            client.TempOtp = null;
                            client.PendingEmail = null;
                            client.IsOtpVerified = false;
                            return "ERROR|Mã OTP đã hết hạn. Vui lòng yêu cầu lại.";
                        }

                        if (!string.Equals(client.TempOtp, otp, StringComparison.Ordinal))
                        {
                            return "ERROR|Mã OTP không chính xác.";
                        }

                        client.IsOtpVerified = true;
                        return "OTP_OK|Xác minh OTP thành công.";
                    }

                case "REGISTER":
                    {
                        if (parts.Length < 6)
                            return "ERROR|Thiếu tham số đăng ký.";

                        string username = parts[1];
                        string password = parts[2];
                        string email = parts[3];
                        string fullName = parts[4];
                        string birthday = parts[5];

                        // Bắt buộc phải xác minh OTP trước khi đăng ký
                        if (!client.IsOtpVerified ||
                            client.PendingEmail == null ||
                            !string.Equals(client.PendingEmail, email, StringComparison.OrdinalIgnoreCase) ||
                            (client.OtpExpire != default && client.OtpExpire < DateTime.UtcNow))
                        {
                            return "ERROR|Vui lòng xác minh OTP hợp lệ trước khi đăng ký.";
                        }

                        string res = await _userRepo.RegisterUserAsync(username, password, email, fullName, birthday);

                        // Nếu đăng ký thành công thì xoá OTP
                        if (res.StartsWith("REGISTER_SUCCESS"))
                        {
                            client.TempOtp = null;
                            client.PendingEmail = null;
                            client.IsOtpVerified = false;
                        }

                        return res;
                    }

                case "LOGIN":
                    {
                        string res = await _userRepo.LoginUserAsync(parts[1], parts[2]);
                        if (res.StartsWith("LOGIN_SUCCESS"))
                        {
                            client.UserId = GetUserId(parts[1]);
                            client.Username = parts[1];
                            _userRepo.SetOnline(client.UserId, true);
                        }
                        return res;
                    }

                case "LOGOUT":
                    _userRepo.SetOnline(client.UserId, false);
                    return "LOGOUT_OK";

                // ======================================================
                //                       ⭐ PROFILE / AVATAR / INGAME ⭐
                // ======================================================

                case "GET_PROFILE":
                    {
                        string username = parts[1];
                        var stats = await _userRepo.GetUserStatsAsync(username);

                        if (stats == null)
                            return "ERROR|NO_PROFILE";

                        // PROFILE|ingame|rank|highest|wins|losses|minutes
                        return $"PROFILE|{stats.IngameName}|{stats.Rank}|{stats.HighestRank}|{stats.Wins}|{stats.Losses}|{stats.TotalPlayTimeMinutes}";
                    }

                case "GET_AVATAR":
                    {
                        string username = parts[1];
                        var avatar = await _userRepo.GetAvatarAsync(username);

                        if (avatar == null)
                            return "AVATAR_NULL";

                        return "AVATAR|" + Convert.ToBase64String(avatar);
                    }

                case "SET_AVATAR":
                    {
                        string username = parts[1];
                        byte[] bytes = Convert.FromBase64String(parts[2]);
                        await _userRepo.UpdateAvatarAsync(username, bytes);
                        return "SET_AVATAR_OK";
                    }

                // Đổi tên In-Game
                case "SET_INGAME":
                    {
                        string username = parts[1];
                        string newName = parts[2];

                        bool ok = await _userRepo.UpdateIngameNameAsync(username, newName);
                        return ok ? "SET_INGAME_OK" : "SET_INGAME_FAIL";
                    }

                // ======================================================
                //                   FRIEND SYSTEM (GIỮ LOGIC CŨ)
                // ======================================================

                // LỆNH CŨ CỦA CLIENT: FRIEND_SEARCH|username
                // → Mặc định: Gửi lời mời kết bạn
                case "FRIEND_SEARCH":
                    {
                        return _friendRepo.SendFriendRequest(client.UserId, parts[1]);
                    }

                // LỆNH MỚI: FRIEND_SEND|username (nếu phía client có dùng)
                case "FRIEND_SEND":
                    {
                        return _friendRepo.SendFriendRequest(client.UserId, parts[1]);
                    }

                // GET FRIEND LIST
                case "FRIEND_LIST":
                case "FRIEND_GET_LIST":
                    {
                        var list = _friendRepo.GetListFriends(client.UserId);
                        return "FRIEND_LIST|" + string.Join(";", list);
                    }

                // GET REQUESTS
                case "FRIEND_REQUESTS":
                case "FRIEND_GET_REQUESTS":
                    {
                        var req = _friendRepo.GetFriendRequests(client.UserId);
                        return "FRIEND_REQUESTS|" + string.Join(";", req);
                    }

                // ACCEPT FRIEND
                case "FRIEND_ACCEPT":
                    {
                        _friendRepo.AcceptFriend(int.Parse(parts[1]));
                        // Giữ nguyên như bản cũ: client đang check chứa "FRIEND_ACCEPTED"
                        return "FRIEND_ACCEPTED";
                    }

                // REMOVE FRIEND
                case "FRIEND_REMOVE":
                    {
                        bool ok = _friendRepo.RemoveFriend(client.UserId, parts[1]);
                        return ok ? "FRIEND_REMOVED" : "FRIEND_REMOVE_FAIL";
                    }

                // ======================================================
                //                MATCHMAKING (TÌM TRẬN)
                // ======================================================

                case "FIND_GAME":
                    await GameManager.FindGame(client);
                    return null;

                case "MATCH_RESPONSE":
                    await GameManager.HandleMatchResponse(client, parts[1], parts[2]);
                    return null;

                // ======================================================
                //                    PRIVATE ROOM
                // ======================================================

                case "CREATE_ROOM":
                    await GameManager.CreateRoom(client);
                    return null;

                case "JOIN_ROOM":
                    await GameManager.JoinRoom(client, parts[1]);
                    return null;

                // ======================================================
                //                    GAME COMMANDS
                // ======================================================

                case "MOVE":
                case "CHAT":
                case "REQUEST_RESTART":
                case "RESTART_NO":
                case "LEAVE_GAME":
                case "REQUEST_ANALYSIS":
                    await GameManager.ProcessGameCommand(client, msg);
                    return null;

                default:
                    return "ERROR|Unknown command";
            }
        }

        // ======================================================
        //          UPDATE MATCH RESULT (WIN/LOSE)  (GIỮ CODE CŨ)
        // ======================================================

        public static async Task UpdateMatchAsync(string winner, string loser, int minutes)
        {
            try
            {
                await _matchRepo.UpdateMatchResult(winner, loser, minutes);
                Console.WriteLine($"Match updated: {winner} thắng {loser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UpdateMatch Error] " + ex.Message);
            }
        }

        private static int GetUserId(string username)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();

            var cmd = new SqlCommand("SELECT UserId FROM Users WHERE Username=@u", conn);
            cmd.Parameters.AddWithValue("@u", username);

            var r = cmd.ExecuteScalar();
            return (r != null) ? (int)r : 0;
        }
    }
}

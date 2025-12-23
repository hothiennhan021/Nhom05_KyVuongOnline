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
        static Dictionary<string, (string otp, DateTime expire)> forgotOtps
    = new Dictionary<string, (string, DateTime)>();

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _config = builder.Build();

            string connString = _config.GetConnectionString("DefaultConnection");
            Console.WriteLine("[DB] DefaultConnection = " + connString);
            GameManager.Init(connString);

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
                case "FORGOT_SEND_OTP":
                    {
                        if (parts.Length < 2)
                            return "ERROR|Thiếu email";

                        string email = parts[1];

                        bool exists = await _userRepo.EmailExistsAsync(email);
                        if (!exists)
                            return "ERROR|Email chưa đăng ký tài khoản";

                        string otp = new Random().Next(100000, 999999).ToString();
                        forgotOtps[email] = (otp, DateTime.UtcNow.AddMinutes(5));

                        bool sent = await EmailService.SendForgotPasswordOtpAsync(email, otp);
                        if (!sent)
                            return "ERROR|Gửi OTP thất bại";

                        return "OK";   // ✅ BẮT BUỘC
                    }


                case "FORGOT_VERIFY_OTP":
                    {
                        string email = parts[1];
                        string otp = parts[2];

                        if (!forgotOtps.ContainsKey(email))
                            return "ERROR|OTP không tồn tại";

                        var info = forgotOtps[email];

                        if (DateTime.UtcNow > info.expire)
                        {
                            forgotOtps.Remove(email);
                            return "ERROR|OTP hết hạn";
                        }

                        return otp == info.otp ? "OK" : "ERROR|OTP sai";
                    }
                case "FORGOT_RESET_PASSWORD":
                    {
                        string email = parts[1];
                        string newHash = parts[2];

                        try
                        {
                            using var conn = new SqlConnection(
                                _config.GetConnectionString("DefaultConnection"));
                            conn.Open();

                            var sqlCmd = new SqlCommand(
                                "UPDATE Users SET PasswordHash=@p WHERE Email=@e", conn);

                            sqlCmd.Parameters.AddWithValue("@p", newHash);
                            sqlCmd.Parameters.AddWithValue("@e", email);

                            int rows = sqlCmd.ExecuteNonQuery();
                            forgotOtps.Remove(email);

                            return rows > 0 ? "OK" : "ERROR|Email không tồn tại";
                        }
                        catch (Exception ex)
                        {
                            return "ERROR|" + ex.Message;
                        }
                    }



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
                        return $"PROFILE|{stats.IngameName}|{stats.Elo}|{stats.HighestRank}|{stats.Wins}|{stats.Losses}|{stats.TotalPlayTimeMinutes}";
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
                //                   FRIEND SYSTEM 
                // ======================================================


                case "FRIEND_SEARCH":
                    {
                        if (client.UserId <= 0) return "FRIEND_SEARCH_NOT_LOGGED_IN";

                        string result = _friendRepo.SendFriendRequest(client.UserId, parts[1]);

                        return "FRIEND_SEARCH_" + result;
                    }


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
                        return "FRIEND_ACCEPTED";
                    }

                // REMOVE FRIEND
                case "FRIEND_REMOVE":
                    {
                        bool ok = _friendRepo.RemoveFriend(client.UserId, parts[1]);
                        return ok ? "FRIEND_REMOVED" : "FRIEND_REMOVE_FAIL";
                    }
                // ======================================================
                //                    LEADERBOARD
                // ======================================================
                case "LEADERBOARD_GET":
                    {
                        // LEADERBOARD_GET|top
                        int top = 20;
                        if (parts.Length > 1)
                            int.TryParse(parts[1], out top);

                        if (top <= 0) top = 20;
                        if (top > 100) top = 100;

                        var list = await _userRepo.GetLeaderboardAsync(top);

                        // Format:
                        // LEADERBOARD|username,elo,wins,losses;username,elo,wins,losses;...
                        string payload = string.Join(";",
                            list.Select(u => $"{u.Username},{u.Elo},{u.Wins},{u.Losses}")
                        );

                        return "LEADERBOARD|" + payload;
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
                case "RESIGN":          // Xin thua
                case "DRAW_OFFER":      // Xin hòa
                case "DRAW_ACCEPT":     // Đồng ý hòa
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

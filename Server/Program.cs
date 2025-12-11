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
                // =====================================================================
                //                       OTP + REGISTER + LOGIN
                // =====================================================================

                case "REQUEST_OTP":
                    {
                        if (parts.Length < 2)
                            return "ERROR|Thiếu email.";

                        string email = parts[1];

                        if (await _userRepo.EmailExistsAsync(email))
                            return "ERROR|Email đã tồn tại.";

                        // Tạo OTP
                        string otp = new Random().Next(100000, 999999).ToString();

                        client.TempOtp = otp;
                        client.PendingEmail = email;
                        client.OtpExpire = DateTime.UtcNow.AddMinutes(5);
                        client.IsOtpVerified = false;

                        bool ok = await EmailService.SendOtpAsync(email, otp);
                        return ok ? "OTP_SENT" : "ERROR|Gửi OTP thất bại.";
                    }

                case "VERIFY_OTP":
                    {
                        if (parts.Length < 3)
                            return "ERROR|Thiếu thông tin.";

                        if (client.PendingEmail == null || client.TempOtp == null)
                            return "ERROR|Chưa yêu cầu OTP.";

                        if (client.OtpExpire < DateTime.UtcNow)
                            return "ERROR|OTP hết hạn.";

                        if (parts[2] != client.TempOtp)
                            return "ERROR|Sai OTP.";

                        client.IsOtpVerified = true;
                        return "OTP_OK";
                    }

                case "REGISTER":
                    {
                        if (parts.Length < 6)
                            return "ERROR|Thiếu tham số.";

                        if (!client.IsOtpVerified)
                            return "ERROR|Chưa xác minh OTP.";

                        string res = await _userRepo.RegisterUserAsync(parts[1], parts[2], parts[3], parts[4], parts[5]);

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


                // =====================================================================
                //                       ⭐⭐  PROFILE SYSTEM  ⭐⭐
                // =====================================================================

                case "GET_PROFILE":
                    {
                        string username = parts[1];
                        var stats = await _userRepo.GetUserStatsAsync(username);

                        if (stats == null)
                            return "ERROR|NO_PROFILE";

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


                // =====================================================================
                //                       FRIEND SYSTEM (NGUYÊN CODE)
                // =====================================================================

                case "FRIEND_SEARCH":
                case "FRIEND_SEND":
                    return _friendRepo.SendFriendRequest(client.UserId, parts[1]);

                case "FRIEND_LIST":
                case "FRIEND_GET_LIST":
                    return "FRIEND_LIST|" + string.Join(";", _friendRepo.GetListFriends(client.UserId));

                case "FRIEND_REQUESTS":
                case "FRIEND_GET_REQUESTS":
                    return "FRIEND_REQUESTS|" + string.Join(";", _friendRepo.GetFriendRequests(client.UserId));

                case "FRIEND_ACCEPT":
                    _friendRepo.AcceptFriend(int.Parse(parts[1]));
                    return "FRIEND_ACCEPTED";

                case "FRIEND_REMOVE":
                    return _friendRepo.RemoveFriend(client.UserId, parts[1])
                        ? "FRIEND_REMOVED"
                        : "FRIEND_REMOVE_FAIL";


                // =====================================================================
                //                       MATCHMAKING (GIỮ NGUYÊN)
                // =====================================================================

                case "FIND_GAME":
                    await GameManager.FindGame(client);
                    return null;

                case "MATCH_RESPONSE":
                    await GameManager.HandleMatchResponse(client, parts[1], parts[2]);
                    return null;


                // =====================================================================
                //                       PRIVATE ROOM (GIỮ NGUYÊN)
                // =====================================================================

                case "CREATE_ROOM":
                    await GameManager.CreateRoom(client);
                    return null;

                case "JOIN_ROOM":
                    await GameManager.JoinRoom(client, parts[1]);
                    return null;


                // =====================================================================
                //                              GAME COMMANDS
                // =====================================================================

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

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
                // REGISTER & LOGIN
                // ======================
                case "REGISTER":
                    return await _userRepo.RegisterUserAsync(parts[1], parts[2], parts[3], parts[4], parts[5]);

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
                //                   FRIEND SYSTEM
                // ======================================================

                // LỆNH CŨ CỦA CLIENT: FRIEND_SEARCH|username
                // → Mặc định: Gửi lời mời kết bạn
                case "FRIEND_SEARCH":
                    {
                        return _friendRepo.SendFriendRequest(client.UserId, parts[1]);
                    }

                // LỆNH MỚI: FRIEND_SEND|username
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
        //          UPDATE MATCH RESULT (WIN/LOSE)
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

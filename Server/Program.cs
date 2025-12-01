using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ChessData;
using MyTcpServer;
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
            // 1. Load cấu hình (Logic gốc giữ nguyên)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();

            // 2. Kết nối DB (Logic gốc giữ nguyên)
            string connString = _config.GetConnectionString("DefaultConnection");
            try
            {
                _userRepo = new UserRepository(connString);
                _friendRepo = new FriendRepository(connString);
                Console.WriteLine("Database: OK.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return;
            }

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

        static async Task<string> ProcessRequest(ConnectedClient client, string requestMessage)
        {
            string[] parts = requestMessage.Split('|');
            string command = parts[0];

            switch (command)
            {
                case "REGISTER":
                    if (parts.Length == 6)
                        return await _userRepo.RegisterUserAsync(parts[1], parts[2], parts[3], parts[4], parts[5]);
                    return "ERROR|Format REGISTER sai.";

                case "LOGIN":
                    if (parts.Length == 3)
                    {
                        client.UserId = GetUserId(parts[1]);
                        client.Username = parts[1];  // <— QUAN TRỌNG
                    }
                    return res;

                case "CREATE_ROOM":
                    await GameManager.CreateRoom(client);
                    return null;

                case "JOIN_ROOM":
                    await GameManager.JoinRoom(client, parts[1]);
                    return null;

                case "MOVE":
                case "CHAT":
                case "REQUEST_RESTART":
                case "RESTART_NO":
                case "LEAVE_GAME":
                case "REQUEST_ANALYSIS":
                    await GameManager.ProcessGameCommand(client, msg);
                    return null;

                default:
                    return "ERROR|Lệnh không xác định.";
            }
        }

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
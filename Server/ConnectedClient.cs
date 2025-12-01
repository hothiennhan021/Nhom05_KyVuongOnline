using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyTcpServer
{
    public class ConnectedClient
    {
        public TcpClient Client { get; }
        public StreamReader Reader { get; }
        public StreamWriter Writer { get; }

        public int UserId { get; set; } = 0;
        public string Username { get; set; } = "";

        // [MỚI] Lưu Username để cập nhật Rank/Wins/Losses
        public string Username { get; set; } = "";

        public ConnectedClient(TcpClient client)
        {
            Client = client;
            var stream = client.GetStream();

            Reader = new StreamReader(stream, Encoding.UTF8);
            Writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = false };
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (Client.Connected)
                {
                    await Writer.WriteLineAsync(message);
                    await Writer.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send Error (User {UserId}): {ex.Message}");
            }
        }

        public void Close()
        {
            try { Reader.Close(); } catch { }
            try { Writer.Close(); } catch { }
            try { Client.Close(); } catch { }
        }
    }
}

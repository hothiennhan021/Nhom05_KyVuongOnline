using System.Threading.Tasks;

namespace ChessClient
{
    public static class ClientManager
    {
        private static NetworkClient _instance;

        // LƯU TÊN USER HIỆN TẠI SAU KHI LOGIN
        public static string Username { get; set; }

        public static NetworkClient Instance
        {
            get
            {
                // Nếu instance chưa có hoặc đã bị ngắt kết nối thì tạo mới
                if (_instance == null || !_instance.IsConnected)
                {
                    _instance = new NetworkClient();
                }
                return _instance;
            }
        }

        public static async Task ConnectToServerAsync(string ip, int port)
        {
            if (!Instance.IsConnected)
            {
                await Instance.ConnectAsync(ip, port);
            }
        }

        // RESET KẾT NỐI
        public static void Disconnect()
        {
            if (_instance != null)
            {
                _instance.CloseConnection();
                _instance = null;
            }

            // RESET username khi logout
            Username = null;
        }
    }
}

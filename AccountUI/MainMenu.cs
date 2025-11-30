using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessClient;

namespace AccountUI
{
    public partial class MainMenu : Form
    {
        private bool _isListening = false;

        public MainMenu() { InitializeComponent(); }
        private void MainMenu_Load(object sender, EventArgs e) { }

        // BUTTON EVENTS
        private async void button1_Click(object sender, EventArgs e) => await SendReq("FIND_GAME", "Đang tìm...", button1);
        private async void btnCreateRoom_Click(object sender, EventArgs e) => await SendReq("CREATE_ROOM", "Đang tạo...", btnCreateRoom);
        private async void btnJoinRoom_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRoomId.Text)) { MessageBox.Show("Nhập ID!"); return; }
            await SendReq($"JOIN_ROOM|{txtRoomId.Text}", "Đang vào...", btnJoinRoom);
        }

        private async Task SendReq(string cmd, string waitText, Button btn)
        {
            if (_isListening) return;
            if (!ClientManager.Instance.IsConnected) { MessageBox.Show("Mất kết nối!"); return; }

            try
            {
                _isListening = true;
                btn.Text = waitText;
                button1.Enabled = false; btnCreateRoom.Enabled = false; btnJoinRoom.Enabled = false;

                await ClientManager.Instance.SendAsync(cmd);
                await ListenLoop();
            }
            catch { ResetUI(); }
        }

        private async Task ListenLoop()
        {
            await Task.Run(() =>
            {
                while (_isListening)
                {
                    try
                    {
                        string msg = ClientManager.Instance.WaitForMessage();
                        if (string.IsNullOrEmpty(msg)) { this.Invoke((MethodInvoker)ResetUI); break; }

                        if (msg.StartsWith("GAME_START"))
                        {
                            _isListening = false;
                            LaunchGame(msg);
                            break;
                        }
                        else if (msg.StartsWith("ROOM_CREATED"))
                        {
                            string id = msg.Split('|')[1];
                            this.Invoke((MethodInvoker)(() => {
                                txtRoomId.Text = id;
                                labelRoom.Text = "Mã phòng: " + id;
                                btnCreateRoom.Text = "Đang chờ...";
                            }));
                        }
                        else if (msg.StartsWith("ERROR") || msg.StartsWith("ROOM_ERROR"))
                        {
                            this.Invoke((MethodInvoker)(() => { MessageBox.Show(msg); ResetUI(); }));
                            break;
                        }
                        else if (msg.StartsWith("WAITING"))
                        {
                            this.Invoke((MethodInvoker)(() => button1.Text = "Đang đợi..."));
                        }
                    }
                    catch { _isListening = false; }
                }
            });
        }

        private void LaunchGame(string msg)
        {
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                try
                {
                    ChessUI.MainWindow win = new ChessUI.MainWindow(msg);
                    win.Loaded += (s, e) => this.Invoke((MethodInvoker)(() => this.Hide()));
                    win.Closed += (s, e) =>
                    {
                        this.Invoke((MethodInvoker)(() => { this.Show(); ResetUI(); }));
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Background);
                    };
                    win.Show();
                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception ex) { this.Invoke((MethodInvoker)(() => { MessageBox.Show("Lỗi: " + ex.Message); ResetUI(); })); }
            });
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.IsBackground = false;
            t.Start();
        }

        private void ResetUI()
        {
            _isListening = false;
            button1.Enabled = true; btnCreateRoom.Enabled = true; btnJoinRoom.Enabled = true;
            button1.Text = "Ghép trận ngẫu nhiên";
            btnCreateRoom.Text = "Tạo phòng";
            btnJoinRoom.Text = "Vào phòng";
            labelRoom.Text = "";
        }

        private void button3_Click(object sender, EventArgs e) { }
        private void button4_Click(object sender, EventArgs e) { ClientManager.Disconnect(); Application.Exit(); }
        private void btnFriend_Click(object sender, EventArgs e) { new Friend().ShowDialog(); }
    }
}
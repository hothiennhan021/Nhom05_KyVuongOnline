using System;
using System.Linq;
using System.Windows.Forms;
using ChessClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace AccountUI
{
    public partial class Friend : Form
    {
        public Friend()
        {
            InitializeComponent();
        }

        // 1. Sự kiện khi Form vừa mở lên -> Tự động tải danh sách
        private async void FriendForm_Load(object sender, EventArgs e)
        {
            await LoadFriendListAsync();
            await LoadFriendRequestsAsync();
        }


        // 2. Hàm hỗ trợ: Tải danh sách bạn bè từ Server
        private async Task LoadFriendListAsync()
        {
            try
            {
                string response = await SendAndWaitAsync("FRIEND_GET_LIST", "FRIEND_LIST|");
                if (response == null) return;

                lbFriends.Items.Clear();

                int firstSplitIndex = response.IndexOf('|');
                if (firstSplitIndex == -1) return;

                string data = response.Substring(firstSplitIndex + 1)
                                      .Replace("\0", "")
                                      .Trim()
                                      .TrimEnd(';');

                if (string.IsNullOrWhiteSpace(data)) return;

                var tempList = new List<string>();
                foreach (var item in data.Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;
                    var info = item.Split('|');
                    if (info.Length >= 2)
                    {
                        string name = info[0];
                        string elo = info[1];
                        string status = (info.Length > 2) ? info[2].ToLower() : "false";
                        tempList.Add($"{name}|{elo}|{status}");
                    }
                }

                var sortedList = tempList
                    .OrderByDescending(x => x.Split('|')[2] == "true")
                    .ThenByDescending(x => int.Parse(x.Split('|')[1]))
                    .ToList();

                lbFriends.Items.Clear();
                foreach (var item in sortedList) lbFriends.Items.Add(item);
            }
            catch { }
        }


        // Hàm tải lời mời 
        private async Task LoadFriendRequestsAsync()
        {
            try
            {
                string response = await SendAndWaitAsync("FRIEND_GET_REQUESTS", "FRIEND_REQUESTS|");
                if (response == null) return;

                lbRequests.Items.Clear();

                int firstSplitIndex = response.IndexOf('|');
                if (firstSplitIndex == -1) return;

                string data = response.Substring(firstSplitIndex + 1)
                                      .Replace("\0", "")
                                      .Trim()
                                      .TrimEnd(';');

                if (string.IsNullOrWhiteSpace(data)) return;

                foreach (var r in data.Split(';'))
                    if (!string.IsNullOrWhiteSpace(r)) lbRequests.Items.Add(r);
            }
            catch { }
        }

        private async Task<string> SendAndWaitAsync(string cmd, string expectedPrefix, int timeoutMs = 3000)
        {
            await ClientManager.Instance.SendAsync(cmd);

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                string msg = ClientManager.Instance.WaitForMessage(200);
                if (msg == "TIMEOUT") continue;
                if (string.IsNullOrWhiteSpace(msg)) return null;

                msg = msg.Replace("\0", "").Trim();
                if (msg.StartsWith(expectedPrefix)) return msg;
            }
            return null;
        }



        // --- CÁC NÚT BẤM (BUTTON EVENTS) ---

        // Nút LÀM MỚI (Refresh)
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadFriendListAsync();
            LoadFriendRequestsAsync();
        }

        // Nút GỬI LỜI MỜI (Search & Add)
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string name = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập tên người chơi cần tìm!");
                return;
            }

            // Gửi lệnh tìm kiếm lên Server
            string response = ClientSocket.SendAndReceive($"FRIEND_SEARCH|{name}");

            // Xử lý các trường hợp Server trả về
            if (response.Contains("SUCCESS"))
            {
                MessageBox.Show($"Đã gửi lời mời kết bạn tới {name} thành công!");
                txtSearch.Clear(); // Xóa ô nhập cho sạch
            }
            else if (response.Contains("NOT_FOUND"))
            {
                MessageBox.Show("Không tìm thấy người chơi này.");
            }
            else if (response.Contains("SELF_ERROR"))
            {
                MessageBox.Show("Bạn không thể tự kết bạn với chính mình.");
            }
            else if (response.Contains("EXISTED"))
            {
                MessageBox.Show("Người này đã là bạn hoặc đã có lời mời đang chờ.");
            }
            else
            {
                MessageBox.Show("Lỗi từ Server: " + response);
            }
        }

        // Nút ĐỒNG Ý KẾT BẠN (Accept)
        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (lbRequests.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một lời mời trong danh sách!");
                return;
            }

            try
            {
                // Item trong ListBox có dạng "15|HungNguyen"
                string selected = lbRequests.SelectedItem.ToString();

                // Cắt lấy ID (phần trước dấu |)
                string reqId = selected.Split('|')[0];

                // Gửi lệnh đồng ý
                string response = ClientSocket.SendAndReceive($"FRIEND_ACCEPT|{reqId}");

                // Chỉ cần chứa từ khóa "OK" hoặc "SUCCESS" là được
                if (response.Contains("OK") || response.Contains("SUCCESS") || response.Contains("FRIEND_ACCEPTED") || response.Contains("FRIEND_REQUESTS"))
                {
                    MessageBox.Show("Đã chấp nhận kết bạn!");

                    // Tải lại danh sách ngay lập tức
                    LoadFriendListAsync();
                    LoadFriendRequestsAsync();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý: " + ex.Message);
            }
        }

        private void btnRefreshRequest_Click(object sender, EventArgs e)
        {
            LoadFriendRequestsAsync();
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            LoadFriendListAsync();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbFriends.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một người bạn để xóa!");
                return;
            }

            // Lấy dòng chữ đang chọn
            string selectedText = lbFriends.SelectedItem.ToString();

            // 1. Xóa bỏ các icon (nếu lỡ có) và khoảng trắng thừa
            string cleanText = selectedText.Replace("🟢", "").Replace("⚪", "").Trim();

            // 2. Cắt lấy Tên (Lấy phần trước dấu mở ngoặc "(" hoặc dấu gạch "|")
           
            string friendName = cleanText.Split(new char[] { '(', '|' })[0].Trim();

            // Hỏi xác nhận
            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa '{friendName}' khỏi danh sách?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                string response = ClientSocket.SendAndReceive($"FRIEND_REMOVE|{friendName}");

               
                if (response.Contains("SUCCESS") || response.Contains("OK") || response.Contains("REMOVED"))
                {
                    MessageBox.Show("Đã xóa thành công!");
                    LoadFriendListAsync(); // Tải lại danh sách
                }
                else
                {
                    MessageBox.Show("Lỗi: " + response);
                }
            }
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // 1. Lấy TabPage đang vẽ
            TabPage page = tabControl1.TabPages[e.Index];

            // 2. Tô màu nền cho Header (Màu xám đậm)
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(45, 45, 48)), e.Bounds);

            // 3. Xác định vùng vẽ chữ
            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);

            // 4. Chọn màu chữ (Vàng nếu đang chọn, Xám nhạt nếu không)
            Color textColor = (e.State == DrawItemState.Selected) ? Color.Gold : Color.LightGray;

            // 5. Vẽ chữ ra (Quan trọng nhất)
            TextRenderer.DrawText(e.Graphics, page.Text, Font, paddedBounds, textColor);
        }


        private void lbFriends_DrawItem(object sender, DrawItemEventArgs e)
        {
            // 1. Kiểm tra an toàn (Chống crash)
            if (e.Index < 0) return;

            ListBox lb = (ListBox)sender;
            string rawData = lb.Items[e.Index].ToString(); // Dữ liệu dạng: "Hung|1200|true"

            // Tách dữ liệu
            string[] parts = rawData.Split('|');
            string name = parts.Length > 0 ? parts[0] : "Unknown";
            string elo = parts.Length > 1 ? parts[1] : "0";
            bool isOnline = (parts.Length > 2 && parts[2] == "true");

            Graphics g = e.Graphics;

            // 2. Vẽ nền (Giữ nguyên)
            Brush bgBrush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                            ? new SolidBrush(Color.FromArgb(70, 70, 70))
                            : new SolidBrush(Color.FromArgb(45, 45, 48));
            g.FillRectangle(bgBrush, e.Bounds);

            // ---------------------------------------------------------
            // BƯỚC 3: VẼ AVATAR (Cố định bên trái)
            // ---------------------------------------------------------
            int avatarSize = 40;
            int avatarX = e.Bounds.X + 5;
            int avatarY = e.Bounds.Y + (e.Bounds.Height - avatarSize) / 2; // Căn giữa dọc

            if (imgListAvatar.Images.Count > 0)
            {
                int avatarIdx = name.Length % imgListAvatar.Images.Count;
                g.DrawImage(imgListAvatar.Images[avatarIdx], avatarX, avatarY, avatarSize, avatarSize);
            }
            else
            {
                g.FillEllipse(Brushes.Gray, avatarX, avatarY, avatarSize, avatarSize);
            }

            // ---------------------------------------------------------
            // BƯỚC 4: VẼ TÊN (Ngay sau Avatar)
            // ---------------------------------------------------------
            Font nameFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Brush textBrush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                              ? Brushes.Gold : Brushes.WhiteSmoke;

            int textX = avatarX + avatarSize + 12; // Cách avatar 12px
            int textY = e.Bounds.Y + 10;

            // Vẽ tên ra màn hình
            g.DrawString(name, nameFont, textBrush, textX, textY);

            // ---------------------------------------------------------
            // BƯỚC 5: VẼ ICON STATUS (Tự động dính theo sau Tên) - QUAN TRỌNG
            // ---------------------------------------------------------
            // Đo xem cái Tên chiếm bao nhiêu chiều rộng
            SizeF nameSize = g.MeasureString(name, nameFont);

            // Tính vị trí X của icon: Bằng vị trí Tên + Chiều rộng tên + 5px khoảng cách
            float iconX = textX + nameSize.Width + 5;

            // Căn icon cho nó ngang hàng với dòng chữ (chỉnh số +13 cho vừa mắt)
            float iconY = textY + 3;
            int iconSize = 14;

            // Chọn icon (0: Xám/Offline, 1: Xanh/Online)
            int statusIdx = isOnline ? 1 : 0;

            if (imgListStatus.Images.Count >= 2)
            {
                g.DrawImage(imgListStatus.Images[statusIdx], iconX, iconY, iconSize, iconSize);
            }

            // ---------------------------------------------------------
            // BƯỚC 6: VẼ ELO (Nằm bên dưới tên)
            // ---------------------------------------------------------
            Font eloFont = new Font("Segoe UI", 9, FontStyle.Italic);
            g.DrawString($"Elo: {elo}", eloFont, Brushes.Gray, textX, textY + 22);

            // Vẽ viền focus
            e.DrawFocusRectangle();
        }      
    }
}
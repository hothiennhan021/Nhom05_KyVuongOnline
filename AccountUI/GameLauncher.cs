using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace AccountUI
{
    public static class GameLauncher
    {
        public static void StartGame(Form parentForm, string gameStartMessage)
        {
            // Ẩn form cha
            if (parentForm.InvokeRequired) parentForm.Invoke((MethodInvoker)(() => parentForm.Hide()));
            else parentForm.Hide();

            Thread wpfThread = new Thread(() =>
            {
                try
                {
                    // QUAN TRỌNG: Truyền nguyên văn chuỗi vào MainWindow
                    // MainWindow của bạn đã có logic tự xử lý chuỗi này rồi (ServerResponseHandler)
                    ChessUI.MainWindow gameWindow = new ChessUI.MainWindow(gameStartMessage);

                    // Trong GameLauncher.cs

                    gameWindow.Closed += (s, e) =>
                    {
                        // Tắt luồng WPF
                        Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                        // [FIX CRASH] Kiểm tra kỹ trạng thái của Form cha trước khi gọi Invoke
                        if (parentForm != null && !parentForm.IsDisposed)
                        {
                            // Chỉ Invoke khi Handle đã được tạo (Khắc phục lỗi InvalidOperationException)
                            if (parentForm.IsHandleCreated)
                            {
                                parentForm.Invoke((MethodInvoker)delegate
                                {
                                    parentForm.Show();
                                    parentForm.BringToFront();
                                });
                            }
                            else
                            {
                                // Trường hợp hiếm: Nếu Handle chưa có, cứ thử Show bình thường (chạy trên UI thread gốc nếu may mắn)
                                parentForm.Show();
                            }
                        }
                    };

                    gameWindow.Show();
                    Dispatcher.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi mở game: " + ex.Message);
                    parentForm.Invoke((MethodInvoker)(() => parentForm.Show()));
                }
            });

            wpfThread.SetApartmentState(ApartmentState.STA);
            wpfThread.IsBackground = false;
            wpfThread.Start();
        }
    }
}
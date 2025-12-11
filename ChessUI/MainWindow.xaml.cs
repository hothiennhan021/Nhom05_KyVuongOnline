using ChessClient;
using ChessLogic;
using ChessUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChessUI
{
    public partial class MainWindow : Window
    {
        // --- TRẠNG THÁI GAME ---
        private bool _allowClose = false;
        private bool _isExiting = false;
        private bool _isGameOver = false;

        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position, List<Move>> moveCache = new Dictionary<Position, List<Move>>();

        private GameState _localGameState;
        private Position selectedPos = null;
        private Player _myColor;

        private NetworkClient _networkClient;
        private ServerResponseHandler _responseHandler;
        private ChessTimer _gameTimer;

        // --- XIN HÒA ---
        private DispatcherTimer _drawTimer;
        private int _drawCountDown = 15;
        private bool _hasOfferedDraw = false;

        public MainWindow(string gameStartMessage)
        {
            InitializeComponent();
            LoadBoardImageSafe();
            InitializedBoard();

            _networkClient = ClientManager.Instance;
            _responseHandler = new ServerResponseHandler();
            _gameTimer = new ChessTimer(10);

            // Timer Xin Hòa
            _drawTimer = new DispatcherTimer();
            _drawTimer.Interval = TimeSpan.FromSeconds(1);
            _drawTimer.Tick += DrawTimer_Tick;

            RegisterEvents();

            try
            {
                if (!_networkClient.IsConnected) throw new Exception("Mất kết nối.");
                _responseHandler.ProcessMessage(gameStartMessage);
                StartServerListener();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                Close();
            }
        }

        // ==========================================================
        // 1. EVENT HANDLER
        // ==========================================================
        private void RegisterEvents()
        {
            _responseHandler.GameStarted += (s, e) =>
            {
                _myColor = e.MyColor;
                _localGameState = new GameState(Player.White, e.Board);

                Dispatcher.Invoke(() =>
                {
                    MenuOverlay.Visibility = Visibility.Collapsed;
                    this.Title = $"Bạn là quân: {e.MyColor}";
                    _hasOfferedDraw = false;
                    btnDraw.IsEnabled = true;
                    btnDraw.Content = "Xin Hòa";
                });

                DrawBoard(_localGameState.Board);
                SetCursor(_localGameState.CurrentPlayer);
                _gameTimer.Sync(e.WhiteTime, e.BlackTime);
                _gameTimer.Start(Player.White);
                UpdateTimerColor();
            };

            // Trong hàm RegisterEvents()

            _responseHandler.GameUpdated += (s, e) =>
            {
                // [MỚI] CẬP NHẬT EN PASSANT TỪ SERVER
                // Nếu Server gửi về vị trí bắt tốt qua đường, ta phải cài đặt nó vào bàn cờ Client
                if (e.EnPassantPos != null)
                {
                    // Logic: Server gửi vị trí ô "bóng ma" (ô d6).
                    // Ta cần set vị trí này cho quân cờ vừa di chuyển 2 bước (là ĐỐI THỦ của người đang đi).
                    // e.CurrentPlayer là người đang đến lượt đi -> Quân vừa đi là Opponent.
                    Player opponent = (e.CurrentPlayer == Player.White) ? Player.Black : Player.White;

                    // Cài đặt trạng thái En Passant cho bàn cờ Client
                    // (Đảm bảo class Board của bạn có hàm SetPawnSkipPosition)
                    e.Board.SetPawnSkipPosition(opponent, e.EnPassantPos);
                }

                // Cập nhật lại GameState cục bộ với bàn cờ mới (đã có thông tin En Passant)
                _localGameState = new GameState(e.CurrentPlayer, e.Board);

                // Cập nhật giao diện (Bắt buộc chạy trên luồng UI)
                Dispatcher.Invoke(() =>
                {
                    DrawBoard(_localGameState.Board);
                    SetCursor(_localGameState.CurrentPlayer);
                    _gameTimer.Sync(e.WhiteTime, e.BlackTime);
                    _gameTimer.Start(e.CurrentPlayer);
                    UpdateTimerColor();
                });
            };

            _responseHandler.ChatReceived += (s, e) => AppendChatMessage(e.Sender, e.Content);
            _responseHandler.WaitingReceived += () => MessageBox.Show("Đang tìm đối thủ...");

            _responseHandler.GameOverFullReceived += (winner, reason) =>
            {
                _isGameOver = true;
                _gameTimer.Stop();
                _drawTimer.Stop();

                Dispatcher.Invoke(() =>
                {
                    DrawRequestBorder.Visibility = Visibility.Collapsed; // Sửa tên biến cho khớp XAML
                    MenuOverlay.Visibility = Visibility.Visible;
                    MenuOverlay.ShowGameOver(winner, reason);
                });
            };

            // Nhận lời mời hòa
            _responseHandler.DrawOfferReceived += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    DrawRequestBorder.Visibility = Visibility.Visible; // Sửa tên biến cho khớp XAML
                    _drawCountDown = 15;
                    lblDrawTimer.Text = $"Đối thủ xin hòa ({_drawCountDown}s)";
                    _drawTimer.Start();
                });
            };

            // Menu Overlay
            MenuOverlay.OptionSelected += option =>
            {
                if (option == Option.Restart)
                {
                    _ = _networkClient.SendAsync("REQUEST_RESTART");
                    MenuOverlay.DisableRestartButton();
                }
                else if (option == Option.Exit)
                {
                    _isExiting = true;
                    try { _ = _networkClient.SendAsync("LEAVE_GAME"); } catch { }
                    this.Close();
                }
                else if (option == Option.Analyze)
                {
                    _ = _networkClient.SendAsync("REQUEST_ANALYSIS");
                }
            };

            _responseHandler.AnalysisDataReceived += OnAnalysisDataReceived;

            _responseHandler.AskRestartReceived += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    var res = MessageBox.Show("Đối thủ muốn chơi lại?", "Tái đấu", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes) _networkClient.SendAsync("REQUEST_RESTART");
                    else _networkClient.SendAsync("RESTART_NO");
                });
            };

            _responseHandler.RestartDeniedReceived += () => Dispatcher.Invoke(() => MessageBox.Show("Đối thủ từ chối."));

            _responseHandler.OpponentLeftReceived += () =>
            {
                if (_isExiting || _isGameOver) return;
                Dispatcher.Invoke(() => { MessageBox.Show("Đối thủ đã thoát. Bạn thắng!"); _isExiting = true; this.Close(); });
            };

            _gameTimer.Tick += (w, b) =>
            {
                if (_isExiting) return;
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (_isExiting) return;
                        lblWhiteTime.Text = FormatTime(w);
                        lblBlackTime.Text = FormatTime(b);
                    });
                }
                catch { }
            };
        }

        // ==========================================================
        // 2. LOGIC XIN HÒA & ĐẦU HÀNG
        // ==========================================================
        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            _drawCountDown--;
            lblDrawTimer.Text = $"Đối thủ xin hòa ({_drawCountDown}s)";

            if (_drawCountDown <= 0)
            {
                _drawTimer.Stop();
                DrawRequestBorder.Visibility = Visibility.Collapsed;
                AppendChatMessage("Hệ thống", "Lời mời hòa đã hết hiệu lực.");
            }
        }

        private async void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            if (_isGameOver) return;
            if (_hasOfferedDraw)
            {
                MessageBox.Show("Mỗi trận đấu bạn chỉ được xin hòa 1 lần duy nhất!");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc muốn xin hòa? (Chỉ được dùng 1 lần)", "Xác nhận", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _hasOfferedDraw = true;
                btnDraw.IsEnabled = false;
                btnDraw.Content = "Đã xin hòa";
                await _networkClient.SendAsync("DRAW_OFFER");
                AppendChatMessage("Hệ thống", "Bạn đã gửi lời mời hòa. Đang chờ phản hồi (15s)...");
            }
        }

        private async void btnResign_Click(object sender, RoutedEventArgs e)
        {
            if (_isGameOver) return;
            var result = MessageBox.Show("Bạn có chắc chắn muốn đầu hàng không?", "Xác nhận đầu hàng", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _networkClient.SendAsync("RESIGN");
            }
        }

        private async void BtnAcceptDraw_Click(object sender, RoutedEventArgs e)
        {
            _drawTimer.Stop();
            DrawRequestBorder.Visibility = Visibility.Collapsed;
            await _networkClient.SendAsync("DRAW_ACCEPT");
        }

        private void BtnDeclineDraw_Click(object sender, RoutedEventArgs e)
        {
            _drawTimer.Stop();
            DrawRequestBorder.Visibility = Visibility.Collapsed;
            AppendChatMessage("Hệ thống", "Bạn đã từ chối lời mời hòa.");
        }

        // ==========================================================
        // 3. LOGIC BÀN CỜ & MẠNG
        // ==========================================================
        private void StartServerListener()
        {
            Task.Run(() =>
            {
                try
                {
                    while (!_isExiting && _networkClient.IsConnected)
                    {
                        string msg = _networkClient.WaitForMessage(500);
                        if (_isExiting) break;
                        if (msg == "TIMEOUT") continue;
                        if (msg == null)
                        {
                            if (!_isExiting) Dispatcher.Invoke(() => { if (!_isExiting) { MessageBox.Show("Mất kết nối!"); Close(); } });
                            break;
                        }
                        if (!_isExiting) Dispatcher.Invoke(() => { if (!_isExiting) _responseHandler.ProcessMessage(msg); });
                    }
                }
                catch { }
            });
        }

        private void OnAnalysisDataReceived(string moveHistoryString)
        {
            var moves = moveHistoryString.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            Dispatcher.Invoke(() =>
            {
                AnalysisWindow analysisWin = new AnalysisWindow(moves);
                this.Hide();
                analysisWin.Closed += (s, args) =>
                {
                    try { this.Show(); MenuOverlay.Visibility = Visibility.Visible; } catch { }
                };
                analysisWin.Show();
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isGameOver || _allowClose || _isExiting)
            {
                _gameTimer.Stop();
                _isExiting = true;
                return;
            }
            e.Cancel = true;
            var result = MessageBox.Show("Trận đấu đang diễn ra. Bạn có chấp nhận THUA để thoát không?", "Xác nhận thoát", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _allowClose = true;
                _isExiting = true;
                Task.Run(async () => { try { await _networkClient.SendAsync("LEAVE_GAME"); } catch { } });
                e.Cancel = false;
            }
        }

        private void InitializedBoard()
        {
            for (int r = 0; r < 8; r++) for (int c = 0; c < 8; c++)
                {
                    Image i = new Image(); pieceImages[r, c] = i; PieceGrid.Children.Add(i);
                    Rectangle h = new Rectangle(); highlights[r, c] = h; HighlightGrid.Children.Add(h);
                }
        }

        private void LoadBoardImageSafe()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("ChessUI.Assets.Board.png"))
                {
                    if (stream != null)
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit(); bitmap.StreamSource = stream; bitmap.CacheOption = BitmapCacheOption.OnLoad; bitmap.EndInit();
                        BoardGrid.Background = new ImageBrush(bitmap);
                    }
                }
            }
            catch { }
        }

        private void DrawBoard(Board board) { for (int r = 0; r < 8; r++) for (int c = 0; c < 8; c++) { Position p = (_myColor == Player.Black) ? new Position(7 - r, 7 - c) : new Position(r, c); pieceImages[r, c].Source = Images.GetImage(board[p]); } }

        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_localGameState == null || _isGameOver) return;
            Point p = e.GetPosition(BoardGrid);
            Position pos = ToSquarePosition(p);
            if (selectedPos == null) OnFromPositionSelected(pos); else OnToPositionSelected(pos);
        }

        private Position ToSquarePosition(Point p) { double s = BoardGrid.ActualWidth / 8; int r = (int)(p.Y / s); int c = (int)(p.X / s); if (_myColor == Player.Black) { r = 7 - r; c = 7 - c; } return new Position(r, c); }

        private void OnFromPositionSelected(Position pos)
        {
            if (_localGameState.CurrentPlayer != _myColor) return;
            var moves = _localGameState.MovesForPiece(pos);
            if (moves.Any()) { selectedPos = pos; CacheMoves(moves); ShowHighlights(); }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null; HideHighlights();
            if (moveCache.TryGetValue(pos, out List<Move> moves))
            {
                Move promotionMove = moves.FirstOrDefault(m => m.Type == MoveType.PawnPromotion);
                if (promotionMove != null) HandlePromotion(promotionMove.FromPos, promotionMove.ToPos);
                else HandleMove(moves.First());
            }
        }

        private void HandlePromotion(Position from, Position to)
        {
            PromotionMenu promMenu = new PromotionMenu(_localGameState.CurrentPlayer);
            MenuContainer.Content = promMenu;
            promMenu.PieceSelected += type => { MenuContainer.Content = null; Move finalMove = new PawnPromotion(from, to, type); HandleMove(finalMove); };
        }

        private void HandleMove(Move move)
        {
            if (_localGameState.CurrentPlayer != _myColor) return;
            Task.Run(async () =>
            {
                if (move.Type != MoveType.PawnPromotion && !move.IsLegal(_localGameState.Board))
                {
                    Dispatcher.Invoke(() => MessageBox.Show("Nước đi không hợp lệ")); return;
                }
                string cmd = $"MOVE|{move.FromPos.Row}|{move.FromPos.Column}|{move.ToPos.Row}|{move.ToPos.Column}";
                if (move is PawnPromotion promoMove) cmd += $"|{(int)promoMove.newType}";
                await _networkClient.SendAsync(cmd);
            });
        }

        private void CacheMoves(IEnumerable<Move> moves) { moveCache.Clear(); foreach (var m in moves) { if (!moveCache.ContainsKey(m.ToPos)) moveCache[m.ToPos] = new List<Move>(); moveCache[m.ToPos].Add(m); } }
        private void ShowHighlights() { Color c = Color.FromArgb(159, 125, 255, 125); foreach (var p in moveCache.Keys) { int r = p.Row; int col = p.Column; if (_myColor == Player.Black) { r = 7 - r; col = 7 - col; } highlights[r, col].Fill = new SolidColorBrush(c); } }
        private void HideHighlights() { foreach (var p in moveCache.Keys) { int r = p.Row; int c = p.Column; if (_myColor == Player.Black) { r = 7 - r; c = 7 - c; } highlights[r, c].Fill = Brushes.Transparent; } }
        private void SetCursor(Player p) { Cursor = (p == Player.White) ? ChessCursors.WhiteCursor : ChessCursors.BlackCursor; }

        private async void btnSendChat_Click(object s, RoutedEventArgs e) { if (!string.IsNullOrEmpty(txtChatInput.Text)) { await _networkClient.SendAsync($"CHAT|{txtChatInput.Text}"); txtChatInput.Text = ""; } }
        private void txtChatInput_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Enter) btnSendChat_Click(s, e); }
        private void AppendChatMessage(string s, string m) { Paragraph p = new Paragraph(); Run r1 = new Run(s + ": ") { FontWeight = FontWeights.Bold, Foreground = (s == "Trắng" || s == "You" || s == "Hệ thống") ? Brushes.CornflowerBlue : Brushes.Orange }; Run r2 = new Run(m) { Foreground = Brushes.White }; p.Inlines.Add(r1); p.Inlines.Add(r2); txtChatHistory.Document.Blocks.Add(p); txtChatHistory.ScrollToEnd(); }
        private string FormatTime(int s) => TimeSpan.FromSeconds(s).ToString(@"mm\:ss");
        private void UpdateTimerColor() { if (_localGameState == null) return; if (_localGameState.CurrentPlayer == Player.White) { lblWhiteTime.Foreground = Brushes.Red; lblBlackTime.Foreground = Brushes.White; } else { lblWhiteTime.Foreground = Brushes.White; lblBlackTime.Foreground = Brushes.Red; } }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChessClient.Models;
using ChessLogic;
using ChessUI.Models;
using ChessUI.Services;

namespace ChessUI
{
    public partial class AnalysisWindow : Window
    {
        // --- FIELDS ---
        private readonly List<string> _uciMoves;
        private readonly AnalysisService _service = new AnalysisService();
        private Dictionary<int, MoveAnalysis> _cache = new Dictionary<int, MoveAnalysis>();
        private bool _isClosing = false;
        private List<MoveRecord> _moveRecords = new List<MoveRecord>();
        private int _currentIndex = -1; // -1: Start position

        private readonly Image[,] _pieceImages = new Image[8, 8];

        // --- CẤU HÌNH MÀU BÀN CỜ (Xanh Thép - Giống MainWindow) ---
        private readonly Brush ColorLight = (Brush)new BrushConverter().ConvertFrom("#DDE7F0");
        private readonly Brush ColorDark = (Brush)new BrushConverter().ConvertFrom("#4B7399");

        // --- CONSTRUCTOR ---
        public AnalysisWindow(List<string> moves)
        {
            InitializeComponent();
            _uciMoves = moves;

            // 1. Khởi tạo bàn cờ (Vẽ màu + Ảnh quân)
            InitBoardUI();

            // 2. Khởi tạo danh sách nước đi
            InitMoveList();

            // 3. Load vị trí đầu tiên
            LoadBoardAt(-1);

            // 4. Chạy phân tích ngầm
            RunAnalysis();
        }

        private void AnalysisWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isClosing = true; // Dừng luồng phân tích
        }

        // --- KHỞI TẠO UI BÀN CỜ (Đã sửa để dùng màu Custom) ---
        private void InitBoardUI()
        {
            BoardSquareGrid.Children.Clear();
            PieceGrid.Children.Clear();

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    // A. VẼ Ô MÀU NỀN
                    Rectangle bgSquare = new Rectangle();
                    bool isLightSquare = (r + c) % 2 == 0;
                    bgSquare.Fill = isLightSquare ? ColorLight : ColorDark;
                    BoardSquareGrid.Children.Add(bgSquare);

                    // B. TẠO ẢNH QUÂN CỜ (Để trống ban đầu)
                    Image img = new Image();
                    _pieceImages[r, c] = img;
                    PieceGrid.Children.Add(img);
                }
            }
        }

        private void InitMoveList()
        {
            _moveRecords.Clear();
            int turn = 1;
            for (int i = 0; i < _uciMoves.Count; i += 2)
            {
                var rec = new MoveRecord
                {
                    TurnNumber = turn++,
                    WhiteMove = _uciMoves[i],
                    WhiteIndex = i,
                    WhiteEval = "...",
                    BlackMove = (i + 1 < _uciMoves.Count) ? _uciMoves[i + 1] : "",
                    BlackIndex = i + 1,
                    BlackEval = (i + 1 < _uciMoves.Count) ? "..." : ""
                };
                _moveRecords.Add(rec);
            }
            lstMoves.ItemsSource = _moveRecords;
        }

        // --- LOGIC BÀN CỜ ---
        private void LoadBoardAt(int index)
        {
            _currentIndex = index;

            // Tái tạo lại bàn cờ từ đầu đến nước thứ 'index'
            GameState tempState = new GameState(Player.White, Board.Initial());
            for (int i = 0; i <= index; i++)
            {
                Move m = ParseHelper.ParseUci(tempState, _uciMoves[i]);
                if (m != null) tempState.MakeMove(m);
            }

            // Vẽ quân cờ lên giao diện
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    _pieceImages[r, c].Source = Images.GetImage(tempState.Board[r, c]);

            // Vẽ mũi tên gợi ý (nếu đã phân tích xong)
            if (_cache.ContainsKey(index)) DrawArrow(_cache[index].Result.move);
            else ArrowCanvas.Children.Clear();

            SyncListSelection();
        }

        // --- LOGIC PHÂN TÍCH (GIỮ NGUYÊN) ---
        private async void RunAnalysis()
        {
            GameState tempState = new GameState(Player.White, Board.Initial());

            // Phân tích vị trí đầu tiên
            await AnalyzeAndCache(-1, tempState.Board.ToFenString(Player.White));

            // Phân tích từng nước đi
            for (int i = 0; i < _uciMoves.Count; i++)
            {
                if (_isClosing) return;
                Move m = ParseHelper.ParseUci(tempState, _uciMoves[i]);
                if (m != null) tempState.MakeMove(m);

                string fen = tempState.Board.ToFenString(tempState.CurrentPlayer);
                await AnalyzeAndCache(i, fen);
                UpdateListRecord(i);
            }

            if (!_isClosing) CalculateStats();
        }

        private async Task AnalyzeAndCache(int index, string fen)
        {
            var res = await _service.GetLichessAnalysisAsync(fen);
            if (res == null) res = await _service.GetAnalysisAsync(fen);

            if (res != null)
            {
                _cache[index] = new MoveAnalysis { MoveIndex = index, Result = res };
                if (_currentIndex == index) DrawArrow(res.move);
            }
        }

        private void UpdateListRecord(int i)
        {
            if (_isClosing) return;
            if (!_cache.ContainsKey(i)) return;

            var currRes = _cache[i].Result;
            int? currCp = currRes.centipawns;
            int? currMate = currRes.mate;

            // Cập nhật UI an toàn trên luồng chính
            Dispatcher.Invoke(() =>
            {
                if (_isClosing) return;
                var rec = _moveRecords[i / 2];
                lstMoves.Items.Refresh();
            });

            // Lấy điểm số nước trước đó để so sánh
            int? prevCp = 0;
            int? prevMate = null;

            if (i > 0 && _cache.ContainsKey(i - 1))
            {
                prevCp = _cache[i - 1].Result.centipawns;
                prevMate = _cache[i - 1].Result.mate;
            }
            else if (i == 0) prevCp = 0;

            // Phân loại nước đi (Tốt/Xấu/Sai lầm...)
            bool isWhiteTurn = (i % 2 == 0);
            var quality = MoveClassifier.Classify(prevCp, prevMate, currCp, currMate, isWhiteTurn);
            string color = MoveClassifier.GetColorHex(quality);
            string iconPath = MoveClassifier.GetIconPath(quality);

            // Cập nhật hiển thị vào danh sách
            Dispatcher.Invoke(() =>
            {
                var rec = _moveRecords[i / 2];
                string displayScore;

                if (currMate != null)
                {
                    displayScore = $"M{Math.Abs(currMate.Value)}";
                }
                else
                {
                    double scoreVal = currCp.GetValueOrDefault();
                    displayScore = (scoreVal > 0 ? "+" : "") + (scoreVal / 100.0).ToString("0.0");
                }

                if (isWhiteTurn)
                {
                    rec.WhiteEval = displayScore;
                    rec.WhiteColor = color;
                    rec.WhiteIcon = iconPath;
                }
                else
                {
                    rec.BlackEval = displayScore;
                    rec.BlackColor = color;
                    rec.BlackIcon = iconPath;
                }
                lstMoves.Items.Refresh();
            });
        }

        private void CalculateStats()
        {
            int wBrilliantCount = 0, wBestCount = 0, wMistakeCount = 0, wBlunderCount = 0;
            int bBrilliantCount = 0, bBestCount = 0, bMistakeCount = 0, bBlunderCount = 0;

            foreach (var kvp in _cache)
            {
                int i = kvp.Key;
                if (i < 0) continue;

                var currRes = kvp.Value.Result;
                int? prevCp = 0; int? prevMate = null;

                if (_cache.ContainsKey(i - 1))
                {
                    prevCp = _cache[i - 1].Result.centipawns;
                    prevMate = _cache[i - 1].Result.mate;
                }
                else if (i == 0) prevCp = 0;

                bool isWhiteTurn = (i % 2 == 0);
                var quality = MoveClassifier.Classify(prevCp, prevMate, currRes.centipawns, currRes.mate, isWhiteTurn);

                if (isWhiteTurn)
                {
                    switch (quality)
                    {
                        case MoveQuality.Brilliant: wBrilliantCount++; break;
                        case MoveQuality.Best: wBestCount++; break;
                        case MoveQuality.Mistake: wMistakeCount++; break;
                        case MoveQuality.Blunder: wBlunderCount++; break;
                    }
                }
                else
                {
                    switch (quality)
                    {
                        case MoveQuality.Brilliant: bBrilliantCount++; break;
                        case MoveQuality.Best: bBestCount++; break;
                        case MoveQuality.Mistake: bMistakeCount++; break;
                        case MoveQuality.Blunder: bBlunderCount++; break;
                    }
                }
            }

            Dispatcher.Invoke(() =>
            {
                if (_isClosing) return;
                wBrilliant.Text = wBrilliantCount.ToString();
                wBest.Text = wBestCount.ToString();
                wMistake.Text = wMistakeCount.ToString();
                wBlunder.Text = wBlunderCount.ToString();

                bBrilliant.Text = bBrilliantCount.ToString();
                bBest.Text = bBestCount.ToString();
                bMistake.Text = bMistakeCount.ToString();
                bBlunder.Text = bBlunderCount.ToString();
            });
        }

        // --- SỰ KIỆN CLICK CHUỘT ---
        private void WhiteMove_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is MoveRecord rec)
            {
                LoadBoardAt(rec.WhiteIndex);
                e.Handled = true;
            }
        }

        private void BlackMove_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is MoveRecord rec)
            {
                if (!string.IsNullOrEmpty(rec.BlackMove)) LoadBoardAt(rec.BlackIndex);
                e.Handled = true;
            }
        }

        private void lstMoves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMoves.SelectedItem is MoveRecord rec)
            {
                int target = string.IsNullOrEmpty(rec.BlackMove) ? rec.WhiteIndex : rec.BlackIndex;
                if (target != _currentIndex) LoadBoardAt(target);
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex > -1) LoadBoardAt(_currentIndex - 1);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex < _uciMoves.Count - 1) LoadBoardAt(_currentIndex + 1);
        }

        private void SyncListSelection()
        {
            if (_currentIndex == -1) return;
            int rowIndex = _currentIndex / 2;
            lstMoves.SelectedIndex = rowIndex;
            lstMoves.ScrollIntoView(lstMoves.Items[rowIndex]);
        }

        private void DrawArrow(string uciMove)
        {
            if (ArrowCanvas == null) return;
            ArrowCanvas.Children.Clear();
            if (string.IsNullOrEmpty(uciMove) || uciMove.Length < 4) return;

            double cellSize = 600.0 / 8.0;
            Position from = ParseHelper.ParseSquare(uciMove.Substring(0, 2));
            Position to = ParseHelper.ParseSquare(uciMove.Substring(2, 2));

            Line line = new Line
            {
                X1 = from.Column * cellSize + cellSize / 2,
                Y1 = from.Row * cellSize + cellSize / 2,
                X2 = to.Column * cellSize + cellSize / 2,
                Y2 = to.Row * cellSize + cellSize / 2,
                Stroke = Brushes.GreenYellow,
                StrokeThickness = 4,
                Opacity = 0.8
            };

            Ellipse circle = new Ellipse
            {
                Width = 15,
                Height = 15,
                Fill = Brushes.GreenYellow,
                Margin = new Thickness(line.X2 - 7.5, line.Y2 - 7.5, 0, 0)
            };

            ArrowCanvas.Children.Add(line);
            ArrowCanvas.Children.Add(circle);
        }
    }

    public static class ParseHelper
    {
        public static Move ParseUci(GameState state, string uci)
        {
            if (string.IsNullOrEmpty(uci) || uci.Length < 4) return null;
            Position from = ParseSquare(uci.Substring(0, 2));
            Position to = ParseSquare(uci.Substring(2, 2));
            var moves = state.MovesForPiece(from);

            if (uci.Length == 5) // Phong cấp
            {
                return moves.FirstOrDefault(m => m.ToPos == to && m.Type == MoveType.PawnPromotion);
            }
            return moves.FirstOrDefault(m => m.ToPos == to);
        }

        public static Position ParseSquare(string sq)
        {
            return new Position(8 - (sq[1] - '0'), sq[0] - 'a');
        }
    }
}
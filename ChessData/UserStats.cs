namespace ChessData
{
    /// <summary>
    /// Thống kê người chơi dùng cho màn Profile.
    /// Chỉ là DTO, không có logic DB ở đây.
    /// </summary>
    public class UserStats
    {
        public string Username { get; set; } = "";
        public string IngameName { get; set; } = "";

        public int Rank { get; set; }
        public int HighestRank { get; set; }

        public int Wins { get; set; }
        public int Losses { get; set; }

        /// <summary>
        /// Tổng số ván đã chơi = Wins + Losses
        /// </summary>
        public int TotalGames => Wins + Losses;

        /// <summary>
        /// Thời gian chơi tổng (phút)
        /// </summary>
        public int TotalPlayTimeMinutes { get; set; }

        /// <summary>
        /// Tỉ lệ thắng (0–100). Nếu chưa chơi trận nào thì = 0.
        /// </summary>
        public double WinRate => TotalGames > 0
            ? (double)Wins / TotalGames * 100.0
            : 0.0;

        /// <summary>
        /// Danh hiệu (tùy bạn map theo Rank).
        /// </summary>
        public int TitleId { get; set; }
        public string TitleName { get; set; } = "";
    }
}

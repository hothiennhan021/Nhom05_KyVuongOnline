namespace ChessData
{
    public class UserStats
    {
        public string Username { get; set; }
        public string IngameName { get; set; }

        public int Rank { get; set; }
        public int HighestRank { get; set; }

        public int Wins { get; set; }
        public int Losses { get; set; }

        public int TotalPlayTimeMinutes { get; set; }

        public int Elo { get; set; }
        //....gitl 

        // Tỉ lệ thắng (tính trong code)
        public double WinRate => (Wins + Losses) == 0 ? 0 : (Wins * 100.0 / (Wins + Losses));
    }
}

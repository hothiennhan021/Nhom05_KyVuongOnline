using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ChessData
{
    public class MatchRepository
    {
        private readonly string _conn;

        public MatchRepository(string conn)
        {
            _conn = conn;
        }

        public async Task UpdateMatchResult(string winner, string loser, int minutesPlayed)
        {
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            // WINNER UPDATE
            string sqlWinner =
            @"UPDATE Users
              SET Wins = Wins + 1,
                  Rank = Rank + 10,
                  HighestRank = CASE WHEN Rank + 10 > HighestRank THEN Rank + 10 ELSE HighestRank END,
                  TotalPlayTimeMinutes = TotalPlayTimeMinutes + @m
              WHERE Username = @user";

            using (var cmd = new SqlCommand(sqlWinner, conn))
            {
                cmd.Parameters.AddWithValue("@user", winner);
                cmd.Parameters.AddWithValue("@m", minutesPlayed);
                await cmd.ExecuteNonQueryAsync();
            }

            // LOSER UPDATE
            string sqlLoser =
            @"UPDATE Users
              SET Losses = Losses + 1,
                  Rank = CASE WHEN Rank > 800 THEN Rank - 10 ELSE Rank END,
                  TotalPlayTimeMinutes = TotalPlayTimeMinutes + @m
              WHERE Username = @user";

            using (var cmd = new SqlCommand(sqlLoser, conn))
            {
                cmd.Parameters.AddWithValue("@user", loser);
                cmd.Parameters.AddWithValue("@m", minutesPlayed);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}

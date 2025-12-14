using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ChessData
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connection)
        {
            _connectionString = connection;
        }

        // --- GET AVATAR ---
        public async Task<byte[]?> GetAvatarAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "SELECT Avatar FROM Users WHERE Username=@u";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);

            object result = await cmd.ExecuteScalarAsync();
            return result == DBNull.Value ? null : (byte[])result;
        }

        // --- UPDATE AVATAR ---
        public async Task UpdateAvatarAsync(string username, byte[] avatar)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "UPDATE Users SET Avatar=@a WHERE Username=@u";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@a", avatar);
            cmd.Parameters.AddWithValue("@u", username);

            await cmd.ExecuteNonQueryAsync();
        }

        // --- REGISTER ---
        public async Task<string> RegisterUserAsync(string username, string password, string email, string fullName, string birthday)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string checkSql = "SELECT COUNT(*) FROM Users WHERE Username=@u OR Email=@e";
                using (var checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@u", username);
                    checkCmd.Parameters.AddWithValue("@e", email);

                    int count = (int)await checkCmd.ExecuteScalarAsync();
                    if (count > 0)
                        return "ERROR|Tên đăng nhập hoặc email đã tồn tại.";
                }

                string hash = BCrypt.Net.BCrypt.HashPassword(password);

                string insertSql =
                    @"INSERT INTO Users 
                      (Username, PasswordHash, Email, FullName, Birthday, IngameName, Rank, HighestRank, Wins, Losses, TotalPlayTimeMinutes, IsOnline) 
                      VALUES (@u, @p, @e, @f, @b, @in, 800, 800, 0, 0, 0, 0)";

                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", hash);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@f", fullName);
                    cmd.Parameters.AddWithValue("@b", DateTime.Parse(birthday));
                    cmd.Parameters.AddWithValue("@in", username);
                    await cmd.ExecuteNonQueryAsync();
                }

                return "REGISTER_SUCCESS";
            }
            catch (Exception ex)
            {
                return "ERROR|" + ex.Message;
            }
        }

        // --- LOGIN ---
        public async Task<string> LoginUserAsync(string username, string password)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"SELECT PasswordHash, Email, FullName, Birthday 
                               FROM Users WHERE Username=@u";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);

                using var reader = await cmd.ExecuteReaderAsync();
                if (!reader.Read())
                    return "ERROR|Sai tài khoản";

                string hash = reader["PasswordHash"].ToString();

                if (!BCrypt.Net.BCrypt.Verify(password, hash))
                    return "ERROR|Sai mật khẩu";

                string fullName = reader["FullName"].ToString();
                string email = reader["Email"].ToString();
                string birthday = Convert.ToDateTime(reader["Birthday"]).ToString("yyyy-MM-dd");

                return $"LOGIN_SUCCESS|{fullName}|{email}|{birthday}";
            }
            catch (Exception ex)
            {
                return "ERROR|" + ex.Message;
            }
        }

        // --- GET STATS ---
        public async Task<UserStats?> GetUserStatsAsync(string username)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    SELECT Username, IngameName, Rank, HighestRank, Wins, Losses, TotalPlayTimeMinutes, Elo
                    FROM Users WHERE Username=@u";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);

                using var reader = await cmd.ExecuteReaderAsync();
                if (!reader.Read()) return null;

                return new UserStats
                {
                    Username = reader["Username"].ToString(),
                    IngameName = reader["IngameName"].ToString(),
                    Rank = Convert.ToInt32(reader["Rank"]),
                    HighestRank = Convert.ToInt32(reader["HighestRank"]),
                    Wins = Convert.ToInt32(reader["Wins"]),
                    Losses = Convert.ToInt32(reader["Losses"]),
                    TotalPlayTimeMinutes = reader["TotalPlayTimeMinutes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalPlayTimeMinutes"]),
                    Elo = reader["Elo"] != DBNull.Value ? Convert.ToInt32(reader["Elo"]) : 1200
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB Error] GetUserStatsAsync({username}): {ex.Message}");
                return null;
            }
        }

        // --- SET ONLINE ---
        public void SetOnline(int userId, bool isOnline)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = "UPDATE Users SET IsOnline=@o WHERE UserId=@id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@o", isOnline ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", userId);

            cmd.ExecuteNonQuery();
        }

        // --- CHECK EMAIL ---
        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = "SELECT COUNT(*) FROM Users WHERE Email=@e";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@e", email);

                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        // ⭐⭐ HÀM MỚI: UPDATE INGAME NAME ⭐⭐
        public async Task<bool> UpdateIngameNameAsync(string username, string newName)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"UPDATE Users SET IngameName=@n WHERE Username=@u";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@n", newName);
                cmd.Parameters.AddWithValue("@u", username);

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        // --- UPDATE GAME RESULT (ELO & STATS) ---
        public async Task UpdateGameResultAsync(string username, int eloChange, int resultType)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                // SỬA LẠI: Dùng cột [Elo] để cộng trừ điểm
                string sql = @"
            UPDATE Users 
            SET 
                Elo = Elo + @elo,  -- Sửa Rank -> Elo
                Wins = Wins + (CASE WHEN @type = 1 THEN 1 ELSE 0 END),
                Losses = Losses + (CASE WHEN @type = -1 THEN 1 ELSE 0 END),
                
                -- Cập nhật HighestRank dựa trên điểm Elo mới
                HighestRank = CASE 
                                WHEN (Elo + @elo) > ISNULL(HighestRank, 0) 
                                THEN (Elo + @elo) 
                                ELSE HighestRank 
                              END
            WHERE Username = @u";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@elo", eloChange);
                cmd.Parameters.AddWithValue("@type", resultType);
                cmd.Parameters.AddWithValue("@u", username);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB Error] Update Result: {ex.Message}");
            }
        }
    }
}

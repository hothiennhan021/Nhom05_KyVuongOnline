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

        // ==============================
        //  ĐĂNG KÝ
        // ==============================
        public async Task<string> RegisterUserAsync(string username, string password, string email, string fullName, string birthday)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                // Check tồn tại
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
                    cmd.Parameters.AddWithValue("@in", username); // ingame = username
                    await cmd.ExecuteNonQueryAsync();
                }

                return "REGISTER_SUCCESS";
            }
            catch (Exception ex)
            {
                return "ERROR|" + ex.Message;
            }
        }

        // ==============================
        //  ĐĂNG NHẬP
        // ==============================
        public async Task<string> LoginUserAsync(string username, string password)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"SELECT PasswordHash, Email, FullName, Birthday 
                               FROM Users WHERE Username=@u";

                string passwordHash = "";
                string fullName = "";
                string email = "";
                string birthday = "";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using var reader = await cmd.ExecuteReaderAsync();
                    if (!reader.Read())
                        return "ERROR|Sai tài khoản";

                    passwordHash = reader["PasswordHash"].ToString();
                    fullName = reader["FullName"].ToString();
                    email = reader["Email"].ToString();
                    birthday = Convert.ToDateTime(reader["Birthday"]).ToString("yyyy-MM-dd");
                }

                if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
                    return "ERROR|Sai mật khẩu";

                return $"LOGIN_SUCCESS|{fullName}|{email}|{birthday}";
            }
            catch (Exception ex)
            {
                return "ERROR|" + ex.Message;
            }
        }

        // ==============================
        //  LẤY THỐNG KÊ PROFILE
        // ==============================
        public async Task<UserStats?> GetUserStatsAsync(string username)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    SELECT Username, IngameName, Rank, HighestRank, Wins, Losses, TotalPlayTimeMinutes
                    FROM Users
                    WHERE Username=@u";

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
                    TotalPlayTimeMinutes = Convert.ToInt32(reader["TotalPlayTimeMinutes"])
                };
            }
            catch
            {
                return null;
            }
        }

        // ==============================
        //  CẬP NHẬT TRẠNG THÁI ONLINE
        // ==============================
        public void SetOnline(int userId, bool isOnline)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = "UPDATE Users SET IsOnline = @o WHERE UserId = @id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@o", isOnline ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ChessData
{
    public class FriendRepository
    {
        private readonly string _connectionString;

        public FriendRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ==============================================================
        // 1. Gửi lời mời kết bạn
        // ==============================================================
        public string SendFriendRequest(int fromUserId, string toUsername)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // A. Tìm user nhận
                string findIdSql = "SELECT UserId FROM Users WHERE Username = @u";
                SqlCommand cmdFind = new SqlCommand(findIdSql, conn);
                cmdFind.Parameters.AddWithValue("@u", toUsername);

                object result = cmdFind.ExecuteScalar();
                if (result == null) return "NOT_FOUND";

                int toUserId = (int)result;
                if (toUserId == fromUserId) return "SELF_ERROR";

                // B. Kiểm tra đã tồn tại quan hệ bạn bè hoặc lời mời chưa
                string checkSql = @"
                    SELECT COUNT(*) FROM Friendships 
                    WHERE (RequesterId=@f AND ReceiverId=@t)
                       OR (RequesterId=@t AND ReceiverId=@f)";

                SqlCommand cmdCheck = new SqlCommand(checkSql, conn);
                cmdCheck.Parameters.AddWithValue("@f", fromUserId);
                cmdCheck.Parameters.AddWithValue("@t", toUserId);

                if ((int)cmdCheck.ExecuteScalar() > 0)
                    return "EXISTED";

                // C. Tạo lời mời
                string insertSql = "INSERT INTO Friendships (RequesterId, ReceiverId, Status) VALUES (@f, @t, 0)";
                SqlCommand cmdInsert = new SqlCommand(insertSql, conn);
                cmdInsert.Parameters.AddWithValue("@f", fromUserId);
                cmdInsert.Parameters.AddWithValue("@t", toUserId);
                cmdInsert.ExecuteNonQuery();

                return "SUCCESS";
            }
        }

        // ==============================================================
        // 2. Lấy danh sách bạn bè
        // ==============================================================
        public List<string> GetListFriends(int userId)
        {
            List<string> list = new List<string>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();


                string sql = @"
                    SELECT u.Username, u.Elo, u.IsOnline
                    FROM Friendships f
                    JOIN Users u 
                        ON (u.UserId = f.RequesterId OR u.UserId = f.ReceiverId)
                    WHERE (f.RequesterId = @uid OR f.ReceiverId = @uid)
                      AND u.UserId != @uid
                      AND f.Status = 1"; // ⭐ CHỈ LẤY BẠN ĐÃ ACCEPT

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add($"{reader["Username"]}|{reader["Elo"]}|{reader["IsOnline"]}");
                }
            }

            return list;
        }

        // ==============================================================
        // 3. Lấy danh sách lời mời kết bạn (Status = 0)
        // ==============================================================
        public List<string> GetFriendRequests(int userId)
        {
            List<string> list = new List<string>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT f.Id AS RequestId, u.Username
                    FROM Friendships f
                    JOIN Users u ON u.UserId = f.RequesterId
                    WHERE f.ReceiverId = @uid AND f.Status = 0";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add($"{reader["RequestId"]}|{reader["Username"]}");
                }
            }

            return list;
        }

        // ==============================================================
        // 4. Đồng ý kết bạn
        // ==============================================================
        public void AcceptFriend(int requestId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // ⭐ CAST để tương thích mọi kiểu dữ liệu (bit, int, tinyint, varchar)
                string sql = "UPDATE Friendships SET Status = CAST(1 AS int) WHERE Id = @id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", requestId);

                cmd.ExecuteNonQuery();
            }
        }

        // ==============================================================
        // 5. Xóa / Hủy kết bạn
        // ==============================================================
        public bool RemoveFriend(int myUserId, string friendName)
        {
            try
            {
                friendName = friendName.Split(' ')[0]; // name (fix safety)

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // A. Lấy friendId
                    string findIdSql = "SELECT UserId FROM Users WHERE Username = @u";
                    SqlCommand cmd = new SqlCommand(findIdSql, conn);
                    cmd.Parameters.AddWithValue("@u", friendName);

                    object result = cmd.ExecuteScalar();
                    if (result == null) return false;

                    int friendId = (int)result;

                    // B. Xóa quan hệ
                    string deleteSql = @"
                        DELETE FROM Friendships
                        WHERE (RequesterId=@me AND ReceiverId=@friend)
                           OR (RequesterId=@friend AND ReceiverId=@me)";

                    SqlCommand cmdDel = new SqlCommand(deleteSql, conn);
                    cmdDel.Parameters.AddWithValue("@me", myUserId);
                    cmdDel.Parameters.AddWithValue("@friend", friendId);

                    return cmdDel.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using FinanceML.Core.Models;

namespace FinanceML.Core.Data
{
    public class UserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public int CreateUser(User user)
        {
            const string sql = @"
                INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, CreatedDate, LastLoginDate, IsActive)
                VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @CreatedDate, @LastLoginDate, @IsActive);
                SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@CreatedDate", user.CreatedDate);
            command.Parameters.AddWithValue("@LastLoginDate", user.LastLoginDate);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public List<User> GetAllUsers()
        {
            const string sql = "SELECT * FROM Users WHERE IsActive = 1";
            var users = new List<User>();

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    PasswordHash = reader["PasswordHash"].ToString() ?? "",
                    FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader["FirstName"].ToString() ?? "",
                    LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader["LastName"].ToString() ?? "",
                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                    LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? DateTime.MinValue : Convert.ToDateTime(reader["LastLoginDate"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }

            return users;
        }

        public User? GetUserById(int id)
        {
            const string sql = "SELECT * FROM Users WHERE Id = @Id AND IsActive = 1";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    PasswordHash = reader["PasswordHash"].ToString() ?? "",
                    FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader["FirstName"].ToString() ?? "",
                    LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader["LastName"].ToString() ?? "",
                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                    LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? DateTime.MinValue : Convert.ToDateTime(reader["LastLoginDate"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }

            return null;
        }

        public User? GetUserByUsername(string username)
        {
            const string sql = "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Username", username);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    PasswordHash = reader["PasswordHash"].ToString() ?? "",
                    FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader["FirstName"].ToString() ?? "",
                    LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader["LastName"].ToString() ?? "",
                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                    LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? DateTime.MinValue : Convert.ToDateTime(reader["LastLoginDate"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }

            return null;
        }

        public bool UpdateUser(User user)
        {
            const string sql = @"
                UPDATE Users 
                SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash, 
                    FirstName = @FirstName, LastName = @LastName, LastLoginDate = @LastLoginDate
                WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@LastLoginDate", user.LastLoginDate);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateLastLoginDate(int userId)
        {
            const string sql = "UPDATE Users SET LastLoginDate = @LastLoginDate WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", userId);
            command.Parameters.AddWithValue("@LastLoginDate", DateTime.Now);

            return command.ExecuteNonQuery() > 0;
        }

        public bool IsUsernameAvailable(string username)
        {
            const string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Username", username);

            return Convert.ToInt32(command.ExecuteScalar()) == 0;
        }

        public bool IsEmailAvailable(string email)
        {
            const string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Email", email);

            return Convert.ToInt32(command.ExecuteScalar()) == 0;
        }

        public bool DeleteUser(int id)
        {
            const string sql = "UPDATE Users SET IsActive = 0 WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }
    }
}

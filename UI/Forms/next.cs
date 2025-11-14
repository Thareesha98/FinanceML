namespace ECommerce.Data.Repositories
{
    using ECommerce.Data.Interfaces;
    using ECommerce.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient; // Using SQL Server as the example database
    using System.Linq;
    using System.Threading.Tasks;

    // Placeholder for User Model (needs to be defined elsewhere)
    public class User { public int Id { get; set; } public string Email { get; set; } public string Username { get; set; } public string HashedPassword { get; set; } public DateTime CreatedDate { get; set; } public bool IsActive { get; set; } }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                ?? throw new InvalidOperationException("DefaultConnection string is missing.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            const string sql = "SELECT Id, Email, Username, HashedPassword, CreatedDate, IsActive FROM Users WHERE Id = @UserId AND IsActive = 1;";
            
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapUserFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error retrieving user by ID: {UserId}", userId);
                // Optionally throw a custom application exception here
                throw; 
            }
            return null;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            const string sql = "SELECT Id, Email, Username, HashedPassword, CreatedDate, IsActive FROM Users WHERE Email = @Email AND IsActive = 1;";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapUserFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error retrieving user by Email: {Email}", email);
                throw;
            }
            return null;
        }

        public async Task<int> AddAsync(User user)
        {
            const string sql = "INSERT INTO Users (Email, Username, HashedPassword, CreatedDate, IsActive) VALUES (@Email, @Username, @Password, @CreatedDate, 1); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            int newId = 0;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        // Use DBNull for potentially null values if needed, otherwise rely on C# type safety
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@Password", user.HashedPassword);
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);

                        newId = (int)await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error adding new user: {Email}", user.Email);
                throw;
            }
            return newId;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            const string sql = "UPDATE Users SET Email = @Email, Username = @Username, IsActive = @IsActive WHERE Id = @Id;";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@IsActive", user.IsActive);
                        
                        // We use ExecuteNonQuery because we don't expect a result set.
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error updating user: {UserId}", user.Id);
                throw;
            }
        }
        
        public async Task<bool> DeleteAsync(int userId)
        {
            // Note: In real applications, this is usually a soft delete (SET IsActive = 0).
            const string sql = "UPDATE Users SET IsActive = 0 WHERE Id = @UserId;"; 

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        return await command.ExecuteNonQueryAsync() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error deleting (soft delete) user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            const string sql = "SELECT COUNT(Id) FROM Users WHERE Email = @Email;";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        int count = (int)await command.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error checking existence by email: {Email}", email);
                throw;
            }
        }
        
        public async Task<IEnumerable<User>> GetPagedActiveUsersAsync(int pageIndex, int pageSize)
        {
            // SQL Server T-SQL Paging: OFFSET FETCH NEXT
            const string sql = @"
                SELECT Id, Email, Username, HashedPassword, CreatedDate, IsActive 
                FROM Users 
                WHERE IsActive = 1
                ORDER BY CreatedDate DESC 
                OFFSET @OffsetRows

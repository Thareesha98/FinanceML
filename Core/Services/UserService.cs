using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FinanceML.Core.Models;
using FinanceML.Core.Data;

namespace FinanceML.Core.Services
{
    public class UserService : IDisposable
    {
        private static UserService? _instance;
        private readonly DatabaseContext _context;
        private readonly UserRepository _userRepository;
        private User? _currentUser;

        public static UserService Instance => _instance ??= new UserService();

        public User? CurrentUser => _currentUser;

        private UserService()
        {
            _context = new DatabaseContext();
            _userRepository = new UserRepository(_context);
            InitializeSampleUsers();
        }

        public bool RegisterUser(string username, string email, string password, string firstName = "", string lastName = "")
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            if (!_userRepository.IsUsernameAvailable(username))
                return false;

            if (!_userRepository.IsEmailAvailable(email))
                return false;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            var userId = _userRepository.CreateUser(user);
            user.Id = userId;
            return userId > 0;
        }

        public bool LoginUser(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);
            
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                _currentUser = user;
                _userRepository.UpdateLastLoginDate(user.Id);
                
                // Set the current user ID in DataService
                DataService.Instance.SetCurrentUserId(user.Id);
                
                // Load user settings
                SettingsService.Instance.LoadUserSettings(user.Id);
                
                // Seed data for this user if they don't have any
                var transactionRepo = new TransactionRepository(_context);
                var budgetRepo = new BudgetRepository(_context);
                DataSeeder.SeedUserData(user.Id, transactionRepo, budgetRepo);
                
                return true;
            }

            return false;
        }

        public void LogoutUser()
        {
            _currentUser = null;
        }

        public bool UpdateUserProfile(string firstName, string lastName, string email)
        {
            if (_currentUser == null)
                return false;

            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Check if email is already taken by another user
            if (!_userRepository.IsEmailAvailable(email))
            {
                // If email is not available, check if it belongs to current user
                var existingUsers = _userRepository.GetAllUsers();
                var userWithEmail = existingUsers.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                if (userWithEmail != null && userWithEmail.Id != _currentUser.Id)
                    return false;
            }

            _currentUser.FirstName = firstName;
            _currentUser.LastName = lastName;
            _currentUser.Email = email;

            return _userRepository.UpdateUser(_currentUser);
        }

        public bool ChangePassword(string currentPassword, string newPassword)
        {
            if (_currentUser == null)
                return false;

            if (!VerifyPassword(currentPassword, _currentUser.PasswordHash))
                return false;

            if (string.IsNullOrWhiteSpace(newPassword))
                return false;

            _currentUser.PasswordHash = HashPassword(newPassword);
            return _userRepository.UpdateUser(_currentUser);
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public bool IsUsernameAvailable(string username)
        {
            return _userRepository.IsUsernameAvailable(username);
        }

        public bool IsEmailAvailable(string email)
        {
            return _userRepository.IsEmailAvailable(email);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "FINANCEML_SALT_2024"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private void InitializeSampleUsers()
        {
            // Create sample users if no users exist
            var existingUsers = _userRepository.GetAllUsers();
            if (!existingUsers.Any())
            {
                // Create multiple sample users with different profiles and realistic data
                var sampleUsers = new List<(string username, string email, string password, string firstName, string lastName, DateTime createdDate)>
                {
                    ("demo", "demo@example.com", "demo123", "Demo", "User", DateTime.Now.AddDays(-90)),
                    ("john_doe", "john.doe@email.com", "password123", "John", "Doe", DateTime.Now.AddDays(-75)),
                    ("jane_smith", "jane.smith@email.com", "secure456", "Jane", "Smith", DateTime.Now.AddDays(-60)),
                    ("alex_johnson", "alex.johnson@email.com", "mypass789", "Alex", "Johnson", DateTime.Now.AddDays(-45)),
                    ("sarah_wilson", "sarah.wilson@email.com", "safepass321", "Sarah", "Wilson", DateTime.Now.AddDays(-30)),
                    ("mike_brown", "mike.brown@email.com", "brown2024", "Michael", "Brown", DateTime.Now.AddDays(-20)),
                    ("lisa_davis", "lisa.davis@email.com", "davis456", "Lisa", "Davis", DateTime.Now.AddDays(-15)),
                    ("david_miller", "david.miller@email.com", "miller789", "David", "Miller", DateTime.Now.AddDays(-10)),
                    ("emma_garcia", "emma.garcia@email.com", "garcia123", "Emma", "Garcia", DateTime.Now.AddDays(-5)),
                    ("ryan_martinez", "ryan.martinez@email.com", "martinez2024", "Ryan", "Martinez", DateTime.Now.AddDays(-2))
                };

                foreach (var (username, email, password, firstName, lastName, createdDate) in sampleUsers)
                {
                    var user = new User
                    {
                        Username = username,
                        Email = email,
                        PasswordHash = HashPassword(password),
                        FirstName = firstName,
                        LastName = lastName,
                        CreatedDate = createdDate,
                        LastLoginDate = createdDate.AddDays(Random.Shared.Next(1, 10)), // Random login within 10 days of creation
                        IsActive = true
                    };

                    _userRepository.CreateUser(user);
                }

                // Sample users initialized silently
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
// IMPORTANT: For production, replace System.Security.Cryptography with a library like BCrypt.Net for secure password hashing.
using System.Security.Cryptography; 
using System.Text;
using FinanceML.Core.Models;
using FinanceML.Core.Data;

namespace FinanceML.Core.Services; // C# 10 File-Scoped Namespace

/// <summary>
/// Service layer for user management, authentication, and profile updates.
/// </summary>
/// <remarks>
/// This class is designed to be registered as a scoped or transient service 
/// using Dependency Injection, hence the removal of the Singleton pattern.
/// </remarks>
public class UserService // Removed IDisposable since context is handled by DI container
{
    private readonly DatabaseContext _context;
    private readonly UserRepository _userRepository;
    private User? _currentUser;
    // NOTE: Static Random is deprecated; using Random.Shared in InitializeSampleUsers is correct.
    private const string PasswordSalt = "FINANCEML_SALT_2024"; 

    /// <summary>
    /// Gets the currently authenticated user.
    /// </summary>
    public User? CurrentUser => _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class using Dependency Injection.
    /// </summary>
    /// <param name="context">The database context (injected).</param>
    /// <param name="userRepository">The user repository (injected).</param>
    public UserService(DatabaseContext context, UserRepository userRepository)
    {
        // Dependency Injection - Context and Repository are now received here.
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        
        // This initialization logic should ideally be moved to an application startup routine or a dedicated Seeder service.
        InitializeSampleUsers();
    }

    // --- Authentication and Registration ---

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public bool RegisterUser(string username, string email, string password, string firstName = "", string lastName = "")
    {
        // Use short-circuiting to check for null/whitespace and existence
        if (string.IsNullOrWhiteSpace(username) || 
            string.IsNullOrWhiteSpace(email) || 
            string.IsNullOrWhiteSpace(password) ||
            !_userRepository.IsUsernameAvailable(username) || 
            !_userRepository.IsEmailAvailable(email))
        {
            return false;
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            CreatedDate = DateTime.UtcNow, // Use UTC for consistency
            IsActive = true
        };

        var userId = _userRepository.CreateUser(user);
        user.Id = userId; // Update the entity with the generated ID
        return userId > 0;
    }

    /// <summary>
    /// Authenticates a user with a given username and password.
    /// </summary>
    public bool LoginUser(string username, string password)
    {
        var user = _userRepository.GetUserByUsername(username);
        
        // Use null check and method call with expression body
        if (user is not null && VerifyPassword(password, user.PasswordHash))
        {
            _currentUser = user;
            _userRepository.UpdateLastLoginDate(user.Id);
            
            // Assume these services are Singletons or globally accessible if not using DI throughout the application
            if (DataService.Instance is not null)
                DataService.Instance.SetCurrentUserId(user.Id);
            
            if (SettingsService.Instance is not null)
                SettingsService.Instance.LoadUserSettings(user.Id);
            
            // Refactor data seeding to use injected repositories for better testability
            var transactionRepo = new TransactionRepository(_context);
            var budgetRepo = new BudgetRepository(_context);
            DataSeeder.SeedUserData(user.Id, transactionRepo, budgetRepo);
            
            return true;
        }

        return false;
    }

    /// <summary>
    /// Clears the current user session.
    /// </summary>
    public void LogoutUser() => _currentUser = null;

    // --- Profile Management ---

    /// <summary>
    /// Updates the current user's profile details (first name, last name, email).
    /// </summary>
    public bool UpdateUserProfile(string firstName, string lastName, string email)
    {
        if (_currentUser is null || string.IsNullOrWhiteSpace(email))
            return false;
        
        // Check if email is already taken by another user by fetching the user with that email.
        // This is a cleaner, more efficient check than fetching ALL users.
        var userWithEmail = _userRepository.GetUserByEmail(email);
        
        if (userWithEmail is not null && userWithEmail.Id != _currentUser.Id)
        {
            // Email is already taken by someone else
            return false;
        }

        _currentUser.FirstName = firstName;
        _currentUser.LastName = lastName;
        _currentUser.Email = email;

        return _userRepository.UpdateUser(_currentUser);
    }

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    public bool ChangePassword(string currentPassword, string newPassword)
    {
        if (_currentUser is null || string.IsNullOrWhiteSpace(newPassword))
            return false;

        // Use the existing VerifyPassword function
        if (!VerifyPassword(currentPassword, _currentUser.PasswordHash))
            return false;

        _currentUser.PasswordHash = HashPassword(newPassword);
        return _userRepository.UpdateUser(_currentUser);
    }

    // --- Utility Methods ---

    public List<User> GetAllUsers() => _userRepository.GetAllUsers();

    public bool IsUsernameAvailable(string username) => _userRepository.IsUsernameAvailable(username);

    public bool IsEmailAvailable(string email) => _userRepository.IsEmailAvailable(email);

    /// <summary>
    /// Hashes the given password using SHA256 with a hardcoded salt.
    /// </summary>
    /// <remarks>
    /// **SECURITY WARNING:** This method should be replaced with a modern library 
    /// like BCrypt or Argon2 (e.g., using <c>BCrypt.Net</c> package) for real-world applications.
    /// </remarks>
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + PasswordSalt));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verifies a plain-text password against a hash.
    /// </summary>
    private bool VerifyPassword(string password, string hash) => HashPassword(password) == hash;

    // Initializes sample users if the database is empty.
    private void InitializeSampleUsers()
    {
        if (!_userRepository.GetAllUsers().Any())
        {
            var sampleUsers = new List<(string username, string email, string password, string firstName, string lastName, DateTime createdDate)>
            {
                ("demo", "demo@example.com", "demo123", "Demo", "User", DateTime.UtcNow.AddDays(-90)),
                // ... (rest of the sample users)
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
                    LastLoginDate = createdDate.AddDays(Random.Shared.Next(1, 10)),
                    IsActive = true
                };
                _userRepository.CreateUser(user);
            }
        }
    }
}

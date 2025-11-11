using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// --- MODEL & INTERFACE DEFINITIONS (Usually in separate files) ---

// Placeholder for a simple User model
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

// Placeholder for the application's database context
public interface IAppDbContext : IDisposable
{
    DbSet<User> Users { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

// Interface defining the User Repository contract
public interface IUserRepository
{
    Task<User> GetByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllActiveUsersAsync();
    Task<User> AddAsync(User newUser);
    Task<bool> UpdateAsync(User userToUpdate);
    Task<bool> DeleteAsync(int userId);
    Task<bool> IsEmailUniqueAsync(string email, int excludeUserId = 0);
}

// --- REPOSITORY IMPLEMENTATION ---

public class UserRepository : IUserRepository
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IAppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("UserRepository initialized.");
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The User object, or null if not found.</returns>
    public async Task<User> GetByIdAsync(int userId)
    {
        if (userId <= 0)
        {
            _logger.LogWarning("Attempted to retrieve user with invalid ID: {UserId}", userId);
            return null;
        }

        try
        {
            // Use FindAsync for primary key lookups, which is optimized
            return await _context.Users.FindAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}.", userId);
            throw; // Re-throw to be handled by the service layer
        }
    }

    /// <summary>
    /// Gets all active users, ordered by date created.
    /// </summary>
    /// <returns>A list of active users.</returns>
    public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
    {
        try
        {
            return await _context.Users
                                 .Where(u => u.IsActive)
                                 .OrderByDescending(u => u.DateCreated)
                                 .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all active users.");
            throw;
        }
    }

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="newUser">The user object to add.</param>
    /// <returns>The added User object with the generated ID.</returns>
    public async Task<User> AddAsync(User newUser)
    {
        if (newUser == null)
        {
            _logger.LogWarning("Attempted to add a null user object.");
            throw new ArgumentNullException(nameof(newUser));
        }

        try
        {
            // Basic validation for demonstration
            if (string.IsNullOrWhiteSpace(newUser.Email))
            {
                throw new InvalidOperationException("User email cannot be empty.");
            }
            
            // Check for uniqueness before adding (example of business logic at repository level)
            if (await IsEmailUniqueAsync(newUser.Email))
            {
                 newUser.DateCreated = DateTime.UtcNow;
                 var entityEntry = await _context.Users.AddAsync(newUser);
                 await _context.SaveChangesAsync();

                 _logger.LogInformation("New user added successfully with ID {UserId}.", entityEntry.Entity.Id);
                 return entityEntry.Entity;
            }
            else
            {
                throw new InvalidOperationException($"User with email {newUser.Email} already exists.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding new user: {UserEmail}.", newUser?.Email);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing user's details.
    /// </summary>
    /// <param name="userToUpdate">The detached user object with updated details.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    public async Task<bool> UpdateAsync(User userToUpdate)
    {
        if (userToUpdate == null)
        {
            _logger.LogWarning("Attempted to update with a null user object.");
            return false;
        }

        try
        {
            var existingUser = await _context.Users.FindAsync(userToUpdate.Id);

            if (existingUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update.", userToUpdate.Id);
                return false;
            }

            // Map updated properties
            existingUser.FirstName = userToUpdate.FirstName;
            existingUser.LastName = userToUpdate.LastName;
            existingUser.Email = userToUpdate.Email;
            existingUser.IsActive = userToUpdate.IsActive;
            
            // Context tracks changes, only need to call SaveChanges
            int changes = await _context.SaveChangesAsync();

            if (changes > 0)
            {
                _logger.LogInformation("User with ID {UserId} updated successfully.", userToUpdate.Id);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}.", userToUpdate.Id);
            throw;
        }
    }

    /// <summary>
    /// Soft deletes a user by setting IsActive to false.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>True if the user was successfully marked as inactive, otherwise false.</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        if (userId <= 0) return false;
        
        try
        {
            var existingUser = await _context.Users.FindAsync(userId);
            
            if (existingUser == null)
            {
                _logger.LogWarning("Attempted to delete non-existent user with ID {UserId}.", userId);
                return false;
            }

            if (existingUser.IsActive == false)
            {
                 _logger.LogInformation("User with ID {UserId} already inactive.", userId);
                 return true; // Already deleted
            }

            existingUser.IsActive = false; // Soft delete
            int changes = await _context.SaveChangesAsync();

            if (changes > 0)
            {
                _logger.LogInformation("User with ID {UserId} successfully soft-deleted.", userId);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}.", userId);
            throw;
        }
    }

    /// <summary>
    /// Checks if a given email is unique in the database.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="excludeUserId">Optional: User ID to exclude (for update operations).</param>
    /// <returns>True if the email is unique, false otherwise.</returns>
    public async Task<bool> IsEmailUniqueAsync(string email, int excludeUserId = 0)
    {
        try
        {
            var query = _context.Users.AsQueryable();

            if (excludeUserId > 0)
            {
                query = query.Where(u => u.Id != excludeUserId);
            }

            return !await query.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email uniqueness for {Email}.", email);
            throw;
        }
    }
}

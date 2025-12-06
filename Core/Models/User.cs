using System;

namespace FinanceML.Core.Models
{
    /// <summary>
    /// Represents an application user with immutable identity fields
    /// and safely initialized defaults.
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Primary database identifier.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Unique username chosen by the user.
        /// </summary>
        public string Username { get; init; } = string.Empty;

        /// <summary>
        /// Unique email associated with the user’s account.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Secure hashed password string.
        /// Never store plaintext passwords.
        /// </summary>
        
        /// <summary>
        /// Optional first name of the user.
        /// </summary>
        public string FirstName { get; init; } = string.Empty;

        /// <summary>
        /// Optional last name of the user.
        /// </summary>
        public string LastName { get; init; } = string.Empty;

        /// <summary>
        /// Date the account was created.
        /// </summary>
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Last login timestamp for auditing.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Determines if the user account is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Returns the user's full name for display purposes.
        /// Computed property—no storage.
        /// </summary>
        public string FullName =>
            $"{FirstName} {LastName}".Trim();
    }
}


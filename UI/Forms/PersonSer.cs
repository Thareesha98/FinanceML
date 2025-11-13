using System;

namespace MySimpleApp
{
    /// <summary>
    /// Represents a simple Person object.
    /// This class stores information about an individual.
    /// </summary>
    public class Person
    {
        // --- Properties ---
        // These are data fields that hold the state of the object.
        // { get; set; } allows them to be read from and written to.
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // --- Constructor ---
        /// <summary>
        /// This method is called when a new 'Person' object is created.
        /// It sets the initial values for the properties.
        /// </summary>
        /// <param name="firstName">The person's first name.</param>
        /// <param name="lastName">The person's last name.</param>
        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        // --- Method ---
        /// <summary>
        /// A method that performs an action using the object's data.
        /// </summary>
        /// <returns>The person's full name as a single string.</returns>
        public string GetFullName()
        {
            // Uses string interpolation to combine the properties
            return $"{FirstName} {LastName}";
        }
    }
}

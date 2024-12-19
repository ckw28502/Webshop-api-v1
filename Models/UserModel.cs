using System.ComponentModel.DataAnnotations;
using V1.Attributes;

namespace V1.Models
{
    /// <summary>
    /// Represents a user who accesses the application.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// Automatically generated as a new GUID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the unique username for the user.
        /// The username must be between 3 and 50 characters long.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public required string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// The password must meet a custom set of requirements enforced by the <see cref="PasswordValidationAttribute"/>.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [PasswordValidation(ErrorMessage = "Password must meet the required criteria.")]
        public required string Password { get; set; }

        /// <summary>
        /// Gets or sets the salt for the password hashing.
        /// Stored as a byte array.
        /// </summary>
        [Required(ErrorMessage = "Salt is required.")]
        public required byte[] Salt { get; set; } = [];
    }
}

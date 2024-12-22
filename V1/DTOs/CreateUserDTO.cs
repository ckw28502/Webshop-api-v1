using System.ComponentModel.DataAnnotations;
using V1.Attributes;

namespace V1.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) for creating a new user.
    /// Contains validation rules for user creation input.
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// Gets or sets the unique username for the user.
        /// The username must be between 3 and 50 characters long.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public required string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the user.
        /// The password must meet the following criteria:
        /// - At least 8 characters in length
        /// - Contain at least one uppercase letter
        /// - Contain at least one lowercase letter
        /// - Contain at least one number
        /// </summary>
        [PasswordValidation()]
        public required string Password { get; set; }
    }
}

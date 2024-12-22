using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace V1.Attributes
{
    /// <summary>
    /// Custom validation attribute to enforce password validation.
    /// Validates that the password meets certain security criteria.
    /// </summary>
    public partial class PasswordValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Regex to check for uppercase letters in the password.
        /// </summary>
        [GeneratedRegex(@"[A-Z]")]
        private static partial Regex UppercaseRegex();

        /// <summary>
        /// Regex to check for lowercase letters in the password.
        /// </summary>
        [GeneratedRegex(@"[a-z]")]
        private static partial Regex LowercaseRegex();

        /// <summary>
        /// Regex to check for digits in the password.
        /// </summary>
        [GeneratedRegex(@"\d")]
        private static partial Regex NumberRegex();

        /// <summary>
        /// Validates the password according to the following criteria:
        /// 1. Password must be at least 8 characters long.
        /// 2. Password must contain at least one uppercase letter.
        /// 3. Password must contain at least one lowercase letter.
        /// 4. Password must contain at least one number.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which validation is performed.</param>
        /// <returns>A <see cref="ValidationResult"/> indicating the result of validation.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Ensure the password value is not null or empty
            string? password = value as string;
            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required.");
            }

            // Check if password meets the minimum length requirement
            if (password.Length < 8)
            {
                return new ValidationResult("Password must be at least 8 characters long.");
            }

            // Ensure the password contains at least one uppercase letter
            if (!UppercaseRegex().IsMatch(password))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            // Ensure the password contains at least one lowercase letter
            if (!LowercaseRegex().IsMatch(password))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            // Ensure the password contains at least one number
            if (!NumberRegex().IsMatch(password))
            {
                return new ValidationResult("Password must contain at least one number.");
            }

            // Password passed all validation checks
            return ValidationResult.Success;
        }
    }
}

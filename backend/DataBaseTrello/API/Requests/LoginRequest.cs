using System.ComponentModel.DataAnnotations;

namespace API.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MinLength(5, ErrorMessage = "Email must be at least 5 characters long")]
        [MaxLength(320, ErrorMessage = "Email cannot exceed 320 characters")]
        public required string UserEmail { get; init; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [MaxLength(256, ErrorMessage = "Password cannot exceed 256 characters")]
        public required string UserPassword { get; init; }
    }
}

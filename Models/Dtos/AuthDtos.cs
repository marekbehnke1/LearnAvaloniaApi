using System.ComponentModel.DataAnnotations;

namespace LearnAvaloniaApi.Models.Dtos
{
    // Class to hold all of the information required in a registration request
    public class ApiRegisterRequest
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty ;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }    

    //Class to hold information for a login request
    public class ApiLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Object which carries the server's authorisation response
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message {  get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }

    // Carries the information of the logged in user
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

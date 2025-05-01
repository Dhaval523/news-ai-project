using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class RegisterRequest
    {   
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string Role {get; set;}

    }
}

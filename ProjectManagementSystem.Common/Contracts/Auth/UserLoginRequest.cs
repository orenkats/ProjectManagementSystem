using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Common.Contracts.Auth
{
    public class UserLoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
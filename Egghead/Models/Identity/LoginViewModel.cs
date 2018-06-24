using System.ComponentModel.DataAnnotations;

namespace Egghead.Models.Identity
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public override string ToString()
        {
            return $"Email: {Email} Password: {Password}";
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Egghead.Models.Users
{
    public class LogInModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
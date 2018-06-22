using System.ComponentModel.DataAnnotations;

namespace Egghead.Models
{
    public class SignUpViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(128, ErrorMessage = "Use {0} or more characters with a mix of letters, numbers & symbols", MinimumLength = 8)]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        
        [Required]
        [DataType(DataType.Text)]  
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"Email: {Email} Password: {Password} Fisrt Name: {FirstName} Last Name: {LastName}";
        }
    }
}
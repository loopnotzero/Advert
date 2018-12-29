using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Advert.Common;

namespace Advert.Models.Users
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
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Advert.Models.Account
{
    public class ErrorModel
    {
        public string ReturnUrl { get; set; }
        
        public List<IdentityError> Errors { get; set; }
    }
}
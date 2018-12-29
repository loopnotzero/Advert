﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Advert.Models.Users
{
    public class SignUpModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public string Culture { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(128, ErrorMessage = "Use {0} or more characters with a mix of letters, numbers & symbols", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
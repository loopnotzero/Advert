using System;
using Advert.Common.Profiles;

namespace Advert.Models.Profiles
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }        
        public string Email { get; set; }       
        public string Gender { get; set; }
        public string Location { get; set; }
        public string Birthday { get; set; }
        public string CreatedAt { get; set; }
        public string ImagePath { get; set; }    
        public string PhoneNumber { get; set; } 
    }
}
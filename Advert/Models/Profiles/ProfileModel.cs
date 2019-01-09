using System;
using System.Collections.Generic;

namespace Advert.Models.Profiles
{
    public class ProfileModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        
        public string Email { get; set; }
        public string ImagePath { get; set; }
        
        public string PrimaryPhoneNumber { get; set; }
        
        public List<string> SecondaryPhoneNumbers { get; set; }
    }
}
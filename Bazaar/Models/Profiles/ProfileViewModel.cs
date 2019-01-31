using System;
using System.Collections.Generic;
using Bazaar.Common.Profiles;
using Bazaar.Models.Settings;

namespace Bazaar.Models.Profiles
{
    public class ProfileViewModel
    {
        public bool Owner { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }        
        public string Email { get; set; }       
        public string Gender { get; set; }
        public string Location { get; set; }
        public string Birthday { get; set; }
        public string CreatedAt { get; set; }
        public string ImagePath { get; set; }    
        public string CallingCode { get; set; }
        public string PhoneNumber { get; set; }       
        public IEnumerable<CountryCode> CountryCodes { get; set; }
    }
}
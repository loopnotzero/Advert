using System.Collections.Generic;

namespace Advert.Models.Profiles
{
    public class ProfileErrorModel
    {
        public string ReturnUrl { get; set; }
        
        public List<ProfileError> Errors { get; set; }
    }
}
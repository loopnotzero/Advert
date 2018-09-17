using System.Collections.Generic;
using Egghead.Common.Profiles;

namespace Egghead.Models.Profiles
{
    public class ProfileDescription
    {
        public string ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Headline { get; set; }       
        public long ArticlesCount { get; set; }
        public long FollowingCount { get; set; }
        public List<SocialLink> SocialLinks { get; set; }
    }
}
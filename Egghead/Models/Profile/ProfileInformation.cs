using Egghead.Common.Profile;

namespace Egghead.Models.Profile
{
    public class ProfileInformation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Headline { get; set; }
        public long ArticlesCount { get; set; }
        public long FollowingCount { get; set; }
        public ContactType ContactType { get; set; }
    }
}
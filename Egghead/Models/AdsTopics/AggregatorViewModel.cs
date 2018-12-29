using System.Collections.Generic;
using Egghead.Models.AdsTopics;
using Egghead.Models.Profiles;

namespace Egghead.Models.AdsTopic
{
    public class AggregatorViewModel
    {
        public long BeginPage { get; set; }
        public long EndPage { get; set; }
        public long CurrentPage { get; set; }
        public long LastPage { get; set; }
        public ProfileModel Profile { get; set; }
        public IEnumerable<AdsTopicViewModel> AdsTopics { get; set; }
        public IEnumerable<AdsTopicCommentViewModel> AdsTopicComments { get; set; }   
        public IEnumerable<RecommendedAdsTopicViewModel> RecommendedAdsTopics { get; set; }
    }
}
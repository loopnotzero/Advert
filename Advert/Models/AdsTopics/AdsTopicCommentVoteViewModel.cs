using Advert.Common.AdsTopics;

namespace Advert.Models.AdsTopics
{
    public class AdsTopicCommentVoteViewModel
    {
        public string AdsId { get; set; }
        public string CommentId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
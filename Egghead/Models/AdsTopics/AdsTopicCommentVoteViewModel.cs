using Egghead.Common.AdsTopics;

namespace Egghead.Models.AdsTopics
{
    public class AdsTopicCommentVoteViewModel
    {
        public string AdsId { get; set; }
        public string CommentId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
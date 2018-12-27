using Egghead.Common.Advertisements;

namespace Egghead.Models.Advertisements
{
    public class AdvertisementCommentVoteViewModel
    {
        public string AdsId { get; set; }
        public string CommentId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
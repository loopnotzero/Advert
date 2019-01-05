using Advert.Common.Posts;

namespace Advert.Models.Post
{
    public class PostCommentVoteViewModel
    {
        public string VotesCount { get; set; }
        public VoteType VoteType { get; set; }
    }
}
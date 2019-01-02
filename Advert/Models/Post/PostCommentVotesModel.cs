using Advert.Common.Posts;

namespace Advert.Models.Post
{
    public class PostCommentVotesModel
    {
        public string VotesCount { get; set; }
        public VoteType VoteType { get; set; }
    }
}
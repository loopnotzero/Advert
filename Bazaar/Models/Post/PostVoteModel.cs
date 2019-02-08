using Bazaar.Common.Posts;

namespace Bazaar.Models.Post
{
    public class PostVoteModel
    {
        public string VotesCount { get; set; }
        public VoteType VoteType { get; set; }
    }
}
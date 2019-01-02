using Advert.Common.Posts;

namespace Advert.Models.Post
{
    public class PostVoteViewModel
    {
        public string PostId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
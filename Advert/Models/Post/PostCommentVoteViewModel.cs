using Advert.Common.Posts;

namespace Advert.Models.Post
{
    public class PostCommentVoteViewModel
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
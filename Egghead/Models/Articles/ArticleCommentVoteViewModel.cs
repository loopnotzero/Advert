using Egghead.Common.Articles;

namespace Egghead.Models.Articles
{
    public class ArticleCommentVoteViewModel
    {
        public string CommentId { get; set; }
        public string ArticleId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
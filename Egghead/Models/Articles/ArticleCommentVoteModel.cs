using Egghead.Common.Articles;

namespace Egghead.Models.Articles
{
    public class ArticleCommentVoteModel
    {
        public string CommentId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
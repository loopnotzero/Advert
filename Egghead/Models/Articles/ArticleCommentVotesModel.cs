using Egghead.Common.Articles;

namespace Egghead.Models.Articles
{
    public class ArticleCommentVotesModel
    {
        public string VotesCount { get; set; }
        public VoteType VoteType { get; set; }
    }
}
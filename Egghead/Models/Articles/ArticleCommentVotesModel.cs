using Egghead.Common.Articles;

namespace Egghead.Models.Articles
{
    public class ArticleCommentVotesModel
    {
        public VoteType VoteType { get; set; }
        public string VotesCount { get; set; }
    }
}
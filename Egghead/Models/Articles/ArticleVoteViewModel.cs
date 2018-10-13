using Egghead.Common.Articles;

namespace Egghead.Models.Articles
{
    public class ArticleVoteViewModel
    {
        public string ArticleId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
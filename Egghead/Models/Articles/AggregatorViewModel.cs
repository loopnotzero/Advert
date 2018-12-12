using System.Collections.Generic;
using Egghead.Models.Profiles;

namespace Egghead.Models.Articles
{
    public class AggregatorViewModel
    {
        public ProfileModel Profile { get; set; }
        public IEnumerable<ArticleViewModel> Articles { get; set; }
        public IEnumerable<PopularArticleViewModel> PopularArticles { get; set; }
        public IEnumerable<ArticleCommentViewModel> ArticleComments { get; set; }
        public int PageIndex { get; set; }
        public int PagesCount { get; set; }
    }
}
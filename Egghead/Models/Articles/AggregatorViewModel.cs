using System.Collections.Generic;
using Egghead.Models.Profiles;

namespace Egghead.Models.Articles
{
    public class AggregatorViewModel
    {
        public long BeginPage { get; set; }
        public long EndPage { get; set; }
        public long CurrentPage { get; set; }
        public long LastPage { get; set; }
        public ProfileModel Profile { get; set; }
        public IEnumerable<ArticleViewModel> Articles { get; set; }
        public IEnumerable<PopularArticleViewModel> PopularArticles { get; set; }
        public IEnumerable<ArticleCommentViewModel> ArticleComments { get; set; }       
    }
}
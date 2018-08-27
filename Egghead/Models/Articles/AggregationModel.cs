using System.Collections.Generic;

namespace Egghead.Models.Articles
{
    public class AggregationModel
    {
        public List<ArticleModel> ArticlesPreview { get; set; }
        public List<TopRatedArticleModel> TopRatedArticles { get; set; }
    }
}
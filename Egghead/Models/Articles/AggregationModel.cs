using System.Collections.Generic;

namespace Egghead.Models.Articles
{
    public class AggregationModel
    {
        public List<ArticlePreviewModel> ArticlesPreview { get; set; }
        public List<PopularArticleModel> PopularArticles { get; set; }
    }
}
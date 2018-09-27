using System.Collections.Generic;
using Egghead.Models.Profiles;

namespace Egghead.Models.Articles
{
    public class CompositeArticleModel
    {
        public ProfileModel Profile { get; set; }
        public List<ArticleModel> Articles { get; set; }
    }
}
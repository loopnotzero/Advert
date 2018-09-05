using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Articles
{
    public class MongoDbPopularArticle
    {
        public ObjectId ArticleId { get; set; }
        public int ViewsCount { get; set; }
    }
}
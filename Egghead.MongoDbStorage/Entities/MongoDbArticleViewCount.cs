using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Entities
{
    public class MongoDbArticleViewCount
    {
        public MongoDbArticleViewCount()
        {
            Id = ObjectId.GenerateNewId().ToString();
            //Create indeces
        }

        public string Id { get; set; }
        public string ByWhom { get; set; }
        public string ArticleId { get; set; }
        public string AtWhichTime { get; set; }
    }
}
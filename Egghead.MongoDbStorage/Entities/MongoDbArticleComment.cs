using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Entities
{
    public class MongoDbArticleComment
    {
        public MongoDbArticleComment()
        {
            Id = ObjectId.GenerateNewId().ToString();
            //Create indeces
        }

        public string Id { get; set; }
        public string Text { get; set; }
        public string ByWhom { get; set; }
        public string ArticleId { get; set; }
        public string CreatedAt { get; set; }
        public string ChangedAt { get; set; }
        public string DeletedAt { get; set; }
    }
}
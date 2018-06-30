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
    }
}
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Entities
{
    public class MongoDbArticleViews
    {
        public MongoDbArticleViews()
        {
            Id = ObjectId.GenerateNewId().ToString();
            //Create indeces
        }

        public string Id { get; set; }
    }
}
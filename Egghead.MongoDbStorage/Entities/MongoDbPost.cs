using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Entities
{
    public class MongoDbPost
    {
        public MongoDbPost()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
        
        public string Id { get; set; }
    }
}
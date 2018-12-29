using MongoDB.Bson;

namespace Advert.MongoDbStorage.Roles
{
    public class MongoDbRole
    {
        public MongoDbRole()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
        
        public MongoDbRole(string name) : this()
        {
            Name = name;
        }
        
        public MongoDbRole(string name, string normalizedName) : this()
        {
            Name = name;
            NormalizedName = normalizedName;
        }
        
        public MongoDbRole(string name, string normalizedName, string concurrencyStamp) : this()
        {
            Name = name;
            NormalizedName = normalizedName;
            ConcurrencyStamp = concurrencyStamp;
        }
      
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }
    }
}
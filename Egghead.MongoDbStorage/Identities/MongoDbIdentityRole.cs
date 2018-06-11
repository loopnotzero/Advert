using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Egghead.MongoDbStorage.Identities
{
    public class MongoDbIdentityRole
    {
        public MongoDbIdentityRole()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
        
        public MongoDbIdentityRole(string name) : this()
        {
            Name = name;
        }
        
        public MongoDbIdentityRole(string name, string normalizedName) : this()
        {
            Name = name;
            NormalizedName = normalizedName;
        }
        
        public MongoDbIdentityRole(string name, string normalizedName, string concurrencyStamp) : this()
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
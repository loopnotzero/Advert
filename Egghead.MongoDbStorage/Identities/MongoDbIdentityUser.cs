using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Egghead.MongoDbStorage.Identities
{
    [BsonIgnoreExtraElements]
    public class MongoDbIdentityUser
    {
        private MongoDbIdentityUser()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
        
        public MongoDbIdentityUser(string email) : this()
        {        
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
        
        public bool EmailConfirmed { get; set; }
        
        public string Id { get; private set; }
        
        public string Email { get; private set; }
        
        public string PasswordHash { get; set; }
        
    }
}
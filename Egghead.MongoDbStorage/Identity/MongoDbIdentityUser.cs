using System;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Identity
{
    public class MongoDbIdentityUser
    {
        public MongoDbIdentityUser(string email)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
        
        public string Id { get; private set; }
        
        public string Email { get; private set; }
    }
}
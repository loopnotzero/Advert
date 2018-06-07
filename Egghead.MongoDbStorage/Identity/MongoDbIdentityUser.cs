using System;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Identity
{
    public class MongoDbIdentityUser
    {
        public MongoDbIdentityUser(string email, string password)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));; 
        }
        
        public string Id { get; private set; }
        
        public string Email { get; private set; }
        
        public string Password { get; private set; }
    }
}
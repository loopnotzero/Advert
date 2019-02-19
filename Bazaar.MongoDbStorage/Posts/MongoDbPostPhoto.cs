using System;
using System.Collections.Generic;
using Bazaar.Common.Posts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bazaar.MongoDbStorage.Posts
{
    public class MongoDbPostPhoto : IPostPhoto
    {
        public MongoDbPostPhoto()
        {
            _id = ObjectId.GenerateNewId();
        }
    
        [BsonId] 
        public ObjectId _id { get; set; }       
        public string PhotoPath { get; set; }        
        public string IdentityName { get; set; }
        public ObjectId PostId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? DeletedAt { get; set; }
    }
}
using System;
using Bazaar.Common.Posts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bazaar.MongoDbStorage.Posts
{
    public class MongoDbPostViewsCount : IPostViewsCount
    {
        public MongoDbPostViewsCount()
        {
            _id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        [BsonId]     
        public ObjectId _id { get; set; }        
        public ObjectId PostId { get; set; }
        
        public ObjectId ProfileId { get; set; }
        public DateTime CreatedAt { get; set; }    
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? DeletedAt { get; set; }
    }
}
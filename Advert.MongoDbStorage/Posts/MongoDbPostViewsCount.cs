using System;
using Advert.Common.Posts;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.Posts
{
    public class MongoDbPostViewsCount : IPostViewsCount
    {
        public MongoDbPostViewsCount()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public ObjectId Id { get; set; }    
        public ObjectId PostId { get; set; }  
        public DateTime CreatedAt { get; set; }      
        public DateTime UdpatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
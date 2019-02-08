using System;
using Bazaar.Common.Posts;
using Bazaar.Common.Profiles;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bazaar.MongoDbStorage.Profiles
{
    public class MongoDbProfilePhoto : IProfilePhoto
    {
        public MongoDbProfilePhoto()
        {
            _id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public ObjectId _id { get; set; }
        public ObjectId ProfileId { get; set; }  
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
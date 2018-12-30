using System;
using Advert.Common.Profiles;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Advert.MongoDbStorage.Profiles
{
    public class MongoDbProfileImage : IProfileImage
    {
        public MongoDbProfileImage()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public ObjectId Id { get; set; }
        public ObjectId ProfileId { get; set; }  
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Egghead.MongoDbStorage.Profiles
{
    public class MongoDbProfilePhoto
    {
        public MongoDbProfilePhoto()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public string PhotoPath { get; set; }     
        public ObjectId Id { get; set; }
        public ObjectId ProfileId { get; set; }       
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
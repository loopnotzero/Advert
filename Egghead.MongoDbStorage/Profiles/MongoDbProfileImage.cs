using System;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Profiles
{
    public class MongoDbProfileImage
    {
        public MongoDbProfileImage()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }

        public ObjectId Id { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
    }
}
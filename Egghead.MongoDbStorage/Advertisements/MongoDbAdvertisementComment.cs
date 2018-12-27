using System;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Advertisements
{
    public class MongoDbAdvertisementComment
    {
        public MongoDbAdvertisementComment()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public long VotesCount { get; set; }
        public string Text { get; set; }  
        public string ProfileName { get; set; }
        public string ProfileImagePath { get; set; }
        public ObjectId Id { get; set; }
        public ObjectId ReplyTo { get; set; }
        public ObjectId ProfileId { get; set; }
        public ObjectId AdsId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
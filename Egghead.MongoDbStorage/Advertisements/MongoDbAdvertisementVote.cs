using System;
using Egghead.Common.Advertisements;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Advertisements
{
    public class MongoDbAdvertisementVote
    {
        public MongoDbAdvertisementVote()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public ObjectId Id { get; set; }  
        public ObjectId AdsId { get; set; }     
        public VoteType VoteType { get; set; }   
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
using System;
using Advert.Common.AdsTopic;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.AdsTopics
{
    public class MongoDbAdsTopicViewsCount : IAdsTopicViewsCount
    {
        public MongoDbAdsTopicViewsCount()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public ObjectId Id { get; set; }    
        public ObjectId AdsId { get; set; }  
        public DateTime CreatedAt { get; set; }      
        public DateTime UdpatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
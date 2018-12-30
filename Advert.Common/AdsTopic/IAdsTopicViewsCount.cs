using System;
using MongoDB.Bson;

namespace Advert.Common.AdsTopic
{
    public interface IAdsTopicViewsCount
    {
        string Email { get; set; }
        string NormalizedEmail { get; set; }
        ObjectId Id { get; set; }    
        ObjectId AdsId { get; set; }  
        DateTime CreatedAt { get; set; }      
        DateTime UdpatedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}
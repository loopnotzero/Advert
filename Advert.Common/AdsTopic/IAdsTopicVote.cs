using System;
using Advert.Common.AdsTopics;
using MongoDB.Bson;

namespace Advert.Common.AdsTopic
{
    public interface IAdsTopicVote
    {
        string Email { get; set; }
        string NormalizedEmail { get; set; }
        ObjectId Id { get; set; }
        ObjectId AdsId { get; set; }
        VoteType VoteType { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}
using System;
using Advert.Common.AdsTopics;
using MongoDB.Bson;

namespace Advert.Common.AdsTopic
{
    public interface IAdsTopicCommentVote
    {
        ObjectId Id { get; set; }
        ObjectId AdsId { get; set; }      
        ObjectId CommentId { get; set; }  
        ObjectId ProfileId { get; set; }
        VoteType VoteType { get; set; }       
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}
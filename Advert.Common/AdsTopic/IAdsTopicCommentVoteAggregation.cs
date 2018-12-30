using Advert.Common.AdsTopics;
using MongoDB.Bson;

namespace Advert.Common.AdsTopic
{
    public interface IAdsTopicCommentVoteAggregation
    {
        long VotesCount { get; set; }
        ObjectId Id { get; set; }
        VoteType VoteType { get; set; }
    }
}
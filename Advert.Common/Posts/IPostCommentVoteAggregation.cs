using Advert.Common.Posts;
using MongoDB.Bson;

namespace Advert.Common.Posts
{
    public interface IPostCommentVoteAggregation
    {
        long VotesCount { get; set; }
        ObjectId Id { get; set; }
        VoteType VoteType { get; set; }
    }
}
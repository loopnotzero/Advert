using System;
using Bazaar.Common.Posts;
using MongoDB.Bson;

namespace Bazaar.Common.Posts
{
    public interface IPostVote
    {
        ObjectId _id { get; set; }
        ObjectId PostId { get; set; }
        ObjectId ProfileId { get; set; }
        VoteType VoteType { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
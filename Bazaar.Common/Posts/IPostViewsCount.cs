using System;
using MongoDB.Bson;

namespace Bazaar.Common.Posts
{
    public interface IPostViewsCount
    {
        ObjectId _id { get; set; }    
        string IdentityName { get; set; }
        ObjectId PostId { get; set; }  
        DateTime CreatedAt { get; set; }      
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
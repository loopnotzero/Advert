using System;
using MongoDB.Bson;

namespace Advert.Common.Posts
{
    public interface IPostViewsCount
    {
        ObjectId _id { get; set; }    
        ObjectId PostId { get; set; }  
        ObjectId ProfileId { get; set; }
        DateTime CreatedAt { get; set; }      
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
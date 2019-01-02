using System;
using MongoDB.Bson;

namespace Advert.Common.Posts
{
    public interface IPostViewsCount
    {
        string Email { get; set; }
        string NormalizedEmail { get; set; }
        ObjectId Id { get; set; }    
        ObjectId PostId { get; set; }  
        DateTime CreatedAt { get; set; }      
        DateTime UdpatedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}
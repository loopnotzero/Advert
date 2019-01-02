using System;
using MongoDB.Bson;

namespace Advert.Common.Posts
{
    public interface IProfile
    {
        string Name { get; set; }    
        string Email { get; set; }
        string Culture { get; set; }
        string Location { get; set; }
        string ImagePath { get; set; }
        string PhoneNumber { get; set; }  
        string NormalizedName { get; set; }
        string NormalizedEmail { get; set; }
        string LocationNormalized { get; set; }          
        ObjectId Id { get; set; }           
        DateTime CreatedAt { get; set; }
        DateTime ChangedAt { get; set; }  
        DateTime DeletedAt { get; set; }    
    }
}
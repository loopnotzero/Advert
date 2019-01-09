using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Advert.Common.Posts
{
    public interface IProfile
    {
        ObjectId _id { get; set; }   
        bool IsDeleted { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Culture { get; set; }
        string Location { get; set; }
        string ImagePath { get; set; }
        string NormalizedName { get; set; }
        string NormalizedEmail { get; set; }
        string PrimaryPhoneNumber { get; set; }  
        DateTime CreatedAt { get; set; }
        DateTime ChangedAt { get; set; }  
        DateTime DeletedAt { get; set; } 
        List<string> SecondaryPhoneNumbers { get; set; }
    }
}
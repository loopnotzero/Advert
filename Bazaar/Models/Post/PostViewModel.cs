using System.Collections.Generic;

namespace Bazaar.Models.Post
{
    public class PostViewModel
    {     
        public bool Hidden { get; set; }
        public bool IsOwner { get; set; }
        public bool IsVoted { get; set; }
        public string PostId { get; set; }
        public string Text { get; set; }
        public string Title { get; set; } 
        public string Currency { get; set; }
        public string Location { get; set; }
        public string CreatedAt { get; set; }
        public string ViewsCount { get; set; }
        public string LikesCount { get; set; }
        public string SharesCount { get; set; }
        public string CommentsCount { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImagePath { get; set; }   
        public decimal Price { get; set; }
        public List<string> Tags { get; set; }
        
        public List<string> PostPhotos { get; set; }
    }
}
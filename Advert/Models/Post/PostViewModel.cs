using System.Collections.Generic;

namespace Advert.Models.Post
{
    public class PostViewModel
    {           
        public bool IsTopicOwner { get; set; }
        public long Price { get; set; }
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
        public List<string> Tags { get; set; }
    }
}
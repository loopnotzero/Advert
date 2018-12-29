namespace Egghead.Models.Advertisements
{
    public class AdvertisementViewModel
    {    
        public string AdsId { get; set; }
        public string Text { get; set; }
        public string Title { get; set; } 
        public string CreatedAt { get; set; }
        public string ViewsCount { get; set; }
        public string LikesCount { get; set; }
        public string SharesCount { get; set; }
        public string CommentsCount { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImagePath { get; set; }   
        
        public bool IsTopicOwner { get; set; }
    }
}
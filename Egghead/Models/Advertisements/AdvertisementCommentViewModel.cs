using System.Collections.Generic;

namespace Egghead.Models.Advertisements
{
    public class AdvertisementCommentViewModel
    {
        public string AdsId { get; set; }
        public string Text { get; set; }
        public string ReplyTo { get; set; }
        public string CreatedAt { get; set; }    
        public string CommentId { get; set; }        
        public string VotesCount { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImagePath { get; set; }
        public List<AdvertisementCommentViewModel> Comments { get; set; }
    }
}
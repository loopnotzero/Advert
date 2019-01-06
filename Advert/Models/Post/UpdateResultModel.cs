namespace Advert.Models.Post
{
    public class UpdateResultModel
    {
        public long MatchedCount { get; set; }
        public long ModifiedCount { get; set; }
        public bool IsAcknowledged { get; set; }
        public bool IsModifiedCountAvailable { get; set; }
    }
}
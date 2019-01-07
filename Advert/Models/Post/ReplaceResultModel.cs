namespace Advert.Models.Post
{
    public class ReplaceResultModel
    {
        public long MatchedCount { get; set; }
        public long ModifiedCount { get; set; }
        public bool IsAcknowledged { get; set; }
        public bool IsModifiedCountAvailable { get; set; }
    }
}
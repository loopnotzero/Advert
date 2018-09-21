namespace Egghead.Utils
{
    public class Metrics
    {
        public double GetApprovalRating(double likesCount, double viewsCount)
        {
            return likesCount / viewsCount;
        } 
          
        public double GetDiscussionRating(double commentsCount, double articlesCount)
        {
            return commentsCount / articlesCount;
        }
               
        public double GetAudienceEngagment(double likesCount, double dislikesCount, double commentsCount, double articleShares, double viewsCount)
        {
            return (likesCount / dislikesCount + commentsCount + articleShares) / viewsCount;
        } 
        
        public double GetAverageViewsCount(double likesCount, double articlesCount)
        {
            return likesCount / articlesCount;
        }
        
        public double GetDistributionRating(double articlesShares, double viewsCount)
        {
            return articlesShares / viewsCount;
        }
    }
}
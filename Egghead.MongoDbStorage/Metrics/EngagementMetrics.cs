namespace Egghead.MongoDbStorage.Metrics
{
    public static class EngagementMetrics
    {
        public static double ComputeApprovalRating(long likesCount, long viewsCount)
        {
            return (double)likesCount / (double)viewsCount;
        } 
          
        public static double ComputeDiscussionRating(long commentsCount, long articlesCount)
        {
            return (double)commentsCount / (double)articlesCount;
        }
               
        public static double ComputeAudienceEngagment(long likesCount, long dislikesCount, long commentsCount, long sharesCount, long viewsCount)
        {
            return ((double)likesCount / (double)dislikesCount + commentsCount + sharesCount) / (double)viewsCount;
        } 
        
        public static double ComputeAverageViewsCount(long likesCount, long articlesCount)
        {
            return (double)likesCount / (double)articlesCount;
        }
        
        public static double ComputeDistributionRating(long articlesShares, long viewsCount)
        {
            return (double)articlesShares / (double)viewsCount;
        }


        public static double ComputeViewsCount(long viewsCount)
        {
            return (double) viewsCount;
        }
    }
}
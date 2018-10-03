namespace Egghead.Common.Metrics
{
    public class EngagementRate
    {
        public static double ComputeApprovalRate(double likesCount, double viewsCount)
        {
            return likesCount / viewsCount;
        } 
          
        public static double ComputeDiscussionRate(double commentsCount, double articlesCount)
        {
            return commentsCount / articlesCount;
        }
               
        public static double ComputeEngagementRate(double likesCount, double dislikesCount, double sharesCount, double commentsCount, double viewsCount)
        {
            if (dislikesCount > 0)
            {
                return (likesCount / dislikesCount + commentsCount + sharesCount) / (viewsCount > 0 ? viewsCount : 1);
            }

            return (likesCount + commentsCount + sharesCount) / (viewsCount > 0 ? viewsCount : 1);
        }
        
        public static double ComputeDistributionRate(double shares, double viewsCount)
        {
            return shares / viewsCount;
        }

        public static double ComputeAverageViewsCount(double likesCount, double articlesCount)
        {
            return likesCount / articlesCount;
        }       
    }
}
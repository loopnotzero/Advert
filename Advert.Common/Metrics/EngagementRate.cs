namespace Advert.Common.Metrics
{
    public class EngagementRate
    {
        public static double ComputeApprovalRate(double likesCount, double viewsCount)
        {
            return likesCount / viewsCount;
        } 
          
        public static double ComputeDiscussionRate(double commentsCount, double postsCount)
        {
            return commentsCount / postsCount;
        }

        public static double ComputeEngagementRate(double likesCount, double sharesCount, double commentsCount, double viewsCount)
        {
            if (likesCount > 0)
            {
                var likesPercent = likesCount / 100;

                var engagementRate = (likesCount + commentsCount + viewsCount) * likesPercent;

                return engagementRate;
            }
            else
            {
                var engagementRate = (likesCount + commentsCount + viewsCount) / 100;
               
                return engagementRate;
            }
        }


        public static double ComputeDistributionRate(double shares, double viewsCount)
        {
            return shares / viewsCount;
        }

        public static double ComputeAverageViewsCount(double likesCount, double postsCount)
        {
            return likesCount / postsCount;
        }       
    }
}
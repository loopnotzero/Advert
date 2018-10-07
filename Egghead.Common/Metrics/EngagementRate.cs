using System;
using MongoDB.Bson.Serialization.Serializers;

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
            //todo: Decrease likes percent if user was not made a move to article
            //todo: Increase percent of engagement rate if user shared an article
            //todo: Decrease percent of engagement rate if user didn't spent time to read article
            //todo: Estimate approximate time for reading article

            if (likesCount > 0)
            {
                var likesPercent = likesCount / 100;

                var engagementRate = (likesCount + commentsCount + viewsCount) * likesPercent;

                if (dislikesCount > 0)
                {
                    return engagementRate * (dislikesCount / 100);
                }

                return engagementRate;
            }
            else
            {
                var engagementRate = (likesCount + commentsCount + viewsCount) / 100;

                if (dislikesCount > 0)
                {
                    return engagementRate * (dislikesCount / 100);
                }

                return engagementRate;
            }
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
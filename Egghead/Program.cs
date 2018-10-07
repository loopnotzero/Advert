using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Egghead.Common.Metrics;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Egghead
{
    public static class Program
    {
//        private const int Like = 0;
//        private const int Dislike = 1;
//        private const int Comment = 2;
//        private const int ViewCount = 3;
        
        public static void Main(string[] args)
        {
//            var matrix = new List<List<double>>
//            {
//                new List<double>
//                {
//                    1, 50, 10, 15075
//                },
//                new List<double>
//                {
//                    42, 7, 15, 24
//                },
//                new List<double>
//                {
//                    1, 1, 11, 90
//                },
//                new List<double>
//                {
//                    21, 11, 11, 90
//                },
//                new List<double>
//                {
//                    1440, 40, 11, 90
//                },
//                new List<double>
//                {
//                    0, 1, 0, 4
//                },
//                new List<double>
//                {
//                    0, 0, 0, 3
//                }
//            };
//
//            foreach (var list in matrix)
//            {
//                
//                Console.WriteLine($"ER: {EngagementRate.ComputeEngagementRate(list[Like], list[Dislike], 0, list[Comment], list[ViewCount])}");           
//            }
            
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()        
                .Build();         
            return builder;
        }
    }
}
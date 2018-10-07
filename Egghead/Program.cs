using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Egghead
{
    public static class Program
    {
        private const int Like = 0;
        private const int Dislike = 1;
        private const int Comment = 2;
        private const int ViewCount = 3;
        
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
//                }
//            };

//            var maxDislikes = matrix.Max(x => x[Dislike]);

//            foreach (var list in matrix)
//            {
//                var likesPercent = list[Like] / 100;
//                var dislikePercent = list[Dislike] / 100;
//                Console.WriteLine($"Likes: {likesPercent}%  Dislikes: {dislikePercent}$, Final ER: {(list[Like] + list[Comment] + list[ViewCount]) * likesPercent / dislikePercent}");           
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
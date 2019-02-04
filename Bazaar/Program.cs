using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Bazaar
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
//                .UseKestrel(options =>
//                {
//                    options.Listen(IPAddress.Parse("192.168.0.50"), 5000);
//                })
                .Build();
            return builder;
        }
    }
}
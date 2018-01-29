using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace Trackable.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build()
                .Run();
        }
    }
}

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ExchangeRates.Service
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var web_host = CreateWebHostBuilder(args).Build())
            {
                web_host.Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

using AWS.Equinox.ApiClient.Configuration;
using AWS.Equinox.ApiClient.Handler;
using AWS.Equinox.Infastracture.MiddleWare.Debug;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;

namespace AWS.Equinox.ApiClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
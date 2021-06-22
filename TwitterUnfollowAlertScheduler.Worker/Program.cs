using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitterUnfollowAlertScheduler.Worker.Configs;
using TwitterUnfollowAlertScheduler.Worker.Services;

namespace TwitterUnfollowAlertScheduler.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    AuthConfig authConfig = configuration.GetSection("App.Configurations:AuthConfig").Get<AuthConfig>();
                    services.AddSingleton(authConfig);
                    TweetConfig tweetConfig = configuration.GetSection("App.Configurations:TweetConfig").Get<TweetConfig>();
                    services.AddSingleton(tweetConfig);

                    services.AddSingleton<ITwitterService, TwitterService>();

                    services.AddHostedService<Worker>();
                });
    }
}

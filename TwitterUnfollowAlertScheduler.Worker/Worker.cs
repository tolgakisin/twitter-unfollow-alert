using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using TwitterUnfollowAlertScheduler.Worker.Configs;
using TwitterUnfollowAlertScheduler.Worker.Models;
using TwitterUnfollowAlertScheduler.Worker.Services;

namespace TwitterUnfollowAlertScheduler.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITwitterService _twitterService;
        private readonly TweetConfig _tweetConfig;

        public Worker(ILogger<Worker> logger, ITwitterService twitterService, TweetConfig tweetConfig)
        {
            _logger = logger;
            _twitterService = twitterService;
            _tweetConfig = tweetConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            User currentUser = await _twitterService.GetCurrentUserAsync();
            string _previousUserIds = String.Empty;
            UserResponse _previousUserResponse = new UserResponse();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    var (PreviousUserIds, PreviousUserResponse) = await InitializeJobAsync(currentUser, _previousUserIds, _previousUserResponse);
                    _previousUserIds = PreviousUserIds;
                    _previousUserResponse = PreviousUserResponse;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(Convert.ToDouble(_tweetConfig.ScheduleTimeAsMinute)), stoppingToken);
            }
        }

        public async Task<(string PreviousUserIds, UserResponse PreviousUserResponse)> InitializeJobAsync(User currentUser, string previousUserIds, UserResponse previousUserResponse)
        {
            var currentUserResponse = await _twitterService.GetFollowerIdsAsync();

            var currentIds = String.Join(",", currentUserResponse.UserIds.OrderBy(x => x));
            if (!String.IsNullOrEmpty(previousUserIds) && !String.IsNullOrEmpty(currentIds) && previousUserIds != currentIds)
            {
                _logger.LogInformation($"Current Followers: {currentUserResponse.TotalCount} - Previous Followers: {previousUserResponse.TotalCount}");
                foreach (long previousUserId in previousUserResponse.UserIds)
                {
                    bool isFollow = currentUserResponse.UserIds.Any(currentUserId => currentUserId == previousUserId);
                    if (!isFollow)
                    {
                        User previousUser = await _twitterService.GetUserByIdAsync(previousUserId);
                        string message = $"@{previousUser.ScreenName} unfollowed you.";
                        _logger.LogInformation(message);

                        await _twitterService.SendDMAsync(message, currentUser.Id);
                    }
                }
            }

            return (currentIds, currentUserResponse);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using TwitterUnfollowAlertScheduler.Worker.Configs;
using TwitterUnfollowAlertScheduler.Worker.Models;

namespace TwitterUnfollowAlertScheduler.Worker.Services
{
    public class TwitterService : ITwitterService
    {
        private readonly AuthConfig _authConfig;
        private readonly TweetConfig _tweetConfig;
        private readonly TwitterClient _userClient;

        public TwitterService(AuthConfig authConfig, TweetConfig tweetConfig)
        {
            _authConfig = authConfig;
            _tweetConfig = tweetConfig;
            _userClient = new TwitterClient(_authConfig.CONSUMER_KEY, _authConfig.CONSUMER_KEY_SECRET, _authConfig.ACCESS_TOKEN, _authConfig.ACCESS_TOKEN_SECRET);
        }

        public async Task<UserResponse> GetFollowerIdsAsync()
        {
            try
            {
                var currentUserResponse = new UserResponse();
                string cursor = "-1";
                while (cursor != "0")
                {
                    var twitterResult = await _userClient.Execute.RequestAsync(request =>
                    {
                        request.Url = $"https://api.twitter.com/1.1/followers/ids.json?cursor={cursor}&count={_tweetConfig.TweetCountPerRequest}&screen_name={_tweetConfig.Username}&skip_status=true&include_user_entities=false";
                        request.HttpMethod = HttpMethod.GET;
                    });
                    string jsonResponse = twitterResult.Content;
                    UserResponse response = JsonConvert.DeserializeObject<UserResponse>(jsonResponse);
                    currentUserResponse.UserIds.AddRange(response.UserIds);
                    currentUserResponse.TotalCount += response.UserIds.Count;
                    cursor = response.NextCursor.ToString();
                }

                return currentUserResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            try
            {
                IUser user = await _userClient.Users.GetUserAsync(id);
                if (user == null)
                {
                    return new User();
                }

                return new User
                {
                    Id = user.Id,
                    IdStr = user.IdStr,
                    Name = user.Name,
                    ScreenName = user.ScreenName
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            try
            {
                IUser user = await _userClient.Users.GetUserAsync(_tweetConfig.Username);
                if (user == null)
                {
                    return new User();
                }

                return new User
                {
                    Id = user.Id,
                    IdStr = user.IdStr,
                    Name = user.Name,
                    ScreenName = user.ScreenName
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendDMAsync(string message, long id)
        {
            try
            {
                await _userClient.Messages.PublishMessageAsync(message, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

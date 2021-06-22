using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterUnfollowAlertScheduler.Worker.Models;

namespace TwitterUnfollowAlertScheduler.Worker.Services
{
    public interface ITwitterService
    {
        Task<User> GetUserByIdAsync(long id);
        Task<User> GetCurrentUserAsync();
        Task<UserResponse> GetFollowerIdsAsync();
        Task SendDMAsync(string message, long id);
    }
}

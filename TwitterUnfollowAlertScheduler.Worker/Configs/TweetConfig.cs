
namespace TwitterUnfollowAlertScheduler.Worker.Configs
{
    public class TweetConfig
    {
        public string Username { get; set; }
        public string TweetCountPerRequest { get; set; }
        public string ScheduleTimeAsMinute { get; set; }
    }
}

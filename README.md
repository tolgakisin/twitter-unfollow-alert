# twitter-unfollow-alert
Unfollow Alert is an application that notifies you when someone unfollows you.
# SETUP
You will need to edit AppSettings.json for Twitter Authentication Tokens.
```json
  "App.Configurations": {
    "AuthConfig": {
      "CONSUMER_KEY": "YOUR CONSUMER KEY",
      "CONSUMER_KEY_SECRET": "YOUR CONSUMER KEY SECRET",
      "ACCESS_TOKEN": "YOUR ACCESS TOKEN",
      "ACCESS_TOKEN_SECRET": "YOUR ACCESS TOKEN SECRET"
    },
    "TweetConfig": {
      "Username": "tolgakisin",
      "TweetCountPerRequest": "5000",
      "ScheduleTimeAsMinute": "5"
    }
  }
```
* Username: Your Twitter username
* TweetCountPerRequest: Followers count per request
* ScheduleTimeAsMinute: Schedule Time as minute for the service to run

# BACKGROUND SERVICE
If you want to start this app as a Linux Daemon, you need to add ```UseSystemd()``` to Program.cs

```cs
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
```

# PUBLISH
<pre>dotnet publish -o artifact</pre>

## Linux
To promote the application as a Daemon, create the "twitter-unfollow-alert-worker.service" file under the "/etc/systemd/system" directory.
```
[Unit]
Description=Twitter Unfollow Alert Worker

[Service]
Type=notify

WorkingDirectory=/srv/TwitterUnfollowAlertScheduler.Worker

ExecStart=/usr/bin/dotnet /srv/TwitterUnfollowAlertScheduler.Worker/TwitterUnfollowAlertScheduler.Worker.dll
SyslogIdentifier=Worker
User=root
Restart=always
RestartSec=5

Environment=DOTNET_ROOT=/opt/rh/rh-dotnet31/root/usr/lib64/dotnet

[Install]
WantedBy=multi-user.target
```

When the .service file is prepared, the service is enabled by running the ```systemctl daemon-reload``` command

#### Run ```systemctl status twitter-unfollow-alert-worker.service``` to get information about the status of the service
![Unfollow Alert Status](https://i.imgur.com/MWx8LPQ.jpeg)

# DEMO
![Unfollow Alert Unfollowers](https://i.imgur.com/FcdB1lw.jpeg)

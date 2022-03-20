using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Twitch.Net.Api;
using Twitch.Net.Api.Client;
using Twitch.Net.Client;
using Twitch.Net.Client.Client;
using Twitch.Net.PubSub;
using Twitch.Net.PubSub.Client;
using Twitch.Net.PubSub.Events;

namespace Twitch.Net.Samples;

public class TwitchNetSamples
{
    private TwitchNetSamples()
    {
        Task.Run(async () => await Run());
        Thread.Sleep(Timeout.Infinite);
    }

    private async Task Run()
    {
        // If you are doing this in a webapp you'll not have to do this part, since this is only required if you
        // are not using the default "startup" flow of webserver
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // And this would be something you just do with ".Configure<T>()" with the service collection
        var config = new CustomTwitchConfig();
        configuration.GetSection("Twitch").Bind(config);
            
        // Since we are in a console app, we will just create this manually
        // there is also a TwitchLibFactory, but it is if you want everything (event sub included)
        // but in this sample it wont be here, as that will be part of the "event sub sample instead"
        var collection = new ServiceCollection()
            .AddTwitchIrcClient(cfg =>
                {
                    cfg.Username = config.Username;
                    cfg.OAuth = config.OAuth;
                },
                cfg =>
                {
                    cfg.ClientId = config.ClientId;
                    cfg.ClientSecret = config.ClientSecret;
                })
            .AddTwitchPubSubClient(cfg =>
            {
                cfg.OAuth = config.OAuth;
            })
            .AddTwitchApiClient(cfg =>
            {
                cfg.ClientId = config.ClientId;
                cfg.ClientSecret = config.ClientSecret;
            });
            
        var provider = collection.BuildServiceProvider();
        var api = provider.GetService<IApiClient>();
        var irc = provider.GetService<IIrcClient>();
        var pubSub = provider.GetService<IPubSubClient>();

        // sample of api client
        var users = await api.Helix.Users.GetUsersAsync(logins: new List<string> {"ixyles", "ixylesbot"});

        irc.Events.OnIrcConnected += () =>
        {
            Console.WriteLine("[IRC] Connected");
            return Task.CompletedTask;
        };

        // Event listening - which are sent as "tasks" so you can easily async/await the event instead of
        // having to do a manual Task.Run() for doing async executions as example API / Database, you name it.
        pubSub.Events.OnPubSubConnected += () =>
        {
            Console.WriteLine("[PUBSUB] Connected");
            OnPubSubConnected(pubSub, config.OAuth, long.Parse(users.Users.First().Id));
            return Task.CompletedTask;
        };
        pubSub.Events.OnPubSubDisconnect += msg =>
        {
            Console.WriteLine($"[PUBSUB] Disconnected - {msg.Message}");
            return Task.CompletedTask;
        };
        pubSub.Events.OnPubSubReconnect += () =>
        {
            Console.WriteLine("[PUBSUB] Reconnected");
            return Task.CompletedTask;
        };
        pubSub.Events.OnCustomRedeemEvent += PubSubOnRedeemEvent;

        irc.Events.OnIrcConnected += () =>
        {
            Console.WriteLine("Connected to twitch..");
            return Task.CompletedTask;
        };
        irc.Events.OnTwitchAuthenticated += auth =>
        {
            Console.WriteLine($"Twitch authenticated: {auth.Username}");
            irc.JoinChannel(config.BaseChannel);
            return Task.CompletedTask;
        };
        irc.Events.OnJoinedChannel += channel =>
        {
            Console.WriteLine($"Joined chat channel: {channel.Channel.ChannelName}");
            return Task.CompletedTask;
        };
        irc.Events.OnLeftChannel += channel =>
        {
            Console.WriteLine($"Left chat channel: {channel.Channel.ChannelName}");
            return Task.CompletedTask;
        };
        irc.Events.OnChatMessage += message =>
        {
            Console.WriteLine($"[{message.Channel}] {message.DisplayName} : {message.Message} - {message.IsModerator} - {message.IsSubscriber}");
            return Task.CompletedTask;
        };
            
        // if you wanna make sure the pubsub client initial connects goes through, just while await
        // this is because since the initial connection will not "retry", only after first establishment
        while (!await pubSub.ConnectAsync())
            Console.WriteLine("[PUBSUB] Failed initial connecting, retrying...");

        while (!await irc.ConnectAsync())
            Console.WriteLine("[IRC] Failed initial connecting, retrying...");
    }

    private Task PubSubOnRedeemEvent(CommunityPointsEvent arg)
    {
        Console.WriteLine($"ON REDEEM EVENT FIRE - TYPE: {arg.Type} - {arg.Data.Reward.Id} - {arg.Data.Reward.Status} - {arg.Data.Reward.Reward.Title} - {arg.Data.Reward.UserInput}");
        return Task.CompletedTask;
    }

    private void OnPubSubConnected(
        IPubSubClient pubSub, 
        string oauth,
        long userId
        ) => 
        pubSub.CreateBuilder()
            .CreateChannelPointsRedeemTopic(userId)
            .Listen(oauth);

    private static void Main() => _ = new TwitchNetSamples();
}
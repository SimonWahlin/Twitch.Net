using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Twitch.Net.Api.Client;
using Twitch.Net.Client.Client;
using Twitch.Net.PubSub.Client;
using Twitch.Net.PubSub.Events;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Credential;
using Twitch.Net.Shared.RateLimits;

namespace Twitch.Net.Samples
{
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
            var config = new TwitchCredentialConfiguration(); // this is just to make it convenient for anyone who just wants a pre-defined setup
            configuration.GetSection("Twitch").Bind(config);
            
            // In a real app we would use a service collection for everything, but it was easier to just make 
            // an easy sample like this to test with
            var collection = new ServiceCollection()
                .AddHttpClient();
            var provider = collection.BuildServiceProvider();
            var factory = provider.GetService<IHttpClientFactory>();
            var userAccountResolver = new UserAccountResolver(factory, config);

            var userAccount = await userAccountResolver.TryResolveUserAccountStatusOrDefaultAsync();
            var api = ApiClientFactory.CreateClient(config, factory, new ClientCredentialTokenResolver(factory, config));
            var users = await api.Helix.Users.GetUsersAsync(logins: new List<string> {"ixyles"});
            var pubSub = PubSubClientFactory.CreateClient();
            var irc = IrcClientFactory.CreateClient(config, userAccountStatus: userAccount);

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
                OnPubSubConnected(pubSub, config, long.Parse(users.Users.First().Id));
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
                OnPubSubConnected(pubSub, config, long.Parse(users.Users.First().Id));
                return Task.CompletedTask;
            };
            pubSub.Events.OnCustomRedeemEvent += PubSubOnRedeemEvent;

            irc.Events.OnIrcConnected += () =>
            {
                irc.JoinChannel(config.BaseChannel);
                return Task.CompletedTask;
            };
            irc.Events.OnJoinedChannel += channel =>
            {
                irc.SendMessage(channel.Channel, "Hello! I AM HEREEEE!");
                return Task.CompletedTask;
            };
            irc.Events.OnChatMessage += message =>
            {
                Console.WriteLine($"[{message.Channel}] {message.DisplayName} : {message.Message}");
                return Task.CompletedTask;
            };
            
            // if you wanna make sure the pubsub client initial connects
            // you can just do it in a while loop, since the initial connection will not "retry", only reconnects
            while (!await pubSub.ConnectAsync())
                Console.WriteLine("[PUBSUB] Failed initial connecting, retrying...");

            while (!await irc.ConnectAsync())
                Console.WriteLine("[IRC] Failed initial connecting, retrying...");
        }

        private Task PubSubOnRedeemEvent(CommunityPointsEvent arg)
        {
            Console.WriteLine($"ON REDEEM EVENT FIRE - TYPE: {arg.Type}");
            return Task.CompletedTask;
        }

        private void OnPubSubConnected(
            IPubSubClient pubSub, 
            IPubSubCredentialConfiguration config,
            long userId
            ) => 
            pubSub.CreateBuilder()
                .CreateChannelPointsRedeemTopic(userId)
                .Listen(config.OAuth);

        private static void Main() => _ = new TwitchNetSamples();
    }
}
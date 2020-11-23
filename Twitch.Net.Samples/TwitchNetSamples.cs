using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Twitch.Net.Client.Client;
using Twitch.Net.Client.Models;
using Twitch.Net.Communication.Clients;
using Twitch.Net.PubSub.Client;
using Twitch.Net.PubSub.Events;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Credential;
using Twitch.Net.Shared.RateLimits;

namespace Twitch.Net.Samples
{
    public class TwitchNetSamples
    {
        private const long UserId = 137132000; // Temporary to use for pubsub (later on this would be resolved by API)
        
        private TwitchNetSamples()
        {
            Task.Run(async () => await Run());
            ListenToInput();
        }

        private async Task Run()
        {
                        // If you are doing this in a webapp you'll not have to do this part, since this is only required if you
            // are not using the default "startup" flow of webserver
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // And this would be something you just do with ".Configure<T>()" with the servicecollection
            var config = new TwitchCredentialConfiguration();
            configuration.GetSection("Twitch").Bind(config);
            
            // In a real app we would use a service collection for everything, but it was easier to just make 
            // an easy sample like this to test with
            var collection = new ServiceCollection()
                .AddHttpClient();
            var provider = collection.BuildServiceProvider();
            var factory = provider.GetService<IHttpClientFactory>();
            var userAccountResolver = new UserAccountResolver(factory, config);

            var userAccount = await userAccountResolver.TryResolveUserAccountStatusOrDefaultAsync();
            var pubsub = PubSubClientFactory.CreateClient();
            var irc = IrcClientFactory.CreateClient(config, userAccountStatus: userAccount);

            irc.Events.OnIrcConnected += () =>
            {
                Console.WriteLine("[IRC] Connected");
                return Task.CompletedTask;
            };

            // Event listening - which are sent as "tasks" so you can easily async/await the event instead of
            // having to do a manual Task.Run() for doing async executions as example API / Database, you name it.
            pubsub.Events.OnPubSubConnected += () =>
            {
                Console.WriteLine("[PUBSUB] Connected");
                OnPubSubConnected(pubsub, config);
                return Task.CompletedTask;
            };
            pubsub.Events.OnPubSubDisconnect += () =>
            {
                Console.WriteLine("[PUBSUB] Disconnected");
                return Task.CompletedTask;
            };
            pubsub.Events.OnPubSubReconnect += () =>
            {
                Console.WriteLine("[PUBSUB] Reconnected");
                OnPubSubConnected(pubsub, config);
                return Task.CompletedTask;
            };
            pubsub.Events.OnCustomRedeemEvent += PubSubOnRedeemEvent;

            irc.Events.OnIrcConnected += async () =>
            {
                var chat = irc.JoinChannel(config.BaseChannel);
                await Task.Delay(5000); // this is just dummy for testing till I fixed events
                irc.SendMessage(chat, "Hello World!");
            };
            
            // if you wanna make sure the pubsub client initial connects
            // you can just do it in a while loop, since the initial connection will not "retry", only reconnects
            while (!await pubsub.ConnectAsync())
                Console.WriteLine("[PUBSUB] Failed initial connecting, retrying...");

            while (!await irc.ConnectAsync())
                Console.WriteLine("[IRC] Failed initial connecting, retrying...");
        }

        private Task PubSubOnRedeemEvent(CommunityPointsEvent arg)
        {
            // this works
            Console.WriteLine($"ON REDEEM EVENT FIRE - TYPE: {arg.Type}");
            return Task.CompletedTask;
        }

        private void OnPubSubConnected(
            IPubSubClient pubsub, 
            IPubSubCredentialConfiguration config
            ) => 
            pubsub.CreateBuilder()
                .CreateChannelPointsRedeemTopic(UserId)
                .Listen(config.OAuth);

        /**
         * Never returns which makes the console stay open - this is for testing purposes only!
         * You should not really do this in a real application.
         */
        private void ListenToInput()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    continue;

                switch (input.ToLower())
                {
                    case "exit":
                        Environment.Exit(1);
                        break;
                }
            }
            // ReSharper disable once FunctionNeverReturns - for the people using ReSharper.
        }
        
        static void Main() => _ = new TwitchNetSamples();
    }
}
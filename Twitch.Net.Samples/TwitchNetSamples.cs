using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Twitch.Net.PubSub.Client;
using Twitch.Net.PubSub.Events;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Credential;

namespace Twitch.Net.Samples
{
    public class TwitchNetSamples
    {
        public const long UserId = 137132000; // Temporary to use for pubsub (later on this would be resolved by API)
        
        private TwitchNetSamples()
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

            var helper = new ClientCredentialTokenResolver(factory, config);
            var pubsub = PubSubClientFactory.CreateClient();
            
            pubsub.ConnectionLoggerConfiguration.OutputLog = true;
            pubsub.ConnectionLoggerConfiguration.OutputMessageLog = true;

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

            Task.Run(async () =>
            {
                // if you wanna make sure the pubsub client initial connects
                // you can just do it in a while loop, since the initial connection will not "retry", only reconnects
                while (!await pubsub.ConnectAsync())
                {
                    Console.WriteLine("Failed initial connecting, retrying...");
                }
            });

            ListenToInput();
        }

        private Task PubSubOnRedeemEvent(CommunityPointsEvent arg)
        {
            // this works
            return Task.CompletedTask;
        }

        private void OnPubSubConnected(
            IPubSubClient pubsub, 
            TwitchCredentialConfiguration config
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
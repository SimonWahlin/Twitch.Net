using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Twitch.Net.EventSub
{
    public static class EventSubServiceFactory
    {
        public const string EventSubFactory = "Twitch-EventSub";
        
        public static void AddEventSubService(this IServiceCollection service, Action<EventSubConfig> config = null)
        {
            var configuration = new EventSubConfig();

            // pass the pre-created config to action so it can modified when being added with DI
            config?.Invoke(configuration);
            
            AddService(service, configuration);
        }

        public static void AddEventSubService(this IServiceCollection service, EventSubConfig config = null) =>
            AddService(service, config ?? new EventSubConfig());

        private static void AddService(IServiceCollection serviceCollection, EventSubConfig config)
        {
            serviceCollection.AddHttpClient("Twitch-EventSub", c =>
            {
                c.BaseAddress = new Uri("https://api.twitch.tv/");
                c.DefaultRequestHeaders.Add("Client-ID", config.ClientId);
            });

            serviceCollection.Configure<EventSubConfig>(cfg =>
            {
                cfg.CallbackUrl = config.CallbackUrl;
                cfg.ClientId = config.ClientId;
                cfg.SignatureSecret = config.SignatureSecret;
            });
            serviceCollection.TryAddSingleton<IEventSubService, EventSubService>();
        }
    }
}
namespace Twitch.Net.Sample.EventSubServer;

public class EventSubSample
{
    private EventSubSample() => CreateHostBuilder().Build().Run();
        
    public static void Main() => _ = new EventSubSample();

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
using Microsoft.Extensions.Logging;
using Moq;
using Optional.Unsafe;
using Twitch.Net.EventSub;
using Twitch.Net.EventSub.Notifications;
using Xunit;

namespace Twitch.Net.Unit.Tests;

public class EventSubModelConverterTests
{
    private readonly EventSubModelConverter _converter;

    public EventSubModelConverterTests()
    {
        _converter = new EventSubModelConverter(new Mock<ILogger<EventSubModelConverter>>().Object);
    }
        
    [Fact]
    public void ConvertEvent_ConvertsSubscriptionCorrectly()
    {
        // arrange
        var type = EventSubTypes.ChannelUpdate;
        var data = "{\"subscription\":{\"id\":\"f1c2a387-161a-49f9-a165-0f21d7a4e1c4\",\"type\":\"channel.update\",\"version\":\"1\",\"status\":\"enabled\",\"cost\":5,\"condition\":{\"broadcaster_user_id\":\"1337\"},\"transport\":{\"method\":\"webhook\",\"callback\":\"https://example.com/webhooks/callback\"},\"created_at\":\"2019-11-16T10:11:12.123Z\"},\"event\":{\"broadcaster_user_id\":\"1337\",\"broadcaster_user_login\":\"cool_user\",\"broadcaster_user_name\":\"Cool_User\",\"title\":\"BestStreamEver\",\"language\":\"en\",\"category_id\":\"21779\",\"category_name\":\"Fortnite\",\"is_mature\":false}}";

        // act
        var sut = _converter.GetModel(data, type);
            
        // assert
        Assert.True(sut.HasValue);
        var model = sut.ValueOrDefault() as NotificationEvent<ChannelUpdateNotificationEvent>;
        Assert.NotNull(model);
            
        Assert.Equal("f1c2a387-161a-49f9-a165-0f21d7a4e1c4", model.Subscription.Id);
        Assert.Equal("channel.update", model.Subscription.Type);
        Assert.Equal("1", model.Subscription.Version);
        Assert.Equal("enabled", model.Subscription.Status);
        Assert.Equal(5, model.Subscription.Cost);
        Assert.Equal("webhook", model.Subscription.Transport.Method);
        Assert.Equal("https://example.com/webhooks/callback", model.Subscription.Transport.Callback);
        Assert.Equal(DateTime.Parse("2019-11-16T10:11:12.123Z").Date, model.Subscription.CreatedAt.Date);
    }
        
    [Fact]
    public void ConvertChannelUpdateNotificationJson_ConvertsToModelCorrectly()
    {
        // arrange
        var type = EventSubTypes.ChannelUpdate;
        var data = "{\"subscription\":{\"id\":\"f1c2a387-161a-49f9-a165-0f21d7a4e1c4\",\"type\":\"channel.update\",\"version\":\"1\",\"status\":\"enabled\",\"cost\":5,\"condition\":{\"broadcaster_user_id\":\"1337\"},\"transport\":{\"method\":\"webhook\",\"callback\":\"https://example.com/webhooks/callback\"},\"created_at\":\"2019-11-16T10:11:12.123Z\"},\"event\":{\"broadcaster_user_id\":\"1337\",\"broadcaster_user_login\":\"cool_user\",\"broadcaster_user_name\":\"Cool_User\",\"title\":\"BestStreamEver\",\"language\":\"en\",\"category_id\":\"21779\",\"category_name\":\"Fortnite\",\"is_mature\":false}}";

        // act
        var sut = _converter.GetModel(data, type);
            
        // assert
        Assert.True(sut.HasValue);
        var model = sut.ValueOrDefault() as NotificationEvent<ChannelUpdateNotificationEvent>;
        Assert.NotNull(model);
            
        Assert.Equal(1337, model.Event.BroadcasterId);
        Assert.Equal("1337", model.Event.BroadcasterIdString);
        Assert.Equal("cool_user", model.Event.BroadcasterUserLogin);
        Assert.Equal("Cool_User", model.Event.BroadcasterUserName);
        Assert.Equal(21779, model.Event.CategoryId);
        Assert.Equal("21779", model.Event.CategoryIdString);
        Assert.Equal("Fortnite", model.Event.CategoryName);
        Assert.False(model.Event.IsMature);
        Assert.Equal("en", model.Event.Language);
        Assert.Equal("BestStreamEver", model.Event.Title);
    }
}
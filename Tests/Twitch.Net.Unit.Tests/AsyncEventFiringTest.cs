using Twitch.Net.Shared.Extensions;
using Xunit;

namespace Twitch.Net.Unit.Tests;

/// <summary>
/// Made it because I was doing a refactor of how events are fired internally
/// </summary>
public class AsyncEventFiringTest
{
    [Fact]
    public async Task AsyncEvent_FireTest()
    {
        AsyncEvent<Func<Task>> events = new();
        var sut = new FakeClass(events);
        var task = new TaskCompletionSource();
        sut.OnEvent += () =>
        {
            task.TrySetResult();
            return task.Task;
        };

        events.Invoke();
        await task.Task;
            
        Assert.True(task.Task.IsCompleted);
    }
}

public class FakeClass
{
    private readonly AsyncEvent<Func<Task>> _events;

    public FakeClass(AsyncEvent<Func<Task>> events)
    {
        _events = events;
    }
        
    public event Func<Task> OnEvent 
    {
        add => _events.Add(value);
        remove => _events.Remove(value);
    }
}
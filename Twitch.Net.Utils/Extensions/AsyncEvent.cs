using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Twitch.Net.Utils.Extensions
{
    public class AsyncEvent<T> where T : class
    {
        private readonly object _subLock = new();
        private ImmutableArray<T> _subscriptions;

        public bool HasSubscribers => _subscriptions.Length != 0;
        public IReadOnlyList<T> Subscriptions => _subscriptions;

        public AsyncEvent()
        {
            _subscriptions = ImmutableArray.Create<T>();
        }

        public void Add(T subscriber)
        {
            lock (_subLock)
                _subscriptions = _subscriptions.Add(subscriber);
        }
        
        public void Remove(T subscriber)
        {
            lock (_subLock)
                _subscriptions = _subscriptions.Remove(subscriber);
        }
    }

    public static class AsyncEventExtensions
    { 
        public static async Task InvokeAsync(this AsyncEvent<Func<Task>> eventHandler)
        {
            var subscribers = eventHandler.Subscriptions;
            foreach (var t in subscribers)
                await t.Invoke().ConfigureAwait(false);
        }
        
        public static async Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> eventHandler, T arg)
        {
            var subscribers = eventHandler.Subscriptions;
            foreach (var t in subscribers)
                await t.Invoke(arg).ConfigureAwait(false);
        }
    }
}
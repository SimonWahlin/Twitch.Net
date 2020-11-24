using System;
using System.Collections.Generic;

namespace Twitch.Net.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach(var item in enumeration)
                action(item);
        }
    }
}
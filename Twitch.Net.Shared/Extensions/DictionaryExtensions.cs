using System.Collections.Generic;
using System.Linq;

namespace Twitch.Net.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static IEnumerable<Dictionary<T1, T2>> SplitChunks<T1, T2>(
            this IDictionary<T1, T2> dictionary,
            int chunkSize)
            => dictionary.Select((kvp, n) => new { kvp, k = n % chunkSize })
                .GroupBy(x => x.k, x => x.kvp)
                .Select(x => x.ToDictionary(y => y.Key, y => y.Value));
    }
}
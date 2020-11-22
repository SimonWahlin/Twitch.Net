using System.Collections.Generic;
using System.Text.Json;

namespace Twitch.Net.Shared.Extensions
{
    public static class JsonSerializerHelper
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            IgnoreNullValues = true
        };

        public static string SerializeModel(object model) =>
            JsonSerializer.Serialize(model, SerializerOptions);

        public static Dictionary<string, object> Deserialize(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }
    }
}
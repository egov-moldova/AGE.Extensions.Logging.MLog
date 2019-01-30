using Newtonsoft.Json;
using System.Collections.Generic;

namespace AGE.Extensions.Logging.MLog
{
    public static class MLogMessageExtensions
    {
        public static string ToJson(this Dictionary<string, object> message)
        {
            return JsonConvert.SerializeObject(message, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}

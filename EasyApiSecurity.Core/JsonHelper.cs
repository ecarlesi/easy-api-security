using Newtonsoft.Json;

namespace EasyApiSecurity.Core
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerSettings? Settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static T? Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public static object? Deserialize(string json, Type returnType)
        {
            return JsonConvert.DeserializeObject(json, returnType, Settings);
        }

        public static string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o, Settings);
        }
    }
}

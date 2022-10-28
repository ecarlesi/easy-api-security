using Newtonsoft.Json;

namespace EasyApiSecurity.Core
{
    public class JsonHelper
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static object Deserialize(string json, Type returnType)
        {
            return JsonConvert.DeserializeObject(json, returnType, settings);
        }

        public static string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o, settings);
        }
    }
}

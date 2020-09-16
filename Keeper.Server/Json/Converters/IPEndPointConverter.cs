using Newtonsoft.Json;
using System;
using System.Net;

namespace Keeper.Json.Converters
{
    public class IPEndPointConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            string str = serializer.Deserialize<string>(reader);
            string[] arr = str.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            return new IPEndPoint(IPAddress.Parse(arr[0]), int.Parse(arr[1]));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPEndPoint);
        }
    }
}
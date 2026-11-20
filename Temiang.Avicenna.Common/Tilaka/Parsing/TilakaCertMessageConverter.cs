using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaCertMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TilakaCertMessage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var token = JToken.Load(reader);
            var message = new TilakaCertMessage();

            if (token.Type == JTokenType.String)
            {
                message.Info = token.Value<string>();
                return message;
            }

            if (token.Type == JTokenType.Object)
            {
                var obj = (JObject)token;
                message.Info = Str(obj, "info");
                message.Name = Str(obj, "name");
                message.Email = Str(obj, "email");
                message.Company = Str(obj, "company");
                message.Country = Str(obj, "country");
                message.SerialNumber = Str(obj, "serialnumber");
            }

            return message;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        private static string Str(JObject obj, string name)
        {
            var token = obj[name];
            return token == null || token.Type == JTokenType.Null ? null : token.ToString();
        }
    }
}

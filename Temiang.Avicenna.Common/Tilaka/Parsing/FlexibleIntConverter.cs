using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class FlexibleIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int) || objectType == typeof(int?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return 0;
            if (reader.TokenType == JsonToken.Integer) return Convert.ToInt32(reader.Value, CultureInfo.InvariantCulture);

            int value;
            var text = reader.Value == null ? string.Empty : reader.Value.ToString();
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : 0;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PathwayGames.Infrastructure.Json
{
    public class UnixTimeMillisecondsConverter : DateTimeConverterBase
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dateTimeOffset = new DateTimeOffset((DateTime)value);
            writer.WriteRawValue(dateTimeOffset.ToUnixTimeMilliseconds().ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value == null ? UnixEpoch : DateTimeOffset.FromUnixTimeMilliseconds((long)reader.Value).UtcDateTime;
        }
    }
}

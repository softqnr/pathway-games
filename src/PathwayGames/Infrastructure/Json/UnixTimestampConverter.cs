using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PathwayGames.Infrastructure.Json
{
    public class UnixTimestampConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(UnixTimestampFromDateTime((DateTime)value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value == null ? _epoch : TimeFromUnixTimestamp((long)reader.Value);
        }

        private static DateTime TimeFromUnixTimestamp(long unixTimestamp)
        {
            long unixTimeStampInTicks = unixTimestamp * TimeSpan.TicksPerSecond;
            return new DateTime(_epoch.Ticks + unixTimeStampInTicks);
        }

        public static long UnixTimestampFromDateTime(DateTime date)
        {
            long unixTimestamp = date.Ticks - _epoch.Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }
    }
}

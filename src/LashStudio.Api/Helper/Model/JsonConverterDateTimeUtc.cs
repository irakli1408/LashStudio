using System.Text.Json;
using System.Text.Json.Serialization;

namespace LashStudio.Api.Helper.Model
{
    public class JsonConverterDateTimeUtc : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.SpecifyKind(DateTime.Parse(reader.GetString()!), DateTimeKind.Utc);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(
                DateTime.SpecifyKind(value, DateTimeKind.Utc)
                        .ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")
            );
    }

}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace LashStudio.Api.Helper.Model
{
    public class JsonConverterNullableDateTimeUtc : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            return str is null ? null : DateTime.SpecifyKind(DateTime.Parse(str), DateTimeKind.Utc);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(
                    DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                            .ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")
                );
            else
                writer.WriteNullValue();
        }
    }
}

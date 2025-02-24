using System.Text.Json;
using System.Text.Json.Serialization;
using V1.Commons.Enums;

namespace V1.Converters
{
    public class LanguageConverter : JsonConverter<Language>
    {
        public override Language Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                throw new JsonException("Language cannot be empty.");
            }

            return value.ToLower() switch
            {
                "en" => Language.English,
                "id" => Language.Indonesia,
                _ => throw new JsonException($"Invalid language: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options)
        {
            string language = value switch
            {
                Language.English => "en",
                Language.Indonesia => "id",
                _ => throw new JsonException($"Invalid language: {value}")
            };

            writer.WriteStringValue(language);
        }
    }
}
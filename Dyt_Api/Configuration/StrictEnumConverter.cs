using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dyt_Api.Configuration
{
    // JsonStringEnumConverter parses "Male,Female" as combined flags, which none of our enums use.
    public sealed class StrictEnumConverter : JsonConverterFactory
    {
        private readonly JsonStringEnumConverter _inner = new();

        public override bool CanConvert(Type typeToConvert)
        {
            return _inner.CanConvert(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(StrictConverter<>).MakeGenericType(typeToConvert),
                _inner.CreateConverter(typeToConvert, options))!;
        }

        private sealed class StrictConverter<TEnum> : JsonConverter<TEnum>
            where TEnum : struct, Enum
        {
            private readonly JsonConverter<TEnum> _converter;

            public StrictConverter(JsonConverter converter)
            {
                _converter = (JsonConverter<TEnum>)converter;
            }

            public override TEnum Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String && reader.GetString()!.Contains(','))
                {
                    throw new JsonException();
                }

                return _converter.Read(ref reader, typeToConvert, options);
            }

            public override void Write(
                Utf8JsonWriter writer,
                TEnum value,
                JsonSerializerOptions options)
            {
                _converter.Write(writer, value, options);
            }
        }
    }
}

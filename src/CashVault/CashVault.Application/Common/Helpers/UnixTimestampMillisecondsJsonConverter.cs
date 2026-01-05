using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CashVault.Application.Common.Helpers
{
    public class UnixTimestampMillisecondsJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                long timestamp = reader.GetInt64();
                return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
            }

            throw new JsonException("Expected number token.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            long timestamp = new DateTimeOffset(value).ToUnixTimeMilliseconds();
            writer.WriteNumberValue(timestamp);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CashVault.Application.Common.Helpers;

public class StringToDateTimeJsonConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();
            
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            if (DateTime.TryParse(stringValue, out DateTime dateTimeValue))
            {
                return dateTimeValue;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).DateTime;
        }

        throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {typeToConvert}.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CashVault.Application.Common.Helpers;

public class AmountInCentsJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal() / 100m;
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();
            if (decimal.TryParse(stringValue, out decimal decimalValue))
            {
                return decimalValue / 100m;
            }
        }

        throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {typeToConvert}.");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        int intValue = (int)(value * 100);
        writer.WriteNumberValue(intValue);
    }
}

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class StringToBoolJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();
            if (bool.TryParse(stringValue, out bool boolValue))
            {
                return boolValue;
            }
        }
        else if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
        {
            return reader.GetBoolean();
        }

        throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {typeToConvert}.");
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;
using FullStack.Application.Common;

namespace FullStack.Application.DTOs.Common;

/// <summary>
/// Reads true/false/1/0/"1"/"0"/"true"/"false"/"" and writes 1/0 (like MySQL tinyint did in the Express API),
/// so the React client keeps working with <c>item.Status + ""</c> and "1"/"0" selects.
/// </summary>
public sealed class LegacyBoolConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.GetDouble() != 0,
            JsonTokenType.String => LegacyParse.ToBool(reader.GetString()),
            _ => throw new JsonException("Invalid boolean value.")
        };

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.HasValue) writer.WriteNumberValue(value.Value ? 1 : 0);
        else writer.WriteNullValue();
    }
}

/// <summary>Reads ISO / yyyy-MM-dd strings; empty, "null" and "undefined" become null.</summary>
public sealed class LegacyDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String:
                var text = reader.GetString();
                if (LegacyParse.IsEmpty(text)) return null;
                return LegacyParse.ToDate(text) ?? throw new JsonException("Invalid date value.");
            default:
                throw new JsonException("Invalid date value.");
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue) writer.WriteStringValue(value.Value);
        else writer.WriteNullValue();
    }
}

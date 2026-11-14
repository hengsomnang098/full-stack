using System.Globalization;

namespace FullStack.Api.Application.Common;

/// <summary>
/// Lenient parsing helpers matching the behaviour of the old Express API, where the React client sends
/// strings like "1", "0", "All", "null" and "undefined" for query strings and multipart form fields.
/// </summary>
public static class LegacyParse
{
    public static bool IsEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value)
        || value.Equals("null", StringComparison.OrdinalIgnoreCase)
        || value.Equals("undefined", StringComparison.OrdinalIgnoreCase);

    public static bool IsAll(string? value) =>
        value is not null && value.Trim().Equals("All", StringComparison.OrdinalIgnoreCase);

    public static string? Text(string? value) => IsEmpty(value) ? null : value!.Trim();

    public static bool? ToBool(string? value)
    {
        if (IsEmpty(value)) return null;
        return value!.Trim().ToLowerInvariant() switch
        {
            "1" or "true" => true,
            "0" or "false" => false,
            _ => null
        };
    }

    public static int? ToInt(string? value) =>
        !IsEmpty(value) && int.TryParse(value!.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
            ? n
            : null;

    public static decimal? ToDecimal(string? value) =>
        !IsEmpty(value) && decimal.TryParse(value!.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var n)
            ? n
            : null;

    public static DateTime? ToDate(string? value) =>
        !IsEmpty(value) && DateTime.TryParse(value!.Trim(), CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind, out var d)
            ? d
            : null;

    /// <summary>Status query filter: empty / "All" => no filter, otherwise 1/0.</summary>
    public static bool? StatusFilter(string? value) => IsAll(value) ? null : ToBool(value);

    /// <summary>Integer id filter: empty / "All" => no filter.</summary>
    public static int? IdFilter(string? value) => IsAll(value) ? null : ToInt(value);
}

using System.Collections;

namespace Exizent.ComponentModel.DataAnnotations;

internal static class ValueFormatter
{
    public static string FormatValue(object? value)
        => value switch
        {
            null => "null",
            string s => s,
            // A collection's default ToString() only yields its type name (e.g.
            // "System.Collections.Generic.List`1[System.Guid]"), which is meaningless in a
            // validation message. Render the actual elements instead.
            IEnumerable enumerable => string.Join(", ", enumerable.Cast<object?>().Select(FormatValue)),
            _ => value.ToString() ?? "null"
        };

    public static string FormatOrValues(object[] values)
        => FormatValues(values, "or");
    
    public static string FormatAndValues(object[] values)
        => FormatValues(values, "and");

    private static string FormatValues(object[] values, string joiningWord)
    {
        if (values is { Length: 1 })
            return FormatValue(values[0]);

        var formatted = string.Join(", ", values[..^1].Select(FormatValue));

        return formatted + " " + joiningWord + " " + FormatValue(values[^1]);
    }
}
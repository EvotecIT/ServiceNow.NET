using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ServiceNow.Extensions;

/// <summary>
/// Helper methods for converting dictionaries to query strings.
/// </summary>
public static class QueryStringExtensions {
    /// <summary>
    /// Converts a dictionary of string values to a URL encoded query string.
    /// </summary>
    /// <param name="parameters">Parameters to encode.</param>
    public static string ToQueryString(this IDictionary<string, string?> parameters)
        => parameters.ToDictionary(static kvp => kvp.Key, static kvp => (object?)kvp.Value)
            .ToQueryString();

    /// <summary>
    /// Converts a dictionary of object values to a URL encoded query string.
    /// </summary>
    /// <param name="parameters">Parameters to encode.</param>
    public static string ToQueryString(this IDictionary<string, object?> parameters) {
        var builder = new StringBuilder();
        foreach (var kvp in parameters) {
            AppendValue(builder, kvp.Key, kvp.Value);
        }

        return builder.ToString();
    }

    private static void AppendValue(StringBuilder builder, string key, object? value) {
        if (value is null) {
            return;
        }

        if (value is string str) {
            AppendPair(builder, key, str);
        } else if (value is IEnumerable<string> list && value is not string) {
            foreach (var item in list) {
                if (item is not null) {
                    AppendPair(builder, key, item);
                }
            }
        } else {
            AppendPair(builder, key, value.ToString() ?? string.Empty);
        }
    }

    private static void AppendPair(StringBuilder builder, string key, string value) {
        if (builder.Length > 0) {
            builder.Append('&');
        }

        builder.Append(Uri.EscapeDataString(key));
        builder.Append('=');
        builder.Append(Uri.EscapeDataString(value));
    }
}
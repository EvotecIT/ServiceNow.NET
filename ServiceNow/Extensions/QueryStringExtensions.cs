using System.Text;

namespace ServiceNow.Extensions;

public static class QueryStringExtensions {
    public static string ToQueryString(this IDictionary<string, string?> parameters) {
        var builder = new StringBuilder();
        foreach (var kvp in parameters) {
            if (kvp.Value is null) {
                continue;
            }

            if (builder.Length > 0) {
                builder.Append('&');
            }

            builder.Append(Uri.EscapeDataString(kvp.Key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(kvp.Value));
        }

        return builder.ToString();
    }
}
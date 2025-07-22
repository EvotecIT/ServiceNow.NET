using System.Collections.Generic;
using ServiceNow;

namespace ServiceNow.Extensions;

/// <summary>
/// Extension methods for <see cref="TableQueryOptions"/>.
/// </summary>
public static class TableQueryOptionsExtensions {
    /// <summary>
    /// Converts the options to a URL encoded query string.
    /// </summary>
    /// <param name="options">The options to convert.</param>
    public static string ToQueryString(this TableQueryOptions options) {
        var dict = options.AdditionalParameters is not null
            ? new Dictionary<string, string?>(options.AdditionalParameters)
            : new Dictionary<string, string?>();

        if (!string.IsNullOrEmpty(options.Fields)) {
            dict["sysparm_fields"] = options.Fields;
        }
        if (!string.IsNullOrEmpty(options.Query)) {
            dict["sysparm_query"] = options.Query;
        }
        if (options.DisplayValue is not null) {
            dict["sysparm_display_value"] = options.DisplayValue.Value ? "true" : "false";
        }
        if (options.ExcludeReferenceLink is not null) {
            dict["sysparm_exclude_reference_link"] = options.ExcludeReferenceLink.Value ? "true" : "false";
        }
        if (options.Limit is not null) {
            dict["sysparm_limit"] = options.Limit.Value.ToString();
        }
        if (options.Offset is not null) {
            dict["sysparm_offset"] = options.Offset.Value.ToString();
        }

        return dict.ToQueryString();
    }
}

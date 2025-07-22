namespace ServiceNow;

using System.Collections.Generic;
using ServiceNow.Extensions;
using ServiceNow.Queries;

/// <summary>
/// Options for querying the table API.
/// </summary>
public record TableQueryOptions {
    /// <summary>
    /// Gets or sets the encoded query to filter results.
    /// </summary>
    public QueryBuilder? Query { get; init; }

    /// <summary>
    /// Gets or sets the list of fields to return.
    /// </summary>
    public IEnumerable<string>? Fields { get; init; }

    /// <summary>
    /// Gets or sets the display value option.
    /// </summary>
    public string? DisplayValue { get; init; }

    /// <summary>
    /// Gets or sets whether to exclude reference links.
    /// </summary>
    public bool? ExcludeReferenceLinks { get; init; }

    /// <summary>
    /// Gets additional query parameters not covered by known options.
    /// </summary>
    public Dictionary<string, string?> AdditionalParameters { get; init; } = new();

    internal string ToQueryString() {
        var dict = new Dictionary<string, object?>();
        if (Fields is not null) {
            dict["sysparm_fields"] = Fields;
        }
        if (Query is not null) {
            dict["sysparm_query"] = Query.ToQueryString();
        }
        if (!string.IsNullOrEmpty(DisplayValue)) {
            dict["sysparm_display_value"] = DisplayValue;
        }
        if (ExcludeReferenceLinks.HasValue) {
            dict["sysparm_exclude_reference_link"] = ExcludeReferenceLinks.Value ? "true" : "false";
        }
        foreach (var kv in AdditionalParameters) {
            dict[kv.Key] = kv.Value;
        }
        return dict.Count > 0 ? dict.ToQueryString() : string.Empty;
    }
}

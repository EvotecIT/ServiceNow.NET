using System.Collections.Generic;
namespace ServiceNow;

/// <summary>
/// Options for querying table records.
/// </summary>
public record TableQueryOptions {
    /// <summary>List of fields to return.</summary>
    public string? Fields { get; init; }

    /// <summary>Encoded query string.</summary>
    public string? Query { get; init; }

    /// <summary>Include display values in the response.</summary>
    public bool? DisplayValue { get; init; }

    /// <summary>Exclude reference link properties.</summary>
    public bool? ExcludeReferenceLink { get; init; }

    /// <summary>Maximum number of records to return.</summary>
    public int? Limit { get; init; }

    /// <summary>Record offset.</summary>
    public int? Offset { get; init; }

    /// <summary>Additional query parameters.</summary>
    public IDictionary<string, string?>? AdditionalParameters { get; init; }
}

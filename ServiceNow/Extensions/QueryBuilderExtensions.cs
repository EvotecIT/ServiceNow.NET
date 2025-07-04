using ServiceNow.Queries;

namespace ServiceNow.Extensions;

/// <summary>
/// Extension methods for <see cref="QueryBuilder"/>.
/// </summary>
public static class QueryBuilderExtensions {
    /// <summary>
    /// Builds the encoded query string from this <see cref="QueryBuilder"/>.
    /// </summary>
    /// <param name="builder">The query builder.</param>
    public static string ToQueryString(this QueryBuilder builder) => builder.ToString();
}

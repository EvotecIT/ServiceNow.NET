namespace ServiceNow.Builders;

using ServiceNow.Extensions;

/// <summary>
/// Provides a fluent API for building query string parameters.
/// </summary>
public sealed class QueryBuilder
{
    private readonly IDictionary<string, string?> _parameters = new Dictionary<string, string?>();

    /// <summary>
    /// Adds or updates a query parameter.
    /// </summary>
    /// <param name="key">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <returns>The current <see cref="QueryBuilder"/>.</returns>
    public QueryBuilder Add(string key, string? value)
    {
        _parameters[key] = value;
        return this;
    }

    /// <summary>
    /// Adds a query parameter if the value is not null.
    /// </summary>
    /// <param name="key">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <returns>The current <see cref="QueryBuilder"/>.</returns>
    public QueryBuilder AddIfNotNull(string key, string? value)
    {
        if (value is not null)
        {
            _parameters[key] = value;
        }

        return this;
    }

    /// <summary>
    /// Builds the query string from the configured parameters.
    /// </summary>
    /// <returns>The query string.</returns>
    public string Build() => _parameters.ToQueryString();
}

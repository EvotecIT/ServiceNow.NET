using ServiceNow.Clients;
using ServiceNow.Fluent;

namespace ServiceNow.Extensions;

/// <summary>
/// Extension methods that expose a fluent API for <see cref="ServiceNowClient"/>.
/// </summary>
public static class ServiceNowClientFluentExtensions
{
    /// <summary>
    /// Starts a fluent call chain for working with a specific table.
    /// </summary>
    public static FluentTableApi Table(this ServiceNowClient client, string table)
        => new(client, table);
}

using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Clients;
using ServiceNow.Configuration;

namespace ServiceNow.Extensions;

/// <summary>
/// Extension methods for registering ServiceNow clients with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers ServiceNow typed HTTP clients.
    /// </summary>
    public static IServiceCollection AddServiceNow(this IServiceCollection services, ServiceNowSettings settings) {
        services.AddSingleton(settings);
        services.AddHttpClient<IServiceNowClient, ServiceNowClient>();
        services.AddTransient<TableApiClient>();
        services.AddTransient<AttachmentApiClient>();
        return services;
    }
}

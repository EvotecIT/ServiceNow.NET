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
    /// <param name="services">The service collection.</param>
    /// <param name="settings">Configuration settings.</param>
    public static IServiceCollection AddServiceNow(this IServiceCollection services, ServiceNowSettings settings) {
        services.AddSingleton(settings);
        services.AddHttpClient<IServiceNowClient, ServiceNowClient>();
        services.AddTransient<TableApiClient>();
        services.AddTransient<AttachmentApiClient>();
        services.AddTransient<TableMetadataClient>();
        services.AddTransient<WorkflowApiClient>();
        services.AddTransient<CatalogItemClient>();
        services.AddTransient<CatalogRequestClient>();
        services.AddTransient<EventApiClient>();
        services.AddTransient<GroupApiClient>();
        services.AddTransient<UserApiClient>();
        services.AddTransient<ReportApiClient>();
        return services;
    }
}

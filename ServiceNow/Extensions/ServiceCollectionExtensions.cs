using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Clients;
using ServiceNow.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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

        services.AddHttpClient(ServiceNowClient.HttpClientName, (sp, client) =>
        {
            var opts = sp.GetRequiredService<ServiceNowSettings>();
            client.BaseAddress = new Uri(opts.BaseUrl!);
            client.Timeout = opts.Timeout;
            if (!opts.UseOAuth)
            {
                var authBytes = Encoding.ASCII.GetBytes($"{opts.Username}:{opts.Password}");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
            }
            client.DefaultRequestHeaders.UserAgent.ParseAdd(opts.UserAgent);
        });

        services.AddTransient<IServiceNowClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var http = factory.CreateClient(ServiceNowClient.HttpClientName);
            var opts = sp.GetRequiredService<ServiceNowSettings>();
            return new ServiceNowClient(http, opts);
        });

        services.AddTransient<TableApiClient>();
        services.AddTransient<AttachmentApiClient>();
        services.AddTransient<TableMetadataClient>();
        services.AddTransient<WorkflowApiClient>();
        services.AddTransient<CatalogItemClient>();
        services.AddTransient<CatalogRequestClient>();
        services.AddTransient<EventApiClient>();
        services.AddTransient<GroupApiClient>();
        services.AddTransient<RoleApiClient>();
        services.AddTransient<UserApiClient>();
        services.AddTransient<ReportApiClient>();
        services.AddTransient<CmdbRelationsClient>();
        services.AddTransient<DataExportClient>();
        services.AddTransient<SamApiClient>();
        services.AddTransient<ResourcePlanClient>();
        services.AddTransient<GrcClient>();
        services.AddTransient<ScriptedRestClient>();
        return services;
    }
}

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
                if (string.IsNullOrEmpty(opts.Username))
                {
                    throw new ArgumentException("Username is required when not using OAuth.", nameof(ServiceNowSettings.Username));
                }

                if (string.IsNullOrEmpty(opts.Password))
                {
                    throw new ArgumentException("Password is required when not using OAuth.", nameof(ServiceNowSettings.Password));
                }

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
            var tokenService = new TokenService(http, opts);
            return new ServiceNowClient(http, opts, tokenService);
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

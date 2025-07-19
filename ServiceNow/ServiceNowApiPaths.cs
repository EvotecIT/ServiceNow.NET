using System;

namespace ServiceNow;

/// <summary>
/// Provides format strings for ServiceNow REST API endpoints.
/// </summary>
public static class ServiceNowApiPaths {
    /// <summary>
    /// Format string for attachment retrieval and deletion.
    /// </summary>
    public const string Attachment = "/api/now/{0}/attachment/{1}";

    /// <summary>
    /// Format string for uploading an attachment.
    /// </summary>
    public const string AttachmentFile = "/api/now/{0}/attachment/file?table_name={1}&table_sys_id={2}";

    /// <summary>
    /// Format string for table record operations.
    /// </summary>
    public const string TableRecord = "/api/now/{0}/table/{1}/{2}";

    /// <summary>
    /// Format string for table collection operations.
    /// </summary>
    public const string Table = "/api/now/{0}/table/{1}";

    /// <summary>
    /// Format string for retrieving table metadata.
    /// </summary>
    public const string TableMetadata = "/api/now/table/sys_dictionary?sysparm_query=name={0}&sysparm_fields=element,internal_type";

    /// <summary>
    /// Format string for starting a workflow execution.
    /// </summary>
    public const string WorkflowStart = "/api/now/{0}/workflow/{1}/start";

    /// <summary>
    /// Format string for retrieving workflow execution status.
    /// </summary>
    public const string WorkflowExecution = "/api/now/{0}/workflow/execution/{1}";

    /// <summary>
    /// Format string for retrieving catalog items.
    /// </summary>
    public const string CatalogItems = "/api/sn_sc/{0}/catalog/items";

    /// <summary>
    /// Format string for retrieving a single catalog item.
    /// </summary>
    public const string CatalogItem = "/api/sn_sc/{0}/catalog/items/{1}";

    /// <summary>
    /// Format string for submitting a catalog item request.
    /// </summary>
    public const string CatalogItemOrder = "/api/sn_sc/{0}/catalog/items/{1}/order_now";

    /// <summary>
    /// Format string for retrieving a catalog request.
    /// </summary>
    public const string CatalogRequest = "/api/sn_sc/{0}/request/{1}";

    /// <summary>
    /// Format string for retrieving approvals for a catalog request.
    /// </summary>
    public const string CatalogRequestApprovals = "/api/sn_sc/{0}/request/{1}/approvals";

    /// <summary>
    /// Endpoint for posting Event Management events.
    /// </summary>
    public const string EmEvent = "/api/global/em_event";

    /// <summary>
    /// Endpoint for sending outbound email notifications.
    /// </summary>
    public const string EmailOutbound = "/api/now/{0}/email/outbound";

    /// <summary>
    /// Format string for retrieving inbound email details.
    /// </summary>
    public const string EmailInbound = "/api/now/{0}/email/inbound/{1}";
  
    /// <summary>
    /// Format string for searching knowledge articles.
    /// </summary>
    public const string KnowledgeSearch = "/api/now/{0}/knowledge";

    /// <summary>
    /// Format string for retrieving a single knowledge article.
    /// </summary>
    public const string KnowledgeArticle = "/api/now/{0}/knowledge/{1}";

    /// <summary>
    /// Format string for retrieving article attachments.
    /// </summary>
    public const string KnowledgeArticleAttachments = "/api/now/{0}/knowledge/{1}/attachments";

    /// <summary>
    /// Format string for retrieving knowledge categories.
    /// </summary>
    public const string KnowledgeCategories = "/api/now/{0}/knowledge/categories";
  
    /// <summary>
    /// Format string for retrieving application services.
    /// </summary>
    public const string ApplicationService = "/api/now/{0}/table/cmdb_ci_service";

    /// <summary>
    /// Format string for retrieving a service map for an application service.
    /// </summary>
    public const string ServiceMap = "/api/now/{0}/cmdb_ci_service/{1}/service-map";

    /// <summary>
    /// Format string for retrieving CMDB relationships for a configuration item.
    /// </summary>
    public const string CmdbRelationships = "/api/now/{0}/cmdb/instance/{1}/{2}/relationships";
  
    /// <summary>
    /// Format string for posting to the Import Set API.
    /// </summary>
    public const string ImportSet = "/api/now/import/{0}";

    /// <summary>
    /// Endpoint for starting a data export job.
    /// </summary>
    public const string DataExportStart = "/api/now/{0}/export";

    /// <summary>
    /// Format string for downloading an export file.
    /// </summary>
    public const string DataExportFile = "/api/now/{0}/export/{1}/file";

    /// <summary>
    /// Format string for listing reports or analytics data.
    /// </summary>
    public const string Reports = "/api/now/{0}/report";

    /// <summary>
    /// Format string for retrieving a specific report or analytics dataset.
    /// </summary>
    public const string ReportRecord = "/api/now/{0}/report/{1}";

    /// <summary>
    /// Format string for retrieving a software asset record.
    /// </summary>
    public const string SamSoftwareAsset = "/api/sn_sam/{0}/software/{1}";

    /// <summary>
    /// Format string for listing software asset records.
    /// </summary>
    public const string SamSoftwareAssets = "/api/sn_sam/{0}/software";

    /// <summary>
    /// Format string for retrieving a GRC item record.
    /// </summary>
    public const string GrcItem = "/api/sn_grc/{0}/grc/items/{1}";

    /// <summary>
    /// Format string for creating or listing GRC items.
    /// </summary>
    public const string GrcItems = "/api/sn_grc/{0}/grc/items";
}

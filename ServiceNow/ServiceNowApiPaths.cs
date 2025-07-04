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
}

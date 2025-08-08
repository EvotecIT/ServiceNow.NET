using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.IO;

namespace ServiceNow.PowerShell;

/// <summary>
/// Uploads a file as an attachment to a ServiceNow record.
/// </summary>
/// <remarks>
/// <para>Sends the specified file to the ServiceNow Attachment API.</para>
/// <list type="alertSet">
/// <item>
/// <term>Note</term>
/// <description>Large files may require additional time and bandwidth to upload.</description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <summary>Attach a log file to an incident.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Add-ServiceNowAttachment -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123" -FilePath "C:\\logs\\error.txt"</code>
/// <para>Uploads the log file to the specified incident record.</para>
/// </example>
/// <example>
/// <summary>Attach documentation to a problem record.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Add-ServiceNowAttachment -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "problem" -SysId "def456" -FilePath "C:\\docs\\readme.md"</code>
/// <para>Stores the documentation alongside the problem record.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/">PowerShell Documentation</seealso>
/// <seealso href="https://github.com/ServiceNowNET/ServiceNow.NET">Project documentation</seealso>
[Cmdlet(VerbsCommon.Add, "ServiceNowAttachment")]
public class AddServiceNowAttachment : PSCmdlet {
    /// <summary>
    /// Base URL of the ServiceNow instance.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Username used for authentication.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password used for authentication.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Name of the table containing the record.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Table { get; set; } = string.Empty;

    /// <summary>
    /// Sys_id of the record to attach the file to.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string SysId { get; set; } = string.Empty;

    /// <summary>
    /// Path to the file to upload.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Executes the cmdlet.
    /// </summary>
    protected override void ProcessRecord() {
        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<AttachmentApiClient>();
        using var stream = File.OpenRead(FilePath);
        client.UploadAttachmentAsync(Table, SysId, stream, Path.GetFileName(FilePath), CancellationToken.None).GetAwaiter().GetResult();
    }
}

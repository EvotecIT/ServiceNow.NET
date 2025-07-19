using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.IO;

namespace ServiceNow.PowerShell;

/// <summary>
/// PowerShell cmdlet for uploading an attachment to a ServiceNow record.
/// </summary>
[Cmdlet(VerbsCommon.Add, "ServiceNowAttachment")]
public class AddServiceNowAttachment : PSCmdlet {
    [Parameter(Mandatory = true)]
    /// <summary>
    /// Base URL of the ServiceNow instance.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Username used for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Password used for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Name of the table containing the record.
    /// </summary>
    public string Table { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Sys_id of the record to attach the file to.
    /// </summary>
    public string SysId { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Path to the file to upload.
    /// </summary>
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

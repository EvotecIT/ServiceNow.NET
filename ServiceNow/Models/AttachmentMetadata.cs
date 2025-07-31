namespace ServiceNow.Models;

/// <summary>
/// Represents metadata for a record attachment.
/// </summary>
public class AttachmentMetadata {
    /// <summary>
    /// Gets or sets the unique identifier of the attachment.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the attachment file name.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Gets or sets the download URL.
    /// </summary>
    public string? DownloadLink { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the file.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the table containing the record.
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// Gets or sets the sys_id of the associated record.
    /// </summary>
    public string? TableSysId { get; set; }
}

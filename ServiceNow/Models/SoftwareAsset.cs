namespace ServiceNow.Models;

/// <summary>
/// Represents a software asset record.
/// </summary>
public class SoftwareAsset {
    /// <summary>
    /// Gets or sets the unique identifier of the asset.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the name of the asset.
    /// </summary>
    public string? Name { get; set; }
}

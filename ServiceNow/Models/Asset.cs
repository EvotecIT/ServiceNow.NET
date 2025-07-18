namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow asset record.
/// </summary>
public class Asset {
    /// <summary>
    /// Gets or sets the unique identifier of the asset.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the asset tag value.
    /// </summary>
    public string? AssetTag { get; set; }

    /// <summary>
    /// Gets or sets the display name of the asset.
    /// </summary>
    public string? Name { get; set; }
}

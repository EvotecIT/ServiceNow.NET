namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow configuration item (CMDB) record.
/// </summary>
public class ConfigurationItem {
    /// <summary>
    /// Gets or sets the unique identifier of the configuration item.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the name of the configuration item.
    /// </summary>
    public string? Name { get; set; }
}

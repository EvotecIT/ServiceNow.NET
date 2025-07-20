namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow sys_domain record.
/// </summary>
public class SysDomain {
    /// <summary>
    /// Gets or sets the unique identifier of the domain record.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the domain name.
    /// </summary>
    public string? Name { get; set; }
}

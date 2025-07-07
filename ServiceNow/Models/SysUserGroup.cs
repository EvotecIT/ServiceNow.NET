namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow sys_user_group record.
/// </summary>
public class SysUserGroup {
    /// <summary>
    /// Gets or sets the unique identifier of the group record.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the group description.
    /// </summary>
    public string? Description { get; set; }
}

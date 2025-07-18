namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow sys_user_role record.
/// </summary>
public class SysUserRole {
    /// <summary>
    /// Gets or sets the unique identifier of the role.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the role name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the role description.
    /// </summary>
    public string? Description { get; set; }
}

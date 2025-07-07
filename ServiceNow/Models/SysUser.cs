namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow sys_user record.
/// </summary>
public class SysUser {
    /// <summary>
    /// Gets or sets the unique identifier of the user record.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the full display name of the user.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string? Email { get; set; }
}

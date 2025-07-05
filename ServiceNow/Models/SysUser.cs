namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow sys_user record.
/// </summary>
public class SysUser {
    public string? SysId { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}

namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow user.
/// </summary>
public class User
{
    public string? SysId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
}

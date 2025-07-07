namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow user.
/// </summary>
public class User {
    /// <summary>
    /// Gets or sets the unique identifier of the user record.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string? Email { get; set; }
}
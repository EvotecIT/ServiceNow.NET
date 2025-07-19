namespace ServiceNow.Models;

/// <summary>
/// Represents a generic GRC item.
/// </summary>
public class GrcItem {
    /// <summary>
    /// Gets or sets the unique identifier of the item.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the item name or number.
    /// </summary>
    public string? Name { get; set; }
}

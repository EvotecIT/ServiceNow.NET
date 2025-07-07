namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow catalog item.
/// </summary>
public class CatalogItem {
    /// <summary>
    /// Gets or sets the unique identifier of the catalog item.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public string? Name { get; set; }
}

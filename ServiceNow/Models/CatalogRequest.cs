namespace ServiceNow.Models;

/// <summary>
/// Represents a catalog request.
/// </summary>
public class CatalogRequest {
    /// <summary>
    /// Gets or sets the unique identifier of the catalog request.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the request number.
    /// </summary>
    public string? Number { get; set; }
}

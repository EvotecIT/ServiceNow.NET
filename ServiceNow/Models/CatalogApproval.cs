using ServiceNow.Enums;

namespace ServiceNow.Models;

/// <summary>
/// Represents an approval record for a catalog request.
/// </summary>
public class CatalogApproval {
    /// <summary>
    /// Gets or sets the unique identifier of the approval record.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the current approval state.
    /// </summary>
    public CatalogApprovalState State { get; set; }
}

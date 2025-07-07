using ServiceNow.Enums;

namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow incident record.
/// </summary>
public class Incident {
    /// <summary>
    /// Gets or sets the unique identifier of the incident.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the incident number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets a brief description of the incident.
    /// </summary>
    public string? ShortDescription { get; set; }

    /// <summary>
    /// Gets or sets the current incident state.
    /// </summary>
    public IncidentState State { get; set; }
}
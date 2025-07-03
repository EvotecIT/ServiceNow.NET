using ServiceNow.Enums;

namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow incident record.
/// </summary>
public class Incident {
    public string? SysId { get; set; }
    public string? Number { get; set; }
    public string? ShortDescription { get; set; }
    public IncidentState State { get; set; }
}
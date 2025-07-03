namespace ServiceNow.Models;

/// <summary>
/// Represents a generic task record in ServiceNow.
/// </summary>
public class TaskRecord {
    public string? SysId { get; set; }
    public string? Number { get; set; }
    public string? ShortDescription { get; set; }
}
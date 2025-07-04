namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow problem record.
/// </summary>
public class Problem {
    public string? SysId { get; set; }
    public string? Number { get; set; }
    public string? ShortDescription { get; set; }
}

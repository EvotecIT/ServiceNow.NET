namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow change request record.
/// </summary>
public class ChangeRequest {
    public string? SysId { get; set; }
    public string? Number { get; set; }
    public string? ShortDescription { get; set; }
}

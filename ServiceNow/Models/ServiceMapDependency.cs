namespace ServiceNow.Models;

/// <summary>
/// Represents a dependency between two service map nodes.
/// </summary>
public class ServiceMapDependency {
    public string? Parent { get; set; }
    public string? Child { get; set; }
}

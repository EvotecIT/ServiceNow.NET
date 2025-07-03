namespace ServiceNow.Enums;

/// <summary>
/// Represents the state of an incident.
/// </summary>
public enum IncidentState {
    New = 1,
    InProgress = 2,
    OnHold = 3,
    Resolved = 6,
    Closed = 7,
    Canceled = 8
}
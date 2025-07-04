using System.ComponentModel.DataAnnotations;

namespace ServiceNow.Enums;

/// <summary>
/// Represents the state of an incident.
/// </summary>
public enum IncidentState {
    [Display(Name = "New")]
    New = 1,

    [Display(Name = "In Progress")]
    InProgress = 2,

    [Display(Name = "On Hold")]
    OnHold = 3,

    [Display(Name = "Resolved")]
    Resolved = 6,

    [Display(Name = "Closed")]
    Closed = 7,

    [Display(Name = "Canceled")]
    Canceled = 8
}
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ServiceNow.Enums;

/// <summary>
/// Represents the state of an incident.
/// </summary>
public enum IncidentState {
    [Display(Name = "New")]
    [EnumMember(Value = "New")]
    New = 1,

    [Display(Name = "In Progress")]
    [EnumMember(Value = "In Progress")]
    InProgress = 2,

    [Display(Name = "On Hold")]
    [EnumMember(Value = "On Hold")]
    OnHold = 3,

    [Display(Name = "Resolved")]
    [EnumMember(Value = "Resolved")]
    Resolved = 6,

    [Display(Name = "Closed")]
    [EnumMember(Value = "Closed")]
    Closed = 7,

    [Display(Name = "Canceled")]
    [EnumMember(Value = "Canceled")]
    Canceled = 8
}
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ServiceNow.Enums;

/// <summary>
/// Represents the state of a change request.
/// </summary>
public enum ChangeRequestState {
    [Display(Name = "New")]
    [EnumMember(Value = "New")]
    New = 1,

    [Display(Name = "Assess")]
    [EnumMember(Value = "Assess")]
    Assess = 2,

    [Display(Name = "Authorize")]
    [EnumMember(Value = "Authorize")]
    Authorize = 3,

    [Display(Name = "Scheduled")]
    [EnumMember(Value = "Scheduled")]
    Scheduled = 4,

    [Display(Name = "Implement")]
    [EnumMember(Value = "Implement")]
    Implement = 5,

    [Display(Name = "Review")]
    [EnumMember(Value = "Review")]
    Review = 6,

    [Display(Name = "Closed")]
    [EnumMember(Value = "Closed")]
    Closed = 7,

    [Display(Name = "Canceled")]
    [EnumMember(Value = "Canceled")]
    Canceled = 8
}
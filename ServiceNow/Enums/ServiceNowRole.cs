using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ServiceNow.Enums;

/// <summary>
/// Common ServiceNow roles.
/// </summary>
public enum ServiceNowRole {
    [Display(Name = "Admin")]
    [EnumMember(Value = "Admin")]
    Admin,

    [Display(Name = "ITIL")]
    [EnumMember(Value = "ITIL")]
    ITIL,

    [Display(Name = "Approver")]
    [EnumMember(Value = "Approver")]
    Approver,

    [Display(Name = "User")]
    [EnumMember(Value = "User")]
    User
}
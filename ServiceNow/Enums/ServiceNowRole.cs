using System.ComponentModel.DataAnnotations;

namespace ServiceNow.Enums;

/// <summary>
/// Common ServiceNow roles.
/// </summary>
public enum ServiceNowRole {
    [Display(Name = "Admin")]
    Admin,

    [Display(Name = "ITIL")]
    ITIL,

    [Display(Name = "Approver")]
    Approver,

    [Display(Name = "User")]
    User
}
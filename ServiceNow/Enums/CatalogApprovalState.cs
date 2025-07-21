using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ServiceNow.Enums;

/// <summary>
/// Possible approval states for catalog requests.
/// </summary>
public enum CatalogApprovalState {
    [Display(Name = "Requested")]
    [EnumMember(Value = "Requested")]
    Requested,

    [Display(Name = "Approved")]
    [EnumMember(Value = "Approved")]
    Approved,

    [Display(Name = "Rejected")]
    [EnumMember(Value = "Rejected")]
    Rejected,

    [Display(Name = "Canceled")]
    [EnumMember(Value = "Canceled")]
    Canceled
}

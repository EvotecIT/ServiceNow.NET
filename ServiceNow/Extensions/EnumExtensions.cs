using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ServiceNow.Enums;

namespace ServiceNow.Extensions;

/// <summary>
/// Extension methods for formatting enumeration values.
/// </summary>
public static class EnumExtensions {
    /// <summary>
    /// Gets the display string for an incident state value.
    /// </summary>
    /// <param name="state">The incident state.</param>
    public static string ToDisplayString(this IncidentState state)
        => GetDisplayName(state);

    /// <summary>
    /// Gets the display string for a ServiceNow role value.
    /// </summary>
    /// <param name="role">The role value.</param>
    public static string ToDisplayString(this ServiceNowRole role)
        => GetDisplayName(role);

    private static string GetDisplayName(Enum value) {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
    }
}

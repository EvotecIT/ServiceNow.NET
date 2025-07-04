using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ServiceNow.Enums;

namespace ServiceNow.Extensions;

public static class EnumExtensions {
    public static string ToDisplayString(this IncidentState state)
        => GetDisplayName(state);

    public static string ToDisplayString(this ServiceNowRole role)
        => GetDisplayName(role);

    private static string GetDisplayName(Enum value) {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
    }
}

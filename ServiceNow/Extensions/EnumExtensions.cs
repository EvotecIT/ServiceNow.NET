using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ServiceNow.Enums;

namespace ServiceNow.Extensions;

/// <summary>
/// Extension methods for formatting enumeration values.
/// </summary>
public static class EnumExtensions {
    private static readonly ConcurrentDictionary<Type, Dictionary<Enum, string>> _displayNameCache = new();
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
        Type type = value.GetType();
        Dictionary<Enum, string> map = _displayNameCache.GetOrAdd(type, CreateMap);
        if (map.TryGetValue(value, out string? name)) {
            return name;
        }
        return value.ToString();
    }

    private static Dictionary<Enum, string> CreateMap(Type type) {
        Dictionary<Enum, string> map = new();
        foreach (Enum enumValue in Enum.GetValues(type)) {
            MemberInfo? member = type.GetMember(enumValue.ToString()).FirstOrDefault();
            string text = member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? enumValue.ToString();
            map[enumValue] = text;
        }
        return map;
    }
}

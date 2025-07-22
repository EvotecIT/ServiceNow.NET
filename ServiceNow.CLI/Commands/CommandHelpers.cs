using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Models;
using System.Collections.Generic;
using System.Text;

namespace ServiceNow.CLI.Commands;

internal static class CommandHelpers
{
    internal static ServiceProvider BuildProvider(ServiceNowSettings settings)
    {
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        return services.BuildServiceProvider();
    }

    internal static Dictionary<string, string?> ParseFilters(IEnumerable<string> pairs)
    {
        var dict = new Dictionary<string, string?>();
        foreach (var pair in pairs)
        {
            if (string.IsNullOrEmpty(pair))
            {
                continue;
            }

            var idx = pair.IndexOf('=');
            if (idx < 0)
            {
                dict[pair] = null;
                continue;
            }

            var key = pair.Substring(0, idx);
            var value = pair.Substring(idx + 1);
            dict[key] = value;
        }
        return dict;
    }

    internal static string GenerateClass(TableMetadata metadata)
    {
        var className = ToPascal(metadata.Table);
        var sb = new StringBuilder();
        sb.AppendLine("namespace ServiceNow.Models;");
        sb.AppendLine();
        sb.AppendLine($"public class {className} {{");
        foreach (var field in metadata.Fields)
        {
            var type = MapType(field.Type);
            var name = ToPascal(field.Name);
            sb.AppendLine($"    public {type} {name} {{ get; set; }}");
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string ToPascal(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var parts = value.Split('_');
        var sb = new StringBuilder();
        foreach (var p in parts)
        {
            if (p.Length == 0)
            {
                continue;
            }

            sb.Append(char.ToUpperInvariant(p[0]));
            if (p.Length > 1)
            {
                sb.Append(p.Substring(1));
            }
        }

        return sb.ToString();
    }

    private static string MapType(string type) => type switch
    {
        "integer" => "int?",
        "number" => "decimal?",
        "float" => "float?",
        "double" => "double?",
        "boolean" => "bool?",
        "glide_date_time" => "DateTime?",
        "glide_date" => "DateTime?",
        _ => "string?"
    };
}

using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Models;
using ServiceNow;
using System;
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

    internal static TableQueryOptions ParseQueryOptions(IEnumerable<string> pairs)
    {
        var dict = ParseFilters(pairs);

        string? fields = null;
        if (dict.TryGetValue("sysparm_fields", out var f) || dict.TryGetValue("fields", out f))
        {
            fields = f;
            dict.Remove("sysparm_fields");
            dict.Remove("fields");
        }

        string? dv = null;
        if (dict.TryGetValue("sysparm_display_value", out var dvTemp) || dict.TryGetValue("display_value", out dvTemp))
        {
            dv = dvTemp;
            dict.Remove("sysparm_display_value");
            dict.Remove("display_value");
        }

        bool? erl = null;
        if (dict.TryGetValue("sysparm_exclude_reference_link", out var ex) || dict.TryGetValue("exclude_reference_link", out ex))
        {
            if (bool.TryParse(ex, out var b))
            {
                erl = b;
            }
            dict.Remove("sysparm_exclude_reference_link");
            dict.Remove("exclude_reference_link");
        }

        var additional = new Dictionary<string, string?>(dict);

        return new TableQueryOptions
        {
            Fields = fields?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
            DisplayValue = dv,
            ExcludeReferenceLinks = erl,
            AdditionalParameters = additional
        };
    }
}

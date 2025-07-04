using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceNow.Utilities;

/// <summary>
/// Factory for creating <see cref="EnumMemberJsonConverter{T}"/> instances.
/// </summary>
public class EnumMemberJsonConverterFactory : JsonConverterFactory {
    public override bool CanConvert(Type typeToConvert)
        => (Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert).IsEnum;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
        var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        var converterType = typeof(EnumMemberJsonConverter<>).MakeGenericType(enumType);
        var converter = (JsonConverter)Activator.CreateInstance(converterType)!;
        if (typeToConvert == enumType) {
            return converter;
        }
        // nullable wrapper
        var wrapperType = typeof(NullableConverter<>).MakeGenericType(enumType);
        return (JsonConverter)Activator.CreateInstance(wrapperType, converter)!;
    }

    private class NullableConverter<T> : JsonConverter<T?> where T : struct, Enum {
        private readonly JsonConverter<T> _inner;
        public NullableConverter(JsonConverter<T> inner) => _inner = inner;
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null ? null : _inner.Read(ref reader, typeof(T), options);
        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options) {
            if (value is null) {
                writer.WriteNullValue();
            } else {
                _inner.Write(writer, value.Value, options);
            }
        }
    }
}

/// <summary>
/// JSON converter that uses <see cref="EnumMemberAttribute"/> values when serializing enums.
/// </summary>
public class EnumMemberJsonConverter<T> : JsonConverter<T> where T : struct, Enum {
    private readonly Dictionary<T, string> _toString = new();
    private readonly Dictionary<string, T> _fromString = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the converter.
    /// </summary>
    public EnumMemberJsonConverter() {
        var type = typeof(T);
        foreach (var value in Enum.GetValues(type).Cast<T>()) {
            var member = type.GetMember(value.ToString()).First();
            var attr = member.GetCustomAttribute<EnumMemberAttribute>();
            var str = attr?.Value ?? value.ToString();
            _toString[value] = str;
            if (!_fromString.ContainsKey(str)) {
                _fromString[str] = value;
            }
        }
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.String) {
            var text = reader.GetString();
            if (text != null) {
                if (_fromString.TryGetValue(text, out var val)) {
                    return val;
                }
                if (Enum.TryParse(text, true, out val)) {
                    return val;
                }
            }
        }
        if (reader.TokenType == JsonTokenType.Number) {
            var number = reader.GetInt32();
            return (T)Enum.ToObject(typeof(T), number);
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
        if (_toString.TryGetValue(value, out var text)) {
            writer.WriteStringValue(text);
        } else {
            writer.WriteStringValue(value.ToString());
        }
    }
}

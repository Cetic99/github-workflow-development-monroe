using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CashVault.Domain.Common;


public class Enumeration
{
    public int Id { get; private set; }
    public string Code { get; private set; }

    protected Enumeration()
    {
    }

    protected Enumeration(int id, string code)
    {
        Id = id;
        Code = code;
    }

    public override string ToString() => Code;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(System.Reflection.BindingFlags.Public |
                                         System.Reflection.BindingFlags.Static |
                                         System.Reflection.BindingFlags.DeclaredOnly);

        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Enumeration otherValue))
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Code.Equals(otherValue.Code);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Code.GetHashCode();

    public int CompareTo(object other)
    {
        if (other is null)
        {
            return -1;
        }
        else
        {
            return Code.CompareTo(((Enumeration)other).Code);
        }
    }

    public static bool operator ==(Enumeration left, Enumeration right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Enumeration left, Enumeration right)
    {
        return !Equals(left, right);
    }

    public static T? GetByCode<T>(string? code) where T : Enumeration
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        return GetAll<T>().FirstOrDefault(x => x.Code.ToLower() == code.ToLower());
    }

    public static T? GetById<T>(int? id) where T : Enumeration
    {
        if (!id.HasValue) return null;
        return GetAll<T>().FirstOrDefault(x => x.Id == id.Value);
    }

    public static bool Contains<T>(string? code) where T : Enumeration
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        return GetAll<T>().Any(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }
}

public class EnumerationJsonConverter<T> : JsonConverter<T> where T : Enumeration
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var code = reader.GetString();
        return Enumeration.GetByCode<T>(code);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Code);
    }
}


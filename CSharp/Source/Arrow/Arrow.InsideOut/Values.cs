using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arrow.InsideOut;

/// <summary>
/// Base type for all values in the InsideOut universe
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(StructValue), "Struct")]
[JsonDerivedType(typeof(BoolValue), "Bool")]
[JsonDerivedType(typeof(Int32Value), "Int32")]
[JsonDerivedType(typeof(Int64Value), "Int64")]
[JsonDerivedType(typeof(DoubleValue), "Double")]
[JsonDerivedType(typeof(DecimalValue), "Decimal")]
[JsonDerivedType(typeof(TimeSpanValue), "TimeSpan")]
[JsonDerivedType(typeof(DateTimeValue), "DateTime")]
[JsonDerivedType(typeof(StringValue), "String")]
[JsonDerivedType(typeof(Details), "Details")]
[JsonDerivedType(typeof(ExceptionResponse), "ExceptionResponse")]
[JsonDerivedType(typeof(ExecuteResponse), "ExecuteResponse")]
public abstract partial class Value
{
    /// <summary>
    /// Only this assembly can implement the InsideOut types
    /// </summary>
    private protected Value()
    {
    }

    /// <summary>
    /// The .NET type that represents the value
    /// </summary>
    /// <returns></returns>
    public abstract Type Type();
}

/// <summary>
/// A basic value is a type that represents a single value, such as a int or a string
/// </summary>
public abstract class BasicValue : Value
{
    /// <summary>
    /// Returns the value as a boxed object
    /// </summary>
    /// <returns></returns>
    public abstract object? AsObject();
}

/// <summary>
/// A basic value that is based on an existing .NET type
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BasicValue<T> : BasicValue, IEquatable<BasicValue<T>>
{
    /// <summary>
    /// The actual value
    /// </summary>
    [MaybeNull]
    public T Value{get; set;} = default!;

    /// <inheritdoc/>
    public override object? AsObject()
    {
        return this.Value;
    }

    /// <inheritdoc/>
    public override Type Type()
    {
        return typeof(T);
    }

    /// <inheritdoc/>
    public bool Equals(BasicValue<T>? other)
    {
        return other is not null && EqualityComparer<T>.Default.Equals(this.Value, other.Value);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as BasicValue<T>);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.Value?.GetHashCode() ?? 0;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Value?.ToString() ?? "null";
    }    
}

/// <summary>
/// A value that is composed of other values, much like a C struct
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(StructValue), "Struct")]
public abstract class CompositeValue : Value
{
    private protected CompositeValue()
    {
    }
}

/// <summary>
/// Models a C structure
/// </summary>
public sealed class StructValue : CompositeValue
{
    private Dictionary<string, Value>? m_Members;

    /// <summary>
    /// Adds a new member
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentException"></exception>
    public void Add(string name, Value value)
    {
        ArgumentNullException.ThrowIfNull(name);
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));
        ArgumentNullException.ThrowIfNull(value);

        this.Members.Add(name, value);
    }

    /// <inheritdoc/>
    public override Type Type()
    {
        return typeof(StructValue);
    }

        /// <summary>
    /// The members of the struct
    /// </summary>
    public Dictionary<string, Value> Members
    {
        get{return m_Members ??= new();}
        set{m_Members = value;}
    }
}

/// <summary>
/// A boolean
/// </summary>
public sealed class BoolValue : BasicValue<bool>
{
}

/// <summary>
/// An int
/// </summary>
public sealed class Int32Value : BasicValue<int>
{
}

/// <summary>
/// A long
/// </summary>
public sealed class Int64Value : BasicValue<long>
{
}

/// <summary>
/// A double
/// </summary>
public sealed class DoubleValue : BasicValue<double>
{
}

//
public sealed class DecimalValue : BasicValue<decimal>
{
}

public sealed class TimeSpanValue : BasicValue<TimeSpan>
{
}

public sealed class DateTimeValue : BasicValue<DateTime>
{
}

public sealed class StringValue : BasicValue<string?>
{
}

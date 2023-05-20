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
[JsonDerivedType(typeof(SequenceValue), "Sequence")]
[JsonDerivedType(typeof(BoolValue), "Bool")]
[JsonDerivedType(typeof(Int32Value), "Int32")]
[JsonDerivedType(typeof(Int64Value), "Int64")]
[JsonDerivedType(typeof(DoubleValue), "Double")]
[JsonDerivedType(typeof(DecimalValue), "Decimal")]
[JsonDerivedType(typeof(TimeSpanValue), "TimeSpan")]
[JsonDerivedType(typeof(DateTimeValue), "DateTime")]
[JsonDerivedType(typeof(StringValue), "String")]
[JsonDerivedType(typeof(JsonValue), "Json")]
[JsonDerivedType(typeof(NodeDetails), "Details")]
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
/// A value that is composed of an ordered sequence of values
/// </summary>
public sealed class SequenceValue : Value
{
    private List<Value>? m_Values;

    /// <inheritdoc/>
    public override Type Type()
    {
        return typeof(SequenceValue);
    }

    /// <summary>
    /// The values in the sequence
    /// </summary>
    public List<Value> Values
    {
        get{return m_Values ??= new();}
        set{m_Values = value;}
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
    private Dictionary<string, Value>? m_Values;
    
    /// <inheritdoc/>
    public override Type Type()
    {
        return typeof(StructValue);
    }

        /// <summary>
    /// The members of the struct
    /// </summary>
    public Dictionary<string, Value> Values
    {
        get{return m_Values ??= new();}
        set{m_Values = value;}
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

/// <summary>
/// A decimal
/// </summary>
public sealed class DecimalValue : BasicValue<decimal>
{
}

/// <summary>
/// A timespan
/// </summary>
public sealed class TimeSpanValue : BasicValue<TimeSpan>
{
}

/// <summary>
/// A date and time
/// </summary>
public sealed class DateTimeValue : BasicValue<DateTime>
{
}

/// <summary>
/// A string, which may be null
/// </summary>
public sealed class StringValue : BasicValue<string?>
{
}

/// <summary>
/// Although this is a string it tells the user that the value
/// within is a valid json that should be interpreted in some
/// domain specific way.
/// </summary>
public sealed class JsonValue : BasicValue<string?>
{
}

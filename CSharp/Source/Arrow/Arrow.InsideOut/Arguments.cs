using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// Defines a readonly arguement.
/// This interface is covariant to simplify getting arguments out.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReadOnlyArgument<out T>
{
    /// <summary>
    /// The name of the argument
    /// </summary>
    public string Name{get;}

    /// <summary>
    /// The value of the argument
    /// </summary>
    public T Value{get;}

    /// <summary>
    /// The value, but boxed
    /// </summary>
    /// <returns></returns>
    public object? AsObject();

    /// <summary>
    /// The .net type representing the argument
    /// </summary>
    public Type Type();
}

[JsonPolymorphic]
[JsonDerivedType(typeof(BoolArgument), "Bool")]
[JsonDerivedType(typeof(Int32Argument), "Int32")]
[JsonDerivedType(typeof(Int64Argument), "Int64")]
[JsonDerivedType(typeof(DoubleArgument), "Double")]
[JsonDerivedType(typeof(DecimalArgument), "Decimal")]
[JsonDerivedType(typeof(TimeSpanArgument), "TimeSpan")]
[JsonDerivedType(typeof(DateTimeArgument), "DateTime")]
[JsonDerivedType(typeof(DateOnlyArgument), "DateOnly")]
[JsonDerivedType(typeof(TimeOnlyArgument), "TimeOnly")]
[JsonDerivedType(typeof(StringArgument), "String")]
[JsonDerivedType(typeof(SingleItemArgument), "SingleItem")]
[JsonDerivedType(typeof(MultipleItemsArgument), "MultipleItems")]
public abstract class Argument
{
    private protected Argument(string name)
    {
        if(name is null) throw new ArgumentNullException(nameof(name));
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid argument name", nameof(name));

        this.Name = name;
    }

    /// <summary>
    /// The name of the argument
    /// </summary>
    public string Name{get;}

    /// <summary>
    /// The value of the argument, as an object
    /// </summary>
    /// <returns></returns>
    public abstract object? AsObject();

    /// <summary>
    /// The .NET type that implements the argument
    /// </summary>
    /// <returns></returns>
    public abstract Type Type();

    /// <summary>
    /// Renders the argument as a string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Name} = {AsObject()?.ToString()}";
    }
}


/// <summary>
/// A argument that is based on an existing .NET type
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Argument<T> : Argument, IReadOnlyArgument<T>
{
    private protected Argument(string name) : base(name)
    {
    }

    public T Value{get; set;} = default!;

    public override object? AsObject()
    {
        return this.Value;
    }

    public override Type Type()
    {
        return typeof(T);
    }
}

/// <summary>
/// A boolean argument
/// </summary>
public sealed class BoolArgument : Argument<bool>
{
    public BoolArgument(string name) : base(name)
    {
    }
}

/// <summary>
/// An integer argument
/// </summary>
public sealed class Int32Argument : Argument<int>
{
    public Int32Argument(string name) : base(name)
    {
    }
}

/// <summary>
/// A long argument
/// </summary>
public sealed class Int64Argument : Argument<long>
{
    public Int64Argument(string name) : base(name)
    {
    }
}

/// <summary>
/// A double argument
/// </summary>
public sealed class DoubleArgument : Argument<double>
{
    public DoubleArgument(string name) : base(name)
    {
    }
}

/// <summary>
/// A decimal argument
/// </summary>
public sealed class DecimalArgument : Argument<decimal>
{
    public DecimalArgument(string name) : base(name)
    {
    }
}

/// <summary>
/// A timespan argument
/// </summary>
public sealed class TimeSpanArgument : Argument<TimeSpan>
{
    public TimeSpanArgument(string name) : base(name)
    {
    }
}

/// <summary>
/// A date time argument
/// </summary>
public sealed class DateTimeArgument : Argument<DateTime>
{
    public DateTimeArgument(string name) : base(name)
    {
    }
}

/// <summary>
/// A time only argument
/// </summary>
public sealed class TimeOnlyArgument : Argument<TimeOnly>
{
    public TimeOnlyArgument(string name) : base(name)
    {
    }
}

/// <summary>
/// A date only argument
/// </summary>
public sealed class DateOnlyArgument : Argument<DateOnly>
{
    public DateOnlyArgument(string name) : base(name)
    {
    }
}

/// <summary>
/// A string argument
/// </summary>
public sealed class StringArgument : Argument<string?>
{
    public StringArgument(string name) : base(name)
    {
    }

    /// <summary>
    /// Returns the value in the argument if not null, otherwise a default value
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull("defaultValue")]
    public string? ValueOr(string? defaultValue)
    {
        return this.Value ?? defaultValue;
    }
}

/// <summary>
/// An argument made up of a single ordinal item
/// </summary>
public sealed class SingleItemArgument : Argument<OrdinalItem?>
{
    public SingleItemArgument(string name) : base(name)
    {
    }

    public override string ToString()
    {
        var item = this.Value?.ToString();
        return $"{Name} = ({item})";
    }
}

/// <summary>
/// An argument made up of a multiple ordinal items
/// </summary>
public sealed class MultipleItemsArgument : Argument<List<OrdinalItem>>
{
    public MultipleItemsArgument(string name) : base(name)
    {
        this.Value = new();
    }

    public override string ToString()
    {        
        return $"{Name} = (Count = {Value.Count})";
    }
}


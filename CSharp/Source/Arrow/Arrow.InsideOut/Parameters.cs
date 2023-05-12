using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Arrow.Reflection;

namespace Arrow.InsideOut;

/// <summary>
/// Base class for all function parameters
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(BoolParameter), "Bool")]
[JsonDerivedType(typeof(Int32Parameter), "Int32")]
[JsonDerivedType(typeof(Int64Parameter), "Int64")]
[JsonDerivedType(typeof(DoubleParameter), "Double")]
[JsonDerivedType(typeof(DecimalParameter), "Decimal")]
[JsonDerivedType(typeof(TimeSpanParameter), "TimeSpan")]
[JsonDerivedType(typeof(DateTimeParameter), "DateTime")]
[JsonDerivedType(typeof(StringParameter), "String")]
[JsonDerivedType(typeof(SuggestionParameter), "Suggest")]
[JsonDerivedType(typeof(SingleItemParameter), "SingleItem")]
[JsonDerivedType(typeof(MultipleItemsParameter), "MultipleItems")]
public abstract class Parameter
{
    private protected Parameter(string name)
    {
        if(name is null) throw new ArgumentNullException(nameof(name));
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid parameter name", nameof(name));

        this.Name = name;
    }

    /// <summary>
    /// The name of the parameter
    /// </summary>
    public string Name{get;}

    /// <summary>
    /// An optional description of the parameter
    /// </summary>
    public string? Description{get; init;}

    /// <summary>
    /// The default value for the parameter.
    /// A null value means there is no default
    /// </summary>
    /// <returns></returns>
    public abstract object? DefaultAsObject();

    /// <summary>
    /// The type of the parameter
    /// </summary>
    /// <returns></returns>
    public abstract Type Type();

    /// <summary>
    /// Creates an argument of the type expected by the parameter
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract Argument MakeArgumentFromObject(object? value);

    /// <summary>
    /// Renders the parameter as a string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.Name;
    }
}

/// <summary>
/// A typed parameter
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Parameter<T> : Parameter
{
    /// <summary>
    /// Initializes the instance
    /// </summary>
    /// <param name="name"></param>
    public Parameter(string name) : base(name)
    {
    }

    /// <summary>
    /// Makes a strongly typed argument for the parameter
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract Argument<T> MakeArgument(T value);

    [return: MaybeNull]
    public T CoerceToType(object? value)
    {
        return (T)TypeResolver.CoerceToType(typeof(T), value)!;
    }

    /// <inheritdoc/>
    public override Type Type()
    {
        return typeof(T);
    }

    /// <summary>
    /// The default value for the parameter
    /// </summary>
    public T? DefaultValue{get; set;}

    public override object? DefaultAsObject()
    {
        return this.DefaultValue;
    }
}

/// <summary>
/// A boolean parameter
/// </summary>
public sealed class BoolParameter : Parameter<bool>
{
    public BoolParameter(string name) : base(name)
    {
    }

    public override BoolArgument MakeArgument(bool value)
    {
        return new BoolArgument(this.Name){Value = value};
    }

    public override BoolArgument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// An int parameter
/// </summary>
public sealed class Int32Parameter : Parameter<int>
{
    public Int32Parameter(string name) : base(name)
    {
    }

    public override Int32Argument MakeArgument(int value)
    {
        return new Int32Argument(this.Name){Value = value};
    }

    public override Int32Argument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// A long parameter
/// </summary>
public sealed class Int64Parameter : Parameter<long>
{
    public Int64Parameter(string name) : base(name)
    {
    }

    public override Int64Argument MakeArgument(long value)
    {
        return new Int64Argument(this.Name){Value = value};
    }

    public override Int64Argument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// A double parameter
/// </summary>
public sealed class DoubleParameter : Parameter<double>
{
    public DoubleParameter(string name) : base(name)
    {
    }

    public override DoubleArgument MakeArgument(double value)
    {
        return new DoubleArgument(this.Name){Value = value};
    }

    public override DoubleArgument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// A decimal parameter
/// </summary>
public sealed class DecimalParameter : Parameter<decimal>
{
    public DecimalParameter(string name) : base(name)
    {
    }

    public override DecimalArgument MakeArgument(decimal value)
    {
        return new DecimalArgument(this.Name){Value = value};
    }

    public override DecimalArgument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// A time span parameter
/// </summary>
public sealed class TimeSpanParameter : Parameter<TimeSpan>
{
    public TimeSpanParameter(string name) : base(name)
    {
    }

    public override TimeSpanArgument MakeArgument(TimeSpan value)
    {
        return new TimeSpanArgument(this.Name){Value = value};
    }

    public override TimeSpanArgument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// A date time parameter
/// </summary>
public sealed class DateTimeParameter : Parameter<DateTime>
{
    public DateTimeParameter(string name) : base(name)
    {
    }

    public override DateTimeArgument MakeArgument(DateTime value)
    {
        return new DateTimeArgument(this.Name){Value = value};
    }

    public override DateTimeArgument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// A string parameter
/// </summary>
public sealed class StringParameter : Parameter<string?>
{
    public StringParameter(string name) : base(name)
    {
    }

    public override StringArgument MakeArgument(string? value)
    {
        return new StringArgument(this.Name){Value = value};
    }

    public override StringArgument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// A parameter that givens the users a list of suggestions
/// but doesn't require them to select one.
/// </summary>
public sealed class SuggestionParameter : Parameter<string?>
{
    private List<string>? m_Suggestions;

    public SuggestionParameter(string name) : base(name)
    {
    }

    public List<string> Suggestions
    {
        get{return m_Suggestions ??= new();}
        init{m_Suggestions = value;}
    }

    public override Argument<string?> MakeArgument(string? value)
    {
        return new StringArgument(this.Name){Value = value};
    }

    public override Argument MakeArgumentFromObject(object? value)
    {
        return MakeArgument(CoerceToType(value));
    }
}

/// <summary>
/// A parameter which gives the user a series of items to select from.
/// The user can only select one item.
/// </summary>
public sealed class SingleItemParameter : Parameter<OrdinalItem?>
{
    private List<OrdinalItem>? m_Items;

    public SingleItemParameter(string name) : base(name)
    {
    }

    /// <summary>
    /// The items that the user can choose from
    /// </summary>
    public List<OrdinalItem> Items
    {
        get{return m_Items ??= new();}
        init{m_Items = value;}
    }

    public override SingleItemArgument MakeArgument(OrdinalItem? value)
    {
        var item = value switch
        {
            OrdinalItem i => this.Items.Single(o => o.Equals(i)),
            null          => null
        };

        return new(this.Name)
        {
            Value = item
        };
    }

    public override SingleItemArgument MakeArgumentFromObject(object? value)
    {
        var item = value switch
        {
            OrdinalItem o => this.Items.Single(other => other.Equals(o)),
            long ordinal  => this.Items.Single(other => other.Ordinal == ordinal),
            int ordinal   => this.Items.Single(other => other.Ordinal == ordinal),
            string name   => this.Items.Single(other => other.Name == name),
            null          => null,
            _             => throw new InsideOutException("value cannot be mapped to a single item")
        };

        return new(this.Name)
        {
            Value = item
        };
    }
}

public sealed class MultipleItemsParameter : Parameter<List<OrdinalItem>>
{
    private List<OrdinalItem>? m_Items;

    public MultipleItemsParameter(string name) : base(name)
    {
    }

     /// <summary>
    /// The items that the user can choose from
    /// </summary>
    public List<OrdinalItem> Items
    {
        get{return m_Items ??= new();}
        init{m_Items = value;}
    }

    public override MultipleItemsArgument MakeArgument(List<OrdinalItem> value)
    {
        if(value is null) throw new ArgumentNullException(nameof(value));

        var argument = new MultipleItemsArgument(this.Name);
        argument.Value.AddRange(value);

        return argument;
    }

    public override MultipleItemsArgument MakeArgumentFromObject(object? value)
    {
        IReadOnlyList<OrdinalItem> items = value switch
        {
            OrdinalItem o              => this.Items.Where(other => other.Equals(o)).ToArray(),
            long ordinal               => this.Items.Where(other => other.Ordinal == ordinal).ToArray(),
            int ordinal                => this.Items.Where(other => other.Ordinal == ordinal).ToArray(),
            string name                => this.Items.Where(other => other.Name == name).ToArray(),
            IEnumerable<OrdinalItem> i => i.ToArray(),
            null                       => Array.Empty<OrdinalItem>(),
            _                          => throw new InsideOutException("value cannot be mapped to a single item")
        };

        var argument = new MultipleItemsArgument(this.Name);
        argument.Value.AddRange(items);

        return argument;
    }
}
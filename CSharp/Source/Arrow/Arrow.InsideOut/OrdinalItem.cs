using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// An item within a sequence which has a name and a number.
/// </summary>
public sealed partial class OrdinalItem : IEquatable<OrdinalItem>, IComparable<OrdinalItem>
{
    /// <summary>
    /// Initializes the instance
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ordinal"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OrdinalItem(string name, long ordinal)
    {
        if(name is null) throw new ArgumentNullException(nameof(name));

        this.Name = name;
        this.Ordinal = ordinal;
    }

    /// <summary>
    /// The name of the indexed item
    /// </summary>
    public string Name{get;}

    /// <summary>
    /// The ordinal of the item
    /// </summary>
    public long Ordinal{get;}

    /// <inheritdoc/>
    public int CompareTo(OrdinalItem? other)
    {
        if(other is null) return 1;

        return (this.Ordinal, this.Name).CompareTo((other.Ordinal, other.Name));
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as OrdinalItem);
    }

    /// <inheritdoc/>
    public bool Equals(OrdinalItem? other)
    {
        return other is not null && 
               other.Ordinal == this.Ordinal && 
               other.Name == this.Name;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (this.Ordinal, this.Name).GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name} => {Ordinal}";
    }
}

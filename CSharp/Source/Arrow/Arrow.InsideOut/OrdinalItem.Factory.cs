using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

public sealed partial class OrdinalItem
{
    /// <summary>
    /// Converts a sequence of values into an list
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<OrdinalItem> AsList(params object[] values)
    {
        return MakeList(0, values);
    }

    /// <summary>
    /// Converts a sequence of values into an list
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<OrdinalItem> AsList(IEnumerable<object> values)
    {
        return MakeList(0, values);
    }

    /// <summary>
    /// Converts an enum into a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<OrdinalItem> AsList<T>() where T : struct, Enum
    {
        var enumValues = EnumCache<T>.Values;

        var list = new List<OrdinalItem>(enumValues.Count);
        
        for(var i = 0; i < enumValues.Count; i++)
        {
            var @enum = enumValues[i];
            var asLong = Convert.ToInt64(@enum);
            var ordinalItem = new OrdinalItem(@enum.ToString(), asLong);
            list.Add(ordinalItem);
        }

        return list;
    }

    private static List<OrdinalItem> MakeList(long index, IEnumerable<object> values)
    {
        if(values is null) throw new ArgumentNullException(nameof(values));

        var list = new List<OrdinalItem>();

        foreach(var value in values)
        {
            var name = value.ToString();
            if(name is null) throw new ArgumentException("value evaluated to null", nameof(values));

            var ordinalItem = new OrdinalItem(name, index);
            list.Add(ordinalItem);

            index++;
        }

        return list;
    }

    private static class EnumCache<T> where T : struct, Enum
    {
        public static readonly IReadOnlyList<T> Values = Enum.GetValues<T>();
    }
}

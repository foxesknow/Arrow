using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

public abstract partial class Value
{
    public static BoolValue From(bool value)
    {
        return new(){Value = value};
    }

    public static Int32Value From(int value)
    {
        return new(){Value = value};
    }

    public static Int64Value From(long  value)
    {
        return new(){Value = value};
    }

    public static DoubleValue From(double value)
    {
        return new(){Value = value};
    }

    public static DecimalValue From(decimal value)
    {
        return new(){Value = value};
    }

    public static TimeSpanValue From(TimeSpan value)
    {
        return new(){Value = value};
    }

    public static DateTimeValue From(DateTime value)
    {
        return new(){Value = value};
    }

    public static StringValue From(string? value)
    {
        return new(){Value = value};
    }
}

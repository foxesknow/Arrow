using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;

namespace Arrow.Windows.Data
{
	/// <summary>
	/// Allows a string to be formatted using a .NET format string.
	/// You specify the format string in the <b>ConverterParameter</b> xaml markup
	/// Note that when using .NET format braces they must be escaped using \{ and \}
	/// For example:
	/// <code>
	///    <TextBlock Text="{Binding Path=Now, Converter={StaticResource Format}, ConverterParameter='Today is \{0\}'}"/>
	/// </code>
	/// </summary>
	[ValueConversion(typeof(object),typeof(string))]
	public class FormatConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string formatString=parameter as string;
            if(formatString!=null)
            {
                return string.Format(culture,formatString,value);
            }
            else
            {
                return value.ToString();
            }
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}

		#endregion
	}
}

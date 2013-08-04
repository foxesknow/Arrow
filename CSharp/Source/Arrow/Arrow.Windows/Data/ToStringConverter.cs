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
	/// Calls the ToString method on an object.
	/// If the object supports the IFormattable interface then this method is used
	/// and the <b>ConverterParameter</b> is passed to the method
	/// 
	/// For example:
	/// <code>
	///    <TextBlock Text="{Binding Path=Now, Converter={StaticResource ToString}, ConverterParameter=ddMMyyyy}"/>
	/// </code>
	/// </summary>
	[ValueConversion(typeof(object),typeof(string))]
	public class ToStringConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string formatString=parameter as string;
			if(formatString!=null)
			{
				IFormattable formattable=value as IFormattable;
				if(formattable!=null)
				{
					return formattable.ToString(formatString,null);
				}
				else
				{
					return value.ToString();
				}
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

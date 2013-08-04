using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Arrow.Windows.Data
{
	/// <summary>
	/// Takes a boolean value and negates the value
	/// </summary>
	[ValueConversion(typeof(bool),typeof(bool))]
	public class NegationConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool current=bool.Parse(value.ToString());
			return !current;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool current=bool.Parse(value.ToString());
			return !current;
		}

		#endregion
	}
}

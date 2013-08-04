using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Arrow.Windows.Data
{
	/// <summary>
	/// Converts a string to it's Trim()d state
	/// </summary>
	[ValueConversion(typeof(string),typeof(string))]
	public class StringTrimConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value,Type targetType,object parameter,CultureInfo culture)
		{
			if(value==null) return null;
			return value.ToString().Trim();
		}

		object IValueConverter.ConvertBack(object value,Type targetType,object parameter,CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

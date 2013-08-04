using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Arrow.Windows.Data
{
	/// <summary>
	/// Converts a string containing nothing but whitespace to true,
	/// and everything else to false
	/// </summary>
	[ValueConversion(typeof(string),typeof(bool))]
	public class IsAllWhitespaceConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value,Type targetType,object parameter,CultureInfo culture)
		{
			if(value==null) return true;
			
			string text=value.ToString().Trim();
			return text=="";
		}

		object IValueConverter.ConvertBack(object value,Type targetType,object parameter,CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

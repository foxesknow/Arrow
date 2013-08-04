using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Arrow.Windows.Data
{
	/// <summary>
	/// Converts a string that contains text to boolean true.
	/// If the string just contains whitespace then false will be returned
	/// </summary>
	[ValueConversion(typeof(string),typeof(bool))]
	public class StringToBoolConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value,Type targetType,object parameter,CultureInfo culture)
		{
			if(value==null) return false;
			
			string text=value.ToString().Trim();
			return text!="";
		}

		object IValueConverter.ConvertBack(object value,Type targetType,object parameter,CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

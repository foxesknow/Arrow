using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Arrow.Windows.Data
{
	/// <summary>
	/// Maps a zero number to false, and anything else to true
	/// </summary>
	[ValueConversion(typeof(int),typeof(bool))]
	public class BooleanNumberConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value==null) return false;
			int number=Convert.ToInt32(value);
			return number!=0 ? true : false;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool b=bool.Parse(value.ToString());
			return b ? 1 : 0;
		}

		#endregion
	}
}

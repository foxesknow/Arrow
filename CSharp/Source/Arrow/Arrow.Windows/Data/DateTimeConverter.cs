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
	/// Allows you to format a DateTime instance
	/// For example:
	/// <code>
	///   <TextBlock Text='{Binding Path=SomeDate, Converter={StaticResource formatDateTime}, ConverterParameter="dd/MM/yyyy"}'/>
	/// </code>
	/// 
	/// where the converter has been referenced in the window resources:
	/// <code>
	/// <![CDATA[
	///   <arrowWindowsData:DateTimeConverter x:Key="formatDateTime" />
	/// ]]>
	/// </code>
	/// 
	/// </summary>
	[ValueConversion(typeof(DateTime),typeof(string))]
	public class DateTimeConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string formatString=parameter as string;
			DateTime dateTime=(DateTime)value;
			
            if(formatString!=null)
            {
                return dateTime.ToString(formatString);
            }
            else
            {
                return dateTime.ToString();
            }
		}

		object IValueConverter.ConvertBack(object value,Type targetType,object parameter,CultureInfo culture)
		{
			return Binding.DoNothing;
		}

		#endregion
	}
}

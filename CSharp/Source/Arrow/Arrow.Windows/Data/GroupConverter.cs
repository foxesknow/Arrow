using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;

namespace Arrow.Windows.Data
{
	/// <summary>
	/// Calls a sequence of converters.
	/// For example:
	/// <code>
	/// <![CDATA[
	/// <local:GroupConverter>
	///		<local:NegationConverter>
	///		<local:BooleanNumberConverter>
	/// </local:GroupConverter>
	/// ]]>
	/// </code>
	/// </summary>
	[ContentProperty("Converters")]
	[ValueConversion(typeof(object),typeof(object))]
	public class GroupConverter : DependencyObject, IValueConverter
	{
		private static readonly DependencyProperty s_Converters=DependencyProperty.Register
		(
			"Converters",typeof(Collection<IValueConverter>),typeof(GroupConverter)
		);
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public GroupConverter()
		{
			this.Converters=new Collection<IValueConverter>();
		}
		
		/// <summary>
		/// The converters that will be applied
		/// </summary>
		public Collection<IValueConverter> Converters
		{
			get{return GetValue(s_Converters) as Collection<IValueConverter>;}
			private set{SetValue(s_Converters,value);}
		}
	
		#region IValueConverter Members

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Collection<IValueConverter> converters=this.Converters;
			
			if(converters.Count==0) return Binding.DoNothing;
			
			foreach(IValueConverter converter in converters)
			{
				value=converter.Convert(value,targetType,parameter,culture);
			}
			
			return value;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Collection<IValueConverter> converters=this.Converters;
			
			if(converters.Count==0) return Binding.DoNothing;
			
			foreach(IValueConverter converter in converters.Reverse())
			{
				value=converter.Convert(value,targetType,parameter,culture);
			}
			
			return value;
		}

		#endregion
	}
}

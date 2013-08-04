using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Arrow.Windows.Data
{
	/// <summary>
	/// Calls the ConvertBack method of an attached Converter when Convert is called
	/// </summary>
	[ContentProperty("Converter")]
	[ValueConversion(typeof(object),typeof(object))]
	public class ConvertBackConverter : DependencyObject, IValueConverter
	{
		private static readonly DependencyProperty s_Converter=DependencyProperty.Register
		(
			"Converter",typeof(IValueConverter),typeof(ConvertBackConverter)
		);
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public ConvertBackConverter() : this(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="converter">The converter to delegate to. May be null</param>
		public ConvertBackConverter(IValueConverter converter)
		{
			this.Converter=converter;
		}
		
		/// <summary>
		/// The converter whose ConvertBack method will be called
		/// </summary>
		public IValueConverter Converter
		{
			get{return GetValue(s_Converter) as IValueConverter;}
			set{SetValue(s_Converter,value);}
		}
	
		#region IValueConverter Members

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IValueConverter converter=this.Converter;
			if(converter==null) return DependencyProperty.UnsetValue;
			
			return converter.ConvertBack(value,targetType,parameter,culture);
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IValueConverter converter=this.Converter;
			if(converter==null) return Binding.DoNothing;
			
			return converter.Convert(value,targetType,parameter,culture);
		}

		#endregion
	}
}

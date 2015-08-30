using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace Arrow.PowerShell.Calendar
{
	[TypeConverter(typeof(BusinessDateConverter))]
	public class BusinessDate : IEquatable<BusinessDate>, IComparable<BusinessDate>
	{
		private readonly int m_Year;
		private readonly int m_Month;
		private readonly int m_Day;

		private readonly int m_EncodedValue;

		public BusinessDate(int year, int month, int day)
		{
			m_Year=year;
			m_Month=month;
			m_Day=day;

			m_EncodedValue=(((year*100)+month)*100)+day;
		}

		public int Year
		{
			get{return m_Year;}
		}

		public int Month
		{
			get{return m_Month;}
		}

		public int Day
		{
			get{return m_Day;}
		}

		public int EncodedValue
		{
			get{return m_EncodedValue;}
		}

		public override string ToString()
		{
			return string.Format("{0:0000}-{1:00}-{2:00}",m_Year,m_Month,m_Day);
		}

		public override int GetHashCode()
		{
			return m_EncodedValue;
		}

		public override bool Equals(object obj)
		{
			if(obj==null) return false;

			var rhs=obj as BusinessDate;
			if(object.ReferenceEquals(rhs,null))
			{
				return false;
			}
			else
			{
				return Equals((BusinessDate)obj);
			}
		}

		public bool Equals(BusinessDate other)
		{
			if(object.ReferenceEquals(other,null)) return false;

			return m_EncodedValue==other.m_EncodedValue;
		}


		public int CompareTo(BusinessDate other)
		{
			return m_EncodedValue.CompareTo(other.m_EncodedValue);
		}		
	}

	public class BusinessDateConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType==typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text=value.ToString();

			DateTime date;
			if(DateTime.TryParseExact(text,"yyyy-MM-dd",null,DateTimeStyles.None,out date))
			{
				return new BusinessDate(date.Year,date.Month,date.Day);
			}

			if(DateTime.TryParseExact(text,"yyyy/MM/dd",null,DateTimeStyles.None,out date))
			{
				return new BusinessDate(date.Year,date.Month,date.Day);
			}

			if(DateTime.TryParseExact(text,"yyyyMMdd",null,DateTimeStyles.None,out date))
			{
				return new BusinessDate(date.Year,date.Month,date.Day);
			}

			return base.ConvertFrom(context,culture,value);
		}
	}
}

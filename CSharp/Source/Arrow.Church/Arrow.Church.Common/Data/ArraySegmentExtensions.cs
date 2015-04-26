using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Data
{
	public static class ArraySegmentExtensions
	{
		public static IList<ArraySegment<byte>> ToList(this ArraySegment<byte> segment)
		{
			var list=new List<ArraySegment<byte>>(1);
			list.Add(segment);

			return list;
		}

		public static int TotalLength<T>(this IList<ArraySegment<T>> segments)
		{
			int length=0;

			for(int i=0; i<segments.Count; i++)
			{
				length+=segments[i].Count;
			}

			return length;
		}

		public static T[] ToArray<T>(this IList<ArraySegment<T>> segments)
		{
			int totalLength=segments.TotalLength();
			var data=new T[totalLength];
			int offset=0;

			for(int i=0; i<segments.Count; i++)
			{
				var segment=segments[i];
				Array.Copy(segment.Array,segment.Offset,data,offset,segment.Count);
				offset+=segment.Count;
			}

			return data;
		}
	}
}

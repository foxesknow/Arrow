using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Useful extension methods
	/// </summary>
	public static class MessageExtensions
	{
		/// <summary>
		/// Create a copy of a text message
		/// </summary>
		/// <param name="source">The message to copy</param>
		/// <param name="sender">The sender that will be used to create the new message instance</param>
		/// <returns>A new message that is valid for the sender</returns>
		public static ITextMessage CopyInto(this ITextMessage source, MessageSender sender)
		{
			if(source==null) throw new ArgumentNullException("source");
			if(sender==null) throw new ArgumentNullException("sender");

			var copy=sender.CreateTextMessage(source.Text);
			CopyBase(copy,source);

			return copy;
		}

		/// <summary>
		/// Create a copy of a map message
		/// </summary>
		/// <param name="source">The message to copy</param>
		/// <param name="sender">The sender that will be used to create the new message instance</param>
		/// <returns>A new message that is valid for the sender</returns>
		public static IMapMessage CopyInto(this IMapMessage source, MessageSender sender)
		{
			if(source==null) throw new ArgumentNullException("source");
			if(sender==null) throw new ArgumentNullException("sender");

			var copy=sender.CreateMapMessage();
			CopyBase(copy,source);

			foreach(string name in source.Names)
			{
				object value=source.GetObject(name);
				copy.SetObject(name,value);
			}

			return copy;
		}

		/// <summary>
		/// Create a copy of a byte message
		/// </summary>
		/// <param name="source">The message to copy</param>
		/// <param name="sender">The sender that will be used to create the new message instance</param>
		/// <returns>A new message that is valid for the sender</returns>
		public static IByteMessage CopyInto(this IByteMessage source, MessageSender sender)
		{
			if(source==null) throw new ArgumentNullException("source");
			if(sender==null) throw new ArgumentNullException("sender");

			byte[] buffer=CloneArray(source.Data);
			var copy=sender.CreateByteMessage(buffer);
			CopyBase(copy,source);
			
			return copy;
		}

		/// <summary>
		/// Create a copy of an object message
		/// </summary>
		/// <param name="source">The message to copy</param>
		/// <param name="sender">The sender that will be used to create the new message instance</param>
		/// <returns>A new message that is valid for the sender</returns>
		public static IObjectMessage CopyInto(this IObjectMessage source, MessageSender sender)
		{
			if(source==null) throw new ArgumentNullException("source");
			if(sender==null) throw new ArgumentNullException("sender");

			var copy=sender.CreateObjectMessage(source.TheObject);
			CopyBase(copy,source);

			return copy;
		}

		private static void CopyBase(IMessage destination, IMessage source)
		{
			destination.CorrelationID=source.CorrelationID;
			destination.MessageID=source.MessageID;
			destination.MessageType=source.MessageType;

			foreach(string name in source.PropertyNames)
			{
				object value=source.GetProperty(name);
				destination.SetProperty(name,value);
			}
		}

		private static byte[] CloneArray(byte[] data)
		{
			if(data==null) return data;
			
			byte[] copy=new byte[data.Length];
			Array.Copy(data,copy,data.Length);
			
			return copy;
		}
	}
}

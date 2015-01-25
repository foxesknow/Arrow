using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Execution;
using Arrow.Messaging.Support;

using Arrow.Messaging.Memory.Messages;

namespace Arrow.Messaging.Memory
{
	/// <summary>
	/// Sends messages into the memory system
	/// </summary>
	public class MemoryMessageSender : MessageSender
	{
		private Session m_Session;
		
		/// <summary>
		/// Sends a message.
		/// The message must have been created via a call to one of the CreateXXXMessage methods.
		/// </summary>
		/// <param name="message"></param>
		public override void Send(IMessage message)
		{
			if(message==null) throw new ArgumentNullException("message");
			if(m_Session==null) throw new InvalidOperationException("not connected");
			if(IsValidMessage(message)==false) throw new ArgumentException("not a memory message");

			m_Session.Send(message);
		}

		public override ITextMessage CreateTextMessage(string text)
		{
			if(m_Session==null) throw new InvalidOperationException("not connected");
			return new MemTextMessage(text);
		}

		public override IByteMessage CreateByteMessage(byte[] data)
		{
			if(m_Session==null) throw new InvalidOperationException("not connected");
			if(data==null) throw new ArgumentNullException("data");

			return new MemByteMessage(data);
		}

		public override IMapMessage CreateMapMessage()
		{
			if(m_Session==null) throw new InvalidOperationException("not connected");
			return new MemMapMessage();
		}

		
		public override IObjectMessage CreateObjectMessage(object theObject)
		{
			if(m_Session==null) throw new InvalidOperationException("not connected");
			if(theObject==null) throw new ArgumentNullException("theObject");
			if(theObject.GetType().IsSerializable==false) throw new ArgumentException("theObject is not serializable");

			return new MemObjectMessage(theObject);
		}

		public override void Connect(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			if(m_Session!=null) throw new InvalidOperationException("already connected");

			m_Session=ConnectionManager.GetSession(uri);
		}

		public override void Disconnect()
		{
			if(m_Session!=null)
			{
				MethodCall.AllowFail(()=>m_Session.Close());
				m_Session=null;
			}
		}

		public override bool IsConnected
		{
			get{return m_Session!=null;}
		}

		private bool IsValidMessage(IMessage message)
		{
			return message is IMemoryMessage;
		}
	}
}

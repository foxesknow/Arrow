using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Execution;
using Arrow.Threading;

namespace Arrow.Messaging.Memory
{
	/// <summary>
	/// Provides the ability to receive memory messages
	/// </summary>
	public class MemoryMessageReceiver : MessageReceiver
	{
		private volatile Session m_Session;

		public override event EventHandler<MessageEventArgs> MessageReceived;

		/// <summary>
		/// Always returns false.
		/// Users of this class must receive by subscribing to the MessageReceived event
		/// </summary>
		public override bool CanReceive
		{
			get{return false;}
		}

        /// <summary>
        /// Throws NotImplementedException
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public override IMessage Receive(long milliseconds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connects to the underlying messaging system
        /// </summary>
        /// <param name="uri">The address to connect to</param>
        public override void Connect(Uri uri)
        {
            if(uri == null) throw new ArgumentNullException("uri");
            if(m_Session != null) throw new InvalidOperationException("already connected");

            m_Session = ConnectionManager.GetSession(uri);
            m_Session.MessageAvailable += MessageAvailable;
        }

        /// <summary>
        /// Disconnects from the messaging system
        /// </summary>
        public override void Disconnect()
        {
            if(m_Session != null)
            {
                MethodCall.AllowFail(() =>
                {
                    m_Session.MessageAvailable -= MessageAvailable;
                    m_Session.Close();
                });

                m_Session = null;
                MessageReceived = null;
            }
        }

        /// <summary>
        /// Indicates if the receiver is currently connected
        /// </summary>
        public override bool IsConnected
		{
			get{return m_Session != null;}
		}

        private void MessageAvailable(object sender, MessageEventArgs args)
        {
            var session = m_Session;
            if(session == null) return;

            MethodCall.AllowFail(() =>
            {
                IMessage message;

                if(args != null && args.Message != null)
                {
                    message = args.Message;
                }
                else
                {
                    session.TryGetMessage(out message);
                }

                if(message != null)
                {
                    var d = MessageReceived;
                    if(d != null) d(this, new MessageEventArgs(message));
                }
            });
        }
    }
}

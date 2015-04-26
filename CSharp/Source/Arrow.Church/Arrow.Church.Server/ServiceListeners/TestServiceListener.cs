using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Threading;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Net;

namespace Arrow.Church.Server.ServiceListeners
{
	public class TestServiceListener : ServiceListener
	{
		private static long s_SenderSystemID;
		
		private readonly long m_SenderSystemID;
		private long m_SenderCorrelationID;

		private readonly IWorkDispatcher m_CallDispatcher=new ThreadPoolWorkDispatcher();

		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<long,OutstandingCall> m_OutstandingCalls=new Dictionary<long,OutstandingCall>();

		public TestServiceListener(MessageProtocol messageProtocol) : base(messageProtocol)
		{
			m_SenderSystemID=Interlocked.Increment(ref s_SenderSystemID);
		}

		public Task<object> Call(string serviceName, string serviceMethod, object request)
		{
			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(serviceMethod==null) throw new ArgumentNullException("serviceMethod");

			byte[] data=null;

			using(var stream=new MemoryStream())
			{
				using(var encoder=new DataEncoder(stream))
				{
					var callRequest=new ServiceCallRequest(serviceName,serviceMethod);
					encoder.Write(callRequest);
				}

				this.MessageProtocol.ToStream(stream,request);
				data=stream.ToArray();
			}

			var envelope=new MessageEnvelope();
			envelope.DataLength=data.Length;
			envelope.SenderSystemID=m_SenderSystemID;
			envelope.SenderCorrelationID=Interlocked.Increment(ref m_SenderCorrelationID);

			var call=new OutstandingCall();
			lock(m_SyncRoot)
			{
				m_OutstandingCalls.Add(envelope.SenderCorrelationID,call);
			}

			m_CallDispatcher.QueueUserWorkItem(s=>DispatchCall(envelope,data));

			return call.Source.Task;
		}

		private void DispatchCall(MessageEnvelope envelope, byte[] data)
		{
			var args=new ServiceCallEventArgs(this,envelope,data);
			OnServiceCall(args);
		}

		public override void Respond(MessageEnvelope senderMessageEnvelope, IList<ArraySegment<byte>> buffers)
		{
			var data=buffers.ToArray();
			using(var stream=new MemoryStream(data,false))
			{
				ServiceCallResponse response=null;

				using(var decoder=new DataDecoder(stream))
				{
					response=decoder.ReadEncodedData(d=>new ServiceCallResponse(d));
				}

				object message=this.MessageProtocol.FromStream(stream);

				OutstandingCall outstandingCall=null;

				lock(m_SyncRoot)
				{
					if(m_OutstandingCalls.TryGetValue(senderMessageEnvelope.SenderCorrelationID,out outstandingCall))
					{
						m_OutstandingCalls.Remove(senderMessageEnvelope.SenderCorrelationID);
					}
				}

				// TODO: error handling
				if(response.IsFaulted)
				{
					outstandingCall.Source.SetException((Exception)message);
				}
				else
				{
					outstandingCall.Source.SetResult(message);
				}
			}
		}

		class OutstandingCall
		{
			public readonly TaskCompletionSource<object> Source=new TaskCompletionSource<object>();
		}
	}
}

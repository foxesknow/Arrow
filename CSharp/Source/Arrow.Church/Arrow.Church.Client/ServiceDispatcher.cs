using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Net;
using Arrow.Threading;

namespace Arrow.Church.Client
{
    public abstract partial class ServiceDispatcher : IDisposable
    {
		private static long s_SenderSystemID;
		
		private readonly long m_SenderSystemID;
		private long m_SenderCorrelationID;

		private readonly MessageProtocol m_MessageProtocol;
		private readonly Uri m_Endpoint;

		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<long,IOutstandingCall> m_OutstandingCalls=new Dictionary<long,IOutstandingCall>();

		private readonly IWorkDispatcher m_CompletionDispatcher=new ThreadPoolWorkDispatcher();

		protected ServiceDispatcher(Uri endpoint, MessageProtocol messageProtocol)
		{
			if(messageProtocol==null) throw new ArgumentNullException("messageProtocol");
			
			m_SenderSystemID=Interlocked.Increment(ref s_SenderSystemID);
			m_MessageProtocol=messageProtocol;
			m_Endpoint=endpoint;
		}

		public Uri Endpoint
		{
			get{return m_Endpoint;}
		}

		public long SystemID
		{
			get{return m_SenderSystemID;}
		}

		protected abstract void SendRequest(MessageEnvelope envelope, byte[] data);

		/// <summary>
		/// Signals the appropriate task than it has completed successfully
		/// </summary>
		/// <param name="correlationID"></param>
		/// <param name="result"></param>
		protected void CompleteSuccess(long correlationID, object result)
		{
			m_CompletionDispatcher.QueueUserWorkItem(s=>
			{
				var call=RemoveCall(correlationID);
				if(call!=null) call.Accept(false,result);
			});
		}

		/// <summary>
		/// Signals the appropriate task that something went wrong
		/// </summary>
		/// <param name="correlationID"></param>
		/// <param name="exception"></param>
		protected void CompleteError(long correlationID, Exception exception)
		{
			m_CompletionDispatcher.QueueUserWorkItem(s=>
			{
				var call=RemoveCall(correlationID);
				if(call!=null) call.Accept(true,exception);
			});
		}

		protected void HandleResponse(MessageEnvelope senderMessageEnvelope, IList<ArraySegment<byte>> buffers)
		{
			var data=buffers.ToArray();
			using(var stream=new MemoryStream(data,false))
			{
				ServiceCallResponse response=null;

				using(var decoder=new DataDecoder(stream))
				{
					response=decoder.ReadEncodedData(d=>new ServiceCallResponse(d));
				}

				object message=m_MessageProtocol.FromStream(stream);


				// TODO: error handling
				if(response.IsFaulted)
				{
					CompleteError(senderMessageEnvelope.SenderCorrelationID,(Exception)message);
				}
				else
				{
					CompleteSuccess(senderMessageEnvelope.SenderCorrelationID,message);
				}
			}
		}

		internal Task Call(string serviceName, string serviceMethod, object request)
		{
			MessageEnvelope envelope=null;
			byte[] data=null;

			Encode(serviceName,serviceMethod,request,out envelope,out data);

			var callData=new OutstandingCall();

			lock(m_SyncRoot)
			{
				m_OutstandingCalls.Add(envelope.SenderCorrelationID,callData);				
			}

			SendRequest(envelope,data);
			return callData.GetTask();
		}

		internal Task<T> Call<T>(string serviceName, string serviceMethod, object request)
		{
			MessageEnvelope envelope=null;
			byte[] data=null;

			Encode(serviceName,serviceMethod,request,out envelope,out data);

			var callData=new OutstandingCall<T>();

			lock(m_SyncRoot)
			{
				m_OutstandingCalls.Add(envelope.SenderCorrelationID,callData);				
			}

			SendRequest(envelope,data);
			return callData.GetTask();
		}

		private void Encode(string serviceName, string serviceMethod, object request, out MessageEnvelope envelope, out byte[] data)
		{
			using(var stream=new MemoryStream())
			{
				using(var encoder=new DataEncoder(stream))
				{
					var callRequest=new ServiceCallRequest(serviceName,serviceMethod);
					encoder.Write(callRequest);
				}

				m_MessageProtocol.ToStream(stream,request);
				data=stream.ToArray();
			}

			envelope=new MessageEnvelope();
			envelope.DataLength=data.Length;
			envelope.MessageType=MessageType.Request;
			envelope.SenderSystemID=m_SenderSystemID;
			envelope.SenderCorrelationID=AllocateCorrelationID();
		}

		private IOutstandingCall RemoveCall(MessageEnvelope envelope)
		{
			return RemoveCall(envelope.SenderCorrelationID);
		}

		private IOutstandingCall RemoveCall(long correlationID)
		{
			IOutstandingCall call=null;

			lock(m_SyncRoot)
			{
				if(m_OutstandingCalls.TryGetValue(correlationID,out call))
				{
					m_OutstandingCalls.Remove(correlationID);
				}
			}

			return call;
		}

		protected long AllocateCorrelationID()
		{
			return Interlocked.Increment(ref m_SenderCorrelationID);
		}

		public override string ToString()
		{
			return m_Endpoint.ToString();
		}

		public virtual void Dispose()
		{
			// Does nothing
		}
	}
}

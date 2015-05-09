using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Client.Proxy;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Data.DotNet;
using Arrow.Church.Common.Net;
using Arrow.Logging;
using Arrow.Threading;

namespace Arrow.Church.Client.ServiceDispatchers
{
    public abstract partial class ServiceDispatcher : IDisposable
    {
		private static readonly ILog Log=LogManager.GetDefaultLog();
		private static readonly MessageProtocol s_DotNetSerializer=new SerializationMessageProtocol();

		private static long s_SystemID;
		
		private readonly long m_SystemID;
		private long m_CorrelationID;

		private readonly Uri m_Endpoint;

		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<long,IOutstandingCall> m_OutstandingCalls=new Dictionary<long,IOutstandingCall>();

		private readonly IWorkDispatcher m_CompletionDispatcher=new ThreadPoolWorkDispatcher();

		protected ServiceDispatcher(Uri endpoint)
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");

			m_SystemID=Interlocked.Increment(ref s_SystemID);
			m_Endpoint=endpoint;
		}

		public Uri Endpoint
		{
			get{return m_Endpoint;}
		}

		public long SystemID
		{
			get{return m_SystemID;}
		}

		protected abstract void SendRequest(MessageEnvelope envelope, byte[] data);

		/// <summary>
		/// Signals the appropriate task than it has completed successfully
		/// </summary>
		/// <param name="correlationID"></param>
		/// <param name="result"></param>
		protected void CompleteSuccess(long correlationID, object result)
		{
			var call=RemoveCall(correlationID);

			if(call!=null)
			{
				m_CompletionDispatcher.QueueUserWorkItem(s=>call.Accept(false,result));
			}
		}

		/// <summary>
		/// Signals the appropriate task that something went wrong
		/// </summary>
		/// <param name="correlationID"></param>
		/// <param name="exception"></param>
		protected void CompleteError(long correlationID, Exception exception)
		{
			var call=RemoveCall(correlationID);

			if(call!=null)
			{
				var unpackedException=UnpackException(exception);
				m_CompletionDispatcher.QueueUserWorkItem(s=>call.Accept(true,unpackedException));
			}
		}

		protected void CompleteAllError(Exception exception)
		{
			var unpackedException=UnpackException(exception);

			lock(m_SyncRoot)
			{
				foreach(var call in m_OutstandingCalls.Values)
				{
					var theCall=call;

					if(theCall!=null)
					{
						m_CompletionDispatcher.QueueUserWorkItem(s=>theCall.Accept(true,unpackedException));
					}
				}
				
				m_OutstandingCalls.Clear();
			}
		}

		protected Exception UnpackException(Exception exception)
		{
			var aggregate=exception as AggregateException;
			if(aggregate==null) return exception;

			var flattenedExceptions=aggregate.Flatten();
			var innerExceptions=flattenedExceptions.InnerExceptions;

			return innerExceptions.Count==1 ? innerExceptions[0] : flattenedExceptions;
		}

		protected void HandleResponse(MessageEnvelope responseMessageEnvelope, IList<ArraySegment<byte>> buffers)
		{
			var data=buffers.ToArray();
			using(var stream=new MemoryStream(data,false))
			{
				ServiceCallResponse response=null;

				using(var decoder=new DataDecoder(stream))
				{
					response=decoder.ReadEncodedDataNeverNull(d=>new ServiceCallResponse(d));
				}

				long correlationID=responseMessageEnvelope.ResponseCorrelationID;

				// There's always the chance that we've been returned a type we
				// can't deserialize, such as if the assembly containing the type
				// was deployed to the server but not the client
				try
				{
					if(response.IsFaulted)
					{
						// Exceptions are always serialized using standard .NET serialization
						var message=s_DotNetSerializer.FromStream(stream,typeof(Exception));
						CompleteError(correlationID,(Exception)message);
					}
					else
					{
						var call=GetCall(correlationID);
						var returnType=call.ReturnType;
						object message=null;
					
						if(returnType!=typeof(void)) message=call.MessageProtocol.FromStream(stream,returnType);
						CompleteSuccess(correlationID,message);
					}
				}
				catch(Exception e)
				{
					Log.Error("ServiceDispatcher.HandleResponse - failed to complete response",e);
					CompleteError(correlationID,e);
				}
			}
		}

		internal Task Call(ProxyBase proxy, string serviceName, string serviceMethod, object request)
		{
			MessageEnvelope envelope=null;
			byte[] data=null;

			Encode(proxy,serviceName,serviceMethod,request,out envelope,out data);

			var callData=new OutstandingCall(serviceMethod,proxy);

			lock(m_SyncRoot)
			{
				m_OutstandingCalls.Add(envelope.MessageCorrelationID,callData);				
			}

			SendRequest(envelope,data);
			return callData.GetTask();
		}

		internal Task<T> Call<T>(ProxyBase proxy, string serviceName, string serviceMethod, object request)
		{
			MessageEnvelope envelope=null;
			byte[] data=null;

			Encode(proxy,serviceName,serviceMethod,request,out envelope,out data);

			var callData=new OutstandingCall<T>(serviceMethod,proxy);

			lock(m_SyncRoot)
			{
				m_OutstandingCalls.Add(envelope.MessageCorrelationID,callData);				
			}

			SendRequest(envelope,data);
			return callData.GetTask();
		}

		private void Encode(ProxyBase proxy, string serviceName, string serviceMethod, object request, out MessageEnvelope envelope, out byte[] data)
		{
			using(var stream=new MemoryStream())
			{
				using(var encoder=new DataEncoder(stream))
				{
					var callRequest=new ServiceCallRequest(serviceName,serviceMethod);
					encoder.WriteNeverNull(callRequest);
				}

				proxy.MessageProtocol.ToStream(stream,request);
				data=stream.ToArray();
			}

			envelope=new MessageEnvelope();
			envelope.DataLength=data.Length;
			envelope.MessageType=MessageType.ServiceRequest;
			envelope.MessageSystemID=m_SystemID;
			envelope.MessageCorrelationID=AllocateCorrelationID();
		}

		private IOutstandingCall RemoveCall(MessageEnvelope envelope)
		{
			return RemoveCall(envelope.MessageCorrelationID);
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

		private IOutstandingCall GetCall(long correlationID)
		{
			lock(m_SyncRoot)
			{
				IOutstandingCall call=null;
				m_OutstandingCalls.TryGetValue(correlationID,out call);
				return call;
			}
		}

		protected long AllocateCorrelationID()
		{
			return Interlocked.Increment(ref m_CorrelationID);
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

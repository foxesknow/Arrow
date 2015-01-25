using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Provides access to all the functionality of a messaging system.
	/// Derives classes may add system specific information about the system, if necessary
	/// </summary>
	public abstract class MessagingSystem
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		protected MessagingSystem()
		{
		}
		
		/// <summary>
		/// Indicates if the underlying messaging system allows for
		/// message selection prior to delivering them to the client
		/// </summary>
		public virtual bool SupportsMessageSelection
		{
			get{return false;}
		}
		
		/// <summary>
		/// Indicates if the underlying messaging systems allows for
		/// the sending of raw byte streams
		/// </summary>
		public virtual bool SupportByteStreaming
		{
			get{return false;}
		}
		
		/// <summary>
		/// Creates a MessageSender instance to allow messages to be published.
		/// NOTE: The returned object should only be used on the thread that makes the call
		/// </summary>
		/// <returns>A MessageSender instance</returns>
		public abstract MessageSender CreateSender();
		
		/// <summary>
		/// Creates a MessageReceiver instance to allow messages to be received
		/// NOTE: The returned object should only be used on the thread that makes the call
		/// </summary>
		/// <returns>A MessageReceiver instance</returns>
		public abstract MessageReceiver CreateReceiver();
		
		/// <summary>
		/// Creates a concrete MessagingSystem instance for a specific resource
		/// </summary>
		/// <param name="uri">The messaging resource to access</param>
		/// <returns>A MessagingSystem for the specified resource</returns>
		public static MessagingSystem Create(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			return Create(uri.Scheme);
		}
		
		/// <summary>
		/// Creates a concrete MessagingSystem instance for a specific resource
		/// </summary>
		/// <param name="scheme">The scheme (Uri.Scheme) to access</param>
		/// <returns>A MessagingSystem for the specified resource</returns>
		public static MessagingSystem Create(string scheme)
		{
			if(scheme==null) throw new ArgumentNullException("scheme");			
			
			MessagingSystem system=MessagingSystemFactory.TryCreate(scheme);
			if(system==null) throw new MessagingException("unsupported scheme: "+scheme);
			
			return system;
		}
		
		/// <summary>
		/// Create a MessageSender instance for a uri
		/// </summary>
		/// <param name="uri">The messaging resource to access</param>
		/// <returns>A MessageSender instance</returns>
		public static MessageSender CreateSender(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			return CreateSender(uri.Scheme);
		}
		
		/// <summary>
		/// Create a MessageSender instance for a uri
		/// </summary>
		/// <param name="scheme">The messaging resource to access</param>
		/// <returns>A MessageSender instance</returns>
		public static MessageSender CreateSender(string scheme)
		{
			var system=MessagingSystem.Create(scheme);
			return system.CreateSender();
		}
		
		/// <summary>
		/// Create a MessageReceiver instance for a uri
		/// </summary>
		/// <param name="uri">The messaging resource to access</param>
		/// <returns>A MessageReceiver instance</returns>
		public static MessageReceiver CreateReceiver(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			return CreateReceiver(uri.Scheme);
		}
		
		/// <summary>
		/// Create a MessageReceiver instance for a uri
		/// </summary>
		/// <param name="scheme">The messaging resource to access</param>
		/// <returns>A MessageReceiver instance</returns>
		public static MessageReceiver CreateReceiver(string scheme)
		{
			var system=MessagingSystem.Create(scheme);
			return system.CreateReceiver();
		}
		
		/// <summary>
		/// Checks if the messaging system to handle the uri is available
		/// </summary>
		/// <param name="uri">The uri to check</param>
		/// <returns>true if available, false otherwise</returns>
		public static bool IsPresent(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");			
			return IsPresent(uri.Scheme);
		}
		
		/// <summary>
		/// Checks if the messaging system to handle the uri is available
		/// </summary>
		/// <param name="scheme">The scheme to check</param>
		/// <returns>true if available, false otherwise</returns>
		public static bool IsPresent(string scheme)
		{
			if(scheme==null) throw new ArgumentNullException("scheme");			
			return MessagingSystemFactory.IsPresent(scheme);
		}
	}
}

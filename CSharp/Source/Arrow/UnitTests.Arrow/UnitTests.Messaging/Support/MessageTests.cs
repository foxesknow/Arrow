using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Messaging;
using Arrow.Messaging.Support;
using Arrow.Serialization;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Messaging.Support
{
	public abstract class MessageTests
	{
		protected abstract IMessage CreateMessage();
	
		[Test]
		public void AddProperties()
		{
			IMessage message=CreateMessage();
			message.SetProperty("User","Jack");
			
			Assert.That(message.ContainsProperty("User"),Is.True);
			Assert.That(message.GetProperty("User"),Is.EqualTo("Jack"));
		}
		
		[Test]
		public void ClearProperties()
		{
			IMessage message=CreateMessage();
			message.SetProperty("User","Jack");
			
			Assert.That(message.ContainsProperty("User"),Is.True);
			message.ClearProperties();
			Assert.That(message.ContainsProperty("User"),Is.False);
			Assert.That(message.PropertyNames.Count(),Is.EqualTo(0));
		}
		
		[Test]
		public void PropertyNames()
		{
			IMessage message=CreateMessage();
			message.SetProperty("User","Jack");
			message.SetProperty("Location","Island");
			
			Assert.That(message.PropertyNames.Count(),Is.EqualTo(2));
			Assert.That(message.PropertyNames,Has.Member("User"));
			Assert.That(message.PropertyNames,Has.Member("Location"));
		}
		
		[Test]
		public void MessageType()
		{
			IMessage message=CreateMessage();
			
			message.MessageType="Broadcast";
			Assert.That(message.MessageType,Is.EqualTo("Broadcast"));
			
			message.MessageType="Target";
			Assert.That(message.MessageType,Is.EqualTo("Target"));
		}
		
		[Test]
		public void MessageID()
		{
			IMessage message=CreateMessage();
			
			message.MessageID="100";
			Assert.That(message.MessageID,Is.EqualTo("100"));
			
			message.MessageID="200";
			Assert.That(message.MessageID,Is.EqualTo("200"));
		}
		
		[Test]
		public void CorrelationID()
		{
			IMessage message=CreateMessage();
			
			message.CorrelationID="100";
			Assert.That(message.CorrelationID,Is.EqualTo("100"));
			
			message.CorrelationID="200";
			Assert.That(message.CorrelationID,Is.EqualTo("200"));
		}
		
		[Test]
		public void Serialization()
		{
			IMessage message=CreateMessage();
			message.MessageType="Foo";
			message.MessageID="100";
			message.CorrelationID="999";
			message.SetProperty("User","Jack");
			message.SetProperty("Location","Island");
			
			IMessage copy=Clone(message);
			Assert.That(message.GetType(),Is.EqualTo(copy.GetType()));
			
			Assert.That(message.MessageType,Is.EqualTo(copy.MessageType));
			Assert.That(message.MessageID,Is.EqualTo(copy.MessageID));
			Assert.That(message.CorrelationID,Is.EqualTo(copy.CorrelationID));
			
			Assert.That(message.PropertyNames.Count(),Is.EqualTo(copy.PropertyNames.Count()));
			Assert.That(message.GetProperty("User"),Is.EqualTo(copy.GetProperty("User")));
			Assert.That(message.GetProperty("Location"),Is.EqualTo(copy.GetProperty("Location")));
		}
		
		private IMessage Clone(IMessage message)
		{
			byte[] data=GenericBinaryFormatter.ToArray(message);
			return GenericBinaryFormatter.FromArray<IMessage>(data);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Messaging;
using Arrow.Messaging.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Messaging.Support
{
	[TestFixture]
	public class ByteMessageTests : MessageTests
	{
		[Test]
		public void Construction()
		{
			byte[] buffer=new byte[]{10,20,30};
		
			IByteMessage message=new ByteMessage(buffer);
			Assert.That(message.Data,Is.Not.Null);
			Assert.That(message.Data.Length,Is.EqualTo(3));
			
			Assert.That(message.Data[0],Is.EqualTo(10));
			Assert.That(message.Data[1],Is.EqualTo(20));
			Assert.That(message.Data[2],Is.EqualTo(30));
		}

		protected override IMessage CreateMessage()
		{
			return new ByteMessage(new byte[]{1,2,3});
		}
	}
}

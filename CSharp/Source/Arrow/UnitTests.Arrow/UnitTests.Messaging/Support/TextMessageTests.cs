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
	public class TextMessageTests : MessageTests
	{
		[Test]
		public void Construction()
		{
			ITextMessage message=new TextMessage("Hello, world");
			Assert.That(message.Text,Is.Not.Null);
			Assert.That(message.Text,Is.EqualTo("Hello, world"));
		}

		protected override IMessage CreateMessage()
		{
			return new TextMessage("Lost");
		}
	}
}

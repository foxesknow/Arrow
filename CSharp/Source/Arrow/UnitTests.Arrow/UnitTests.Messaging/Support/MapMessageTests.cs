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
	public class MapMessageTests : MessageTests
	{
		[Test]
		public void TestBool()
		{
			IMapMessage message=new MapMessage();
			
			message.SetBool("foo",true);
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetBool("foo"),Is.EqualTo(true));
		}
		
		[Test]
		public void TestChar()
		{
			IMapMessage message=new MapMessage();
			
			message.SetChar("foo",'X');
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetChar("foo"),Is.EqualTo('X'));
		}
		
		[Test]
		public void TestByte()
		{
			IMapMessage message=new MapMessage();
			
			message.SetByte("foo",23);
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetByte("foo"),Is.EqualTo((byte)23));
		}
		
		[Test]
		public void TestShort()
		{
			IMapMessage message=new MapMessage();
			
			message.SetShort("foo",12345);
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetShort("foo"),Is.EqualTo((short)12345));
		}
		
		[Test]
		public void TestInt()
		{
			IMapMessage message=new MapMessage();
			
			message.SetInt("foo",1000000);
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetInt("foo"),Is.EqualTo((int)1000000));
		}
		
		[Test]
		public void TestLong()
		{
			IMapMessage message=new MapMessage();
			
			message.SetLong("foo",long.MaxValue);
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetLong("foo"),Is.EqualTo(long.MaxValue));
		}
		
		[Test]
		public void TestFloat()
		{
			IMapMessage message=new MapMessage();
			
			message.SetFloat("foo",float.MaxValue);
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetFloat("foo"),Is.EqualTo(float.MaxValue));
		}
		
		[Test]
		public void TestDouble()
		{
			IMapMessage message=new MapMessage();
			
			message.SetDouble("foo",double.MaxValue);
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetDouble("foo"),Is.EqualTo(double.MaxValue));
		}
		
		[Test]
		public void TestString()
		{
			IMapMessage message=new MapMessage();
			
			message.SetString("foo","bar");
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetString("foo"),Is.EqualTo("bar"));
		}
		
		[Test]
		public void TestObject()
		{
			IMapMessage message=new MapMessage();
			
			message.SetObject("foo",double.MaxValue);
			Assert.That(message.Contains("foo"),Is.True);
			Assert.That(message.GetObject("foo"),Is.EqualTo(double.MaxValue));
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestBadObject()
		{
			IMapMessage message=new MapMessage();
			
			// This should fail 
			message.SetObject("foo",DateTime.Now);
		}
		
		[Test]
		public void TestByteArray()
		{
			byte[] baseData=new byte[]{1,2,3,5,7,11};
			
			IMapMessage message=new MapMessage();
			
			message.SetBytes("foo",baseData);
			Assert.That(message.Contains("foo"),Is.True);
			
			var data=message.GetBytes("foo");
			Assert.That(data,Is.Not.Null);
			Assert.That(data.Length,Is.EqualTo(baseData.Length));
			
			// The data we get back should be a copy of the baseline data
			data[0]=99;
			Assert.That(baseData[0],Is.Not.EqualTo(data[0]));
		}
		
		[Test]
		public void TestContains()
		{
			IMapMessage message=new MapMessage();
			
			Assert.That(message.Contains("foo"),Is.False);
			message.SetString("foo","bar");
			Assert.That(message.Contains("foo"),Is.True);
		}
		
		[Test]
		public void TestNames()
		{
			IMapMessage message=new MapMessage();
			
			message.SetString("foo","bar");
			message.SetString("jack","doctor");
			
			var names=message.Names;
			Assert.That(names.Contains("foo"),Is.True);
			Assert.That(names.Contains("jack"),Is.True);
			Assert.That(names.Contains("locke"),Is.False);
		}
		
		[Test]
		public void TestConversions()
		{
			IMapMessage message=new MapMessage();
			
			message.SetString("foo","10");
			Assert.That(message.GetInt("foo"),Is.EqualTo(10));
			Assert.That(message.GetByte("foo"),Is.EqualTo((byte)10));
			
			message.SetString("bar","true");
			Assert.That(message.GetBool("bar"),Is.True);
		}

		protected override IMessage CreateMessage()
		{
			return new MapMessage();
		}
	}
}

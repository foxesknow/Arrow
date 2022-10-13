using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Arrow.Storage;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Storage
{
	[TestFixture]
	public class FileAccessorTests
	{
		[Test]
		public void Load()
		{
			Uri uri=MakeUri("xcopy.exe");
			Accessor access=new FileAccessor(uri);
			
			using(Stream stream=access.OpenRead())
			{
				Assert.IsNotNull(stream);
			}
		}
		
		[Test]
		public void TestNotFound()
		{
            Assert.Throws<FileNotFoundException>(() =>
            {
			    Uri uri=MakeUri("foo.bar");
			    Accessor access=new FileAccessor(uri);
			
			    using(Stream stream=access.OpenRead())
			    {
				    // We should never get here
				    Assert.Fail();
			    }
            });
		}
		
		[Test]
		public void Exists()
		{
			Uri uri=MakeUri("xcopy.exe");
			Accessor access=new FileAccessor(uri);
			Assert.IsTrue(access.CanExists);
			Assert.IsTrue(access.Exists());
		}
		
		[Test]
		public void NotExists()
		{
			Uri uri=MakeUri("foo.bar");
			Accessor access=new FileAccessor(uri);
			Assert.IsTrue(access.CanExists);
			Assert.IsFalse(access.Exists());
		}
	
		private Uri MakeUri(string filename)
		{
			string system32=Environment.SystemDirectory;
			filename=system32+"\\"+filename;
			
			return Accessor.CreateFileUri(filename);
		}
	}
}

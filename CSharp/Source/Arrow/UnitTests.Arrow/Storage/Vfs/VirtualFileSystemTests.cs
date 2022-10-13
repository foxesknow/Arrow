using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Storage.Vfs;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Storage.Vfs
{
	[TestFixture]
	public class VirtualFileSystemTests
	{
		[Test]
		public void EmptyFilespace()
		{
			var space=new VirtualFileSystem();

			Assert.That(space.GetDirectories("/").Count,Is.EqualTo(0));
			Assert.That(space.GetFiles("/").Count,Is.EqualTo(0));
		}

		[Test]
		public void RootExists()
		{
			var space=new VirtualFileSystem();
			Assert.That(space.DirectoryExists("/"),Is.True);
		}

		[Test]
		public void DirectoryExists_NotPresent()
		{
			var space=new VirtualFileSystem();

			Assert.That(space.DirectoryExists("foo"),Is.False);
			Assert.That(space.DirectoryExists("/foo/bar"),Is.False);
		}

		[Test]
		public void DirectoryExists()
		{
			var space=new VirtualFileSystem();
			space.CreateDirectory("foo");
			space.CreateDirectory("/foo/bar/rod/jane/freddy");
			space.CreateDirectory("/foo/bar/rod/jane");

			Assert.That(space.DirectoryExists("foo"),Is.True);
			Assert.That(space.DirectoryExists("/foo/bar"),Is.True);
			Assert.That(space.DirectoryExists("/foo/bar/rod/jane/freddy"),Is.True);
			Assert.That(space.DirectoryExists("/foo/bar/rod/jane"),Is.True);
			Assert.That(space.DirectoryExists("/foo/bar/rod"),Is.True);
		}

		[Test]
		public void FileExists_NotPresent()
		{
			var space=new VirtualFileSystem();

			Assert.That(space.FileExists(VirtualFileSystem.FromUnixPath("foo.txt")),Is.False);
			Assert.That(space.FileExists(VirtualFileSystem.FromUnixPath("/foo/bar.txt")),Is.False);
		}

		[Test]
		public void FileExists()
		{
			var space=new VirtualFileSystem();
			var file=VirtualFileSystem.CreateTextFile("hello, world!");
			space.CreateFile(VirtualFileSystem.FromUnixPath("/foo/bar/hello.txt"),file);

			Assert.That(space.DirectoryExists("foo"),Is.True);
			Assert.That(space.DirectoryExists("/foo/bar"),Is.True);
			Assert.That(space.FileExists("/foo/bar/hello.txt"),Is.True);

		}

		[Test]
		public void OpenFile()
		{
			var space=new VirtualFileSystem();
			space.CreateFile("/foo/bar/hello.txt","hello, world!");

			using(var stream=space.OpenFile("/foo/bar/hello.txt"))
			using(var reader=new StreamReader(stream))
			{
				var text=reader.ReadLine();
				Assert.That(text,Is.EqualTo("hello, world!"));
			}
		}

		[Test]
		public void ReplaceExistingFile()
		{
			var space=new VirtualFileSystem();
			var file=VirtualFileSystem.CreateTextFile("hello, world!");
			space.CreateFile("/foo/bar/hello.txt",file);

			space.CreateFile("/foo/bar/hello.txt","Hi there");

			using(var stream=space.OpenFile("/foo/bar/hello.txt"))
			using(var reader=new StreamReader(stream))
			{
				var text=reader.ReadLine();
				Assert.That(text,Is.EqualTo("Hi there"));
			}
		}

		[Test]
		public void OpenFile_FileNotFound()
		{
			var space=new VirtualFileSystem();
			var file=VirtualFileSystem.CreateTextFile("hello, world!");
			space.CreateFile("/foo/bar/hello.txt",file);

			Assert.Throws<IOException>(()=>
			{
				var stream=space.OpenFile("/foo/bar/world.txt");
			});
		}

		[Test]
		public void OpenFile_DirectoryNotFound()
		{
			var space=new VirtualFileSystem();
			var file=VirtualFileSystem.CreateTextFile("hello, world!");
			space.CreateFile("/foo/bar/hello.txt",file);

			Assert.Throws<IOException>(()=>
			{
				var stream=space.OpenFile("/foo/foobar/hello.txt");
			});
		}

		[Test]
		public void GetDirectories()
		{
			var space=new VirtualFileSystem();
			space.CreateDirectory("/foo/bar");
			space.CreateDirectory("/foo/baz");
			space.CreateDirectory("/foo/hello/world");
			space.CreateDirectory("/good/morning");

			var directories=space.GetDirectories(VirtualFileSystem.Root);
			Assert.That(directories,Is.Not.Null);
			Assert.That(directories.Count,Is.EqualTo(2));
			Assert.That(directories,Has.Member("foo"));
			Assert.That(directories,Has.Member("good"));

			directories=space.GetDirectories("/foo");
			Assert.That(directories,Is.Not.Null);
			Assert.That(directories.Count,Is.EqualTo(3));
			Assert.That(directories,Has.Member("bar"));
			Assert.That(directories,Has.Member("baz"));
			Assert.That(directories,Has.Member("hello"));

			directories=space.GetDirectories("/good/morning");
			Assert.That(directories,Is.Not.Null);
			Assert.That(directories.Count,Is.EqualTo(0));
		}

		[Test]
		public void GetDirectories_DoesNotExist()
		{
			var space=new VirtualFileSystem();
			space.CreateDirectory("/foo/bar");
			space.CreateDirectory("/foo/baz");
			space.CreateDirectory("/foo/hello/world");
			space.CreateDirectory("/good/morning");

			var directories=space.GetDirectories("/a");
			Assert.That(directories,Is.Not.Null);
			Assert.That(directories.Count,Is.EqualTo(0));
			
			directories=space.GetDirectories("/a/b/c");
			Assert.That(directories,Is.Not.Null);
			Assert.That(directories.Count,Is.EqualTo(0));
		}

		[Test]
		public void GetFiles()
		{
			var space=new VirtualFileSystem();
			space.CreateFile("/info","root file");
			space.CreateFile("/foo/bar1","another file-1");
			space.CreateFile("/foo/bar2","another file-2");

			var files=space.GetFiles("/");
			Assert.That(files,Is.Not.Null);
			Assert.That(files.Count,Is.EqualTo(1));
			Assert.That(files,Has.Member("info"));

			files=space.GetFiles("/foo");
			Assert.That(files,Is.Not.Null);
			Assert.That(files.Count,Is.EqualTo(2));
			Assert.That(files,Has.Member("bar1"));
			Assert.That(files,Has.Member("bar2"));
		}

		[Test]
		public void MakePath()
		{
			Assert.That(VirtualFileSystem.MakePath(),Is.Empty);

			var path=VirtualFileSystem.MakePath("path","to","file.txt");
			
			Assert.That(path,Is.Not.Null);
			Assert.That(path.Count,Is.EqualTo(3));
			Assert.That(path[0],Is.EqualTo("path"));
			Assert.That(path[1],Is.EqualTo("to"));
			Assert.That(path[2],Is.EqualTo("file.txt"));
		}

		[Test]
		public void MakePath_BadPath()
		{
			Assert.Throws<ArgumentException>(()=>
			{
				VirtualFileSystem.MakePath("path",null,"file.txt");
			});
		}

		[Test]
		public void FromUnixPath()
		{
			var path=VirtualFileSystem.FromUnixPath("/");
			Assert.That(path,Is.Not.Null);
			Assert.That(path,Is.Empty);

			path=VirtualFileSystem.FromUnixPath("/path/to/file.txt");

			Assert.That(path,Is.Not.Null);
			Assert.That(path.Count,Is.EqualTo(3));
			Assert.That(path[0],Is.EqualTo("path"));
			Assert.That(path[1],Is.EqualTo("to"));
			Assert.That(path[2],Is.EqualTo("file.txt"));
		}

		[Test]
		public void CreateTextFile()
		{
			var file=VirtualFileSystem.CreateTextFile("hello, world");

			using(var stream=file())
			using(var reader=new StreamReader(stream))
			{
				string text=reader.ReadLine();
				Assert.That(text,Is.EqualTo("hello, world"));
			}
		}

		[Test]
		public void CreateBinaryFile()
		{
			byte[] buffer={1,1,2,3,5,8,13};

			var file=VirtualFileSystem.CreateBinaryFile(buffer);
			Assert.That(file,Is.Not.Null);

			var buffer1=new byte[buffer.Length];
			using(var stream=file())
			{
				Assert.That(stream,Is.Not.Null);

				int read=stream.Read(buffer1,0,buffer1.Length);
				Assert.That(read,Is.EqualTo(buffer.Length));

				CompareArrays(buffer,buffer1);
			}

			// If we change the original buffer we'll get back the changes
			buffer[0]=99;

			var buffer2=new byte[buffer.Length];
			using(var stream=file())
			{
				Assert.That(stream,Is.Not.Null);

				int read=stream.Read(buffer2,0,buffer2.Length);
				Assert.That(read,Is.EqualTo(buffer.Length));

				CompareArrays(buffer,buffer2);
			}
		}

		private void CompareArrays(byte[] array1, byte[] array2)
		{
			Assert.That(array1.Length,Is.EqualTo(array2.Length));

			for(int i=0; i<array1.Length; i++)
			{
				Assert.That(array1[i],Is.EqualTo(array2[i]));
			}
		}
	}
}

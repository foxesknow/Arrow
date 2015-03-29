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
	public class FilespaceTests
	{
		[Test]
		public void EmptyFilespace()
		{
			var space=new Filespace();

			Assert.That(space.GetDirectories(Filespace.Root).Count,Is.EqualTo(0));
			Assert.That(space.GetFiles(Filespace.Root).Count,Is.EqualTo(0));
		}

		[Test]
		public void RootExists()
		{
			var space=new Filespace();
			Assert.That(space.DirectoryExists(Filespace.Root),Is.True);
		}

		[Test]
		public void DirectoryExists_NotPresent()
		{
			var space=new Filespace();

			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("foo")),Is.False);
			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("/foo/bar")),Is.False);
		}

		[Test]
		public void DirectoryExists()
		{
			var space=new Filespace();
			space.CreateDirectory(Filespace.FromUnixPath("foo"));
			space.CreateDirectory(Filespace.FromUnixPath("/foo/bar/rod/jane/freddy"));
			space.CreateDirectory(Filespace.FromUnixPath("/foo/bar/rod/jane"));

			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("foo")),Is.True);
			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("/foo/bar")),Is.True);
			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("/foo/bar/rod/jane/freddy")),Is.True);
			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("/foo/bar/rod/jane")),Is.True);
			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("/foo/bar/rod")),Is.True);
		}

		[Test]
		public void FileExists_NotPresent()
		{
			var space=new Filespace();

			Assert.That(space.FileExists(Filespace.FromUnixPath("foo.txt")),Is.False);
			Assert.That(space.FileExists(Filespace.FromUnixPath("/foo/bar.txt")),Is.False);
		}

		[Test]
		public void FileExists()
		{
			var space=new Filespace();
			var file=Filespace.CreateTextFile("hello, world!");
			space.CreateFile(Filespace.FromUnixPath("/foo/bar/hello.txt"),file);

			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("foo")),Is.True);
			Assert.That(space.DirectoryExists(Filespace.FromUnixPath("/foo/bar")),Is.True);
			Assert.That(space.FileExists(Filespace.FromUnixPath("/foo/bar/hello.txt")),Is.True);

		}

		[Test]
		public void OpenFile()
		{
			var space=new Filespace();
			var file=Filespace.CreateTextFile("hello, world!");
			space.CreateFile(Filespace.FromUnixPath("/foo/bar/hello.txt"),file);

			using(var stream=space.OpenFile(Filespace.FromUnixPath("/foo/bar/hello.txt")))
			using(var reader=new StreamReader(stream))
			{
				var text=reader.ReadLine();
				Assert.That(text,Is.EqualTo("hello, world!"));
			}
		}

		[Test]
		public void OpenFile_FileNotFound()
		{
			var space=new Filespace();
			var file=Filespace.CreateTextFile("hello, world!");
			space.CreateFile(Filespace.FromUnixPath("/foo/bar/hello.txt"),file);

			Assert.Throws<IOException>(()=>
			{
				var stream=space.OpenFile(Filespace.FromUnixPath("/foo/bar/world.txt"));
			});
		}

		[Test]
		public void OpenFile_DirectoryNotFound()
		{
			var space=new Filespace();
			var file=Filespace.CreateTextFile("hello, world!");
			space.CreateFile(Filespace.FromUnixPath("/foo/bar/hello.txt"),file);

			Assert.Throws<IOException>(()=>
			{
				var stream=space.OpenFile(Filespace.FromUnixPath("/foo/foobar/hello.txt"));
			});
		}

		[Test]
		public void MakePath()
		{
			Assert.That(Filespace.MakePath(),Is.Empty);

			var path=Filespace.MakePath("path","to","file.txt");
			
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
				Filespace.MakePath("path",null,"file.txt");
			});
		}

		[Test]
		public void FromUnixPath()
		{
			var path=Filespace.FromUnixPath("/");
			Assert.That(path,Is.Not.Null);
			Assert.That(path,Is.Empty);

			path=Filespace.FromUnixPath("/path/to/file.txt");

			Assert.That(path,Is.Not.Null);
			Assert.That(path.Count,Is.EqualTo(3));
			Assert.That(path[0],Is.EqualTo("path"));
			Assert.That(path[1],Is.EqualTo("to"));
			Assert.That(path[2],Is.EqualTo("file.txt"));
		}

		[Test]
		public void CreateTextFile()
		{
			var file=Filespace.CreateTextFile("hello, world");

			using(var stream=file())
			using(var reader=new StreamReader(stream))
			{
				string text=reader.ReadLine();
				Assert.That(text,Is.EqualTo("hello, world"));
			}
		}
	}
}

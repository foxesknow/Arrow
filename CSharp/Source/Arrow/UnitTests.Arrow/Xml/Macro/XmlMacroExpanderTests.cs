using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Arrow.Xml.Macro;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Xml.Macro
{
	[TestFixture]
	public class XmlMacroExpanderTests
	{
		[Test]
		public void TestDefine()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestDefine.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);
			
			Assert.IsNotNull(output.SelectSingleNode("root/Person"));
			Assert.IsTrue(output.SelectNodes("root/Person").Count==1);
			
			Assert.IsNotNull(output.SelectSingleNode("root/Superuser"));
			Assert.IsNotNull(output.SelectSingleNode("root/Superuser/Person"));
			Assert.IsTrue(output.SelectSingleNode("root/Superuser/Person/Name").InnerText=="Bob");
			Assert.IsTrue(output.SelectSingleNode("root/Superuser/Person/Age").InnerText=="58");
			
			Assert.IsTrue(output.SelectSingleNode("root/Insert").ChildNodes.Count==2);
			Assert.IsNotNull(output.SelectSingleNode("root/Insert/Foo"));
			Assert.IsNotNull(output.SelectSingleNode("root/Insert/Bar"));
			
			Assert.IsNotNull(output.SelectSingleNode("root/Node"));
			Assert.IsNotNull(output.SelectSingleNode("root/Node/User"));
			Assert.IsTrue(output.SelectSingleNode("root/Node/User").InnerText=="John");
		}
		
		[Test]
		public void TestInsertSetting()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestInsertSetting.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);
			
			Assert.IsNotNull(output.SelectSingleNode("root/UserDetails"));
			Assert.IsTrue(output.SelectSingleNode("root/UserDetails/Username").InnerText!="");
			Assert.IsTrue(output.SelectSingleNode("root/UserDetails/Dept").InnerText!="");
			Assert.IsTrue(output.SelectSingleNode("root/UserDetails/OS").InnerText!="");
		}
		
		[Test]
		public void TestVariables()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestVariables.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);		
			
			Assert.IsTrue(output.SelectSingleNode("root/File/@Mode").Value=="read");
			Assert.IsTrue(output.SelectSingleNode("root/File/Filename").InnerText!="");
			
			Assert.IsTrue(output.SelectSingleNode("root/Mode1").InnerText=="read");
			Assert.IsTrue(output.SelectSingleNode("root/Block/Mode").InnerText=="execute");
			Assert.IsTrue(output.SelectSingleNode("root/Mode2").InnerText=="read");
			Assert.IsTrue(output.SelectSingleNode("root/Share").InnerText=="Write");
			Assert.That(output.SelectSingleNode("root/NoSub").InnerText,Is.EqualTo(System.IO.FileShare.Write.ToString()));
			
			string when=DateTime.Parse("25/12/2008").ToString("ddMMMyyyy");
			Assert.IsTrue(output.SelectSingleNode("root/When").InnerText==when);
		}
		
		[Test]
		public void TestConditionalDeclare()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestTryDeclare.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			expander.AddVariable("app","nunit");
			
			XmlDocument output=expander.Expand(source);
			
			Assert.IsTrue(output.SelectSingleNode("root/username").InnerText=="Jack");
			
			// The xml says "Firefox" but as we've added a variable it will be the value used
			Assert.IsTrue(output.SelectSingleNode("root/app").InnerText=="nunit");
		}
		
		[Test]
		public void TestRequire()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestRequire.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);
			
			Assert.IsTrue(output.SelectSingleNode("root/User/Name").InnerText=="Sawyer");
		}
		
		[Test]
		public void TestRequireFail()
		{
            Assert.Throws<XmlMacroExpanderException>(() =>
            {
			    XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestRequireFail.xml");
			    XmlMacroExpander expander=new XmlMacroExpander();
			    XmlDocument output=expander.Expand(source);
			
			    Assert.IsTrue(output.SelectSingleNode("root/User/Name").InnerText=="Sawyer");
            });
		}
		
		[Test]
		public void TestForEach()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestForEach.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);		
			
			Assert.IsTrue(output.SelectNodes("root/Numbers/Number").Count==4);
			
			Assert.IsTrue(output.SelectSingleNode("root/Pairs/Pair[@key='a']").Attributes["value"].Value=="1");
			Assert.IsTrue(output.SelectSingleNode("root/Pairs/Pair[@key='b']").Attributes["value"].Value=="2");
			Assert.IsTrue(output.SelectSingleNode("root/Pairs/Pair[@key='c']").Attributes["value"].Value=="4");
			Assert.IsTrue(output.SelectSingleNode("root/Pairs/Pair[@key='d']").Attributes["value"].Value=="8");
		}
		
		[Test]
		public void TestInclude()
		{
			Uri uri=ResourceLoader.MakeUri("Macro/XmlMacro_TestInclude.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			
			XmlDocument output=expander.Expand(uri);
			
			Assert.IsTrue(output.SelectNodes("root/Level1").Count==1);
			Assert.IsTrue(output.SelectNodes("root/Level1/Level2").Count==1);
		}
				
		[Test]
		public void TestComment()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestComment.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);	
			
			Assert.IsNotNull(output.SelectSingleNode("root/details/Name"));
			Assert.IsNull(output.SelectSingleNode("root/details/username"));
			Assert.IsNull(output.SelectSingleNode("root/details/app"));
		}
		
		[Test]
		public void TestNamespaceCopy()
		{
			XmlDocument source=ResourceLoader.LoadXml("People.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);
			
			XmlNode node=output.SelectSingleNode("People/Leader");
			Assert.IsNotNull(node);
			
			// There should be an url attribute in the factory namespace
			string @namespace=global::Arrow.Xml.ObjectCreation.CustomXmlCreation.FactoryNS;
			XmlAttribute attr=node.Attributes["uri",@namespace];
			Assert.IsNotNull(attr);
		}
		
		[Test]
		public void TestOptional()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestOptional.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);	
			
			Assert.IsTrue(output.SelectSingleNode("root/Supervisor").InnerText=="Kate");
			Assert.IsTrue(output.SelectSingleNode("root/Admin").InnerText=="");
		}
				
		[Test]
		public void TestInjectXmlWithExpand()
		{
			// Here's theres a macro expansion within the arguments which needs to be done first
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_ExpandInArg.xml");
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);	
			
			Assert.IsNotNull(output.SelectSingleNode("root/Leader/Person/Name"));
		}
		
		[Test]
		public void TestMetaData()
		{
			// Here's theres a macro expansion within the arguments which needs to be done first
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_MetaData.xml");
			
			XmlNamespaceManager nsm=new XmlNamespaceManager(new NameTable());
			nsm.AddNamespace("m",XmlMacroExpander.NS);
			
			Assert.IsNotNull(source.SelectSingleNode("/root/m:Meta[@name='island']",nsm));
			
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);	
			
			Assert.IsNull(output.SelectSingleNode("/root/m:Meta[@name='island']",nsm));
			Assert.IsNotNull(output.SelectSingleNode("/root/m:Meta[@name='castle']",nsm));
		}
		
		[Test]
		public void TestXmlComments()
		{
			// Here's theres a macro expansion within the arguments which needs to be done first
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestXmlComments.xml");
			
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);	
		}
		
		[Test]
		public void TestLoadXml()
		{
			XmlDocument source=ResourceLoader.LoadXml("Macro.XmlMacro_TestLoadXml.xml");
			
			XmlMacroExpander expander=new XmlMacroExpander();
			XmlDocument output=expander.Expand(source);	
			
			Assert.IsNotNull(output.SelectSingleNode("root/details/People"));
			Assert.IsNotNull(output.SelectSingleNode("root/details/People/Hunter"));
			Assert.IsNotNull(output.SelectSingleNode("root/details/People/Fisherman"));
			Assert.IsNotNull(output.SelectSingleNode("root/John/Hunter"));
			Assert.IsNotNull(output.SelectSingleNode("root/MoreJohn/Hunter"));
			
			Assert.IsNull(output.SelectSingleNode("root/MoreJohn/Fisherman"));
		}
	}
}

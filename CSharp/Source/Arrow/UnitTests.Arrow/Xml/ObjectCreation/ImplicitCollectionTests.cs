using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class ImplicitCollectionTests
    {
        [Test]
        public void WeakTypes()
        {
            var doc = ResourceLoader.LoadXml("ImplicitCollections.xml");
            var weakNode = doc.SelectSingleNode("root/Weak");

            ImplicitWeakData weak = XmlCreation.Create<ImplicitWeakData>(weakNode);
            Assert.That(weak.Name, Is.EqualTo("Charles"));
            Assert.That(weak.Value, Is.Not.Null);
            Assert.That(weak.Value, Is.TypeOf(typeof(Hashtable)));

            Hashtable table = (Hashtable)weak.Value;
            Assert.That(table, Has.Count.EqualTo(2));
            Assert.That(table.ContainsKey("Numbers"), Is.True);
            Assert.That(table.ContainsKey("Business"), Is.True);

            ArrayList list = (ArrayList)table["Numbers"];
            Assert.That(list, Is.Not.Null);
            Assert.That(list, Has.Count.EqualTo(6));

            // Make sure all the numbers are there
            Assert.That(list, Has.Member(4));
            Assert.That(list, Has.Member(8));
            Assert.That(list, Has.Member(15));
            Assert.That(list, Has.Member(16));
            Assert.That(list, Has.Member(23));
            Assert.That(list, Has.Member(42));
            Assert.That(list.Capacity, Is.EqualTo(20));

            list = (ArrayList)table["Business"];
            Assert.That(list, Is.Not.Null);
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list, Has.Member("Construction"));
        }

        [Test]
        public void StrongTypes()
        {
            var doc = ResourceLoader.LoadXml("ImplicitCollections.xml");
            var strongNode = doc.SelectSingleNode("root/Strong");

            ImplicitStrongData strong = XmlCreation.Create<ImplicitStrongData>(strongNode);
            Assert.That(strong, Is.Not.Null);

            Assert.That(strong.Employers, Has.Count.EqualTo(2));
            Assert.That(strong.Employers, Has.Member("Dharma"));
            Assert.That(strong.Employers, Has.Member("Others"));
            Assert.That(strong.Employers.Capacity, Is.EqualTo(20));

            Assert.That(strong.EmployeeAges, Has.Count.EqualTo(2));
            Assert.That(strong.EmployeeAges["Juliet"], Is.EqualTo(39));
            Assert.That(strong.EmployeeAges["Tom"], Is.EqualTo(52));
        }
    }

    class ImplicitWeakData
    {
        private string m_Name;
        private object m_Value;

        public string Name
        {
            get{return m_Name;}
            set{m_Name = value;}
        }

        public object Value
        {
            get{return m_Value;}
            set{m_Value = value;}
        }
    }

    class ImplicitStrongData
    {
        private string m_Name;
        private List<string> m_Employers = new List<string>();
        private Dictionary<string, int> m_EmployeeAges = new Dictionary<string, int>();

        public string Name
        {
            get{return m_Name;}
            set{m_Name = value;}
        }

        public List<string> Employers
        {
            get{return m_Employers;}
        }

        public Dictionary<string, int> EmployeeAges
        {
            get{return m_EmployeeAges;}
        }
    }
}

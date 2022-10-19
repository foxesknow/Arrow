using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

#nullable enable

namespace Arrow.Xml.ObjectCreation
{
    public sealed class InstanceFactory : IInstanceFactory
    {
        private Func<string, object?>? m_UnknownVariableLookup;
        private Func<Type, object?[]?, object>? m_InstanceCreator;

        private InstanceFactory()
        {

        }

        public static IInstanceFactory New()
        {
            return new InstanceFactory();
        }

        public T Apply<T>(XmlNode node, T existingInstance) where T : class
        {
            if(node is null) throw new ArgumentNullException(nameof(node));
            if(existingInstance is null) throw new ArgumentNullException(nameof(existingInstance));

            var c = MakeCustomXmlCreation();
            c.Apply(existingInstance, node);

            return existingInstance;
        }

        public T ApplyNodeAttributes<T>(XmlNode node, T existingInstance) where T : class
        {
            if(node is null) throw new ArgumentNullException(nameof(node));
            if(existingInstance is null) throw new ArgumentNullException(nameof(existingInstance));

            var c = MakeCustomXmlCreation();
            c.ApplyNodeAttributes(existingInstance, node);

            return existingInstance;
        }

        public T Create<T>(XmlNode node) where T : class
        {
            if(node is null) throw new ArgumentNullException(nameof(node));

            var c = MakeCustomXmlCreation();
            var obj = c.Create<T>(typeof(T), node, null);

            if(obj is null) throw new XmlCreationException($"could not create instance of type {typeof(T).Name}");
            return obj;
        }

        public object Create(XmlNode node, Type type)
        {
            if(node is null) throw new ArgumentNullException(nameof(node));
            if(type is null) throw new ArgumentNullException(nameof(type));

            var c = MakeCustomXmlCreation();
            var obj = c.Create(type, node, null);

            if(obj is null) throw new XmlCreationException($"could not create instance of type {type.Name}");
            if(type.IsAssignableFrom(obj.GetType()) == false) throw new XmlCreationException($"created instance is not of type {type.Name}");
            
            return obj;
        }

        public List<T> CreateList<T>(XmlNodeList nodes) where T : class
        {
            if(nodes is null) throw new ArgumentNullException(nameof(nodes));

            var c = MakeCustomXmlCreation();
            return c.CreateList<T>(nodes);
        }

        public T InitializeInstance<T>(XmlNode node, T existingInstance) where T : class
        {
            if(node is null) throw new ArgumentNullException(nameof(node));
            if(existingInstance is null) throw new ArgumentNullException(nameof(existingInstance));

            var c = MakeCustomXmlCreation();
            c.InitializeInstance(existingInstance, node);

            return existingInstance;
        }

        public IInstanceFactory InstanceCreator(Func<Type, object?[]?, object>? instanceCreator)
        {
            m_InstanceCreator = instanceCreator;
            return this;
        }

        public IInstanceFactory UnknownVariableLookup(Func<string, object?>? unknownVariableLookup)
        {
            m_UnknownVariableLookup = unknownVariableLookup;
            return this;
        }

        public T TrySupportInitialize<T>(T instance) where T : class
        {
            if(instance is ISupportInitialize initializer)
            {
                initializer.BeginInit();
                initializer.EndInit();
            }

            return instance;
        }

        private CustomXmlCreation MakeCustomXmlCreation()
        {
            var creation = new CustomXmlCreation(m_UnknownVariableLookup);
            if(m_InstanceCreator is not null)
            {
                creation.MakeInstance = m_InstanceCreator;
            }

            return creation;
        }
    }
}

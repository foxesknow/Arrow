using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

#nullable enable

namespace Arrow.Xml.ObjectCreation
{
    public interface IInstanceFactory
    {
        public T Create<T>(XmlNode node) where T : class;

        public object Create(XmlNode node, Type type);

        public List<T> CreateList<T>(XmlNodeList nodes) where T : class;
        
        public T Apply<T>(XmlNode node, T existingInstance) where T : class;

        public T TrySupportInitialize<T>(T instance) where T : class;
        
        public T InitializeInstance<T>(XmlNode node, T existingInstance) where T : class;
        
        public T ApplyNodeAttributes<T>(XmlNode node, T existingInstance) where T : class;

        public IInstanceFactory InstanceCreator(Func<Type, object?[]?, object>? instanceCreator);
        
        public IInstanceFactory UnknownVariableLookup(Func<string, object?>? unknownVariableLookup);
    }
}

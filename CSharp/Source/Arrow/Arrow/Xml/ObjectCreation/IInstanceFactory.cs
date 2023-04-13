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
        /// <summary>
        /// Creates on object from an xml description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public T Create<T>(XmlNode node) where T : class;

        /// <summary>
        /// Creates on object from an xml description
        /// </summary>
        /// <param name="node"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Create(XmlNode node, Type type);

        /// <summary>
        /// Creates a list of objects from an xml description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public List<T> CreateList<T>(XmlNodeList nodes) where T : class;
        
        /// <summary>
        /// Applys the xml description to an existing object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="existingInstance"></param>
        /// <returns></returns>
        public T Apply<T>(XmlNode node, T existingInstance) where T : class;

        /// <summary>
        /// Calls the ISupportInitialize methods on an instance, if the type supports the interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns>The instance passed into the function</returns>
        public T TrySupportInitialize<T>(T instance) where T : class;
        
        /// <summary>
        /// Applies the initialization cycle (ISupportInitialize, ICustomXmlInitialization, properties, methods)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="existingInstance"></param>
        /// <returns></returns>
        public T InitializeInstance<T>(XmlNode node, T existingInstance) where T : class;
        
        /// <summary>
        /// Applies the attributes on an xml node to an existing instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="existingInstance"></param>
        /// <returns>The instance passed into the function</returns>
        public T ApplyNodeAttributes<T>(XmlNode node, T existingInstance) where T : class;

        /// <summary>
        /// Sets the method that is called to create instance of type.
        /// If not set then Activatior.CreateInstance is used
        /// </summary>
        /// <param name="instanceCreator"></param>
        /// <returns>The current instance factory</returns>
        public IInstanceFactory InstanceCreator(Func<Type, object?[]?, object>? instanceCreator);
        
        /// <summary>
        /// Sets the optional function that will be called to look up any unknown variables
        /// </summary>
        /// <param name="unknownVariableLookup"></param>
        /// <returns>The current instance factory</returns>
        public IInstanceFactory UnknownVariableLookup(Func<string, object?>? unknownVariableLookup);
    }
}

using System;
using System.Collections.Generic;
using System.Xml;

namespace Arrow.Xml.ObjectCreation
{
    /// <summary>
    /// Defines the behaviour of creation via xml
    /// </summary>
    public interface ICustomXmlCreation
    {
        /// <summary>
        /// Applies an xml node to an existing object
        /// </summary>
        /// <param name="object"></param>
        /// <param name="objectNode"></param>
        void Apply(object @object, XmlNode objectNode);

        /// <summary>
        /// Applies the attributes of a node to an existing object
        /// </summary>
        /// <param name="object"></param>
        /// <param name="node"></param>
        void ApplyNodeAttributes(object @object, XmlNode node);
        
        /// <summary>
        /// Applies the sequence of nodes to an existing object
        /// </summary>
        /// <param name="object"></param>
        /// <param name="nodes"></param>
        void ApplyNodes(object @object, IEnumerable<XmlNode> nodes);

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        object Create(Type type, XmlNode node);

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        /// <param name="baseLocation"></param>
        /// <returns></returns>
        object Create(Type type, XmlNode node, Uri? baseLocation);

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="node"></param>
        /// <param name="baseLocation"></param>
        /// <returns></returns>
        T Create<T>(Type type, XmlNode node, Uri? baseLocation);

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        T Create<T>(XmlNode node);

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="baseLocation"></param>
        /// <returns></returns>
        T Create<T>(XmlNode node, Uri baseLocation);

        /// <summary>
        /// Creates a list from a sequence of nodes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        List<T> CreateList<T>(IEnumerable<XmlNode> nodes);

        /// <summary>
        /// Creates a list from a sequence of nodes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <param name="baseLocation"></param>
        /// <returns></returns>
        List<T> CreateList<T>(IEnumerable<XmlNode> nodes, Uri? baseLocation);

        /// <summary>
        /// Creates a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        List<T> CreateList<T>(XmlNodeList nodes);

        /// <summary>
        /// Creates a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <param name="baseLocation"></param>
        /// <returns></returns>
        List<T> CreateList<T>(XmlNodeList nodes, Uri? baseLocation);

        /// <summary>
        /// Creates a type instace
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Type CreateType(XmlNode node);


        DelayedCreator DelayedCreate<T>(XmlNode node);
        DelayedCreator DelayedCreate<T>(XmlNode node, Uri? baseLocation);
        
        /// <summary>
        /// Initializes an object via ISupportInitialize
        /// </summary>
        /// <param name="object"></param>
        /// <param name="objectNode"></param>
        void InitializeInstance(object @object, XmlNode objectNode);

        /// <summary>
        /// Populates a dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="nodes"></param>
        void PopulateDictionary<T>(IDictionary<string, T> dictionary, XmlNodeList nodes);

        /// <summary>
        /// Populates a dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="nodes"></param>
        /// <param name="baseLocation"></param>
        void PopulateDictionary<T>(IDictionary<string, T> dictionary, XmlNodeList nodes, Uri? baseLocation);

        void PopulateKeyValuePair<K, V>(IDictionary<K, V> dictionary, XmlNodeList nodes) where K : notnull;
    }
}
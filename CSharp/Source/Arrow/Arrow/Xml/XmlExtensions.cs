﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;

namespace Arrow.Xml
{
    /// <summary>
    /// Useful Xml extension methods
    /// </summary>
    public static class XmlExtensions
    {
        private static readonly XmlNodeList s_EmptyXmlNodeList = new EmptyXmlNodeList();

        /// <summary>
        /// Looks up an attribute, returning the value it holds, or defaultValue if not found
        /// </summary>
        /// <param name="attributes">The attribute collection</param>
        /// <param name="name">The qualified name of the attribute</param>
        /// <param name="defaultValue">The value to return if the attribute does not exist</param>
        /// <returns>A value</returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static string? GetValueOrDefault(this XmlAttributeCollection attributes, string name, string? defaultValue)
        {
            var attr = attributes[name];
            return attr != null ? attr.Value : defaultValue;
        }

        /// <summary>
        /// Looks up an attribute, returning the value it holds, or defaultValue if not found
        /// </summary>
        /// <param name="attributes">The attribute collection</param>
        /// <param name="localName">The local name of the attribute</param>
        /// <param name="namespaceURI">The namespace URI of the attribute</param>
        /// <param name="defaultValue">The value to return if the attribute does not exist</param>
        /// <returns>A value</returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static string? GetValueOrDefault(this XmlAttributeCollection attributes, string localName, string namespaceURI, string? defaultValue)
        {
            var attr = attributes[localName, namespaceURI];
            return attr != null ? attr.Value : defaultValue;
        }

        /// <summary>
        /// Converts an XmlAttributeCollection to a typed sequence
        /// </summary>
        /// <param name="attributes">The attributes to convert</param>
        /// <returns>A typed sequence</returns>
        public static IEnumerable<XmlAttribute> AsEnumerable(this XmlAttributeCollection attributes)
        {
            return attributes.Cast<XmlAttribute>();
        }

        /// <summary>
        /// Converts an XmlNodeList to a typed sequence
        /// </summary>
        /// <param name="nodes">The nodes to convert</param>
        /// <returns>A typed sequence</returns>
        public static IEnumerable<XmlNode> AsEnumerable(this XmlNodeList nodes)
        {
            return nodes.Cast<XmlNode>();
        }

        /// <summary>
        /// Returns a list of xml nodes that match the given path
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static XmlNodeList SelectNodesOrEmpty(this XmlNode node, string xpath)
        {
            return node.SelectNodes(xpath) ?? s_EmptyXmlNodeList;
        }

        /// <summary>
        /// Given an xml node list only yields the items that are not null
        /// </summary>
        /// <param name="xmlNodeList"></param>
        /// <returns></returns>
        public static IEnumerable<XmlNode> NonNullNodes(this XmlNodeList xmlNodeList)
        {
            if(xmlNodeList.Count != 0)
            {
                foreach(XmlNode? node in xmlNodeList)
                {
                    if(node is not null) yield return node;
                }
            }
        }

        /// <summary>
        /// Returns all the attributes on the given node, if present
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<XmlAttribute> AllAttributes(this XmlNode node)
        {
            var attributeCollection = node.Attributes;

            if(attributeCollection is not null && attributeCollection.Count != 0)
            {
                foreach(XmlAttribute? attribute in attributeCollection)
                {
                    if(attribute is not null) yield return attribute;
                }
            }
        }

        /// <summary>
        /// Returns a list of xml nodes that match the given path
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="nsmgr"></param>
        /// <returns></returns>
        public static IEnumerable<XmlNode> SelectNodesOrEmpty(this XmlNode node, string xpath, XmlNamespaceManager nsmgr)
        {
            var nodes = node.SelectNodes(xpath, nsmgr);

            if(nodes is not null)
            {
                foreach(XmlNode? n in nodes)
                {
                    if(n is not null) yield return n;
                }
            }
        }

        private sealed class EmptyXmlNodeList : XmlNodeList
        {
            public override XmlNode Item(int index)
            {
                throw new NotImplementedException();
            }

            public override IEnumerator GetEnumerator()
            {
                return Array.Empty<object>().GetEnumerator();
            }

            public override int Count
            {
                get{return 0;}
            }
        }
    }
}

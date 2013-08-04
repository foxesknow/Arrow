using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using Arrow.Settings;
using Arrow.Text;
using Arrow.Storage;
using Arrow.Reflection;

namespace Arrow.Xml.Macro
{
	/// <summary>
	/// Implements a simple macro expansion language for use within xml.
	/// All user macros are placed in the "urn:arrow.xml.macro.def" namespace
	/// </summary>
	/// <remarks>
	/// Macro variables begin with an @ symbol and are follwed by brackets 
	/// If the brackets are braces <b>{ }</b> then the variable in mandatory
	/// and expansion will fail if the variable does not exist.
	/// If the brackets are square brackets <b>[ ]</b> then the variable is
	/// optional and will be replaced with a empty string if the variable
	/// does not exist.
	/// </remarks>
	public class XmlMacroExpander
	{
		/// <summary>
		/// The namespace for the macro tags
		/// </summary>
		public static readonly string NS="urn:arrow.xml.macro";
		
		/// <summary>
		/// The namespace for macros defined by the user
		/// </summary>
		public static readonly string ExpandNS="urn:arrow.xml.macro.def";
		
		
		/// <summary>
		/// The name of the variable that always holds the current file being processed
		/// </summary>
		private static readonly string CurrentFile="cfile";
		
		/// <summary>
		/// The macro being expanded
		/// </summary>
		private static readonly string ExpandingNode="expandingNode";
		
		enum ScopeRule
		{
			CreateNew,
			UseExisting
		}
		
		private delegate void MacroCommand(XmlNode source, XmlNode destination, NameScope variables, Scope scope);
		
		/// <summary>
		/// The commands available
		/// </summary>
		private Dictionary<string,MacroCommand> m_Commands=new Dictionary<string,MacroCommand>();
		
		/// <summary>
		/// An xml namespace manager to use for xpath queries
		/// </summary>
		XmlNamespaceManager m_NamespaceManager;
		
		/// <summary>
		/// The maximum include depth. This can be overridden by a pragma
		/// </summary>
		private int m_MaximumIncludeDepth=20;
		
		private NameScope m_InitialScope=NameScope.CreateScriptNameScope();
		
		private bool m_Strict;
		
		private Dictionary<string,object> m_AdditionalVariables=new Dictionary<string,object>();
		
		/// <summary>
		/// Initializes an instance
		/// </summary>
		public XmlMacroExpander()
		{
			Initialize();
		}
		
		
		/// <summary>
		/// Enables stricter error reporting
		/// </summary>
		public bool Strict
		{
			get{return m_Strict;}
			set{m_Strict=value;}
		}
		
		/// <summary>
		/// Adds a variable to the expander.
		/// This is useful if the host app want to supplied app specific values
		/// </summary>
		/// <param name="name">The name of the variable</param>
		/// <param name="value">The value for the variable</param>
		/// <exception cref="System.ArgumentNullException">name is null</exception>
		public void AddVariable(string name, object value)
		{
			if(name==null) throw new ArgumentNullException("name");
			
			m_AdditionalVariables.Add(name,value);
		}
		
		/// <summary>
		/// Adds a group of variables
		/// </summary>
		/// <param name="variables">The variables to add</param>
		/// <exception cref="System.ArgumentNullException">variables is null</exception>
		public void AddVariables(IDictionary<string,object> variables)
		{
			if(variables==null) throw new ArgumentNullException("variables");
			
			foreach(KeyValuePair<string,object> pair in variables)
			{
				m_AdditionalVariables[pair.Key]=pair.Value;
			}
		}
		
		/// <summary>
		/// Expands source. The base directory is set to the current directory
		/// </summary>
		/// <param name="source">The document to expand</param>
		/// <returns>A new document holding the expanded version of source</returns>
		/// <exception cref="System.ArgumentNullException">source is null</exception>
		public XmlDocument Expand(XmlDocument source)
		{
			return Expand(source,null);
		}
		
		/// <summary>
		/// Loads a document from a uri and expands it
		/// </summary>
		/// <param name="uri">The uri to load the document from</param>
		/// <returns>The expanded document</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public XmlDocument Expand(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			
			XmlDocument doc=StorageManager.Get(uri).ReadXmlDocument();
			return Expand(doc,uri);
		}
		
		/// <summary>
		/// Expands source.
		/// </summary>
		/// <param name="source">The document to expand</param>
		/// <param name="sourceUri">The uri for the document being expanded</param>
		/// <returns>A new document holding the expanded version of source</returns>
		/// <exception cref="System.ArgumentNullException">source is null</exception>
		public XmlDocument Expand(XmlDocument source, Uri sourceUri)
		{
			if(source==null) throw new ArgumentNullException("source");
			
			// Create the target document
			XmlDocument destination=new XmlDocument();
			XmlElement destinationRoot=destination.CreateElement(source.DocumentElement.Name,source.DocumentElement.NamespaceURI);
			destination.AppendChild(destinationRoot);
			
			NameScope variables=m_InitialScope;
			if(variables==null) variables=NameScope.CreateScriptNameScope();
			
			variables.Declare(CurrentFile,sourceUri);
			
			// Add the user supplied variables
			foreach(KeyValuePair<string,object> pair in m_AdditionalVariables)
			{
				variables.Declare(pair.Key,pair.Value);
			}
			
			CopyAttributes(source.DocumentElement,destinationRoot,variables);
			DoCompile(source.DocumentElement,destinationRoot,variables,null);
			
			return destination;
		}
		
		/// <summary>
		/// Performs common initialization for the class
		/// </summary>
		private void Initialize()
		{
			m_NamespaceManager=CreateNamespaceManager();
		
			m_Commands["Define"]=Define;
			m_Commands["Declare"]=Declare;
			m_Commands["TryDeclare"]=TryDeclare;
			m_Commands["Assign"]=Assign;
			m_Commands["ForEach"]=ForEach;
			m_Commands["Pragma"]=Pragma;
			m_Commands["Pass"]=Pass;
			m_Commands["Include"]=Include;
			m_Commands["Comment"]=Comment;
			m_Commands["InjectXml"]=InjectXml;
			m_Commands["Require"]=Require;
			m_Commands["Meta"]=Meta;
			m_Commands["LoadXml"]=LoadXml;
		}
		
		/// <summary>
		/// Compiles all child elements of source into destination, creating a new scope
		/// </summary>
		/// <param name="source">The source xml</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void DoCompile(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			DoCompile(source,destination,variables,scope,ScopeRule.CreateNew);
		}
		
		/// <summary>
		/// Compiles all child elements of source into destination
		/// </summary>
		/// <param name="source">The source xml</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		/// <param name="scopeRule">The scoping rules</param>
		private void DoCompile(XmlNode source, XmlNode destination, NameScope variables, Scope scope, ScopeRule scopeRule)
		{
			if(scopeRule==ScopeRule.CreateNew)
			{
				scope=new Scope(scope);
			}
		
			foreach(XmlNode node in source.ChildNodes)
			{
				if(node.NodeType==XmlNodeType.Element)
				{
					XmlElement sourceElement=(XmlElement)node;
					
					if(sourceElement.NamespaceURI==NS)
					{
						string name=sourceElement.LocalName;
						
						MacroCommand command=null;
						if(m_Commands.TryGetValue(name,out command))
						{
							command(node,destination,variables,scope);
						}
						else if(m_Strict)
						{
							throw new XmlMacroExpanderException("xml command not found: "+name);
						}
					}
					else
					{
						// If it's not a macro command then it's either an expansion or a regular element
						XmlNode macroNode=null;
						string name=sourceElement.Name;
						
						if(sourceElement.NamespaceURI==ExpandNS)
						{
							macroNode=scope.GetMacro(sourceElement.LocalName);
							if(macroNode==null && m_Strict)
							{
								throw new XmlMacroExpanderException("macro not defined: "+sourceElement.Name);
							}
						}
						
						if(macroNode!=null)
						{
							ExpandMacro(null,macroNode,node,destination,variables,scope);
						}
						else
						{						
							XmlNode root=destination.OwnerDocument.CreateElement(name,sourceElement.NamespaceURI);
							CopyAttributes(sourceElement,root,variables);
							DoCompile(node,root,variables.CreateChildScope(),scope);
							destination.AppendChild(root);
						}
					}
				}
				else
				{
					if(node is XmlCharacterData)
					{
						ProcessXmlCharacterData((XmlCharacterData)node,destination,variables);
					}
					else
					{
						// For any other node just import them
						XmlNode destinationNode=destination.OwnerDocument.ImportNode(node.Clone(),true);
						destination.AppendChild(destinationNode);
					}
				}
			}
		}
		
		private void AddCharacterData(XmlCharacterData charData, XmlNode destination, NameScope variables)
		{
			string data=ApplySubstitutions(charData.Value,variables);
						
			XmlNode destinationNode=charData.CloneNode(true);
			destinationNode.Value=data;
			destinationNode=destination.OwnerDocument.ImportNode(destinationNode,true);
			destination.AppendChild(destinationNode);
		}
		
		/// <summary>
		/// Expands a macro 
		/// </summary>
		/// <param name="instanceName">Always null</param>
		/// <param name="macroNode">The macro definition to expand</param>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void ExpandMacro(string instanceName, XmlNode macroNode, XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			variables=variables.CreateChildScope();
			variables.Declare(ExpandingNode,source);
						
			// Install any arguments
			foreach(XmlAttribute attr in source.Attributes)
			{
				if(attr.NamespaceURI=="")
				{
					// Unqualified attributes are expanded out to strings
					string name=attr.Name;
					string value=ApplySubstitutions(attr.Value,variables);
					
					// We need to declare/assign the variable
					variables.DeclareOrAssign(name,value);
				}
				else if(attr.NamespaceURI==ExpandNS)
				{
					// Any arguments in the expand namespace don't get special treatment.
					// Instead, the value of the argument is treated as the name of a variable
					// and the variable is looked up and passed without any type conversion
					string name=attr.LocalName;
					string requiredVariable=attr.Value;					
					
					object value=null;
					if(variables.TryLookup(requiredVariable,out value))
					{
						// We need to declare/assign the variable
						variables.DeclareOrAssign(name,value);
					}
					else
					{
						throw new XmlMacroExpanderException("variable not found: "+requiredVariable);
					}
				}
			}
			
			// See if there are any argument elements.
			// These allow you to pass structured xml as arguments
			foreach(XmlNode argNode in source.SelectNodes("m:Arg",m_NamespaceManager))
			{
				RequireAttributes(source.Name,argNode,"name");
				string name=argNode.Attributes["name"].Value;
				
				XmlNode expandedArg=ExpandXmlArgument(argNode,variables,scope);
				
				// We need to declare/assign the variable
				variables.DeclareOrAssign(name,expandedArg);
			}
			
			DoCompile(macroNode,destination,variables,scope);
		}
		
		/// <summary>
		/// XML arguments may contain macros which need to be expanded.
		/// </summary>
		/// <param name="argNode">The xml argument node to expand</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		/// <returns>The expanded argument</returns>
		private XmlNode ExpandXmlArgument(XmlNode argNode, NameScope variables, Scope scope)
		{
			// NOTE: Although we create an element we never actually add it to the document
			XmlElement element=argNode.OwnerDocument.CreateElement(argNode.Name,argNode.NamespaceURI);
			CopyAttributes(argNode,element,variables);
			
			variables=variables.CreateChildScope();
			DoCompile(argNode,element,variables,scope);
			
			return element;
		}
		
		
		/// <summary>
		/// Copies all non-macro attributes from source to destination
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		private void CopyAttributes(XmlNode source, XmlNode destination, NameScope variables)
		{
			foreach(XmlAttribute attr in source.Attributes)
			{
				// Ignore anything in the macro namespace
				if(attr.NamespaceURI==NS) continue;
				
				string value=ApplySubstitutions(attr.Value,variables);
				XmlAttribute newAttr=destination.OwnerDocument.CreateAttribute(attr.Name,attr.NamespaceURI);
				newAttr.Value=value;
				destination.Attributes.Append(newAttr);
			}
		}
		
		/// <summary>
		/// Applies any substitutions to a value
		/// </summary>
		/// <param name="value">The value to check for substitutions</param>
		/// <param name="variables">The current variables</param>
		private string ApplySubstitutions(string value, NameScope variables)
		{
			// Expand the mandatory tokens
			value=TokenExpander.ExpandText(value,"@{","}",delegate(string name)
			{
				object obj=variables.Lookup(name);
				return obj;
			});
			
			// Expand the optional tokens
			value=TokenExpander.ExpandText(value,"@[","]",delegate(string name)
			{
				object obj=variables.Lookup(name);
				return obj==null ? "" : obj;
			});
			
			return value;
		}
		
			
		
		/// <summary>
		/// Includes a file into the current scope
		/// </summary>
		/// <param name="filename">The name of the file to include</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void DoInclude(string filename, XmlNode destination, NameScope variables, Scope scope)
		{
			Uri currentSource=variables.Lookup(CurrentFile) as Uri;
			Uri newSource=DetermineUri(currentSource,filename);
			
			XmlDocument source=new XmlDocument();
			using(Stream stream=StorageManager.Get(newSource).OpenRead())
			{
				source.Load(stream);
			}
			
			// We'll change the cwd for the include and flip it back after we've compiled it
			variables.Assign(CurrentFile,newSource);
			
			scope.PushInclude(filename);
			// NOTE: We use ScopeRule.UseExisting to add any macros to our scope
			DoCompile(source.DocumentElement,destination,variables,scope,ScopeRule.UseExisting);
			scope.PopInclude();
			
			variables.Assign(CurrentFile,currentSource);
		}
		
		private Uri DetermineUri(Uri currentSource, string newSource)
		{
			if(currentSource==null) return Accessor.CreateUri(newSource);
			return Accessor.ResolveRelative(currentSource,newSource);
		}
		
		/// <summary>
		/// Defines a new macro
		/// <![CDATA[ <m:define name="foo"> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void Define(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("Define",source,"name");
		
			XmlNode nameAttr=source.Attributes.GetNamedItem("name");
			string name=nameAttr.Value;
			scope.Add(name,source);
		}
		
		/// <summary>
		/// Declares a new variable
		/// <![CDATA[ <m:Declare name="foo">value</m:Declare> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void Declare(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("Declare",source,"name");
		
			XmlNode nameAttr=source.Attributes.GetNamedItem("name");
			
			string name=nameAttr.Value;
			object value=CreateVariableValue(source,variables,scope);
			
			if(variables.Declare(name,value)==false)
			{
				throw new XmlMacroExpanderException(name+" is already declared");
			}
		}
		
		/// <summary>
		/// Tries to declare a new variable
		/// <![CDATA[ <m:TryDeclare name="foo">bar</m:TryDeclare> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void TryDeclare(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("TryDeclare",source,"name");
		
			XmlNode nameAttr=source.Attributes.GetNamedItem("name");
			string name=nameAttr.Value;
			
			string declarationScope=GetDeclarationScope("TryDeclare",source);
			
			if(declarationScope=="local")
			{
				// Only declare if the variable doesn't exist in the local scope
				if(variables.IsDeclaredInActiveScope(name)) return;
			}
			else
			{
				// Only declare if there's no variable present at any scope
				object temp;
				if(variables.TryLookup(name,out temp)) return;
			}
			
			object value=CreateVariableValue(source,variables,scope);
			variables.Declare(name,value);
		}
		
		
		/// <summary>
		/// Assigns a value to a previously declared variable
		/// <![CDATA[ <m:Assign name="foo">bar</m:Assign> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void Assign(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("Assign",source,"name");
		
			XmlNode nameAttr=source.Attributes.GetNamedItem("name");
			
			string name=nameAttr.Value;
			object value=CreateVariableValue(source,variables,scope);
			
			if(variables.Assign(name,value)==false)
			{
				throw new XmlMacroExpanderException(name+" is not declared");
			}
		}
		
		/// <summary>
		/// Creates a variable value that will be assigned to a named variable
		/// </summary>
		/// <param name="source">The source element. Typically a declare,assign or try-declare element</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		/// <returns>The value for the variable</returns>
		private object CreateVariableValue(XmlNode source, NameScope variables, Scope scope)
		{
			object value=null;
		
			XmlAttribute typeNode=source.Attributes["type"];
			if(typeNode!=null && typeNode.Value=="xml") 
			{
				// Leave it as xml. This allows the user to create "data islands"
				value=ExpandXmlArgument(source,variables,scope);
				//value=source;
			}
			else
			{		
				string expanded=ApplySubstitutions(source.InnerText,variables);			
				if(typeNode!=null) 
				{
					value=ConvertToType(expanded,typeNode);
				}
				else
				{
					value=expanded;
				}
			}
			
			return value;
		}
		
		/// <summary>
		/// Requires a variable to be available. If not an exception is thrown and expansion will stop
		/// <![CDATA[ <m:Require name="foo"/>]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void Require(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("Require",source,"name");
			
			string name=source.Attributes.GetNamedItem("name").Value;
			string declarationScope=GetDeclarationScope("Require",source);
			
			bool found=false;
			if(declarationScope=="local")
			{
				// Only declare if the variable doesn't exist in the local scope
				found=variables.IsDeclaredInActiveScope(name);
			}
			else
			{
				object value;
				found=variables.TryLookup(name,out value); 
			}
			
			if(found==false) throw new XmlMacroExpanderException("required variable not declared: "+name);
		}
		
		private string GetDeclarationScope(string construct, XmlNode node)
		{
			return GetAttributeValue(construct,node,"scope","local","local","any");
		}
		
		private object ConvertToType(string value, XmlNode typeNode)
		{
			string type=XmlTypeResolver.ExtractTypeName(typeNode);
			Type actualType=TypeResolver.GetEncodedType(type);
			return TypeResolver.CoerceToType(actualType,value);
		}
		
		
		/// <summary>
		/// Loops over a set of values
		/// <![CDATA[ <m:ForEach name="index" values="1,2,3,4"> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void ForEach(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("ForEach",source,"name","values");
			
			XmlNode nameNode=source.Attributes["name"];
			XmlNode valuesNode=source.Attributes["values"];
			
			char split=',';
			XmlNode splitNode=source.Attributes["split"];
			if(splitNode!=null && splitNode.Value.Length!=0)
			{
				split=splitNode.Value[0];
			}
			
			string[] names=ApplySubstitutions(nameNode.Value,variables).Split(',');
			string[] values=ApplySubstitutions(valuesNode.Value,variables).Split(split);
			
			for(int i=0; i+names.Length<=values.Length; i+=names.Length)
			{
				NameScope inner=variables.CreateChildScope();
				
				for(int j=0; j<names.Length; j++)
				{
					inner.Declare(names[j],values[i+j]);
				}
				
				DoCompile(source,destination,inner,scope);
			}
		}
		
		/// <summary>
		/// Applies settings to the macro expander
		/// <![CDATA[ <m:Pragma name="foo" value="bar" /> ]]>
		/// 
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		/// <remarks>
		/// Valid variables are:
		/// 
		///		include-depth:			the value is an integer>0 that specifies the maximum depth
		///		fail:					forces expansion to fail, throwing an exception with value as the message
		///		strip-macro-namespaces:	true to remove the xml macro namespaces from the root element, false otherwise
		///		script:					sets the default scripting language
		///		strict:					sets strict error reporting
		/// </remarks>
		private void Pragma(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("Pragma",source,"name","value");
		
			string variable=source.Attributes["name"].Value;
			string value=source.Attributes["value"].Value;
			
			switch(variable.Trim().ToLower())
			{
				case "include-depth":
					int includeDepth=0;
					if(int.TryParse(value,out includeDepth) && includeDepth>0)
					{
						m_MaximumIncludeDepth=includeDepth;
					}
					break;
					
				case "fail":
					throw new XmlMacroExpanderException("pragma fail: "+value);
					
				case "strict":
					bool strict;
					if(bool.TryParse(value,out strict))
					{
						m_Strict=strict;
					}
					break;
					
				case "strip-macro-namespaces":
					bool strip=false;
					if(bool.TryParse(value,out strip) && strip)
					{
						RemoveMacroNamespaces(destination);
					}
					break;
					
				default:
					// Ignore any unknown pragmas
					break;
			}
		}
		
		private void RemoveMacroNamespaces(XmlNode node)
		{
			XmlDocument doc=node.OwnerDocument;
			XmlNode root=doc.DocumentElement;
			
			List<XmlAttribute> toRemove=new List<XmlAttribute>();
			
			foreach(XmlAttribute attrNode in root.Attributes)
			{
				if(attrNode.Value==NS || attrNode.Value==ExpandNS)
				{
					toRemove.Add(attrNode);
				}
			}
			
			foreach(XmlAttribute attrNode in toRemove)
			{
				root.Attributes.Remove(attrNode);
			}
		}
		
	
	
		
		/// <summary>
		/// Passes all children through without expanding them
		/// <![CDATA[ <m:Pass> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void Pass(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			foreach(XmlNode node in source.ChildNodes)
			{
				XmlNode destinationNode=destination.OwnerDocument.ImportNode(node.Clone(),true);
				destination.AppendChild(destinationNode);
			}
		}
		
		/// <summary>
		/// Comments out all child elements
		/// <![CDATA[ <m:Comment> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void Comment(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			// Does nothing!
		}
		
		/// <summary>
		/// Stores metadata in the xml which an app can query before or after exmapnsion.
		/// By default the metadata is not copied to the destination. If "copy" is true the
		/// data will go to the destination
		/// <![CDATA[ <m:Meta name="some-name" copy="true/false"> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void Meta(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			// Does nothing!
			bool copy=false;
			
			XmlAttribute copyAttr=source.Attributes["copy"];
			if(copyAttr!=null) bool.TryParse(copyAttr.Value,out copy);
			
			if(copy)
			{
				CopyElement((XmlElement)source,destination,variables,scope);
			}
		}
		
		
		/// <summary>
		/// Injects xml nodes or xml node lists that are held in a variable
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <param name="variables"></param>
		/// <param name="scope"></param>
		private void InjectXml(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("InjectXml",source,"name");
			
			string variable=source.Attributes["name"].Value;
						
			object value;
			if(variables.TryLookup(variable,out value)==false)
			{
				throw new XmlMacroExpanderException("InjectXml: variable not found: "+variable);
			}
			
			string select=null;
			XmlAttribute selectAttr=source.Attributes["select"];
			XmlAttribute selectSingleAttr=source.Attributes["selectSingle"];
			if(selectAttr!=null) select=selectAttr.Value;
			
			// If there's a select then it's against the variable
			if(value is XmlNode)
			{
				if(selectAttr!=null)
				{
					string xpath=ApplySubstitutions(selectAttr.Value,variables);
					value=((XmlNode)value).SelectNodes(xpath);
				}
				else if(selectSingleAttr!=null)
				{
					string xpath=ApplySubstitutions(selectSingleAttr.Value,variables);
					value=((XmlNode)value).SelectSingleNode(xpath);
				}
			}
			else if(selectAttr!=null || selectSingleAttr!=null)
			{
				// If the user has tried to select on anything else (such as an XmlNodeList)
				// then it doesn't make any sense and we should bail out
				throw new XmlMacroExpanderException("InjectXml: can only apply select or selectSingle to an XmlNode");
			}
			
			if(value is XmlDocument)
			{
				XmlDocument doc=(XmlDocument)value;
				CopyElement(doc.DocumentElement,destination,variables,scope);
			}
			else if(value is XmlElement)
			{
				CopyElement((XmlElement)value,destination,variables,scope);
			}
			else if(value is XmlNodeList)
			{
				XmlNodeList nodes=(XmlNodeList)value;
				foreach(XmlNode node in nodes)
				{
					if(node is XmlElement)
					{
						XmlElement sourceElement=(XmlElement)node;
						CopyElement(sourceElement,destination,variables,scope);
					}
					else if(node is XmlCharacterData)
					{
						ProcessXmlCharacterData((XmlCharacterData)node,destination,variables);
					}
				}
			}
			else
			{
				throw new XmlMacroExpanderException("InjectXml: not xml: "+variable);
			}
		}
		
		private void CopyElement(XmlElement sourceElement, XmlNode destination, NameScope variables, Scope scope)
		{
			XmlElement element=destination.OwnerDocument.CreateElement(sourceElement.Name,sourceElement.NamespaceURI);
			CopyAttributes(sourceElement,element,variables);
			DoCompile(sourceElement,element,variables.CreateChildScope(),scope);
			destination.AppendChild(element);
		}
		
		private void ProcessXmlCharacterData(XmlCharacterData node, XmlNode destination, NameScope variables)
		{
			if(node.NodeType==XmlNodeType.Comment)
			{
				// Comments may have macro code in and we won't want to
				// expand it as it may fail. We need an explicit check here
				// as XmlComment derives from XmlCharacterData and would pass
				// the test below
				XmlNode destinationNode=destination.OwnerDocument.ImportNode(node.Clone(),true);
				destination.AppendChild(destinationNode);
			}
			else
			{
				// Do a substitution and add
				XmlCharacterData charData=(XmlCharacterData)node;
				AddCharacterData(charData,destination,variables);
			}
		}
		
		/// <summary>
		/// Includes an xml document into the current destination location
		/// <![CDATA[ <m:Include uri="foo" /> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void Include(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			if(scope.IncludeCount==m_MaximumIncludeDepth)
			{
				throw new XmlMacroExpanderException("Include depth exceeded: "+m_MaximumIncludeDepth);
			}
			
			RequireAttributes("Include",source,"uri");
			
			XmlNode filenameNode=source.Attributes["uri"];
			string filename=ApplySubstitutions(filenameNode.Value,variables);
			
			DoInclude(filename,destination,variables,scope);
		}
		
		/// <summary>
		/// Loads xml into an existing variable.
		/// You can specify "select" or "selectSingle" to restrict what is stored, otherwise the document is stored
		/// <![CDATA[ <m:LoadXml name="destination-variable" uri="foo" /> ]]>
		/// </summary>
		/// <param name="source">The source to process</param>
		/// <param name="destination">Where to store the compiled xml</param>
		/// <param name="variables">The current variables</param>
		/// <param name="scope">The current scope</param>
		private void LoadXml(XmlNode source, XmlNode destination, NameScope variables, Scope scope)
		{
			RequireAttributes("LoadXml",source,"name","uri");
			
			string name=source.Attributes["name"].Value;
			string uriString=source.Attributes["uri"].Value;
			uriString=ApplySubstitutions(uriString,variables);
			
			Uri currentSource=variables.Lookup(CurrentFile) as Uri;
			Uri uri=DetermineUri(currentSource,uriString);
			
			XmlDocument document=new XmlDocument();
			using(Stream stream=StorageManager.Get(uri).OpenRead())
			{
				document.Load(stream);
			}
			
			// If there's a select clause then process it
			XmlAttribute selectAttr=source.Attributes["select"];
			XmlAttribute selectSingleAttr=source.Attributes["selectSingle"];
			
			if(selectAttr!=null)
			{
				string xpath=ApplySubstitutions(selectAttr.Value,variables);
				XmlNodeList nodes=document.SelectNodes(xpath);
				variables.Assign(name,nodes);
			}
			else if(selectSingleAttr!=null)
			{
				string xpath=ApplySubstitutions(selectSingleAttr.Value,variables);
				XmlNode node=document.SelectSingleNode(xpath);
				if(node==null) throw new XmlMacroExpanderException("LoadXml could not select a single node with "+xpath);
				variables.Assign(name,node);
			}
			else
			{
				variables.Assign(name,document);
			}
		}
		
		
		/// <summary>
		/// Checks that mandatory attributes are present
		/// </summary>
		/// <param name="construct">The macro construct</param>
		/// <param name="node">The node to check</param>
		/// <param name="names">The names to check for</param>
		private void RequireAttributes(string construct, XmlNode node, params string[] names)
		{
			for(int i=0; i<names.Length; i++)
			{
				XmlNode attrNode=node.Attributes[names[i]];
				if(attrNode==null) throw new XmlMacroExpanderException(construct+" required attribute "+names[i]);
			}
		}
		
		private string GetAttributeValue(string construct, XmlNode node, string name, string defaultValue, params string[] validValues)
		{
			XmlAttribute attr=node.Attributes[name];
			if(attr==null) return defaultValue;
			
			string value=attr.Value;
			if(Array.IndexOf(validValues,value)==-1)
			{
				throw new XmlMacroExpanderException(construct+": invalid value for "+name);
			}
			
			return value;
		}
		
		/// <summary>
		/// Create a namespace manager with mappings for the expander namespaces.
		/// "m" is mapped to XmlMacroExpander.NS and "x" is mapped to XmlMacroExpander.ExpandNS
		/// </summary>
		/// <returns>A namespace manager instance</returns>
		public static XmlNamespaceManager CreateNamespaceManager()
		{
			XmlNamespaceManager manager=new XmlNamespaceManager(new NameTable());
			manager.AddNamespace("m",NS);
			manager.AddNamespace("x",ExpandNS);
			
			return manager;
		}
	}
}

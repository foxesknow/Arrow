using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Arrow.Settings;
using Arrow.Collections;

namespace Arrow.Xml.Macro
{
    /// <summary>
    /// Represent a namespace scope
    /// </summary>
    class NameScope
    {
        private NameScope? m_Previous;

        private Dictionary<string, ScopedItem> m_Variables = new Dictionary<string, ScopedItem>(IgnoreCaseEqualityComparer.Instance);

        private bool m_Locked;

        private Dictionary<string, object>? m_LanguageData;

        private readonly object m_SyncRoot;

        /// <summary>
        /// Initializes a root scope 
        /// </summary>
        public NameScope()
        {
            // Only the root has a language data instance
            m_LanguageData = new Dictionary<string, object>();

            m_SyncRoot = new object();
        }

        /// <summary>
        /// Initializes a new child scope 
        /// </summary>
        /// <param name="previous">The parent scope</param>
        protected NameScope(NameScope previous)
        {
            m_Previous = previous;

            // Everyone shares the same lock
            m_SyncRoot = previous.m_SyncRoot;
        }

        /// <summary>
        /// The synchronization object used by the instance
        /// </summary>
        public object SyncRoot
        {
            get{return m_SyncRoot;}
        }

        /// <summary>
        /// Creates a new scope with the current instance as the parent
        /// </summary>
        /// <returns>A new scope</returns>
        public NameScope CreateChildScope()
        {
            return new NameScope(this);
        }

        private NameScope GetRootScope()
        {
            NameScope current = this;
            while(current.m_Previous != null)
            {
                current = current.m_Previous;
            }

            return current;
        }

        /// <summary>
        /// Returns the previous scope, or null if there is no previous scope
        /// </summary>
        protected internal NameScope? Previous
        {
            get{return m_Previous;}
        }

        private ScopedItem? FindItem(string name)
        {
            ScopedItem? item = null;

            for(NameScope? scope = this; scope != null && item == null; scope = scope.Previous)
            {
                item = scope.FindInCurrentScope(name);
            }

            return item;
        }

        private ScopedItem? FindInCurrentScope(string name)
        {
            m_Variables.TryGetValue(name, out var item);
            return item;
        }

        /// <summary>
        /// Returns the names of all variables in the current scope
        /// </summary>
        /// <returns></returns>
        public List<string> GetNames()
        {
            lock(m_SyncRoot)
            {
                List<string> names = new List<string>(m_Variables.Keys);
                return names;
            }
        }

        /// <summary>
        /// Locks the scope so that no changes can be made to it
        /// </summary>
        public void Lock()
        {
            lock(m_SyncRoot)
            {
                if(m_Locked == false)
                {
                    foreach(ScopedItem item in m_Variables.Values)
                    {
                        item.ScopedItemMode = ScopedItemMode.ReadOnly;
                    }

                    m_Locked = true;
                }
            }
        }

        /// <summary>
        /// Returns true if the scope is locked, false otherwise
        /// </summary>
        public bool IsLocked
        {
            get{return m_Locked;}
        }

        /// <summary>
        /// Gets the value for a variable or throw an exception if the variable does not exist
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The value of the variable</returns>
        public object GetValue(string name)
        {
            if(name == null) throw new ArgumentNullException("name");

            lock(m_SyncRoot)
            {
                var item = FindItem(name);

                if(item == null) throw new ArgumentException("variable not found: " + name);
                return item.Value;
            }
        }

        /// <summary>
        /// Looks up an item
        /// </summary>
        /// <param name="name">The name of the item to lookup</param>
        /// <returns>The value for the item, or null if it could not be found</returns>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public object? Lookup(string name)
        {
            if(name == null) throw new ArgumentNullException("name");

            lock(m_SyncRoot)
            {
                var item = FindItem(name);
                return item == null ? null : item.Value;
            }
        }

        /// <summary>
        /// Attempts to lookup an item
        /// </summary>
        /// <param name="name">The name of the itme to lookup</param>
        /// <param name="value">On success the value of the item</param>
        /// <returns>true if the name as found, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public bool TryLookup(string name, out object? value)
        {
            if(name == null) throw new ArgumentNullException("name");

            lock(m_SyncRoot)
            {
                var item = FindItem(name);
                value = (item == null ? null : item.Value);

                return item != null;
            }
        }

        /// <summary>
        /// Attempts to declare a read-write variable in the current scope.
        /// If a variable with the same name is already declared this method will return false.
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="value">The value for the variable</param>
        /// <returns>true if the variable was declared, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public bool Declare(string name, object value)
        {
            return Declare(name, value, ScopedItemMode.ReadWrite);
        }

        /// <summary>
        /// Attempts to declare a variable in the current scope.
        /// If a variable with the same name is already declared this method will return false.
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="value">The value for the variable</param>
        /// <param name="mode">The mode for the value</param>
        /// <returns>true if the variable was declared, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public bool Declare(string name, object value, ScopedItemMode mode)
        {
            if(name == null) throw new ArgumentNullException("name");

            lock(m_SyncRoot)
            {
                if(m_Locked) return false;
                if(FindInCurrentScope(name) != null) return false;

                ScopedItem item = new ScopedItem(value, mode);
                m_Variables[name] = item;

                return true;
            }
        }

        /// <summary>
        /// Tries to declare a read-write variable. If the variable is already declared then it will be assigned to
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="value">The value for the variable</param>
        /// <returns>true is declared or assigned, false otherwise</returns>
        public bool DeclareOrAssign(string name, object value)
        {
            return DeclareOrAssign(name, value, ScopedItemMode.ReadWrite);
        }

        /// <summary>
        /// Tries to declare a variable. If the variable is already declared then it will be assigned to
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="value">The value for the variable</param>
        /// <param name="mode">The mode for the value</param>
        /// <returns>true is declared or assigned, false otherwise</returns>
        public bool DeclareOrAssign(string name, object value, ScopedItemMode mode)
        {
            lock(m_SyncRoot)
            {
                bool success = Declare(name, value, mode);
                if(success == false)
                {
                    success = Assign(name, value);
                }

                return success;
            }
        }

        /// <summary>
        /// Attempts to assign a value to a previous declared variable.
        /// If the variable is read-only this method will fail
        /// </summary>
        /// <param name="name">The name of the variable to assign to</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>true if the variable was changed, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public bool Assign(string name, object value)
        {
            if(name == null) throw new ArgumentNullException("name");

            lock(m_SyncRoot)
            {
                bool assigned = false;

                var item = FindItem(name);
                if(item != null && item.ScopedItemMode == ScopedItemMode.ReadWrite)
                {
                    item.Value = value;
                    assigned = true;
                }

                return assigned;
            }
        }

        /// <summary>
        /// Assigns a value to a variable. If the variable does not exist it will be declared
        /// </summary>
        /// <param name="name">The name of the variable to assign to</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>true if the value was set, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public bool AssignOrDeclare(string name, object value)
        {
            lock(m_SyncRoot)
            {
                bool success = Assign(name, value);
                if(success == false) success = Declare(name, value);
                return success;
            }
        }

        /// <summary>
        /// Determines if a variable is declared in the active scope
        /// </summary>
        /// <param name="name">The name of the variable to check</param>
        /// <returns>true if the variable is declared, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public bool IsDeclaredInActiveScope(string name)
        {
            if(name == null) throw new ArgumentNullException("name");

            lock(m_SyncRoot)
            {
                return FindInCurrentScope(name) != null;
            }
        }

        /// <summary>
        /// Tries to get the scoped item mode for a variable
        /// </summary>
        /// <param name="name">The name of the variable to get</param>
        /// <param name="mode">On success, the mode of the variable</param>
        /// <returns>true if the variable is found, otherwise false</returns>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public bool TryGetMode(string name, out ScopedItemMode mode)
        {
            if(name == null) throw new ArgumentNullException("name");

            lock(m_SyncRoot)
            {
                bool found = false;

                var item = FindItem(name);
                if(item != null)
                {
                    found = true;
                    mode = item.ScopedItemMode;
                }
                else
                {
                    mode = ScopedItemMode.Unknown;
                }

                return found;
            }
        }

        /// <summary>
        /// Creates a root NameScope instance for use within scripts
        /// </summary>
        /// <returns>A NameScope instance</returns>
        public static NameScope CreateScriptNameScope()
        {
            NameScope scope = new NameScope();
            return scope;
        }

        public object GetVariable(string variableName)
        {
            lock(m_SyncRoot)
            {
                var item = FindItem(variableName);
                if(item == null) throw new XmlMacroExpanderException("variable not found: " + variableName);

                return item.Value;
            }
        }

        public bool TryGetVariable(string variableName, out object? result)
        {
            result = null;

            lock(m_SyncRoot)
            {
                var item = FindItem(variableName);
                if(item != null) result = item.Value;
                return item != null;
            }
        }

        public bool IsDeclared(string variableName)
        {
            lock(m_SyncRoot)
            {
                var item = FindItem(variableName);
                return item != null;
            }
        }
    }
}

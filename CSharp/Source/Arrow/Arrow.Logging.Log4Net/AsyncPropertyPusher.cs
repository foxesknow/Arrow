using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;

using log4net.Util;
using log4net.Core;
using System.Threading;

namespace Arrow.Logging.Log4Net
{
    /// <summary>
    /// Pushes properties into a log4net context
    /// </summary>
    internal class AsyncPropertyPusher : PropertyPusherBase
    {
        private static readonly AsyncLocal<PropertyNode> s_Properties = new();

        protected override IDisposable PushProperty(string name, object value)
        {
            throw new NotImplementedException();
        }

        public static void Combine(PropertiesDictionary target)
        {
            if(target is null) return;

            var node = s_Properties.Value;
            if(node is null && GlobalProperties.IsEmpty()) return;

            var seen = new HashSet<string>();

            while(node is not null)
            {
                if(seen.Add(node.Name))
                {
                    var value = ActiveProperties.Expand(node.Name, node.Value);
                    Combine(target, node.Name, value);
                }

                node = node.Next;
            }

            GlobalProperties.Apply((seen, target), static (state, name, value) => ApplyGlobalProperty(state.seen, state.target, name, value));
        }


        private static void Combine(PropertiesDictionary target, string name, object value)
        {
            if(value is IFixingRequired fixable)
            {
                value = fixable.GetFixedObject();
            }

            if(value is not null)
            {
                target[name] = value;
            }
        }

        private static void ApplyGlobalProperty(HashSet<string> seen, PropertiesDictionary target, string name, object unexpandedValue)
        {
            if(seen.Add(name))
            {
                var value = ActiveProperties.Expand(name, unexpandedValue);
                Combine(target, name, value);
            }
        }

        private class PropertyNode : IDisposable
        {
            public PropertyNode(string name, object value, PropertyNode next)
            {
                this.Name = name;
                this.Value = value;
                this.Next = next;
            }

            public string Name{get;}
            public object Value{get;}
            public PropertyNode Next{get;}

            public void Dispose()
            {
                s_Properties.Value = this.Next;
            }
        }
    }
}

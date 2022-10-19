using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

#nullable enable

namespace Arrow.Logging
{
    public abstract class PropertyPusherBase : IPropertyPusher
    {
        /// <inheritdoc/>
        public IDisposable Push(IEnumerable<(string Name, object? Value)> properties)
        {
            if(properties is null) return NullDisposable.Instance;

            var head = NullDisposable.Instance;

            foreach(var (name, value) in properties)
            {
                if(string.IsNullOrWhiteSpace(name)) continue;

                var newHead = PushProperty(name, value);
                if(newHead is not null)
                {
                    head = newHead.Cons(head);
                }
            }

            return head;
        }

        /// <summary>
        /// Pushes the property into the log context and returns an object that can remove it
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract IDisposable? PushProperty(string name, object? value);
    }
}

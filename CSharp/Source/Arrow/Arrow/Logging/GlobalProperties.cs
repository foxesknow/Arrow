using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Logging
{
    public static class GlobalProperties
    {
        private static Node? s_Head;

        /// <summary>
        /// Adds a new global property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void Push(string name, object value)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrEmpty(name)) throw new ArgumentException("invalid name", nameof(name));

            Node? current = null;
            var existing = s_Head;

            do
            {
                current = existing;
                var newHead = new Node(name, value, current);
                existing = Interlocked.CompareExchange(ref s_Head, newHead, current);
            }while(current != existing);
        }

        /// <summary>
        /// Checks to see if the global properties are empty
        /// </summary>
        /// <returns></returns>
        public static bool IsEmpty()
        {
            return Interlocked.CompareExchange(ref s_Head, null, null) is null;
        }

        /// <summary>
        /// Clears all properties
        /// </summary>
        public static void Clear()
        {
            Interlocked.Exchange(ref s_Head, null);
        }

        /// <summary>
        /// Applies an action to all properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state"></param>
        /// <param name="action"></param>
        public static void Apply<T>(T state, Action<T, string, object?> action)
        {
            if(action is null) return;

            var head = Interlocked.CompareExchange(ref s_Head, null, null);

            for(var node = head; node is not null; node = node.Next)
            {
                action(state, node.Name, node.Value);
            }
        }

        private class Node
        {
            public Node(string name, object value, Node? next)
            {
                this.Name = name;
                this.Value = value;
                this.Next = next;
            }

            public string Name{get;}
            public object? Value{get;}
            public Node? Next{get;}
        }
    }
}

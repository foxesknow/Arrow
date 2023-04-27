using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
    /// <summary>
    /// Allows you to force a reference to assembly that may not be used in your code
    /// but your application will dynamically load, adn so you don't want the compiler
    /// to be clever and not copy it to the output directory
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ForceAssemblyReferenceAttribute : Attribute
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="type">A type from an assembly we want to force a reference to</param>
        public ForceAssemblyReferenceAttribute(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// The type we used to force the reference
        /// </summary>
        public Type Type{get;}

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Type.ToString();
        }
    }
}

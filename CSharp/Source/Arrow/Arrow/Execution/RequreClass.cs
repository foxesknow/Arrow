using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Allows you to overload a method which take a generic parameter
    /// that name the same number of parameters but a generic contraint
    /// restricting the type to be a class of struct
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <example>
    /// Instead of
    /// <![CDATA[
    /// void Foo<T>(T value) where T : struct{}
    /// void Foo<T>(T value) where T : class{}
    /// 
    /// which won't compile, use:
    /// 
    /// void Foo<T>(T value) where T : struct{}
    /// void Foo<T>(T value, RequireClass<T>? _ = null) where T : class{}
    /// ]]>
    /// </example>
    public sealed class RequreClass<T> where T : class
    {
        private RequreClass()
        {
        }
    }
}

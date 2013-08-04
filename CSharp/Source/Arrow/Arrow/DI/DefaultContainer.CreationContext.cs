using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Execution;

namespace Arrow.DI
{
	public partial class DefaultContainer
	{
		/// <summary>
		/// The creation context is used to detect circular type dependencies during construction
		/// </summary>
		class CreationContext
		{
			private HashSet<Type> m_ActiveTypes=new HashSet<Type>();

			public IDisposable Scope(Type type)
			{
				if(m_ActiveTypes.Contains(type)) throw new ContainerException("Circular type dependency: "+type.ToString());

				return new Disposer(()=>End(type));
			}

			private void End(Type type)
			{
				m_ActiveTypes.Remove(type);
			}
		}
	}
}

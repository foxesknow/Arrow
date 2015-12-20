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
			private readonly HashSet<Type> m_ActiveTypes=new HashSet<Type>();
			private readonly DefaultContainer m_StartContainer;

			public CreationContext(DefaultContainer startContainer)
			{
				m_StartContainer=startContainer;
			}

			public DefaultContainer StartContainer
			{
				get{return m_StartContainer;}
			}

			public IDisposable Scope(Type type)
			{
				if(m_ActiveTypes.Contains(type)) throw new ContainerException("Circular type dependency: "+type.ToString());

				m_ActiveTypes.Add(type);
				return new Disposer(()=>End(type));
			}

			private void End(Type type)
			{
				m_ActiveTypes.Remove(type);
			}
		}
	}
}

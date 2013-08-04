using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Storage
{
	class LambdaAccessorFactory : IAccessorFactory
	{
		private readonly Func<Uri,Accessor> m_Creator;

		public LambdaAccessorFactory(Func<Uri,Accessor> creator)
		{
			m_Creator=creator;
		}

		public Accessor Create(Uri uri)
		{
			return m_Creator(uri);
		}
	}
}

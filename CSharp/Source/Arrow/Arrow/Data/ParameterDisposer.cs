using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data
{
    /// <summary>
    /// Manages any database parameters that may be disposable, such as Oracle parameters.
    /// </summary>
    public sealed class ParameterDisposer : IDisposable
    {
        private List<IDisposable>? m_Parameters;

        /// <summary>
        /// Stores a parameter if it is disposable.
        /// </summary>
        /// <param name="parameter"></param>
        public void Add(IDbDataParameter parameter)
        {
            if(parameter is IDisposable disposable)
            {
                if(m_Parameters is null) m_Parameters = new();

                m_Parameters.Add(disposable);
            }
        }

        /// <summary>
        /// Disposes of any parameters
        /// </summary>
        public void Dispose()
        {
            if(m_Parameters is not null)
            {
                foreach(var parameter in m_Parameters)
                {
                    parameter.Dispose();
                }
            }

            m_Parameters = null;
        }
    }
}

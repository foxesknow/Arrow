using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// The result of reading data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public readonly ref struct ReadResult<TData> where TData : class
    {
        public ReadResult(bool isSubscribed, TData? data)
        {
            this.IsSubscribed = isSubscribed;
            this.Data = data;
        }

        /// <summary>
        /// True if the data is subscribed to
        /// </summary>
        public bool IsSubscribed{get;}

        /// <summary>
        /// The alertable data.
        /// If IsSubscibed is false then the data is always false
        /// </summary>
        public TData? Data{get;}
    }
}

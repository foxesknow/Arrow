using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Data.DatabaseManagers
{
    /// <summary>
    /// Database details
    /// </summary>
    public sealed class Database
    {
        /// <summary>
        /// The name of the database
        /// </summary>
        public string? Name{get; set;}

        public bool Transactional{get; set;}

        /// <summary>
        /// The specific details for the database
        /// </summary>
        public DatabaseDetails? Details{get; set;}
    }
}

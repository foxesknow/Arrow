using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench
{
    /// <summary>
    /// Defines how the script will be run
    /// </summary>
    public sealed class RunConfig
    {
        private RunConfig(RunConfig.Mode mode, string groupName)
        {
            this.RunMode = mode;
            this.GroupName = groupName;
        }

        public RunConfig.Mode RunMode{get;}

        public string GroupName{get;}

        public override string ToString()
        {
            return this.RunMode switch
            {
                Mode.All => $"RunMode = {RunMode}",
                _        =>  $"RunMode = {RunMode}, GroupName = {GroupName}"
            };            
        }

        /// <summary>
        /// Runs all groups in the script
        /// </summary>
        /// <returns></returns>
        public static RunConfig All()
        {
            return new(Mode.All, "");
        }        

        /// <summary>
        /// Runs a single group in the script
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static RunConfig Single(string groupName)
        {
            if(groupName is null) throw new ArgumentNullException(nameof(groupName));
            if(string.IsNullOrWhiteSpace(groupName)) throw new ArgumentException("invalid group name", nameof(groupName));

            return new(Mode.Single, groupName);
        }

        /// <summary>
        /// Runs from a group to the end of the script
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static RunConfig From(string groupName)
        {
            if(groupName is null) throw new ArgumentNullException(nameof(groupName));
            if(string.IsNullOrWhiteSpace(groupName)) throw new ArgumentException("invalid group name", nameof(groupName));

            return new(Mode.From, groupName);
        }

        public enum Mode
        {
            /// <summary>
            /// Run all groups
            /// </summary>
            All,

            /// <summary>
            /// Run a single group
            /// </summary>
            Single,

            /// <summary>
            /// Run from a given group to the end of the script
            /// </summary>
            From
        }
    }
}

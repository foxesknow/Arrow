using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    /// <summary>
    /// Defines how the script will be run
    /// </summary>
    public sealed class RunConfig
    {
        private RunConfig(RunMode runMode, string groupName)
        {
            this.RunMode = runMode;
            this.GroupName = groupName;
        }

        public RunMode RunMode{get;}

        public string GroupName{get;}

        public override string ToString()
        {
            return this.RunMode switch
            {
                RunMode.All => $"RunMode = {RunMode}",
                _           =>  $"RunMode = {RunMode}, GroupName = {GroupName}"
            };            
        }

        /// <summary>
        /// Runs all groups in the script
        /// </summary>
        /// <returns></returns>
        public static RunConfig All()
        {
            return new(RunMode.All, "");
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

            return new(RunMode.Single, groupName);
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

            return new(RunMode.From, groupName);
        }
    }
}

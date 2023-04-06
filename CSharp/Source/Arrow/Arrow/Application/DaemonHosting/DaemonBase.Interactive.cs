using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Application.DaemonHosting
{
    public abstract partial class DaemonBase
    {
        private delegate IEnumerable<object?> FilterFunction(IEnumerable<object?> input);

        private static readonly string[] BlankLine = new[]{""};

        private bool m_KeepRunning = true;

        private readonly Dictionary<string, InteractiveCommand> m_Commands = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, InteractiveFilter> m_Filters = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Indicates if the application should keep running.
        /// </summary>
        public bool KeepRunning
        {
            get
            {
                lock(this.KeepRunningMonitor)
                {
                    return m_KeepRunning;
                }
            }

            set
            {
                lock(this.KeepRunningMonitor)
                {
                    m_KeepRunning = value;
                    Monitor.Pulse(this.KeepRunningMonitor);
                }
            }
        }

        /// <summary>
        /// Returns the available commands.
        /// If you override this then you must call the base version
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<InteractiveCommand> GetCommands()
        {
            yield return new("?", "Displays the commands available", part => Commands.CommandHelp(part, m_Commands));
            yield return new("Cls", "Clears the screen", Commands.Cls);
            yield return new("Filters", "Displays the filters available", part => Commands.FilterHelp(part, m_Filters));
            yield return new("Now", "Displays the application time", Commands.Now);
            yield return new("GetSetting", "settingName", "Displays the arrow setting", Commands.GetSetting);
            yield return new("Echo", "Echoes back its arguments", Commands.Echo);
            yield return new("Exit", "Exits the application", part => Commands.Exit(part, this));
            yield return new("Motivate", "number", "Says something motivational", Commands.Motivate);
        }

        /// <summary>
        /// Returns the available filters.
        /// If you override this then you must call the base version
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<InteractiveFilter> GetFilters()
        {
            yield return new("Match", "pattern", "Matches a string", Filters.Match);
            yield return new("Regex", "pattern", "Matches a string", Filters.Regex);
            
            yield return new("Top", "numberOfItems", "Returns the top-most items", Filters.Top);
            yield return new("Bottom", "numberOfItems", "Returns the bottom-most items", Filters.Bottom);
            yield return new("Count", "Returns number of items", (args, items) => SingleItem(items.Count()));
            yield return new("Reverse", "Reverses the items", (args, items) => items.Reverse());
            yield return new("First", "Returns the first item", (args, items) => SingleItem(items.FirstOrDefault()));
            yield return new("Last", "Returns the last item", (args, items) => SingleItem(items.LastOrDefault()));
            
            yield return new("Save", "filename", "Writes data to a file", Filters.Save);
            yield return new("Append", "filename", "Appends data to a file", Filters.Append);
            
            yield return new("More", "Writes to the console one screen at a time", Filters.More);
            yield return new("Log", "Writes the output to the application log", Filters.Log);
            yield return new("Ignore", "Ignores the data", Filters.Ignore);
        }

        internal object KeepRunningMonitor{get;} = new object();

        public string CommandLinePrompt{get; protected set;} = "Type exit to exit: ";

        internal void ProcessUserInput(string line)
        {
            if(string.IsNullOrWhiteSpace(line)) return;

            /*
             * User input consists of a command followed by 0 or more filters, seperated by a |
             * For example:
             * 
             * dir *.txt | count
             */

            try
            {
                var parts = PipelineSplitter.Split(line);
                if(parts.Count == 0) return;

                FetchCommandsAndFilters();

                var commandPart = parts[0];
                if(m_Commands.TryGetValue(commandPart.Name, out var command) == false)
                {
                    Console.Error.WriteLine($"no such command: {commandPart.Name}");
                    return;
                }

                // We skip(1) as the first part is the command and the rest are the filters
                var filterPipeline = MakeFilterPipeline(parts.Skip(1));
                var commandOutput = command.Handler(commandPart);
                Execute(filterPipeline, commandOutput);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine();
            }
        }

        /// <summary>
        /// Populates the commands and filters
        /// </summary>
        private void FetchCommandsAndFilters()
        {
            if(m_Commands.Count == 0)
            {
                foreach(var command in GetCommands())
                {
                    m_Commands[command.Name] = command;
                }
            }

            if(m_Filters.Count == 0)
            {
                foreach(var filter in GetFilters())
                {
                    m_Filters[filter.Name] = filter;
                }
            }
        }

        /// <summary>
        /// Executes the pipeline, sending any output to the console
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="input"></param>
        private void Execute(FilterFunction pipeline, IEnumerable<object?> input)
        {
            foreach(var line in pipeline(input))
            {
                Console.WriteLine(line?.ToString() ?? "");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Converts the filter parts into a single function call
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        private FilterFunction MakeFilterPipeline(IEnumerable<PipelinePart> parts)
        {
            FilterFunction function = input => input;

            foreach(var part in parts)
            {
                var filter= MakeFilter(part);
                var first = function;
                function = input => filter(first(input));
            }

            return function;
        }

        private FilterFunction MakeFilter(PipelinePart part)
        {
            if(m_Filters.TryGetValue(part.Name, out var filter))
            {
                return input => filter.Handler(part, input);
            }

            throw new ArrowException($"no such filter: {part.Name}");
        }
    }
}

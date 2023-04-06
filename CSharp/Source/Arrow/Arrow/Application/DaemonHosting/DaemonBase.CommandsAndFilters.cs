using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Arrow.Calendar;
using Arrow.Logging;
using Arrow.Settings;
using Arrow.Text;

namespace Arrow.Application.DaemonHosting
{
    public abstract partial class DaemonBase
    {
        private static string ToLine(object? value)
        {
            return value?.ToString() ?? "";
        }

        private static IEnumerable<object> SkipNulls(IEnumerable<object?> input)
        {
            return input.Where(i => i is not null).Select(i => i!);
        }

        private static IEnumerable<object?> SingleItem<T>(T item)
        {
            yield return item;
        }

        /// <summary>
        /// The default commands
        /// </summary>
        private static class Commands
        {
            private static readonly string[] Motivations =
            {
                "Donald Knuth respects your code",
                "Wow, that's fast",
                "You don't look a day over 21",
                "You don't need Resharper!",
                "I bet you've never needed more that 640K",
                "Nice hair ;-)",
                "Elon Musk would never sack you",
                "Impressive, most impressive",
                "You know where the any-key is"
            };

            public static IEnumerable<object?> Exit(PipelinePart part, DaemonBase daemonBase)
            {
                daemonBase.KeepRunning = false;
                return Enumerable.Empty<object?>();
            }

            public static IEnumerable<object?> Cls(PipelinePart part)
            {
                Console.Clear();
                return Enumerable.Empty<object?>();
            }

            public static IEnumerable<string> Now(PipelinePart part)
            {
                yield return $"Now = {Clock.Now}";
                yield return $"UtcNow = {Clock.UtcNow}";
            }

            public static IEnumerable<string> Echo(PipelinePart part)
            {
                return part.Arguments;
            }

            public static IEnumerable<string> GetSetting(PipelinePart part)
            {
                foreach(var setting in part.Arguments)
                {
                    var value = TokenExpander.ExpandToken(setting);
                    yield return value ?? "";
                }
            }

            public static IEnumerable<string> CommandHelp(PipelinePart part, IReadOnlyDictionary<string, InteractiveCommand> commands)
            {
                var padding = new string(' ', 4);
                var maxWidth = commands.Values.Select(c => c.DisplayName().Length).Max();

                foreach(var command in commands.Values.OrderBy(c => c.Name))
                {
                    var line = string.Format("{0}{1}{2}", 
                                             command.DisplayName().PadRight(maxWidth),
                                             padding,
                                             command.Description);

                    yield return line;
                }
            }

            public static IEnumerable<string> FilterHelp(PipelinePart part, IReadOnlyDictionary<string, InteractiveFilter> filters)
            {
                var padding = new string(' ', 4);
                var maxWidth = filters.Values.Select(c => c.DisplayName().Length).Max();

                foreach(var filter in filters.Values.OrderBy(c => c.Name))
                {
                    var line = string.Format("{0}{1}{2}", 
                                             filter.DisplayName().PadRight(maxWidth),
                                             padding,
                                             filter.Description);

                    yield return line;
                }
            }

            public static IEnumerable<string> Motivate(PipelinePart part)
            {
                var arg = (part.HasArguments ? part.Arguments[0] : "1");
                if(int.TryParse(arg, out var count) == false)
                {
                    yield return "That's not a number. Try and focus!";
                }
                else
                {
                    if(count == 0)
                    {
                        yield return "You seem confused";
                    }
                    else if(count < 0)
                    {
                        yield return "You're looking for negative motivation..?";
                    }
                    else if(count > 1)
                    {
                        yield return "Wow, how much motivation do you really need?";
                    }
                    else
                    {
                        var random = new Random(Guid.NewGuid().GetHashCode());
                        var index = random.Next(Motivations.Length);
                        yield return Motivations[index];
                    }

                }
            }
        }


        /// <summary>
        /// The default filters
        /// </summary>
        private static class Filters
        {
            public static IEnumerable<object?> More(PipelinePart part, IEnumerable<object?> input)
            {
                var lines = input.Select(line => ToLine(line));
                Arrow.Application.More.Render(lines.Concat(BlankLine));

                return Enumerable.Empty<object?>();
            }

            public static IEnumerable<object?> Log(PipelinePart part, IEnumerable<object?> input)
            {
                var log = LogManager.GetDefaultLog();

                foreach(var line in input)
                {
                    log.Info(ToLine(input));
                    yield return line;
                }
            }

            public static IEnumerable<object?> Match(PipelinePart part, IEnumerable<object?> input)
            {
                if(part.HasArguments == false) throw new ArgumentException("no match pattern");

                var pattern = part.Arguments[0].ToLower();
                return SkipNulls(input)
                       .Where(i => ToLine(i).ToLower().Contains(pattern));
            }

            public static IEnumerable<object?> Regex(PipelinePart part, IEnumerable<object?> input)
            {
                if(part.HasArguments == false) throw new ArgumentException("no match pattern");

                var pattern = part.Arguments[0].ToLower();
                return SkipNulls(input)
                       .Where(i => System.Text.RegularExpressions.Regex.IsMatch(ToLine(i), pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
            }

            public static IEnumerable<object?> Top(PipelinePart part, IEnumerable<object?> input)
            {
                if(part.HasArguments == false) throw new ArgumentException("no number of lines specified");

                if(int.TryParse(part.Arguments[0], out var numberOfLines) == false)
                {
                    throw new ArrowException("invalid number of lines");
                }

                return SkipNulls(input).Take(numberOfLines);
            }

            public static IEnumerable<object?> Bottom(PipelinePart part, IEnumerable<object?> input)
            {
                if(part.HasArguments == false) throw new ArgumentException("no number of lines specified");

                if(int.TryParse(part.Arguments[0], out var numberOfLines) == false)
                {
                    throw new ArrowException("invalid number of lines");
                }

                var queue = new Queue<object>();
                foreach(var line in SkipNulls(input))
                {
                    queue.Enqueue(line);
                    if(queue.Count > numberOfLines) queue.Dequeue();
                }

                return queue;
            }

            public static IEnumerable<object?> Save(PipelinePart part, IEnumerable<object?> input)
            {
                if(part.HasArguments == false) throw new ArgumentException("no filename specified");

                var lines = input.Select(i => i?.ToString() ?? "");
                foreach(var filename in part.Arguments)
                {
                    File.WriteAllLines(filename, lines);
                }

                return Enumerable.Empty<object>();
            }

            public static IEnumerable<object?> Append(PipelinePart part, IEnumerable<object?> input)
            {
                if(part.HasArguments == false) throw new ArgumentException("no filename specified");

                var lines = input.Select(i => i?.ToString() ?? "");
                foreach(var filename in part.Arguments)
                {
                    File.AppendAllLines(filename, lines);
                }

                return Enumerable.Empty<object>();
            }

            public static IEnumerable<object?> Ignore(PipelinePart part, IEnumerable<object?> input)
            {
                // We need to enumerate the input so that the filter pipeline executes
                foreach(var _ in input)
                {
                    // Do nothing
                }

                return Enumerable.Empty<object>();
            }
        }
    }
}

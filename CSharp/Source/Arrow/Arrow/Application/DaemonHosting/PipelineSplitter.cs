using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.DaemonHosting
{
    /// <summary>
    /// Converts a line into a series of pipeline parts
    /// </summary>
    public static class PipelineSplitter
    {
        private static readonly IReadOnlyList<PipelinePart> Empty = Enumerable.Empty<PipelinePart>().ToList();

        /// <summary>
        /// Converts a line into a series of pipeline parts
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArrowException"></exception>
        public static IReadOnlyList<PipelinePart> Split(string line)
        {
            if(line is null) throw new ArgumentNullException(nameof(line));

            if(string.IsNullOrWhiteSpace(line)) return Empty;

            var parts = new List<PipelinePart>();

            string? name = null;
            var arguments = new List<string>();

            foreach(var token in PipelineTokenizer.Parse(line))
            {
                if(token.TokenType == PipelineTokenizer.TokenType.None) break;

                if(token.TokenType == PipelineTokenizer.TokenType.Pipe)
                {
                    if(name is null) throw new ArrowException("unexpected pipe");

                    parts.Add(new(name, arguments));
                    name = null;
                    arguments = new List<string>();
                }
                else
                {
                    if(name is null)
                    {
                        if(token.TokenType != PipelineTokenizer.TokenType.String)
                        {
                            throw new ArrowException("name must be a string");
                        }

                        name = token.Value;
                    }
                    else
                    {
                        arguments.Add(token.Value!);
                    }
                }
            }

            if(name is not null)
            {
                parts.Add(new(name, arguments));
            }

            return parts;
        }
    }
}

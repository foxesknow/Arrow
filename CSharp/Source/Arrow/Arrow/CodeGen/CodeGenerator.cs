using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.CodeGen
{
    /// <summary>
    /// Base class for language code generators
    /// </summary>
    public class CodeGenerator
    {
        private readonly StringBuilder m_Builder = new();
        private int m_TabCount = 0;

        /// <summary>
        /// The text to use to indent.
        /// The default is 4 spaces
        /// </summary>
        public string IndentText{get;init; } = "    ";

        /// <summary>
        /// The StringBuilder that holds the generated code
        /// </summary>
        protected StringBuilder Builder
        {
            get{return m_Builder;}
        }

        /// <summary>
        /// Writes a blank line to the generator
        /// </summary>
        /// <returns></returns>
        public void WriteLine()
        {
            var indent = MakeIndentText();
            this.Builder.AppendLine(indent);
        }

        /// <summary>
        /// Writes a line to the generator
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public void WriteLine(string line)
        {
            var indent = MakeIndentText();
            this.Builder.Append(indent).AppendLine(line);
        }

        /// <summary>
        /// Writes a sequence of lines to the generator
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public void WriteLines(IEnumerable<string> lines)
        {
            var indent = MakeIndentText();

            foreach(var line in lines)
            {
                this.Builder.Append(indent).AppendLine(line);
            }
        }

        /// <summary>
        /// Writes a sequence of lines to the generator
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public void WriteLines(params string[] lines)
        {
            WriteLines((IEnumerable<string>)lines);
        }

        /// <summary>
        /// Indents the output by one "tab"
        /// </summary>
        /// <returns></returns>
        public IDisposable Indent()
        {
            return new Indenter(this);
        }

        /// <summary>
        /// Creates the text that will be used to indent the line
        /// </summary>
        /// <returns></returns>
        protected string MakeIndentText()
        {
            var length = this.IndentText.Length * m_TabCount;

            return string.Create(length, this, static(span, state) =>
            {
                var indent = state.IndentText;
                for(int i = 0; i < state.m_TabCount; i++)
                {
                    indent.AsSpan().CopyTo(span.Slice(i * indent.Length, indent.Length));
                }
            });
        }

        /// <summary>
        /// Returns the code generated so far
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_Builder.ToString();
        }

        /// <summary>
        /// Responsible for indenting a "block".
        /// In most languages this is denoted by some sort of open block text (eg { )
        /// and some close block text (eg } )
        /// </summary>
        protected sealed class BlockIndent : IDisposable
        {
            private readonly CodeGenerator m_CodeGenerator;

            private readonly string m_End;

            public BlockIndent(CodeGenerator codeGenerator, string begin, string end)
            {
                m_End = end;

                m_CodeGenerator = codeGenerator;
                m_CodeGenerator.WriteLine(begin);
                m_CodeGenerator.m_TabCount++;
            }

            public void Dispose()
            {
                m_CodeGenerator.m_TabCount--;
                m_CodeGenerator.WriteLine(m_End);
            }
        }

        /// <summary>
        /// Responsible for indenting, but without writing any addition text
        /// </summary>
        protected sealed class Indenter : IDisposable
        {
            private readonly CodeGenerator m_CodeGenerator;

            public Indenter(CodeGenerator codeGenerator)
            {
                m_CodeGenerator = codeGenerator;
                m_CodeGenerator.m_TabCount++;
            }

            public void Dispose()
            {
                m_CodeGenerator.m_TabCount--;
            }
        }
    }
}

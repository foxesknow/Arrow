using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.CodeGen
{
    /// <summary>
    /// Provides C# specific code generation methods
    /// </summary>
    public class CSharpCodeGenerator : CodeGenerator
    {
        /// <summary>
        /// Creates a block using braces to open and close the block
        /// </summary>
        /// <returns></returns>
        public IDisposable Block()
        {
            return new BlockIndent(this, "{", "}");
        }

        /// <summary>
        /// Creates a block that has some sort of overall access statement,
        /// such as an if, while or switch statement and it wrapped with braces
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public IDisposable Block(string line)
        {
            var indent = MakeIndentText();
            this.Builder.Append(indent).AppendLine(line);

            return new BlockIndent(this, "{", "}");
        }

        /// <summary>
        /// Handles a C# switch expression which requires a semi-colon after the closing brace
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public IDisposable SwitchExpression(string line)
        {
            var indent = MakeIndentText();
            this.Builder.Append(indent).AppendLine(line);

            return new BlockIndent(this, "{", "};");
        }
    }
}

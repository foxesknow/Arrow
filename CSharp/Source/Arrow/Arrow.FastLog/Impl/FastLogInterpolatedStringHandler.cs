using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.FastLog.Impl
{
    [InterpolatedStringHandler]
    public ref struct FastLogInterpolatedStringHandler
    {
        private DefaultInterpolatedStringHandler m_Handler;

        public FastLogInterpolatedStringHandler(int literalLength, int formattedCount, IFastLogLevel logger)
        {
            Enabled = logger.Enabled;

            if(Enabled)
            {
                m_Handler = new(literalLength, formattedCount);
            }
            else
            {
                m_Handler = new();
            }
        }

        public bool Enabled{get;}

        public void AppendLiteral(string value) => m_Handler.AppendLiteral(value);

        public void AppendFormatted<T>(T value) => m_Handler.AppendFormatted(value);

        public void AppendFormatted<T>(T value, string? format) => m_Handler.AppendFormatted(value, format);

        public void AppendFormatted<T>(T value, int alignment) => m_Handler.AppendFormatted(value, alignment);

        public void AppendFormatted<T>(T value, int alignment, string? format) => m_Handler.AppendFormatted(value, alignment, format);

        public void AppendFormatted(ReadOnlySpan<char> value) => m_Handler.AppendFormatted(value);

        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null) => m_Handler.AppendFormatted(value, alignment, format);

        public void AppendFormatted(string? value) => m_Handler.AppendFormatted(value);

        public void AppendFormatted(string? value, int alignment = 0, string? format = null) => m_Handler.AppendFormatted(value, alignment, format);

        public void AppendFormatted(object? value, int alignment = 0, string? format = null) => m_Handler.AppendFormatted(value, alignment, format);

        public string ToStringAndClear() => m_Handler.ToStringAndClear();
    }
}

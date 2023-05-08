using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers.InterpolationHandlers
{
    [InterpolatedStringHandler]
    public ref struct LogLevelInterpolatedStringHandler
    {
        private DefaultInterpolatedStringHandler m_Handler;

        public LogLevelInterpolatedStringHandler(int literalLength, int formattedCount, ILog log, LogLevel logLevel, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsEnabled(logLevel);
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount);
            }
            else
            {
                m_Handler = new();
            }
        }

        public LogLevelInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, ILog log, LogLevel logLevel, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsEnabled(logLevel);
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount, provider);
            }
            else
            {
                m_Handler = new();
            }
        }

        public LogLevelInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, Span<char> initialBuffer, ILog log, LogLevel logLevel, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsEnabled(logLevel);
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount, provider, initialBuffer);
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

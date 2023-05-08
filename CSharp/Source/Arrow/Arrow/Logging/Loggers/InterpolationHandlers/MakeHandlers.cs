

using System;
using System.Runtime.CompilerServices;

namespace Arrow.Logging.Loggers.InterpolationHandlers
{

	
    [InterpolatedStringHandler]
    public ref struct DebugLogInterpolatedStringHandler
    {
        private DefaultInterpolatedStringHandler m_Handler;

        public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsDebugEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount);
            }
            else
            {
                m_Handler = new();
            }
        }

        public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsDebugEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount, provider);
            }
            else
            {
                m_Handler = new();
            }
        }

        public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, Span<char> initialBuffer, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsDebugEnabled;
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
	
    [InterpolatedStringHandler]
    public ref struct InfoLogInterpolatedStringHandler
    {
        private DefaultInterpolatedStringHandler m_Handler;

        public InfoLogInterpolatedStringHandler(int literalLength, int formattedCount, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsInfoEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount);
            }
            else
            {
                m_Handler = new();
            }
        }

        public InfoLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsInfoEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount, provider);
            }
            else
            {
                m_Handler = new();
            }
        }

        public InfoLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, Span<char> initialBuffer, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsInfoEnabled;
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
	
    [InterpolatedStringHandler]
    public ref struct WarnLogInterpolatedStringHandler
    {
        private DefaultInterpolatedStringHandler m_Handler;

        public WarnLogInterpolatedStringHandler(int literalLength, int formattedCount, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsWarnEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount);
            }
            else
            {
                m_Handler = new();
            }
        }

        public WarnLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsWarnEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount, provider);
            }
            else
            {
                m_Handler = new();
            }
        }

        public WarnLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, Span<char> initialBuffer, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsWarnEnabled;
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
	
    [InterpolatedStringHandler]
    public ref struct ErrorLogInterpolatedStringHandler
    {
        private DefaultInterpolatedStringHandler m_Handler;

        public ErrorLogInterpolatedStringHandler(int literalLength, int formattedCount, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsErrorEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount);
            }
            else
            {
                m_Handler = new();
            }
        }

        public ErrorLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsErrorEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount, provider);
            }
            else
            {
                m_Handler = new();
            }
        }

        public ErrorLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, Span<char> initialBuffer, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsErrorEnabled;
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
	
    [InterpolatedStringHandler]
    public ref struct FatalLogInterpolatedStringHandler
    {
        private DefaultInterpolatedStringHandler m_Handler;

        public FatalLogInterpolatedStringHandler(int literalLength, int formattedCount, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsFatalEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount);
            }
            else
            {
                m_Handler = new();
            }
        }

        public FatalLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsFatalEnabled;
            if(shouldAppend)
            {
                m_Handler = new(literalLength, formattedCount, provider);
            }
            else
            {
                m_Handler = new();
            }
        }

        public FatalLogInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, Span<char> initialBuffer, ILog log, out bool shouldAppend)
        {
            this.Enabled = shouldAppend = log.IsFatalEnabled;
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
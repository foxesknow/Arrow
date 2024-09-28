using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.FastLog.Impl
{
    public sealed class FastLogLevel : IFastLogLevel
    {
        private const int InitialBufferSize = 64;
        private const int MaxFormatAttemts = 6;

        private static readonly ArrayPool<char> s_LogLinePool = ArrayPool<char>.Create(1024, Environment.ProcessorCount);

        private char[] m_Line = Array.Empty<char>();
        private int m_Offset = 0;
        private int m_StartOfWrite;

        private readonly FastLogger m_Owner;
        private readonly string m_Level;

        public FastLogLevel(FastLogger owner, LogLevel level)
        {
            m_Owner = owner;
            m_Level = $"[{level.ToString().ToUpper()}]";
        }

        public bool Enabled { get; } = true;

        public void Send()
        {
            throw new NotImplementedException();
        }

        public string LineAndClear()
        {
            var line = new string(m_Line, m_StartOfWrite, m_Offset - m_StartOfWrite);

            m_Offset = 0;
            m_StartOfWrite = 0;

            return line;
        }

        public IFastLogLevel Write(string data)
        {
            if(this.Enabled) Append(data);
            return this;
        }

        public IFastLogLevel Write(bool data)
        {
            return Write(data ? "true" : "false");
        }

        public IFastLogLevel Write<T>(T data) where T : ISpanFormattable
        {
            return Write(data, default);
        }

        public IFastLogLevel Write<T>(T data, ReadOnlySpan<char> format) where T : ISpanFormattable
        {
            if(this.Enabled == false) return this;

            if(StackAllocWrite(data, format, InitialBufferSize))
            {
                return this;
            }

            var lengthGuess = InitialBufferSize * 2;
            var formatted = false;

            for(var i = 0; i < MaxFormatAttemts; i++)
            {
                var buffer = s_LogLinePool.Rent(lengthGuess);
                try
                {
                    if(data.TryFormat(buffer, out var charsWritten, format, default))
                    {
                        Span<char> slice = new(buffer, 0, charsWritten);
                        Append(slice);
                        formatted = true;
                        break;
                    }
                    else
                    {
                        lengthGuess = buffer.Length * 2;
                    }
                }
                finally
                {
                    s_LogLinePool.Return(buffer);
                }
            }

            if(formatted == false)
            {
                Append("failed to format data of type");
                Append(typeof(T).Name);
            }

            return this;
        }

        public IFastLogLevel Write([InterpolatedStringHandlerArgument("")] ref FastLogInterpolatedStringHandler handler)
        {
            if(this.Enabled && handler.Enabled)
            {
                var data = handler.ToStringAndClear();
                Write(data);
            }


            return this;
        }

        private bool StackAllocWrite<T>(T data, ReadOnlySpan<char> format, int bufferSize) where T : ISpanFormattable
        {
            // Rather that take memory from the pool, which is shared across threads
            // and therefore locks in some way, we'll try to grab a chunk from the
            // stack on the pretext that most bits of data don't render to huge strings.
            Span<char> buffer = stackalloc char[bufferSize];

            if(data.TryFormat(buffer, out var charsWritten, format, null))
            {
                Append(buffer.Slice(0, charsWritten));
                return true;
            }

            return false;
        }

        private void Append(ReadOnlySpan<char> data)
        {
            if(m_Offset == 0)
            {
                // It's the beginning of a new log line, so write the preable
                WriteBeginLine();
            }

            EnsureSpaceFor(data.Length);
            var target = new Span<char>(m_Line, m_Offset, data.Length);
            data.CopyTo(target);

            m_Offset += data.Length;
        }

        private void EnsureSpaceFor(int length)
        {
            if(m_Offset + length < m_Line.Length) return;

            var newLength = (m_Line.Length + length) * 2;

            var newLine = s_LogLinePool.Rent(newLength);
            Array.Copy(m_Line, newLine, m_Offset);
            s_LogLinePool.Return(m_Line);
            m_Line = newLine;
        }

        private void WriteBeginLine()
        {
            EnsureSpaceFor(64);

            Span<char> buffer = stackalloc char[32];
            
            // The time...
            DateTime.Now.TryFormat(buffer, out var bytesWritten, "yyyyMMdd-HH:mm:ss.fff");
            buffer.Slice(0, bytesWritten).CopyTo(m_Line);
            m_Offset += bytesWritten;

            EnsureSpaceFor(1);
            m_Line[m_Offset++] = ' ';

            // The level...
            EnsureSpaceFor(m_Level.Length);
            m_Level.AsSpan().CopyTo(new Span<char>(m_Line, m_Offset, m_Level.Length));
            m_Offset += m_Level.Length;

            EnsureSpaceFor(1);
            m_Line[m_Offset++] = ' ';

            // Thread info..
            var threadName = Thread.CurrentThread.Name;
            if(threadName is not null)
            {
                EnsureSpaceFor(threadName.Length + 3);
                m_Line[m_Offset++] = '[';
                threadName.AsSpan().CopyTo(new Span<char>(m_Line, m_Offset, threadName.Length));
                m_Offset += threadName.Length;
                m_Line[m_Offset++] = ']';
                m_Line[m_Offset++] = ' ';
            }
            else
            {
                // We'll use the thread id
                Environment.CurrentManagedThreadId.TryFormat(buffer, out bytesWritten);
                EnsureSpaceFor(bytesWritten + 3);
                m_Line[m_Offset++] = '[';
                buffer.Slice(0, bytesWritten).CopyTo(new Span<char>(m_Line, m_Offset, bytesWritten));
                m_Offset += bytesWritten;
                m_Line[m_Offset++] = ']';
                m_Line[m_Offset++] = ' ';
            }

            m_StartOfWrite = m_Offset;
        }
    }
}

﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.FastLog
{
    public sealed class FastLogger : IFastLogger
    {
        private const int InitialBufferSize = 64;
        private const int MaxFormatAttemts = 6;

        private static readonly ArrayPool<char> s_LogLinePool = ArrayPool<char>.Create(1024, Environment.ProcessorCount);

        private char[] m_Line = Array.Empty<char>();
        private int m_Offset = 0;

        public bool Enabled{get;} = true;

        public void Send()
        {
            throw new NotImplementedException();
        }

        public string LineSoFar()
        {
            return new string(m_Line, 0, m_Offset);
        }

        public IFastLogger Write(string data)
        {
            Append(data);
            return this;
        }

        public IFastLogger Write(bool data)
        {
            return Write(data ? "true" : "false");
        }

        public IFastLogger Write<T>(T data) where T : ISpanFormattable
        {
            return Write(data, default);
        }

        public IFastLogger Write<T>(T data, ReadOnlySpan<char> format) where T : ISpanFormattable
        {
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

        public IFastLogger Write(ref DefaultInterpolatedStringHandler handler)
        {
            var data = handler.ToStringAndClear();
            return Write(data);
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
    }
}

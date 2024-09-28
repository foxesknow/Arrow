using System.Runtime.CompilerServices;
using Arrow.FastLog.Impl;

namespace Arrow.FastLog
{
    public interface IFastLogLevel
    {
        public bool Enabled{get;}

        public IFastLogLevel Write(string data);
        public IFastLogLevel Write<T>(T data) where T : ISpanFormattable;
        public IFastLogLevel Write<T>(T data, ReadOnlySpan<char> format) where T : ISpanFormattable;
        public IFastLogLevel Write(bool data);

        public IFastLogLevel Write([InterpolatedStringHandlerArgument("")] ref FastLogInterpolatedStringHandler handler);

        public void Send();
        
    }
}

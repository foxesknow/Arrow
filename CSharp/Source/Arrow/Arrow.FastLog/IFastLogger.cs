using System.Runtime.CompilerServices;

namespace Arrow.FastLog
{
    public interface IFastLogger
    {
        public bool Enabled{get;}

        public IFastLogger Write(string data);
        public IFastLogger Write<T>(T data) where T : ISpanFormattable;
        public IFastLogger Write<T>(T data, ReadOnlySpan<char> format) where T : ISpanFormattable;
        public IFastLogger Write(bool data);

        public IFastLogger Write(ref DefaultInterpolatedStringHandler handler);

        public void Send();
        
    }
}

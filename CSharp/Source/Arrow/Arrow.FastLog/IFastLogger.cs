namespace Arrow.FastLog
{
    public interface IFastLogger
    {
        public IFastLogger Write(string data);
        public IFastLogger Write<T>(T data) where T : ISpanFormattable;
        public IFastLogger Write<T>(T data, ReadOnlySpan<char> format) where T : ISpanFormattable;
        public IFastLogger Write(bool data);

        public void Send();
        
    }
}

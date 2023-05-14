using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.IO;
using Arrow.Numeric;

using BenchmarkDotNet.Attributes;

namespace BenchmarkArrow.IO
{
    [MemoryDiagnoser]   
    public class ArrayPoolMemoryStreamMemory
    {
        private byte[] m_Buffer = default!;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            var buffer = new List<byte>();
            foreach(var prime in PrimeSequence.First1000)
            {
                var b = BitConverter.GetBytes(prime);
                buffer.AddRange(b);
            }

            m_Buffer = buffer.ToArray();

        }

        [Params(1, 5, 10)]
        public int N{get; set;}

        [Benchmark]
        public void ArrayPoolStream()
        {
            using(var stream = new ArrayPoolMemoryStream())
            {
                FillStream(stream);
            }
        }

        [Benchmark(Baseline = true)]
        public void DefaultMemoryStream()
        {
            using(var stream = new MemoryStream())
            {
                FillStream(stream);
            }
        }

        private void FillStream(Stream stream)
        {
            for(int i = 0; i < this.N; i++)
            {
                stream.Write(m_Buffer, 0, m_Buffer.Length);
            }
        }
    }
}

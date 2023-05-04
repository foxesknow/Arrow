using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Sources
{
    [Source("RandomNumbers")]
    public sealed class RandomNumbersSource : Source
    {
        public override async IAsyncEnumerable<object> Run()
        {
            await ForceAsync();

            var generator = MakeRandomGenerator();
            var numbersToGenerate = this.Count;
            var infinite = (numbersToGenerate == 0);

            for(int i = 0; i < numbersToGenerate || infinite; i++)
            {
                var number = generator();
                yield return number;
            }
        }

        private Func<object> MakeRandomGenerator()
        {
            var random = new Random(this.Seed);

            if(this.YieldDoubles)
            {
                return () => random.NextDouble();
            }

            if(this.MinValue is int minValue && this.MaxValue is int maxValue)
            {
                return () => random.Next(minValue, maxValue);
            }
            else if(this.MaxValue is not null)
            {
                maxValue = this.MaxValue.Value;
                return () => random.Next(maxValue);
            }

            return () => random.Next();
        }

        /// <summary>
        /// How many random numbers to generate.
        /// If zero then an infinite number will be generated
        /// </summary>
        public int Count{get; set;}

        /// <summary>
        /// The seed for the random number.
        /// If not set then one will be generated
        /// </summary>
        public int Seed{get; set;} = Guid.NewGuid().GetHashCode();

        /// <summary>
        /// The minimum value to return
        /// </summary>
        public int? MinValue{get; set;}
        
        /// <summary>
        /// The maximum value to return
        /// </summary>
        public int? MaxValue{get; set;}

        /// <summary>
        /// True to yield doubles, rather than ints.
        /// If set then MinValue and MaxValue are ignored
        /// </summary>
        public bool YieldDoubles{get; set;}
    }
}

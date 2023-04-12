using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Arrow.AlertableData
{
    /// <summary>
    /// We'll model trading market data
    /// </summary>
    class MarketData
    {
        public decimal BidSize{get; set;}
        public decimal BidPrice{get; set;}

        public decimal AskSize{get; set;}
        public decimal AskPrice{get; set;}
    }
}

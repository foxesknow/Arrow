using Arrow.InsideOut.Transport;
using Arrow.InsideOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Arrow.InsideOut.Transport
{
    public abstract class TransportTestsBase : TestBase
    {
        protected readonly RequestID m_RequestID = new(Guid.NewGuid(), Guid.NewGuid());
        protected readonly PublisherID m_PublisherID = new("Foo", "Bar");

        protected readonly BoolParameter P_Bool = new("a bool"){DefaultValue = true};
        protected readonly Int32Parameter P_Int32 = new("an int32"){DefaultValue = 10};
        protected readonly Int64Parameter P_Int64 = new("an int64"){DefaultValue = 20};
        protected readonly DoubleParameter P_Double = new("a double"){DefaultValue = 40};
        protected readonly DecimalParameter P_Decimal = new("a decimal"){DefaultValue = 80m};
        protected readonly TimeSpanParameter P_TimeSpan = new("a timespan"){DefaultValue = new TimeSpan(10, 20, 30, 40)};
        protected readonly DateTimeParameter P_DateTime= new("a datetime"){DefaultValue = DateTime.Now};
        protected readonly StringParameter P_String = new("a string"){DefaultValue = "hello, world"};
        protected readonly SuggestionParameter P_Suggestion = new("a suggestion")
        {
            Suggestions = {"X", "Y", "Z"},
            DefaultValue = "Y"
        };

        protected readonly SingleItemParameter P_SingleItem = new("a single item")
        {
            Items = {new("X", 1), new("Y", 2), new ("Z", 4)},
        };

        protected readonly MultipleItemsParameter P_MultiItems = new("a multi items")
        {
            Items = {new("X", 1), new("Y", 2), new ("Z", 4)},
        };

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.AlertableData;

using NUnit.Framework;

namespace UnitTests.Arrow.AlertableData
{
    [TestFixture]
    public class NonRecursiveLockTests
    {
        [Test]
        [TestCase(LockMode.ReadWrite)]
        public void RecursiveReadFails(LockMode lockMode)
        {
            using(var alertableData = Make<string, MarketData>(lockMode))
            {
                alertableData.Subscribe("VOD.L", new MarketData());

                using(alertableData.Read("VOD.L", out var readResult))
                {
                    Assert.That(readResult.Data, Is.Not.Null);

                    Assert.Catch(() =>
                    {
                        using(alertableData.Read("VOD.L", out var _))
                        {
                            Assert.Fail("we shouldn't be here!");
                        }
                    });
                }
            }
        }

        [Test]
        [TestCase(LockMode.ReadWrite)]
        [TestCase(LockMode.BucketReadWrite)]
        public void RecursiveTryReadFails(LockMode lockMode)
        {
            using(var alertableData = Make<string, MarketData>(lockMode))
            {
                alertableData.Subscribe("VOD.L", new MarketData());

                var result = alertableData.TryRead("VOD.L", NoState.Data, (key, state, data) =>
                {
                    Assert.That(data, Is.Not.Null);

                    Assert.Catch(() =>
                    {
                        alertableData.TryRead("VOD.L", NoState.Data, (key, state, data) =>
                        {
                            Assert.That(data, Is.Not.Null);
                            return (data.BidSize, data.AskSize);
                        });
                    });

                    return (data.BidSize, data.AskSize);
                });
            }
        }

        private OnDemandAlertableData<TKey, TData> Make<TKey, TData>(LockMode lockMode) where TData : class
        {
            ILockPolicy<TKey> lockPolicy = LockPolicyFactory.Make<TKey>(lockMode);
            return new(lockPolicy);
        }
    }
}

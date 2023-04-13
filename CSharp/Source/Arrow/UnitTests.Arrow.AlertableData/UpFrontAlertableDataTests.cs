﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.AlertableData;

using NUnit.Framework;

namespace UnitTests.Arrow.AlertableData
{
    [TestFixture(LockMode.Monitor)]
    [TestFixture(LockMode.BucketReadWrite)]
    [TestFixture(LockMode.ReadWrite)]
    public class UpFrontAlertableDataTests
    {
        private const int SymbolCount = 200;

        private readonly LockMode m_LockMode;

        public UpFrontAlertableDataTests(LockMode lockMode)
        {
            m_LockMode = lockMode;
        }

        [Test]
        public void Subscribe()
        {
            using(var data = MakeAlertableData<string, MarketData>("VOD.L"))
            {
                Assert.That(data.IsSubscribed("VOD.L"), Is.True);
                Assert.That(data.IsSubscribed("vod.l"), Is.False);
                Assert.That(data.IsSubscribed("BT.L"), Is.False);
            }
        }

        [Test]
        public void Publish_NoSuchSymbol()
        {
            using(var data = MakeAlertableData<string, MarketData>())
            {
                var published = data.Publish("VOD.L", NoState.Data, (key, state, data) =>
                {
                    Assert.Fail("the publish lambda should not be called");
                    return data;
                });

                Assert.That(published, Is.False);
            }
        }

        [Test]
        public void Publish()
        {
            using(var data = MakeAlertableDataWithoutInitialState<string, MarketData>("VOD.L"))
            {
                var state = (BidSize: 20m, BidPrice: 100.5m, AskSize: 30m, AskPrice: 102m);

                var published = data.Publish("VOD.L", state, (key, state, current) =>
                {
                    Assert.That(current, Is.Null);

                    current = new MarketData()
                    {
                        BidSize = state.BidSize,
                        BidPrice = state.BidPrice,
                        AskSize = state.AskSize,
                        AskPrice = state.AskPrice
                    };

                    return current;
                });

                Assert.That(published, Is.True);

                var result = data.TryRead("VOD.L", NoState.Data, (key, state, current) =>
                {
                    Assert.That(current, Is.Not.Null);
                    return (current.BidSize, current.BidPrice);
                });

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Data.BidSize, Is.EqualTo(20m));
                Assert.That(result.Data.BidPrice, Is.EqualTo(100.5m));
            }
        }

        [Test]
        public void Read()
        {
            using(var data = MakeAlertableDataWithoutInitialState<string, MarketData>("VOD.L"))
            {
                var state = (BidSize: 20m, BidPrice: 100.5m, AskSize: 30m, AskPrice: 102m);

                var published = data.Publish("VOD.L", state, (key, state, current) =>
                {
                    Assert.That(current, Is.Null);

                    current = new MarketData()
                    {
                        BidSize = state.BidSize,
                        BidPrice = state.BidPrice,
                        AskSize = state.AskSize,
                        AskPrice = state.AskPrice
                    };

                    return current;
                });

                Assert.That(published, Is.True);

                using(data.Read("VOD.L", out var readResult))
                {
                    Assert.That(readResult.IsSubscribed, Is.True);
                    Assert.That(readResult.Data, Is.Not.Null);
                    Assert.That(readResult.Data.BidSize, Is.EqualTo(20m));
                    Assert.That(readResult.Data.BidPrice, Is.EqualTo(100.5m));
                }

                using(data.Read("BT.L", out var readResult))
                {
                    Assert.That(readResult.IsSubscribed, Is.False);
                    Assert.That(readResult.Data, Is.Null);
                }
            }
        }

        [Test]
        public void TryReadByRef()
        {
            using(var data = MakeAlertableDataWithoutInitialState<string, MarketData>("VOD.L"))
            {
                var state = (BidSize: 20m, BidPrice: 100.5m, AskSize: 30m, AskPrice: 102m);

                var published = data.Publish("VOD.L", state, (key, state, current) =>
                {
                    return  new MarketData()
                    {
                        BidSize = state.BidSize,
                        BidPrice = state.BidPrice,
                        AskSize = state.AskSize,
                        AskPrice = state.AskPrice
                    };
                });

                Assert.That(published, Is.True);

                decimal bidSize = 0;

                var succeeded = data.TryReadByRef("VOD.L", NoState.Data, ref bidSize, (string key, NoState state, MarketData current, ref decimal result) =>
                {
                    Assert.That(current, Is.Not.Null);
                    result = current.BidSize;
                });

                Assert.That(succeeded, Is.True);
                Assert.That(bidSize, Is.EqualTo(20m));
            }
        }

        [Test]
        public void TryReadByRef_NoSubscription()
        {
            using(var data = MakeAlertableDataWithoutInitialState<string, MarketData>("VOD.L"))
            {
                decimal bidSize = -1;

                var succeeded = data.TryReadByRef("VOD.L", NoState.Data, ref bidSize, (string key, NoState state, MarketData current, ref decimal result) =>
                {
                    Assert.That(current, Is.Not.Null);
                    result = current.BidSize;
                });

                Assert.That(succeeded, Is.False);
                Assert.That(bidSize, Is.EqualTo(-1m));
            }
        }

        [Test]
        public void TryRead_NoSuchSymbol()
        {
            using(var data = MakeAlertableData<string, MarketData>())
            {
                var result = data.TryRead("VOD.L", NoState.Data, (key, state, current) =>
                {
                    Assert.Fail("the publish lambda should not be called");
                    return (current.BidSize, current.BidPrice, current.AskSize, current.AskPrice);
                });

                Assert.That(result.Succeeded, Is.False);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        public void Blast(int repetitions)
        {
            var symbols = Enumerable.Range(0, SymbolCount)
                                    .Select(i => $"VOD.L.{i}")
                                    .ToArray();

            using(var data = MakeAlertableData<string, MarketData>(symbols))
            {
                for(var i = 0; i < repetitions; i++)
                {
                    var symbol = $"VOD.L.{i % SymbolCount}";
                    var state = (BidSize: 20m, BidPrice: 100.5m, AskSize: (decimal)i, AskPrice: 102m);

                    data.Publish(symbol, state, static (key, state, current) =>
                    {
                        current.BidSize = state.BidSize;
                        current.BidPrice = state.BidPrice;
                        current.AskSize = state.AskSize;
                        current.AskPrice = state.AskPrice;

                        return current;
                    });

                    var bidSize = 0m;
                    var bidPrice = 0m;
                    var askSize = 0m;

                    using(data.Read(symbol, out var readResult))
                    {
                        bidSize = readResult.Data.BidSize;
                        bidPrice = readResult.Data.BidPrice;
                        askSize = readResult.Data.AskSize;
                    }

                    Assert.That(bidSize, Is.EqualTo(20m));
                    Assert.That(bidPrice, Is.EqualTo(100.5m));
                    Assert.That(askSize, Is.EqualTo((decimal)i));
                }
            }
        }

        private UpFrontAlertableData<TKey, TData> MakeAlertableData<TKey, TData>(params TKey[] symbols) where TData : class, new()
        {
            var subscriptions = symbols.Select(symbol => (symbol, new TData()));
            var lockPolicy = LockPolicyFactory.Make<TKey>(m_LockMode);

            return new(lockPolicy, subscriptions);
        }

        private UpFrontAlertableData<TKey, TData> MakeAlertableDataWithoutInitialState<TKey, TData>(params TKey[] symbols) where TData : class, new()
        {
            var subscriptions = symbols.Select(symbol => (symbol, (TData)null));
            var lockPolicy = LockPolicyFactory.Make<TKey>(m_LockMode);

            return new(lockPolicy, subscriptions);
        }
    }
}

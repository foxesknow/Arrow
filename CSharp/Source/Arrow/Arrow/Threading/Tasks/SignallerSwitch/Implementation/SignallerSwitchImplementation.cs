using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks.SignallerSwitch.Implementation
{
    public static class SignallerSwitchImplementation
    {
        /// <summary>
        /// Data passed to a case condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TState"></typeparam>
        public readonly struct CaseData<T, TState>
        {
            public CaseData(T value, TState state, CancellationToken cancellationToken)
            {
                Value = value;
                State = state;
                CancellationToken = cancellationToken;
            }

            public T Value{get;}

            public TState State{get;}

            public CancellationToken CancellationToken{get;}
        }

        public readonly struct InitialCase<T, TState>
        {
            private readonly Signaller<T> m_Signaller;
            private readonly TState m_State;
            private readonly Func<TState, Task> m_Function;

            internal InitialCase(Signaller<T> signaller, TState state, Func<TState, Task> function)
            {
                m_Signaller = signaller;
                m_State = state;
                m_Function = function;
            }

            /// <summary>
            /// Creates a case statement where the "then" function returns a value
            /// </summary>
            /// <typeparam name="TResult"></typeparam>
            /// <param name="if"></param>
            /// <param name="then"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public ExecuteAndSwithDataWithResult<T, TState, TResult> Case<TResult>(Func<CaseData<T, TState>, bool> @if, Func<CaseData<T, TState>, Task<TResult>> then)
            {
                if(@if is null) throw new ArgumentNullException(nameof(@if));
                if(then is null) throw new ArgumentNullException(nameof(then));

                var withResult = new ExecuteAndSwithDataWithResult<T, TState, TResult>(m_Signaller, m_State, m_Function);
                withResult.Case(@if, then);

                return withResult;
            }

            /// <summary>
            /// Creates a case statement where the "then" function returns a value
            /// </summary>
            /// <typeparam name="TResult"></typeparam>
            /// <param name="if"></param>
            /// <param name="then"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public ExecuteAndSwithDataWithResult<T, TState, TResult> Case<TResult>(Func<CaseData<T, TState>, bool> @if, Func<CaseData<T, TState>, TResult> then)
            {
                if(@if is null) throw new ArgumentNullException(nameof(@if));
                if(then is null) throw new ArgumentNullException(nameof(then));

                var withResult = new ExecuteAndSwithDataWithResult<T, TState, TResult>(m_Signaller, m_State, m_Function);
                withResult.Case(@if, then);

                return withResult;
            }

            /// <summary>
            /// Creates a case where the when function does not return a value
            /// </summary>
            /// <param name="if"></param>
            /// <param name="then"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public ExecuteAndSwithDataNoResult<T, TState> Case(Func<CaseData<T, TState>, bool> @if, Func<CaseData<T, TState>, Task> then)
            {
                if(@if is null) throw new ArgumentNullException(nameof(@if));
                if(then is null) throw new ArgumentNullException(nameof(then));

                var withoutResult = new ExecuteAndSwithDataNoResult<T, TState>(m_Signaller, m_State, m_Function);
                withoutResult.Case(@if, then);

                return withoutResult;
            }

            /// <summary>
            /// Creates a case where the when function does not return a value
            /// </summary>
            /// <param name="if"></param>
            /// <param name="then"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public ExecuteAndSwithDataNoResult<T, TState> Case(Func<CaseData<T, TState>, bool> @if, Action<CaseData<T, TState>> then)
            {
                if(@if is null) throw new ArgumentNullException(nameof(@if));
                if(then is null) throw new ArgumentNullException(nameof(then));

                var withoutResult = new ExecuteAndSwithDataNoResult<T, TState>(m_Signaller, m_State, m_Function);
                withoutResult.Case(@if, then);

                return withoutResult;
            }
        }



        public abstract class ExecuteAndSwitchDataBase<T, TState, TResult>
        {
            private readonly Func<TState, Task> m_Function;
            private TimeSpan? m_Timeout;

            private protected ExecuteAndSwitchDataBase(Signaller<T> signaller, TState state, Func<TState, Task> function)
            {
                Signaller = signaller;
                State = state;
                m_Function = function;
            }

            /// <summary>
            /// Specified a timeout for the switch which applies to the case conditions
            /// </summary>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public ExecuteAndSwitchDataBase<T, TState, TResult> TimeoutAfter(TimeSpan timeout)
            {
                m_Timeout = timeout;
                return this;
            }

            /// <summary>
            /// Executes the switch statement
            /// </summary>
            /// <returns></returns>
            public Task<(TResult Result, T SignalledData)> Execute()
            {
                return DoExecute();
            }

            /// <summary>
            /// Sets the cancellation token that will be checked to see if the switch should cancel.
            /// NOTE: The token is only checked prior to the initial function being executed, after the 
            /// initial function executes and when evaluating each case condition.
            /// This means that data must pass through the signaller in order for the cancellation token to be checked
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public ExecuteAndSwitchDataBase<T, TState, TResult> CancelWhen(CancellationToken cancellationToken)
            {
                CancellationToken = cancellationToken;
                return this;
            }

            private protected List<Func<CaseData<T, TState>, bool>> If { get; } = new();

            private protected TState State { get; }

            private protected CancellationToken CancellationToken { get; private set; }

            private protected Signaller<T> Signaller { get; }

            /// <summary>
            /// Executes the switch statement
            /// </summary>
            /// <returns></returns>
            private protected abstract Task<(TResult Result, T SignalledData)> DoExecute();

            /// <summary>
            /// Executes the entire switch statement
            /// </summary>
            /// <returns></returns>
            private protected async Task<(T SignalledData, int Index)> ExecuteSwitch()
            {
                var index = -1;

                // We need to collapse the conditions into one giant condition
                Func<T, bool> macroWhen = data =>
                {
                    var ct = CancellationToken;
                    var conditions = If;
                    var count = conditions.Count;
                    var caseData = new CaseData<T, TState>(data, State, ct);

                    for(var i = 0; i < count; i++)
                    {
                        ct.ThrowIfCancellationRequested();

                        if (conditions[i](caseData))
                        {
                            // We've got a match.
                            // However, there may be multiple threads signalling data, so we need to
                            // ensure that we only record the first one to flag the condition

                            return Interlocked.CompareExchange(ref index, i, -1) == -1;
                        }
                    }

                    // Nothing matched
                    return false;
                };

                var whenToken = Signaller.ImplementationWhen(macroWhen);

                try
                {
                    if(m_Timeout is null)
                    {
                        var signalledData = await CallAndWait(this, whenToken).ContinueOnAnyContext();
                        return (signalledData, Interlocked.CompareExchange(ref index, 0, 0));
                    }
                    else
                    {
                        var signalledData = await CallAndWait(this, whenToken)
                                                  .TimeoutAfter(m_Timeout.Value)
                                                  .ContinueOnAnyContext();

                        return (signalledData, Interlocked.CompareExchange(ref index, 0, 0));
                    }
                }
                finally
                {
                    Signaller.Remove(whenToken);
                }

                static async Task<T> CallAndWait(ExecuteAndSwitchDataBase<T, TState, TResult> self, Signaller<T>.WhenToken whenToken)
                {
                    self.CancellationToken.ThrowIfCancellationRequested();

                    await self.m_Function(self.State).ContinueOnAnyContext();
                    self.CancellationToken.ThrowIfCancellationRequested();

                    return await whenToken.Task.ContinueOnAnyContext();
                }
            }
        }

        /// <summary>
        /// Models a case statement that return a result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public sealed class ExecuteAndSwithDataWithResult<T, TState, TResult> : ExecuteAndSwitchDataBase<T, TState, TResult>
        {
            private readonly List<Func<CaseData<T, TState>, Task<TResult>>> m_Then = new();

            internal ExecuteAndSwithDataWithResult(Signaller<T> signaller, TState state, Func<TState, Task> function) : base(signaller, state, function)
            {
            }

            /// <summary>
            /// Adds a case statement
            /// </summary>
            /// <param name="if"></param>
            /// <param name="then"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public ExecuteAndSwithDataWithResult<T, TState, TResult> Case(Func<CaseData<T, TState>, bool> @if, Func<CaseData<T, TState>, Task<TResult>> then)
            {
                if(@if is null) throw new ArgumentNullException(nameof(@if));
                if(then is null) throw new ArgumentNullException(nameof(then));

                If.Add(@if);
                m_Then.Add(then);

                return this;
            }

            /// <summary>
            /// Adds a case statement
            /// </summary>
            /// <param name="if"></param>
            /// <param name="then"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public ExecuteAndSwithDataWithResult<T, TState, TResult> Case(Func<CaseData<T, TState>, bool> @if, Func<CaseData<T, TState>, TResult> then)
            {
                if(@if is null) throw new ArgumentNullException(nameof(@if));
                if(then is null) throw new ArgumentNullException(nameof(then));

                Func<CaseData<T, TState>, Task<TResult>> asyncThen = c =>
                {
                    var result = then(c);
                    return Task.FromResult(result);
                };

                If.Add(@if);
                m_Then.Add(asyncThen);

                return this;
            }

            private protected override async Task<(TResult Result, T SignalledData)> DoExecute()
            {
                var (signalledData, index) = await ExecuteSwitch().ContinueOnAnyContext();
                if (index != -1)
                {
                    var caseData = new CaseData<T, TState>(signalledData, State, CancellationToken);
                    var result = await m_Then[index](caseData).ContinueOnAnyContext();

                    return (result, signalledData);
                }

                // We shouldn't get here...
                throw new ArrowException("no condition matched");
            }
        }

        /// <summary>
        /// Models a case statement that return does not result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public sealed class ExecuteAndSwithDataNoResult<T, TState> : ExecuteAndSwitchDataBase<T, TState, Unit>
        {
            private readonly List<Func<CaseData<T, TState>, Task>> m_Then = new();

            internal ExecuteAndSwithDataNoResult(Signaller<T> signaller, TState state, Func<TState, Task> function) : base(signaller, state, function)
            {
            }

            /// <summary>
            /// Adds a case statement
            /// </summary>
            /// <param name="if"></param>
            /// <param name="then"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public ExecuteAndSwithDataNoResult<T, TState> Case(Func<CaseData<T, TState>, bool> @if, Func<CaseData<T, TState>, Task> then)
            {
                if(@if is null) throw new ArgumentNullException(nameof(@if));
                if(then is null) throw new ArgumentNullException(nameof(then));

                If.Add(@if);
                m_Then.Add(then);

                return this;
            }

            /// <summary>
            /// Adds a case statement
            /// </summary>
            /// <param name="if"></param>
            /// <param name="then"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public ExecuteAndSwithDataNoResult<T, TState> Case(Func<CaseData<T, TState>, bool> @if, Action<CaseData<T, TState>> then)
            {
                if(@if is null) throw new ArgumentNullException(nameof(@if));
                if(then is null) throw new ArgumentNullException(nameof(then));

                Func<CaseData<T, TState>, Task> asyncThen = c =>
                {
                    then(c);
                    return Task.CompletedTask;
                };

                If.Add(@if);
                m_Then.Add(asyncThen);

                return this;
            }

            private protected override async Task<(Unit Result, T SignalledData)> DoExecute()
            {
                var (signalledData, index) = await ExecuteSwitch().ContinueOnAnyContext();
                if(index != -1)
                {
                    var caseData = new CaseData<T, TState>(signalledData, State, CancellationToken);
                    await m_Then[index](caseData).ContinueOnAnyContext();

                    return (Unit.Default, signalledData);
                }

                // We shouldn't get here...
                throw new ArrowException("no condition matched");
            }
        }
    }
}

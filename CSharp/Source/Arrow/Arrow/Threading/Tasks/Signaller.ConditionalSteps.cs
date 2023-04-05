using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public sealed partial class Signaller<T>
    {
        /// <summary>
        /// Collapses multiple conditions into a single condition.
        /// When each condition is satisfied we move onto the next condition.
        /// This repeats until all conditions have been satisfied. Any subsequent calls will throw an exception.
        /// The piece of data used to make the final condition true is the one returned as the signal data.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Func<T, bool> MakeConditionalSteps(Func<T, bool> first, params Func<T, bool>[] rest)
        {
            return DoMakeConditionalSteps(first, (IReadOnlyList<Func<T, bool>>)rest);
        }

        /// <summary>
        /// Collapses multiple conditions into a single condition.
        /// When each condition is satisfied we move onto the next condition.
        /// This repeats until all conditions have been satisfied. Any subsequent calls will throw an exception.
        /// The piece of data used to make the final condition true is the one returned as the signal data.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Func<T, bool> MakeConditionalSteps(Func<T, bool> first, IReadOnlyList<Func<T, bool>> rest)
        {
            if(first is null) throw new ArgumentNullException(nameof(first));
            if(rest is null) throw new ArgumentNullException(nameof(rest));
            if(rest.Any(f => f is null)) throw new ArgumentException("null condition in rest sequence", nameof(rest));

            return DoMakeConditionalSteps(first, rest.ToArray());
        }

        private static Func<T, bool> DoMakeConditionalSteps<T>(Func<T, bool> first, IReadOnlyList<Func<T, bool>> rest)
        {
            Func<T, bool>? activeFunction = first;
            var nextIndex = 0;
            var syncRoot = new object();

            return Evaluate;

            bool Evaluate(T value)
            {
                lock(syncRoot)
                {
                    // This should never happen
                    if(activeFunction is null) throw new ArrowException("unexpected call");

                    var success = activeFunction(value);
                    if(success)
                    {
                        if(nextIndex < rest.Count)
                        {
                            // We've got more functions to go
                            activeFunction = rest[nextIndex];
                            nextIndex++;

                            // We'll return false to tell the signaller to keep calling us
                            success = false;

                        }
                        else
                        {
                            // We're done
                            activeFunction = null;
                        }
                    }

                    return success;
                }
            }
        }
    }
}

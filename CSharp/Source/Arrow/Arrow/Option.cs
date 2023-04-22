using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Arrow
{
    /// <summary>
    /// Represents an optional value.
    /// NOTE: null is not treated as None, it is a valid value in many scenarios
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public readonly struct Option<T> : IEquatable<Option<T>>, IOption
    {
        private readonly T m_Value;

        /// <summary>
        /// Initializes the instance to some value
        /// </summary>
        /// <param name="value"></param>
        public Option(T value)
        {
            m_Value = value;
            IsSome = true;
        }

        /// <summary>
        /// Initializes the instance to none
        /// </summary>
        /// <param name="none"></param>
        public Option(None none)
        {
            m_Value = default!;
            IsSome = false;
        }

        /// <summary>
        /// True if the option has some value, otherwise false
        /// </summary>
        public bool IsSome{get;}

        /// <summary>
        /// True if the option is none, otherwise false
        /// </summary>
        public bool IsNone
        {
            get{return !IsSome;}
        }

        /// <summary>
        /// Returns the value in the option if set, otherwise throws an exception
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The option is none</exception>
        public T Value()
        {
            if(IsSome) return m_Value;
            throw new InvalidOperationException("value not set");
        }

        /// <summary>
        /// Implements a match statement, calling the appropriate function
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="some">Called if the option is some</param>
        /// <param name="none">Called if the option in none</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">One of the functions was none</exception>
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            if(some is null) throw new ArgumentNullException(nameof(some));
            if(none is null) throw new ArgumentNullException(nameof(none));

            if(IsSome)
            {
                return some(m_Value);
            }
            else
            {
                return none();
            }
        }

        /// <summary>
        /// Implements a match statement, calling the appropriate function
        /// This implementation takes state data to allow callers to avoid memory allocations
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="state">State data to pass to the functions</param>
        /// <param name="some">Called if the option is some</param>
        /// <param name="none">Called if the option in none</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">One of the functions was none</exception>
        public TResult Match<TState, TResult>(TState state, Func<T, TState, TResult> some, Func<TState, TResult> none)
        {
            if(some is null) throw new ArgumentNullException(nameof(some));
            if(none is null) throw new ArgumentNullException(nameof(none));

            if(IsSome)
            {
                return some(m_Value, state);
            }
            else
            {
                return none(state);
            }
        }

        /// <summary>
        /// Bind is for working with functions that return an option
        /// (This is the equivilant of Map)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Option<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if(selector is null) throw new ArgumentNullException(nameof(selector));

            if(IsSome)
            {
                return new(selector(m_Value));
            }

            return new Option<TResult>();
        }

        /// <summary>
        /// Calls a selector if some, otherwise returns none
        /// This implementation takes state data to allow callers to avoid memory allocations
        /// (This is the equivilant of Map)
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="state">State data to pass to the selector</param>
        /// <param name="selector">The function to call with the data in the option</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">selector is null</exception>
        public Option<TResult> Select<TState, TResult>(TState state, Func<T, TState, TResult> selector)
        {
            if(selector is null) throw new ArgumentNullException(nameof(selector));

            if(IsSome)
            {
                return new(selector(m_Value, state));
            }

            return new Option<TResult>();
        }

        /// <summary>
        /// Implements SelectMany
        /// (This is the equivilant of Bind)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="binder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">binder is null</exception>
        public Option<TResult> Bind<TResult>(Func<T, Option<TResult>> binder)
        {
            if(binder is null) throw new ArgumentNullException(nameof(binder));

            if(IsNone) return default;

            return binder(m_Value);
        }

        /// <summary>
        /// This implementation takes state data to allow callers to avoid memory allocations
        /// (This is the equivilant of Bind)
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="state">The state data to pass to the binder</param>
        /// <param name="binder">The function to call with the data in the binder</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">binder is null</exception>
        public Option<TResult> Bind<TState, TResult>(TState state, Func<T, TState, Option<TResult>> binder)
        {
            if(binder is null) throw new ArgumentNullException(nameof(binder));

            if(IsNone) return default;

            return binder(m_Value, state);
        }        

        /// <summary>
        /// Attempts to get the value from the option
        /// </summary>
        /// <param name="value">Set to the data in the option on success</param>
        /// <returns>true if there is some data in the option, otherwise false</returns>
        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            if(IsSome)
            {
                value = m_Value;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Returns the contained value if some, otherwise the default value
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T ValueOr(T defaultValue)
        {
            return IsSome ? m_Value : defaultValue;
        }

        /// <summary>
        /// Returns the contained value if some, otherwise calls the factory to get a default value
        /// </summary>
        /// <param name="defaultValueFactory">Called to generate a value if the option is none</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">defaultValueFactory is null</exception>
        public T ValueOr(Func<T> defaultValueFactory)
        {
            if(defaultValueFactory is null) throw new ArgumentNullException(nameof(defaultValueFactory));

            return IsSome ? m_Value : defaultValueFactory();
        }

        /// <summary>
        /// Returns the contained value if some, otherwise calls the factory to get a default value
        /// </summary>
        /// <param name="state">Any additional state to pass to the factory</param>
        /// <param name="defaultValueFactory">Called to generate a value if the option is none</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">defaultValueFactory is null</exception>
        public T ValueOr<TState>(TState state, Func<TState, T> defaultValueFactory)
        {
            if(defaultValueFactory is null) throw new ArgumentNullException(nameof(defaultValueFactory));

            return IsSome ? m_Value : defaultValueFactory(state);
        }

        /// <inheritdoc/>
        public bool Equals(Option<T> other)
        {
            return (IsSome, other.IsSome) switch
            {
                (true, true)    => EqualityComparer<T>.Default.Equals(m_Value, other.m_Value),
                (false, false)  => true,
                _               => false
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if(obj is None) return IsNone;

            return obj is Option<T> other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if(IsSome)
            {
                return m_Value?.GetHashCode() ?? 0;
            }

            return 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if(IsSome)
            {
                var text = m_Value?.ToString() ?? "null";
                return $"Some({text})";
            }
            else
            {
                return "None";
            }
        }

        /// <summary>
        /// Compares 2 options for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(in Option<T> left, in Option<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares 2 options for inequality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(in Option<T> left, in Option<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares a typed option to see if it is none
        /// </summary>
        /// <param name="left"></param>
        /// <param name="none"></param>
        /// <returns></returns>
        public static bool operator ==(in Option<T> left, in None none)
        {
            return left.IsNone;
        }

        /// <summary>
        /// Compares a typed option to see if it is not none
        /// </summary>
        /// <param name="left"></param>
        /// <param name="none"></param>
        /// <returns></returns>
        public static bool operator !=(in Option<T> left, in None none)
        {
            return !left.IsNone;
        }

        /// <summary>
        /// Compares a typed option to see if it is none
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(in None left, in Option<T> right)
        {
            return right.IsNone;
        }

        /// <summary>
        /// Compares a typed option to see if it is none
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(in None left, in Option<T> right)
        {
            return !right.IsNone;
        }

        /// <summary>
        /// Converts none to an option
        /// </summary>
        /// <param name="none"></param>
        public static implicit operator Option<T>(None none)
        {
            return default;
        }

        /// <summary>
        /// Converts a value to an option
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Option<T>(T value)
        {
           return new(value);
        }
    }

    /// <summary>
    /// Useful option construction methods
    /// </summary>
    public static class Option
    {
        /// <summary>
        /// Returns an option containing some value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(value);
        }

        /// <summary>
        /// Creates an option from a nullable value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Option<T> TreatNullAsNone<T>(Nullable<T> value) where T : struct
        {
            return value.HasValue ? Some(value.Value) : None;
        }

        /// <summary>
        /// Creates an option from a nullable reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Option<T> TreatNullAsNone<T>(T? value) where T : class
        {
            if(value is not null) return Some(value);

            return None;
        }

        /// <summary>
        /// The none state
        /// </summary>
        public static readonly None None = default;        
    }
}

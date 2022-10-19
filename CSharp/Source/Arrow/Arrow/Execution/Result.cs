using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Represents the results of a calculation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Result<T>
    {
        public Result(T value)
        {
            this.Success = true;
            this.Value = value;
        }

        public bool Success{get;}

        public T Value{get;}

        public void Deconstruct(out bool success, out T value)
        {
            success = this.Success;
            value = this.Value;
        }
    }

    public static class Result
    {
        public static Result<T> Success<T>(T value)
        {
            return new(value);
        }

        public static Result<T> Fail<T>()
        {
            return default;
        }
    }
}

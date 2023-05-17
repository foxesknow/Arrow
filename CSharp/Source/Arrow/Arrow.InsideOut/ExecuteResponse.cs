using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// The results from a call to IInsideOutNode.Execute
/// </summary>
public sealed class ExecuteResponse : ResponseBase
{
    /// <summary>
    /// True if the execution was a success, false if it failed
    /// </summary>
    public bool Success{get; init;}

    /// <summary>
    /// An optional message set on failure
    /// </summary>
    public string? Message{get; set;}

    /// <summary>
    /// An optional result returned from the command.
    /// The nature of the value is command specific
    /// </summary>
    public Value? Result{get; set;}

    /// <summary>
    /// Attemtps to cast the result to a derived type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public T ResultAs<T>() where T : Value
    {
        if(this.Result is T derived) return derived;

        throw new InvalidCastException($"value is not a {typeof(T).Name}");
    }

    /// <inheritdoc/>
    public override Type Type()
    {
        return typeof(ExceptionResponse);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"Success = {Success}, Message = {Message}, Result = {Result}";
    }
}

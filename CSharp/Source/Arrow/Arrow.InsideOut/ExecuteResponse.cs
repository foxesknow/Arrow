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
    /// The nature of the string is command specific
    /// </summary>
    public string? Result{get; set;}

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Arrow.InsideOut;

/// <summary>
/// A response that represents an exception thrown in the server
/// </summary>
public sealed class ExceptionResponse : ResponseBase
{
    /// <summary>
    /// Initializes the instance
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ExceptionResponse(string message)
    {
        if(message is null) throw new ArgumentNullException(nameof(message));

        this.Message = message;
    }

    /// <summary>
    /// The exception message
    /// </summary>
    public string Message{get;}

    /// <inheritdoc/>
    public override Type Type()
    {
        return typeof(ExceptionResponse);
    }

    /// <summary>
    /// The type of the exception.
    /// This can be used to map to a concrete type on the client
    /// </summary>
    public string? ExceptionType{get; set;}

    /// <summary>
    /// Converts the response to an exception.
    /// The ExceptionType property is used to attemtp to create 
    /// the correct strongly typed exception.
    /// </summary>
    /// <returns></returns>
    public Exception AsException()
    {
        return this.ExceptionType switch
        {
            nameof(NullReferenceException) => new NullReferenceException(this.Message),
            nameof(ArgumentNullException) => new ArgumentNullException(this.Message),
            nameof(ArgumentException) => new ArgumentException(this.Message),
            nameof(ArgumentOutOfRangeException) => new ArgumentOutOfRangeException(this.Message),
            nameof(IOException) => new IOException(this.Message),
            nameof(ObjectDisposedException) => new ObjectDisposedException(this.Message),
            nameof(IndexOutOfRangeException) => new IndexOutOfRangeException(this.Message),
            nameof(NotImplementedException) => new NotImplementedException(this.Message),
            nameof(OperationCanceledException) => new OperationCanceledException(this.Message),
            nameof(TaskCanceledException) => new TaskCanceledException(this.Message),
            nameof(Exception) => new Exception(this.Message),
            nameof(ArrowException) => new ArrowException(this.Message),
            nameof(InsideOutException) => new InsideOutException(this.Message),
            _ => new InsideOutException(this.Message)
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Message;
    }

    public static ExceptionResponse From(Exception exception)
    {
        return new ExceptionResponse(exception.Message)
        {
            ExceptionType = exception.GetType().Name
        };
    }
}

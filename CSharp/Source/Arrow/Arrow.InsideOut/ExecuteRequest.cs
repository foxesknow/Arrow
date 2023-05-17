using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// Represents the data passed to an IInsideOutNode.Execute() 
/// 
/// Because the server returns a tree representation of its state a command is typically nested.
/// Therefore the command path specified the path down the tree to the place where the command exists.
/// </summary>
public sealed class ExecuteRequest : RequestBase
{
    private const string PathDivider = "/";

    private Queue<string>? m_CommandLevels;

    private List<Argument>? m_Arguments;

    public ExecuteRequest(string commandPath)
    {
        if(commandPath is null) throw new ArgumentNullException(nameof(commandPath));
        if(string.IsNullOrWhiteSpace(commandPath)) throw new ArgumentException("invalid command path", nameof(commandPath));

        this.CommandPath = commandPath;
    }

    /// <summary>
    /// The path to the command
    /// </summary>
    public string CommandPath{get;}

    /// <summary>
    /// The arguments to the command
    /// </summary>
    public List<Argument> Arguments
    {
        get{return m_Arguments ??= new();}
        init{m_Arguments = value;}
    }

    /// <summary>
    /// Ensures that the execution details contains the correct number of arguments,
    /// throwing an exception if not.
    /// </summary>
    /// <param name="numberOfArgumentsRequired"></param>
    /// <exception cref="InsideOutException"></exception>
    public void EnsureArgumentCount(int numberOfArgumentsRequired)
    {
        if(this.Arguments.Count != numberOfArgumentsRequired)
        {
            throw new InsideOutException($"expected {numberOfArgumentsRequired} arguments, but only {Arguments.Count} are available");
        }
    }

    /// <summary>
    /// Gets an argument, ensuring that it is the correct type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="InsideOutException"></exception>
    public T GetArgument<T>(int index) where T : Argument
    {
        if(index >= this.Arguments.Count) throw new IndexOutOfRangeException($"index {index} was requested, but there are only {Arguments.Count} arguments");

        if(this.Arguments[index] is T argument) return argument;

        throw new InsideOutException($"argument at {index} is not a {typeof(T).Name}");
    }

    /// <summary>
    /// Returns the number of remaining levels
    /// </summary>
    /// <returns></returns>
    public int LevelsRemaining()
    {
        return GetLevels().Count;
    }

    public string PopLevel()
    {
        if(TryPeekLevel(out var level)) return level;

        throw new InsideOutException("no more levels available");
    }

    /// <summary>
    /// Tries to get the current level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool TryPeekLevel([NotNullWhen(true)] out string? level)
    {
        var levels = GetLevels();

        if(levels.Count == 0)
        {
            level = null;
            return false;
        }

        level = levels.Peek();
        return true;
    }

    /// <summary>
    /// Tries to remove the current level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool TryPopLevel([NotNullWhen(true)] out string? level)
    {
        var levels = GetLevels();

        if(levels.Count == 0)
        {
            level = null;
            return false;
        }

        level = levels.Dequeue();
        return true;
    }    

    /// <summary>
    /// Breaks the command path up into its constituent parts
    /// </summary>
    /// <returns></returns>
    private Queue<string> GetLevels()
    {
        if(m_CommandLevels is not null) return m_CommandLevels;

        var parts = this.CommandPath.Split(PathDivider, StringSplitOptions.RemoveEmptyEntries);
        m_CommandLevels = new(parts);

        return m_CommandLevels;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.CommandPath;
    }

    /// <summary>
    /// Creates a command paths
    /// </summary>
    /// <param name="commandPathParts"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string MakeCommandPath(IEnumerable<string> commandPathParts)
    {
        if(commandPathParts is null) throw new ArgumentNullException(nameof(commandPathParts));

        return string.Join(PathDivider, commandPathParts);
    }
}

public static class ExecuteRequestExtensions
{
    public static R Let<T, R>(this ExecuteRequest executeRequest, Func<T, R> function) 
        where T : Argument
    {
        executeRequest.EnsureArgumentCount(1);

        var arg1 = executeRequest.GetArgument<T>(0);
        return function(arg1);
    }

    public static R LetX<T, R>(this ExecuteRequest executeRequest, Func<T, R> function) 
    {
        executeRequest.EnsureArgumentCount(1);

        var arg1 = executeRequest.GetArgument<Argument<T>>(0).Value;
        return function(arg1);
    }

    public static R Let<T1, T2, R>(this ExecuteRequest executeRequest, Func<T1, T2, R> function) 
        where T1 : Argument
        where T2 : Argument
    {
        executeRequest.EnsureArgumentCount(2);

        var arg1 = executeRequest.GetArgument<T1>(0);
        var arg2 = executeRequest.GetArgument<T2>(1);
        return function(arg1, arg2);
    }

    public static R Let<T1, T2, T3, R>(this ExecuteRequest executeRequest, Func<T1, T2, T3, R> function) 
        where T1 : Argument
        where T2 : Argument
        where T3 : Argument
    {
        executeRequest.EnsureArgumentCount(3);

        var arg1 = executeRequest.GetArgument<T1>(0);
        var arg2 = executeRequest.GetArgument<T2>(1);
        var arg3 = executeRequest.GetArgument<T3>(2);
        return function(arg1, arg2, arg3);
    }

    public static R Let<T1, T2, T3, T4, R>(this ExecuteRequest executeRequest, Func<T1, T2, T3, T4, R> function) 
        where T1 : Argument
        where T2 : Argument
        where T3 : Argument
        where T4 : Argument
    {
        executeRequest.EnsureArgumentCount(4);

        var arg1 = executeRequest.GetArgument<T1>(0);
        var arg2 = executeRequest.GetArgument<T2>(1);
        var arg3 = executeRequest.GetArgument<T3>(2);
        var arg4 = executeRequest.GetArgument<T4>(3);
        return function(arg1, arg2, arg3, arg4);
    }

    public static R Let<T1, T2, T3, T4, T5, R>(this ExecuteRequest executeRequest, Func<T1, T2, T3, T4, T5, R> function) 
        where T1 : Argument
        where T2 : Argument
        where T3 : Argument
        where T4 : Argument
        where T5 : Argument
    {
        executeRequest.EnsureArgumentCount(5);

        var arg1 = executeRequest.GetArgument<T1>(0);
        var arg2 = executeRequest.GetArgument<T2>(1);
        var arg3 = executeRequest.GetArgument<T3>(2);
        var arg4 = executeRequest.GetArgument<T4>(3);
        var arg5 = executeRequest.GetArgument<T5>(4);
        return function(arg1, arg2, arg3, arg4, arg5);
    }
}

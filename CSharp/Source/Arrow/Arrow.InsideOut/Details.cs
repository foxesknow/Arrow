using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

public sealed class Details : ResponseBase
{
    private Dictionary<string, Value> ? m_Values;
    private List<Command>? m_Commands;

    public override Type Type()
    {
        return typeof(Details);
    }

    /// <summary>
    /// The available commands
    /// </summary>
    public List<Command> Commands
    {
        get{return m_Commands ??= new();}
        init{m_Commands = value;}
    }

    /// <summary>
    /// The values.
    /// This can be recursive as a Details instance is also a Value
    /// </summary>
    public Dictionary<string, Value> Values
    {
        get{return m_Values ??= new();}
        init{m_Values = value;}
    }

    /// <summary>
    /// Tries to get the specified command
    /// </summary>
    /// <param name="name"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool TryGetCommand(string name, [NotNullWhen(true)] out Command? command)
    {
        if(name is null) throw new ArgumentNullException(nameof(name));

        command = this.Commands.FirstOrDefault(c => c.Name == name);
        return command is not null;
    }

    /// <summary>
    /// Returns the named command
    /// </summary>
    /// <param name="commandName"></param>
    /// <returns></returns>
    /// <exception cref="InsideOutException"></exception>
    public Command Command(string commandName)
    {
        if(TryGetCommand(commandName, out var command)) return command;

        throw new InsideOutException($"could not find command {commandName}");
    }

    /// <summary>
    /// Tries to get a value from the values collection that is of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue<T>(string name, [NotNullWhen(true)] out T? value) where T : Value
    {
        if(this.Values.TryGetValue(name, out var item) && item is T asType)
        {
            value = asType;
            return true;
        }

        value = null;
        return false;
    }
}

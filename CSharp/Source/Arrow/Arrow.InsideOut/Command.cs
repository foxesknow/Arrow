using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// A command is a piece of functionality that can be executed on the server
/// </summary>
public sealed class Command
{
    private List<Parameter>? m_Parameters;

    /// <summary>
    /// Initializes the instance
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Command(string name)
    {
        if(name is null) throw new ArgumentNullException(nameof(name));
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("command name is blank", nameof(name));

        this.Name = name;
    }

    /// <summary>
    /// The name of the command
    /// </summary>
    public string Name{get;}

    /// <summary>
    /// A brief description of the command
    /// </summary>
    public string? Description{get; set;}

    /// <summary>
    /// The parameters for the command
    /// </summary>
    public List<Parameter> Parameters
    {
        get{return m_Parameters ??= new();}
        init{m_Parameters = value;}
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Name;
    }
}

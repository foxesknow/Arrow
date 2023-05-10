using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Nodes;

public sealed class CalculatorNode : IInsideOutNode
{
    private static readonly DecimalParameter Lhs = new("lhs");
    private static readonly DecimalParameter Rhs = new("rhs");

    private decimal m_LastResult = 0;

    private static readonly Command Add = new("Add")
    {
        Parameters = {Lhs, Rhs},
        Description = "Adds 2 numbers"
    };

    private static readonly Command Subtract = new("Subtract")
    {
        Parameters = {Lhs, Rhs},
        Description = "Subtract 2 numbers"
    };

    private static readonly Command Divide = new("Divide")
    {
        Parameters = {Lhs, Rhs},
        Description = "Divides 2 numbers"
    };

    private static readonly Command Multiply = new("Multiply")
    {
        Parameters = {Lhs, Rhs},
        Description = "Multiplies 2 numbers"
    };

    public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
    {
        request.EnsureArgumentCount(2);
        var lhs = request.GetArgument<DecimalArgument>(0);
        var rhs = request.GetArgument<DecimalArgument>(1);

        var command = request.PopLevel();

        decimal result = command switch
        {
            "Add"       => lhs.Value + rhs.Value,
            "Subtract"  => lhs.Value - rhs.Value,
            "Divide"    => lhs.Value / rhs.Value,
            "Multiply"  => lhs.Value * rhs.Value,
            _           => throw new InsideOutException($"unsupported operation: {command}")
        };

        m_LastResult = result;

        var response = new ExecuteResponse()
        {
            Success = true,
            Result = result.ToString()
        };

        return new(response);
    }

    public ValueTask<Details> GetDetails(CancellationToken ct)
    {
        var details = new Details()
        {
            Commands = {Add, Subtract, Divide, Multiply},
            Values =
            {
                {"LastResult", Value.From(m_LastResult)}
            }
        };

        return new(details);
    }
}

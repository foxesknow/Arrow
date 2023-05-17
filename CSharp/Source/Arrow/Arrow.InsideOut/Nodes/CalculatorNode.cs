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
    private static readonly DecimalParameter TheValue = new("value");

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

    private static readonly Command Negate = new("Negate")
    {
        Parameters = {TheValue},
        Description = "Negates a value"
    };

    private static readonly Command Abs = new("Abs")
    {
        Parameters = {TheValue},
        Description = "Returns the absolute value"
    };

    public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
    {
        var command = request.PopLevel();

        decimal result = command switch
        {
            "Add"       => request.Let((decimal x, decimal y) => x + y),
            "Subtract"  => request.Let((decimal x, decimal y) => x - y),
            "Divide"    => request.Let((decimal x, decimal y) => x / y),
            "Multiply"  => request.Let((decimal x, decimal y) => x * y),
            "Negate"    => request.Let((decimal value) => -value),
            "Abs"       => request.Let((decimal value) => Math.Abs(value)),
            _           => throw new InsideOutException($"unsupported operation: {command}")
        };
        
        var response = new ExecuteResponse()
        {
            Success = true,
            Result = new StructValue()
            {
                Members =
                {
                    {"Result", Value.From(result)},
                    {"LastResult", Value.From(m_LastResult)},
                }
            }
        };

        m_LastResult = result;

        return new(response);
    }

    public ValueTask<Details> GetDetails(CancellationToken ct)
    {
        var details = new Details()
        {
            Commands = {Add, Subtract, Divide, Multiply, Negate, Abs},
            Values =
            {
                {"LastResult", Value.From(m_LastResult)}
            }
        };

        return new(details);
    }
}

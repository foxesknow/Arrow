﻿using System;
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
            "Add"       => request.Let((DecimalArgument x, DecimalArgument y) => x.Value + y.Value),
            "Subtract"  => request.Let((DecimalArgument x, DecimalArgument y) => x.Value - y.Value),
            "Divide"    => request.Let((DecimalArgument x, DecimalArgument y) => x.Value / y.Value),
            "Multiply"  => request.Let((DecimalArgument x, DecimalArgument y) => x.Value * y.Value),
            "Negate"    => request.Let((DecimalArgument value) => -value.Value),
            "Abs"       => request.Let((DecimalArgument value) => Math.Abs(value.Value)),
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
            Commands = {Add, Subtract, Divide, Multiply, Negate, Abs},
            Values =
            {
                {"LastResult", Value.From(m_LastResult)}
            }
        };

        return new(details);
    }
}

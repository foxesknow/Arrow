﻿using System;

using BenchmarkDotNet.Running;

namespace BenchmarkArrow
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
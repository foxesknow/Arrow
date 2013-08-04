using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;

using Arrow.Application;
using Arrow.Scripting.Wire.StaticExpression;
using Arrow.Scripting.Wire.DynamicExpression;
using Arrow.Scripting;

namespace WireConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			ApplicationRunner.Run(()=>Run(args));
		}


		static void Run(string[] args)
		{
			bool generateDynamic=true;

			if(args.Length>0)
			{
				switch(args[0].ToLower())
				{
					case "/static":
					case "-static":
						generateDynamic=false;
						break;

					case "/dynamic":
					case "-dynamic":
						generateDynamic=true;
						break;

					default:
						generateDynamic=true;
						break;
				}
			}

			Console.WriteLine("Running in {0} expression mode",generateDynamic ? "dynamic" : "static");
			Console.WriteLine();
			Console.WriteLine("type exit to exit");
			Console.WriteLine("start an expression with ? to see the generated lambda");
			Console.WriteLine();

			while(true)
			{
				Console.Write("Expression: ");
				string line=Console.ReadLine().Trim();

				if(line=="") continue;
				if(line=="exit") break;

				bool dumpLambda=false;
				if(line.StartsWith("?"))
				{
					dumpLambda=true;
					line=line.Substring(1);
				}

				try
				{
					object result=null;

					if(generateDynamic)
					{
						result=RunDynamic(line,dumpLambda);
					}
					else
					{
						result=RunStatic(line,dumpLambda);
					}

					Console.WriteLine(result);
				}
				catch(Exception e)
				{
					string message=e.Message;
					if(e.InnerException!=null) message=e.InnerException.Message;
					
					Console.Error.WriteLine(message);
				}
			}

		}

		private static object RunStatic(string line, bool dumpLambda)
		{
			var context=new StaticParseContext();
			context.References.Add(typeof(string).Assembly);
			context.Usings.Add("System");

			context.References.Add(typeof(TestData).Assembly);
			context.Usings.Add("WireConsole");

			context.Parameters.Add(Expression.Parameter(typeof(int),"a"));
			context.Parameters.Add(Expression.Parameter(typeof(double),"b"));
			context.Parameters.Add(Expression.Parameter(typeof(TestData),"c"));
			context.Parameters.Add(Expression.Parameter(typeof(string),"d"));

			var generator=new StaticCodeGenerator();
			LambdaExpression expression=generator.CreateLambda(line,context);
			if(dumpLambda) return expression.ToString();
					
			// Because we don't know anything specific about the 
			// expression we'll have to call it via a general purpose delegate
			Delegate d=expression.Compile();
			object result=d.DynamicInvoke(10,2.5,new TestData(),null); // a=10, b=2.5
			return result;
		}

		private static object RunDynamic(string line, bool dumpLambda)
		{
			var context=new DynamicParseContext();
			context.References.Add(typeof(string).Assembly);
			context.Usings.Add("System");

			context.References.Add(typeof(TestData).Assembly);
			context.Usings.Add("WireConsole");

			var generator=new DynamicCodeGenerator();
			LambdaExpression expression=generator.CreateLambda(line,context);
			if(dumpLambda) return expression.ToString();
					
			// Because we don't know anything specific about the 
			// expression we'll have to call it via a general purpose delegate
			Delegate d=expression.Compile();

			LightweightScope scope=new LightweightScope();
			scope.Declare("a",10);
			scope.Declare("b",2.5);
			scope.Declare("c",new TestData());
			scope.Declare("d",(string)null);

			dynamic expando=new ExpandoObject();
			expando.Length=23;
			expando.Name="Jack";
			scope.Declare("Details",expando);

			Func<int,int,int> adder=Add;
			expando.Adder=adder;

			scope.Declare("adder",adder);

			d.DynamicInvoke(scope);
			object result=d.DynamicInvoke(scope);
			return result;
		}

		static int Add(int x, int y)
		{
			return x+y;
		}

		static void StaticTest()
		{
			string expression="(a+b)";

			var context=new StaticParseContext();
			context.Parameters.Add(Expression.Parameter(typeof(int),"a"));
			context.Parameters.Add(Expression.Parameter(typeof(int),"b"));

			var generator=new StaticCodeGenerator();
			var function=generator.CreateFunction<Func<int,int,int>>(expression,context);
			var result=function(10,20);
			Console.WriteLine(result);
		}
	}

	public class TestData
	{
		public List<string> m_Names=new List<string>();

		public string What="HelloWorld";

		public TestData()
		{
			m_Names.AddRange(new string[]{"Rod","Jane","Freddy"});
		}

		public IList<string> Names
		{
			get{return m_Names;}
		}

		public static int Counter
		{
			get{return 58;}
		}
	}
}

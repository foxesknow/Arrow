using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Arrow.IO;
using Arrow.Scripting;
using Arrow.Scripting.Wire;
using Arrow.Scripting.Wire.StaticExpression;
using Arrow.Scripting.Wire.DynamicExpression;

using NUnit.Framework;
using System.Linq.Expressions;

namespace UnitTests.Arrow.Scripting.Wire
{
	class ScriptRunner
	{
		public static void RunStaticExpressions(string filename, StaticParseContext parseContext, params object[] parameters)
		{
			string script=LoadScript(filename);
			using(var reader=new StringReader(script))
			{
				int lineNumber=0;
				string line;
				while((line=reader.ReadLine())!=null)
				{
					lineNumber++;

					line=line.Trim();
					if(line.Length==0) continue;
					if(line[0]=='#') continue;

					string result,expression;
					ExtractParts(line,out result,out expression);
					Console.WriteLine(expression);

					var generator=new StaticCodeGenerator();
					LambdaExpression lambda=null;
					
					try
					{
						lambda=generator.CreateLambda(expression,parseContext);
					}
					catch(Exception e)
					{
						if(result=="@nocompile")
						{
							// It was suppose to fail
							Console.WriteLine("script successfully failed to compile: {0}",e.Message);
							continue;
						}
						else
						{
							throw;
						}
					}

					var func=lambda.Compile();
					object dynamicResult=func.DynamicInvoke(parameters);

					object expectedResult=null;
					
					if(result=="null")
					{
						expectedResult=null;
					}
					else
					{
						expectedResult=Convert.ChangeType(result,dynamicResult.GetType());
					}

					if(object.Equals(dynamicResult,expectedResult)==false)
					{
						string message=string.Format("Expression: Script {0}, Line {1}, Expected {2}, got {3}, Expression={4}",filename,lineNumber,expectedResult,dynamicResult,expression);
						throw new AssertionException(message);
					}
				}
			}
		}

		public static void RunDynamicExpressions(string filename, DynamicParseContext parseContext)
		{
			RunDynamicExpressions(filename,parseContext,new LightweightScope());
		}

		public static void RunDynamicExpressions(string filename, DynamicParseContext parseContext, IVariableRead variableRead)
		{
			string script=LoadScript(filename);
			using(var reader=new StringReader(script))
			{
				int lineNumber=0;
				string line;
				while((line=reader.ReadLine())!=null)
				{
					lineNumber++;

					line=line.Trim();
					if(line.Length==0) continue;
					if(line[0]=='#') continue;

					string result,expression;
					ExtractParts(line,out result,out expression);
					Console.WriteLine(expression);

					var generator=new DynamicCodeGenerator();
					LambdaExpression lambda=null;
					
					try
					{
						lambda=generator.CreateLambda(expression,parseContext);
					}
					catch(Exception e)
					{
						if(result=="@nocompile")
						{
							// It was suppose to fail
							Console.WriteLine("script successfully failed to compile: {0}",e.Message);
							continue;
						}
						else
						{
							throw;
						}
					}

					var func=lambda.Compile();
					object dynamicResult=func.DynamicInvoke(variableRead);

					object expectedResult=null;

					if(result=="null")
					{
						expectedResult=null;
					}
					else
					{
						expectedResult=Convert.ChangeType(result,dynamicResult.GetType());
					}

					if(object.Equals(dynamicResult,expectedResult)==false)
					{
						string message=string.Format("Expression: Script {0}, Line {1}, Expected {2}, got {3}, Expression={4}",filename,lineNumber,expectedResult,dynamicResult,expression);
						throw new AssertionException(message);
					}
				}
			}
		}

		private static string LoadScript(string filename)
		{
			string path="UnitTests.Arrow.Scripting.Wire.Scripts.";
			filename=path+filename;
			
			using(Stream stream=ResourceLoader.Open<ScriptRunner>(filename))
			using(StreamReader reader=new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		private static void ExtractParts(string line, out string result, out string expression)
		{
			int pivot=line.IndexOf(',');
			if(pivot==-1) throw new Exception("badly formed string: "+line);

			result=line.Substring(0,pivot).Trim();
			expression=line.Substring(pivot+1).Trim();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Application
{
	/// <summary>
	/// Displays a page of information to the console window and give the user the 
	/// option to move through it be displaying a menu at the bottom of the screen
	/// </summary>
	public static class More
	{
		/// <summary>
		/// Displays line on the console.
		/// When the console window is full the user is prompted to stop paging, page by one line or page by a page
		/// </summary>
		/// <param name="lines">The sequence of lines to display in the console</param>
		public static void Render(IEnumerable<string> lines)
		{
			if(lines==null) throw new ArgumentNullException("lines");
		
			long rowsOutput=0;
			
			foreach(string line in lines)
			{
				Console.WriteLine(line);
				
				rowsOutput++;
				
				if(rowsOutput+1>=Console.WindowHeight)
				{
					string prompt="--more--[q=quit, enter=line, other=page]";
					Console.Write(prompt);
					ConsoleKeyInfo what=Console.ReadKey(true);
					
					// Remove the prompt
					string wipeout=new string('\b',prompt.Length);
					Console.Write(wipeout);
					Console.Write(new string(' ',prompt.Length));
					Console.Write(wipeout);
					
					if(what.KeyChar=='q') return;
					if(what.KeyChar=='\r') continue;
									
					rowsOutput=0;
				}
			}
		}
	}
}

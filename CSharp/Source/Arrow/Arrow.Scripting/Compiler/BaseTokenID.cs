using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Compiler
{
	/// <summary>
	/// A set of tokens used by the general purpose tokenizer
	/// This numbers 0-99 (inclusive) are reserved for it.
	/// </summary>
	public abstract class BaseTokenID
	{
		/// <summary>
		/// The id for no tokens
		/// </summary>
		public const int None=0;
		
		/// <summary>
		/// The id for an unknown token
		/// </summary>
		public const int Unknown=1;
		
		/// <summary>
		/// The id for a symbol
		/// </summary>
		public const int Symbol=2;
		
		/// <summary>
		/// The id for a number
		/// </summary>
		public const int Number=3;
		
		/// <summary>
		/// The id for a string
		/// </summary>
		public const int String=4;
		
		/// <summary>
		/// The id for a character
		/// </summary>
		public const int Char=5;
	}
}

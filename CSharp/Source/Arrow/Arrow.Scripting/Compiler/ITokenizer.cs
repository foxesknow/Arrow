using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Compiler
{
	/// <summary>
	/// Defines the behaviour of a tokenizer
	/// </summary>
	public interface ITokenizer
	{
		/// <summary>
		/// The current line number with the token stream
		/// </summary>
		int LineNumber{get;}
		
		/// <summary>
		/// The name of the file being tokenized
		/// </summary>
		string Filename{get;}

		/// <summary>
		/// Attempts to accept a token type.
		/// If there is a match the next token is retrived and true is returned, otherwise false is returned
		/// </summary>
		/// <param name="id">The token to accept</param>
		/// <returns>true if the token could be accepted, otherwise false</returns>
		bool TryAccept(int id);

		/// <summary>
		/// Attempts to accep one of a series of token ids
		/// </summary>
		/// <param name="token">On success the token that was accepted</param>
		/// <param name="ids">A list of ids to try</param>
		/// <returns>true if a token was accepted, otherwise false</returns>
		bool TryAcceptOneOf(out Token token, params int[] ids);

		/// <summary>
		/// Accepts the current token, regardless of what it is
		/// </summary>
		void Accept();

		/// <summary>
		/// Expects the specified token to be the current token, or throws an exception
		/// </summary>
		/// <param name="id">The token to expect</param>
		/// <returns>The token that was expected</returns>
		Token Expect(int id);

		/// <summary>
		/// Gets the next token in the stream
		/// </summary>
		/// <returns>The next token in the stream</returns>
		Token NextToken();

		/// <summary>
		/// The current token
		/// </summary>
		Token Current{get;}

		/// <summary>
		/// Checks if the current token is one of the specified ids
		/// </summary>
		/// <param name="ids">The ids to check</param>
		/// <returns>true if theres a match, otherwise false</returns>
		bool CurrentOneOf(params int[] ids);

		/// <summary>
		/// Returns the next token in the stream without removing it from the stream
		/// </summary>
		/// <returns>The next token in the stream</returns>
		Token Peek();
	}
}

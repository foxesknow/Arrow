using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

using Arrow.Collections;

namespace Arrow.Compiler
{
	/// <summary>
	/// Provides a reasonable implementation of a general purpose parser.
	/// To use just derive from the c
	/// </summary>
	public abstract class GeneralPurposeTokenizer : ITokenizer
	{
		private const string HexDigit="0123456789abcdefABCDEF";
		
		private TextReader m_Reader;
		
		private Token m_Current=Token.NoToken;
		private Token? m_Peek;
		
		private int m_LineNumber=1;
		private string m_Filename;

		private Dictionary<string,int> m_Keywords = new();
		private Dictionary<string,int> m_Operators = new();
		private List<string> m_OperatorLookup = new();

		private IComparer<string>? m_Comparer;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="reader">The reader to read tokens from</param>
		/// <param name="filename">The name of the file being tokenized</param>
		protected GeneralPurposeTokenizer(TextReader reader, string filename)
		{
			if(reader==null) throw new ArgumentNullException("reader");
			if(filename==null) throw new ArgumentNullException("filename");
			
			m_Reader=reader;
			m_Filename=filename;
		}
		
		/// <summary>
		/// Performs any initialization prior to supplying tokens
		/// </summary>
		public virtual void Initialize()
		{
			IEqualityComparer<string>? comparer=null;
			
			if(this.CaseSensitive)
			{
				comparer=null;
				m_Comparer=null;
			}
			else
			{
				comparer=IgnoreCaseEqualityComparer.Instance;
				m_Comparer=IgnoreCaseComparer.Instance;
			}

			m_Keywords=new Dictionary<string,int>(this.Keywords,comparer);
			m_Operators=new Dictionary<string,int>(this.Operators,comparer);
			
			m_OperatorLookup=new List<string>(m_Operators.Keys);
			m_OperatorLookup.Sort(m_Comparer);
			
			// Position ourselves on the first token
			NextToken();
		}

		/// <summary>
		/// Indicates if the tokenizer is case sensitive
		/// </summary>
		public bool CaseSensitive{get;protected set;}
		
		/// <summary>
		/// The current line number with the token stream
		/// </summary>
		public int LineNumber
		{
			get{return m_LineNumber;}
		}
		
		/// <summary>
		/// The name of the file being tokenized
		/// </summary>
		public string Filename
		{
			get{return m_Filename;}
		}
		
		/// <summary>
		/// Returns all valid operators for the language
		/// </summary>
		protected abstract IDictionary<string,int> Operators
		{
			get;
		}
		
		/// <summary>
		/// Returns all valid keywords for the language
		/// </summary>
		protected abstract IDictionary<string,int> Keywords
		{
			get;
		}

		/// <summary>
		/// Returns the character used for comments
		/// </summary>
		protected virtual char CommentChar
		{
			get{return '#';}
		}
				
		/// <summary>
		/// Checks is a character may be used as the start of a symbol name.
		/// By default an alphabetic or underscore are valid
		/// </summary>
		/// <param name="c">the character to check</param>
		/// <returns>true if valid, otherwise false</returns>
		protected virtual bool IsValidFirstSymbolCharacter(char c)
		{
			return char.IsLetter(c) || c=='_';
		}
		
		/// <summary>
		/// Checks if a character is valid for a symbol name from the second character onwards.
		/// Dy default alpanumberic, underscores or periods are valid
		/// </summary>
		/// <param name="c">The character to check</param>
		/// <returns>true if valid, otherwise false</returns>
		protected virtual bool IsValidSubsequentSymbolCharacter(char c)
		{
			return char.IsLetterOrDigit(c) || c=='_';
		}
		
		/// <summary>
		/// Attempts to accept a token type.
		/// If there is a match the next token is retrived and true is returned, otherwise false is returned
		/// </summary>
		/// <param name="id">The token to accept</param>
		/// <returns>true if the token could be accepted, otherwise false</returns>
		public bool TryAccept(int id)
		{
			if(m_Current==null || m_Current.ID!=id) return false;
			
			NextToken();
			return true;
		}

		/// <summary>
		/// Attempts to accep one of a series of token ids
		/// </summary>
		/// <param name="token">On success the token that was accepted</param>
		/// <param name="ids">A list of ids to try</param>
		/// <returns>true if a token was accepted, otherwise false</returns>
		public bool TryAcceptOneOf(out Token? token, params int[] ids)
		{
			bool accepted=false;
			token=null;

			if(m_Current==null) return false;

			for(int i=0; i<ids.Length && !accepted; i++)
			{
				accepted=(ids[i]==m_Current.ID);
			}

			if(accepted)
			{			
				token=m_Current;
				NextToken();
			}

			return accepted;
		}

		/// <summary>
		/// Checks if the current token is one of the specified ids
		/// </summary>
		/// <param name="ids">The ids to check</param>
		/// <returns>true if theres a match, otherwise false</returns>
		public bool CurrentOneOf(params int[] ids)
		{
			return ids.Contains(this.Current.ID);
		}
		
		/// <summary>
		/// Accepts the current token, regardless of what it is
		/// </summary>
		public void Accept()
		{
			NextToken();
		}
		
		/// <summary>
		/// Expects the specified token to be the current token, or throws an exception
		/// </summary>
		/// <param name="id">The token to expect</param>
		/// <returns>The token that was expected</returns>
		public Token Expect(int id)
		{
			Token current=m_Current;
		
			if(TryAccept(id)==false) 
			{
				ThrowException("expected token: "+TokenIDToText(id));
			}
			
			return current;
		}
		
		/// <summary>
		/// Gets the next token in the stream
		/// </summary>
		/// <returns>The next token in the stream</returns>
		public Token NextToken()
		{
			if(m_Peek!=null)
			{
				m_Current=m_Peek;
				m_Peek=null;
			}
			else
			{
				m_Current=ExtractToken();
			}
			
			return m_Current;
		}
		
		/// <summary>
		/// The current token
		/// </summary>
		public Token Current
		{
			get{return m_Current;}
		}
		
		/// <summary>
		/// Returns the next token in the stream without removing it from the stream
		/// </summary>
		/// <returns>The next token in the stream</returns>
		public Token Peek()
		{
			if(m_Peek==null) m_Peek=ExtractToken();
			
			return m_Peek;
		}
		
		private Token ExtractToken()
		{
			ConsumeWhitespace();
			
			Token token=Token.NoToken;
			
			char peek=PeekChar();
			
			char commentChar=this.CommentChar;

			while(peek==commentChar)
			{
				NextChar();
				for(; peek!='\n'; NextChar())
				{
					peek=PeekChar();
					if(peek==0) return Token.NoToken;
				}
				
				ConsumeWhitespace();
				peek=PeekChar();
			}
			
			if(char.IsNumber(peek))
			{
				token=ExtractNumber();
			}
			else if(peek=='\'')
			{
				token=ExtractChar();
			}
			else if(peek=='\"')
			{
				token=ExtractString();
			}
			else if(peek=='@')
			{
				token=ExtractQuotedString();
			}
			else
			{
				if(IsStartOfOperator(peek))
				{
					token=ExtractOperator();
				}
				else if(IsValidFirstSymbolCharacter(peek))
				{
					token=ExtractSymbol();
				}
				else if(peek==0)
				{
					token=Token.NoToken;
				}
				else
				{
					token=Token.UnknownToken;
				}
			}
			
			return token;
		}

		private bool IsStartOfOperator(char c)
		{
			string s=c.ToString();
			int index=m_OperatorLookup.BinarySearch(s,m_Comparer);

			bool isStart=false;

			if(index>=0)
			{
				isStart=true;
			}
			else
			{
				index=~index;
				if(index==m_OperatorLookup.Count)
				{
					isStart=false;
				}
				else if(m_OperatorLookup[index][0]==c)
				{
					isStart=true;
				}
			}

			return isStart;
		}

		private bool IsPartialOperator(string s)
		{
			int index=m_OperatorLookup.BinarySearch(s,m_Comparer);

			bool isStart=false;

			if(index>=0)
			{
				isStart=true;
			}
			else
			{
				index=~index;
				if(index==m_OperatorLookup.Count)
				{
					isStart=false;
				}
				else if(m_OperatorLookup[index].StartsWith(s,!this.CaseSensitive,null))
				{
					isStart=true;
				}
			}

			return isStart;
		}
		
		/// <summary>
		/// Extracts a character from the stream
		/// </summary>
		/// <returns>A TokenData instance for the character</returns>
		protected virtual Token ExtractChar()
		{
			NextChar(); // Move past the open quote
			char c=NextChar();
			if(c=='\\')
			{
				c=NextChar();
				c=DecodeEscapeCharacter(c);
			}
			
			// We should now be at the closing quote
			if(NextChar()!='\'')
			{
				ThrowException("could not find close of character sequence");
			}
			
			return new Token(BaseTokenID.Char,new string(c,1),c);
		}
		
		/// <summary>
		/// Extracts a string from the stream
		/// </summary>
		/// <returns>A TokenData instance for the string</returns>
		protected virtual Token ExtractString()
		{
			NextChar();
			
			StringBuilder builder=new StringBuilder();
			
			bool foundClose=false;
			for(char c=NextChar(); c!=0 && !foundClose;)
			{
				if(c==0) break;
				
				if(c=='\"')
				{
					foundClose=true;
					break;
				}
				else
				{				
					if(c=='\\') c=DecodeEscapeCharacter(NextChar());
					builder.Append(c);
					
					c=NextChar();
				}
			}
			
			if(!foundClose)
			{
				ThrowException("no close string encountered");
			}
			
			return new Token(BaseTokenID.String,builder.ToString());
		}
		
		/// <summary>
		/// Extracts a quoted string from the stream
		/// </summary>
		/// <returns>A TokenData instance for the string</returns>
		protected virtual Token ExtractQuotedString()
		{
			NextChar(); // Move past the @
			NextChar(); // Most past the "
			
			StringBuilder builder=new StringBuilder();
			
			bool foundClose=false;
			for(char c=NextChar(); c!=0 && !foundClose;)
			{
				if(c==0) break;
				
				if(c=='"')
				{
					if(PeekChar()!='"')
					{
						foundClose=true;
						break;
					}
				
					NextChar(); // Consume the quote
				}
				
				builder.Append(c);				
				c=NextChar();
			}
			
			if(!foundClose)
			{
				ThrowException("no close string encountered");
			}
			
			return new Token(BaseTokenID.String,builder.ToString());
		}
		
		private char DecodeEscapeCharacter(char c)
		{
			switch(c)
			{
				case '\'':	return '\'';
				case '"':	return '\"';
				case '\\':	return '\\';
				case '0':	return '\0';
				case 'a':	return '\a';
				case 'b':	return '\b';
				case 'f':	return '\f';
				case 'n':	return '\n';
				case 'r':	return '\r';
				case 't':	return '\t';
			
				default:
					ThrowException(string.Format("invalid escape char: \\{0}",c));
					return (char)0;
			}
		}
		
		/// <summary>
		/// Extracts a number from the stream
		/// </summary>
		/// <returns>A TokenData instance for the number. TokenData.ConvertedData must contain the number in its underlying type.</returns>
		protected virtual Token ExtractNumber()
		{
			string number="";
			
			if(PeekChar()=='0')
			{
				number+=NextChar();
				
				if(PeekChar()=='x')
				{
					return ExtractHexNumber();
				}
			}
			
			bool seenDecimalPoint=false;
			while(char.IsNumber(PeekChar()) || PeekChar()=='.')
			{
				char c=NextChar();
				number+=c;
				
				if(c=='.')
				{
					if(seenDecimalPoint) ThrowException("too many decimal points");
					seenDecimalPoint=true;
				}
			}
			
			Token tokenData=Token.UnknownToken;
			object? convertedNumber=null;
			
			char typeQualifier=PeekChar();
			
			if(char.IsLetter(typeQualifier))
			{
				if(typeQualifier=='L')
				{
					NextChar();
					convertedNumber=long.Parse(number);
				}
				else if(typeQualifier=='d' || typeQualifier=='D')
				{
					NextChar();
					convertedNumber=double.Parse(number);
				}
				else if(typeQualifier=='f' || typeQualifier=='F')
				{
					NextChar();
					convertedNumber=float.Parse(number);
				}
				else if(typeQualifier=='m' || typeQualifier=='M')
				{
					NextChar();
					convertedNumber=decimal.Parse(number);
				}
				else if(typeQualifier=='b' || typeQualifier=='B')
				{
					// This isn't a C# suffix, but it does allow us to explicitly create a byte
					NextChar();
					convertedNumber=byte.Parse(number);
				}
			}
			else
			{
				if(seenDecimalPoint)
				{
					// It's got to be a double
					convertedNumber=double.Parse(number);
				}
				else
				{
					int anInt;
					if(int.TryParse(number,out anInt))
					{
						convertedNumber=anInt;
					}
					else
					{
						// We'll be flexible and treat it as a long
						convertedNumber=long.Parse(number);
					}
				}
			}			
			
			return new Token(BaseTokenID.Number,number,convertedNumber);
		}
		
		/// <summary>
		/// Extracts a hex number from the stream
		/// </summary>
		/// <returns>A TokenData instance for the number. TokenData.ConvertedData must contain the number in its underlying type.</returns>
		protected virtual Token ExtractHexNumber()
		{
			// Initially we're pointing to the x
			NextChar();
			
			string hexNumber="";
			
			// We're now at the fist hex character
			while(char.IsLetterOrDigit(PeekChar()))
			{
				if(HexDigit.IndexOf(PeekChar())!=-1)
				{
					hexNumber+=NextChar();
				}
				else
				{
					ThrowException("invalid hex character: "+PeekChar());
				}
			}
			object? converted=null;
			
			int i;
			if(int.TryParse(hexNumber,NumberStyles.HexNumber,null,out i))
			{
				converted=i;
			}
			else
			{
				// Treat it as a long
				converted=long.Parse(hexNumber,NumberStyles.HexNumber,null);
			}
			
			return new Token(BaseTokenID.Number,hexNumber,converted);
		}
		
		/// <summary>
		/// Extracts a symbol from the stream
		/// </summary>
		/// <returns>A TokenData instance for the symbol</returns>
		protected virtual Token ExtractSymbol()
		{
			string symbol="";
			
			// We know the first character is valid
			symbol+=NextChar();
			
			while(IsValidSubsequentSymbolCharacter(PeekChar()))
			{
				symbol+=NextChar();
			}
			
			Token? token=null;
			int id;
			
			if(m_Keywords.TryGetValue(symbol,out id))
			{
				token=new Token(id,symbol);
			}
			else
			{
				token=new Token(BaseTokenID.Symbol,symbol);
			}
			
			return token;
		}
		
		
		/// <summary>
		/// Extracts an operator from the stream
		/// </summary>
		/// <returns>A TokenData representing the operator</returns>
		protected virtual Token ExtractOperator()
		{
			char c=NextChar();
			
			if(c==0) return Token.NoToken;
			
			Token token=Token.UnknownToken;
			
			// We'll do maximal munch to identify operators
			string op=new string(c,1);
			if(IsPartialOperator(op))
			{
				char peek='\0';
				while((peek=PeekChar())!=0)
				{
					string newOp=op+peek;
					if(IsPartialOperator(newOp)==false)
					{
						break;
					}
					else
					{
						NextChar(); // We need to consume the char
						op=newOp;
					}
				}
			}
			
			int id;
			if(m_Operators.TryGetValue(op,out id))
			{
				token=new Token(id,op);
			}	
			
			return token;
		}
		
		/// <summary>
		/// Consumes and whitespace in the reader
		/// </summary>
		private void ConsumeWhitespace()
		{
			while(char.IsWhiteSpace(PeekChar()))
			{
				char c=NextChar();
			}
		}
		
		/// <summary>
		/// Returns the next character in the stream, or the null character if no more are available
		/// </summary>
		/// <returns></returns>
		private char NextChar()
		{
			int c=m_Reader.Read();			
			if(c=='\n') m_LineNumber++;
			
			return c==-1 ? (char)0 : (char)c;
		}
		
		/// <summary>
		/// Returns the next character in the stream without removing it, or the null character if no more are available
		/// </summary>
		/// <returns></returns>
		private char PeekChar()
		{
			int c=m_Reader.Peek();
			return c==-1 ? (char)0 : (char)c;
		}
		
		private string TokenIDToText(int id)
		{
			 foreach(KeyValuePair<string,int> pair in this.Keywords)
			 {
				if(pair.Value==id) return pair.Key;
			 }
			 
			 foreach(KeyValuePair<string,int> pair in this.Operators)
			 {
				if(pair.Value==id) return pair.Key;
			 }
			 
			 return string.Format("[unknown token id {0}]",id);
		}
		
		private void ThrowException(string message)
		{
			var e=new TokenizationException(message);
			e.LineNumber=m_LineNumber;
			e.Filename=m_Filename;
			
			throw e;
		}
	}
}

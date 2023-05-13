using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Compiler
{
	/// <summary>
	/// Repersents a token read by the tokenizer
	/// </summary>
	public class Token
	{
        /// <summary>
        /// Represents to no tokens left
        /// </summary>
        public static readonly Token NoToken = new Token(BaseTokenID.None);

        /// <summary>
        /// Represents an unknown token
        /// </summary>
        public static readonly Token UnknownToken = new Token(BaseTokenID.Unknown);

        private readonly int m_ID;
        private readonly string? m_Data;
        private readonly object? m_ConvertedData;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="tokenType">The token identifier</param>
        public Token(int tokenType) : this(tokenType, null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="id">The token identifier</param>
        /// <param name="data">Any textual data about the token</param>
        public Token(int id, string? data)
        {
            m_ID = id;
            m_Data = data;
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="id">The token identifier</param>
        /// <param name="data">Any textual data about the token</param>
        /// <param name="convertedData">Any additional data about the token</param>
        public Token(int id, string data, object? convertedData) : this(id, data)
        {
            m_ConvertedData = convertedData;
        }

        /// <summary>
        /// The token ID
        /// </summary>
        public int ID
		{
			get{return m_ID;}
		}
		
		/// <summary>
		/// Any token data
		/// </summary>
		public string? Data
		{
			get{return m_Data;}
		}
		
		/// <summary>
		/// Any addition data
		/// </summary>
		public object? ConvertedData
		{
			get{return m_ConvertedData;}
		}

        /// <summary>
        /// Returns a string representation of the token
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return string.Format("id={0}, data={1}", this.ID, this.Data);
        }

        /// <summary>
        /// Generates a hash code
        /// </summary>
        /// <returns>A hash for the token</returns>
        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
    }
}

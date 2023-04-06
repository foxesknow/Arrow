using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.DaemonHosting
{
    /// <summary>
    /// Parses a string into a a Powershell inspired pipeline
    /// </summary>
    internal static class PipelineTokenizer
    {
        private const char Escape = '`';
        private const char Pipe = '|';
        private const char DoubleQuote = '"';
        private const char SingleQuote = '\'';

        public static IEnumerable<Token> Parse(string line)
        {
            var inEscape = false;
            var inQuote = false;
            var lastToken = TokenType.None;
            var quoteBlockTerminator = '\0';

            var active = "";

            foreach(var c in line)
            {
                if(inQuote)
                {
                    if(inEscape)
                    {
                        active += ConvertEscape(c);
                        inEscape = false;
                        continue;
                    }

                    // Powershell only escapes text in double quotes. We'll do the same
                    if(c == Escape && quoteBlockTerminator == DoubleQuote)
                    {
                        inEscape = true;
                        continue;
                    }

                    if(c == quoteBlockTerminator)
                    {
                        yield return MakeAndReset(TokenType.QuotedText);
                        inQuote = false;
                        quoteBlockTerminator = '\0';
                        continue;
                    }

                    active += c;
                }
                else
                {
                    if(c == Escape) throw new ArrowException("you can only escape in quoted text");

                    if(char.IsWhiteSpace(c))
                    {
                        if(active.Length != 0)
                        {
                            yield return MakeAndReset(TokenType.String);
                        }

                        continue;
                    }

                    if(c == DoubleQuote || c == SingleQuote || c == Pipe)
                    {
                        if(active.Length != 0)
                        {
                            yield return MakeAndReset(TokenType.String);
                        }

                        if(c == Pipe)
                        {
                            if(lastToken == TokenType.String || lastToken == TokenType.QuotedText)
                            {
                                yield return MakeAndReset(TokenType.Pipe);
                                continue;
                            }
                            else
                            {
                                throw new ArrowException("an empty pipe element is not allowed");
                            }
                        }

                        inQuote = true;
                        quoteBlockTerminator = c;

                        continue;
                    }

                    active += c;
                }
            }

            if(inQuote) throw new ArrowException($"the string is missing the terminator: {quoteBlockTerminator}");
            if(inEscape) throw new ArrowException("incomplete escape sequence");

            if(active.Length != 0)
            {
                yield return new Token(TokenType.String, active);
            }
            else if(lastToken == TokenType.None)
            {
                yield return new Token(TokenType.None, null);
            }
            else if(lastToken == TokenType.Pipe)
            {
                throw new ArrowException("an empty pipe element is not allowed");
            }

            Token MakeAndReset(TokenType tokenType)
            {
                var token = new Token(tokenType, active);
                lastToken = tokenType;
                active = "";

                return token;
            }
        }

        internal static char ConvertEscape(char c)
        {
            return c switch
            {
                '0' => '\0',
                'a' => '\a',
                'b' => '\b',
                'f' => '\f',
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                'v' => '\v',
                _   => c,
            };
        }


        public readonly struct Token
        {
            public Token(TokenType tokenType, string? value)
            {
                this.TokenType = tokenType;
                this.Value = value;
            }

            public TokenType TokenType{get;}

            public string? Value{get;}

            public override string ToString()
            {
                if(this.TokenType == TokenType.String)
                {
                    return $"String = [{this.Value}]";
                }
                else if(this.TokenType == TokenType.QuotedText)
                {
                    return $"Quoted = [{this.Value}]";
                }
                else
                {
                    return this.TokenType.ToString();
                }
            }
        }

        public enum TokenType
        {
            None,
            Pipe,
            String,
            QuotedText
        }
    }
}

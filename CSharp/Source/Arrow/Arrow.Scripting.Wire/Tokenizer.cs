using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Arrow.Compiler;

namespace Arrow.Scripting.Wire
{
	class Tokenizer : GeneralPurposeTokenizer
	{
		private static readonly Dictionary<string,int> s_Operators=new Dictionary<string,int>();
		private static readonly Dictionary<string,int> s_Keywords=new Dictionary<string,int>();

        static Tokenizer()
        {
            s_Keywords["and"] = TokenID.LogicalAnd;
            s_Keywords["or"] = TokenID.LogicalOr;
            s_Keywords["not"] = TokenID.Not;
            s_Keywords["true"] = TokenID.True;
            s_Keywords["false"] = TokenID.False;
            s_Keywords["null"] = TokenID.Null;

            s_Operators["("] = TokenID.LeftParen;
            s_Operators[")"] = TokenID.RightParen;
            s_Operators["["] = TokenID.LeftSquare;
            s_Operators["]"] = TokenID.RightSquare;

            s_Operators["+"] = TokenID.Add;
            s_Operators["-"] = TokenID.Subtract;
            s_Operators["*"] = TokenID.Multiply;
            s_Operators["/"] = TokenID.Divide;
            s_Operators["%"] = TokenID.Modulo;
            s_Operators["??"] = TokenID.NullCoalesce;

            s_Operators["=="] = TokenID.EqualTo;
            s_Operators["!="] = TokenID.NotEquals;
            s_Operators[">"] = TokenID.GreaterThan;
            s_Operators[">="] = TokenID.GreaterThanOrEqual;
            s_Operators["<"] = TokenID.LessThan;
            s_Operators["<="] = TokenID.LessThanOrEqual;
            s_Operators["==="] = TokenID.EqualToNoCase;
            s_Operators["!=="] = TokenID.NotEqualsNoCase;
            s_Operators["~="] = TokenID.RegexEquals;

            s_Operators["."] = TokenID.MemberAccess;
            s_Operators[","] = TokenID.Comma;
            s_Operators["?."] = TokenID.ConditionalMemberAccess;

            s_Keywords["in"] = TokenID.In;
            s_Operators["~in"] = TokenID.InNoCase;
            s_Keywords["between"] = TokenID.Between;

            s_Keywords["is"] = TokenID.Is;
            s_Keywords["as"] = TokenID.As;
            s_Keywords["cast"] = TokenID.Cast;

            s_Operators["?"] = TokenID.Question;
            s_Operators[":"] = TokenID.Colon;
            s_Keywords["iif"] = TokenID.IIf;

            s_Keywords["like"] = TokenID.Like;
            s_Operators["~like"] = TokenID.LikeNoCase;

            s_Keywords["select"] = TokenID.Select;
            s_Keywords["default"] = TokenID.Default;
            s_Operators["=>"] = TokenID.LeadsTo;

            s_Operators["|"] = TokenID.BitwiseOr;
            s_Operators["&"] = TokenID.BitwiseAnd;
        }

        public Tokenizer(TextReader reader, string filename) : base(reader, filename)
        {
            this.CaseSensitive = false;
        }

        protected override IDictionary<string, int> Operators
        {
            get{return s_Operators;}
        }

        protected override IDictionary<string, int> Keywords
        {
            get{return s_Keywords;}
        }
    }
}

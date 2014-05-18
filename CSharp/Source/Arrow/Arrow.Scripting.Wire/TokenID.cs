using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Compiler;

namespace Arrow.Scripting.Wire
{
	public sealed class TokenID : BaseTokenID
	{
		public const int LogicalAnd=100;
		public const int LogicalOr=101;
		public const int Not=102;		

		public const int Add=120;
		public const int Subtract=121;
		public const int Multiply=122;
		public const int Divide=123;
		public const int Modulo=124;
		public const int NullCoalesce=125;

		public const int EqualTo=130;
		public const int NotEquals=131;
		public const int GreaterThan=132;
		public const int GreaterThanOrEqual=133;
		public const int LessThan=134;
		public const int LessThanOrEqual=135;
		public const int EqualToNoCase=136;
		public const int NotEqualsNoCase=137;
		public const int RegexEquals=138;

		public const int True=140;
		public const int False=141;
		public const int Null=142;

		public const int MemberAccess=150;
		public const int Comma=151;
		public const int ConditionalMemberAccess=152;

		public const int LeftParen=160;
		public const int RightParen=161;
		public const int LeftSquare=162;
		public const int RightSquare=163;

		public const int In=170;
		public const int InNoCase=171;
		public const int Between=172;

		public const int Cast=180;
		public const int Is=181;
		public const int As=182;

		public const int Question=190;
		public const int Colon=191;
		public const int IIf=192;

		public const int Like=193;
		public const int LikeNoCase=194;

		public const int Select=195;
		public const int Default=196;
		public const int LeadsTo=197;
	}
}

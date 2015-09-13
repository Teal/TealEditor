using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.CSharp
{
	public static class Tokens
	{
		public const int EOF = 0;

		public const int Identifier = 1;

		public const int Literal = 2;

		public const int Assign = 3;

		public const int Plus = 4;

		public const int Minus = 5;

		public const int Times = 6;

		public const int Div = 7;

		public const int Mod = 8;

		public const int Colon = 9;

		public const int DoubleColon = 10;

		public const int Semicolon = 11;

		public const int Question = 12;

		public const int DoubleQuestion = 13;

		public const int Comma = 14;

		public const int Dot = 15;

		public const int OpenCurlyBrace = 16;

		public const int CloseCurlyBrace = 17;

		public const int OpenSquareBracket = 18;

		public const int CloseSquareBracket = 19;

		public const int OpenParenthesis = 20;

		public const int CloseParenthesis = 21;

		public const int GreaterThan = 22;

		public const int LessThan = 23;

		public const int Not = 24;

		public const int LogicalAnd = 25;

		public const int LogicalOr = 26;

		public const int BitwiseComplement = 27;

		public const int BitwiseAnd = 28;

		public const int BitwiseOr = 29;

		public const int Xor = 30;

		public const int Increment = 31;

		public const int Decrement = 32;

		public const int Equal = 33;

		public const int NotEqual = 34;

		public const int GreaterEqual = 35;

		public const int LessEqual = 36;

		public const int ShiftLeft = 37;

		public const int PlusAssign = 38;

		public const int MinusAssign = 39;

		public const int TimesAssign = 40;

		public const int DivAssign = 41;

		public const int ModAssign = 42;

		public const int BitwiseAndAssign = 43;

		public const int BitwiseOrAssign = 44;

		public const int XorAssign = 45;

		public const int ShiftLeftAssign = 46;

		public const int Pointer = 47;

		public const int Abstract = 48;

		public const int As = 49;

		public const int Base = 50;

		public const int Bool = 51;

		public const int Break = 52;

		public const int Byte = 53;

		public const int Case = 54;

		public const int Catch = 55;

		public const int Char = 56;

		public const int Checked = 57;

		public const int Class = 58;

		public const int Const = 59;

		public const int Continue = 60;

		public const int Decimal = 61;

		public const int Default = 62;

		public const int Delegate = 63;

		public const int Do = 64;

		public const int Double = 65;

		public const int Else = 66;

		public const int Enum = 67;

		public const int Event = 68;

		public const int Explicit = 69;

		public const int Extern = 70;

		public const int False = 71;

		public const int Finally = 72;

		public const int Fixed = 73;

		public const int Float = 74;

		public const int For = 75;

		public const int Foreach = 76;

		public const int Goto = 77;

		public const int If = 78;

		public const int Implicit = 79;

		public const int In = 80;

		public const int Int = 81;

		public const int Interface = 82;

		public const int Internal = 83;

		public const int Is = 84;

		public const int Lock = 85;

		public const int Long = 86;

		public const int Namespace = 87;

		public const int New = 88;

		public const int Null = 89;

		public const int Object = 90;

		public const int Operator = 91;

		public const int Out = 92;

		public const int Override = 93;

		public const int Params = 94;

		public const int Private = 95;

		public const int Protected = 96;

		public const int Public = 97;

		public const int Readonly = 98;

		public const int Ref = 99;

		public const int Return = 100;

		public const int Sbyte = 101;

		public const int Sealed = 102;

		public const int Short = 103;

		public const int Sizeof = 104;

		public const int Stackalloc = 105;

		public const int Static = 106;

		public const int String = 107;

		public const int Struct = 108;

		public const int Switch = 109;

		public const int This = 110;

		public const int Throw = 111;

		public const int True = 112;

		public const int Try = 113;

		public const int Typeof = 114;

		public const int Uint = 115;

		public const int Ulong = 116;

		public const int Unchecked = 117;

		public const int Unsafe = 118;

		public const int Ushort = 119;

		public const int Using = 120;

		public const int Virtual = 121;

		public const int Void = 122;

		public const int Volatile = 123;

		public const int While = 124;

		public const int MaxToken = 125;

		public static BitArray OverloadableUnaryOp = Tokens.NewSet(new int[]
		{
			5,
			24,
			27,
			31,
			32,
			112,
			71
		});

		public static BitArray OverloadableBinaryOp = Tokens.NewSet(new int[]
		{
			4,
			5,
			6,
			7,
			8,
			28,
			29,
			30,
			37,
			33,
			34,
			22,
			23,
			35,
			36
		});

		public static BitArray TypeKW = Tokens.NewSet(new int[]
		{
			56,
			51,
			90,
			107,
			101,
			53,
			103,
			119,
			81,
			115,
			86,
			116,
			74,
			65,
			61
		});

		public static BitArray UnaryHead = Tokens.NewSet(new int[]
		{
			4,
			5,
			24,
			27,
			6,
			31,
			32,
			28
		});

		public static BitArray AssnStartOp = Tokens.NewSet(new int[]
		{
			4,
			5,
			24,
			27,
			6
		});

		public static BitArray CastFollower = Tokens.NewSet(new int[]
		{
			1,
			2,
			20,
			88,
			110,
			50,
			89,
			57,
			117,
			114,
			104,
			63,
			5,
			24,
			27,
			31,
			32,
			112,
			71,
			4,
			5,
			24,
			27,
			6,
			31,
			32,
			28
		});

		public static BitArray AssgnOps = Tokens.NewSet(new int[]
		{
			3,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			46
		});

		public static BitArray UnaryOp = Tokens.NewSet(new int[]
		{
			4,
			5,
			24,
			27,
			6,
			31,
			32,
			28
		});

		public static BitArray TypeDeclarationKW = Tokens.NewSet(new int[]
		{
			58,
			82,
			108,
			67,
			63
		});

		private static string[] tokenList = new string[]
		{
			"<EOF>",
			"<Identifier>",
			"<Literal>",
			"=",
			"+",
			"-",
			"*",
			"/",
			"%",
			":",
			"::",
			";",
			"?",
			"??",
			",",
			".",
			"{",
			"}",
			"[",
			"]",
			"(",
			")",
			">",
			"<",
			"!",
			"&&",
			"||",
			"~",
			"&",
			"|",
			"^",
			"++",
			"--",
			"==",
			"!=",
			">=",
			"<=",
			"<<",
			"+=",
			"-=",
			"*=",
			"/=",
			"%=",
			"&=",
			"|=",
			"^=",
			"<<=",
			"->",
			"abstract",
			"as",
			"base",
			"bool",
			"break",
			"byte",
			"case",
			"catch",
			"char",
			"checked",
			"class",
			"const",
			"continue",
			"decimal",
			"default",
			"delegate",
			"do",
			"double",
			"else",
			"enum",
			"event",
			"explicit",
			"extern",
			"false",
			"finally",
			"fixed",
			"float",
			"for",
			"foreach",
			"goto",
			"if",
			"implicit",
			"in",
			"int",
			"interface",
			"internal",
			"is",
			"lock",
			"long",
			"namespace",
			"new",
			"null",
			"object",
			"operator",
			"out",
			"override",
			"params",
			"private",
			"protected",
			"public",
			"readonly",
			"ref",
			"return",
			"sbyte",
			"sealed",
			"short",
			"sizeof",
			"stackalloc",
			"static",
			"string",
			"struct",
			"switch",
			"this",
			"throw",
			"true",
			"try",
			"typeof",
			"uint",
			"ulong",
			"unchecked",
			"unsafe",
			"ushort",
			"using",
			"virtual",
			"void",
			"volatile",
			"while"
		};

		private static BitArray NewSet(params int[] values)
		{
			BitArray bitArray = new BitArray(125);
			for (int i = 0; i < values.Length; i++)
			{
				int val = values[i];
				bitArray[val] = true;
			}
			return bitArray;
		}

		public static string GetTokenString(int token)
		{
			if (token >= 0 && token < Tokens.tokenList.Length)
			{
				return Tokens.tokenList[token];
			}
			throw new NotSupportedException("Unknown token:" + token);
		}
	}
}

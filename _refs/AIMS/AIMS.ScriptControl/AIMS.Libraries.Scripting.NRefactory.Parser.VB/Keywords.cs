using System;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.VB
{
	public class Keywords
	{
		private static readonly string[] keywordList;

		private static LookupTable keywords;

		static Keywords()
		{
			Keywords.keywordList = new string[]
			{
				"ADDHANDLER",
				"ADDRESSOF",
				"ALIAS",
				"AND",
				"ANDALSO",
				"ANSI",
				"AS",
				"ASSEMBLY",
				"AUTO",
				"BINARY",
				"BOOLEAN",
				"BYREF",
				"BYTE",
				"BYVAL",
				"CALL",
				"CASE",
				"CATCH",
				"CBOOL",
				"CBYTE",
				"CCHAR",
				"CDATE",
				"CDBL",
				"CDEC",
				"CHAR",
				"CINT",
				"CLASS",
				"CLNG",
				"COBJ",
				"COMPARE",
				"CONST",
				"CSHORT",
				"CSNG",
				"CSTR",
				"CTYPE",
				"DATE",
				"DECIMAL",
				"DECLARE",
				"DEFAULT",
				"DELEGATE",
				"DIM",
				"DIRECTCAST",
				"DO",
				"DOUBLE",
				"EACH",
				"ELSE",
				"ELSEIF",
				"END",
				"ENDIF",
				"ENUM",
				"ERASE",
				"ERROR",
				"EVENT",
				"EXIT",
				"EXPLICIT",
				"FALSE",
				"FINALLY",
				"FOR",
				"FRIEND",
				"FUNCTION",
				"GET",
				"GETTYPE",
				"GOSUB",
				"GOTO",
				"HANDLES",
				"IF",
				"IMPLEMENTS",
				"IMPORTS",
				"IN",
				"INHERITS",
				"INTEGER",
				"INTERFACE",
				"IS",
				"LET",
				"LIB",
				"LIKE",
				"LONG",
				"LOOP",
				"ME",
				"MOD",
				"MODULE",
				"MUSTINHERIT",
				"MUSTOVERRIDE",
				"MYBASE",
				"MYCLASS",
				"NAMESPACE",
				"NEW",
				"NEXT",
				"NOT",
				"NOTHING",
				"NOTINHERITABLE",
				"NOTOVERRIDABLE",
				"OBJECT",
				"OFF",
				"ON",
				"OPTION",
				"OPTIONAL",
				"OR",
				"ORELSE",
				"OVERLOADS",
				"OVERRIDABLE",
				"OVERRIDES",
				"PARAMARRAY",
				"PRESERVE",
				"PRIVATE",
				"PROPERTY",
				"PROTECTED",
				"PUBLIC",
				"RAISEEVENT",
				"READONLY",
				"REDIM",
				"REMOVEHANDLER",
				"RESUME",
				"RETURN",
				"SELECT",
				"SET",
				"SHADOWS",
				"SHARED",
				"SHORT",
				"SINGLE",
				"STATIC",
				"STEP",
				"STOP",
				"STRICT",
				"STRING",
				"STRUCTURE",
				"SUB",
				"SYNCLOCK",
				"TEXT",
				"THEN",
				"THROW",
				"TO",
				"TRUE",
				"TRY",
				"TYPEOF",
				"UNICODE",
				"UNTIL",
				"VARIANT",
				"WEND",
				"WHEN",
				"WHILE",
				"WITH",
				"WITHEVENTS",
				"WRITEONLY",
				"XOR",
				"CONTINUE",
				"OPERATOR",
				"USING",
				"ISNOT",
				"SBYTE",
				"UINTEGER",
				"ULONG",
				"USHORT",
				"CSBYTE",
				"CUSHORT",
				"CUINT",
				"CULNG",
				"GLOBAL",
				"TRYCAST",
				"OF",
				"NARROWING",
				"WIDENING",
				"PARTIAL",
				"CUSTOM"
			};
			Keywords.keywords = new LookupTable(false);
			for (int i = 0; i < Keywords.keywordList.Length; i++)
			{
				Keywords.keywords[Keywords.keywordList[i]] = i + 42;
			}
		}

		public static int GetToken(string keyword)
		{
			return Keywords.keywords[keyword];
		}
	}
}
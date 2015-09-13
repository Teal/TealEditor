using System;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.CSharp
{
	public static class Keywords
	{
		private static readonly string[] keywordList;

		private static LookupTable keywords;

		static Keywords()
		{
			Keywords.keywordList = new string[]
			{
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
			Keywords.keywords = new LookupTable(true);
			for (int i = 0; i < Keywords.keywordList.Length; i++)
			{
				Keywords.keywords[Keywords.keywordList[i]] = i + 48;
			}
		}

		public static int GetToken(string keyword)
		{
			return Keywords.keywords[keyword];
		}
	}
}

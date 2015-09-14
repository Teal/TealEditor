using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.NRefactory.Parser.CSharp;
using AIMS.Libraries.Scripting.NRefactory.Parser.VB;
using System;
using System.IO;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public static class ParserFactory
	{
		public static ILexer CreateLexer(SupportedLanguage language, TextReader textReader)
		{
			ILexer result;
			switch (language)
			{
			case SupportedLanguage.CSharp:
				result = new AIMS.Libraries.Scripting.NRefactory.Parser.CSharp.Lexer(textReader);
				break;
			case SupportedLanguage.VBNet:
				result = new AIMS.Libraries.Scripting.NRefactory.Parser.VB.Lexer(textReader);
				break;
			default:
				throw new NotSupportedException(language + " not supported.");
			}
			return result;
		}

		public static IParser CreateParser(SupportedLanguage language, TextReader textReader)
		{
			ILexer lexer = ParserFactory.CreateLexer(language, textReader);
			IParser result;
			switch (language)
			{
			case SupportedLanguage.CSharp:
				result = new AIMS.Libraries.Scripting.NRefactory.Parser.CSharp.Parser(lexer);
				break;
			case SupportedLanguage.VBNet:
				result = new AIMS.Libraries.Scripting.NRefactory.Parser.VB.Parser(lexer);
				break;
			default:
				throw new NotSupportedException(language + " not supported.");
			}
			return result;
		}

		public static IParser CreateParser(string fileName)
		{
			return ParserFactory.CreateParser(fileName, Encoding.UTF8);
		}

		public static IParser CreateParser(string fileName, Encoding encoding)
		{
			string ext = Path.GetExtension(fileName);
			IParser result;
			if (ext.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
			{
				result = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StreamReader(fileName, encoding));
			}
			else if (ext.Equals(".vb", StringComparison.InvariantCultureIgnoreCase))
			{
				result = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StreamReader(fileName, encoding));
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}

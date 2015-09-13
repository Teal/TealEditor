using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.PrettyPrinter;
using AIMS.Libraries.Scripting.NRefactory.Visitors;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.Converter
{
	internal static class CodeConverter
	{
		public static string ConvertCode(string SourceCode, ScriptLanguage SourceLanguage, ScriptLanguage TargetLanguage)
		{
			string result;
			if (SourceLanguage == TargetLanguage || SourceCode.Length == 0)
			{
				result = SourceCode;
			}
			else if (SourceLanguage == ScriptLanguage.CSharp)
			{
				result = CodeConverter.ConvertCSharpCodeToVb(SourceCode);
			}
			else
			{
				result = CodeConverter.ConvertVBCodeToCSharp(SourceCode);
			}
			return result;
		}

		private static string ConvertVBCodeToCSharp(string Code)
		{
			IParser p = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(Code));
			p.Parse();
			string result;
			if (p.Errors.Count > 0)
			{
				result = Code;
			}
			else
			{
				CSharpOutputVisitor output = new CSharpOutputVisitor();
				List<ISpecial> specials = p.Lexer.SpecialTracker.CurrentSpecials;
				PreprocessingDirective.VBToCSharp(specials);
				p.CompilationUnit.AcceptVisitor(new VBNetConstructsConvertVisitor(), null);
				p.CompilationUnit.AcceptVisitor(new ToCSharpConvertVisitor(), null);
				using (SpecialNodesInserter.Install(specials, output))
				{
					output.VisitCompilationUnit(p.CompilationUnit, null);
				}
				result = output.Text;
			}
			return result;
		}

		private static string ConvertCSharpCodeToVb(string Code)
		{
			IParser p = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(Code));
			p.Parse();
			string result;
			if (p.Errors.Count > 0)
			{
				result = Code;
			}
			else
			{
				VBNetOutputVisitor output = new VBNetOutputVisitor();
				List<ISpecial> specials = p.Lexer.SpecialTracker.CurrentSpecials;
				PreprocessingDirective.CSharpToVB(specials);
				p.CompilationUnit.AcceptVisitor(new CSharpConstructsVisitor(), null);
				p.CompilationUnit.AcceptVisitor(new ToVBNetConvertVisitor(), null);
				using (SpecialNodesInserter.Install(specials, output))
				{
					output.VisitCompilationUnit(p.CompilationUnit, null);
				}
				result = output.Text;
			}
			return result;
		}
	}
}

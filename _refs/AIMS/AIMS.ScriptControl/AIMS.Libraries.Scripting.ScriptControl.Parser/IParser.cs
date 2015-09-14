using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.ScriptControl.Project;
using System;

namespace AIMS.Libraries.Scripting.ScriptControl.Parser
{
	public interface IParser
	{
		string[] LexerTags
		{
			get;
			set;
		}

		LanguageProperties Language
		{
			get;
		}

		Errors LastErrors
		{
			get;
		}

		IExpressionFinder CreateExpressionFinder(string fileName);

		bool CanParse(string fileName);

		bool CanParse(IProject project);

		ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent);

		IResolver CreateResolver();
	}
}

using AIMS.Libraries.Scripting.NRefactory.Parser;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public interface IOutputAstVisitor : IAstVisitor
	{
		NodeTracker NodeTracker
		{
			get;
		}

		string Text
		{
			get;
		}

		Errors Errors
		{
			get;
		}

		AbstractPrettyPrintOptions Options
		{
			get;
		}

		IOutputFormatter OutputFormatter
		{
			get;
		}
	}
}

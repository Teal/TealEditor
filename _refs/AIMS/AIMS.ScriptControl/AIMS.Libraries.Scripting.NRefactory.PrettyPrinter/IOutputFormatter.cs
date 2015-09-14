using System;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public interface IOutputFormatter
	{
		int IndentationLevel
		{
			get;
			set;
		}

		string Text
		{
			get;
		}

		void NewLine();

		void Indent();

		void PrintComment(Comment comment, bool forceWriteInPreviousBlock);

		void PrintPreprocessingDirective(PreprocessingDirective directive, bool forceWriteInPreviousBlock);

		void PrintBlankLine(bool forceWriteInPreviousBlock);
	}
}

using System;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public class SpecialOutputVisitor : ISpecialVisitor
	{
		private IOutputFormatter formatter;

		public bool ForceWriteInPreviousLine;

		public SpecialOutputVisitor(IOutputFormatter formatter)
		{
			this.formatter = formatter;
		}

		public object Visit(ISpecial special, object data)
		{
			Console.WriteLine("Warning: SpecialOutputVisitor.Visit(ISpecial) called with " + special);
			return data;
		}

		public object Visit(BlankLine special, object data)
		{
			this.formatter.PrintBlankLine(this.ForceWriteInPreviousLine);
			return data;
		}

		public object Visit(Comment special, object data)
		{
			this.formatter.PrintComment(special, this.ForceWriteInPreviousLine);
			return data;
		}

		public object Visit(PreprocessingDirective special, object data)
		{
			this.formatter.PrintPreprocessingDirective(special, this.ForceWriteInPreviousLine);
			return data;
		}
	}
}

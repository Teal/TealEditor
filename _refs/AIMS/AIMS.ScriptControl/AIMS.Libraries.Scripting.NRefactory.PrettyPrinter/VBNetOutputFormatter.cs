using AIMS.Libraries.Scripting.NRefactory.Parser.VB;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public sealed class VBNetOutputFormatter : AbstractOutputFormatter
	{
		public VBNetOutputFormatter(VBNetPrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
		{
		}

		public override void PrintToken(int token)
		{
			base.PrintText(Tokens.GetTokenString(token));
		}

		public override void PrintIdentifier(string identifier)
		{
			int token = Keywords.GetToken(identifier);
			if (token < 0 || Tokens.Unreserved[token])
			{
				base.PrintText(identifier);
			}
			else
			{
				base.PrintText("[");
				base.PrintText(identifier);
				base.PrintText("]");
			}
		}

		public override void PrintComment(Comment comment, bool forceWriteInPreviousBlock)
		{
			switch (comment.CommentType)
			{
			case CommentType.Block:
				base.WriteLineInPreviousLine("'" + comment.CommentText.Replace("\n", "\n'"), forceWriteInPreviousBlock);
				return;
			case CommentType.Documentation:
				base.WriteLineInPreviousLine("'''" + comment.CommentText, forceWriteInPreviousBlock);
				return;
			}
			base.WriteLineInPreviousLine("'" + comment.CommentText, forceWriteInPreviousBlock);
		}

		public void PrintLineContinuation()
		{
			if (!base.LastCharacterIsWhiteSpace)
			{
				base.Space();
			}
			base.PrintText("_\r\n");
		}
	}
}

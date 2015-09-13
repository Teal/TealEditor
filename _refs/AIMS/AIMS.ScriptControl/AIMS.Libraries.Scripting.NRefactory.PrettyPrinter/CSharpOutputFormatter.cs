using AIMS.Libraries.Scripting.NRefactory.Parser.CSharp;
using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public sealed class CSharpOutputFormatter : AbstractOutputFormatter
	{
		private PrettyPrintOptions prettyPrintOptions;

		private bool emitSemicolon = true;

		private Stack braceStack = new Stack();

		public bool EmitSemicolon
		{
			get
			{
				return this.emitSemicolon;
			}
			set
			{
				this.emitSemicolon = value;
			}
		}

		public CSharpOutputFormatter(PrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
		{
			this.prettyPrintOptions = prettyPrintOptions;
		}

		public override void PrintToken(int token)
		{
			if (token != 11 || this.EmitSemicolon)
			{
				base.PrintText(Tokens.GetTokenString(token));
			}
		}

		public void BeginBrace(BraceStyle style)
		{
			switch (style)
			{
			case BraceStyle.EndOfLine:
				if (!base.LastCharacterIsWhiteSpace)
				{
					base.Space();
				}
				this.PrintToken(16);
				this.NewLine();
				base.IndentationLevel++;
				break;
			case BraceStyle.NextLine:
				this.NewLine();
				base.Indent();
				this.PrintToken(16);
				this.NewLine();
				base.IndentationLevel++;
				break;
			case BraceStyle.NextLineShifted:
				this.NewLine();
				base.IndentationLevel++;
				base.Indent();
				this.PrintToken(16);
				this.NewLine();
				break;
			case BraceStyle.NextLineShifted2:
				this.NewLine();
				base.IndentationLevel++;
				base.Indent();
				this.PrintToken(16);
				this.NewLine();
				base.IndentationLevel++;
				break;
			}
			this.braceStack.Push(style);
		}

		public void EndBrace()
		{
			switch ((BraceStyle)this.braceStack.Pop())
			{
			case BraceStyle.EndOfLine:
			case BraceStyle.NextLine:
				base.IndentationLevel--;
				base.Indent();
				this.PrintToken(17);
				this.NewLine();
				break;
			case BraceStyle.NextLineShifted:
				base.Indent();
				this.PrintToken(17);
				this.NewLine();
				base.IndentationLevel--;
				break;
			case BraceStyle.NextLineShifted2:
				base.IndentationLevel--;
				base.Indent();
				this.PrintToken(17);
				this.NewLine();
				base.IndentationLevel--;
				break;
			}
		}

		public override void PrintIdentifier(string identifier)
		{
			if (Keywords.GetToken(identifier) >= 0)
			{
				base.PrintText("@");
			}
			base.PrintText(identifier);
		}

		public override void PrintComment(Comment comment, bool forceWriteInPreviousBlock)
		{
			switch (comment.CommentType)
			{
			case CommentType.Block:
				if (forceWriteInPreviousBlock)
				{
					base.WriteInPreviousLine("/*" + comment.CommentText + "*/", forceWriteInPreviousBlock);
				}
				else
				{
					base.PrintSpecialText("/*" + comment.CommentText + "*/");
				}
				return;
			case CommentType.Documentation:
				base.WriteLineInPreviousLine("///" + comment.CommentText, forceWriteInPreviousBlock);
				return;
			}
			base.WriteLineInPreviousLine("//" + comment.CommentText, forceWriteInPreviousBlock);
		}
	}
}

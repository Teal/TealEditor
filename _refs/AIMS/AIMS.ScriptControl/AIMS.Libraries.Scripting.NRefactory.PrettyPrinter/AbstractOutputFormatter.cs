using System;
using System.Collections;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public abstract class AbstractOutputFormatter : IOutputFormatter
	{
		private StringBuilder text = new StringBuilder();

		private int indentationLevel = 0;

		private bool indent = true;

		private bool doNewLine = true;

		private AbstractPrettyPrintOptions prettyPrintOptions;

		internal int lastLineStart = 0;

		internal int lineBeforeLastStart = 0;

		public int IndentationLevel
		{
			get
			{
				return this.indentationLevel;
			}
			set
			{
				this.indentationLevel = value;
			}
		}

		public string Text
		{
			get
			{
				return this.text.ToString();
			}
		}

		public int TextLength
		{
			get
			{
				return this.text.Length;
			}
		}

		public bool DoIndent
		{
			get
			{
				return this.indent;
			}
			set
			{
				this.indent = value;
			}
		}

		public bool DoNewLine
		{
			get
			{
				return this.doNewLine;
			}
			set
			{
				this.doNewLine = value;
			}
		}

		public bool LastCharacterIsNewLine
		{
			get
			{
				return this.text.Length == this.lastLineStart;
			}
		}

		public bool LastCharacterIsWhiteSpace
		{
			get
			{
				return this.text.Length == 0 || char.IsWhiteSpace(this.text[this.text.Length - 1]);
			}
		}

		protected AbstractOutputFormatter(AbstractPrettyPrintOptions prettyPrintOptions)
		{
			this.prettyPrintOptions = prettyPrintOptions;
		}

		public void Indent()
		{
			if (this.DoIndent)
			{
				int indent = 0;
				while (indent < this.prettyPrintOptions.IndentSize * this.indentationLevel)
				{
					char ch = this.prettyPrintOptions.IndentationChar;
					if (ch == '\t' && indent + this.prettyPrintOptions.TabSize > this.prettyPrintOptions.IndentSize * this.indentationLevel)
					{
						ch = ' ';
					}
					this.text.Append(ch);
					if (ch == '\t')
					{
						indent += this.prettyPrintOptions.TabSize;
					}
					else
					{
						indent++;
					}
				}
			}
		}

		public void Space()
		{
			this.text.Append(' ');
		}

		public virtual void NewLine()
		{
			if (this.DoNewLine)
			{
				if (!this.LastCharacterIsNewLine)
				{
					this.lineBeforeLastStart = this.lastLineStart;
				}
				this.text.AppendLine();
				this.lastLineStart = this.text.Length;
			}
		}

		public virtual void EndFile()
		{
		}

		protected void WriteLineInPreviousLine(string txt, bool forceWriteInPreviousBlock)
		{
			this.WriteInPreviousLine(txt + Environment.NewLine, forceWriteInPreviousBlock);
		}

		protected void WriteInPreviousLine(string txt, bool forceWriteInPreviousBlock)
		{
			if (txt.Length != 0)
			{
				bool lastCharacterWasNewLine = this.LastCharacterIsNewLine;
				if (lastCharacterWasNewLine)
				{
					if (!forceWriteInPreviousBlock)
					{
						if (txt != Environment.NewLine)
						{
							this.Indent();
						}
						this.text.Append(txt);
						this.lineBeforeLastStart = this.lastLineStart;
						this.lastLineStart = this.text.Length;
						return;
					}
					this.lastLineStart = this.lineBeforeLastStart;
				}
				string lastLine = this.text.ToString(this.lastLineStart, this.text.Length - this.lastLineStart);
				this.text.Remove(this.lastLineStart, this.text.Length - this.lastLineStart);
				if (txt != Environment.NewLine)
				{
					if (forceWriteInPreviousBlock)
					{
						this.indentationLevel++;
					}
					this.Indent();
					if (forceWriteInPreviousBlock)
					{
						this.indentationLevel--;
					}
				}
				this.text.Append(txt);
				this.lineBeforeLastStart = this.lastLineStart;
				this.lastLineStart = this.text.Length;
				this.text.Append(lastLine);
				if (lastCharacterWasNewLine)
				{
					this.lineBeforeLastStart = this.lastLineStart;
					this.lastLineStart = this.text.Length;
				}
			}
		}

		protected void PrintSpecialText(string specialText)
		{
			this.lineBeforeLastStart = this.text.Length;
			this.text.Append(specialText);
			this.lastLineStart = this.text.Length;
		}

		public void PrintTokenList(ArrayList tokenList)
		{
			foreach (int token in tokenList)
			{
				this.PrintToken(token);
				this.Space();
			}
		}

		public abstract void PrintComment(Comment comment, bool forceWriteInPreviousBlock);

		public virtual void PrintPreprocessingDirective(PreprocessingDirective directive, bool forceWriteInPreviousBlock)
		{
			if (string.IsNullOrEmpty(directive.Arg))
			{
				this.WriteLineInPreviousLine(directive.Cmd, forceWriteInPreviousBlock);
			}
			else
			{
				this.WriteLineInPreviousLine(directive.Cmd + " " + directive.Arg, forceWriteInPreviousBlock);
			}
		}

		public void PrintBlankLine(bool forceWriteInPreviousBlock)
		{
			this.WriteInPreviousLine(Environment.NewLine, forceWriteInPreviousBlock);
		}

		public abstract void PrintToken(int token);

		public void PrintText(string text)
		{
			this.text.Append(text);
		}

		public abstract void PrintIdentifier(string identifier);
	}
}

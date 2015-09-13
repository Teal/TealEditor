using System;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public class AbstractPrettyPrintOptions
	{
		private char indentationChar = '\t';

		private int tabSize = 4;

		private int indentSize = 4;

		public char IndentationChar
		{
			get
			{
				return this.indentationChar;
			}
			set
			{
				this.indentationChar = value;
			}
		}

		public int TabSize
		{
			get
			{
				return this.tabSize;
			}
			set
			{
				this.tabSize = value;
			}
		}

		public int IndentSize
		{
			get
			{
				return this.indentSize;
			}
			set
			{
				this.indentSize = value;
			}
		}
	}
}

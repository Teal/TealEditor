using System;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	public class Token
	{
		public int kind;

		public int col;

		public int line;

		public object literalValue;

		public string val;

		public Token next;

		public Location EndLocation
		{
			get
			{
				return new Location((this.val == null) ? (this.col + 1) : (this.col + this.val.Length), this.line);
			}
		}

		public Location Location
		{
			get
			{
				return new Location(this.col, this.line);
			}
		}

		public Token(int kind) : this(kind, 0, 0)
		{
		}

		public Token(int kind, int col, int line) : this(kind, col, line, null)
		{
		}

		public Token(int kind, int col, int line, string val) : this(kind, col, line, val, null)
		{
		}

		public Token(int kind, int col, int line, string val, object literalValue)
		{
			this.kind = kind;
			this.col = col;
			this.line = line;
			this.val = val;
			this.literalValue = literalValue;
		}
	}
}

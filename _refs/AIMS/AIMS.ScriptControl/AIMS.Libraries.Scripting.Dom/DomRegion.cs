using AIMS.Libraries.Scripting.NRefactory;
using System;

namespace AIMS.Libraries.Scripting.Dom
{
	[Serializable]
	public struct DomRegion : IComparable, IComparable<DomRegion>
	{
		private readonly int beginLine;

		private readonly int endLine;

		private readonly int beginColumn;

		private readonly int endColumn;

		public static readonly DomRegion Empty = new DomRegion(-1, -1);

		public bool IsEmpty
		{
			get
			{
				return this.BeginLine <= 0;
			}
		}

		public int BeginLine
		{
			get
			{
				return this.beginLine;
			}
		}

		public int EndLine
		{
			get
			{
				return this.endLine;
			}
		}

		public int BeginColumn
		{
			get
			{
				return this.beginColumn;
			}
		}

		public int EndColumn
		{
			get
			{
				return this.endColumn;
			}
		}

		public DomRegion(Location start, Location end)
		{
			this = new DomRegion(start.Y, start.X, end.Y, end.X);
		}

		public DomRegion(int beginLine, int beginColumn, int endLine, int endColumn)
		{
			this.beginLine = beginLine;
			this.beginColumn = beginColumn;
			this.endLine = endLine;
			this.endColumn = endColumn;
		}

		public DomRegion(int beginLine, int beginColumn)
		{
			this.beginLine = beginLine;
			this.beginColumn = beginColumn;
			this.endLine = -1;
			this.endColumn = -1;
		}

		public bool IsInside(int row, int column)
		{
			return !this.IsEmpty && (row >= this.BeginLine && (row <= this.EndLine || this.EndLine == -1) && (row != this.BeginLine || column >= this.BeginColumn)) && (row != this.EndLine || column <= this.EndColumn);
		}

		public override string ToString()
		{
			return string.Format("[Region: BeginLine = {0}, EndLine = {1}, BeginColumn = {2}, EndColumn = {3}]", new object[]
			{
				this.beginLine,
				this.endLine,
				this.beginColumn,
				this.endColumn
			});
		}

		public int CompareTo(DomRegion value)
		{
			int cmp;
			int result;
			if (0 != (cmp = this.BeginLine - value.BeginLine))
			{
				result = cmp;
			}
			else if (0 != (cmp = this.BeginColumn - value.BeginColumn))
			{
				result = cmp;
			}
			else if (0 != (cmp = this.EndLine - value.EndLine))
			{
				result = cmp;
			}
			else
			{
				result = this.EndColumn - value.EndColumn;
			}
			return result;
		}

		int IComparable.CompareTo(object value)
		{
			return this.CompareTo((DomRegion)value);
		}
	}
}

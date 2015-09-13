using AIMS.Libraries.Scripting.NRefactory;
using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public struct FilePosition
	{
		private string filename;

		private Location position;

		private ICompilationUnit compilationUnit;

		public static readonly FilePosition Empty = new FilePosition(null, Location.Empty);

		public string FileName
		{
			get
			{
				return this.filename;
			}
		}

		public ICompilationUnit CompilationUnit
		{
			get
			{
				return this.compilationUnit;
			}
		}

		public Location Position
		{
			get
			{
				return this.position;
			}
		}

		public int Line
		{
			get
			{
				return this.position.Y;
			}
		}

		public int Column
		{
			get
			{
				return this.position.X;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.filename == null;
			}
		}

		public FilePosition(ICompilationUnit compilationUnit, int line, int column)
		{
			this.position = new Location(column, line);
			this.compilationUnit = compilationUnit;
			if (compilationUnit != null)
			{
				this.filename = compilationUnit.FileName;
			}
			else
			{
				this.filename = null;
			}
		}

		public FilePosition(string filename)
		{
			this = new FilePosition(filename, Location.Empty);
		}

		public FilePosition(string filename, int line, int column)
		{
			this = new FilePosition(filename, new Location(column, line));
		}

		public FilePosition(string filename, Location position)
		{
			this.compilationUnit = null;
			this.filename = filename;
			this.position = position;
		}

		public override string ToString()
		{
			return string.Format("{0} : (line {1}, col {2})", this.filename, this.Line, this.Column);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (!(obj is FilePosition))
			{
				result = false;
			}
			else
			{
				FilePosition b = (FilePosition)obj;
				result = (this.FileName == b.FileName && this.Position == b.Position);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.filename.GetHashCode() ^ this.position.GetHashCode();
		}
	}
}

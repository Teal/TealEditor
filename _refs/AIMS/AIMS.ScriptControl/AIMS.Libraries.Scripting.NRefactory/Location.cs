using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public struct Location : IComparable<Location>, IEquatable<Location>
	{
		public static readonly Location Empty = new Location(-1, -1);

		private int x;

		private int y;

		public int X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		public int Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public int Line
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public int Column
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.x <= 0 && this.y <= 0;
			}
		}

		public Location(int column, int line)
		{
			this.x = column;
			this.y = line;
		}

		public override string ToString()
		{
			return string.Format("(Line {1}, Col {0})", this.x, this.y);
		}

		public override int GetHashCode()
		{
			return 87 * this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Location && (Location)obj == this;
		}

		public bool Equals(Location other)
		{
			return this == other;
		}

		public static bool operator ==(Location a, Location b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Location a, Location b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public static bool operator <(Location a, Location b)
		{
			return a.y < b.y || (a.y == b.y && a.x < b.x);
		}

		public static bool operator >(Location a, Location b)
		{
			return a.y > b.y || (a.y == b.y && a.x > b.x);
		}

		public static bool operator <=(Location a, Location b)
		{
			return !(a > b);
		}

		public static bool operator >=(Location a, Location b)
		{
			return !(a < b);
		}

		public int CompareTo(Location other)
		{
			int result;
			if (this == other)
			{
				result = 0;
			}
			else if (this < other)
			{
				result = -1;
			}
			else
			{
				result = 1;
			}
			return result;
		}
	}
}

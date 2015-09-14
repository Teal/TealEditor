using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public abstract class AbstractSpecial : ISpecial
	{
		private Location startPosition;

		private Location endPosition;

		public Location StartPosition
		{
			get
			{
				return this.startPosition;
			}
			set
			{
				this.startPosition = value;
			}
		}

		public Location EndPosition
		{
			get
			{
				return this.endPosition;
			}
			set
			{
				this.endPosition = value;
			}
		}

		public abstract object AcceptVisitor(ISpecialVisitor visitor, object data);

		protected AbstractSpecial(Location position)
		{
			this.startPosition = position;
			this.endPosition = position;
		}

		protected AbstractSpecial(Location startPosition, Location endPosition)
		{
			this.startPosition = startPosition;
			this.endPosition = endPosition;
		}

		public override string ToString()
		{
			return string.Format("[{0}: Start = {1}, End = {2}]", base.GetType().Name, this.StartPosition, this.EndPosition);
		}
	}
}

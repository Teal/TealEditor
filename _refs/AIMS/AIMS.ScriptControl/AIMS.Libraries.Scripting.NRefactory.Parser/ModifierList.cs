using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	internal class ModifierList
	{
		private Modifiers cur;

		private Location location = new Location(-1, -1);

		public Modifiers Modifier
		{
			get
			{
				return this.cur;
			}
		}

		public bool isNone
		{
			get
			{
				return this.cur == Modifiers.None;
			}
		}

		public Location GetDeclarationLocation(Location keywordLocation)
		{
			Location result;
			if (this.location.X == -1 && this.location.Y == -1)
			{
				result = keywordLocation;
			}
			else
			{
				result = this.location;
			}
			return result;
		}

		public bool Contains(Modifiers m)
		{
			return (this.cur & m) != Modifiers.None;
		}

		public void Add(Modifiers m, Location tokenLocation)
		{
			if (this.location.X == -1 && this.location.Y == -1)
			{
				this.location = tokenLocation;
			}
			if ((this.cur & m) == Modifiers.None)
			{
				this.cur |= m;
			}
		}

		public void Check(Modifiers allowed)
		{
			Modifiers wrong = this.cur & ~allowed;
			if (wrong != Modifiers.None)
			{
			}
		}
	}
}

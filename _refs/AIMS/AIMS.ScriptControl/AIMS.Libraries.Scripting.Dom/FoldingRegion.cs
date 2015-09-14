using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class FoldingRegion
	{
		private string name;

		private DomRegion region;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public DomRegion Region
		{
			get
			{
				return this.region;
			}
		}

		public FoldingRegion(string name, DomRegion region)
		{
			this.name = name;
			this.region = region;
		}
	}
}

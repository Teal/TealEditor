using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class TagComment
	{
		private string key;

		private string commentString;

		private DomRegion region;

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public string CommentString
		{
			get
			{
				return this.commentString;
			}
			set
			{
				this.commentString = value;
			}
		}

		public DomRegion Region
		{
			get
			{
				return this.region;
			}
			set
			{
				this.region = value;
			}
		}

		public TagComment(string key, DomRegion region)
		{
			this.key = key;
			this.region = region;
		}
	}
}

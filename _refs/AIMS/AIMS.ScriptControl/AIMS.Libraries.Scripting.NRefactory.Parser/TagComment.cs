using System;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	public class TagComment : Comment
	{
		private string tag;

		public string Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		public TagComment(string tag, string comment, Location startPosition, Location endPosition) : base(CommentType.SingleLine, comment, startPosition, endPosition)
		{
			this.tag = tag;
		}
	}
}

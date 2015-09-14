using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultComment : IComment
	{
		private bool isBlockComment;

		private string commentTag;

		private string commentText;

		private DomRegion region;

		public bool IsBlockComment
		{
			get
			{
				return this.isBlockComment;
			}
		}

		public string CommentTag
		{
			get
			{
				return this.commentTag;
			}
		}

		public string CommentText
		{
			get
			{
				return this.commentText;
			}
		}

		public DomRegion Region
		{
			get
			{
				return this.region;
			}
		}

		public DefaultComment(bool isBlockComment, string commentTag, string commentText, DomRegion region)
		{
			this.isBlockComment = isBlockComment;
			this.commentTag = commentTag;
			this.commentText = commentText;
			this.region = region;
		}
	}
}

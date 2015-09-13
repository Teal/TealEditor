using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public class Comment : AbstractSpecial
	{
		private CommentType commentType;

		private string comment;

		public CommentType CommentType
		{
			get
			{
				return this.commentType;
			}
			set
			{
				this.commentType = value;
			}
		}

		public string CommentText
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		public Comment(CommentType commentType, string comment, Location startPosition, Location endPosition) : base(startPosition, endPosition)
		{
			this.commentType = commentType;
			this.comment = comment;
		}

		public override string ToString()
		{
			return string.Format("[{0}: Type = {1}, Text = {2}, Start = {3}, End = {4}]", new object[]
			{
				base.GetType().Name,
				this.CommentType,
				this.CommentText,
				base.StartPosition,
				base.EndPosition
			});
		}

		public override object AcceptVisitor(ISpecialVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}

using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IComment
	{
		bool IsBlockComment
		{
			get;
		}

		string CommentTag
		{
			get;
		}

		string CommentText
		{
			get;
		}

		DomRegion Region
		{
			get;
		}
	}
}

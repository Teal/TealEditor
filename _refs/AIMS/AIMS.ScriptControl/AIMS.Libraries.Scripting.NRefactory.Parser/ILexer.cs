using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	public interface ILexer : IDisposable
	{
		Errors Errors
		{
			get;
		}

		Token Token
		{
			get;
		}

		Token LookAhead
		{
			get;
		}

		string[] SpecialCommentTags
		{
			get;
			set;
		}

		bool SkipAllComments
		{
			get;
			set;
		}

		List<TagComment> TagComments
		{
			get;
		}

		SpecialTracker SpecialTracker
		{
			get;
		}

		void StartPeek();

		Token Peek();

		Token NextToken();

		void SkipCurrentBlock(int targetToken);
	}
}

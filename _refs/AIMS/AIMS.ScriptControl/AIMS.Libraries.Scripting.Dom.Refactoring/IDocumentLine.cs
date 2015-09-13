using System;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public interface IDocumentLine
	{
		int Offset
		{
			get;
		}

		int Length
		{
			get;
		}

		string Text
		{
			get;
		}
	}
}

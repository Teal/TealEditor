using System;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public interface IDocument
	{
		int TextLength
		{
			get;
		}

		int TotalNumberOfLines
		{
			get;
		}

		IDocumentLine GetLine(int lineNumber);

		int PositionToOffset(int line, int column);

		void Insert(int offset, string text);

		void Remove(int offset, int length);

		char GetCharAt(int offset);

		void StartUndoableAction();

		void EndUndoableAction();

		void UpdateView();
	}
}

using AIMS.Libraries.CodeEditor;
using System;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
	public interface ICodeCompletionBinding
	{
		bool HandleKeyPress(CodeEditorControl editor, char ch);
	}
}

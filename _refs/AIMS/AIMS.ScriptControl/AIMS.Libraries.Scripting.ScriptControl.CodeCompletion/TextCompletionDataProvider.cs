using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using System;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class TextCompletionDataProvider : AbstractCompletionDataProvider
	{
		private string[] texts;

		public TextCompletionDataProvider(params string[] texts)
		{
			this.texts = texts;
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
		{
			ICompletionData[] data = new ICompletionData[this.texts.Length];
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = new DefaultCompletionData(this.texts[i], null, AutoListIcons.iClassShortCut);
			}
			return data;
		}
	}
}

using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using System;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public abstract class AbstractCompletionDataProvider : ICompletionDataProvider
	{
		private int defaultIndex = -1;

		protected string preSelection = null;

		private bool insertSpace;

		public virtual ImageList ImageList
		{
			get
			{
				return null;
			}
		}

		public int DefaultIndex
		{
			get
			{
				return this.defaultIndex;
			}
			set
			{
				this.defaultIndex = value;
			}
		}

		public string PreSelection
		{
			get
			{
				return this.preSelection;
			}
		}

		public bool InsertSpace
		{
			get
			{
				return this.insertSpace;
			}
			set
			{
				this.insertSpace = value;
			}
		}

		public virtual CompletionDataProviderKeyResult ProcessKey(char key)
		{
			CompletionDataProviderKeyResult res;
			if (key == ' ' && this.insertSpace)
			{
				this.insertSpace = false;
				res = CompletionDataProviderKeyResult.BeforeStartKey;
			}
			else if (char.IsLetterOrDigit(key) || key == '_')
			{
				this.insertSpace = false;
				res = CompletionDataProviderKeyResult.NormalKey;
			}
			else
			{
				res = CompletionDataProviderKeyResult.InsertionKey;
			}
			return res;
		}

		public virtual bool InsertAction(ICompletionData data, EditViewControl textArea, int insertionOffset, char key)
		{
			if (this.InsertSpace)
			{
				TextPoint loc = textArea.Document.IntPosToPoint(insertionOffset++);
				textArea.Document.InsertText(" ", loc.X, loc.Y);
			}
			textArea.Caret.Position = textArea.Document.IntPosToPoint(insertionOffset);
			return data.InsertAction(textArea, key);
		}

		public abstract ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped);
	}
}

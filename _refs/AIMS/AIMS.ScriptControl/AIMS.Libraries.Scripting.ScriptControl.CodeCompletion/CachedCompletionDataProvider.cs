using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using System;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class CachedCompletionDataProvider : AbstractCompletionDataProvider
	{
		private ICompletionDataProvider baseProvider;

		private ICompletionData[] completionData;

		public ICompletionData[] CompletionData
		{
			get
			{
				return this.completionData;
			}
			set
			{
				this.completionData = value;
			}
		}

		public override ImageList ImageList
		{
			get
			{
				return this.baseProvider.ImageList;
			}
		}

		[Obsolete("Cannot use InsertSpace on CachedCompletionDataProvider, please set it on the underlying provider!")]
		public new bool InsertSpace
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public CachedCompletionDataProvider(ICompletionDataProvider baseProvider)
		{
			this.baseProvider = baseProvider;
		}

		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			return this.baseProvider.ProcessKey(key);
		}

		public override bool InsertAction(ICompletionData data, EditViewControl textArea, int insertionOffset, char key)
		{
			return this.baseProvider.InsertAction(data, textArea, insertionOffset, key);
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
		{
			if (this.completionData == null)
			{
				this.completionData = this.baseProvider.GenerateCompletionData(fileName, textArea, charTyped);
				this.preSelection = this.baseProvider.PreSelection;
				base.DefaultIndex = this.baseProvider.DefaultIndex;
			}
			return this.completionData;
		}
	}
}

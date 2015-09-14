using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using System;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
	public class DefaultCodeCompletionBinding : ICodeCompletionBinding
	{
		private bool enableMethodInsight = true;

		private bool enableIndexerInsight = true;

		private bool enableXmlCommentCompletion = true;

		private bool enableDotCompletion = true;

		public bool EnableMethodInsight
		{
			get
			{
				return this.enableMethodInsight;
			}
			set
			{
				this.enableMethodInsight = value;
			}
		}

		public bool EnableIndexerInsight
		{
			get
			{
				return this.enableIndexerInsight;
			}
			set
			{
				this.enableIndexerInsight = value;
			}
		}

		public bool EnableXmlCommentCompletion
		{
			get
			{
				return this.enableXmlCommentCompletion;
			}
			set
			{
				this.enableXmlCommentCompletion = value;
			}
		}

		public bool EnableDotCompletion
		{
			get
			{
				return this.enableDotCompletion;
			}
			set
			{
				this.enableDotCompletion = value;
			}
		}

		public virtual bool HandleKeyPress(CodeEditorControl editor, char ch)
		{
			bool result;
			if (ch <= '(')
			{
				if (ch == ' ')
				{
					string word = editor.ActiveViewControl.Caret.CurrentWord.Text;
					result = (word != null && this.HandleKeyword(editor, word));
					return result;
				}
				if (ch == '(')
				{
					if (this.enableMethodInsight)
					{
						editor.ActiveViewControl.ShowInsightWindow(new MethodInsightDataProvider());
						result = true;
						return result;
					}
					result = false;
					return result;
				}
			}
			else if (ch != '.')
			{
				if (ch != '<')
				{
					if (ch == '[')
					{
						if (this.enableIndexerInsight)
						{
							editor.ActiveViewControl.ShowInsightWindow(new IndexerInsightDataProvider());
							result = true;
							return result;
						}
						result = false;
						return result;
					}
				}
				else
				{
					if (this.enableXmlCommentCompletion)
					{
						editor.ActiveViewControl.ShowCompletionWindow(new CommentCompletionDataProvider(), ch);
						result = true;
						return result;
					}
					result = false;
					return result;
				}
			}
			else
			{
				if (this.enableDotCompletion)
				{
					editor.ActiveViewControl.ShowCompletionWindow(new CodeCompletionDataProvider(), ch);
					result = true;
					return result;
				}
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		public virtual bool HandleKeyword(CodeEditorControl editor, string word)
		{
			return false;
		}
	}
}

using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class CommentCompletionDataProvider : AbstractCompletionDataProvider
	{
		private class CommentCompletionData : ICompletionData, IComparable
		{
			private string text;

			private string description;

			public AutoListIcons ImageIndex
			{
				get
				{
					return AutoListIcons.iMethod;
				}
			}

			public string Text
			{
				get
				{
					return this.text;
				}
				set
				{
					this.text = value;
				}
			}

			public string Description
			{
				get
				{
					return this.description;
				}
			}

			public double Priority
			{
				get
				{
					return 0.0;
				}
			}

			public bool InsertAction(EditViewControl textArea, char ch)
			{
				textArea.InsertText(this.text);
				return false;
			}

			public CommentCompletionData(string text, string description)
			{
				this.text = text;
				this.description = description;
			}

			public int CompareTo(object obj)
			{
				int result;
				if (obj == null || !(obj is CommentCompletionDataProvider.CommentCompletionData))
				{
					result = -1;
				}
				else
				{
					result = this.text.CompareTo(((CommentCompletionDataProvider.CommentCompletionData)obj).text);
				}
				return result;
			}
		}

		private string[][] commentTags = new string[][]
		{
			new string[]
			{
				"c",
				"marks text as code"
			},
			new string[]
			{
				"code",
				"marks text as code"
			},
			new string[]
			{
				"example",
				"A description of the code example\n(must have a <code> tag inside)"
			},
			new string[]
			{
				"exception cref=\"\"",
				"description to an exception thrown"
			},
			new string[]
			{
				"list type=\"\"",
				"A list"
			},
			new string[]
			{
				"listheader",
				"The header from the list"
			},
			new string[]
			{
				"item",
				"A list item"
			},
			new string[]
			{
				"term",
				"A term in a list"
			},
			new string[]
			{
				"description",
				"A description to a term in a list"
			},
			new string[]
			{
				"para",
				"A text paragraph"
			},
			new string[]
			{
				"param name=\"\"",
				"A description for a parameter"
			},
			new string[]
			{
				"paramref name=\"\"",
				"A reference to a parameter"
			},
			new string[]
			{
				"permission cref=\"\"",
				""
			},
			new string[]
			{
				"remarks",
				"Gives description for a member"
			},
			new string[]
			{
				"include file=\"\" path=\"\"",
				"Includes comments from other files"
			},
			new string[]
			{
				"returns",
				"Gives description for a return value"
			},
			new string[]
			{
				"see cref=\"\"",
				"A reference to a member"
			},
			new string[]
			{
				"seealso cref=\"\"",
				"A reference to a member in the seealso section"
			},
			new string[]
			{
				"summary",
				"A summary of the object"
			},
			new string[]
			{
				"value",
				"A description of a property"
			}
		};

		private bool IsBetween(int row, int column, DomRegion region)
		{
			return row >= region.BeginLine && (row <= region.EndLine || region.EndLine == -1);
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
		{
			string lineText = textArea.Caret.CurrentRow.Text;
			ICompletionData[] result;
			if (!lineText.Trim().StartsWith("///") && !lineText.Trim().StartsWith("'''"))
			{
				result = null;
			}
			else
			{
				ArrayList completionData = new ArrayList();
				string[][] array = this.commentTags;
				for (int i = 0; i < array.Length; i++)
				{
					string[] tag = array[i];
					completionData.Add(new CommentCompletionDataProvider.CommentCompletionData(tag[0], tag[1]));
				}
				result = (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
			}
			return result;
		}
	}
}

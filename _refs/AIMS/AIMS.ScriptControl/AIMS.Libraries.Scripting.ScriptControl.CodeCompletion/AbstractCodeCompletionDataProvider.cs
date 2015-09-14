using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.VBNet;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public abstract class AbstractCodeCompletionDataProvider : AbstractCompletionDataProvider
	{
		private Hashtable insertedElements = new Hashtable();

		private Hashtable insertedPropertiesElements = new Hashtable();

		private Hashtable insertedEventElements = new Hashtable();

		protected int caretLineNumber;

		protected int caretColumn;

		protected string fileName;

		protected ArrayList completionData = null;

		protected ExpressionContext overrideContext;

		public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
		{
			this.completionData = new ArrayList();
			this.fileName = fileName;
			SyntaxDocument document = textArea.Document;
			this.caretLineNumber = textArea.Caret.Position.Y + 1;
			this.caretColumn = textArea.Caret.Position.X + 1;
			this.GenerateCompletionData(textArea, charTyped);
			string cWord = "";
			Word w = textArea._CodeEditor.Caret.CurrentWord;
			if (w != null)
			{
				cWord = w.Text;
			}
			ExpressionResult ex = this.GetExpression(textArea);
			if (charTyped == '\0' && this.completionData.Count > 0 && cWord != "." && ex.ToString().IndexOf('.') <= 0 && ex.Context == ExpressionContext.Default)
			{
				ArrayList LanguageKeyWords = textArea.ExtractKeywords();
				ArrayList org = (ArrayList)this.completionData.Clone();
				org.Sort();
				foreach (string keyword in LanguageKeyWords)
				{
					int i = org.BinarySearch(new CodeCompletionData(keyword, "", AutoListIcons.iStructure));
					if (i < 0)
					{
						i = ~i;
					}
					if (i >= 0 && i < org.Count && ((ICompletionData)org[i]).Text != keyword)
					{
						this.completionData.Add(new CodeCompletionData(keyword, "Keyword " + keyword, AutoListIcons.iOperator));
					}
				}
			}
			return (ICompletionData[])this.completionData.ToArray(typeof(ICompletionData));
		}

		protected ExpressionResult GetExpression(EditViewControl textArea)
		{
			SyntaxDocument document = textArea.Document;
			IExpressionFinder expressionFinder;
			if (ProjectParser.CurrentProjectContent.Language == LanguageProperties.CSharp)
			{
				expressionFinder = new CSharpExpressionFinder(this.fileName);
			}
			else
			{
				expressionFinder = new VBExpressionFinder();
			}
			ExpressionResult result;
			if (expressionFinder == null)
			{
				result = new ExpressionResult(textArea.Caret.CurrentWord.Text);
			}
			else
			{
				TextRange range = new TextRange(0, 0, textArea.Caret.Position.X, textArea.Caret.Position.Y);
				ExpressionResult res = expressionFinder.FindExpression(document.GetRange(range), textArea.Caret.Offset - 1);
				if (this.overrideContext != null)
				{
					res.Context = this.overrideContext;
				}
				result = res;
			}
			return result;
		}

		protected abstract void GenerateCompletionData(EditViewControl textArea, char charTyped);

		protected void AddResolveResults(ICollection list, ExpressionContext context)
		{
			if (list != null)
			{
				this.completionData.Capacity += list.Count;
				CodeCompletionData suggestedData = null;
				foreach (object o in list)
				{
					if (context == null || context.ShowEntry(o))
					{
						CodeCompletionData ccd = this.CreateItem(o, context);
						if (object.Equals(o, context.SuggestedItem))
						{
							suggestedData = ccd;
						}
						if (ccd != null && !ccd.Text.StartsWith("___"))
						{
							this.completionData.Add(ccd);
						}
					}
				}
				if (context.SuggestedItem != null)
				{
					if (suggestedData == null)
					{
						suggestedData = this.CreateItem(context.SuggestedItem, context);
						if (suggestedData != null)
						{
							this.completionData.Add(suggestedData);
						}
					}
					if (suggestedData != null)
					{
						this.completionData.Sort();
						base.DefaultIndex = this.completionData.IndexOf(suggestedData);
					}
				}
			}
		}

		private CodeCompletionData CreateItem(object o, ExpressionContext context)
		{
			CodeCompletionData result;
			if (o is string)
			{
				result = new CodeCompletionData(o.ToString(), "Namespace " + o.ToString(), AutoListIcons.iNamespace);
			}
			else if (o is IClass)
			{
				result = new CodeCompletionData((IClass)o);
			}
			else
			{
				if (o is IProperty)
				{
					IProperty property = (IProperty)o;
					if (property.Name != null && this.insertedPropertiesElements[property.Name] == null)
					{
						this.insertedPropertiesElements[property.Name] = property;
						result = new CodeCompletionData(property);
						return result;
					}
				}
				else if (o is IMethod)
				{
					IMethod method = (IMethod)o;
					if (method.Name != null && !method.IsConstructor)
					{
						CodeCompletionData ccd = new CodeCompletionData(method);
						if (this.insertedElements[method.Name] == null)
						{
							this.insertedElements[method.Name] = ccd;
							result = ccd;
							return result;
						}
						CodeCompletionData oldMethod = (CodeCompletionData)this.insertedElements[method.Name];
						oldMethod.Overloads++;
					}
				}
				else
				{
					if (o is IField)
					{
						result = new CodeCompletionData((IField)o);
						return result;
					}
					if (!(o is IEvent))
					{
						throw new ApplicationException("Unknown object: " + o);
					}
					IEvent e = (IEvent)o;
					if (e.Name != null && this.insertedEventElements[e.Name] == null)
					{
						this.insertedEventElements[e.Name] = e;
						result = new CodeCompletionData(e);
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		protected void AddResolveResults(ResolveResult results, ExpressionContext context)
		{
			this.insertedElements.Clear();
			this.insertedPropertiesElements.Clear();
			this.insertedEventElements.Clear();
			if (results != null)
			{
				this.AddResolveResults(results.GetCompletionData(ProjectParser.CurrentProjectContent), context);
			}
		}
	}
}

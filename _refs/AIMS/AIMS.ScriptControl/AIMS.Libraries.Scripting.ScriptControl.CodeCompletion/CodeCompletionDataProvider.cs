using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class CodeCompletionDataProvider : AbstractCodeCompletionDataProvider
	{
		private ExpressionResult fixedExpression;

		public CodeCompletionDataProvider()
		{
		}

		public CodeCompletionDataProvider(ExpressionResult expression)
		{
			this.fixedExpression = expression;
		}

		protected override void GenerateCompletionData(EditViewControl textArea, char charTyped)
		{
			this.preSelection = null;
			if (this.fixedExpression.Expression == null)
			{
				this.GenerateCompletionData(textArea, base.GetExpression(textArea));
			}
			else
			{
				this.GenerateCompletionData(textArea, this.fixedExpression);
			}
		}

		protected void GenerateCompletionData(EditViewControl textArea, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression != null)
			{
				string textContent = ProjectParser.GetFileContents(this.fileName);
				NRefactoryResolver rr = ProjectParser.GetResolver();
				ResolveResult r = rr.Resolve(expressionResult, this.caretLineNumber, this.caretColumn, textArea.FileName, ProjectParser.GetFileContents(this.fileName));
				base.AddResolveResults(r, expressionResult.Context);
			}
		}
	}
}

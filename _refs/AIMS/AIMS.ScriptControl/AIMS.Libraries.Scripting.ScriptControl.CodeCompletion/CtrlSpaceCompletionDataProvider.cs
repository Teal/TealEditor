using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class CtrlSpaceCompletionDataProvider : CodeCompletionDataProvider
	{
		private bool forceNewExpression;

		public bool ForceNewExpression
		{
			get
			{
				return this.forceNewExpression;
			}
			set
			{
				this.forceNewExpression = value;
			}
		}

		public CtrlSpaceCompletionDataProvider()
		{
		}

		public CtrlSpaceCompletionDataProvider(ExpressionContext overrideContext)
		{
			this.overrideContext = overrideContext;
		}

		protected override void GenerateCompletionData(EditViewControl textArea, char charTyped)
		{
			if (this.forceNewExpression)
			{
				this.preSelection = "";
				if (charTyped != '\0')
				{
					this.preSelection = null;
				}
				ExpressionContext context = this.overrideContext;
				if (context == null)
				{
					context = ExpressionContext.Default;
				}
				NRefactoryResolver rr = ProjectParser.GetResolver();
				base.AddResolveResults(rr.CtrlSpace(this.caretLineNumber, this.caretColumn, this.fileName, ProjectParser.GetFileContents(this.fileName), context), context);
			}
			else
			{
				ExpressionResult expressionResult = base.GetExpression(textArea);
				string expression = expressionResult.Expression;
				this.preSelection = null;
				if (expression == null || expression.Length == 0)
				{
					this.preSelection = "";
					if (charTyped != '\0')
					{
						this.preSelection = null;
					}
					NRefactoryResolver rr = ProjectParser.GetResolver();
					base.AddResolveResults(rr.CtrlSpace(this.caretLineNumber, this.caretColumn, this.fileName, ProjectParser.GetFileContents(this.fileName), expressionResult.Context), expressionResult.Context);
				}
				else
				{
					int idx = expression.LastIndexOf('.');
					if (idx > 0)
					{
						this.preSelection = expression.Substring(idx + 1);
						expressionResult.Expression = expression.Substring(0, idx);
						if (charTyped != '\0')
						{
							this.preSelection = null;
						}
						base.GenerateCompletionData(textArea, expressionResult);
					}
					else
					{
						this.preSelection = expression;
						if (charTyped != '\0')
						{
							this.preSelection = null;
						}
						ArrayList results = ProjectParser.GetResolver().CtrlSpace(this.caretLineNumber, this.caretColumn, this.fileName, ProjectParser.GetFileContents(this.fileName), expressionResult.Context);
						base.AddResolveResults(results, expressionResult.Context);
					}
				}
			}
		}
	}
}

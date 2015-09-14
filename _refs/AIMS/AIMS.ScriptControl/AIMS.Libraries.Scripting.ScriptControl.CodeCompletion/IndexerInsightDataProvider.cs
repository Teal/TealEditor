using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class IndexerInsightDataProvider : MethodInsightDataProvider
	{
		public IndexerInsightDataProvider()
		{
		}

		public IndexerInsightDataProvider(int lookupOffset, bool setupOnlyOnce) : base(lookupOffset, setupOnlyOnce)
		{
		}

		protected override void SetupDataProvider(string fileName, SyntaxDocument document, ExpressionResult expressionResult, int caretLineNumber, int caretColumn)
		{
			ResolveResult result = ProjectParser.GetResolver().Resolve(expressionResult, caretLineNumber, caretColumn, fileName, document.Text);
			if (result != null)
			{
				IReturnType type = result.ResolvedType;
				if (type != null)
				{
					foreach (IProperty i in type.GetProperties())
					{
						if (i.IsIndexer)
						{
							this.methods.Add(i);
						}
					}
				}
			}
		}
	}
}

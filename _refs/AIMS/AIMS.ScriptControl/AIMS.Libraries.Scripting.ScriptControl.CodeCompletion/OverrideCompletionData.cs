using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.Refactoring;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class OverrideCompletionData : DefaultCompletionData
	{
		private IMember member;

		private static string GetName(IMethod method, ConversionFlags flags)
		{
			ProjectParser.CurrentAmbience.ConversionFlags = (flags | ConversionFlags.ShowParameterNames);
			return ProjectParser.CurrentAmbience.Convert(method);
		}

		public OverrideCompletionData(IMethod method) : base(OverrideCompletionData.GetName(method, ConversionFlags.None), "override " + OverrideCompletionData.GetName(method, ConversionFlags.ShowAccessibility | ConversionFlags.ShowReturnType) + "\n\n" + method.Documentation, ScriptControl.GetIcon(method))
		{
			this.member = method;
		}

		public OverrideCompletionData(IProperty property) : base(property.Name, "override " + property.Name + "\n\n" + property.Documentation, ScriptControl.GetIcon(property))
		{
			this.member = property;
		}

		public override bool InsertAction(EditViewControl textArea, char ch)
		{
			ClassFinder context = new ClassFinder(textArea.FileName, textArea.Caret.Position.Y + 1, textArea.Caret.Position.X + 1);
			string lineText = textArea.Caret.CurrentRow.Text;
			string text2 = lineText;
			bool result;
			for (int i = 0; i < text2.Length; i++)
			{
				char c = text2[i];
				if (!char.IsWhiteSpace(c) && !char.IsLetterOrDigit(c))
				{
					result = base.InsertAction(textArea, ch);
					return result;
				}
			}
			string indentation = lineText.Substring(0, lineText.Length - lineText.TrimStart(new char[0]).Length);
			CodeGenerator codeGen = ProjectParser.CurrentProjectContent.Language.CodeGenerator;
			string text = codeGen.GenerateCode(codeGen.GetOverridingMethod(this.member, context), indentation);
			text = text.TrimEnd(new char[0]);
			TextRange tr = textArea.Document.GetRangeFromText(lineText, 0, textArea.Caret.Position.Y);
			tr = textArea.Document.ReplaceRange(tr, text, true);
			textArea.Caret.SetPos(new TextPoint(tr.LastColumn, tr.LastRow));
			textArea.ScrollIntoView();
			result = true;
			return result;
		}
	}
}

using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.Dom.Refactoring;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.NRefactory.Visitors;
using AIMS.Libraries.Scripting.ScriptControl;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.IO;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
	public class CSharpCompletionBinding : NRefactoryCodeCompletionBinding
	{
		private class CaseCompletionSwitchFinder : AbstractAstVisitor
		{
			private Location caretLocation;

			internal SwitchStatement bestStatement;

			public CaseCompletionSwitchFinder(int caretLine, int caretColumn)
			{
				this.caretLocation = new Location(caretColumn, caretLine);
			}

			public override object VisitSwitchStatement(SwitchStatement switchStatement, object data)
			{
				if (switchStatement.StartLocation < this.caretLocation && this.caretLocation < switchStatement.EndLocation)
				{
					this.bestStatement = switchStatement;
				}
				return base.VisitSwitchStatement(switchStatement, data);
			}
		}

		public CSharpCompletionBinding() : base(SupportedLanguage.CSharp)
		{
		}

		public override bool HandleKeyPress(CodeEditorControl editor, char ch)
		{
			CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.ActiveViewControl.FileName);
			int cursor = editor.ActiveViewControl.Caret.Offset;
			ExpressionContext context = null;
			bool result2;
			if (ch == '(')
			{
				if (CodeCompletionOptions.KeywordCompletionEnabled)
				{
					string text = editor.ActiveViewControl.Caret.CurrentWord.Text.Trim();
					switch (text)
					{
					case "for":
					case "lock":
						context = ExpressionContext.Default;
						break;
					case "using":
						context = ExpressionContext.TypeDerivingFrom(ProjectParser.CurrentProjectContent.GetClass("System.IDisposable"), false);
						break;
					case "catch":
						context = ExpressionContext.TypeDerivingFrom(ProjectParser.CurrentProjectContent.GetClass("System.Exception"), false);
						break;
					case "foreach":
					case "typeof":
					case "sizeof":
					case "default":
						context = ExpressionContext.Type;
						break;
					}
				}
				if (context != null)
				{
					if (this.IsInComment(editor))
					{
						result2 = false;
					}
					else
					{
						editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(context), ch);
						result2 = true;
					}
				}
				else if (base.EnableMethodInsight)
				{
					editor.ActiveViewControl.ShowInsightWindow(new MethodInsightDataProvider());
					result2 = true;
				}
				else
				{
					result2 = false;
				}
			}
			else
			{
				if (ch == '[')
				{
					Word curWord = editor.ActiveViewControl.Caret.CurrentWord;
					if (curWord.Text.Length == 0)
					{
						editor.ActiveViewControl.ShowCompletionWindow(new AttributesDataProvider(ProjectParser.CurrentProjectContent), ch);
						result2 = true;
						return result2;
					}
				}
				else if (ch == ',' && CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled)
				{
					if (base.InsightRefreshOnComma(editor, ch))
					{
						result2 = true;
						return result2;
					}
				}
				else if (ch == '=')
				{
					string curLine = editor.ActiveViewControl.Caret.CurrentRow.Text;
					string documentText = ProjectParser.GetFileContents(editor.FileName);
					int position = editor.ActiveViewControl.Caret.Offset - 2;
					if (position > 0 && documentText[position + 1] == '+')
					{
						ExpressionResult result = ef.FindFullExpression(documentText, position);
						if (result.Expression != null)
						{
							ResolveResult resolveResult = ProjectParser.GetResolver().Resolve(result, editor.ActiveViewControl.Caret.Position.Y + 1, editor.ActiveViewControl.Caret.Position.X + 1, editor.ActiveViewControl.FileName, documentText);
							if (resolveResult != null && resolveResult.ResolvedType != null)
							{
								IClass underlyingClass = resolveResult.ResolvedType.GetUnderlyingClass();
								if (underlyingClass == null && resolveResult.ResolvedType.FullyQualifiedName.Length > 0)
								{
									underlyingClass = ProjectParser.CurrentProjectContent.GetClass(resolveResult.ResolvedType.FullyQualifiedName);
								}
								if (underlyingClass != null && underlyingClass.IsTypeInInheritanceTree(ProjectParser.CurrentProjectContent.GetClass("System.Delegate")))
								{
									EventHandlerCompletitionDataProvider eventHandlerProvider = new EventHandlerCompletitionDataProvider(result.Expression, resolveResult);
									eventHandlerProvider.InsertSpace = true;
									editor.ActiveViewControl.ShowCompletionWindow(eventHandlerProvider, ch);
								}
							}
						}
					}
					else if (position > 0)
					{
						ExpressionResult result = ef.FindFullExpression(documentText, position);
						if (result.Expression != null)
						{
							ResolveResult resolveResult = ProjectParser.GetResolver().Resolve(result, editor.ActiveViewControl.Caret.Position.Y + 1, editor.ActiveViewControl.Caret.Position.X + 1, editor.ActiveViewControl.FileName, documentText);
							if (resolveResult != null && resolveResult.ResolvedType != null)
							{
								if (base.ProvideContextCompletion(editor, resolveResult.ResolvedType, ch))
								{
									result2 = true;
									return result2;
								}
							}
						}
					}
				}
				else if (ch == ';')
				{
					string curLine = editor.ActiveViewControl.Caret.CurrentRow.Text;
					this.TryDeclarationTypeInference(editor, curLine);
				}
				result2 = base.HandleKeyPress(editor, ch);
			}
			return result2;
		}

		private bool TryDeclarationTypeInference(CodeEditorControl editor, string curLine)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.CSharp, new StringReader(curLine));
			Token typeToken = lexer.NextToken();
			bool result;
			if (typeToken.kind == 12)
			{
				if (lexer.NextToken().kind == 1)
				{
					Token t = lexer.NextToken();
					if (t.kind == 3)
					{
						string expr = curLine.Substring(t.col);
						ResolveResult rr = ProjectParser.GetResolver().Resolve(new ExpressionResult(expr), editor.ActiveViewControl.Caret.Position.Y + 1, t.col, editor.ActiveViewControl.FileName, ProjectParser.GetFileContents(editor.FileName));
						if (rr != null && rr.ResolvedType != null)
						{
							ClassFinder context = new ClassFinder(editor.ActiveViewControl.FileName, editor.ActiveViewControl.Caret.Position.Y, t.col);
							if (CodeGenerator.CanUseShortTypeName(rr.ResolvedType, context))
							{
								CSharpAmbience.Instance.ConversionFlags = ConversionFlags.None;
							}
							else
							{
								CSharpAmbience.Instance.ConversionFlags = ConversionFlags.UseFullyQualifiedNames;
							}
							string typeName = CSharpAmbience.Instance.Convert(rr.ResolvedType);
							int offset = editor.ActiveViewControl.Document.GetRange(new TextRange(0, 0, 0, editor.ActiveViewControl.Caret.Position.Y)).Length;
							editor.ActiveViewControl.Document.InsertText(typeName, offset + typeToken.col - 1, editor.ActiveViewControl.Caret.Position.Y);
							editor.ActiveViewControl.Caret.SetPos(new TextPoint(editor.ActiveViewControl.Caret.Position.X + typeName.Length - 1, editor.ActiveViewControl.Caret.Position.Y));
							result = true;
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}

		private bool IsInComment(CodeEditorControl editor)
		{
			CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.ActiveViewControl.FileName);
			int cursor = editor.ActiveViewControl.Caret.Offset - 1;
			return ef.FilterComments(ProjectParser.GetFileContents(editor.FileName).Substring(0, cursor + 1), ref cursor) == null;
		}

		public override bool HandleKeyword(CodeEditorControl editor, string word)
		{
			bool result;
			switch (word)
			{
			case "using":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Namespace), ' ');
				result = true;
				return result;
			case "as":
			case "is":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
				result = true;
				return result;
			case "override":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new OverrideCompletionDataProvider(), ' ');
				result = true;
				return result;
			case "new":
				result = this.ShowNewCompletion(editor);
				return result;
			case "case":
				result = (!this.IsInComment(editor) && this.DoCaseCompletion(editor));
				return result;
			case "return":
			{
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				IMember i = base.GetCurrentMember(editor);
				if (i != null)
				{
					result = base.ProvideContextCompletion(editor, i.ReturnType, ' ');
					return result;
				}
				break;
			}
			}
			result = base.HandleKeyword(editor, word);
			return result;
		}

		private bool ShowNewCompletion(CodeEditorControl editor)
		{
			CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.ActiveViewControl.FileName);
			int cursor = editor.ActiveViewControl.Caret.Offset;
			ExpressionContext context = ef.FindExpression(ProjectParser.GetFileContents(editor.FileName).Substring(0, cursor) + " T.", cursor + 2).Context;
			bool result;
			if (context.IsObjectCreation)
			{
				editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(context), ' ');
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool DoCaseCompletion(CodeEditorControl editor)
		{
			Caret caret = editor.ActiveViewControl.Caret;
			NRefactoryResolver r = new NRefactoryResolver(ProjectParser.CurrentProjectContent, LanguageProperties.CSharp);
			bool result;
			if (r.Initialize(editor.ActiveViewControl.FileName, caret.Position.Y + 1, caret.Position.X + 1))
			{
				INode currentMember = r.ParseCurrentMember(ProjectParser.GetFileContents(editor.FileName));
				if (currentMember != null)
				{
					CSharpCompletionBinding.CaseCompletionSwitchFinder ccsf = new CSharpCompletionBinding.CaseCompletionSwitchFinder(caret.Position.Y + 1, caret.Position.X + 1);
					currentMember.AcceptVisitor(ccsf, null);
					if (ccsf.bestStatement != null)
					{
						r.RunLookupTableVisitor(currentMember);
						ResolveResult rr = r.ResolveInternal(ccsf.bestStatement.SwitchExpression, ExpressionContext.Default);
						if (rr != null && rr.ResolvedType != null)
						{
							result = base.ProvideContextCompletion(editor, rr.ResolvedType, ' ');
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}
	}
}

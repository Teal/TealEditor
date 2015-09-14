using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.Refactoring;
using AIMS.Libraries.Scripting.Dom.VBNet;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.ScriptControl;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.IO;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
	public class VBNetCompletionBinding : NRefactoryCodeCompletionBinding
	{
		public VBNetCompletionBinding() : base(SupportedLanguage.VBNet)
		{
			base.EnableIndexerInsight = false;
		}

		public override bool HandleKeyPress(CodeEditorControl editor, char ch)
		{
			VBExpressionFinder ef = new VBExpressionFinder();
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
					case "For":
					case "Lock":
						context = ExpressionContext.Default;
						break;
					case "using":
						context = ExpressionContext.TypeDerivingFrom(ProjectParser.CurrentProjectContent.GetClass("System.IDisposable"), false);
						break;
					case "Catch":
						context = ExpressionContext.TypeDerivingFrom(ProjectParser.CurrentProjectContent.GetClass("System.Exception"), false);
						break;
					case "For Each":
					case "typeof":
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
				if (ch == '<')
				{
					Word curWord = editor.ActiveViewControl.Caret.CurrentWord;
					if (curWord == null || curWord.Text.Length == 0)
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
								if (underlyingClass != null && underlyingClass.IsTypeInInheritanceTree(ProjectParser.CurrentProjectContent.GetClass("System.MulticastDelegate")))
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
				else if (ch == '\n')
				{
					string curLine = editor.ActiveViewControl.Caret.CurrentRow.Text;
					this.TryDeclarationTypeInference(editor, curLine);
				}
				result2 = base.HandleKeyPress(editor, ch);
			}
			return result2;
		}

		private bool IsInComment(CodeEditorControl editor)
		{
			VBExpressionFinder ef = new VBExpressionFinder();
			int cursor = editor.ActiveViewControl.Caret.Offset - 1;
			return ef.FilterComments(ProjectParser.GetFileContents(editor.FileName).Substring(0, cursor + 1), ref cursor) == null;
		}

		public override bool HandleKeyword(CodeEditorControl editor, string word)
		{
			string text = word.ToLowerInvariant();
			bool result;
			switch (text)
			{
			case "imports":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new CodeCompletionDataProvider(new ExpressionResult("Global", ExpressionContext.Importable)), ' ');
				result = true;
				return result;
			case "as":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
				result = true;
				return result;
			case "new":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.ObjectCreation), ' ');
				result = true;
				return result;
			case "inherits":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
				result = true;
				return result;
			case "implements":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Interface), ' ');
				result = true;
				return result;
			case "overrides":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new OverrideCompletionDataProvider(), ' ');
				result = true;
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
					base.ProvideContextCompletion(editor, i.ReturnType, ' ');
					result = true;
					return result;
				}
				break;
			}
			case "option":
				if (this.IsInComment(editor))
				{
					result = false;
					return result;
				}
				editor.ActiveViewControl.ShowCompletionWindow(new TextCompletionDataProvider(new string[]
				{
					"Explicit On",
					"Explicit Off",
					"Strict On",
					"Strict Off",
					"Compare Binary",
					"Compare Text"
				}), ' ');
				result = true;
				return result;
			}
			result = base.HandleKeyword(editor, word);
			return result;
		}

		private bool TryDeclarationTypeInference(CodeEditorControl editor, string curLine)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(curLine));
			bool result;
			if (lexer.NextToken().kind != 81)
			{
				result = false;
			}
			else if (lexer.NextToken().kind != 2)
			{
				result = false;
			}
			else if (lexer.NextToken().kind != 48)
			{
				result = false;
			}
			else
			{
				Token t = lexer.NextToken();
				if (t.kind != 21)
				{
					result = false;
				}
				else
				{
					Token t2 = lexer.NextToken();
					if (t2.kind != 11)
					{
						result = false;
					}
					else
					{
						string expr = curLine.Substring(t2.col);
						ResolveResult rr = ProjectParser.GetResolver().Resolve(new ExpressionResult(expr), editor.ActiveViewControl.Caret.Position.Y + 1, t2.col, editor.ActiveViewControl.FileName, ProjectParser.GetFileContents(editor.FileName));
						if (rr != null && rr.ResolvedType != null)
						{
							ClassFinder context = new ClassFinder(editor.ActiveViewControl.FileName, editor.ActiveViewControl.Caret.Position.Y, t.col);
							if (CodeGenerator.CanUseShortTypeName(rr.ResolvedType, context))
							{
								VBNetAmbience.Instance.ConversionFlags = ConversionFlags.None;
							}
							else
							{
								VBNetAmbience.Instance.ConversionFlags = ConversionFlags.UseFullyQualifiedNames;
							}
							string typeName = VBNetAmbience.Instance.Convert(rr.ResolvedType);
							int offset = editor.ActiveViewControl.Document.GetRange(new TextRange(0, 0, 0, editor.ActiveViewControl.Caret.Position.Y)).Length;
							editor.ActiveViewControl.Document.InsertText(typeName, offset + t.col - 1, editor.ActiveViewControl.Caret.Position.Y);
							editor.ActiveViewControl.Caret.SetPos(new TextPoint(editor.ActiveViewControl.Caret.Position.X + typeName.Length - 1, editor.ActiveViewControl.Caret.Position.Y));
							result = true;
						}
						else
						{
							result = false;
						}
					}
				}
			}
			return result;
		}
	}
}

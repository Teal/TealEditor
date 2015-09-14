using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
	public abstract class NRefactoryCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		protected class InspectedCall
		{
			internal Location start;

			internal List<Location> commas = new List<Location>();

			internal NRefactoryCodeCompletionBinding.InspectedCall parent;

			public InspectedCall(Location start, NRefactoryCodeCompletionBinding.InspectedCall parent)
			{
				this.start = start;
				this.parent = parent;
			}
		}

		private class ContextCompletionDataProvider : CachedCompletionDataProvider
		{
			internal char activationKey;

			internal ContextCompletionDataProvider(ICompletionDataProvider baseProvider) : base(baseProvider)
			{
			}

			public override CompletionDataProviderKeyResult ProcessKey(char key)
			{
				CompletionDataProviderKeyResult result;
				if (key == '=' && this.activationKey == '=')
				{
					result = CompletionDataProviderKeyResult.BeforeStartKey;
				}
				else
				{
					this.activationKey = '\0';
					result = base.ProcessKey(key);
				}
				return result;
			}
		}

		private readonly SupportedLanguage language;

		private readonly int eofToken;

		private readonly int commaToken;

		private readonly int openParensToken;

		private readonly int closeParensToken;

		private readonly int openBracketToken;

		private readonly int closeBracketToken;

		private readonly int openBracesToken;

		private readonly int closeBracesToken;

		private readonly LanguageProperties languageProperties;

		protected NRefactoryCodeCompletionBinding(SupportedLanguage language)
		{
			this.language = language;
			if (language == SupportedLanguage.CSharp)
			{
				this.eofToken = 0;
				this.commaToken = 14;
				this.openParensToken = 20;
				this.closeParensToken = 21;
				this.openBracketToken = 18;
				this.closeBracketToken = 19;
				this.openBracesToken = 16;
				this.closeBracesToken = 17;
				this.languageProperties = LanguageProperties.CSharp;
			}
			else
			{
				this.eofToken = 0;
				this.commaToken = 12;
				this.openParensToken = 24;
				this.closeParensToken = 25;
				this.openBracketToken = -1;
				this.closeBracketToken = -1;
				this.openBracesToken = 22;
				this.closeBracesToken = 23;
				this.languageProperties = LanguageProperties.VBNet;
			}
		}

		protected IList<ResolveResult> ResolveCallParameters(CodeEditorControl editor, NRefactoryCodeCompletionBinding.InspectedCall call)
		{
			List<ResolveResult> rr = new List<ResolveResult>();
			int offset = this.LocationToOffset(editor, call.start);
			string documentText = ProjectParser.GetFileContents(editor.FileName);
			offset = documentText.LastIndexOf('(');
			int newOffset;
			foreach (Location loc in call.commas)
			{
				newOffset = this.LocationToOffset(editor, loc);
				if (newOffset < 0)
				{
					break;
				}
				TextPoint start = editor.ActiveViewControl.Document.IntPosToPoint(offset);
				TextPoint end = editor.ActiveViewControl.Document.IntPosToPoint(newOffset);
				TextRange tr = new TextRange(start.X, start.Y, end.X, end.Y);
				string text = editor.ActiveViewControl.Document.GetRange(tr);
				rr.Add(ProjectParser.GetResolver().Resolve(new ExpressionResult(text), loc.Line, loc.Column, editor.ActiveViewControl.FileName, documentText));
			}
			newOffset = editor.ActiveViewControl.Caret.Offset;
			if (offset < newOffset)
			{
				TextPoint start = editor.ActiveViewControl.Document.IntPosToPoint(offset);
				TextPoint end = editor.ActiveViewControl.Document.IntPosToPoint(newOffset);
				TextRange tr = new TextRange(start.X, start.Y, end.X, end.Y);
				string text = editor.ActiveViewControl.Document.GetRange(tr);
				rr.Add(ProjectParser.GetResolver().Resolve(new ExpressionResult(text), editor.ActiveViewControl.Caret.Position.Y + 1, editor.ActiveViewControl.Caret.Position.X + 1, editor.ActiveViewControl.FileName, documentText));
			}
			return rr;
		}

		protected bool InsightRefreshOnComma(CodeEditorControl editor, char ch)
		{
			NRefactoryResolver r = new NRefactoryResolver(ProjectParser.CurrentProjectContent, this.languageProperties);
			Location cursorLocation = new Location(editor.ActiveViewControl.Caret.Position.X + 1, editor.ActiveViewControl.Caret.Position.Y + 1);
			bool result;
			if (r.Initialize(editor.ActiveViewControl.FileName, cursorLocation.Y, cursorLocation.X))
			{
				TextReader currentMethod = r.ExtractCurrentMethod(ProjectParser.GetFileContents(editor.FileName));
				if (currentMethod != null)
				{
					ILexer lexer = ParserFactory.CreateLexer(this.language, currentMethod);
					NRefactoryCodeCompletionBinding.InspectedCall call = new NRefactoryCodeCompletionBinding.InspectedCall(Location.Empty, null);
					call.parent = call;
					Token token;
					while ((token = lexer.NextToken()) != null && token.kind != this.eofToken && token.Location < cursorLocation)
					{
						if (token.kind == this.commaToken)
						{
							call.commas.Add(token.Location);
						}
						else if (token.kind == this.openParensToken || token.kind == this.openBracketToken || token.kind == this.openBracesToken)
						{
							call = new NRefactoryCodeCompletionBinding.InspectedCall(token.Location, call);
						}
						else if (token.kind == this.closeParensToken || token.kind == this.closeBracketToken || token.kind == this.closeBracesToken)
						{
							call = call.parent;
						}
					}
					int offset = this.LocationToOffset(editor, call.start);
					string docText = ProjectParser.GetFileContents(editor.FileName);
					offset = docText.LastIndexOf('(');
					if (offset >= 0 && offset < docText.Length)
					{
						char c = docText.Substring(offset, 1).ToCharArray(0, 1)[0];
						if (c == '(')
						{
							this.ShowInsight(editor, new MethodInsightDataProvider(offset, true), this.ResolveCallParameters(editor, call), ch);
							result = true;
							return result;
						}
						if (c == '[')
						{
							this.ShowInsight(editor, new IndexerInsightDataProvider(offset, true), this.ResolveCallParameters(editor, call), ch);
							result = true;
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}

		protected bool ProvideContextCompletion(CodeEditorControl editor, IReturnType expected, char charTyped)
		{
			bool result;
			if (expected == null)
			{
				result = false;
			}
			else
			{
				IClass c = expected.GetUnderlyingClass();
				if (c == null)
				{
					result = false;
				}
				else
				{
					if (c.ClassType == ClassType.Enum)
					{
						CtrlSpaceCompletionDataProvider cdp = new CtrlSpaceCompletionDataProvider();
						cdp.ForceNewExpression = true;
						NRefactoryCodeCompletionBinding.ContextCompletionDataProvider cache = new NRefactoryCodeCompletionBinding.ContextCompletionDataProvider(cdp);
						cache.activationKey = charTyped;
						cache.GenerateCompletionData(editor.ActiveViewControl.FileName, editor.ActiveViewControl, charTyped);
						ICompletionData[] completionData = cache.CompletionData;
						Array.Sort<ICompletionData>(completionData);
						for (int i = 0; i < completionData.Length; i++)
						{
							CodeCompletionData ccd = completionData[i] as CodeCompletionData;
							if (ccd != null && ccd.Class != null)
							{
								if (ccd.Class.FullyQualifiedName == expected.FullyQualifiedName)
								{
									cache.DefaultIndex = i;
									break;
								}
							}
						}
						if (cache.DefaultIndex >= 0)
						{
							if (charTyped != ' ')
							{
								cdp.InsertSpace = true;
							}
							editor.ActiveViewControl.ShowCompletionWindow(cache, charTyped);
							result = true;
							return result;
						}
					}
					result = false;
				}
			}
			return result;
		}

		protected void ShowInsight(CodeEditorControl editor, MethodInsightDataProvider dp, ICollection<ResolveResult> parameters, char charTyped)
		{
			int paramCount = parameters.Count;
			dp.SetupDataProvider(editor.ActiveViewControl.FileName, editor.ActiveViewControl);
			List<IMethodOrProperty> methods = dp.Methods;
			if (methods.Count != 0)
			{
				bool overloadIsSure;
				if (methods.Count == 1)
				{
					overloadIsSure = true;
					dp.DefaultIndex = 0;
				}
				else
				{
					IReturnType[] parameterTypes = new IReturnType[paramCount + 1];
					int i = 0;
					foreach (ResolveResult rr in parameters)
					{
						if (rr != null)
						{
							parameterTypes[i] = rr.ResolvedType;
						}
						i++;
					}
					IReturnType[][] tmp;
					int[] ranking = MemberLookupHelper.RankOverloads(methods, parameterTypes, true, out overloadIsSure, out tmp);
					bool multipleBest = false;
					int bestRanking = -1;
					int best = 0;
					for (i = 0; i < ranking.Length; i++)
					{
						if (ranking[i] > bestRanking)
						{
							bestRanking = ranking[i];
							best = i;
							multipleBest = false;
						}
						else if (ranking[i] == bestRanking)
						{
							multipleBest = true;
						}
					}
					if (multipleBest)
					{
						overloadIsSure = false;
					}
					dp.DefaultIndex = best;
				}
				editor.ActiveViewControl.ShowInsightWindow(dp);
				if (overloadIsSure)
				{
					IMethodOrProperty method = methods[dp.DefaultIndex];
					if (paramCount < method.Parameters.Count)
					{
						IParameter param = method.Parameters[paramCount];
						this.ProvideContextCompletion(editor, param.ReturnType, charTyped);
					}
				}
			}
		}

		protected int LocationToOffset(CodeEditorControl editor, Location loc)
		{
			int result;
			if (loc.IsEmpty || loc.Line - 1 >= editor.ActiveViewControl.Document.Lines.Length)
			{
				result = -1;
			}
			else
			{
				string seg = editor.ActiveViewControl.Document.Lines[loc.Line - 1];
				TextRange tr = new TextRange(0, 0, 0, loc.Line);
				int tl = editor.ActiveViewControl.Document.GetRange(tr).Length;
				result = tl + Math.Min(loc.Column, seg.Length) - 1;
			}
			return result;
		}

		protected IMember GetCurrentMember(CodeEditorControl editor)
		{
			Caret caret = editor.ActiveViewControl.Caret;
			NRefactoryResolver r = new NRefactoryResolver(ProjectParser.CurrentProjectContent, this.languageProperties);
			IMember result;
			if (r.Initialize(editor.ActiveViewControl.FileName, caret.Position.Y + 1, caret.Position.X + 1))
			{
				result = r.CallingMember;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}

using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.InsightWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.VBNet;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class MethodInsightDataProvider : IInsightDataProvider
	{
		private string fileName = null;

		private SyntaxDocument document = null;

		private EditViewControl textArea = null;

		protected List<IMethodOrProperty> methods = new List<IMethodOrProperty>();

		private int defaultIndex = -1;

		private int lookupOffset;

		private bool setupOnlyOnce;

		private int initialOffset;

		public List<IMethodOrProperty> Methods
		{
			get
			{
				return this.methods;
			}
		}

		public int InsightDataCount
		{
			get
			{
				return this.methods.Count;
			}
		}

		public int DefaultIndex
		{
			get
			{
				return this.defaultIndex;
			}
			set
			{
				this.defaultIndex = value;
			}
		}

		public string GetInsightData(int number)
		{
			IMember method = this.methods[number];
			IAmbience conv = ProjectParser.CurrentAmbience;
			conv.ConversionFlags = ConversionFlags.StandardConversionFlags;
			string documentation = method.Documentation;
			string text;
			if (method is IMethod)
			{
				text = conv.Convert(method as IMethod);
			}
			else if (method is IProperty)
			{
				text = conv.Convert(method as IProperty);
			}
			else
			{
				text = method.ToString();
			}
			return text + "\n" + CodeCompletionData.GetDocumentation(documentation);
		}

		public MethodInsightDataProvider()
		{
			this.lookupOffset = -1;
		}

		public MethodInsightDataProvider(int lookupOffset, bool setupOnlyOnce)
		{
			this.lookupOffset = lookupOffset;
			this.setupOnlyOnce = setupOnlyOnce;
		}

		public void SetupDataProvider(string fileName, EditViewControl textArea)
		{
			if (!this.setupOnlyOnce || this.textArea == null)
			{
				SyntaxDocument document = textArea.Document;
				this.fileName = fileName;
				this.document = document;
				this.textArea = textArea;
				int useOffset = (this.lookupOffset < 0) ? textArea.Caret.Offset : this.lookupOffset;
				this.initialOffset = useOffset;
				IExpressionFinder expressionFinder;
				if (ProjectParser.CurrentProjectContent.Language == LanguageProperties.CSharp)
				{
					expressionFinder = new CSharpExpressionFinder(fileName);
				}
				else
				{
					expressionFinder = new VBExpressionFinder();
				}
				ExpressionResult expressionResult;
				if (expressionFinder == null)
				{
					expressionResult = new ExpressionResult(textArea.Caret.CurrentWord.Text);
				}
				else
				{
					expressionResult = expressionFinder.FindExpression(ProjectParser.GetFileContents(fileName), useOffset - 1);
				}
				if (expressionResult.Expression != null)
				{
					expressionResult.Expression = expressionResult.Expression.Trim();
					TextPoint tp = document.IntPosToPoint(useOffset + 1);
					int caretLineNumber = tp.Y;
					int caretColumn = tp.X;
					this.SetupDataProvider(fileName, document, expressionResult, caretLineNumber, caretColumn);
				}
			}
		}

		protected virtual void SetupDataProvider(string fileName, SyntaxDocument document, ExpressionResult expressionResult, int caretLineNumber, int caretColumn)
		{
			bool constructorInsight = false;
			if (expressionResult.Context.IsAttributeContext)
			{
				constructorInsight = true;
			}
			else if (expressionResult.Context.IsObjectCreation)
			{
				constructorInsight = true;
				expressionResult.Context = ExpressionContext.Type;
			}
			ResolveResult results = ProjectParser.GetResolver().Resolve(expressionResult, caretLineNumber, caretColumn, fileName, document.Text);
			LanguageProperties language = ProjectParser.CurrentProjectContent.Language;
			IReturnType type = results.ResolvedType;
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
			TypeResolveResult trr = results as TypeResolveResult;
			if (trr == null && language.AllowObjectConstructionOutsideContext)
			{
				if (results is MixedResolveResult)
				{
					trr = (results as MixedResolveResult).TypeResult;
				}
			}
			if (trr != null && !constructorInsight)
			{
				if (language.AllowObjectConstructionOutsideContext)
				{
					constructorInsight = true;
				}
			}
			if (constructorInsight)
			{
				if (trr != null)
				{
					foreach (IMethod method in trr.ResolvedType.GetMethods())
					{
						if (method.IsConstructor && !method.IsStatic)
						{
							this.methods.Add(method);
						}
					}
					if (this.methods.Count == 0 && trr.ResolvedClass != null && !trr.ResolvedClass.IsAbstract && !trr.ResolvedClass.IsStatic)
					{
						this.methods.Add(Constructor.CreateDefault(trr.ResolvedClass));
					}
				}
			}
			else
			{
				MethodResolveResult result = results as MethodResolveResult;
				if (result != null)
				{
					bool classIsInInheritanceTree = false;
					if (result.CallingClass != null)
					{
						classIsInInheritanceTree = result.CallingClass.IsTypeInInheritanceTree(result.ContainingType.GetUnderlyingClass());
					}
					foreach (IMethod method in result.ContainingType.GetMethods())
					{
						if (language.NameComparer.Equals(method.Name, result.Name))
						{
							if (method.IsAccessible(result.CallingClass, classIsInInheritanceTree))
							{
								this.methods.Add(method);
							}
						}
					}
					if (this.methods.Count == 0 && result.CallingClass != null && language.SupportsExtensionMethods)
					{
						ArrayList list = new ArrayList();
						ResolveResult.AddExtensions(language, list, result.CallingClass, result.ContainingType);
						foreach (IMethodOrProperty mp in list)
						{
							if (language.NameComparer.Equals(mp.Name, result.Name) && mp is IMethod)
							{
								IMethod j = (IMethod)mp.Clone();
								j.Parameters.RemoveAt(0);
								this.methods.Add(j);
							}
						}
					}
				}
			}
		}

		public bool CaretOffsetChanged()
		{
			bool closeDataProvider = this.textArea.Caret.Offset <= this.initialOffset;
			int brackets = 0;
			int curlyBrackets = 0;
			bool result;
			if (!closeDataProvider)
			{
				bool insideChar = false;
				bool insideString = false;
				for (int offset = this.initialOffset; offset < Math.Min(this.textArea.Caret.Offset, this.document.Text.Length); offset++)
				{
					char ch = this.document.Text.Substring(offset, 1).ToCharArray(0, 1)[0];
					char c = ch;
					if (c <= ')')
					{
						if (c != '"')
						{
							switch (c)
							{
							case '\'':
								insideChar = !insideChar;
								break;
							case '(':
								if (!insideChar && !insideString)
								{
									brackets++;
								}
								break;
							case ')':
								if (!insideChar && !insideString)
								{
									brackets--;
								}
								if (brackets <= 0)
								{
									result = true;
									return result;
								}
								break;
							}
						}
						else
						{
							insideString = !insideString;
						}
					}
					else if (c != ';')
					{
						switch (c)
						{
						case '{':
							if (!insideChar && !insideString)
							{
								curlyBrackets++;
							}
							break;
						case '}':
							if (!insideChar && !insideString)
							{
								curlyBrackets--;
							}
							if (curlyBrackets < 0)
							{
								result = true;
								return result;
							}
							break;
						}
					}
					else if (!insideChar && !insideString)
					{
						result = true;
						return result;
					}
				}
			}
			result = closeDataProvider;
			return result;
		}

		public bool CharTyped()
		{
			return false;
		}
	}
}

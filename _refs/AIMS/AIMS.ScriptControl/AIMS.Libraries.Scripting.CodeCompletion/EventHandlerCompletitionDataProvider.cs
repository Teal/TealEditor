using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
	public class EventHandlerCompletitionDataProvider : AbstractCompletionDataProvider
	{
		private class DelegateCompletionData : DefaultCompletionData
		{
			private int cursorOffset;

			public DelegateCompletionData(string text, int cursorOffset, string documentation) : base(text, documentation, AutoListIcons.iDelegate)
			{
				this.cursorOffset = cursorOffset;
			}

			public override bool InsertAction(EditViewControl textArea, char ch)
			{
				bool r = base.InsertAction(textArea, ch);
				textArea.Caret.Position.X -= this.cursorOffset;
				return r;
			}
		}

		private string expression;

		private ResolveResult resolveResult;

		private IClass resolvedClass;

		public EventHandlerCompletitionDataProvider(string expression, ResolveResult resolveResult)
		{
			this.expression = expression;
			this.resolveResult = resolveResult;
			this.resolvedClass = resolveResult.ResolvedType.GetUnderlyingClass();
			if (this.resolvedClass == null && resolveResult.ResolvedType.FullyQualifiedName.Length > 0)
			{
				this.resolvedClass = ProjectParser.CurrentProjectContent.GetClass(resolveResult.ResolvedType.FullyQualifiedName);
			}
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
		{
			string methodName = this.resolveResult.CallingClass.Name + "_" + this.expression.Trim().Substring(this.expression.Trim().LastIndexOf('.') + 1);
			List<ICompletionData> completionData = new List<ICompletionData>();
			completionData.Add(new EventHandlerCompletitionDataProvider.DelegateCompletionData("delegate {  };", 3, "Insert Anonymous Method"));
			CSharpAmbience ambience = new CSharpAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
			IMethod invoke = this.resolvedClass.SearchMember("Invoke", LanguageProperties.CSharp) as IMethod;
			DomRegion r = this.resolveResult.CallingMember.BodyRegion;
			DomRegion rm = this.resolveResult.CallingMember.Region;
			TextPoint cPos = textArea.Caret.Position;
			TextRange trIntened = new TextRange(0, rm.BeginLine - 1, rm.BeginColumn - 1, rm.BeginLine - 1);
			string IntendString = textArea.Document.GetRange(trIntened);
			int curPos = textArea.Document.PointToIntPos(new TextPoint(0, r.EndLine - 1));
			StringBuilder parambuilder = new StringBuilder("(");
			if (invoke != null)
			{
				StringBuilder builder = new StringBuilder("delegate(");
				for (int i = 0; i < invoke.Parameters.Count; i++)
				{
					if (i > 0)
					{
						builder.Append(", ");
						parambuilder.Append(", ");
					}
					builder.Append(ambience.Convert(invoke.Parameters[i]));
					parambuilder.Append(ambience.Convert(invoke.Parameters[i]));
				}
				builder.Append(") {  };");
				parambuilder.Append(")");
				string MethodBody = string.Concat(new object[]
				{
					"new ",
					this.resolveResult.ResolvedType.Name,
					"(delegate",
					parambuilder,
					"{   });"
				});
				completionData.Add(new EventHandlerCompletitionDataProvider.DelegateCompletionData(MethodBody, 4, "delegate " + this.resolvedClass.FullyQualifiedName + "\n" + CodeCompletionData.GetDocumentation(this.resolvedClass.Documentation)));
				completionData.Add(new EventHandlerCompletitionDataProvider.DelegateCompletionData(builder.ToString(), 3, "Insert Anonymous Method With Parameters"));
				IClass callingClass = this.resolveResult.CallingClass;
				IClass eventReturnType = invoke.ReturnType.GetUnderlyingClass();
				IClass[] eventParameters = new IClass[invoke.Parameters.Count];
				for (int i = 0; i < eventParameters.Length; i++)
				{
					eventParameters[i] = invoke.Parameters[i].ReturnType.GetUnderlyingClass();
					if (eventParameters[i] == null)
					{
						eventReturnType = null;
						break;
					}
				}
				if (callingClass != null && eventReturnType != null)
				{
					bool inStatic = false;
					if (this.resolveResult.CallingMember != null)
					{
						inStatic = this.resolveResult.CallingMember.IsStatic;
					}
					foreach (IMethod method in callingClass.DefaultReturnType.GetMethods())
					{
						if (!inStatic || method.IsStatic)
						{
							if (method.IsAccessible(callingClass, true))
							{
								if (method.Parameters.Count == invoke.Parameters.Count)
								{
									IClass c2 = method.ReturnType.GetUnderlyingClass();
									if (c2 != null && c2.IsTypeInInheritanceTree(eventReturnType))
									{
										bool ok = true;
										for (int i = 0; i < eventParameters.Length; i++)
										{
											c2 = method.Parameters[i].ReturnType.GetUnderlyingClass();
											if (c2 == null || !eventParameters[i].IsTypeInInheritanceTree(c2))
											{
												ok = false;
												break;
											}
										}
										if (ok)
										{
											completionData.Add(new CodeCompletionData(method));
										}
									}
								}
							}
						}
					}
				}
			}
			return completionData.ToArray();
		}
	}
}

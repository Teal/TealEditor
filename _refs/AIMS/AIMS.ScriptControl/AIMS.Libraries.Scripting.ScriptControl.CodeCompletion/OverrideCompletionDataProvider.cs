using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class OverrideCompletionDataProvider : AbstractCompletionDataProvider
	{
		public static IMethod[] GetOverridableMethods(IClass c)
		{
			if (c == null)
			{
				throw new ArgumentException("c");
			}
			List<IMethod> methods = new List<IMethod>();
			foreach (IMethod i in c.DefaultReturnType.GetMethods())
			{
				if (i.IsOverridable && !i.IsConst && !i.IsPrivate)
				{
					if (i.DeclaringType.FullyQualifiedName != c.FullyQualifiedName)
					{
						methods.Add(i);
					}
				}
			}
			return methods.ToArray();
		}

		public static IProperty[] GetOverridableProperties(IClass c)
		{
			if (c == null)
			{
				throw new ArgumentException("c");
			}
			List<IProperty> properties = new List<IProperty>();
			foreach (IProperty p in c.DefaultReturnType.GetProperties())
			{
				if (p.IsOverridable && !p.IsConst && !p.IsPrivate)
				{
					if (p.DeclaringType.FullyQualifiedName != c.FullyQualifiedName)
					{
						properties.Add(p);
					}
				}
			}
			return properties.ToArray();
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
		{
			IClass c = ProjectParser.GetParseInformation(fileName).ValidCompilationUnit.GetInnermostClass(textArea.Caret.Position.Y, textArea.Caret.Position.X);
			ICompletionData[] result2;
			if (c == null)
			{
				result2 = null;
			}
			else
			{
				List<ICompletionData> result = new List<ICompletionData>();
				IMethod[] overridableMethods = OverrideCompletionDataProvider.GetOverridableMethods(c);
				for (int j = 0; j < overridableMethods.Length; j++)
				{
					IMethod i = overridableMethods[j];
					result.Add(new OverrideCompletionData(i));
				}
				IProperty[] overridableProperties = OverrideCompletionDataProvider.GetOverridableProperties(c);
				for (int j = 0; j < overridableProperties.Length; j++)
				{
					IProperty p = overridableProperties[j];
					result.Add(new OverrideCompletionData(p));
				}
				result2 = result.ToArray();
			}
			return result2;
		}
	}
}

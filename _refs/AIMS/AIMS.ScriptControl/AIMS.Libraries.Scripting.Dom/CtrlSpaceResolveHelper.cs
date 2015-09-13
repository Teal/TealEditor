using System;
using System.Collections;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class CtrlSpaceResolveHelper
	{
		private static void AddTypeParametersForCtrlSpace(ArrayList result, IEnumerable<ITypeParameter> typeParameters)
		{
			foreach (ITypeParameter p in typeParameters)
			{
				DefaultClass c = DefaultTypeParameter.GetDummyClassForTypeParameter(p);
				if (p.Method != null)
				{
					c.Documentation = "Type parameter of " + p.Method.Name;
				}
				else
				{
					c.Documentation = "Type parameter of " + p.Class.Name;
				}
				result.Add(c);
			}
		}

		public static void AddContentsFromCalling(ArrayList result, IClass callingClass, IMember callingMember)
		{
			IMethodOrProperty methodOrProperty = callingMember as IMethodOrProperty;
			if (methodOrProperty != null)
			{
				foreach (IParameter p in methodOrProperty.Parameters)
				{
					result.Add(new DefaultField.ParameterField(p.ReturnType, p.Name, methodOrProperty.Region, callingClass));
				}
				if (callingMember is IMethod)
				{
					CtrlSpaceResolveHelper.AddTypeParametersForCtrlSpace(result, ((IMethod)callingMember).TypeParameters);
				}
			}
			bool inStatic = false;
			if (callingMember != null)
			{
				inStatic = callingMember.IsStatic;
			}
			if (callingClass != null)
			{
				CtrlSpaceResolveHelper.AddTypeParametersForCtrlSpace(result, callingClass.TypeParameters);
				ArrayList members = new ArrayList();
				IReturnType t = callingClass.DefaultReturnType;
				members.AddRange(t.GetMethods());
				members.AddRange(t.GetFields());
				members.AddRange(t.GetEvents());
				members.AddRange(t.GetProperties());
				foreach (IMember i in members)
				{
					if ((!inStatic || i.IsStatic) && i.IsAccessible(callingClass, true))
					{
						result.Add(i);
					}
				}
				members.Clear();
				for (IClass c = callingClass.DeclaringType; c != null; c = c.DeclaringType)
				{
					t = c.DefaultReturnType;
					members.AddRange(t.GetMethods());
					members.AddRange(t.GetFields());
					members.AddRange(t.GetEvents());
					members.AddRange(t.GetProperties());
				}
				foreach (IMember i in members)
				{
					if (i.IsStatic)
					{
						result.Add(i);
					}
				}
			}
		}

		public static void AddImportedNamespaceContents(ArrayList result, ICompilationUnit cu, IClass callingClass)
		{
			IProjectContent projectContent = cu.ProjectContent;
			projectContent.AddNamespaceContents(result, "", projectContent.Language, true);
			foreach (IUsing u in cu.Usings)
			{
				CtrlSpaceResolveHelper.AddUsing(result, u, projectContent);
			}
			CtrlSpaceResolveHelper.AddUsing(result, projectContent.DefaultImports, projectContent);
			if (callingClass != null)
			{
				string[] namespaceParts = callingClass.Namespace.Split(new char[]
				{
					'.'
				});
				for (int i = 1; i <= namespaceParts.Length; i++)
				{
					foreach (object member in projectContent.GetNamespaceContents(string.Join(".", namespaceParts, 0, i)))
					{
						if (!result.Contains(member))
						{
							result.Add(member);
						}
					}
				}
				IClass currentClass = callingClass;
				do
				{
					foreach (IClass innerClass in currentClass.GetAccessibleTypes(currentClass))
					{
						if (!result.Contains(innerClass))
						{
							result.Add(innerClass);
						}
					}
					currentClass = currentClass.DeclaringType;
				}
				while (currentClass != null);
			}
		}

		public static void AddUsing(ArrayList result, IUsing u, IProjectContent projectContent)
		{
			if (u != null)
			{
				bool importNamespaces = projectContent.Language.ImportNamespaces;
				bool importClasses = projectContent.Language.CanImportClasses;
				foreach (string name in u.Usings)
				{
					if (importClasses)
					{
						IClass c = projectContent.GetClass(name, 0);
						if (c != null)
						{
							ArrayList members = new ArrayList();
							IReturnType t = c.DefaultReturnType;
							members.AddRange(t.GetMethods());
							members.AddRange(t.GetFields());
							members.AddRange(t.GetEvents());
							members.AddRange(t.GetProperties());
							foreach (IMember i in members)
							{
								if (i.IsStatic && i.IsPublic)
								{
									result.Add(i);
								}
							}
							continue;
						}
					}
					if (importNamespaces)
					{
						string newName = null;
						if (projectContent.DefaultImports != null)
						{
							newName = projectContent.DefaultImports.SearchNamespace(name);
						}
						projectContent.AddNamespaceContents(result, newName ?? name, projectContent.Language, true);
					}
					else
					{
						foreach (object o in projectContent.GetNamespaceContents(name))
						{
							if (!(o is string))
							{
								result.Add(o);
							}
						}
					}
				}
				if (u.HasAliases)
				{
					foreach (string alias in u.Aliases.Keys)
					{
						result.Add(alias);
					}
				}
			}
		}

		[Obsolete]
		public static ResolveResult GetResultFromDeclarationLine(IClass callingClass, IMethodOrProperty callingMember, int caretLine, int caretColumn, string expression)
		{
			return CtrlSpaceResolveHelper.GetResultFromDeclarationLine(callingClass, callingMember, caretLine, caretColumn, new ExpressionResult(expression));
		}

		public static ResolveResult GetResultFromDeclarationLine(IClass callingClass, IMethodOrProperty callingMember, int caretLine, int caretColumn, ExpressionResult expressionResult)
		{
			string expression = expressionResult.Expression;
			ResolveResult result;
			if (callingClass == null)
			{
				result = null;
			}
			else
			{
				int pos = expression.IndexOf('(');
				if (pos >= 0)
				{
					expression = expression.Substring(0, pos);
				}
				expression = expression.Trim();
				if (!callingClass.BodyRegion.IsInside(caretLine, caretColumn) && callingClass.ProjectContent.Language.NameComparer.Equals(expression, callingClass.Name))
				{
					result = new TypeResolveResult(callingClass, callingMember, callingClass);
				}
				else
				{
					if (expressionResult.Context != ExpressionContext.Type)
					{
						if (callingMember != null && !callingMember.BodyRegion.IsInside(caretLine, caretColumn) && callingClass.ProjectContent.Language.NameComparer.Equals(expression, callingMember.Name))
						{
							result = new MemberResolveResult(callingClass, callingMember, callingMember);
							return result;
						}
					}
					result = null;
				}
			}
			return result;
		}
	}
}

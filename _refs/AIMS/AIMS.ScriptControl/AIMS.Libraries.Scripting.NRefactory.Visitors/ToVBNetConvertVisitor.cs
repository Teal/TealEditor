using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public class ToVBNetConvertVisitor : AbstractAstTransformer
	{
		private List<INode> nodesToMoveToCompilationUnit = new List<INode>();

		private TypeDeclaration currentType;

		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			for (int i = 0; i < this.nodesToMoveToCompilationUnit.Count; i++)
			{
				compilationUnit.Children.Insert(i, this.nodesToMoveToCompilationUnit[i]);
				this.nodesToMoveToCompilationUnit[i].Parent = compilationUnit;
			}
			return null;
		}

		public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			base.VisitUsingDeclaration(usingDeclaration, data);
			if (usingDeclaration.Parent is NamespaceDeclaration)
			{
				this.nodesToMoveToCompilationUnit.Add(usingDeclaration);
				base.RemoveCurrentNode();
			}
			return null;
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (this.currentType != null && (typeDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				typeDeclaration.Modifier |= Modifiers.Private;
			}
			TypeDeclaration outerType = this.currentType;
			this.currentType = typeDeclaration;
			if ((typeDeclaration.Modifier & Modifiers.Static) == Modifiers.Static)
			{
				typeDeclaration.Modifier &= ~Modifiers.Static;
				typeDeclaration.Modifier |= Modifiers.Sealed;
				typeDeclaration.Children.Insert(0, new ConstructorDeclaration("#ctor", Modifiers.Private, null, null));
			}
			List<string> properties = new List<string>();
			foreach (object o in typeDeclaration.Children)
			{
				PropertyDeclaration pd = o as PropertyDeclaration;
				if (pd != null)
				{
					properties.Add(pd.Name);
				}
			}
			List<VariableDeclaration> conflicts = new List<VariableDeclaration>();
			foreach (object o in typeDeclaration.Children)
			{
				FieldDeclaration fd = o as FieldDeclaration;
				if (fd != null)
				{
					foreach (VariableDeclaration var in fd.Fields)
					{
						string name = var.Name;
						foreach (string propertyName in properties)
						{
							if (name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
							{
								conflicts.Add(var);
							}
						}
					}
				}
			}
			new PrefixFieldsVisitor(conflicts, "m_").Run(typeDeclaration);
			base.VisitTypeDeclaration(typeDeclaration, data);
			this.currentType = outerType;
			return null;
		}

		public override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			if (this.currentType != null && (delegateDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				delegateDeclaration.Modifier |= Modifiers.Private;
			}
			return base.VisitDelegateDeclaration(delegateDeclaration, data);
		}

		private string GetAnonymousMethodName()
		{
			int i = 1;
			string name;
			while (true)
			{
				name = "ConvertedAnonymousMethod" + i;
				bool ok = true;
				if (this.currentType != null)
				{
					foreach (object c in this.currentType.Children)
					{
						MethodDeclaration method = c as MethodDeclaration;
						if (method != null && method.Name == name)
						{
							ok = false;
							break;
						}
					}
				}
				if (ok)
				{
					break;
				}
				i++;
			}
			return name;
		}

		public override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			base.VisitExpressionStatement(expressionStatement, data);
			AssignmentExpression ass = expressionStatement.Expression as AssignmentExpression;
			if (ass != null && ass.Right is AddressOfExpression)
			{
				if (ass.Op == AssignmentOperatorType.Add)
				{
					base.ReplaceCurrentNode(new AddHandlerStatement(ass.Left, ass.Right));
				}
				else if (ass.Op == AssignmentOperatorType.Subtract)
				{
					base.ReplaceCurrentNode(new RemoveHandlerStatement(ass.Left, ass.Right));
				}
			}
			return null;
		}

		private static string GetMemberNameOnThisReference(Expression expr)
		{
			IdentifierExpression ident = expr as IdentifierExpression;
			string result;
			if (ident != null)
			{
				result = ident.Identifier;
			}
			else
			{
				FieldReferenceExpression fre = expr as FieldReferenceExpression;
				if (fre != null && fre.TargetObject is ThisReferenceExpression)
				{
					result = fre.FieldName;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private static string GetMethodNameOfDelegateCreation(Expression expr)
		{
			string name = ToVBNetConvertVisitor.GetMemberNameOnThisReference(expr);
			string result;
			if (name != null)
			{
				result = name;
			}
			else
			{
				ObjectCreateExpression oce = expr as ObjectCreateExpression;
				if (oce != null && oce.Parameters.Count == 1)
				{
					result = ToVBNetConvertVisitor.GetMemberNameOnThisReference(oce.Parameters[0]);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			MethodDeclaration method = new MethodDeclaration(this.GetAnonymousMethodName(), Modifiers.Private, new TypeReference("System.Void"), anonymousMethodExpression.Parameters, null);
			method.Body = anonymousMethodExpression.Body;
			if (this.currentType != null)
			{
				this.currentType.Children.Add(method);
			}
			base.ReplaceCurrentNode(new AddressOfExpression(new IdentifierExpression(method.Name)));
			return null;
		}

		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			if (assignmentExpression.Op == AssignmentOperatorType.Add || assignmentExpression.Op == AssignmentOperatorType.Subtract)
			{
				string methodName = ToVBNetConvertVisitor.GetMethodNameOfDelegateCreation(assignmentExpression.Right);
				if (methodName != null && this.currentType != null)
				{
					foreach (object c in this.currentType.Children)
					{
						MethodDeclaration method = c as MethodDeclaration;
						if (method != null && method.Name == methodName)
						{
							assignmentExpression.Right = new AddressOfExpression(new IdentifierExpression(methodName));
							break;
						}
					}
				}
			}
			return base.VisitAssignmentExpression(assignmentExpression, data);
		}

		private bool IsClassType(ClassType c)
		{
			return this.currentType != null && this.currentType.Type == c;
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (!this.IsClassType(ClassType.Interface) && (methodDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				methodDeclaration.Modifier |= Modifiers.Private;
			}
			base.VisitMethodDeclaration(methodDeclaration, data);
			if ((methodDeclaration.Modifier & (Modifiers.Static | Modifiers.Extern)) == (Modifiers.Static | Modifiers.Extern) && methodDeclaration.Body.IsNull)
			{
				foreach (AttributeSection sec in methodDeclaration.Attributes)
				{
					foreach (AIMS.Libraries.Scripting.NRefactory.Ast.Attribute att in sec.Attributes)
					{
						if ("DllImport".Equals(att.Name, StringComparison.InvariantCultureIgnoreCase))
						{
							if (this.ConvertPInvoke(methodDeclaration, att))
							{
								sec.Attributes.Remove(att);
								break;
							}
						}
					}
					if (sec.Attributes.Count == 0)
					{
						methodDeclaration.Attributes.Remove(sec);
						break;
					}
				}
			}
			return null;
		}

		private bool ConvertPInvoke(MethodDeclaration method, AIMS.Libraries.Scripting.NRefactory.Ast.Attribute att)
		{
			bool result;
			if (att.PositionalArguments.Count != 1)
			{
				result = false;
			}
			else
			{
				PrimitiveExpression pe = att.PositionalArguments[0] as PrimitiveExpression;
				if (pe == null || !(pe.Value is string))
				{
					result = false;
				}
				else
				{
					string libraryName = (string)pe.Value;
					string alias = null;
					bool setLastError = false;
					bool exactSpelling = false;
					CharsetModifier charSet = CharsetModifier.Auto;
					foreach (NamedArgumentExpression arg in att.NamedArguments)
					{
						string text = arg.Name;
						if (text != null)
						{
							if (!(text == "SetLastError"))
							{
								if (!(text == "ExactSpelling"))
								{
									if (!(text == "CharSet"))
									{
										if (!(text == "EntryPoint"))
										{
											goto IL_239;
										}
										pe = (arg.Expression as PrimitiveExpression);
										if (pe != null)
										{
											alias = (pe.Value as string);
										}
									}
									else
									{
										FieldReferenceExpression fre = arg.Expression as FieldReferenceExpression;
										if (fre == null || !(fre.TargetObject is IdentifierExpression))
										{
											result = false;
											return result;
										}
										if ((fre.TargetObject as IdentifierExpression).Identifier != "CharSet")
										{
											result = false;
											return result;
										}
										text = fre.FieldName;
										if (text != null)
										{
											if (!(text == "Unicode"))
											{
												if (!(text == "Auto"))
												{
													if (!(text == "Ansi"))
													{
														goto IL_209;
													}
													charSet = CharsetModifier.Ansi;
												}
												else
												{
													charSet = CharsetModifier.Auto;
												}
											}
											else
											{
												charSet = CharsetModifier.Unicode;
											}
											continue;
										}
										IL_209:
										result = false;
										return result;
									}
								}
								else
								{
									pe = (arg.Expression as PrimitiveExpression);
									if (pe == null || !(pe.Value is bool))
									{
										result = false;
										return result;
									}
									exactSpelling = (bool)pe.Value;
								}
							}
							else
							{
								pe = (arg.Expression as PrimitiveExpression);
								if (pe == null || !(pe.Value is bool))
								{
									result = false;
									return result;
								}
								setLastError = (bool)pe.Value;
							}
							continue;
						}
						IL_239:
						result = false;
						return result;
					}
					if (setLastError && exactSpelling)
					{
						DeclareDeclaration decl = new DeclareDeclaration(method.Name, method.Modifier & ~(Modifiers.Static | Modifiers.Extern), method.TypeReference, method.Parameters, method.Attributes, libraryName, alias, charSet);
						base.ReplaceCurrentNode(decl);
						base.VisitDeclareDeclaration(decl, null);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if (!this.IsClassType(ClassType.Interface) && (propertyDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				propertyDeclaration.Modifier |= Modifiers.Private;
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}

		public override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			if (!this.IsClassType(ClassType.Interface) && (eventDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				eventDeclaration.Modifier |= Modifiers.Private;
			}
			return base.VisitEventDeclaration(eventDeclaration, data);
		}

		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if ((constructorDeclaration.Modifier & (Modifiers.Private | Modifiers.Internal | Modifiers.Protected | Modifiers.Public | Modifiers.Static)) == Modifiers.None)
			{
				constructorDeclaration.Modifier |= Modifiers.Private;
			}
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}

		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			base.VisitParenthesizedExpression(parenthesizedExpression, data);
			if (parenthesizedExpression.Expression is CastExpression)
			{
				base.ReplaceCurrentNode(parenthesizedExpression.Expression);
			}
			return null;
		}

		public override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			for (int i = 0; i < arrayCreateExpression.Arguments.Count; i++)
			{
				arrayCreateExpression.Arguments[i] = Expression.AddInteger(arrayCreateExpression.Arguments[i], -1);
			}
			return base.VisitArrayCreateExpression(arrayCreateExpression, data);
		}
	}
}

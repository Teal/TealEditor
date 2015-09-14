using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public class VBNetConstructsConvertVisitor : AbstractAstTransformer
	{
		private class ReturnStatementForFunctionAssignment : AbstractAstTransformer
		{
			private string functionName;

			internal int replacementCount = 0;

			public ReturnStatementForFunctionAssignment(string functionName)
			{
				this.functionName = functionName;
			}

			public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
			{
				if (identifierExpression.Identifier.Equals(this.functionName, StringComparison.InvariantCultureIgnoreCase))
				{
					if (!(identifierExpression.Parent is AddressOfExpression) && !(identifierExpression.Parent is InvocationExpression))
					{
						identifierExpression.Identifier = "functionReturnValue";
						this.replacementCount++;
					}
				}
				return base.VisitIdentifierExpression(identifierExpression, data);
			}
		}

		private class RenameIdentifierVisitor : AbstractAstVisitor
		{
			private string from;

			private string to;

			public RenameIdentifierVisitor(string from, string to)
			{
				this.from = from;
				this.to = to;
			}

			public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
			{
				if (string.Equals(identifierExpression.Identifier, this.from, StringComparison.InvariantCultureIgnoreCase))
				{
					identifierExpression.Identifier = this.to;
				}
				return base.VisitIdentifierExpression(identifierExpression, data);
			}
		}

		public const string FunctionReturnValueName = "functionReturnValue";

		private Dictionary<string, string> usings;

		private List<UsingDeclaration> addedUsings;

		private TypeDeclaration currentTypeDeclaration;

		private static volatile Dictionary<string, Expression> constantTable;

		private static volatile Dictionary<string, Expression> methodTable;

		public static readonly string VBAssemblyName = "Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			this.usings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			this.addedUsings = new List<UsingDeclaration>();
			base.VisitCompilationUnit(compilationUnit, data);
			int i;
			for (i = 0; i < compilationUnit.Children.Count; i++)
			{
				if (!(compilationUnit.Children[i] is UsingDeclaration))
				{
					break;
				}
			}
			foreach (UsingDeclaration decl in this.addedUsings)
			{
				decl.Parent = compilationUnit;
				compilationUnit.Children.Insert(i++, decl);
			}
			this.usings = null;
			this.addedUsings = null;
			return null;
		}

		public override object VisitUsing(Using @using, object data)
		{
			if (this.usings != null && !@using.IsAlias)
			{
				this.usings[@using.Name] = @using.Name;
			}
			return base.VisitUsing(@using, data);
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (this.currentTypeDeclaration != null && (typeDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				typeDeclaration.Modifier |= Modifiers.Public;
			}
			TypeDeclaration oldTypeDeclaration = this.currentTypeDeclaration;
			this.currentTypeDeclaration = typeDeclaration;
			base.VisitTypeDeclaration(typeDeclaration, data);
			this.currentTypeDeclaration = oldTypeDeclaration;
			return null;
		}

		public override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			if (this.currentTypeDeclaration != null && (delegateDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				delegateDeclaration.Modifier |= Modifiers.Public;
			}
			return base.VisitDelegateDeclaration(delegateDeclaration, data);
		}

		private bool IsClassType(ClassType c)
		{
			return this.currentTypeDeclaration != null && this.currentTypeDeclaration.Type == c;
		}

		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if ((constructorDeclaration.Modifier & (Modifiers.Private | Modifiers.Internal | Modifiers.Protected | Modifiers.Public | Modifiers.Static)) == Modifiers.None)
			{
				constructorDeclaration.Modifier |= Modifiers.Public;
			}
			BlockStatement body = constructorDeclaration.Body;
			if (body != null && body.Children.Count > 0)
			{
				ExpressionStatement se = body.Children[0] as ExpressionStatement;
				if (se != null)
				{
					InvocationExpression ie = se.Expression as InvocationExpression;
					if (ie != null)
					{
						FieldReferenceExpression fre = ie.TargetObject as FieldReferenceExpression;
						if (fre != null && "New".Equals(fre.FieldName, StringComparison.InvariantCultureIgnoreCase))
						{
							if (fre.TargetObject is BaseReferenceExpression || fre.TargetObject is ClassReferenceExpression || fre.TargetObject is ThisReferenceExpression)
							{
								body.Children.RemoveAt(0);
								ConstructorInitializer ci = new ConstructorInitializer();
								ci.Arguments = ie.Arguments;
								if (fre.TargetObject is BaseReferenceExpression)
								{
									ci.ConstructorInitializerType = ConstructorInitializerType.Base;
								}
								else
								{
									ci.ConstructorInitializerType = ConstructorInitializerType.This;
								}
								constructorDeclaration.ConstructorInitializer = ci;
							}
						}
					}
				}
			}
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}

		public override object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			if (this.usings != null && !this.usings.ContainsKey("System.Runtime.InteropServices"))
			{
				UsingDeclaration @using = new UsingDeclaration("System.Runtime.InteropServices");
				this.addedUsings.Add(@using);
				base.VisitUsingDeclaration(@using, data);
			}
			MethodDeclaration method = new MethodDeclaration(declareDeclaration.Name, declareDeclaration.Modifier, declareDeclaration.TypeReference, declareDeclaration.Parameters, declareDeclaration.Attributes);
			if ((method.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				method.Modifier |= Modifiers.Public;
			}
			method.Modifier |= (Modifiers.Static | Modifiers.Extern);
			if (method.TypeReference.IsNull)
			{
				method.TypeReference = new TypeReference("System.Void");
			}
			AIMS.Libraries.Scripting.NRefactory.Ast.Attribute att = new AIMS.Libraries.Scripting.NRefactory.Ast.Attribute("DllImport", null, null);
			att.PositionalArguments.Add(VBNetConstructsConvertVisitor.CreateStringLiteral(declareDeclaration.Library));
			if (declareDeclaration.Alias.Length > 0)
			{
				att.NamedArguments.Add(new NamedArgumentExpression("EntryPoint", VBNetConstructsConvertVisitor.CreateStringLiteral(declareDeclaration.Alias)));
			}
			switch (declareDeclaration.Charset)
			{
			case CharsetModifier.Auto:
				att.NamedArguments.Add(new NamedArgumentExpression("CharSet", new FieldReferenceExpression(new IdentifierExpression("CharSet"), "Auto")));
				break;
			case CharsetModifier.Unicode:
				att.NamedArguments.Add(new NamedArgumentExpression("CharSet", new FieldReferenceExpression(new IdentifierExpression("CharSet"), "Unicode")));
				break;
			default:
				att.NamedArguments.Add(new NamedArgumentExpression("CharSet", new FieldReferenceExpression(new IdentifierExpression("CharSet"), "Ansi")));
				break;
			}
			att.NamedArguments.Add(new NamedArgumentExpression("SetLastError", new PrimitiveExpression(true, true.ToString())));
			att.NamedArguments.Add(new NamedArgumentExpression("ExactSpelling", new PrimitiveExpression(true, true.ToString())));
			AttributeSection sec = new AttributeSection(null, null);
			sec.Attributes.Add(att);
			method.Attributes.Add(sec);
			base.ReplaceCurrentNode(method);
			return base.VisitMethodDeclaration(method, data);
		}

		private static PrimitiveExpression CreateStringLiteral(string text)
		{
			return new PrimitiveExpression(text, text);
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (!this.IsClassType(ClassType.Interface) && (methodDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				methodDeclaration.Modifier |= Modifiers.Public;
			}
			object result;
			if ("Finalize".Equals(methodDeclaration.Name, StringComparison.InvariantCultureIgnoreCase) && methodDeclaration.Parameters.Count == 0 && methodDeclaration.Modifier == (Modifiers.Protected | Modifiers.Override) && methodDeclaration.Body.Children.Count == 1)
			{
				TryCatchStatement tcs = methodDeclaration.Body.Children[0] as TryCatchStatement;
				if (tcs != null && tcs.StatementBlock is BlockStatement && tcs.CatchClauses.Count == 0 && tcs.FinallyBlock is BlockStatement && tcs.FinallyBlock.Children.Count == 1)
				{
					ExpressionStatement se = tcs.FinallyBlock.Children[0] as ExpressionStatement;
					if (se != null)
					{
						InvocationExpression ie = se.Expression as InvocationExpression;
						if (ie != null && ie.Arguments.Count == 0 && ie.TargetObject is FieldReferenceExpression && (ie.TargetObject as FieldReferenceExpression).TargetObject is BaseReferenceExpression && "Finalize".Equals((ie.TargetObject as FieldReferenceExpression).FieldName, StringComparison.InvariantCultureIgnoreCase))
						{
							DestructorDeclaration des = new DestructorDeclaration("Destructor", Modifiers.None, methodDeclaration.Attributes);
							base.ReplaceCurrentNode(des);
							des.Body = (BlockStatement)tcs.StatementBlock;
							result = base.VisitDestructorDeclaration(des, data);
							return result;
						}
					}
				}
			}
			if ((methodDeclaration.Modifier & (Modifiers.Static | Modifiers.Extern)) == Modifiers.Static && methodDeclaration.Body.Children.Count == 0)
			{
				foreach (AttributeSection sec in methodDeclaration.Attributes)
				{
					foreach (AIMS.Libraries.Scripting.NRefactory.Ast.Attribute att in sec.Attributes)
					{
						if ("DllImport".Equals(att.Name, StringComparison.InvariantCultureIgnoreCase))
						{
							methodDeclaration.Modifier |= Modifiers.Extern;
							methodDeclaration.Body = null;
						}
					}
				}
			}
			if (methodDeclaration.TypeReference.SystemType != "System.Void" && methodDeclaration.Body.Children.Count > 0)
			{
				if (VBNetConstructsConvertVisitor.IsAssignmentTo(methodDeclaration.Body.Children[methodDeclaration.Body.Children.Count - 1], methodDeclaration.Name))
				{
					ReturnStatement rs = new ReturnStatement(VBNetConstructsConvertVisitor.GetAssignmentFromStatement(methodDeclaration.Body.Children[methodDeclaration.Body.Children.Count - 1]).Right);
					methodDeclaration.Body.Children.RemoveAt(methodDeclaration.Body.Children.Count - 1);
					methodDeclaration.Body.AddChild(rs);
				}
				else
				{
					VBNetConstructsConvertVisitor.ReturnStatementForFunctionAssignment visitor = new VBNetConstructsConvertVisitor.ReturnStatementForFunctionAssignment(methodDeclaration.Name);
					methodDeclaration.Body.AcceptVisitor(visitor, null);
					if (visitor.replacementCount > 0)
					{
						string systemType = methodDeclaration.TypeReference.SystemType;
						Expression init;
						switch (systemType)
						{
						case "System.Int16":
						case "System.Int32":
						case "System.Int64":
						case "System.Byte":
						case "System.UInt16":
						case "System.UInt32":
						case "System.UInt64":
							init = new PrimitiveExpression(0, "0");
							goto IL_4A2;
						case "System.Boolean":
							init = new PrimitiveExpression(false, "false");
							goto IL_4A2;
						}
						init = new PrimitiveExpression(null, "null");
						IL_4A2:
						methodDeclaration.Body.Children.Insert(0, new LocalVariableDeclaration(new VariableDeclaration("functionReturnValue", init, methodDeclaration.TypeReference)));
						methodDeclaration.Body.Children[0].Parent = methodDeclaration.Body;
						methodDeclaration.Body.AddChild(new ReturnStatement(new IdentifierExpression("functionReturnValue")));
					}
				}
			}
			result = base.VisitMethodDeclaration(methodDeclaration, data);
			return result;
		}

		private static AssignmentExpression GetAssignmentFromStatement(INode statement)
		{
			ExpressionStatement se = statement as ExpressionStatement;
			AssignmentExpression result;
			if (se == null)
			{
				result = null;
			}
			else
			{
				result = (se.Expression as AssignmentExpression);
			}
			return result;
		}

		private static bool IsAssignmentTo(INode statement, string varName)
		{
			AssignmentExpression ass = VBNetConstructsConvertVisitor.GetAssignmentFromStatement(statement);
			bool result;
			if (ass == null)
			{
				result = false;
			}
			else
			{
				IdentifierExpression ident = ass.Left as IdentifierExpression;
				result = (ident != null && ident.Identifier.Equals(varName, StringComparison.InvariantCultureIgnoreCase));
			}
			return result;
		}

		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			fieldDeclaration.Modifier &= ~Modifiers.Dim;
			if (this.IsClassType(ClassType.Struct))
			{
				if ((fieldDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
				{
					fieldDeclaration.Modifier |= Modifiers.Public;
				}
			}
			return base.VisitFieldDeclaration(fieldDeclaration, data);
		}

		public override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			if (!this.IsClassType(ClassType.Interface) && (eventDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				eventDeclaration.Modifier |= Modifiers.Public;
			}
			return base.VisitEventDeclaration(eventDeclaration, data);
		}

		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if (!this.IsClassType(ClassType.Interface) && (propertyDeclaration.Modifier & Modifiers.Visibility) == Modifiers.None)
			{
				propertyDeclaration.Modifier |= Modifiers.Public;
			}
			if (propertyDeclaration.HasSetRegion)
			{
				string from = "Value";
				if (propertyDeclaration.SetRegion.Parameters.Count > 0)
				{
					ParameterDeclarationExpression p = propertyDeclaration.SetRegion.Parameters[0];
					from = p.ParameterName;
					p.ParameterName = "Value";
				}
				propertyDeclaration.SetRegion.AcceptVisitor(new VBNetConstructsConvertVisitor.RenameIdentifierVisitor(from, "value"), null);
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}

		private static Dictionary<string, Expression> CreateDictionary(params string[] classNames)
		{
			Dictionary<string, Expression> d = new Dictionary<string, Expression>(StringComparer.InvariantCultureIgnoreCase);
			Assembly asm = Assembly.Load(VBNetConstructsConvertVisitor.VBAssemblyName);
			for (int i = 0; i < classNames.Length; i++)
			{
				string className = classNames[i];
				Type type = asm.GetType("Microsoft.VisualBasic." + className);
				Expression expr = new IdentifierExpression(className);
				MemberInfo[] members = type.GetMembers();
				for (int j = 0; j < members.Length; j++)
				{
					MemberInfo member = members[j];
					if (member.DeclaringType == type)
					{
						d[member.Name] = expr;
					}
				}
			}
			return d;
		}

		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (VBNetConstructsConvertVisitor.constantTable == null)
			{
				VBNetConstructsConvertVisitor.constantTable = VBNetConstructsConvertVisitor.CreateDictionary(new string[]
				{
					"Constants"
				});
			}
			Expression expr;
			object result;
			if (VBNetConstructsConvertVisitor.constantTable.TryGetValue(identifierExpression.Identifier, out expr))
			{
				FieldReferenceExpression fre = new FieldReferenceExpression(expr, identifierExpression.Identifier);
				base.ReplaceCurrentNode(fre);
				result = base.VisitFieldReferenceExpression(fre, data);
			}
			else
			{
				result = base.VisitIdentifierExpression(identifierExpression, data);
			}
			return result;
		}

		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			IdentifierExpression ident = invocationExpression.TargetObject as IdentifierExpression;
			object result;
			if (ident != null)
			{
				if ("IIF".Equals(ident.Identifier, StringComparison.InvariantCultureIgnoreCase) && invocationExpression.Arguments.Count == 3)
				{
					ConditionalExpression ce = new ConditionalExpression(invocationExpression.Arguments[0], invocationExpression.Arguments[1], invocationExpression.Arguments[2]);
					base.ReplaceCurrentNode(new ParenthesizedExpression(ce));
					result = base.VisitConditionalExpression(ce, data);
					return result;
				}
				if ("IsNothing".Equals(ident.Identifier, StringComparison.InvariantCultureIgnoreCase) && invocationExpression.Arguments.Count == 1)
				{
					BinaryOperatorExpression boe = new BinaryOperatorExpression(invocationExpression.Arguments[0], BinaryOperatorType.ReferenceEquality, new PrimitiveExpression(null, "null"));
					base.ReplaceCurrentNode(new ParenthesizedExpression(boe));
					result = base.VisitBinaryOperatorExpression(boe, data);
					return result;
				}
				if (VBNetConstructsConvertVisitor.methodTable == null)
				{
					VBNetConstructsConvertVisitor.methodTable = VBNetConstructsConvertVisitor.CreateDictionary(new string[]
					{
						"Conversion",
						"FileSystem",
						"Financial",
						"Information",
						"Interaction",
						"Strings",
						"VBMath"
					});
				}
				Expression expr;
				if (VBNetConstructsConvertVisitor.methodTable.TryGetValue(ident.Identifier, out expr))
				{
					FieldReferenceExpression fre = new FieldReferenceExpression(expr, ident.Identifier);
					invocationExpression.TargetObject = fre;
				}
			}
			result = base.VisitInvocationExpression(invocationExpression, data);
			return result;
		}

		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			base.VisitUnaryOperatorExpression(unaryOperatorExpression, data);
			if (unaryOperatorExpression.Op == UnaryOperatorType.Not)
			{
				if (unaryOperatorExpression.Expression is BinaryOperatorExpression)
				{
					unaryOperatorExpression.Expression = new ParenthesizedExpression(unaryOperatorExpression.Expression);
				}
				ParenthesizedExpression pe = unaryOperatorExpression.Expression as ParenthesizedExpression;
				if (pe != null)
				{
					BinaryOperatorExpression boe = pe.Expression as BinaryOperatorExpression;
					if (boe != null && boe.Op == BinaryOperatorType.ReferenceEquality)
					{
						boe.Op = BinaryOperatorType.ReferenceInequality;
						base.ReplaceCurrentNode(pe);
					}
				}
			}
			return null;
		}

		public override object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			LocalVariableDeclaration lvd = usingStatement.ResourceAcquisition as LocalVariableDeclaration;
			if (lvd != null && lvd.Variables.Count > 1)
			{
				usingStatement.ResourceAcquisition = new LocalVariableDeclaration(lvd.Variables[0]);
				for (int i = 1; i < lvd.Variables.Count; i++)
				{
					UsingStatement j = new UsingStatement(new LocalVariableDeclaration(lvd.Variables[i]), usingStatement.EmbeddedStatement);
					usingStatement.EmbeddedStatement = new BlockStatement();
					usingStatement.EmbeddedStatement.AddChild(j);
					usingStatement = j;
				}
			}
			return base.VisitUsingStatement(usingStatement, data);
		}

		public override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			for (int i = 0; i < arrayCreateExpression.Arguments.Count; i++)
			{
				arrayCreateExpression.Arguments[i] = Expression.AddInteger(arrayCreateExpression.Arguments[i], 1);
			}
			if (arrayCreateExpression.ArrayInitializer.CreateExpressions.Count == 0)
			{
				arrayCreateExpression.ArrayInitializer = null;
			}
			return base.VisitArrayCreateExpression(arrayCreateExpression, data);
		}
	}
}

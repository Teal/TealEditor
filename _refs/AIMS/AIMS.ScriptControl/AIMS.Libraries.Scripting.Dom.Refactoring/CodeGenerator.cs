using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public abstract class CodeGenerator
	{
		private class DummyCodeGeneratorClass : CodeGenerator
		{
			public override string GenerateCode(AbstractNode node, string indentation)
			{
				return " -  there is no code generator for this language - ";
			}
		}

		public static readonly CodeGenerator DummyCodeGenerator = new CodeGenerator.DummyCodeGeneratorClass();

		private readonly CodeGeneratorOptions options = new CodeGeneratorOptions();

		[CompilerGenerated]
		private static Predicate<IMethod> <>9__CachedAnonymousMethodDelegate1;

		[CompilerGenerated]
		private static Comparison<KeyValuePair<int, int>> <>9__CachedAnonymousMethodDelegate3;

		public CodeGeneratorOptions Options
		{
			get
			{
				return this.options;
			}
		}

		protected CodeGenerator()
		{
			HostCallback.InitializeCodeGeneratorOptions(this);
		}

		public static TypeReference ConvertType(IReturnType returnType, ClassFinder context)
		{
			TypeReference result;
			if (returnType == null)
			{
				result = TypeReference.Null;
			}
			else if (returnType is NullReturnType)
			{
				result = TypeReference.Null;
			}
			else
			{
				TypeReference typeRef;
				if (context != null && CodeGenerator.CanUseShortTypeName(returnType, context))
				{
					typeRef = new TypeReference(returnType.Name);
				}
				else
				{
					typeRef = new TypeReference(returnType.FullyQualifiedName);
				}
				while (returnType.IsArrayReturnType)
				{
					int[] rank = typeRef.RankSpecifier ?? new int[0];
					Array.Resize<int>(ref rank, rank.Length + 1);
					rank[rank.Length - 1] = returnType.CastToArrayReturnType().ArrayDimensions - 1;
					typeRef.RankSpecifier = rank;
					returnType = returnType.CastToArrayReturnType().ArrayElementType;
				}
				if (returnType.IsConstructedReturnType)
				{
					foreach (IReturnType typeArgument in returnType.CastToConstructedReturnType().TypeArguments)
					{
						typeRef.GenericTypes.Add(CodeGenerator.ConvertType(typeArgument, context));
					}
				}
				result = typeRef;
			}
			return result;
		}

		public static bool CanUseShortTypeName(IReturnType returnType, ClassFinder context)
		{
			string fullyQualifiedName = returnType.FullyQualifiedName;
			bool result;
			switch (fullyQualifiedName)
			{
			case "System.Void":
			case "System.String":
			case "System.Char":
			case "System.Boolean":
			case "System.Single":
			case "System.Double":
			case "System.Decimal":
			case "System.Byte":
			case "System.SByte":
			case "System.Int16":
			case "System.Int32":
			case "System.Int64":
			case "System.UInt16":
			case "System.UInt32":
			case "System.UInt64":
				result = false;
				return result;
			}
			int typeArgumentCount = returnType.IsConstructedReturnType ? returnType.CastToConstructedReturnType().TypeArguments.Count : 0;
			IReturnType typeInTargetContext = context.SearchType(returnType.Name, typeArgumentCount);
			result = (typeInTargetContext != null && typeInTargetContext.FullyQualifiedName == returnType.FullyQualifiedName);
			return result;
		}

		public static Modifiers ConvertModifier(ModifierEnum modifiers, ClassFinder targetContext)
		{
			Modifiers result;
			if (targetContext != null && targetContext.ProjectContent != null && targetContext.CallingClass != null)
			{
				if (targetContext.ProjectContent.Language.IsClassWithImplicitlyStaticMembers(targetContext.CallingClass))
				{
					result = (Modifiers)(modifiers & ~ModifierEnum.Static);
					return result;
				}
			}
			result = (Modifiers)modifiers;
			return result;
		}

		public static AIMS.Libraries.Scripting.NRefactory.Ast.ParameterModifiers ConvertModifier(AIMS.Libraries.Scripting.Dom.ParameterModifiers m)
		{
			return (AIMS.Libraries.Scripting.NRefactory.Ast.ParameterModifiers)m;
		}

		public static UsingDeclaration ConvertUsing(IUsing u)
		{
			List<Using> usings = new List<Using>();
			foreach (string name in u.Usings)
			{
				usings.Add(new Using(name));
			}
			if (u.HasAliases)
			{
				foreach (KeyValuePair<string, IReturnType> pair in u.Aliases)
				{
					usings.Add(new Using(pair.Key, CodeGenerator.ConvertType(pair.Value, null)));
				}
			}
			return new UsingDeclaration(usings);
		}

		public static List<ParameterDeclarationExpression> ConvertParameters(IList<IParameter> parameters, ClassFinder targetContext)
		{
			List<ParameterDeclarationExpression> i = new List<ParameterDeclarationExpression>(parameters.Count);
			foreach (IParameter p in parameters)
			{
				i.Add(new ParameterDeclarationExpression(CodeGenerator.ConvertType(p.ReturnType, targetContext), p.Name, CodeGenerator.ConvertModifier(p.Modifiers))
				{
					Attributes = CodeGenerator.ConvertAttributes(p.Attributes, targetContext)
				});
			}
			return i;
		}

		public static List<AttributeSection> ConvertAttributes(IList<IAttribute> attributes, ClassFinder targetContext)
		{
			AttributeSection sec = new AttributeSection(null, null);
			foreach (IAttribute att in attributes)
			{
				sec.Attributes.Add(new AIMS.Libraries.Scripting.NRefactory.Ast.Attribute(att.Name, null, null));
			}
			List<AttributeSection> resultList = new List<AttributeSection>(1);
			if (sec.Attributes.Count > 0)
			{
				resultList.Add(sec);
			}
			return resultList;
		}

		public static List<TemplateDefinition> ConvertTemplates(IList<ITypeParameter> l, ClassFinder targetContext)
		{
			List<TemplateDefinition> o = new List<TemplateDefinition>(l.Count);
			foreach (ITypeParameter p in l)
			{
				TemplateDefinition td = new TemplateDefinition(p.Name, CodeGenerator.ConvertAttributes(p.Attributes, targetContext));
				foreach (IReturnType rt in p.Constraints)
				{
					td.Bases.Add(CodeGenerator.ConvertType(rt, targetContext));
				}
				o.Add(td);
			}
			return o;
		}

		public static BlockStatement CreateNotImplementedBlock()
		{
			BlockStatement b = new BlockStatement();
			b.AddChild(new ThrowStatement(new ObjectCreateExpression(new TypeReference("NotImplementedException"), null)));
			return b;
		}

		public static ParametrizedNode ConvertMember(IMethod m, ClassFinder targetContext)
		{
			ParametrizedNode result;
			if (m.IsConstructor)
			{
				result = new ConstructorDeclaration(m.Name, CodeGenerator.ConvertModifier(m.Modifiers, targetContext), CodeGenerator.ConvertParameters(m.Parameters, targetContext), CodeGenerator.ConvertAttributes(m.Attributes, targetContext));
			}
			else
			{
				result = new MethodDeclaration(m.Name, CodeGenerator.ConvertModifier(m.Modifiers, targetContext), CodeGenerator.ConvertType(m.ReturnType, targetContext), CodeGenerator.ConvertParameters(m.Parameters, targetContext), CodeGenerator.ConvertAttributes(m.Attributes, targetContext))
				{
					Templates = CodeGenerator.ConvertTemplates(m.TypeParameters, targetContext),
					Body = CodeGenerator.CreateNotImplementedBlock()
				};
			}
			return result;
		}

		public static AttributedNode ConvertMember(IMember m, ClassFinder targetContext)
		{
			if (m == null)
			{
				throw new ArgumentNullException("m");
			}
			AttributedNode result;
			if (m is IProperty)
			{
				result = CodeGenerator.ConvertMember((IProperty)m, targetContext);
			}
			else if (m is IMethod)
			{
				result = CodeGenerator.ConvertMember((IMethod)m, targetContext);
			}
			else if (m is IEvent)
			{
				result = CodeGenerator.ConvertMember((IEvent)m, targetContext);
			}
			else
			{
				if (!(m is IField))
				{
					throw new ArgumentException("Unknown member: " + m.GetType().FullName);
				}
				result = CodeGenerator.ConvertMember((IField)m, targetContext);
			}
			return result;
		}

		public static AttributedNode ConvertMember(IProperty p, ClassFinder targetContext)
		{
			AttributedNode result;
			if (p.IsIndexer)
			{
				IndexerDeclaration md = new IndexerDeclaration(CodeGenerator.ConvertType(p.ReturnType, targetContext), CodeGenerator.ConvertParameters(p.Parameters, targetContext), CodeGenerator.ConvertModifier(p.Modifiers, targetContext), CodeGenerator.ConvertAttributes(p.Attributes, targetContext));
				md.Parameters = CodeGenerator.ConvertParameters(p.Parameters, targetContext);
				if (p.CanGet)
				{
					md.GetRegion = new PropertyGetRegion(CodeGenerator.CreateNotImplementedBlock(), null);
				}
				if (p.CanSet)
				{
					md.SetRegion = new PropertySetRegion(CodeGenerator.CreateNotImplementedBlock(), null);
				}
				result = md;
			}
			else
			{
				PropertyDeclaration md2 = new PropertyDeclaration(CodeGenerator.ConvertModifier(p.Modifiers, targetContext), CodeGenerator.ConvertAttributes(p.Attributes, targetContext), p.Name, CodeGenerator.ConvertParameters(p.Parameters, targetContext));
				md2.TypeReference = CodeGenerator.ConvertType(p.ReturnType, targetContext);
				if (p.CanGet)
				{
					md2.GetRegion = new PropertyGetRegion(CodeGenerator.CreateNotImplementedBlock(), null);
				}
				if (p.CanSet)
				{
					md2.SetRegion = new PropertySetRegion(CodeGenerator.CreateNotImplementedBlock(), null);
				}
				result = md2;
			}
			return result;
		}

		public static FieldDeclaration ConvertMember(IField f, ClassFinder targetContext)
		{
			TypeReference type = CodeGenerator.ConvertType(f.ReturnType, targetContext);
			return new FieldDeclaration(CodeGenerator.ConvertAttributes(f.Attributes, targetContext), type, CodeGenerator.ConvertModifier(f.Modifiers, targetContext))
			{
				Fields = 
				{
					new VariableDeclaration(f.Name, null, type)
				}
			};
		}

		public static EventDeclaration ConvertMember(IEvent e, ClassFinder targetContext)
		{
			return new EventDeclaration(CodeGenerator.ConvertType(e.ReturnType, targetContext), e.Name, CodeGenerator.ConvertModifier(e.Modifiers, targetContext), CodeGenerator.ConvertAttributes(e.Attributes, targetContext), null);
		}

		public virtual void InsertCodeAfter(IMember member, IDocument document, params AbstractNode[] nodes)
		{
			if (member is IMethodOrProperty)
			{
				this.InsertCodeAfter(((IMethodOrProperty)member).BodyRegion.EndLine, document, this.GetIndentation(document, member.Region.BeginLine), nodes);
			}
			else
			{
				this.InsertCodeAfter(member.Region.EndLine, document, this.GetIndentation(document, member.Region.BeginLine), nodes);
			}
		}

		public virtual void InsertCodeAtEnd(DomRegion region, IDocument document, params AbstractNode[] nodes)
		{
			this.InsertCodeAfter(region.EndLine - 1, document, this.GetIndentation(document, region.BeginLine) + this.options.IndentString, nodes);
		}

		public virtual void InsertCodeInClass(IClass c, IDocument document, int targetLine, params AbstractNode[] nodes)
		{
			this.InsertCodeAfter(targetLine, document, this.GetIndentation(document, c.Region.BeginLine) + this.options.IndentString, false, nodes);
		}

		protected string GetIndentation(IDocument document, int line)
		{
			string lineText = document.GetLine(line).Text;
			return lineText.Substring(0, lineText.Length - lineText.TrimStart(new char[0]).Length);
		}

		protected void InsertCodeAfter(int insertLine, IDocument document, string indentation, params AbstractNode[] nodes)
		{
			this.InsertCodeAfter(insertLine, document, indentation, true, nodes);
		}

		protected void InsertCodeAfter(int insertLine, IDocument document, string indentation, bool startWithEmptyLine, params AbstractNode[] nodes)
		{
			IDocumentLine lineSegment = document.GetLine(insertLine + 1);
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < nodes.Length; i++)
			{
				if (this.options.EmptyLinesBetweenMembers)
				{
					if (startWithEmptyLine || i > 0)
					{
						b.AppendLine(indentation);
					}
				}
				b.Append(this.GenerateCode(nodes[i], indentation));
			}
			document.Insert(lineSegment.Offset, b.ToString());
			document.UpdateView();
		}

		public abstract string GenerateCode(AbstractNode node, string indentation);

		public virtual string GetPropertyName(string fieldName)
		{
			string result;
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
			{
				result = char.ToUpper(fieldName[1]) + fieldName.Substring(2);
			}
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
			{
				result = char.ToUpper(fieldName[2]) + fieldName.Substring(3);
			}
			else
			{
				result = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
			}
			return result;
		}

		public virtual string GetParameterName(string fieldName)
		{
			string result;
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
			{
				result = char.ToLower(fieldName[1]) + fieldName.Substring(2);
			}
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
			{
				result = char.ToLower(fieldName[2]) + fieldName.Substring(3);
			}
			else
			{
				result = char.ToLower(fieldName[0]) + fieldName.Substring(1);
			}
			return result;
		}

		public virtual PropertyDeclaration CreateProperty(IField field, bool createGetter, bool createSetter)
		{
			ClassFinder targetContext = new ClassFinder(field);
			string name = this.GetPropertyName(field.Name);
			PropertyDeclaration property = new PropertyDeclaration(CodeGenerator.ConvertModifier(field.Modifiers, targetContext), null, name, null);
			property.TypeReference = CodeGenerator.ConvertType(field.ReturnType, new ClassFinder(field));
			if (createGetter)
			{
				BlockStatement block = new BlockStatement();
				block.AddChild(new ReturnStatement(new IdentifierExpression(field.Name)));
				property.GetRegion = new PropertyGetRegion(block, null);
			}
			if (createSetter)
			{
				BlockStatement block = new BlockStatement();
				Expression left = new IdentifierExpression(field.Name);
				Expression right = new IdentifierExpression("value");
				block.AddChild(new ExpressionStatement(new AssignmentExpression(left, AssignmentOperatorType.Assign, right)));
				property.SetRegion = new PropertySetRegion(block, null);
			}
			property.Modifier = (Modifiers.Public | (property.Modifier & Modifiers.Static));
			return property;
		}

		public virtual void CreateChangedEvent(IProperty property, IDocument document)
		{
			ClassFinder targetContext = new ClassFinder(property);
			string name = property.Name + "Changed";
			EventDeclaration ed = new EventDeclaration(new TypeReference("EventHandler"), name, CodeGenerator.ConvertModifier(property.Modifiers & (ModifierEnum.Private | ModifierEnum.Internal | ModifierEnum.Protected | ModifierEnum.Public | ModifierEnum.Static), targetContext), null, null);
			this.InsertCodeAfter(property, document, new AbstractNode[]
			{
				ed
			});
			List<Expression> arguments = new List<Expression>(2);
			if (property.IsStatic)
			{
				arguments.Add(new PrimitiveExpression(null, "null"));
			}
			else
			{
				arguments.Add(new ThisReferenceExpression());
			}
			arguments.Add(new FieldReferenceExpression(new IdentifierExpression("EventArgs"), "Empty"));
			this.InsertCodeAtEnd(property.SetterRegion, document, new AbstractNode[]
			{
				new RaiseEventStatement(name, arguments)
			});
		}

		public virtual MethodDeclaration CreateOnEventMethod(IEvent e)
		{
			ClassFinder context = new ClassFinder(e);
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			bool sender = false;
			if (e.ReturnType != null)
			{
				IMethod invoke = e.ReturnType.GetMethods().Find((IMethod m) => m.Name == "Invoke");
				if (invoke != null)
				{
					foreach (IParameter param in invoke.Parameters)
					{
						parameters.Add(new ParameterDeclarationExpression(CodeGenerator.ConvertType(param.ReturnType, context), param.Name));
					}
					if (parameters.Count > 0 && string.Equals(parameters[0].ParameterName, "sender", StringComparison.InvariantCultureIgnoreCase))
					{
						sender = true;
						parameters.RemoveAt(0);
					}
				}
			}
			ModifierEnum modifier;
			if (e.IsStatic)
			{
				modifier = (ModifierEnum.Private | ModifierEnum.Static);
			}
			else if (e.DeclaringType.IsSealed)
			{
				modifier = ModifierEnum.Protected;
			}
			else
			{
				modifier = (ModifierEnum.Protected | ModifierEnum.Virtual);
			}
			MethodDeclaration method = new MethodDeclaration("On" + e.Name, CodeGenerator.ConvertModifier(modifier, context), new TypeReference("System.Void"), parameters, null);
			List<Expression> arguments = new List<Expression>();
			if (sender)
			{
				if (e.IsStatic)
				{
					arguments.Add(new PrimitiveExpression(null, "null"));
				}
				else
				{
					arguments.Add(new ThisReferenceExpression());
				}
			}
			foreach (ParameterDeclarationExpression param2 in parameters)
			{
				arguments.Add(new IdentifierExpression(param2.ParameterName));
			}
			method.Body = new BlockStatement();
			method.Body.AddChild(new RaiseEventStatement(e.Name, arguments));
			return method;
		}

		protected string GetInterfaceName(IReturnType interf, IMember member, ClassFinder context)
		{
			string result;
			if (CodeGenerator.CanUseShortTypeName(member.DeclaringType.DefaultReturnType, context))
			{
				result = member.DeclaringType.Name;
			}
			else
			{
				result = member.DeclaringType.FullyQualifiedName;
			}
			return result;
		}

		public virtual void ImplementInterface(IReturnType interf, IDocument document, bool explicitImpl, IClass targetClass)
		{
			List<AbstractNode> nodes = new List<AbstractNode>();
			this.ImplementInterface(nodes, interf, explicitImpl, targetClass);
			this.InsertCodeAtEnd(targetClass.Region, document, nodes.ToArray());
		}

		private static bool InterfaceMemberAlreadyImplementedParametersAreIdentical(IMember a, IMember b)
		{
			return !(a is IMethodOrProperty) || !(b is IMethodOrProperty) || DiffUtility.Compare<IParameter>(((IMethodOrProperty)a).Parameters, ((IMethodOrProperty)b).Parameters) == 0;
		}

		private static T CloneAndAddExplicitImpl<T>(T member, IClass targetClass) where T : class, IMember
		{
			T copy = (T)((object)member.Clone());
			copy.DeclaringTypeReference = targetClass.DefaultReturnType;
			copy.InterfaceImplementations.Add(new ExplicitInterfaceImplementation(member.DeclaringTypeReference, member.Name));
			return copy;
		}

		private static bool InterfaceMemberAlreadyImplemented<T>(IEnumerable<T> existingMembers, T interfaceMember, out bool requireAlternativeImplementation) where T : class, IMember
		{
			IReturnType interf = interfaceMember.DeclaringTypeReference;
			requireAlternativeImplementation = false;
			bool result;
			foreach (T existing in existingMembers)
			{
				StringComparer nameComparer = existing.DeclaringType.ProjectContent.Language.NameComparer;
				if (nameComparer.Equals(existing.Name, interfaceMember.Name))
				{
					if (CodeGenerator.InterfaceMemberAlreadyImplementedParametersAreIdentical(existing, interfaceMember))
					{
						if (object.Equals(existing.ReturnType, interfaceMember.ReturnType))
						{
							result = true;
							return result;
						}
						requireAlternativeImplementation = true;
					}
				}
				else
				{
					foreach (ExplicitInterfaceImplementation eii in existing.InterfaceImplementations)
					{
						if (object.Equals(eii.InterfaceReference, interf) && nameComparer.Equals(eii.MemberName, interfaceMember.Name))
						{
							if (CodeGenerator.InterfaceMemberAlreadyImplementedParametersAreIdentical(existing, interfaceMember))
							{
								if (object.Equals(existing.ReturnType, interfaceMember.ReturnType))
								{
									result = true;
									return result;
								}
								requireAlternativeImplementation = true;
							}
						}
					}
				}
			}
			result = false;
			return result;
		}

		private static InterfaceImplementation CreateInterfaceImplementation(IMember interfaceMember, ClassFinder context)
		{
			return new InterfaceImplementation(CodeGenerator.ConvertType(interfaceMember.DeclaringTypeReference, context), interfaceMember.Name);
		}

		public virtual void ImplementInterface(IList<AbstractNode> nodes, IReturnType interf, bool explicitImpl, IClass targetClass)
		{
			ClassFinder context = new ClassFinder(targetClass, targetClass.Region.BeginLine + 1, 0);
			Modifiers implicitImplModifier = CodeGenerator.ConvertModifier(ModifierEnum.Public, context);
			Modifiers explicitImplModifier = CodeGenerator.ConvertModifier(context.Language.ExplicitInterfaceImplementationIsPrivateScope ? ModifierEnum.None : ModifierEnum.Public, context);
			List<IEvent> targetClassEvents = targetClass.DefaultReturnType.GetEvents();
			foreach (IEvent e in interf.GetEvents())
			{
				bool requireAlternativeImplementation;
				if (!CodeGenerator.InterfaceMemberAlreadyImplemented<IEvent>(targetClassEvents, e, out requireAlternativeImplementation))
				{
					EventDeclaration ed = CodeGenerator.ConvertMember(e, context);
					if (explicitImpl || requireAlternativeImplementation)
					{
						ed.InterfaceImplementations.Add(CodeGenerator.CreateInterfaceImplementation(e, context));
						if (context.Language.RequiresAddRemoveRegionInExplicitInterfaceImplementation)
						{
							ed.AddRegion = new EventAddRegion(null);
							ed.AddRegion.Block = CodeGenerator.CreateNotImplementedBlock();
							ed.RemoveRegion = new EventRemoveRegion(null);
							ed.RemoveRegion.Block = CodeGenerator.CreateNotImplementedBlock();
						}
						targetClassEvents.Add(CodeGenerator.CloneAndAddExplicitImpl<IEvent>(e, targetClass));
						ed.Modifier = explicitImplModifier;
					}
					else
					{
						targetClassEvents.Add(e);
						ed.Modifier = implicitImplModifier;
					}
					nodes.Add(ed);
				}
			}
			List<IProperty> targetClassProperties = targetClass.DefaultReturnType.GetProperties();
			foreach (IProperty p in interf.GetProperties())
			{
				bool requireAlternativeImplementation;
				if (!CodeGenerator.InterfaceMemberAlreadyImplemented<IProperty>(targetClassProperties, p, out requireAlternativeImplementation))
				{
					AttributedNode pd = CodeGenerator.ConvertMember(p, context);
					if (explicitImpl || requireAlternativeImplementation)
					{
						InterfaceImplementation impl = CodeGenerator.CreateInterfaceImplementation(p, context);
						if (pd is IndexerDeclaration)
						{
							((IndexerDeclaration)pd).InterfaceImplementations.Add(impl);
						}
						else
						{
							((PropertyDeclaration)pd).InterfaceImplementations.Add(impl);
						}
						targetClassProperties.Add(CodeGenerator.CloneAndAddExplicitImpl<IProperty>(p, targetClass));
						pd.Modifier = explicitImplModifier;
					}
					else
					{
						targetClassProperties.Add(p);
						pd.Modifier = implicitImplModifier;
					}
					nodes.Add(pd);
				}
			}
			List<IMethod> targetClassMethods = targetClass.DefaultReturnType.GetMethods();
			foreach (IMethod i in interf.GetMethods())
			{
				bool requireAlternativeImplementation;
				if (!CodeGenerator.InterfaceMemberAlreadyImplemented<IMethod>(targetClassMethods, i, out requireAlternativeImplementation))
				{
					MethodDeclaration md = CodeGenerator.ConvertMember(i, context) as MethodDeclaration;
					if (md != null)
					{
						if (explicitImpl || requireAlternativeImplementation)
						{
							md.InterfaceImplementations.Add(CodeGenerator.CreateInterfaceImplementation(i, context));
							targetClassMethods.Add(CodeGenerator.CloneAndAddExplicitImpl<IMethod>(i, targetClass));
							md.Modifier = explicitImplModifier;
						}
						else
						{
							targetClassMethods.Add(i);
							md.Modifier = implicitImplModifier;
						}
						nodes.Add(md);
					}
				}
			}
		}

		public virtual AttributedNode GetOverridingMethod(IMember baseMember, ClassFinder targetContext)
		{
			AttributedNode node = CodeGenerator.ConvertMember(baseMember, targetContext);
			node.Modifier &= ~(Modifiers.Dim | Modifiers.Virtual);
			node.Modifier |= Modifiers.Override;
			MethodDeclaration method = node as MethodDeclaration;
			if (method != null)
			{
				method.Body.Children.Clear();
				if (method.TypeReference.SystemType == "System.Void")
				{
					method.Body.AddChild(new ExpressionStatement(CodeGenerator.CreateForwardingMethodCall(method)));
				}
				else
				{
					method.Body.AddChild(new ReturnStatement(CodeGenerator.CreateForwardingMethodCall(method)));
				}
			}
			PropertyDeclaration property = node as PropertyDeclaration;
			if (property != null)
			{
				Expression field = new FieldReferenceExpression(new BaseReferenceExpression(), property.Name);
				if (!property.GetRegion.Block.IsNull)
				{
					property.GetRegion.Block.Children.Clear();
					property.GetRegion.Block.AddChild(new ReturnStatement(field));
				}
				if (!property.SetRegion.Block.IsNull)
				{
					property.SetRegion.Block.Children.Clear();
					Expression expr = new AssignmentExpression(field, AssignmentOperatorType.Assign, new IdentifierExpression("value"));
					property.SetRegion.Block.AddChild(new ExpressionStatement(expr));
				}
			}
			return node;
		}

		private static InvocationExpression CreateForwardingMethodCall(MethodDeclaration method)
		{
			Expression methodName = new FieldReferenceExpression(new BaseReferenceExpression(), method.Name);
			InvocationExpression ie = new InvocationExpression(methodName, null);
			foreach (ParameterDeclarationExpression param in method.Parameters)
			{
				Expression expr = new IdentifierExpression(param.ParameterName);
				if (param.ParamModifier == AIMS.Libraries.Scripting.NRefactory.Ast.ParameterModifiers.Ref)
				{
					expr = new DirectionExpression(FieldDirection.Ref, expr);
				}
				else if (param.ParamModifier == AIMS.Libraries.Scripting.NRefactory.Ast.ParameterModifiers.Out)
				{
					expr = new DirectionExpression(FieldDirection.Out, expr);
				}
				ie.Arguments.Add(expr);
			}
			return ie;
		}

		public virtual void ReplaceUsings(IDocument document, IList<IUsing> oldUsings, IList<IUsing> newUsings)
		{
			if (oldUsings.Count == newUsings.Count)
			{
				bool identical = true;
				for (int i = 0; i < oldUsings.Count; i++)
				{
					if (oldUsings[i] != newUsings[i])
					{
						identical = false;
						break;
					}
				}
				if (identical)
				{
					return;
				}
			}
			int firstLine = 2147483647;
			List<KeyValuePair<int, int>> regions = new List<KeyValuePair<int, int>>();
			foreach (IUsing u in oldUsings)
			{
				if (u.Region.BeginLine < firstLine)
				{
					firstLine = u.Region.BeginLine;
				}
				int st = document.PositionToOffset(u.Region.BeginLine, u.Region.BeginColumn);
				int en = document.PositionToOffset(u.Region.EndLine, u.Region.EndColumn);
				regions.Add(new KeyValuePair<int, int>(st, en - st));
			}
			regions.Sort((KeyValuePair<int, int> a, KeyValuePair<int, int> b) => a.Key.CompareTo(b.Key));
			int insertionOffset = (regions.Count == 0) ? 0 : regions[0].Key;
			string indentation;
			if (firstLine != 2147483647)
			{
				indentation = this.GetIndentation(document, firstLine);
				insertionOffset -= indentation.Length;
			}
			else
			{
				indentation = "";
			}
			document.StartUndoableAction();
			for (int i = regions.Count - 1; i >= 0; i--)
			{
				document.Remove(regions[i].Key, regions[i].Value);
			}
			int lastNewLine = insertionOffset;
			for (int i = insertionOffset; i < document.TextLength; i++)
			{
				char c = document.GetCharAt(i);
				if (!char.IsWhiteSpace(c))
				{
					break;
				}
				if (c == '\n')
				{
					if (i > 0 && document.GetCharAt(i - 1) == '\r')
					{
						lastNewLine = i - 1;
					}
					else
					{
						lastNewLine = i;
					}
				}
			}
			if (lastNewLine != insertionOffset)
			{
				document.Remove(insertionOffset, lastNewLine - insertionOffset);
			}
			StringBuilder txt = new StringBuilder();
			foreach (IUsing us in newUsings)
			{
				if (us == null)
				{
					txt.AppendLine(indentation);
				}
				else
				{
					txt.Append(this.GenerateCode(CodeGenerator.ConvertUsing(us), indentation));
				}
			}
			document.Insert(insertionOffset, txt.ToString());
			document.EndUndoableAction();
		}
	}
}

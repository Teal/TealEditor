using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Visitors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AIMS.Libraries.Scripting.Dom.NRefactoryResolver
{
	public class TypeVisitor : AbstractAstVisitor
	{
		public class NamespaceReturnType : AbstractReturnType
		{
			public NamespaceReturnType(string fullName)
			{
				this.FullyQualifiedName = fullName;
			}

			public override IClass GetUnderlyingClass()
			{
				return null;
			}

			public override List<IMethod> GetMethods()
			{
				return new List<IMethod>();
			}

			public override List<IProperty> GetProperties()
			{
				return new List<IProperty>();
			}

			public override List<IField> GetFields()
			{
				return new List<IField>();
			}

			public override List<IEvent> GetEvents()
			{
				return new List<IEvent>();
			}
		}

		private NRefactoryResolver resolver;

		[CompilerGenerated]
		private static Predicate<IMethod> <>9__CachedAnonymousMethodDelegate1;

		public TypeVisitor(NRefactoryResolver resolver)
		{
			this.resolver = resolver;
		}

		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			object result;
			if (primitiveExpression.Value == null)
			{
				result = NullReturnType.Instance;
			}
			else
			{
				result = this.resolver.ProjectContent.SystemTypes.CreatePrimitive(primitiveExpression.Value.GetType());
			}
			return result;
		}

		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			object result;
			switch (binaryOperatorExpression.Op)
			{
			case BinaryOperatorType.LogicalAnd:
			case BinaryOperatorType.LogicalOr:
			case BinaryOperatorType.GreaterThan:
			case BinaryOperatorType.GreaterThanOrEqual:
			case BinaryOperatorType.Equality:
			case BinaryOperatorType.InEquality:
			case BinaryOperatorType.LessThan:
			case BinaryOperatorType.LessThanOrEqual:
			case BinaryOperatorType.ReferenceEquality:
			case BinaryOperatorType.ReferenceInequality:
				result = this.resolver.ProjectContent.SystemTypes.Boolean;
				return result;
			case BinaryOperatorType.DivideInteger:
				result = this.resolver.ProjectContent.SystemTypes.Int32;
				return result;
			case BinaryOperatorType.Concat:
				result = this.resolver.ProjectContent.SystemTypes.String;
				return result;
			case BinaryOperatorType.NullCoalescing:
				result = binaryOperatorExpression.Right.AcceptVisitor(this, data);
				return result;
			}
			result = MemberLookupHelper.GetCommonType(this.resolver.ProjectContent, binaryOperatorExpression.Left.AcceptVisitor(this, data) as IReturnType, binaryOperatorExpression.Right.AcceptVisitor(this, data) as IReturnType);
			return result;
		}

		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}

		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			IMethodOrProperty i = this.GetMethod(invocationExpression);
			if (i == null)
			{
				IReturnType targetType = invocationExpression.TargetObject.AcceptVisitor(this, data) as IReturnType;
				if (targetType != null)
				{
					IClass c = targetType.GetUnderlyingClass();
					if (c != null && c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Delegate)
					{
						i = c.Methods.Find((IMethod innerMethod) => innerMethod.Name == "Invoke");
					}
				}
			}
			object result;
			if (i != null)
			{
				result = i.ReturnType;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			IProperty i = this.GetIndexer(indexerExpression);
			object result;
			if (i != null)
			{
				result = i.ReturnType;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public IMethod FindOverload(List<IMethod> methods, IReturnType[] typeParameters, IList<Expression> arguments, object data)
		{
			IMethod result;
			if (methods.Count <= 0)
			{
				result = null;
			}
			else
			{
				IReturnType[] types = new IReturnType[arguments.Count];
				for (int i = 0; i < types.Length; i++)
				{
					types[i] = (arguments[i].AcceptVisitor(this, data) as IReturnType);
				}
				result = MemberLookupHelper.FindOverload(methods, typeParameters, types);
			}
			return result;
		}

		public IMethodOrProperty GetMethod(InvocationExpression invocationExpression)
		{
			IReturnType[] typeParameters = this.CreateReturnTypes(invocationExpression.TypeArguments);
			IMethodOrProperty result;
			if (invocationExpression.TargetObject is FieldReferenceExpression)
			{
				FieldReferenceExpression field = (FieldReferenceExpression)invocationExpression.TargetObject;
				IReturnType type = field.TargetObject.AcceptVisitor(this, null) as IReturnType;
				List<IMethod> methods = this.resolver.SearchMethod(type, field.FieldName);
				if (methods.Count == 0 && this.resolver.Language == SupportedLanguage.VBNet)
				{
					result = this.GetVisualBasicIndexer(invocationExpression);
				}
				else
				{
					result = this.FindOverload(methods, typeParameters, invocationExpression.Arguments, null);
				}
			}
			else if (invocationExpression.TargetObject is IdentifierExpression)
			{
				string id = ((IdentifierExpression)invocationExpression.TargetObject).Identifier;
				if (this.resolver.CallingClass == null)
				{
					result = null;
				}
				else
				{
					List<IMethod> methods = this.resolver.SearchMethod(id);
					if (methods.Count == 0 && this.resolver.Language == SupportedLanguage.VBNet)
					{
						result = this.GetVisualBasicIndexer(invocationExpression);
					}
					else
					{
						result = this.FindOverload(methods, typeParameters, invocationExpression.Arguments, null);
					}
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		private IProperty GetVisualBasicIndexer(InvocationExpression invocationExpression)
		{
			return this.GetIndexer(new IndexerExpression(invocationExpression.TargetObject, invocationExpression.Arguments));
		}

		public IProperty GetIndexer(IndexerExpression indexerExpression)
		{
			IReturnType type = (IReturnType)indexerExpression.TargetObject.AcceptVisitor(this, null);
			IProperty result;
			if (type == null)
			{
				result = null;
			}
			else
			{
				List<IProperty> indexers = type.GetProperties();
				for (int i = 0; i < indexers.Count; i++)
				{
					if (!indexers[i].IsIndexer)
					{
						indexers.RemoveAt(i--);
					}
				}
				IReturnType[] parameters = new IReturnType[indexerExpression.Indexes.Count];
				for (int i = 0; i < parameters.Length; i++)
				{
					Expression expr = indexerExpression.Indexes[i];
					if (expr != null)
					{
						parameters[i] = (IReturnType)expr.AcceptVisitor(this, null);
					}
				}
				result = MemberLookupHelper.FindOverload(indexers.ToArray(), parameters);
			}
			return result;
		}

		public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			object result;
			if (fieldReferenceExpression == null)
			{
				result = null;
			}
			else
			{
				if (fieldReferenceExpression.FieldName == null || fieldReferenceExpression.FieldName == "")
				{
					if (fieldReferenceExpression.TargetObject is TypeReferenceExpression)
					{
						result = this.CreateReturnType(((TypeReferenceExpression)fieldReferenceExpression.TargetObject).TypeReference);
						return result;
					}
				}
				IReturnType returnType = fieldReferenceExpression.TargetObject.AcceptVisitor(this, data) as IReturnType;
				if (returnType != null)
				{
					if (returnType is TypeVisitor.NamespaceReturnType)
					{
						string name = returnType.FullyQualifiedName;
						string combinedName;
						if (name.Length == 0)
						{
							combinedName = fieldReferenceExpression.FieldName;
						}
						else
						{
							combinedName = name + "." + fieldReferenceExpression.FieldName;
						}
						if (this.resolver.ProjectContent.NamespaceExists(combinedName))
						{
							result = new TypeVisitor.NamespaceReturnType(combinedName);
						}
						else
						{
							IClass c = this.resolver.GetClass(combinedName);
							if (c != null)
							{
								result = c.DefaultReturnType;
							}
							else
							{
								if (this.resolver.LanguageProperties.ImportModules)
								{
									foreach (object o in this.resolver.ProjectContent.GetNamespaceContents(name))
									{
										IMember member = o as IMember;
										if (member != null && this.resolver.IsSameName(member.Name, fieldReferenceExpression.FieldName))
										{
											result = member.ReturnType;
											return result;
										}
									}
								}
								result = null;
							}
						}
					}
					else
					{
						result = this.resolver.SearchMember(returnType, fieldReferenceExpression.FieldName);
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public override object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			return null;
		}

		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			object result;
			if (identifierExpression == null)
			{
				result = null;
			}
			else
			{
				IClass c = this.resolver.SearchClass(identifierExpression.Identifier);
				if (c != null)
				{
					result = c.DefaultReturnType;
				}
				else
				{
					result = this.resolver.DynamicLookup(identifierExpression.Identifier);
				}
			}
			return result;
		}

		public override object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return this.CreateReturnType(typeReferenceExpression.TypeReference);
		}

		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			object result;
			if (unaryOperatorExpression == null)
			{
				result = null;
			}
			else
			{
				IReturnType expressionType = unaryOperatorExpression.Expression.AcceptVisitor(this, data) as IReturnType;
				switch (unaryOperatorExpression.Op)
				{
				}
				result = expressionType;
			}
			return result;
		}

		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			return assignmentExpression.Left.AcceptVisitor(this, data);
		}

		public override object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			return this.resolver.ProjectContent.SystemTypes.Int32;
		}

		public override object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			return this.resolver.ProjectContent.SystemTypes.Type;
		}

		public override object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			return checkedExpression.Expression.AcceptVisitor(this, data);
		}

		public override object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			return uncheckedExpression.Expression.AcceptVisitor(this, data);
		}

		public override object VisitCastExpression(CastExpression castExpression, object data)
		{
			return this.CreateReturnType(castExpression.CastTo);
		}

		public override object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			return null;
		}

		public override object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			object result;
			if (this.resolver.CallingClass == null)
			{
				result = null;
			}
			else
			{
				result = this.resolver.CallingClass.DefaultReturnType;
			}
			return result;
		}

		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			object result;
			if (this.resolver.CallingClass == null)
			{
				result = null;
			}
			else
			{
				result = this.resolver.CallingClass.DefaultReturnType;
			}
			return result;
		}

		public override object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			object result;
			if (this.resolver.CallingClass == null)
			{
				result = null;
			}
			else
			{
				result = this.resolver.CallingClass.BaseType;
			}
			return result;
		}

		public override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			return this.CreateReturnType(objectCreateExpression.CreateType);
		}

		public override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			return this.CreateReturnType(arrayCreateExpression.CreateType);
		}

		public override object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			return this.resolver.ProjectContent.SystemTypes.Boolean;
		}

		public override object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			return this.CreateReturnType(defaultValueExpression.TypeReference);
		}

		public override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			AnonymousMethodReturnType amrt = new AnonymousMethodReturnType(this.resolver.CompilationUnit);
			foreach (ParameterDeclarationExpression param in anonymousMethodExpression.Parameters)
			{
				amrt.MethodParameters.Add(NRefactoryASTConvertVisitor.CreateParameter(param, this.resolver.CallingMember as IMethod, this.resolver.CallingClass, this.resolver.CompilationUnit));
			}
			return amrt;
		}

		public override object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			return null;
		}

		private IReturnType CreateReturnType(TypeReference reference)
		{
			return TypeVisitor.CreateReturnType(reference, this.resolver);
		}

		private IReturnType[] CreateReturnTypes(List<TypeReference> references)
		{
			IReturnType[] result;
			if (references == null)
			{
				result = new IReturnType[0];
			}
			else
			{
				IReturnType[] types = new IReturnType[references.Count];
				for (int i = 0; i < types.Length; i++)
				{
					types[i] = this.CreateReturnType(references[i]);
				}
				result = types;
			}
			return result;
		}

		public static IReturnType CreateReturnType(TypeReference reference, NRefactoryResolver resolver)
		{
			return TypeVisitor.CreateReturnType(reference, resolver.CallingClass, resolver.CallingMember, resolver.CaretLine, resolver.CaretColumn, resolver.ProjectContent, false);
		}

		public static IReturnType CreateReturnType(TypeReference reference, IClass callingClass, IMember callingMember, int caretLine, int caretColumn, IProjectContent projectContent, bool useLazyReturnType)
		{
			IReturnType result;
			if (reference == null)
			{
				result = null;
			}
			else if (reference.IsNull)
			{
				result = null;
			}
			else
			{
				if (reference is InnerClassTypeReference)
				{
					reference = ((InnerClassTypeReference)reference).CombineToNormalTypeReference();
				}
				LanguageProperties languageProperties = projectContent.Language;
				IReturnType t = null;
				if (callingClass != null && !reference.IsGlobal)
				{
					foreach (ITypeParameter tp in callingClass.TypeParameters)
					{
						if (languageProperties.NameComparer.Equals(tp.Name, reference.SystemType))
						{
							t = new GenericReturnType(tp);
							break;
						}
					}
					if (t == null && callingMember is IMethod && (callingMember as IMethod).TypeParameters != null)
					{
						foreach (ITypeParameter tp in (callingMember as IMethod).TypeParameters)
						{
							if (languageProperties.NameComparer.Equals(tp.Name, reference.SystemType))
							{
								t = new GenericReturnType(tp);
								break;
							}
						}
					}
				}
				if (t == null)
				{
					if (reference.Type != reference.SystemType)
					{
						IClass c = projectContent.GetClass(reference.SystemType);
						if (c != null)
						{
							t = c.DefaultReturnType;
						}
						else
						{
							t = new GetClassReturnType(projectContent, reference.SystemType, 0);
						}
					}
					else
					{
						int typeParameterCount = reference.GenericTypes.Count;
						if (useLazyReturnType)
						{
							if (reference.IsGlobal)
							{
								t = new GetClassReturnType(projectContent, reference.SystemType, typeParameterCount);
							}
							else if (callingClass != null)
							{
								t = new SearchClassReturnType(projectContent, callingClass, caretLine, caretColumn, reference.SystemType, typeParameterCount);
							}
						}
						else
						{
							if (reference.IsGlobal)
							{
								IClass c = projectContent.GetClass(reference.SystemType, typeParameterCount);
								t = ((c != null) ? c.DefaultReturnType : null);
							}
							else if (callingClass != null)
							{
								t = projectContent.SearchType(new SearchTypeRequest(reference.SystemType, typeParameterCount, callingClass, caretLine, caretColumn)).Result;
							}
							if (t == null)
							{
								if (reference.GenericTypes.Count == 0 && !reference.IsArrayType)
								{
									if (reference.IsGlobal)
									{
										if (projectContent.NamespaceExists(reference.Type))
										{
											result = new TypeVisitor.NamespaceReturnType(reference.Type);
											return result;
										}
									}
									else
									{
										string name = projectContent.SearchNamespace(reference.Type, callingClass, (callingClass == null) ? null : callingClass.CompilationUnit, caretLine, caretColumn);
										if (name != null)
										{
											result = new TypeVisitor.NamespaceReturnType(name);
											return result;
										}
									}
								}
								result = null;
								return result;
							}
						}
					}
				}
				if (reference.GenericTypes.Count > 0)
				{
					List<IReturnType> para = new List<IReturnType>(reference.GenericTypes.Count);
					for (int i = 0; i < reference.GenericTypes.Count; i++)
					{
						para.Add(TypeVisitor.CreateReturnType(reference.GenericTypes[i], callingClass, callingMember, caretLine, caretColumn, projectContent, useLazyReturnType));
					}
					t = new ConstructedReturnType(t, para);
				}
				result = TypeVisitor.WrapArray(projectContent, t, reference);
			}
			return result;
		}

		private static IReturnType WrapArray(IProjectContent pc, IReturnType t, TypeReference reference)
		{
			if (reference.IsArrayType)
			{
				for (int i = reference.RankSpecifier.Length - 1; i >= 0; i--)
				{
					int dimensions = reference.RankSpecifier[i] + 1;
					if (dimensions > 0)
					{
						t = new ArrayReturnType(pc, t, dimensions);
					}
				}
			}
			return t;
		}
	}
}

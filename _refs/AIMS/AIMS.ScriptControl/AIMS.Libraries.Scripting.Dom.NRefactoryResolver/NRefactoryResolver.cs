using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Visitors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom.NRefactoryResolver
{
	public class NRefactoryResolver : IResolver
	{
		private class DummyFindVisitor : AbstractAstVisitor
		{
			internal const string dummyName = "___withStatementExpressionDummy";

			internal FieldReferenceExpression result;

			public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
			{
				IdentifierExpression ie = fieldReferenceExpression.TargetObject as IdentifierExpression;
				if (ie != null && ie.Identifier == "___withStatementExpressionDummy")
				{
					this.result = fieldReferenceExpression;
				}
				return base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
			}
		}

		private ICompilationUnit cu;

		private IClass callingClass;

		private IMember callingMember;

		private LookupTableVisitor lookupTableVisitor;

		private IProjectContent projectContent;

		private SupportedLanguage language;

		private int caretLine;

		private int caretColumn;

		private LanguageProperties languageProperties;

		[CompilerGenerated]
		private static Predicate<IMethod> <>9__CachedAnonymousMethodDelegate1;

		public SupportedLanguage Language
		{
			get
			{
				return this.language;
			}
		}

		public IProjectContent ProjectContent
		{
			get
			{
				return this.projectContent;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.projectContent = value;
			}
		}

		public ICompilationUnit CompilationUnit
		{
			get
			{
				return this.cu;
			}
		}

		public IClass CallingClass
		{
			get
			{
				return this.callingClass;
			}
		}

		public IMember CallingMember
		{
			get
			{
				return this.callingMember;
			}
		}

		public int CaretLine
		{
			get
			{
				return this.caretLine;
			}
		}

		public int CaretColumn
		{
			get
			{
				return this.caretColumn;
			}
		}

		public LanguageProperties LanguageProperties
		{
			get
			{
				return this.languageProperties;
			}
		}

		public NRefactoryResolver(IProjectContent projectContent, LanguageProperties languageProperties)
		{
			if (projectContent == null)
			{
				throw new ArgumentNullException("projectContent");
			}
			if (languageProperties == null)
			{
				throw new ArgumentNullException("languageProperties");
			}
			this.languageProperties = languageProperties;
			this.projectContent = projectContent;
			if (languageProperties is LanguageProperties.CSharpProperties)
			{
				this.language = SupportedLanguage.CSharp;
			}
			else
			{
				if (!(languageProperties is LanguageProperties.VBNetProperties))
				{
					throw new NotSupportedException("The language " + languageProperties.ToString() + " is not supported in the resolver");
				}
				this.language = SupportedLanguage.VBNet;
			}
		}

		[Obsolete("Use the IProjectContent, LanguageProperties overload instead to support .cs files inside vb projects or similar.")]
		public NRefactoryResolver(IProjectContent projectContent) : this(projectContent, projectContent.Language)
		{
		}

		private Expression ParseExpression(string expression)
		{
			Expression expr = this.SpecialConstructs(expression);
			if (expr == null)
			{
				using (IParser p = ParserFactory.CreateParser(this.language, new StringReader(expression)))
				{
					expr = p.ParseExpression();
				}
			}
			return expr;
		}

		private string GetFixedExpression(ExpressionResult expressionResult)
		{
			string expression = expressionResult.Expression;
			if (expression == null)
			{
				expression = "";
			}
			return expression.TrimStart(new char[0]);
		}

		public bool Initialize(string fileName, int caretLineNumber, int caretColumn)
		{
			this.caretLine = caretLineNumber;
			this.caretColumn = caretColumn;
			ParseInformation parseInfo = HostCallback.GetParseInformation(fileName);
			bool result;
			if (parseInfo == null)
			{
				result = false;
			}
			else
			{
				this.cu = parseInfo.MostRecentCompilationUnit;
				if (this.cu != null)
				{
					this.callingClass = this.cu.GetInnermostClass(this.caretLine, caretColumn);
					if (this.cu.ProjectContent != null)
					{
						this.ProjectContent = this.cu.ProjectContent;
					}
				}
				this.callingMember = this.GetCurrentMember();
				result = true;
			}
			return result;
		}

		public ResolveResult Resolve(ExpressionResult expressionResult, int caretLineNumber, int caretColumn, string fileName, string fileContent)
		{
			string expression = this.GetFixedExpression(expressionResult);
			ResolveResult result;
			if (!this.Initialize(fileName, caretLineNumber, caretColumn))
			{
				result = null;
			}
			else
			{
				Expression expr = null;
				if (this.language == SupportedLanguage.VBNet)
				{
					if (expression.Length == 0 || expression[0] == '.')
					{
						result = this.WithResolve(expression, fileContent);
						return result;
					}
					if ("global".Equals(expression, StringComparison.InvariantCultureIgnoreCase))
					{
						result = new NamespaceResolveResult(null, null, "");
						return result;
					}
				}
				if (expr == null)
				{
					expr = this.ParseExpression(expression);
					if (expr == null)
					{
						result = null;
						return result;
					}
					if (expressionResult.Context.IsObjectCreation)
					{
						for (Expression tmp = expr; tmp != null; tmp = (tmp as FieldReferenceExpression).TargetObject)
						{
							if (tmp is IdentifierExpression)
							{
								result = this.ResolveInternal(expr, ExpressionContext.Type);
								return result;
							}
							if (!(tmp is FieldReferenceExpression))
							{
								break;
							}
						}
						expr = this.ParseExpression("new " + expression);
						if (expr == null)
						{
							result = null;
							return result;
						}
					}
				}
				if (expressionResult.Context.IsAttributeContext)
				{
					result = this.ResolveAttribute(expr);
				}
				else
				{
					this.RunLookupTableVisitor(fileContent);
					ResolveResult rr = CtrlSpaceResolveHelper.GetResultFromDeclarationLine(this.callingClass, this.callingMember as IMethodOrProperty, this.caretLine, caretColumn, expressionResult);
					if (rr != null)
					{
						result = rr;
					}
					else
					{
						result = this.ResolveInternal(expr, expressionResult.Context);
					}
				}
			}
			return result;
		}

		private ResolveResult WithResolve(string expression, string fileContent)
		{
			this.RunLookupTableVisitor(fileContent);
			WithStatement innermost = null;
			if (this.lookupTableVisitor.WithStatements != null)
			{
				foreach (WithStatement with in this.lookupTableVisitor.WithStatements)
				{
					if (this.IsInside(new Location(this.caretColumn, this.caretLine), with.StartLocation, with.EndLocation))
					{
						innermost = with;
					}
				}
			}
			ResolveResult result;
			if (innermost != null)
			{
				if (expression.Length > 1)
				{
					Expression expr = this.ParseExpression("___withStatementExpressionDummy" + expression);
					if (expr == null)
					{
						result = null;
					}
					else
					{
						NRefactoryResolver.DummyFindVisitor v = new NRefactoryResolver.DummyFindVisitor();
						expr.AcceptVisitor(v, null);
						if (v.result == null)
						{
							result = null;
						}
						else
						{
							v.result.TargetObject = innermost.Expression;
							result = this.ResolveInternal(expr, ExpressionContext.Default);
						}
					}
				}
				else
				{
					result = this.ResolveInternal(innermost.Expression, ExpressionContext.Default);
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		public INode ParseCurrentMember(string fileContent)
		{
			CompilationUnit cu = this.ParseCurrentMemberAsCompilationUnit(fileContent);
			INode result;
			if (cu != null && cu.Children.Count > 0)
			{
				TypeDeclaration td = cu.Children[0] as TypeDeclaration;
				if (td != null && td.Children.Count > 0)
				{
					result = td.Children[0];
					return result;
				}
			}
			result = null;
			return result;
		}

		public CompilationUnit ParseCurrentMemberAsCompilationUnit(string fileContent)
		{
			TextReader content = this.ExtractCurrentMethod(fileContent);
			CompilationUnit result;
			if (content != null)
			{
				IParser p = ParserFactory.CreateParser(this.language, content);
				p.Parse();
				result = p.CompilationUnit;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void RunLookupTableVisitor(string fileContent)
		{
			this.lookupTableVisitor = new LookupTableVisitor(this.language);
			if (this.callingMember != null)
			{
				CompilationUnit cu = this.ParseCurrentMemberAsCompilationUnit(fileContent);
				if (cu != null)
				{
					this.lookupTableVisitor.VisitCompilationUnit(cu, null);
				}
			}
		}

		public void RunLookupTableVisitor(INode currentMemberNode)
		{
			this.lookupTableVisitor = new LookupTableVisitor(this.language);
			currentMemberNode.AcceptVisitor(this.lookupTableVisitor, null);
		}

		private string GetAttributeName(Expression expr)
		{
			string result;
			if (expr is IdentifierExpression)
			{
				result = (expr as IdentifierExpression).Identifier;
			}
			else
			{
				if (expr is FieldReferenceExpression)
				{
					TypeVisitor typeVisitor = new TypeVisitor(this);
					FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)expr;
					IReturnType type = fieldReferenceExpression.TargetObject.AcceptVisitor(typeVisitor, null) as IReturnType;
					if (type is TypeVisitor.NamespaceReturnType)
					{
						result = type.FullyQualifiedName + "." + fieldReferenceExpression.FieldName;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		private IClass GetAttribute(string name)
		{
			IClass result;
			if (name == null)
			{
				result = null;
			}
			else
			{
				IClass c = this.SearchClass(name);
				if (c != null)
				{
					if (c.IsTypeInInheritanceTree(c.ProjectContent.SystemTypes.Attribute.GetUnderlyingClass()))
					{
						result = c;
						return result;
					}
				}
				result = this.SearchClass(name + "Attribute");
			}
			return result;
		}

		private ResolveResult ResolveAttribute(Expression expr)
		{
			string attributeName = this.GetAttributeName(expr);
			IClass c = this.GetAttribute(attributeName);
			ResolveResult result;
			if (c != null)
			{
				result = new TypeResolveResult(this.callingClass, this.callingMember, c);
			}
			else
			{
				if (expr is InvocationExpression)
				{
					InvocationExpression ie = (InvocationExpression)expr;
					attributeName = this.GetAttributeName(ie.TargetObject);
					c = this.GetAttribute(attributeName);
					if (c != null)
					{
						List<IMethod> ctors = new List<IMethod>();
						foreach (IMethod i in c.Methods)
						{
							if (i.IsConstructor && !i.IsStatic)
							{
								ctors.Add(i);
							}
						}
						TypeVisitor typeVisitor = new TypeVisitor(this);
						result = this.CreateMemberResolveResult(typeVisitor.FindOverload(ctors, null, ie.Arguments, null));
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public ResolveResult ResolveInternal(Expression expr, ExpressionContext context)
		{
			TypeVisitor typeVisitor = new TypeVisitor(this);
			ResolveResult result2;
			IReturnType type;
			if (expr is PrimitiveExpression)
			{
				if (((PrimitiveExpression)expr).Value is int)
				{
					result2 = new IntegerLiteralResolveResult(this.callingClass, this.callingMember, this.projectContent.SystemTypes.Int32);
					return result2;
				}
			}
			else if (expr is InvocationExpression)
			{
				IMethodOrProperty method = typeVisitor.GetMethod(expr as InvocationExpression);
				if (method != null)
				{
					result2 = this.CreateMemberResolveResult(method);
					return result2;
				}
				ResolveResult invocationTarget = this.ResolveInternal((expr as InvocationExpression).TargetObject, ExpressionContext.Default);
				if (invocationTarget == null)
				{
					result2 = null;
					return result2;
				}
				type = invocationTarget.ResolvedType;
				if (type == null)
				{
					result2 = null;
					return result2;
				}
				IClass c = type.GetUnderlyingClass();
				if (c == null || c.ClassType != AIMS.Libraries.Scripting.Dom.ClassType.Delegate)
				{
					result2 = null;
					return result2;
				}
				method = c.Methods.Find((IMethod innerMethod) => innerMethod.Name == "Invoke");
				if (method != null)
				{
					invocationTarget.ResolvedType = method.ReturnType;
				}
				result2 = invocationTarget;
				return result2;
			}
			else
			{
				if (expr is IndexerExpression)
				{
					result2 = this.CreateMemberResolveResult(typeVisitor.GetIndexer(expr as IndexerExpression));
					return result2;
				}
				if (expr is FieldReferenceExpression)
				{
					FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)expr;
					if (fieldReferenceExpression.FieldName == null || fieldReferenceExpression.FieldName.Length == 0)
					{
						if (fieldReferenceExpression.TargetObject is TypeReferenceExpression)
						{
							type = TypeVisitor.CreateReturnType(((TypeReferenceExpression)fieldReferenceExpression.TargetObject).TypeReference, this);
							if (type != null)
							{
								result2 = new TypeResolveResult(this.callingClass, this.callingMember, type);
								return result2;
							}
						}
					}
					type = (fieldReferenceExpression.TargetObject.AcceptVisitor(typeVisitor, null) as IReturnType);
					if (type != null)
					{
						ResolveResult result = this.ResolveMemberReferenceExpression(type, fieldReferenceExpression);
						if (result != null)
						{
							result2 = result;
							return result2;
						}
					}
				}
				else if (expr is IdentifierExpression)
				{
					ResolveResult result = this.ResolveIdentifier(((IdentifierExpression)expr).Identifier, context);
					if (result != null)
					{
						result2 = result;
						return result2;
					}
				}
				else if (expr is TypeReferenceExpression)
				{
					type = TypeVisitor.CreateReturnType(((TypeReferenceExpression)expr).TypeReference, this);
					if (type != null)
					{
						if (type is TypeVisitor.NamespaceReturnType)
						{
							result2 = new NamespaceResolveResult(this.callingClass, this.callingMember, type.FullyQualifiedName);
							return result2;
						}
						IClass c = type.GetUnderlyingClass();
						if (c != null)
						{
							result2 = new TypeResolveResult(this.callingClass, this.callingMember, type, c);
							return result2;
						}
					}
					result2 = null;
					return result2;
				}
			}
			type = (expr.AcceptVisitor(typeVisitor, null) as IReturnType);
			if (type == null || type.FullyQualifiedName == "")
			{
				result2 = null;
			}
			else if (expr is ObjectCreateExpression)
			{
				List<IMethod> constructors = new List<IMethod>();
				foreach (IMethod i in type.GetMethods())
				{
					if (i.IsConstructor && !i.IsStatic)
					{
						constructors.Add(i);
					}
				}
				if (constructors.Count == 0)
				{
					IClass c = type.GetUnderlyingClass();
					if (c != null)
					{
						result2 = this.CreateMemberResolveResult(Constructor.CreateDefault(c));
						return result2;
					}
				}
				IReturnType[] typeParameters = null;
				if (type.IsConstructedReturnType)
				{
					typeParameters = new IReturnType[type.CastToConstructedReturnType().TypeArguments.Count];
					type.CastToConstructedReturnType().TypeArguments.CopyTo(typeParameters, 0);
				}
				ResolveResult rr = this.CreateMemberResolveResult(typeVisitor.FindOverload(constructors, typeParameters, ((ObjectCreateExpression)expr).Parameters, null));
				if (rr != null)
				{
					rr.ResolvedType = type;
				}
				result2 = rr;
			}
			else
			{
				result2 = new ResolveResult(this.callingClass, this.callingMember, type);
			}
			return result2;
		}

		private ResolveResult ResolveMemberReferenceExpression(IReturnType type, FieldReferenceExpression fieldReferenceExpression)
		{
			ResolveResult result;
			if (type is TypeVisitor.NamespaceReturnType)
			{
				string combinedName;
				if (type.FullyQualifiedName == "")
				{
					combinedName = fieldReferenceExpression.FieldName;
				}
				else
				{
					combinedName = type.FullyQualifiedName + "." + fieldReferenceExpression.FieldName;
				}
				if (this.projectContent.NamespaceExists(combinedName))
				{
					result = new NamespaceResolveResult(this.callingClass, this.callingMember, combinedName);
				}
				else
				{
					IClass c = this.GetClass(combinedName);
					if (c != null)
					{
						result = new TypeResolveResult(this.callingClass, this.callingMember, c);
					}
					else
					{
						if (this.languageProperties.ImportModules)
						{
							foreach (object o in this.projectContent.GetNamespaceContents(type.FullyQualifiedName))
							{
								IMember member = o as IMember;
								if (member != null && this.IsSameName(member.Name, fieldReferenceExpression.FieldName))
								{
									result = this.CreateMemberResolveResult(member);
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
				IMember member = this.GetMember(type, fieldReferenceExpression.FieldName);
				if (member != null)
				{
					result = this.CreateMemberResolveResult(member);
				}
				else
				{
					IClass c = type.GetUnderlyingClass();
					if (c != null)
					{
						foreach (IClass baseClass in c.ClassInheritanceTree)
						{
							List<IClass> innerClasses = baseClass.InnerClasses;
							if (innerClasses != null)
							{
								foreach (IClass innerClass in innerClasses)
								{
									if (this.IsSameName(innerClass.Name, fieldReferenceExpression.FieldName))
									{
										result = new TypeResolveResult(this.callingClass, this.callingMember, innerClass);
										return result;
									}
								}
							}
						}
					}
					result = this.ResolveMethod(type, fieldReferenceExpression.FieldName);
				}
			}
			return result;
		}

		public TextReader ExtractCurrentMethod(string fileContent)
		{
			TextReader result;
			if (this.callingMember == null)
			{
				result = null;
			}
			else
			{
				result = NRefactoryResolver.ExtractMethod(fileContent, this.callingMember, this.language, this.caretLine);
			}
			return result;
		}

		public static TextReader ExtractMethod(string fileContent, IMember member, SupportedLanguage language, int caretLine)
		{
			TextReader result;
			if (member.Region.IsEmpty)
			{
				result = null;
			}
			else
			{
				int startLine = member.Region.BeginLine;
				if (startLine < 1)
				{
					result = null;
				}
				else
				{
					DomRegion bodyRegion;
					if (member is IMethodOrProperty)
					{
						bodyRegion = ((IMethodOrProperty)member).BodyRegion;
					}
					else
					{
						if (!(member is IEvent))
						{
							result = null;
							return result;
						}
						bodyRegion = ((IEvent)member).BodyRegion;
					}
					if (bodyRegion.IsEmpty)
					{
						result = null;
					}
					else
					{
						int endLine = bodyRegion.EndLine;
						if (language == SupportedLanguage.CSharp)
						{
							if (caretLine > startLine && caretLine < endLine)
							{
								endLine = caretLine;
							}
						}
						int offset = 0;
						for (int i = 0; i < startLine - 1; i++)
						{
							offset = fileContent.IndexOf('\n', offset) + 1;
							if (offset <= 0)
							{
								result = null;
								return result;
							}
						}
						int startOffset = offset;
						for (int i = startLine - 1; i < endLine; i++)
						{
							int newOffset = fileContent.IndexOf('\n', offset) + 1;
							if (newOffset <= 0)
							{
								break;
							}
							offset = newOffset;
						}
						int length = offset - startOffset;
						string classDecl;
						string endClassDecl;
						if (language == SupportedLanguage.VBNet)
						{
							classDecl = "Class A";
							endClassDecl = "End Class\n";
						}
						else
						{
							classDecl = "class A {";
							endClassDecl = "}\n";
						}
						StringBuilder b = new StringBuilder(classDecl, length + classDecl.Length + endClassDecl.Length + startLine - 1);
						b.Append('\n', startLine - 1);
						b.Append(fileContent, startOffset, length);
						b.Append(endClassDecl);
						result = new StringReader(b.ToString());
					}
				}
			}
			return result;
		}

		private ResolveResult ResolveIdentifier(string identifier, ExpressionContext context)
		{
			ResolveResult result = this.ResolveIdentifierInternal(identifier);
			ResolveResult result3;
			if (result is TypeResolveResult)
			{
				result3 = result;
			}
			else
			{
				ResolveResult result2 = null;
				IReturnType t = this.SearchType(identifier);
				if (t != null)
				{
					result2 = new TypeResolveResult(this.callingClass, this.callingMember, t);
				}
				else if (this.callingClass != null)
				{
					if (this.callingMember is IMethod)
					{
						foreach (ITypeParameter typeParameter in (this.callingMember as IMethod).TypeParameters)
						{
							if (this.IsSameName(identifier, typeParameter.Name))
							{
								result3 = new TypeResolveResult(this.callingClass, this.callingMember, new GenericReturnType(typeParameter));
								return result3;
							}
						}
					}
					foreach (ITypeParameter typeParameter in this.callingClass.TypeParameters)
					{
						if (this.IsSameName(identifier, typeParameter.Name))
						{
							result3 = new TypeResolveResult(this.callingClass, this.callingMember, new GenericReturnType(typeParameter));
							return result3;
						}
					}
				}
				if (result == null)
				{
					result3 = result2;
				}
				else if (result2 == null)
				{
					result3 = result;
				}
				else if (context == ExpressionContext.Type)
				{
					result3 = result2;
				}
				else
				{
					result3 = new MixedResolveResult(result, result2);
				}
			}
			return result3;
		}

		private IField CreateLocalVariableField(LocalLookupVariable var, string identifier)
		{
			IReturnType type = this.GetVariableType(var);
			IField f = new DefaultField.LocalVariableField(type, identifier, new DomRegion(var.StartPos, var.EndPos), this.callingClass);
			if (var.IsConst)
			{
				f.Modifiers |= ModifierEnum.Const;
			}
			return f;
		}

		private ResolveResult ResolveIdentifierInternal(string identifier)
		{
			ResolveResult result2;
			if (this.callingMember != null)
			{
				LocalLookupVariable var = this.SearchVariable(identifier);
				if (var != null)
				{
					result2 = new LocalResolveResult(this.callingMember, this.CreateLocalVariableField(var, identifier));
					return result2;
				}
				IParameter para = this.SearchMethodParameter(identifier);
				if (para != null)
				{
					IField field = new DefaultField.ParameterField(para.ReturnType, para.Name, para.Region, this.callingClass);
					result2 = new LocalResolveResult(this.callingMember, field);
					return result2;
				}
				if (this.IsSameName(identifier, "value"))
				{
					IProperty property = this.callingMember as IProperty;
					if (property != null && property.SetterRegion.IsInside(this.caretLine, this.caretColumn))
					{
						IField field = new DefaultField.ParameterField(property.ReturnType, "value", property.Region, this.callingClass);
						result2 = new LocalResolveResult(this.callingMember, field);
						return result2;
					}
				}
			}
			if (this.callingClass != null)
			{
				IMember member = this.GetMember(this.callingClass.DefaultReturnType, identifier);
				if (member != null)
				{
					result2 = this.CreateMemberResolveResult(member);
					return result2;
				}
				ResolveResult result = this.ResolveMethod(this.callingClass.DefaultReturnType, identifier);
				if (result != null)
				{
					result2 = result;
					return result2;
				}
			}
			List<IClass> classes = this.cu.GetOuterClasses(this.caretLine, this.caretColumn);
			foreach (IClass c in classes)
			{
				IClass c;
				IMember member = this.GetMember(c.DefaultReturnType, identifier);
				if (member != null && member.IsStatic)
				{
					result2 = new MemberResolveResult(this.callingClass, this.callingMember, member);
					return result2;
				}
			}
			string namespaceName = this.SearchNamespace(identifier);
			if (namespaceName != null && namespaceName.Length > 0)
			{
				result2 = new NamespaceResolveResult(this.callingClass, this.callingMember, namespaceName);
			}
			else
			{
				if (this.languageProperties.CanImportClasses)
				{
					foreach (IUsing @using in this.cu.Usings)
					{
						foreach (string import in @using.Usings)
						{
							IClass c = this.GetClass(import);
							if (c != null)
							{
								IMember member = this.GetMember(c.DefaultReturnType, identifier);
								if (member != null)
								{
									result2 = this.CreateMemberResolveResult(member);
									return result2;
								}
								ResolveResult result = this.ResolveMethod(c.DefaultReturnType, identifier);
								if (result != null)
								{
									result2 = result;
									return result2;
								}
							}
						}
					}
				}
				if (this.languageProperties.ImportModules)
				{
					ArrayList list = new ArrayList();
					CtrlSpaceResolveHelper.AddImportedNamespaceContents(list, this.cu, this.callingClass);
					foreach (object o in list)
					{
						IClass c = o as IClass;
						if (c != null && this.IsSameName(identifier, c.Name))
						{
							result2 = new TypeResolveResult(this.callingClass, this.callingMember, c);
							return result2;
						}
						IMember member = o as IMember;
						if (member != null && this.IsSameName(identifier, member.Name))
						{
							if (member is IMethod)
							{
								result2 = new MethodResolveResult(this.callingClass, this.callingMember, member.DeclaringType.DefaultReturnType, member.Name);
								return result2;
							}
							result2 = this.CreateMemberResolveResult(member);
							return result2;
						}
					}
				}
				result2 = null;
			}
			return result2;
		}

		private ResolveResult CreateMemberResolveResult(IMember member)
		{
			ResolveResult result;
			if (member == null)
			{
				result = null;
			}
			else
			{
				result = new MemberResolveResult(this.callingClass, this.callingMember, member);
			}
			return result;
		}

		private ResolveResult ResolveMethod(IReturnType type, string identifier)
		{
			ResolveResult result;
			if (type == null)
			{
				result = null;
			}
			else
			{
				foreach (IMethod method in type.GetMethods())
				{
					if (this.IsSameName(identifier, method.Name))
					{
						result = new MethodResolveResult(this.callingClass, this.callingMember, type, identifier);
						return result;
					}
				}
				if (this.languageProperties.SupportsExtensionMethods && this.callingClass != null)
				{
					ArrayList list = new ArrayList();
					ResolveResult.AddExtensions(this.languageProperties, list, this.callingClass, type);
					foreach (IMethodOrProperty mp in list)
					{
						if (mp is IMethod && this.IsSameName(mp.Name, identifier))
						{
							result = new MethodResolveResult(this.callingClass, this.callingMember, type, identifier);
							return result;
						}
					}
				}
				result = null;
			}
			return result;
		}

		private Expression SpecialConstructs(string expression)
		{
			Expression result;
			if (this.language == SupportedLanguage.VBNet)
			{
				if ("mybase".Equals(expression, StringComparison.InvariantCultureIgnoreCase))
				{
					result = new BaseReferenceExpression();
					return result;
				}
				if ("myclass".Equals(expression, StringComparison.InvariantCultureIgnoreCase))
				{
					result = new ClassReferenceExpression();
					return result;
				}
			}
			else if (this.language == SupportedLanguage.CSharp)
			{
				if (expression.EndsWith(">"))
				{
					FieldReferenceExpression expr = this.ParseExpression(expression + ".Prop") as FieldReferenceExpression;
					if (expr != null)
					{
						result = expr.TargetObject;
						return result;
					}
				}
				result = null;
				return result;
			}
			result = null;
			return result;
		}

		public bool IsSameName(string name1, string name2)
		{
			return this.languageProperties.NameComparer.Equals(name1, name2);
		}

		private bool IsInside(Location between, Location start, Location end)
		{
			bool result;
			if (between.Y < start.Y || between.Y > end.Y)
			{
				result = false;
			}
			else if (between.Y > start.Y)
			{
				result = (between.Y < end.Y || between.X <= end.X);
			}
			else
			{
				result = (between.X >= start.X && (between.Y < end.Y || between.X <= end.X));
			}
			return result;
		}

		private IMember GetCurrentMember()
		{
			IMember result;
			if (this.callingClass == null)
			{
				result = null;
			}
			else
			{
				foreach (IMethod method in this.callingClass.Methods)
				{
					if (method.Region.IsInside(this.caretLine, this.caretColumn) || method.BodyRegion.IsInside(this.caretLine, this.caretColumn))
					{
						result = method;
						return result;
					}
				}
				foreach (IProperty property in this.callingClass.Properties)
				{
					if (property.Region.IsInside(this.caretLine, this.caretColumn) || property.BodyRegion.IsInside(this.caretLine, this.caretColumn))
					{
						result = property;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public string SearchNamespace(string name)
		{
			return this.projectContent.SearchNamespace(name, this.callingClass, this.cu, this.caretLine, this.caretColumn);
		}

		public IClass GetClass(string fullName)
		{
			return this.projectContent.GetClass(fullName);
		}

		public IClass SearchClass(string name)
		{
			IReturnType t = this.SearchType(name);
			return (t != null) ? t.GetUnderlyingClass() : null;
		}

		public IReturnType SearchType(string name)
		{
			return this.projectContent.SearchType(new SearchTypeRequest(name, 0, this.callingClass, this.cu, this.caretLine, this.caretColumn)).Result;
		}

		public List<IMethod> SearchMethod(string memberName)
		{
			List<IMethod> methods = this.SearchMethod(this.callingClass.DefaultReturnType, memberName);
			List<IMethod> result;
			if (methods.Count > 0)
			{
				result = methods;
			}
			else
			{
				if (this.languageProperties.CanImportClasses)
				{
					foreach (IUsing @using in this.cu.Usings)
					{
						foreach (string import in @using.Usings)
						{
							IClass c = this.projectContent.GetClass(import, 0);
							if (c != null)
							{
								methods = this.SearchMethod(c.DefaultReturnType, memberName);
								if (methods.Count > 0)
								{
									result = methods;
									return result;
								}
							}
						}
					}
				}
				if (this.languageProperties.ImportModules)
				{
					ArrayList list = new ArrayList();
					CtrlSpaceResolveHelper.AddImportedNamespaceContents(list, this.cu, this.callingClass);
					foreach (object o in list)
					{
						IMethod i = o as IMethod;
						if (i != null && this.IsSameName(i.Name, memberName))
						{
							methods.Add(i);
						}
					}
				}
				result = methods;
			}
			return result;
		}

		public List<IMethod> SearchMethod(IReturnType type, string memberName)
		{
			List<IMethod> methods = new List<IMethod>();
			List<IMethod> result;
			if (type == null)
			{
				result = methods;
			}
			else
			{
				bool isClassInInheritanceTree = false;
				if (this.callingClass != null)
				{
					isClassInInheritanceTree = this.callingClass.IsTypeInInheritanceTree(type.GetUnderlyingClass());
				}
				foreach (IMethod i in type.GetMethods())
				{
					if (this.IsSameName(i.Name, memberName) && i.IsAccessible(this.callingClass, isClassInInheritanceTree))
					{
						methods.Add(i);
					}
				}
				if (methods.Count == 0)
				{
					if (this.languageProperties.SupportsExtensionMethods && this.callingClass != null)
					{
						ArrayList list = new ArrayList();
						ResolveResult.AddExtensions(this.languageProperties, list, this.callingClass, type);
						foreach (IMethodOrProperty mp in list)
						{
							if (mp is IMethod && this.IsSameName(mp.Name, memberName))
							{
								methods.Add((IMethod)mp);
							}
						}
					}
				}
				result = methods;
			}
			return result;
		}

		public IReturnType SearchMember(IReturnType type, string memberName)
		{
			IReturnType result;
			if (type == null)
			{
				result = null;
			}
			else
			{
				IMember member = this.GetMember(type, memberName);
				if (member == null)
				{
					result = null;
				}
				else
				{
					result = member.ReturnType;
				}
			}
			return result;
		}

		public IMember GetMember(IReturnType type, string memberName)
		{
			IMember result;
			if (type == null)
			{
				result = null;
			}
			else
			{
				if (this.callingClass != null)
				{
					bool isClassInInheritanceTree = this.callingClass.IsTypeInInheritanceTree(type.GetUnderlyingClass());
				}
				foreach (IProperty p in type.GetProperties())
				{
					if (this.IsSameName(p.Name, memberName))
					{
						result = p;
						return result;
					}
				}
				foreach (IField f in type.GetFields())
				{
					if (this.IsSameName(f.Name, memberName))
					{
						result = f;
						return result;
					}
				}
				foreach (IEvent e in type.GetEvents())
				{
					if (this.IsSameName(e.Name, memberName))
					{
						result = e;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public IReturnType DynamicLookup(string identifier)
		{
			ResolveResult rr = this.ResolveIdentifierInternal(identifier);
			IReturnType result;
			if (rr is NamespaceResolveResult)
			{
				result = new TypeVisitor.NamespaceReturnType((rr as NamespaceResolveResult).Name);
			}
			else
			{
				result = ((rr != null) ? rr.ResolvedType : null);
			}
			return result;
		}

		private IParameter SearchMethodParameter(string parameter)
		{
			IMethodOrProperty method = this.callingMember as IMethodOrProperty;
			IParameter result;
			if (method == null)
			{
				result = null;
			}
			else
			{
				foreach (IParameter p in method.Parameters)
				{
					if (this.IsSameName(p.Name, parameter))
					{
						result = p;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		private IReturnType GetVariableType(LocalLookupVariable v)
		{
			IReturnType result;
			if (v == null)
			{
				result = null;
			}
			else
			{
				result = TypeVisitor.CreateReturnType(v.TypeRef, this);
			}
			return result;
		}

		private LocalLookupVariable SearchVariable(string name)
		{
			LocalLookupVariable result;
			if (this.lookupTableVisitor == null || !this.lookupTableVisitor.Variables.ContainsKey(name))
			{
				result = null;
			}
			else
			{
				List<LocalLookupVariable> variables = this.lookupTableVisitor.Variables[name];
				if (variables.Count <= 0)
				{
					result = null;
				}
				else
				{
					foreach (LocalLookupVariable v in variables)
					{
						if (this.IsInside(new Location(this.caretColumn, this.caretLine), v.StartPos, v.EndPos))
						{
							result = v;
							return result;
						}
					}
					result = null;
				}
			}
			return result;
		}

		private IClass GetPrimitiveClass(string systemType, string newName)
		{
			IClass c = this.projectContent.GetClass(systemType);
			IClass result;
			if (c == null)
			{
				result = null;
			}
			else
			{
				DefaultClass c2 = new DefaultClass(c.CompilationUnit, newName);
				c2.ClassType = c.ClassType;
				c2.Modifiers = c.Modifiers;
				c2.Documentation = c.Documentation;
				c2.BaseTypes.AddRange(c.BaseTypes);
				c2.Methods.AddRange(c.Methods);
				c2.Fields.AddRange(c.Fields);
				c2.Properties.AddRange(c.Properties);
				c2.Events.AddRange(c.Events);
				result = c2;
			}
			return result;
		}

		public ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent, ExpressionContext context)
		{
			ArrayList result = new ArrayList();
			if (this.language == SupportedLanguage.VBNet)
			{
				foreach (KeyValuePair<string, string> pair in TypeReference.PrimitiveTypesVB)
				{
					if ("System." + pair.Key != pair.Value)
					{
						IClass c = this.GetPrimitiveClass(pair.Value, pair.Key);
						if (c != null)
						{
							result.Add(c);
						}
					}
				}
				result.Add("Global");
				result.Add("New");
			}
			else
			{
				foreach (KeyValuePair<string, string> pair in TypeReference.PrimitiveTypesCSharp)
				{
					IClass c = this.GetPrimitiveClass(pair.Value, pair.Key);
					if (c != null)
					{
						result.Add(c);
					}
				}
			}
			ParseInformation parseInfo = HostCallback.GetParseInformation(fileName);
			ArrayList result2;
			if (parseInfo == null)
			{
				result2 = null;
			}
			else
			{
				this.caretLine = caretLine;
				this.caretColumn = caretColumn;
				this.lookupTableVisitor = new LookupTableVisitor(this.language);
				this.cu = parseInfo.MostRecentCompilationUnit;
				if (this.cu != null)
				{
					this.callingClass = this.cu.GetInnermostClass(caretLine, caretColumn);
				}
				this.callingMember = this.GetCurrentMember();
				if (this.callingMember != null)
				{
					CompilationUnit parsedCu = this.ParseCurrentMemberAsCompilationUnit(fileContent);
					if (parsedCu != null)
					{
						this.lookupTableVisitor.VisitCompilationUnit(parsedCu, null);
					}
				}
				CtrlSpaceResolveHelper.AddContentsFromCalling(result, this.callingClass, this.callingMember);
				foreach (KeyValuePair<string, List<LocalLookupVariable>> pair2 in this.lookupTableVisitor.Variables)
				{
					if (pair2.Value != null && pair2.Value.Count > 0)
					{
						foreach (LocalLookupVariable v in pair2.Value)
						{
							if (this.IsInside(new Location(caretColumn, caretLine), v.StartPos, v.EndPos))
							{
								result.Add(this.CreateLocalVariableField(v, pair2.Key));
								break;
							}
						}
					}
				}
				if (this.callingMember is IProperty)
				{
					IProperty property = (IProperty)this.callingMember;
					if (property.SetterRegion.IsInside(caretLine, caretColumn))
					{
						result.Add(new DefaultField.ParameterField(property.ReturnType, "value", property.Region, this.callingClass));
					}
				}
				CtrlSpaceResolveHelper.AddImportedNamespaceContents(result, this.cu, this.callingClass);
				result2 = result;
			}
			return result2;
		}
	}
}

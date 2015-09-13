using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public class CodeDomVisitor : AbstractAstVisitor
	{
		private Stack<CodeNamespace> namespaceDeclarations = new Stack<CodeNamespace>();

		private Stack<CodeTypeDeclaration> typeDeclarations = new Stack<CodeTypeDeclaration>();

		private Stack<CodeStatementCollection> codeStack = new Stack<CodeStatementCollection>();

		private List<CodeVariableDeclarationStatement> variables = new List<CodeVariableDeclarationStatement>();

		private TypeDeclaration currentTypeDeclaration = null;

		private IEnvironmentInformationProvider environmentInformationProvider = new DummyEnvironmentInformationProvider();

		private CodeStatementCollection NullStmtCollection = new CodeStatementCollection();

		public CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

		private bool methodReference = false;

		public IEnvironmentInformationProvider EnvironmentInformationProvider
		{
			get
			{
				return this.environmentInformationProvider;
			}
			set
			{
				this.environmentInformationProvider = value;
			}
		}

		private CodeTypeReference ConvType(TypeReference type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(type.SystemType))
			{
				throw new InvalidOperationException("empty type");
			}
			CodeTypeReference t = new CodeTypeReference(type.SystemType);
			foreach (TypeReference gt in type.GenericTypes)
			{
				t.TypeArguments.Add(this.ConvType(gt));
			}
			if (type.IsArrayType)
			{
				t = new CodeTypeReference(t, type.RankSpecifier.Length);
			}
			return t;
		}

		private void AddStmt(CodeStatement stmt)
		{
			if (this.codeStack.Count != 0)
			{
				CodeStatementCollection stmtCollection = this.codeStack.Peek();
				if (stmtCollection != null)
				{
					stmtCollection.Add(stmt);
				}
			}
		}

		private static MemberAttributes ConvMemberAttributes(Modifiers modifier)
		{
			MemberAttributes attr = (MemberAttributes)0;
			if ((modifier & Modifiers.Dim) != Modifiers.None)
			{
				attr |= MemberAttributes.Abstract;
			}
			if ((modifier & Modifiers.Const) != Modifiers.None)
			{
				attr |= MemberAttributes.Const;
			}
			if ((modifier & Modifiers.Sealed) != Modifiers.None)
			{
				attr |= MemberAttributes.Final;
			}
			if ((modifier & Modifiers.New) != Modifiers.None)
			{
				attr |= MemberAttributes.New;
			}
			if ((modifier & Modifiers.Virtual) != Modifiers.None)
			{
				attr |= MemberAttributes.Overloaded;
			}
			if ((modifier & Modifiers.Override) != Modifiers.None)
			{
				attr |= MemberAttributes.Override;
			}
			if ((modifier & Modifiers.Static) != Modifiers.None)
			{
				attr |= MemberAttributes.Static;
			}
			if ((modifier & Modifiers.Private) != Modifiers.None)
			{
				attr |= MemberAttributes.Private;
			}
			else if ((modifier & Modifiers.Public) != Modifiers.None)
			{
				attr |= MemberAttributes.Public;
			}
			else if ((modifier & Modifiers.Internal) != Modifiers.None && (modifier & Modifiers.Protected) != Modifiers.None)
			{
				attr |= MemberAttributes.FamilyOrAssembly;
			}
			else if ((modifier & Modifiers.Internal) != Modifiers.None)
			{
				attr |= MemberAttributes.Assembly;
			}
			else if ((modifier & Modifiers.Protected) != Modifiers.None)
			{
				attr |= MemberAttributes.Family;
			}
			return attr;
		}

		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			if (compilationUnit == null)
			{
				throw new ArgumentNullException("compilationUnit");
			}
			CodeNamespace globalNamespace = new CodeNamespace("Global");
			this.namespaceDeclarations.Push(globalNamespace);
			compilationUnit.AcceptChildren(this, data);
			this.codeCompileUnit.Namespaces.Add(globalNamespace);
			return globalNamespace;
		}

		public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			CodeNamespace currentNamespace = new CodeNamespace(namespaceDeclaration.Name);
			foreach (CodeNamespaceImport import in this.namespaceDeclarations.Peek().Imports)
			{
				currentNamespace.Imports.Add(import);
			}
			this.namespaceDeclarations.Push(currentNamespace);
			namespaceDeclaration.AcceptChildren(this, data);
			this.namespaceDeclarations.Pop();
			this.codeCompileUnit.Namespaces.Add(currentNamespace);
			return null;
		}

		public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			foreach (Using u in usingDeclaration.Usings)
			{
				this.namespaceDeclarations.Peek().Imports.Add(new CodeNamespaceImport(u.Name));
			}
			return null;
		}

		public override object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			return null;
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			TypeDeclaration oldTypeDeclaration = this.currentTypeDeclaration;
			this.currentTypeDeclaration = typeDeclaration;
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(typeDeclaration.Name);
			codeTypeDeclaration.IsClass = (typeDeclaration.Type == ClassType.Class);
			codeTypeDeclaration.IsEnum = (typeDeclaration.Type == ClassType.Enum);
			codeTypeDeclaration.IsInterface = (typeDeclaration.Type == ClassType.Interface);
			codeTypeDeclaration.IsStruct = (typeDeclaration.Type == ClassType.Struct);
			if (typeDeclaration.BaseTypes != null)
			{
				foreach (TypeReference typeRef in typeDeclaration.BaseTypes)
				{
					codeTypeDeclaration.BaseTypes.Add(this.ConvType(typeRef));
				}
			}
			this.typeDeclarations.Push(codeTypeDeclaration);
			typeDeclaration.AcceptChildren(this, data);
			this.typeDeclarations.Pop();
			if (this.typeDeclarations.Count > 0)
			{
				this.typeDeclarations.Peek().Members.Add(codeTypeDeclaration);
			}
			else
			{
				this.namespaceDeclarations.Peek().Types.Add(codeTypeDeclaration);
			}
			this.currentTypeDeclaration = oldTypeDeclaration;
			return null;
		}

		public override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			return null;
		}

		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			return null;
		}

		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			for (int i = 0; i < fieldDeclaration.Fields.Count; i++)
			{
				VariableDeclaration field = fieldDeclaration.Fields[i];
				if ((fieldDeclaration.Modifier & Modifiers.WithEvents) != Modifiers.None)
				{
				}
				TypeReference fieldType = fieldDeclaration.GetTypeForField(i);
				if (fieldType.IsNull)
				{
					fieldType = new TypeReference(this.typeDeclarations.Peek().Name);
				}
				CodeMemberField memberField = new CodeMemberField(this.ConvType(fieldType), field.Name);
				memberField.Attributes = CodeDomVisitor.ConvMemberAttributes(fieldDeclaration.Modifier);
				if (!field.Initializer.IsNull)
				{
					memberField.InitExpression = (CodeExpression)field.Initializer.AcceptVisitor(this, data);
				}
				this.typeDeclarations.Peek().Members.Add(memberField);
			}
			return null;
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			CodeMemberMethod memberMethod = new CodeMemberMethod();
			memberMethod.Name = methodDeclaration.Name;
			memberMethod.Attributes = CodeDomVisitor.ConvMemberAttributes(methodDeclaration.Modifier);
			this.codeStack.Push(memberMethod.Statements);
			this.typeDeclarations.Peek().Members.Add(memberMethod);
			foreach (ParameterDeclarationExpression parameter in methodDeclaration.Parameters)
			{
				memberMethod.Parameters.Add((CodeParameterDeclarationExpression)this.VisitParameterDeclarationExpression(parameter, data));
			}
			this.variables.Clear();
			methodDeclaration.Body.AcceptChildren(this, data);
			this.codeStack.Pop();
			return null;
		}

		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			CodeMemberMethod memberMethod = new CodeConstructor();
			this.codeStack.Push(memberMethod.Statements);
			this.typeDeclarations.Peek().Members.Add(memberMethod);
			constructorDeclaration.Body.AcceptChildren(this, data);
			this.codeStack.Pop();
			return null;
		}

		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			blockStatement.AcceptChildren(this, data);
			return null;
		}

		public override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			object exp = expressionStatement.Expression.AcceptVisitor(this, data);
			if (exp is CodeExpression)
			{
				this.AddStmt(new CodeExpressionStatement((CodeExpression)exp));
			}
			return exp;
		}

		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			CodeVariableDeclarationStatement declStmt = null;
			for (int i = 0; i < localVariableDeclaration.Variables.Count; i++)
			{
				CodeTypeReference type = this.ConvType(localVariableDeclaration.GetTypeForVariable(i) ?? new TypeReference("object"));
				VariableDeclaration var = localVariableDeclaration.Variables[i];
				if (!var.Initializer.IsNull)
				{
					declStmt = new CodeVariableDeclarationStatement(type, var.Name, (CodeExpression)((INode)var.Initializer).AcceptVisitor(this, data));
				}
				else
				{
					declStmt = new CodeVariableDeclarationStatement(type, var.Name);
				}
				this.variables.Add(declStmt);
				this.AddStmt(declStmt);
			}
			return declStmt;
		}

		public override object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			CodeSnippetStatement emptyStmt = new CodeSnippetStatement();
			this.AddStmt(emptyStmt);
			return emptyStmt;
		}

		public override object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			CodeMethodReturnStatement returnStmt;
			if (returnStatement.Expression.IsNull)
			{
				returnStmt = new CodeMethodReturnStatement();
			}
			else
			{
				returnStmt = new CodeMethodReturnStatement((CodeExpression)returnStatement.Expression.AcceptVisitor(this, data));
			}
			this.AddStmt(returnStmt);
			return returnStmt;
		}

		public override object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			CodeConditionStatement ifStmt = new CodeConditionStatement();
			ifStmt.Condition = (CodeExpression)ifElseStatement.Condition.AcceptVisitor(this, data);
			this.codeStack.Push(ifStmt.TrueStatements);
			foreach (Statement stmt in ifElseStatement.TrueStatement)
			{
				if (stmt is BlockStatement)
				{
					stmt.AcceptChildren(this, data);
				}
				else
				{
					stmt.AcceptVisitor(this, data);
				}
			}
			this.codeStack.Pop();
			this.codeStack.Push(ifStmt.FalseStatements);
			foreach (Statement stmt in ifElseStatement.FalseStatement)
			{
				if (stmt is BlockStatement)
				{
					stmt.AcceptChildren(this, data);
				}
				else
				{
					stmt.AcceptVisitor(this, data);
				}
			}
			this.codeStack.Pop();
			this.AddStmt(ifStmt);
			return ifStmt;
		}

		public override object VisitForStatement(ForStatement forStatement, object data)
		{
			CodeIterationStatement forLoop = new CodeIterationStatement();
			if (forStatement.Initializers.Count > 0)
			{
				if (forStatement.Initializers.Count > 1)
				{
					throw new NotSupportedException("CodeDom does not support Multiple For-Loop Initializer Statements");
				}
				foreach (object o in forStatement.Initializers)
				{
					if (o is Expression)
					{
						forLoop.InitStatement = new CodeExpressionStatement((CodeExpression)((Expression)o).AcceptVisitor(this, data));
					}
					if (o is Statement)
					{
						this.codeStack.Push(this.NullStmtCollection);
						forLoop.InitStatement = (CodeStatement)((Statement)o).AcceptVisitor(this, data);
						this.codeStack.Pop();
					}
				}
			}
			if (forStatement.Condition == null)
			{
				forLoop.TestExpression = new CodePrimitiveExpression(true);
			}
			else
			{
				forLoop.TestExpression = (CodeExpression)forStatement.Condition.AcceptVisitor(this, data);
			}
			this.codeStack.Push(forLoop.Statements);
			forStatement.EmbeddedStatement.AcceptVisitor(this, data);
			this.codeStack.Pop();
			if (forStatement.Iterator.Count > 0)
			{
				if (forStatement.Initializers.Count > 1)
				{
					throw new NotSupportedException("CodeDom does not support Multiple For-Loop Iterator Statements");
				}
				foreach (Statement stmt in forStatement.Iterator)
				{
					forLoop.IncrementStatement = (CodeStatement)stmt.AcceptVisitor(this, data);
				}
			}
			this.AddStmt(forLoop);
			return forLoop;
		}

		public override object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			CodeLabeledStatement labelStmt = new CodeLabeledStatement(labelStatement.Label, (CodeStatement)labelStatement.AcceptVisitor(this, data));
			this.AddStmt(labelStmt);
			return labelStmt;
		}

		public override object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			CodeGotoStatement gotoStmt = new CodeGotoStatement(gotoStatement.Label);
			this.AddStmt(gotoStmt);
			return gotoStmt;
		}

		public override object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			throw new NotSupportedException("CodeDom does not support Switch Statement");
		}

		public override object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			CodeTryCatchFinallyStatement tryStmt = new CodeTryCatchFinallyStatement();
			this.codeStack.Push(tryStmt.TryStatements);
			tryCatchStatement.StatementBlock.AcceptChildren(this, data);
			this.codeStack.Pop();
			if (!tryCatchStatement.FinallyBlock.IsNull)
			{
				this.codeStack.Push(tryStmt.FinallyStatements);
				tryCatchStatement.FinallyBlock.AcceptChildren(this, data);
				this.codeStack.Pop();
			}
			foreach (CatchClause clause in tryCatchStatement.CatchClauses)
			{
				CodeCatchClause catchClause = new CodeCatchClause(clause.VariableName);
				catchClause.CatchExceptionType = new CodeTypeReference(clause.TypeReference.Type);
				tryStmt.CatchClauses.Add(catchClause);
				this.codeStack.Push(catchClause.Statements);
				clause.StatementBlock.AcceptChildren(this, data);
				this.codeStack.Pop();
			}
			this.AddStmt(tryStmt);
			return tryStmt;
		}

		public override object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			CodeThrowExceptionStatement throwStmt = new CodeThrowExceptionStatement((CodeExpression)throwStatement.Expression.AcceptVisitor(this, data));
			this.AddStmt(throwStmt);
			return throwStmt;
		}

		public override object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			throw new NotSupportedException("CodeDom does not support Fixed Statement");
		}

		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			return new CodePrimitiveExpression(primitiveExpression.Value);
		}

		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			CodeBinaryOperatorType op = CodeBinaryOperatorType.Add;
			switch (binaryOperatorExpression.Op)
			{
			case BinaryOperatorType.BitwiseAnd:
				op = CodeBinaryOperatorType.BitwiseAnd;
				break;
			case BinaryOperatorType.BitwiseOr:
				op = CodeBinaryOperatorType.BitwiseOr;
				break;
			case BinaryOperatorType.LogicalAnd:
				op = CodeBinaryOperatorType.BooleanAnd;
				break;
			case BinaryOperatorType.LogicalOr:
				op = CodeBinaryOperatorType.BooleanOr;
				break;
			case BinaryOperatorType.ExclusiveOr:
				op = CodeBinaryOperatorType.BitwiseAnd;
				break;
			case BinaryOperatorType.GreaterThan:
				op = CodeBinaryOperatorType.GreaterThan;
				break;
			case BinaryOperatorType.GreaterThanOrEqual:
				op = CodeBinaryOperatorType.GreaterThanOrEqual;
				break;
			case BinaryOperatorType.Equality:
				op = CodeBinaryOperatorType.IdentityEquality;
				break;
			case BinaryOperatorType.InEquality:
				op = CodeBinaryOperatorType.IdentityInequality;
				break;
			case BinaryOperatorType.LessThan:
				op = CodeBinaryOperatorType.LessThan;
				break;
			case BinaryOperatorType.LessThanOrEqual:
				op = CodeBinaryOperatorType.LessThanOrEqual;
				break;
			case BinaryOperatorType.Add:
				op = CodeBinaryOperatorType.Add;
				break;
			case BinaryOperatorType.Subtract:
				op = CodeBinaryOperatorType.Subtract;
				break;
			case BinaryOperatorType.Multiply:
				op = CodeBinaryOperatorType.Multiply;
				break;
			case BinaryOperatorType.Divide:
			case BinaryOperatorType.DivideInteger:
				op = CodeBinaryOperatorType.Divide;
				break;
			case BinaryOperatorType.Modulus:
				op = CodeBinaryOperatorType.Modulus;
				break;
			case BinaryOperatorType.ShiftLeft:
			case BinaryOperatorType.ShiftRight:
				op = CodeBinaryOperatorType.Multiply;
				break;
			case BinaryOperatorType.ReferenceEquality:
				op = CodeBinaryOperatorType.IdentityEquality;
				break;
			case BinaryOperatorType.ReferenceInequality:
				op = CodeBinaryOperatorType.IdentityInequality;
				break;
			}
			return new CodeBinaryOperatorExpression((CodeExpression)binaryOperatorExpression.Left.AcceptVisitor(this, data), op, (CodeExpression)binaryOperatorExpression.Right.AcceptVisitor(this, data));
		}

		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}

		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Expression target = invocationExpression.TargetObject;
			string methodName = null;
			CodeExpression targetExpr;
			object result;
			if (target == null)
			{
				targetExpr = new CodeThisReferenceExpression();
			}
			else if (target is FieldReferenceExpression)
			{
				FieldReferenceExpression fRef = (FieldReferenceExpression)target;
				targetExpr = null;
				if (fRef.TargetObject is FieldReferenceExpression)
				{
					if (this.IsPossibleTypeReference((FieldReferenceExpression)fRef.TargetObject))
					{
						targetExpr = CodeDomVisitor.ConvertToTypeReference((FieldReferenceExpression)fRef.TargetObject);
					}
				}
				if (targetExpr == null)
				{
					targetExpr = (CodeExpression)fRef.TargetObject.AcceptVisitor(this, data);
				}
				methodName = fRef.FieldName;
				if (methodName == "ChrW")
				{
					result = new CodeCastExpression("System.Char", this.GetExpressionList(invocationExpression.Arguments)[0]);
					return result;
				}
			}
			else if (target is IdentifierExpression)
			{
				targetExpr = new CodeThisReferenceExpression();
				methodName = ((IdentifierExpression)target).Identifier;
			}
			else
			{
				targetExpr = (CodeExpression)target.AcceptVisitor(this, data);
			}
			result = new CodeMethodInvokeExpression(targetExpr, methodName, this.GetExpressionList(invocationExpression.Arguments));
			return result;
		}

		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			object result;
			if (!this.IsLocalVariable(identifierExpression.Identifier) && this.IsField(identifierExpression.Identifier))
			{
				result = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), identifierExpression.Identifier);
			}
			else
			{
				result = new CodeVariableReferenceExpression(identifierExpression.Identifier);
			}
			return result;
		}

		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			object result;
			switch (unaryOperatorExpression.Op)
			{
			case UnaryOperatorType.Minus:
				if (unaryOperatorExpression.Expression is PrimitiveExpression)
				{
					PrimitiveExpression expression = (PrimitiveExpression)unaryOperatorExpression.Expression;
					if (expression.Value is int)
					{
						result = new CodePrimitiveExpression(-(int)expression.Value);
						break;
					}
					if (expression.Value is uint || expression.Value is ushort)
					{
						result = new CodePrimitiveExpression(int.Parse("-" + expression.StringValue));
						break;
					}
					if (expression.Value is long)
					{
						result = new CodePrimitiveExpression(-(long)expression.Value);
						break;
					}
					if (expression.Value is double)
					{
						result = new CodePrimitiveExpression(-(double)expression.Value);
						break;
					}
					if (expression.Value is float)
					{
						result = new CodePrimitiveExpression(-(float)expression.Value);
						break;
					}
				}
				result = new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data));
				break;
			case UnaryOperatorType.Plus:
				result = unaryOperatorExpression.Expression.AcceptVisitor(this, data);
				break;
			case UnaryOperatorType.Increment:
			{
				CodeExpression var = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data);
				result = new CodeAssignStatement(var, new CodeBinaryOperatorExpression(var, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));
				break;
			}
			case UnaryOperatorType.Decrement:
			{
				CodeExpression var = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data);
				result = new CodeAssignStatement(var, new CodeBinaryOperatorExpression(var, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)));
				break;
			}
			case UnaryOperatorType.PostIncrement:
			{
				CodeExpression var = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data);
				result = new CodeAssignStatement(var, new CodeBinaryOperatorExpression(var, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));
				break;
			}
			case UnaryOperatorType.PostDecrement:
			{
				CodeExpression var = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data);
				result = new CodeAssignStatement(var, new CodeBinaryOperatorExpression(var, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)));
				break;
			}
			default:
				result = null;
				break;
			}
			return result;
		}

		private void AddEventHandler(Expression eventExpr, Expression handler, object data)
		{
			this.methodReference = true;
			CodeExpression methodInvoker = (CodeExpression)handler.AcceptVisitor(this, data);
			this.methodReference = false;
			if (!(methodInvoker is CodeObjectCreateExpression))
			{
				methodInvoker = new CodeObjectCreateExpression(new CodeTypeReference("System.EventHandler"), new CodeExpression[]
				{
					methodInvoker
				});
			}
			if (eventExpr is IdentifierExpression)
			{
				this.AddStmt(new CodeAttachEventStatement(new CodeEventReferenceExpression(new CodeThisReferenceExpression(), ((IdentifierExpression)eventExpr).Identifier), methodInvoker));
			}
			else
			{
				FieldReferenceExpression fr = (FieldReferenceExpression)eventExpr;
				this.AddStmt(new CodeAttachEventStatement(new CodeEventReferenceExpression((CodeExpression)fr.TargetObject.AcceptVisitor(this, data), fr.FieldName), methodInvoker));
			}
		}

		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			if (assignmentExpression.Op == AssignmentOperatorType.Add)
			{
				this.AddEventHandler(assignmentExpression.Left, assignmentExpression.Right, data);
			}
			else if (assignmentExpression.Left is IdentifierExpression)
			{
				this.AddStmt(new CodeAssignStatement((CodeExpression)assignmentExpression.Left.AcceptVisitor(this, null), (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, null)));
			}
			else
			{
				this.AddStmt(new CodeAssignStatement((CodeExpression)assignmentExpression.Left.AcceptVisitor(this, null), (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, null)));
			}
			return null;
		}

		public override object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			this.AddEventHandler(addHandlerStatement.EventExpression, addHandlerStatement.HandlerExpression, data);
			return null;
		}

		public override object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			return addressOfExpression.Expression.AcceptVisitor(this, data);
		}

		public override object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			return new CodeTypeOfExpression(this.ConvType(typeOfExpression.TypeReference));
		}

		public override object VisitCastExpression(CastExpression castExpression, object data)
		{
			CodeTypeReference typeRef = this.ConvType(castExpression.CastTo);
			return new CodeCastExpression(typeRef, (CodeExpression)castExpression.Expression.AcceptVisitor(this, data));
		}

		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			return new CodeIndexerExpression((CodeExpression)indexerExpression.TargetObject.AcceptVisitor(this, data), this.GetExpressionList(indexerExpression.Indexes));
		}

		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return new CodeThisReferenceExpression();
		}

		public override object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return new CodeBaseReferenceExpression();
		}

		public override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			object result;
			if (arrayCreateExpression.ArrayInitializer == null)
			{
				result = new CodeArrayCreateExpression(this.ConvType(arrayCreateExpression.CreateType), arrayCreateExpression.Arguments[0].AcceptVisitor(this, data) as CodeExpression);
			}
			else
			{
				result = new CodeArrayCreateExpression(this.ConvType(arrayCreateExpression.CreateType), this.GetExpressionList(arrayCreateExpression.ArrayInitializer.CreateExpressions));
			}
			return result;
		}

		public override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			return new CodeObjectCreateExpression(this.ConvType(objectCreateExpression.CreateType), (objectCreateExpression.Parameters == null) ? null : this.GetExpressionList(objectCreateExpression.Parameters));
		}

		public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			return new CodeParameterDeclarationExpression(this.ConvType(parameterDeclarationExpression.TypeReference), parameterDeclarationExpression.ParameterName);
		}

		private bool IsField(string type, string fieldName)
		{
			bool isField = this.environmentInformationProvider.HasField(type, fieldName);
			if (!isField)
			{
				int idx = type.LastIndexOf('.');
				if (idx >= 0)
				{
					type = type.Substring(0, idx) + "+" + type.Substring(idx + 1);
					isField = this.IsField(type, fieldName);
				}
			}
			return isField;
		}

		private bool IsFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression)
		{
			return (fieldReferenceExpression.TargetObject is ThisReferenceExpression || fieldReferenceExpression.TargetObject is BaseReferenceExpression) && this.IsField(fieldReferenceExpression.FieldName);
		}

		public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			object result;
			if (this.methodReference)
			{
				this.methodReference = false;
				result = new CodeMethodReferenceExpression((CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data), fieldReferenceExpression.FieldName);
			}
			else if (this.IsFieldReferenceExpression(fieldReferenceExpression))
			{
				result = new CodeFieldReferenceExpression((CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data), fieldReferenceExpression.FieldName);
			}
			else
			{
				if (fieldReferenceExpression.TargetObject is FieldReferenceExpression)
				{
					if (this.IsPossibleTypeReference((FieldReferenceExpression)fieldReferenceExpression.TargetObject))
					{
						CodeTypeReferenceExpression typeRef = CodeDomVisitor.ConvertToTypeReference((FieldReferenceExpression)fieldReferenceExpression.TargetObject);
						if (this.IsField(typeRef.Type.BaseType, fieldReferenceExpression.FieldName))
						{
							result = new CodeFieldReferenceExpression(typeRef, fieldReferenceExpression.FieldName);
							return result;
						}
						result = new CodePropertyReferenceExpression(typeRef, fieldReferenceExpression.FieldName);
						return result;
					}
				}
				CodeExpression codeExpression = (CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data);
				result = new CodePropertyReferenceExpression(codeExpression, fieldReferenceExpression.FieldName);
			}
			return result;
		}

		private bool IsPossibleTypeReference(FieldReferenceExpression fieldReferenceExpression)
		{
			while (fieldReferenceExpression.TargetObject is FieldReferenceExpression)
			{
				fieldReferenceExpression = (FieldReferenceExpression)fieldReferenceExpression.TargetObject;
			}
			IdentifierExpression identifier = fieldReferenceExpression.TargetObject as IdentifierExpression;
			bool result;
			if (identifier != null)
			{
				result = (!this.IsField(identifier.Identifier) && !this.IsLocalVariable(identifier.Identifier));
			}
			else
			{
				TypeReferenceExpression tre = fieldReferenceExpression.TargetObject as TypeReferenceExpression;
				result = (tre != null);
			}
			return result;
		}

		private bool IsLocalVariable(string identifier)
		{
			bool result;
			foreach (CodeVariableDeclarationStatement variable in this.variables)
			{
				if (variable.Name == identifier)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private bool IsField(string identifier)
		{
			bool result;
			if (this.currentTypeDeclaration == null)
			{
				result = false;
			}
			else
			{
				foreach (INode node in this.currentTypeDeclaration.Children)
				{
					if (node is FieldDeclaration)
					{
						FieldDeclaration fd = (FieldDeclaration)node;
						if (fd.GetVariableDeclaration(identifier) != null)
						{
							result = true;
							return result;
						}
					}
				}
				result = (this.currentTypeDeclaration.BaseTypes.Count > 0 && this.IsField(this.currentTypeDeclaration.BaseTypes[0].ToString(), identifier));
			}
			return result;
		}

		private static CodeTypeReferenceExpression ConvertToTypeReference(FieldReferenceExpression fieldReferenceExpression)
		{
			StringBuilder type = new StringBuilder("");
			while (fieldReferenceExpression.TargetObject is FieldReferenceExpression)
			{
				type.Insert(0, '.');
				type.Insert(1, fieldReferenceExpression.FieldName.ToCharArray());
				fieldReferenceExpression = (FieldReferenceExpression)fieldReferenceExpression.TargetObject;
			}
			type.Insert(0, '.');
			type.Insert(1, fieldReferenceExpression.FieldName.ToCharArray());
			CodeTypeReferenceExpression result;
			if (fieldReferenceExpression.TargetObject is IdentifierExpression)
			{
				type.Insert(0, ((IdentifierExpression)fieldReferenceExpression.TargetObject).Identifier.ToCharArray());
				string oldType = type.ToString();
				for (int idx = oldType.LastIndexOf('.'); idx > 0; idx = type.ToString().LastIndexOf('.'))
				{
					if (Type.GetType(type.ToString()) != null)
					{
						break;
					}
					string stype = type.ToString().Substring(idx + 1);
					type = new StringBuilder(type.ToString().Substring(0, idx));
					type.Append("+");
					type.Append(stype);
				}
				if (Type.GetType(type.ToString()) == null)
				{
					type = new StringBuilder(oldType);
				}
				result = new CodeTypeReferenceExpression(type.ToString());
			}
			else if (fieldReferenceExpression.TargetObject is TypeReferenceExpression)
			{
				type.Insert(0, ((TypeReferenceExpression)fieldReferenceExpression.TargetObject).TypeReference.SystemType);
				result = new CodeTypeReferenceExpression(type.ToString());
			}
			else
			{
				result = null;
			}
			return result;
		}

		private CodeExpression[] GetExpressionList(IList expressionList)
		{
			CodeExpression[] result;
			if (expressionList == null)
			{
				result = new CodeExpression[0];
			}
			else
			{
				CodeExpression[] list = new CodeExpression[expressionList.Count];
				for (int i = 0; i < expressionList.Count; i++)
				{
					list[i] = (CodeExpression)((Expression)expressionList[i]).AcceptVisitor(this, null);
					if (list[i] == null)
					{
						list[i] = new CodePrimitiveExpression(0);
					}
				}
				result = list;
			}
			return result;
		}
	}
}

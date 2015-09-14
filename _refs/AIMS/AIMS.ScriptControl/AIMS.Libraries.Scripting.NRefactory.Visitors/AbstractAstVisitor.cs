using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Diagnostics;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public abstract class AbstractAstVisitor : IAstVisitor
	{
		public virtual object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			Debug.Assert(addHandlerStatement != null);
			Debug.Assert(addHandlerStatement.EventExpression != null);
			Debug.Assert(addHandlerStatement.HandlerExpression != null);
			addHandlerStatement.EventExpression.AcceptVisitor(this, data);
			return addHandlerStatement.HandlerExpression.AcceptVisitor(this, data);
		}

		public virtual object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			Debug.Assert(addressOfExpression != null);
			Debug.Assert(addressOfExpression.Expression != null);
			return addressOfExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			Debug.Assert(anonymousMethodExpression != null);
			Debug.Assert(anonymousMethodExpression.Parameters != null);
			Debug.Assert(anonymousMethodExpression.Body != null);
			foreach (ParameterDeclarationExpression o in anonymousMethodExpression.Parameters)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return anonymousMethodExpression.Body.AcceptVisitor(this, data);
		}

		public virtual object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			Debug.Assert(arrayCreateExpression != null);
			Debug.Assert(arrayCreateExpression.CreateType != null);
			Debug.Assert(arrayCreateExpression.Arguments != null);
			Debug.Assert(arrayCreateExpression.ArrayInitializer != null);
			arrayCreateExpression.CreateType.AcceptVisitor(this, data);
			foreach (Expression o in arrayCreateExpression.Arguments)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return arrayCreateExpression.ArrayInitializer.AcceptVisitor(this, data);
		}

		public virtual object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			Debug.Assert(arrayInitializerExpression != null);
			Debug.Assert(arrayInitializerExpression.CreateExpressions != null);
			foreach (Expression o in arrayInitializerExpression.CreateExpressions)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			Debug.Assert(assignmentExpression != null);
			Debug.Assert(assignmentExpression.Left != null);
			Debug.Assert(assignmentExpression.Right != null);
			assignmentExpression.Left.AcceptVisitor(this, data);
			return assignmentExpression.Right.AcceptVisitor(this, data);
		}

		public virtual object VisitAttribute(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
		{
			Debug.Assert(attribute != null);
			Debug.Assert(attribute.PositionalArguments != null);
			Debug.Assert(attribute.NamedArguments != null);
			foreach (Expression o in attribute.PositionalArguments)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (NamedArgumentExpression o2 in attribute.NamedArguments)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			Debug.Assert(attributeSection != null);
			Debug.Assert(attributeSection.Attributes != null);
			foreach (AIMS.Libraries.Scripting.NRefactory.Ast.Attribute o in attributeSection.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			Debug.Assert(baseReferenceExpression != null);
			return null;
		}

		public virtual object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			Debug.Assert(binaryOperatorExpression != null);
			Debug.Assert(binaryOperatorExpression.Left != null);
			Debug.Assert(binaryOperatorExpression.Right != null);
			binaryOperatorExpression.Left.AcceptVisitor(this, data);
			return binaryOperatorExpression.Right.AcceptVisitor(this, data);
		}

		public virtual object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			Debug.Assert(blockStatement != null);
			return blockStatement.AcceptChildren(this, data);
		}

		public virtual object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			Debug.Assert(breakStatement != null);
			return null;
		}

		public virtual object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			Debug.Assert(caseLabel != null);
			Debug.Assert(caseLabel.Label != null);
			Debug.Assert(caseLabel.ToExpression != null);
			caseLabel.Label.AcceptVisitor(this, data);
			return caseLabel.ToExpression.AcceptVisitor(this, data);
		}

		public virtual object VisitCastExpression(CastExpression castExpression, object data)
		{
			Debug.Assert(castExpression != null);
			Debug.Assert(castExpression.CastTo != null);
			Debug.Assert(castExpression.Expression != null);
			castExpression.CastTo.AcceptVisitor(this, data);
			return castExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitCatchClause(CatchClause catchClause, object data)
		{
			Debug.Assert(catchClause != null);
			Debug.Assert(catchClause.TypeReference != null);
			Debug.Assert(catchClause.StatementBlock != null);
			Debug.Assert(catchClause.Condition != null);
			catchClause.TypeReference.AcceptVisitor(this, data);
			catchClause.StatementBlock.AcceptVisitor(this, data);
			return catchClause.Condition.AcceptVisitor(this, data);
		}

		public virtual object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			Debug.Assert(checkedExpression != null);
			Debug.Assert(checkedExpression.Expression != null);
			return checkedExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			Debug.Assert(checkedStatement != null);
			Debug.Assert(checkedStatement.Block != null);
			return checkedStatement.Block.AcceptVisitor(this, data);
		}

		public virtual object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			Debug.Assert(classReferenceExpression != null);
			return null;
		}

		public virtual object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			Debug.Assert(compilationUnit != null);
			return compilationUnit.AcceptChildren(this, data);
		}

		public virtual object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			Debug.Assert(conditionalExpression != null);
			Debug.Assert(conditionalExpression.Condition != null);
			Debug.Assert(conditionalExpression.TrueExpression != null);
			Debug.Assert(conditionalExpression.FalseExpression != null);
			conditionalExpression.Condition.AcceptVisitor(this, data);
			conditionalExpression.TrueExpression.AcceptVisitor(this, data);
			return conditionalExpression.FalseExpression.AcceptVisitor(this, data);
		}

		public virtual object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			Debug.Assert(constructorDeclaration != null);
			Debug.Assert(constructorDeclaration.Attributes != null);
			Debug.Assert(constructorDeclaration.Parameters != null);
			Debug.Assert(constructorDeclaration.ConstructorInitializer != null);
			Debug.Assert(constructorDeclaration.Body != null);
			foreach (AttributeSection o in constructorDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression o2 in constructorDeclaration.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			constructorDeclaration.ConstructorInitializer.AcceptVisitor(this, data);
			return constructorDeclaration.Body.AcceptVisitor(this, data);
		}

		public virtual object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			Debug.Assert(constructorInitializer != null);
			Debug.Assert(constructorInitializer.Arguments != null);
			foreach (Expression o in constructorInitializer.Arguments)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			Debug.Assert(continueStatement != null);
			return null;
		}

		public virtual object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			Debug.Assert(declareDeclaration != null);
			Debug.Assert(declareDeclaration.Attributes != null);
			Debug.Assert(declareDeclaration.Parameters != null);
			Debug.Assert(declareDeclaration.TypeReference != null);
			foreach (AttributeSection o in declareDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression o2 in declareDeclaration.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return declareDeclaration.TypeReference.AcceptVisitor(this, data);
		}

		public virtual object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			Debug.Assert(defaultValueExpression != null);
			Debug.Assert(defaultValueExpression.TypeReference != null);
			return defaultValueExpression.TypeReference.AcceptVisitor(this, data);
		}

		public virtual object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			Debug.Assert(delegateDeclaration != null);
			Debug.Assert(delegateDeclaration.Attributes != null);
			Debug.Assert(delegateDeclaration.ReturnType != null);
			Debug.Assert(delegateDeclaration.Parameters != null);
			Debug.Assert(delegateDeclaration.Templates != null);
			foreach (AttributeSection o in delegateDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			delegateDeclaration.ReturnType.AcceptVisitor(this, data);
			foreach (ParameterDeclarationExpression o2 in delegateDeclaration.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			foreach (TemplateDefinition o3 in delegateDeclaration.Templates)
			{
				Debug.Assert(o3 != null);
				o3.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			Debug.Assert(destructorDeclaration != null);
			Debug.Assert(destructorDeclaration.Attributes != null);
			Debug.Assert(destructorDeclaration.Body != null);
			foreach (AttributeSection o in destructorDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return destructorDeclaration.Body.AcceptVisitor(this, data);
		}

		public virtual object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			Debug.Assert(directionExpression != null);
			Debug.Assert(directionExpression.Expression != null);
			return directionExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			Debug.Assert(doLoopStatement != null);
			Debug.Assert(doLoopStatement.Condition != null);
			Debug.Assert(doLoopStatement.EmbeddedStatement != null);
			doLoopStatement.Condition.AcceptVisitor(this, data);
			return doLoopStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			Debug.Assert(elseIfSection != null);
			Debug.Assert(elseIfSection.Condition != null);
			Debug.Assert(elseIfSection.EmbeddedStatement != null);
			elseIfSection.Condition.AcceptVisitor(this, data);
			return elseIfSection.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			Debug.Assert(emptyStatement != null);
			return null;
		}

		public virtual object VisitEndStatement(EndStatement endStatement, object data)
		{
			Debug.Assert(endStatement != null);
			return null;
		}

		public virtual object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			Debug.Assert(eraseStatement != null);
			Debug.Assert(eraseStatement.Expressions != null);
			foreach (Expression o in eraseStatement.Expressions)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			Debug.Assert(errorStatement != null);
			Debug.Assert(errorStatement.Expression != null);
			return errorStatement.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			Debug.Assert(eventAddRegion != null);
			Debug.Assert(eventAddRegion.Attributes != null);
			Debug.Assert(eventAddRegion.Block != null);
			Debug.Assert(eventAddRegion.Parameters != null);
			foreach (AttributeSection o in eventAddRegion.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			eventAddRegion.Block.AcceptVisitor(this, data);
			foreach (ParameterDeclarationExpression o2 in eventAddRegion.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			Debug.Assert(eventDeclaration != null);
			Debug.Assert(eventDeclaration.Attributes != null);
			Debug.Assert(eventDeclaration.Parameters != null);
			Debug.Assert(eventDeclaration.TypeReference != null);
			Debug.Assert(eventDeclaration.InterfaceImplementations != null);
			Debug.Assert(eventDeclaration.AddRegion != null);
			Debug.Assert(eventDeclaration.RemoveRegion != null);
			Debug.Assert(eventDeclaration.RaiseRegion != null);
			Debug.Assert(eventDeclaration.Initializer != null);
			foreach (AttributeSection o in eventDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression o2 in eventDeclaration.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			eventDeclaration.TypeReference.AcceptVisitor(this, data);
			foreach (InterfaceImplementation o3 in eventDeclaration.InterfaceImplementations)
			{
				Debug.Assert(o3 != null);
				o3.AcceptVisitor(this, data);
			}
			eventDeclaration.AddRegion.AcceptVisitor(this, data);
			eventDeclaration.RemoveRegion.AcceptVisitor(this, data);
			eventDeclaration.RaiseRegion.AcceptVisitor(this, data);
			return eventDeclaration.Initializer.AcceptVisitor(this, data);
		}

		public virtual object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			Debug.Assert(eventRaiseRegion != null);
			Debug.Assert(eventRaiseRegion.Attributes != null);
			Debug.Assert(eventRaiseRegion.Block != null);
			Debug.Assert(eventRaiseRegion.Parameters != null);
			foreach (AttributeSection o in eventRaiseRegion.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			eventRaiseRegion.Block.AcceptVisitor(this, data);
			foreach (ParameterDeclarationExpression o2 in eventRaiseRegion.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			Debug.Assert(eventRemoveRegion != null);
			Debug.Assert(eventRemoveRegion.Attributes != null);
			Debug.Assert(eventRemoveRegion.Block != null);
			Debug.Assert(eventRemoveRegion.Parameters != null);
			foreach (AttributeSection o in eventRemoveRegion.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			eventRemoveRegion.Block.AcceptVisitor(this, data);
			foreach (ParameterDeclarationExpression o2 in eventRemoveRegion.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			Debug.Assert(exitStatement != null);
			return null;
		}

		public virtual object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			Debug.Assert(expressionStatement != null);
			Debug.Assert(expressionStatement.Expression != null);
			return expressionStatement.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			Debug.Assert(fieldDeclaration != null);
			Debug.Assert(fieldDeclaration.Attributes != null);
			Debug.Assert(fieldDeclaration.TypeReference != null);
			Debug.Assert(fieldDeclaration.Fields != null);
			foreach (AttributeSection o in fieldDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			fieldDeclaration.TypeReference.AcceptVisitor(this, data);
			foreach (VariableDeclaration o2 in fieldDeclaration.Fields)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			Debug.Assert(fieldReferenceExpression != null);
			Debug.Assert(fieldReferenceExpression.TargetObject != null);
			return fieldReferenceExpression.TargetObject.AcceptVisitor(this, data);
		}

		public virtual object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			Debug.Assert(fixedStatement != null);
			Debug.Assert(fixedStatement.TypeReference != null);
			Debug.Assert(fixedStatement.PointerDeclarators != null);
			Debug.Assert(fixedStatement.EmbeddedStatement != null);
			fixedStatement.TypeReference.AcceptVisitor(this, data);
			foreach (VariableDeclaration o in fixedStatement.PointerDeclarators)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return fixedStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			Debug.Assert(foreachStatement != null);
			Debug.Assert(foreachStatement.TypeReference != null);
			Debug.Assert(foreachStatement.Expression != null);
			Debug.Assert(foreachStatement.NextExpression != null);
			Debug.Assert(foreachStatement.EmbeddedStatement != null);
			foreachStatement.TypeReference.AcceptVisitor(this, data);
			foreachStatement.Expression.AcceptVisitor(this, data);
			foreachStatement.NextExpression.AcceptVisitor(this, data);
			return foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			Debug.Assert(forNextStatement != null);
			Debug.Assert(forNextStatement.Start != null);
			Debug.Assert(forNextStatement.End != null);
			Debug.Assert(forNextStatement.Step != null);
			Debug.Assert(forNextStatement.NextExpressions != null);
			Debug.Assert(forNextStatement.TypeReference != null);
			Debug.Assert(forNextStatement.EmbeddedStatement != null);
			forNextStatement.Start.AcceptVisitor(this, data);
			forNextStatement.End.AcceptVisitor(this, data);
			forNextStatement.Step.AcceptVisitor(this, data);
			foreach (Expression o in forNextStatement.NextExpressions)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			forNextStatement.TypeReference.AcceptVisitor(this, data);
			return forNextStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitForStatement(ForStatement forStatement, object data)
		{
			Debug.Assert(forStatement != null);
			Debug.Assert(forStatement.Initializers != null);
			Debug.Assert(forStatement.Condition != null);
			Debug.Assert(forStatement.Iterator != null);
			Debug.Assert(forStatement.EmbeddedStatement != null);
			foreach (Statement o in forStatement.Initializers)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			forStatement.Condition.AcceptVisitor(this, data);
			foreach (Statement o in forStatement.Iterator)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return forStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			Debug.Assert(gotoCaseStatement != null);
			Debug.Assert(gotoCaseStatement.Expression != null);
			return gotoCaseStatement.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			Debug.Assert(gotoStatement != null);
			return null;
		}

		public virtual object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			Debug.Assert(identifierExpression != null);
			return null;
		}

		public virtual object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			Debug.Assert(ifElseStatement != null);
			Debug.Assert(ifElseStatement.Condition != null);
			Debug.Assert(ifElseStatement.TrueStatement != null);
			Debug.Assert(ifElseStatement.FalseStatement != null);
			Debug.Assert(ifElseStatement.ElseIfSections != null);
			ifElseStatement.Condition.AcceptVisitor(this, data);
			foreach (Statement o in ifElseStatement.TrueStatement)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (Statement o in ifElseStatement.FalseStatement)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (ElseIfSection o2 in ifElseStatement.ElseIfSections)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			Debug.Assert(indexerDeclaration != null);
			Debug.Assert(indexerDeclaration.Attributes != null);
			Debug.Assert(indexerDeclaration.Parameters != null);
			Debug.Assert(indexerDeclaration.InterfaceImplementations != null);
			Debug.Assert(indexerDeclaration.TypeReference != null);
			Debug.Assert(indexerDeclaration.GetRegion != null);
			Debug.Assert(indexerDeclaration.SetRegion != null);
			foreach (AttributeSection o in indexerDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression o2 in indexerDeclaration.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			foreach (InterfaceImplementation o3 in indexerDeclaration.InterfaceImplementations)
			{
				Debug.Assert(o3 != null);
				o3.AcceptVisitor(this, data);
			}
			indexerDeclaration.TypeReference.AcceptVisitor(this, data);
			indexerDeclaration.GetRegion.AcceptVisitor(this, data);
			return indexerDeclaration.SetRegion.AcceptVisitor(this, data);
		}

		public virtual object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			Debug.Assert(indexerExpression != null);
			Debug.Assert(indexerExpression.TargetObject != null);
			Debug.Assert(indexerExpression.Indexes != null);
			indexerExpression.TargetObject.AcceptVisitor(this, data);
			foreach (Expression o in indexerExpression.Indexes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			Debug.Assert(innerClassTypeReference != null);
			Debug.Assert(innerClassTypeReference.GenericTypes != null);
			Debug.Assert(innerClassTypeReference.BaseType != null);
			foreach (TypeReference o in innerClassTypeReference.GenericTypes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return innerClassTypeReference.BaseType.AcceptVisitor(this, data);
		}

		public virtual object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			Debug.Assert(interfaceImplementation != null);
			Debug.Assert(interfaceImplementation.InterfaceType != null);
			return interfaceImplementation.InterfaceType.AcceptVisitor(this, data);
		}

		public virtual object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Debug.Assert(invocationExpression != null);
			Debug.Assert(invocationExpression.TargetObject != null);
			Debug.Assert(invocationExpression.Arguments != null);
			Debug.Assert(invocationExpression.TypeArguments != null);
			invocationExpression.TargetObject.AcceptVisitor(this, data);
			foreach (Expression o in invocationExpression.Arguments)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (TypeReference o2 in invocationExpression.TypeArguments)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			Debug.Assert(labelStatement != null);
			return null;
		}

		public virtual object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			Debug.Assert(localVariableDeclaration != null);
			Debug.Assert(localVariableDeclaration.TypeReference != null);
			Debug.Assert(localVariableDeclaration.Variables != null);
			localVariableDeclaration.TypeReference.AcceptVisitor(this, data);
			foreach (VariableDeclaration o in localVariableDeclaration.Variables)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitLockStatement(LockStatement lockStatement, object data)
		{
			Debug.Assert(lockStatement != null);
			Debug.Assert(lockStatement.LockExpression != null);
			Debug.Assert(lockStatement.EmbeddedStatement != null);
			lockStatement.LockExpression.AcceptVisitor(this, data);
			return lockStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			Debug.Assert(methodDeclaration != null);
			Debug.Assert(methodDeclaration.Attributes != null);
			Debug.Assert(methodDeclaration.Parameters != null);
			Debug.Assert(methodDeclaration.TypeReference != null);
			Debug.Assert(methodDeclaration.Body != null);
			Debug.Assert(methodDeclaration.InterfaceImplementations != null);
			Debug.Assert(methodDeclaration.Templates != null);
			foreach (AttributeSection o in methodDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression o2 in methodDeclaration.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			methodDeclaration.TypeReference.AcceptVisitor(this, data);
			methodDeclaration.Body.AcceptVisitor(this, data);
			foreach (InterfaceImplementation o3 in methodDeclaration.InterfaceImplementations)
			{
				Debug.Assert(o3 != null);
				o3.AcceptVisitor(this, data);
			}
			foreach (TemplateDefinition o4 in methodDeclaration.Templates)
			{
				Debug.Assert(o4 != null);
				o4.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			Debug.Assert(namedArgumentExpression != null);
			Debug.Assert(namedArgumentExpression.Expression != null);
			return namedArgumentExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			Debug.Assert(namespaceDeclaration != null);
			return namespaceDeclaration.AcceptChildren(this, data);
		}

		public virtual object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			Debug.Assert(objectCreateExpression != null);
			Debug.Assert(objectCreateExpression.CreateType != null);
			Debug.Assert(objectCreateExpression.Parameters != null);
			objectCreateExpression.CreateType.AcceptVisitor(this, data);
			foreach (Expression o in objectCreateExpression.Parameters)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			Debug.Assert(onErrorStatement != null);
			Debug.Assert(onErrorStatement.EmbeddedStatement != null);
			return onErrorStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			Debug.Assert(operatorDeclaration != null);
			Debug.Assert(operatorDeclaration.Attributes != null);
			Debug.Assert(operatorDeclaration.Parameters != null);
			Debug.Assert(operatorDeclaration.TypeReference != null);
			Debug.Assert(operatorDeclaration.Body != null);
			Debug.Assert(operatorDeclaration.InterfaceImplementations != null);
			Debug.Assert(operatorDeclaration.Templates != null);
			Debug.Assert(operatorDeclaration.ReturnTypeAttributes != null);
			foreach (AttributeSection o in operatorDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression o2 in operatorDeclaration.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			operatorDeclaration.TypeReference.AcceptVisitor(this, data);
			operatorDeclaration.Body.AcceptVisitor(this, data);
			foreach (InterfaceImplementation o3 in operatorDeclaration.InterfaceImplementations)
			{
				Debug.Assert(o3 != null);
				o3.AcceptVisitor(this, data);
			}
			foreach (TemplateDefinition o4 in operatorDeclaration.Templates)
			{
				Debug.Assert(o4 != null);
				o4.AcceptVisitor(this, data);
			}
			foreach (AttributeSection o in operatorDeclaration.ReturnTypeAttributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			Debug.Assert(optionDeclaration != null);
			return null;
		}

		public virtual object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			Debug.Assert(parameterDeclarationExpression != null);
			Debug.Assert(parameterDeclarationExpression.Attributes != null);
			Debug.Assert(parameterDeclarationExpression.TypeReference != null);
			Debug.Assert(parameterDeclarationExpression.DefaultValue != null);
			foreach (AttributeSection o in parameterDeclarationExpression.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			parameterDeclarationExpression.TypeReference.AcceptVisitor(this, data);
			return parameterDeclarationExpression.DefaultValue.AcceptVisitor(this, data);
		}

		public virtual object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			Debug.Assert(parenthesizedExpression != null);
			Debug.Assert(parenthesizedExpression.Expression != null);
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			Debug.Assert(pointerReferenceExpression != null);
			Debug.Assert(pointerReferenceExpression.TargetObject != null);
			return pointerReferenceExpression.TargetObject.AcceptVisitor(this, data);
		}

		public virtual object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			Debug.Assert(primitiveExpression != null);
			return null;
		}

		public virtual object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			Debug.Assert(propertyDeclaration != null);
			Debug.Assert(propertyDeclaration.Attributes != null);
			Debug.Assert(propertyDeclaration.Parameters != null);
			Debug.Assert(propertyDeclaration.InterfaceImplementations != null);
			Debug.Assert(propertyDeclaration.TypeReference != null);
			Debug.Assert(propertyDeclaration.GetRegion != null);
			Debug.Assert(propertyDeclaration.SetRegion != null);
			foreach (AttributeSection o in propertyDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression o2 in propertyDeclaration.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			foreach (InterfaceImplementation o3 in propertyDeclaration.InterfaceImplementations)
			{
				Debug.Assert(o3 != null);
				o3.AcceptVisitor(this, data);
			}
			propertyDeclaration.TypeReference.AcceptVisitor(this, data);
			propertyDeclaration.GetRegion.AcceptVisitor(this, data);
			return propertyDeclaration.SetRegion.AcceptVisitor(this, data);
		}

		public virtual object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			Debug.Assert(propertyGetRegion != null);
			Debug.Assert(propertyGetRegion.Attributes != null);
			Debug.Assert(propertyGetRegion.Block != null);
			foreach (AttributeSection o in propertyGetRegion.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return propertyGetRegion.Block.AcceptVisitor(this, data);
		}

		public virtual object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			Debug.Assert(propertySetRegion != null);
			Debug.Assert(propertySetRegion.Attributes != null);
			Debug.Assert(propertySetRegion.Block != null);
			Debug.Assert(propertySetRegion.Parameters != null);
			foreach (AttributeSection o in propertySetRegion.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			propertySetRegion.Block.AcceptVisitor(this, data);
			foreach (ParameterDeclarationExpression o2 in propertySetRegion.Parameters)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			Debug.Assert(raiseEventStatement != null);
			Debug.Assert(raiseEventStatement.Arguments != null);
			foreach (Expression o in raiseEventStatement.Arguments)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			Debug.Assert(reDimStatement != null);
			Debug.Assert(reDimStatement.ReDimClauses != null);
			foreach (InvocationExpression o in reDimStatement.ReDimClauses)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			Debug.Assert(removeHandlerStatement != null);
			Debug.Assert(removeHandlerStatement.EventExpression != null);
			Debug.Assert(removeHandlerStatement.HandlerExpression != null);
			removeHandlerStatement.EventExpression.AcceptVisitor(this, data);
			return removeHandlerStatement.HandlerExpression.AcceptVisitor(this, data);
		}

		public virtual object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			Debug.Assert(resumeStatement != null);
			return null;
		}

		public virtual object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			Debug.Assert(returnStatement != null);
			Debug.Assert(returnStatement.Expression != null);
			return returnStatement.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			Debug.Assert(sizeOfExpression != null);
			Debug.Assert(sizeOfExpression.TypeReference != null);
			return sizeOfExpression.TypeReference.AcceptVisitor(this, data);
		}

		public virtual object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			Debug.Assert(stackAllocExpression != null);
			Debug.Assert(stackAllocExpression.TypeReference != null);
			Debug.Assert(stackAllocExpression.Expression != null);
			stackAllocExpression.TypeReference.AcceptVisitor(this, data);
			return stackAllocExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitStopStatement(StopStatement stopStatement, object data)
		{
			Debug.Assert(stopStatement != null);
			return null;
		}

		public virtual object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			Debug.Assert(switchSection != null);
			Debug.Assert(switchSection.SwitchLabels != null);
			foreach (CaseLabel o in switchSection.SwitchLabels)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return switchSection.AcceptChildren(this, data);
		}

		public virtual object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			Debug.Assert(switchStatement != null);
			Debug.Assert(switchStatement.SwitchExpression != null);
			Debug.Assert(switchStatement.SwitchSections != null);
			switchStatement.SwitchExpression.AcceptVisitor(this, data);
			foreach (SwitchSection o in switchStatement.SwitchSections)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			Debug.Assert(templateDefinition != null);
			Debug.Assert(templateDefinition.Attributes != null);
			Debug.Assert(templateDefinition.Bases != null);
			foreach (AttributeSection o in templateDefinition.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (TypeReference o2 in templateDefinition.Bases)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			Debug.Assert(thisReferenceExpression != null);
			return null;
		}

		public virtual object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			Debug.Assert(throwStatement != null);
			Debug.Assert(throwStatement.Expression != null);
			return throwStatement.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			Debug.Assert(tryCatchStatement != null);
			Debug.Assert(tryCatchStatement.StatementBlock != null);
			Debug.Assert(tryCatchStatement.CatchClauses != null);
			Debug.Assert(tryCatchStatement.FinallyBlock != null);
			tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
			foreach (CatchClause o in tryCatchStatement.CatchClauses)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
		}

		public virtual object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			Debug.Assert(typeDeclaration != null);
			Debug.Assert(typeDeclaration.Attributes != null);
			Debug.Assert(typeDeclaration.BaseTypes != null);
			Debug.Assert(typeDeclaration.Templates != null);
			foreach (AttributeSection o in typeDeclaration.Attributes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			foreach (TypeReference o2 in typeDeclaration.BaseTypes)
			{
				Debug.Assert(o2 != null);
				o2.AcceptVisitor(this, data);
			}
			foreach (TemplateDefinition o3 in typeDeclaration.Templates)
			{
				Debug.Assert(o3 != null);
				o3.AcceptVisitor(this, data);
			}
			return typeDeclaration.AcceptChildren(this, data);
		}

		public virtual object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			Debug.Assert(typeOfExpression != null);
			Debug.Assert(typeOfExpression.TypeReference != null);
			return typeOfExpression.TypeReference.AcceptVisitor(this, data);
		}

		public virtual object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			Debug.Assert(typeOfIsExpression != null);
			Debug.Assert(typeOfIsExpression.Expression != null);
			Debug.Assert(typeOfIsExpression.TypeReference != null);
			typeOfIsExpression.Expression.AcceptVisitor(this, data);
			return typeOfIsExpression.TypeReference.AcceptVisitor(this, data);
		}

		public virtual object VisitTypeReference(TypeReference typeReference, object data)
		{
			Debug.Assert(typeReference != null);
			Debug.Assert(typeReference.GenericTypes != null);
			foreach (TypeReference o in typeReference.GenericTypes)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			Debug.Assert(typeReferenceExpression != null);
			Debug.Assert(typeReferenceExpression.TypeReference != null);
			return typeReferenceExpression.TypeReference.AcceptVisitor(this, data);
		}

		public virtual object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			Debug.Assert(unaryOperatorExpression != null);
			Debug.Assert(unaryOperatorExpression.Expression != null);
			return unaryOperatorExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			Debug.Assert(uncheckedExpression != null);
			Debug.Assert(uncheckedExpression.Expression != null);
			return uncheckedExpression.Expression.AcceptVisitor(this, data);
		}

		public virtual object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			Debug.Assert(uncheckedStatement != null);
			Debug.Assert(uncheckedStatement.Block != null);
			return uncheckedStatement.Block.AcceptVisitor(this, data);
		}

		public virtual object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			Debug.Assert(unsafeStatement != null);
			Debug.Assert(unsafeStatement.Block != null);
			return unsafeStatement.Block.AcceptVisitor(this, data);
		}

		public virtual object VisitUsing(Using @using, object data)
		{
			Debug.Assert(@using != null);
			Debug.Assert(@using.Alias != null);
			return @using.Alias.AcceptVisitor(this, data);
		}

		public virtual object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			Debug.Assert(usingDeclaration != null);
			Debug.Assert(usingDeclaration.Usings != null);
			foreach (Using o in usingDeclaration.Usings)
			{
				Debug.Assert(o != null);
				o.AcceptVisitor(this, data);
			}
			return null;
		}

		public virtual object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			Debug.Assert(usingStatement != null);
			Debug.Assert(usingStatement.ResourceAcquisition != null);
			Debug.Assert(usingStatement.EmbeddedStatement != null);
			usingStatement.ResourceAcquisition.AcceptVisitor(this, data);
			return usingStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}

		public virtual object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			Debug.Assert(variableDeclaration != null);
			Debug.Assert(variableDeclaration.Initializer != null);
			Debug.Assert(variableDeclaration.TypeReference != null);
			Debug.Assert(variableDeclaration.FixedArrayInitialization != null);
			variableDeclaration.Initializer.AcceptVisitor(this, data);
			variableDeclaration.TypeReference.AcceptVisitor(this, data);
			return variableDeclaration.FixedArrayInitialization.AcceptVisitor(this, data);
		}

		public virtual object VisitWithStatement(WithStatement withStatement, object data)
		{
			Debug.Assert(withStatement != null);
			Debug.Assert(withStatement.Expression != null);
			Debug.Assert(withStatement.Body != null);
			withStatement.Expression.AcceptVisitor(this, data);
			return withStatement.Body.AcceptVisitor(this, data);
		}

		public virtual object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			Debug.Assert(yieldStatement != null);
			Debug.Assert(yieldStatement.Statement != null);
			return yieldStatement.Statement.AcceptVisitor(this, data);
		}
	}
}

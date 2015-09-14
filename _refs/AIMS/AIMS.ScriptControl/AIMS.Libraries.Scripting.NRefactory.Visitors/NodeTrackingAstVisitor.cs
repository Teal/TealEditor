using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public abstract class NodeTrackingAstVisitor : AbstractAstVisitor
	{
		protected virtual void BeginVisit(INode node)
		{
		}

		protected virtual void EndVisit(INode node)
		{
		}

		public sealed override object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			this.BeginVisit(addHandlerStatement);
			object result = this.TrackedVisit(addHandlerStatement, data);
			this.EndVisit(addHandlerStatement);
			return result;
		}

		public sealed override object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			this.BeginVisit(addressOfExpression);
			object result = this.TrackedVisit(addressOfExpression, data);
			this.EndVisit(addressOfExpression);
			return result;
		}

		public sealed override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			this.BeginVisit(anonymousMethodExpression);
			object result = this.TrackedVisit(anonymousMethodExpression, data);
			this.EndVisit(anonymousMethodExpression);
			return result;
		}

		public sealed override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			this.BeginVisit(arrayCreateExpression);
			object result = this.TrackedVisit(arrayCreateExpression, data);
			this.EndVisit(arrayCreateExpression);
			return result;
		}

		public sealed override object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			this.BeginVisit(arrayInitializerExpression);
			object result = this.TrackedVisit(arrayInitializerExpression, data);
			this.EndVisit(arrayInitializerExpression);
			return result;
		}

		public sealed override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			this.BeginVisit(assignmentExpression);
			object result = this.TrackedVisit(assignmentExpression, data);
			this.EndVisit(assignmentExpression);
			return result;
		}

		public sealed override object VisitAttribute(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
		{
			this.BeginVisit(attribute);
			object result = this.TrackedVisit(attribute, data);
			this.EndVisit(attribute);
			return result;
		}

		public sealed override object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			this.BeginVisit(attributeSection);
			object result = this.TrackedVisit(attributeSection, data);
			this.EndVisit(attributeSection);
			return result;
		}

		public sealed override object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			this.BeginVisit(baseReferenceExpression);
			object result = this.TrackedVisit(baseReferenceExpression, data);
			this.EndVisit(baseReferenceExpression);
			return result;
		}

		public sealed override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			this.BeginVisit(binaryOperatorExpression);
			object result = this.TrackedVisit(binaryOperatorExpression, data);
			this.EndVisit(binaryOperatorExpression);
			return result;
		}

		public sealed override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			this.BeginVisit(blockStatement);
			object result = this.TrackedVisit(blockStatement, data);
			this.EndVisit(blockStatement);
			return result;
		}

		public sealed override object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			this.BeginVisit(breakStatement);
			object result = this.TrackedVisit(breakStatement, data);
			this.EndVisit(breakStatement);
			return result;
		}

		public sealed override object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			this.BeginVisit(caseLabel);
			object result = this.TrackedVisit(caseLabel, data);
			this.EndVisit(caseLabel);
			return result;
		}

		public sealed override object VisitCastExpression(CastExpression castExpression, object data)
		{
			this.BeginVisit(castExpression);
			object result = this.TrackedVisit(castExpression, data);
			this.EndVisit(castExpression);
			return result;
		}

		public sealed override object VisitCatchClause(CatchClause catchClause, object data)
		{
			this.BeginVisit(catchClause);
			object result = this.TrackedVisit(catchClause, data);
			this.EndVisit(catchClause);
			return result;
		}

		public sealed override object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			this.BeginVisit(checkedExpression);
			object result = this.TrackedVisit(checkedExpression, data);
			this.EndVisit(checkedExpression);
			return result;
		}

		public sealed override object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			this.BeginVisit(checkedStatement);
			object result = this.TrackedVisit(checkedStatement, data);
			this.EndVisit(checkedStatement);
			return result;
		}

		public sealed override object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			this.BeginVisit(classReferenceExpression);
			object result = this.TrackedVisit(classReferenceExpression, data);
			this.EndVisit(classReferenceExpression);
			return result;
		}

		public sealed override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			this.BeginVisit(compilationUnit);
			object result = this.TrackedVisit(compilationUnit, data);
			this.EndVisit(compilationUnit);
			return result;
		}

		public sealed override object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			this.BeginVisit(conditionalExpression);
			object result = this.TrackedVisit(conditionalExpression, data);
			this.EndVisit(conditionalExpression);
			return result;
		}

		public sealed override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			this.BeginVisit(constructorDeclaration);
			object result = this.TrackedVisit(constructorDeclaration, data);
			this.EndVisit(constructorDeclaration);
			return result;
		}

		public sealed override object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			this.BeginVisit(constructorInitializer);
			object result = this.TrackedVisit(constructorInitializer, data);
			this.EndVisit(constructorInitializer);
			return result;
		}

		public sealed override object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			this.BeginVisit(continueStatement);
			object result = this.TrackedVisit(continueStatement, data);
			this.EndVisit(continueStatement);
			return result;
		}

		public sealed override object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			this.BeginVisit(declareDeclaration);
			object result = this.TrackedVisit(declareDeclaration, data);
			this.EndVisit(declareDeclaration);
			return result;
		}

		public sealed override object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			this.BeginVisit(defaultValueExpression);
			object result = this.TrackedVisit(defaultValueExpression, data);
			this.EndVisit(defaultValueExpression);
			return result;
		}

		public sealed override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			this.BeginVisit(delegateDeclaration);
			object result = this.TrackedVisit(delegateDeclaration, data);
			this.EndVisit(delegateDeclaration);
			return result;
		}

		public sealed override object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			this.BeginVisit(destructorDeclaration);
			object result = this.TrackedVisit(destructorDeclaration, data);
			this.EndVisit(destructorDeclaration);
			return result;
		}

		public sealed override object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			this.BeginVisit(directionExpression);
			object result = this.TrackedVisit(directionExpression, data);
			this.EndVisit(directionExpression);
			return result;
		}

		public sealed override object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			this.BeginVisit(doLoopStatement);
			object result = this.TrackedVisit(doLoopStatement, data);
			this.EndVisit(doLoopStatement);
			return result;
		}

		public sealed override object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			this.BeginVisit(elseIfSection);
			object result = this.TrackedVisit(elseIfSection, data);
			this.EndVisit(elseIfSection);
			return result;
		}

		public sealed override object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			this.BeginVisit(emptyStatement);
			object result = this.TrackedVisit(emptyStatement, data);
			this.EndVisit(emptyStatement);
			return result;
		}

		public sealed override object VisitEndStatement(EndStatement endStatement, object data)
		{
			this.BeginVisit(endStatement);
			object result = this.TrackedVisit(endStatement, data);
			this.EndVisit(endStatement);
			return result;
		}

		public sealed override object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			this.BeginVisit(eraseStatement);
			object result = this.TrackedVisit(eraseStatement, data);
			this.EndVisit(eraseStatement);
			return result;
		}

		public sealed override object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			this.BeginVisit(errorStatement);
			object result = this.TrackedVisit(errorStatement, data);
			this.EndVisit(errorStatement);
			return result;
		}

		public sealed override object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			this.BeginVisit(eventAddRegion);
			object result = this.TrackedVisit(eventAddRegion, data);
			this.EndVisit(eventAddRegion);
			return result;
		}

		public sealed override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			this.BeginVisit(eventDeclaration);
			object result = this.TrackedVisit(eventDeclaration, data);
			this.EndVisit(eventDeclaration);
			return result;
		}

		public sealed override object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			this.BeginVisit(eventRaiseRegion);
			object result = this.TrackedVisit(eventRaiseRegion, data);
			this.EndVisit(eventRaiseRegion);
			return result;
		}

		public sealed override object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			this.BeginVisit(eventRemoveRegion);
			object result = this.TrackedVisit(eventRemoveRegion, data);
			this.EndVisit(eventRemoveRegion);
			return result;
		}

		public sealed override object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			this.BeginVisit(exitStatement);
			object result = this.TrackedVisit(exitStatement, data);
			this.EndVisit(exitStatement);
			return result;
		}

		public sealed override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			this.BeginVisit(expressionStatement);
			object result = this.TrackedVisit(expressionStatement, data);
			this.EndVisit(expressionStatement);
			return result;
		}

		public sealed override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			this.BeginVisit(fieldDeclaration);
			object result = this.TrackedVisit(fieldDeclaration, data);
			this.EndVisit(fieldDeclaration);
			return result;
		}

		public sealed override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			this.BeginVisit(fieldReferenceExpression);
			object result = this.TrackedVisit(fieldReferenceExpression, data);
			this.EndVisit(fieldReferenceExpression);
			return result;
		}

		public sealed override object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			this.BeginVisit(fixedStatement);
			object result = this.TrackedVisit(fixedStatement, data);
			this.EndVisit(fixedStatement);
			return result;
		}

		public sealed override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			this.BeginVisit(foreachStatement);
			object result = this.TrackedVisit(foreachStatement, data);
			this.EndVisit(foreachStatement);
			return result;
		}

		public sealed override object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			this.BeginVisit(forNextStatement);
			object result = this.TrackedVisit(forNextStatement, data);
			this.EndVisit(forNextStatement);
			return result;
		}

		public sealed override object VisitForStatement(ForStatement forStatement, object data)
		{
			this.BeginVisit(forStatement);
			object result = this.TrackedVisit(forStatement, data);
			this.EndVisit(forStatement);
			return result;
		}

		public sealed override object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			this.BeginVisit(gotoCaseStatement);
			object result = this.TrackedVisit(gotoCaseStatement, data);
			this.EndVisit(gotoCaseStatement);
			return result;
		}

		public sealed override object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			this.BeginVisit(gotoStatement);
			object result = this.TrackedVisit(gotoStatement, data);
			this.EndVisit(gotoStatement);
			return result;
		}

		public sealed override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			this.BeginVisit(identifierExpression);
			object result = this.TrackedVisit(identifierExpression, data);
			this.EndVisit(identifierExpression);
			return result;
		}

		public sealed override object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			this.BeginVisit(ifElseStatement);
			object result = this.TrackedVisit(ifElseStatement, data);
			this.EndVisit(ifElseStatement);
			return result;
		}

		public sealed override object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			this.BeginVisit(indexerDeclaration);
			object result = this.TrackedVisit(indexerDeclaration, data);
			this.EndVisit(indexerDeclaration);
			return result;
		}

		public sealed override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			this.BeginVisit(indexerExpression);
			object result = this.TrackedVisit(indexerExpression, data);
			this.EndVisit(indexerExpression);
			return result;
		}

		public sealed override object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			this.BeginVisit(innerClassTypeReference);
			object result = this.TrackedVisit(innerClassTypeReference, data);
			this.EndVisit(innerClassTypeReference);
			return result;
		}

		public sealed override object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			this.BeginVisit(interfaceImplementation);
			object result = this.TrackedVisit(interfaceImplementation, data);
			this.EndVisit(interfaceImplementation);
			return result;
		}

		public sealed override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			this.BeginVisit(invocationExpression);
			object result = this.TrackedVisit(invocationExpression, data);
			this.EndVisit(invocationExpression);
			return result;
		}

		public sealed override object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			this.BeginVisit(labelStatement);
			object result = this.TrackedVisit(labelStatement, data);
			this.EndVisit(labelStatement);
			return result;
		}

		public sealed override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			this.BeginVisit(localVariableDeclaration);
			object result = this.TrackedVisit(localVariableDeclaration, data);
			this.EndVisit(localVariableDeclaration);
			return result;
		}

		public sealed override object VisitLockStatement(LockStatement lockStatement, object data)
		{
			this.BeginVisit(lockStatement);
			object result = this.TrackedVisit(lockStatement, data);
			this.EndVisit(lockStatement);
			return result;
		}

		public sealed override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			this.BeginVisit(methodDeclaration);
			object result = this.TrackedVisit(methodDeclaration, data);
			this.EndVisit(methodDeclaration);
			return result;
		}

		public sealed override object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			this.BeginVisit(namedArgumentExpression);
			object result = this.TrackedVisit(namedArgumentExpression, data);
			this.EndVisit(namedArgumentExpression);
			return result;
		}

		public sealed override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			this.BeginVisit(namespaceDeclaration);
			object result = this.TrackedVisit(namespaceDeclaration, data);
			this.EndVisit(namespaceDeclaration);
			return result;
		}

		public sealed override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			this.BeginVisit(objectCreateExpression);
			object result = this.TrackedVisit(objectCreateExpression, data);
			this.EndVisit(objectCreateExpression);
			return result;
		}

		public sealed override object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			this.BeginVisit(onErrorStatement);
			object result = this.TrackedVisit(onErrorStatement, data);
			this.EndVisit(onErrorStatement);
			return result;
		}

		public sealed override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			this.BeginVisit(operatorDeclaration);
			object result = this.TrackedVisit(operatorDeclaration, data);
			this.EndVisit(operatorDeclaration);
			return result;
		}

		public sealed override object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			this.BeginVisit(optionDeclaration);
			object result = this.TrackedVisit(optionDeclaration, data);
			this.EndVisit(optionDeclaration);
			return result;
		}

		public sealed override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			this.BeginVisit(parameterDeclarationExpression);
			object result = this.TrackedVisit(parameterDeclarationExpression, data);
			this.EndVisit(parameterDeclarationExpression);
			return result;
		}

		public sealed override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			this.BeginVisit(parenthesizedExpression);
			object result = this.TrackedVisit(parenthesizedExpression, data);
			this.EndVisit(parenthesizedExpression);
			return result;
		}

		public sealed override object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			this.BeginVisit(pointerReferenceExpression);
			object result = this.TrackedVisit(pointerReferenceExpression, data);
			this.EndVisit(pointerReferenceExpression);
			return result;
		}

		public sealed override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			this.BeginVisit(primitiveExpression);
			object result = this.TrackedVisit(primitiveExpression, data);
			this.EndVisit(primitiveExpression);
			return result;
		}

		public sealed override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			this.BeginVisit(propertyDeclaration);
			object result = this.TrackedVisit(propertyDeclaration, data);
			this.EndVisit(propertyDeclaration);
			return result;
		}

		public sealed override object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			this.BeginVisit(propertyGetRegion);
			object result = this.TrackedVisit(propertyGetRegion, data);
			this.EndVisit(propertyGetRegion);
			return result;
		}

		public sealed override object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			this.BeginVisit(propertySetRegion);
			object result = this.TrackedVisit(propertySetRegion, data);
			this.EndVisit(propertySetRegion);
			return result;
		}

		public sealed override object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			this.BeginVisit(raiseEventStatement);
			object result = this.TrackedVisit(raiseEventStatement, data);
			this.EndVisit(raiseEventStatement);
			return result;
		}

		public sealed override object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			this.BeginVisit(reDimStatement);
			object result = this.TrackedVisit(reDimStatement, data);
			this.EndVisit(reDimStatement);
			return result;
		}

		public sealed override object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			this.BeginVisit(removeHandlerStatement);
			object result = this.TrackedVisit(removeHandlerStatement, data);
			this.EndVisit(removeHandlerStatement);
			return result;
		}

		public sealed override object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			this.BeginVisit(resumeStatement);
			object result = this.TrackedVisit(resumeStatement, data);
			this.EndVisit(resumeStatement);
			return result;
		}

		public sealed override object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			this.BeginVisit(returnStatement);
			object result = this.TrackedVisit(returnStatement, data);
			this.EndVisit(returnStatement);
			return result;
		}

		public sealed override object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			this.BeginVisit(sizeOfExpression);
			object result = this.TrackedVisit(sizeOfExpression, data);
			this.EndVisit(sizeOfExpression);
			return result;
		}

		public sealed override object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			this.BeginVisit(stackAllocExpression);
			object result = this.TrackedVisit(stackAllocExpression, data);
			this.EndVisit(stackAllocExpression);
			return result;
		}

		public sealed override object VisitStopStatement(StopStatement stopStatement, object data)
		{
			this.BeginVisit(stopStatement);
			object result = this.TrackedVisit(stopStatement, data);
			this.EndVisit(stopStatement);
			return result;
		}

		public sealed override object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			this.BeginVisit(switchSection);
			object result = this.TrackedVisit(switchSection, data);
			this.EndVisit(switchSection);
			return result;
		}

		public sealed override object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			this.BeginVisit(switchStatement);
			object result = this.TrackedVisit(switchStatement, data);
			this.EndVisit(switchStatement);
			return result;
		}

		public sealed override object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			this.BeginVisit(templateDefinition);
			object result = this.TrackedVisit(templateDefinition, data);
			this.EndVisit(templateDefinition);
			return result;
		}

		public sealed override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			this.BeginVisit(thisReferenceExpression);
			object result = this.TrackedVisit(thisReferenceExpression, data);
			this.EndVisit(thisReferenceExpression);
			return result;
		}

		public sealed override object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			this.BeginVisit(throwStatement);
			object result = this.TrackedVisit(throwStatement, data);
			this.EndVisit(throwStatement);
			return result;
		}

		public sealed override object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			this.BeginVisit(tryCatchStatement);
			object result = this.TrackedVisit(tryCatchStatement, data);
			this.EndVisit(tryCatchStatement);
			return result;
		}

		public sealed override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			this.BeginVisit(typeDeclaration);
			object result = this.TrackedVisit(typeDeclaration, data);
			this.EndVisit(typeDeclaration);
			return result;
		}

		public sealed override object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			this.BeginVisit(typeOfExpression);
			object result = this.TrackedVisit(typeOfExpression, data);
			this.EndVisit(typeOfExpression);
			return result;
		}

		public sealed override object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			this.BeginVisit(typeOfIsExpression);
			object result = this.TrackedVisit(typeOfIsExpression, data);
			this.EndVisit(typeOfIsExpression);
			return result;
		}

		public sealed override object VisitTypeReference(TypeReference typeReference, object data)
		{
			this.BeginVisit(typeReference);
			object result = this.TrackedVisit(typeReference, data);
			this.EndVisit(typeReference);
			return result;
		}

		public sealed override object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			this.BeginVisit(typeReferenceExpression);
			object result = this.TrackedVisit(typeReferenceExpression, data);
			this.EndVisit(typeReferenceExpression);
			return result;
		}

		public sealed override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			this.BeginVisit(unaryOperatorExpression);
			object result = this.TrackedVisit(unaryOperatorExpression, data);
			this.EndVisit(unaryOperatorExpression);
			return result;
		}

		public sealed override object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			this.BeginVisit(uncheckedExpression);
			object result = this.TrackedVisit(uncheckedExpression, data);
			this.EndVisit(uncheckedExpression);
			return result;
		}

		public sealed override object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			this.BeginVisit(uncheckedStatement);
			object result = this.TrackedVisit(uncheckedStatement, data);
			this.EndVisit(uncheckedStatement);
			return result;
		}

		public sealed override object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			this.BeginVisit(unsafeStatement);
			object result = this.TrackedVisit(unsafeStatement, data);
			this.EndVisit(unsafeStatement);
			return result;
		}

		public sealed override object VisitUsing(Using @using, object data)
		{
			this.BeginVisit(@using);
			object result = this.TrackedVisit(@using, data);
			this.EndVisit(@using);
			return result;
		}

		public sealed override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			this.BeginVisit(usingDeclaration);
			object result = this.TrackedVisit(usingDeclaration, data);
			this.EndVisit(usingDeclaration);
			return result;
		}

		public sealed override object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			this.BeginVisit(usingStatement);
			object result = this.TrackedVisit(usingStatement, data);
			this.EndVisit(usingStatement);
			return result;
		}

		public sealed override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			this.BeginVisit(variableDeclaration);
			object result = this.TrackedVisit(variableDeclaration, data);
			this.EndVisit(variableDeclaration);
			return result;
		}

		public sealed override object VisitWithStatement(WithStatement withStatement, object data)
		{
			this.BeginVisit(withStatement);
			object result = this.TrackedVisit(withStatement, data);
			this.EndVisit(withStatement);
			return result;
		}

		public sealed override object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			this.BeginVisit(yieldStatement);
			object result = this.TrackedVisit(yieldStatement, data);
			this.EndVisit(yieldStatement);
			return result;
		}

		public virtual object TrackedVisit(AddHandlerStatement addHandlerStatement, object data)
		{
			return base.VisitAddHandlerStatement(addHandlerStatement, data);
		}

		public virtual object TrackedVisit(AddressOfExpression addressOfExpression, object data)
		{
			return base.VisitAddressOfExpression(addressOfExpression, data);
		}

		public virtual object TrackedVisit(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			return base.VisitAnonymousMethodExpression(anonymousMethodExpression, data);
		}

		public virtual object TrackedVisit(ArrayCreateExpression arrayCreateExpression, object data)
		{
			return base.VisitArrayCreateExpression(arrayCreateExpression, data);
		}

		public virtual object TrackedVisit(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			return base.VisitArrayInitializerExpression(arrayInitializerExpression, data);
		}

		public virtual object TrackedVisit(AssignmentExpression assignmentExpression, object data)
		{
			return base.VisitAssignmentExpression(assignmentExpression, data);
		}

		public virtual object TrackedVisit(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
		{
			return base.VisitAttribute(attribute, data);
		}

		public virtual object TrackedVisit(AttributeSection attributeSection, object data)
		{
			return base.VisitAttributeSection(attributeSection, data);
		}

		public virtual object TrackedVisit(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return base.VisitBaseReferenceExpression(baseReferenceExpression, data);
		}

		public virtual object TrackedVisit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			return base.VisitBinaryOperatorExpression(binaryOperatorExpression, data);
		}

		public virtual object TrackedVisit(BlockStatement blockStatement, object data)
		{
			return base.VisitBlockStatement(blockStatement, data);
		}

		public virtual object TrackedVisit(BreakStatement breakStatement, object data)
		{
			return base.VisitBreakStatement(breakStatement, data);
		}

		public virtual object TrackedVisit(CaseLabel caseLabel, object data)
		{
			return base.VisitCaseLabel(caseLabel, data);
		}

		public virtual object TrackedVisit(CastExpression castExpression, object data)
		{
			return base.VisitCastExpression(castExpression, data);
		}

		public virtual object TrackedVisit(CatchClause catchClause, object data)
		{
			return base.VisitCatchClause(catchClause, data);
		}

		public virtual object TrackedVisit(CheckedExpression checkedExpression, object data)
		{
			return base.VisitCheckedExpression(checkedExpression, data);
		}

		public virtual object TrackedVisit(CheckedStatement checkedStatement, object data)
		{
			return base.VisitCheckedStatement(checkedStatement, data);
		}

		public virtual object TrackedVisit(ClassReferenceExpression classReferenceExpression, object data)
		{
			return base.VisitClassReferenceExpression(classReferenceExpression, data);
		}

		public virtual object TrackedVisit(CompilationUnit compilationUnit, object data)
		{
			return base.VisitCompilationUnit(compilationUnit, data);
		}

		public virtual object TrackedVisit(ConditionalExpression conditionalExpression, object data)
		{
			return base.VisitConditionalExpression(conditionalExpression, data);
		}

		public virtual object TrackedVisit(ConstructorDeclaration constructorDeclaration, object data)
		{
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}

		public virtual object TrackedVisit(ConstructorInitializer constructorInitializer, object data)
		{
			return base.VisitConstructorInitializer(constructorInitializer, data);
		}

		public virtual object TrackedVisit(ContinueStatement continueStatement, object data)
		{
			return base.VisitContinueStatement(continueStatement, data);
		}

		public virtual object TrackedVisit(DeclareDeclaration declareDeclaration, object data)
		{
			return base.VisitDeclareDeclaration(declareDeclaration, data);
		}

		public virtual object TrackedVisit(DefaultValueExpression defaultValueExpression, object data)
		{
			return base.VisitDefaultValueExpression(defaultValueExpression, data);
		}

		public virtual object TrackedVisit(DelegateDeclaration delegateDeclaration, object data)
		{
			return base.VisitDelegateDeclaration(delegateDeclaration, data);
		}

		public virtual object TrackedVisit(DestructorDeclaration destructorDeclaration, object data)
		{
			return base.VisitDestructorDeclaration(destructorDeclaration, data);
		}

		public virtual object TrackedVisit(DirectionExpression directionExpression, object data)
		{
			return base.VisitDirectionExpression(directionExpression, data);
		}

		public virtual object TrackedVisit(DoLoopStatement doLoopStatement, object data)
		{
			return base.VisitDoLoopStatement(doLoopStatement, data);
		}

		public virtual object TrackedVisit(ElseIfSection elseIfSection, object data)
		{
			return base.VisitElseIfSection(elseIfSection, data);
		}

		public virtual object TrackedVisit(EmptyStatement emptyStatement, object data)
		{
			return base.VisitEmptyStatement(emptyStatement, data);
		}

		public virtual object TrackedVisit(EndStatement endStatement, object data)
		{
			return base.VisitEndStatement(endStatement, data);
		}

		public virtual object TrackedVisit(EraseStatement eraseStatement, object data)
		{
			return base.VisitEraseStatement(eraseStatement, data);
		}

		public virtual object TrackedVisit(ErrorStatement errorStatement, object data)
		{
			return base.VisitErrorStatement(errorStatement, data);
		}

		public virtual object TrackedVisit(EventAddRegion eventAddRegion, object data)
		{
			return base.VisitEventAddRegion(eventAddRegion, data);
		}

		public virtual object TrackedVisit(EventDeclaration eventDeclaration, object data)
		{
			return base.VisitEventDeclaration(eventDeclaration, data);
		}

		public virtual object TrackedVisit(EventRaiseRegion eventRaiseRegion, object data)
		{
			return base.VisitEventRaiseRegion(eventRaiseRegion, data);
		}

		public virtual object TrackedVisit(EventRemoveRegion eventRemoveRegion, object data)
		{
			return base.VisitEventRemoveRegion(eventRemoveRegion, data);
		}

		public virtual object TrackedVisit(ExitStatement exitStatement, object data)
		{
			return base.VisitExitStatement(exitStatement, data);
		}

		public virtual object TrackedVisit(ExpressionStatement expressionStatement, object data)
		{
			return base.VisitExpressionStatement(expressionStatement, data);
		}

		public virtual object TrackedVisit(FieldDeclaration fieldDeclaration, object data)
		{
			return base.VisitFieldDeclaration(fieldDeclaration, data);
		}

		public virtual object TrackedVisit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			return base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		public virtual object TrackedVisit(FixedStatement fixedStatement, object data)
		{
			return base.VisitFixedStatement(fixedStatement, data);
		}

		public virtual object TrackedVisit(ForeachStatement foreachStatement, object data)
		{
			return base.VisitForeachStatement(foreachStatement, data);
		}

		public virtual object TrackedVisit(ForNextStatement forNextStatement, object data)
		{
			return base.VisitForNextStatement(forNextStatement, data);
		}

		public virtual object TrackedVisit(ForStatement forStatement, object data)
		{
			return base.VisitForStatement(forStatement, data);
		}

		public virtual object TrackedVisit(GotoCaseStatement gotoCaseStatement, object data)
		{
			return base.VisitGotoCaseStatement(gotoCaseStatement, data);
		}

		public virtual object TrackedVisit(GotoStatement gotoStatement, object data)
		{
			return base.VisitGotoStatement(gotoStatement, data);
		}

		public virtual object TrackedVisit(IdentifierExpression identifierExpression, object data)
		{
			return base.VisitIdentifierExpression(identifierExpression, data);
		}

		public virtual object TrackedVisit(IfElseStatement ifElseStatement, object data)
		{
			return base.VisitIfElseStatement(ifElseStatement, data);
		}

		public virtual object TrackedVisit(IndexerDeclaration indexerDeclaration, object data)
		{
			return base.VisitIndexerDeclaration(indexerDeclaration, data);
		}

		public virtual object TrackedVisit(IndexerExpression indexerExpression, object data)
		{
			return base.VisitIndexerExpression(indexerExpression, data);
		}

		public virtual object TrackedVisit(InnerClassTypeReference innerClassTypeReference, object data)
		{
			return base.VisitInnerClassTypeReference(innerClassTypeReference, data);
		}

		public virtual object TrackedVisit(InterfaceImplementation interfaceImplementation, object data)
		{
			return base.VisitInterfaceImplementation(interfaceImplementation, data);
		}

		public virtual object TrackedVisit(InvocationExpression invocationExpression, object data)
		{
			return base.VisitInvocationExpression(invocationExpression, data);
		}

		public virtual object TrackedVisit(LabelStatement labelStatement, object data)
		{
			return base.VisitLabelStatement(labelStatement, data);
		}

		public virtual object TrackedVisit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}

		public virtual object TrackedVisit(LockStatement lockStatement, object data)
		{
			return base.VisitLockStatement(lockStatement, data);
		}

		public virtual object TrackedVisit(MethodDeclaration methodDeclaration, object data)
		{
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}

		public virtual object TrackedVisit(NamedArgumentExpression namedArgumentExpression, object data)
		{
			return base.VisitNamedArgumentExpression(namedArgumentExpression, data);
		}

		public virtual object TrackedVisit(NamespaceDeclaration namespaceDeclaration, object data)
		{
			return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		public virtual object TrackedVisit(ObjectCreateExpression objectCreateExpression, object data)
		{
			return base.VisitObjectCreateExpression(objectCreateExpression, data);
		}

		public virtual object TrackedVisit(OnErrorStatement onErrorStatement, object data)
		{
			return base.VisitOnErrorStatement(onErrorStatement, data);
		}

		public virtual object TrackedVisit(OperatorDeclaration operatorDeclaration, object data)
		{
			return base.VisitOperatorDeclaration(operatorDeclaration, data);
		}

		public virtual object TrackedVisit(OptionDeclaration optionDeclaration, object data)
		{
			return base.VisitOptionDeclaration(optionDeclaration, data);
		}

		public virtual object TrackedVisit(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
		}

		public virtual object TrackedVisit(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return base.VisitParenthesizedExpression(parenthesizedExpression, data);
		}

		public virtual object TrackedVisit(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			return base.VisitPointerReferenceExpression(pointerReferenceExpression, data);
		}

		public virtual object TrackedVisit(PrimitiveExpression primitiveExpression, object data)
		{
			return base.VisitPrimitiveExpression(primitiveExpression, data);
		}

		public virtual object TrackedVisit(PropertyDeclaration propertyDeclaration, object data)
		{
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}

		public virtual object TrackedVisit(PropertyGetRegion propertyGetRegion, object data)
		{
			return base.VisitPropertyGetRegion(propertyGetRegion, data);
		}

		public virtual object TrackedVisit(PropertySetRegion propertySetRegion, object data)
		{
			return base.VisitPropertySetRegion(propertySetRegion, data);
		}

		public virtual object TrackedVisit(RaiseEventStatement raiseEventStatement, object data)
		{
			return base.VisitRaiseEventStatement(raiseEventStatement, data);
		}

		public virtual object TrackedVisit(ReDimStatement reDimStatement, object data)
		{
			return base.VisitReDimStatement(reDimStatement, data);
		}

		public virtual object TrackedVisit(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			return base.VisitRemoveHandlerStatement(removeHandlerStatement, data);
		}

		public virtual object TrackedVisit(ResumeStatement resumeStatement, object data)
		{
			return base.VisitResumeStatement(resumeStatement, data);
		}

		public virtual object TrackedVisit(ReturnStatement returnStatement, object data)
		{
			return base.VisitReturnStatement(returnStatement, data);
		}

		public virtual object TrackedVisit(SizeOfExpression sizeOfExpression, object data)
		{
			return base.VisitSizeOfExpression(sizeOfExpression, data);
		}

		public virtual object TrackedVisit(StackAllocExpression stackAllocExpression, object data)
		{
			return base.VisitStackAllocExpression(stackAllocExpression, data);
		}

		public virtual object TrackedVisit(StopStatement stopStatement, object data)
		{
			return base.VisitStopStatement(stopStatement, data);
		}

		public virtual object TrackedVisit(SwitchSection switchSection, object data)
		{
			return base.VisitSwitchSection(switchSection, data);
		}

		public virtual object TrackedVisit(SwitchStatement switchStatement, object data)
		{
			return base.VisitSwitchStatement(switchStatement, data);
		}

		public virtual object TrackedVisit(TemplateDefinition templateDefinition, object data)
		{
			return base.VisitTemplateDefinition(templateDefinition, data);
		}

		public virtual object TrackedVisit(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return base.VisitThisReferenceExpression(thisReferenceExpression, data);
		}

		public virtual object TrackedVisit(ThrowStatement throwStatement, object data)
		{
			return base.VisitThrowStatement(throwStatement, data);
		}

		public virtual object TrackedVisit(TryCatchStatement tryCatchStatement, object data)
		{
			return base.VisitTryCatchStatement(tryCatchStatement, data);
		}

		public virtual object TrackedVisit(TypeDeclaration typeDeclaration, object data)
		{
			return base.VisitTypeDeclaration(typeDeclaration, data);
		}

		public virtual object TrackedVisit(TypeOfExpression typeOfExpression, object data)
		{
			return base.VisitTypeOfExpression(typeOfExpression, data);
		}

		public virtual object TrackedVisit(TypeOfIsExpression typeOfIsExpression, object data)
		{
			return base.VisitTypeOfIsExpression(typeOfIsExpression, data);
		}

		public virtual object TrackedVisit(TypeReference typeReference, object data)
		{
			return base.VisitTypeReference(typeReference, data);
		}

		public virtual object TrackedVisit(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return base.VisitTypeReferenceExpression(typeReferenceExpression, data);
		}

		public virtual object TrackedVisit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			return base.VisitUnaryOperatorExpression(unaryOperatorExpression, data);
		}

		public virtual object TrackedVisit(UncheckedExpression uncheckedExpression, object data)
		{
			return base.VisitUncheckedExpression(uncheckedExpression, data);
		}

		public virtual object TrackedVisit(UncheckedStatement uncheckedStatement, object data)
		{
			return base.VisitUncheckedStatement(uncheckedStatement, data);
		}

		public virtual object TrackedVisit(UnsafeStatement unsafeStatement, object data)
		{
			return base.VisitUnsafeStatement(unsafeStatement, data);
		}

		public virtual object TrackedVisit(Using @using, object data)
		{
			return base.VisitUsing(@using, data);
		}

		public virtual object TrackedVisit(UsingDeclaration usingDeclaration, object data)
		{
			return base.VisitUsingDeclaration(usingDeclaration, data);
		}

		public virtual object TrackedVisit(UsingStatement usingStatement, object data)
		{
			return base.VisitUsingStatement(usingStatement, data);
		}

		public virtual object TrackedVisit(VariableDeclaration variableDeclaration, object data)
		{
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}

		public virtual object TrackedVisit(WithStatement withStatement, object data)
		{
			return base.VisitWithStatement(withStatement, data);
		}

		public virtual object TrackedVisit(YieldStatement yieldStatement, object data)
		{
			return base.VisitYieldStatement(yieldStatement, data);
		}
	}
}

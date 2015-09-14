using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public abstract class AbstractAstTransformer : IAstVisitor
	{
		private Stack<INode> nodeStack = new Stack<INode>();

		public void ReplaceCurrentNode(INode newNode)
		{
			this.nodeStack.Pop();
			this.nodeStack.Push(newNode);
		}

		public void RemoveCurrentNode()
		{
			this.nodeStack.Pop();
			this.nodeStack.Push(null);
		}

		public virtual object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			Debug.Assert(addHandlerStatement != null);
			Debug.Assert(addHandlerStatement.EventExpression != null);
			Debug.Assert(addHandlerStatement.HandlerExpression != null);
			this.nodeStack.Push(addHandlerStatement.EventExpression);
			addHandlerStatement.EventExpression.AcceptVisitor(this, data);
			addHandlerStatement.EventExpression = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(addHandlerStatement.HandlerExpression);
			addHandlerStatement.HandlerExpression.AcceptVisitor(this, data);
			addHandlerStatement.HandlerExpression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			Debug.Assert(addressOfExpression != null);
			Debug.Assert(addressOfExpression.Expression != null);
			this.nodeStack.Push(addressOfExpression.Expression);
			addressOfExpression.Expression.AcceptVisitor(this, data);
			addressOfExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			Debug.Assert(anonymousMethodExpression != null);
			Debug.Assert(anonymousMethodExpression.Parameters != null);
			Debug.Assert(anonymousMethodExpression.Body != null);
			for (int i = 0; i < anonymousMethodExpression.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o = anonymousMethodExpression.Parameters[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o == null)
				{
					anonymousMethodExpression.Parameters.RemoveAt(i--);
				}
				else
				{
					anonymousMethodExpression.Parameters[i] = o;
				}
			}
			this.nodeStack.Push(anonymousMethodExpression.Body);
			anonymousMethodExpression.Body.AcceptVisitor(this, data);
			anonymousMethodExpression.Body = (BlockStatement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			Debug.Assert(arrayCreateExpression != null);
			Debug.Assert(arrayCreateExpression.CreateType != null);
			Debug.Assert(arrayCreateExpression.Arguments != null);
			Debug.Assert(arrayCreateExpression.ArrayInitializer != null);
			this.nodeStack.Push(arrayCreateExpression.CreateType);
			arrayCreateExpression.CreateType.AcceptVisitor(this, data);
			arrayCreateExpression.CreateType = (TypeReference)this.nodeStack.Pop();
			for (int i = 0; i < arrayCreateExpression.Arguments.Count; i++)
			{
				Expression o = arrayCreateExpression.Arguments[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					arrayCreateExpression.Arguments.RemoveAt(i--);
				}
				else
				{
					arrayCreateExpression.Arguments[i] = o;
				}
			}
			this.nodeStack.Push(arrayCreateExpression.ArrayInitializer);
			arrayCreateExpression.ArrayInitializer.AcceptVisitor(this, data);
			arrayCreateExpression.ArrayInitializer = (ArrayInitializerExpression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			Debug.Assert(arrayInitializerExpression != null);
			Debug.Assert(arrayInitializerExpression.CreateExpressions != null);
			for (int i = 0; i < arrayInitializerExpression.CreateExpressions.Count; i++)
			{
				Expression o = arrayInitializerExpression.CreateExpressions[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					arrayInitializerExpression.CreateExpressions.RemoveAt(i--);
				}
				else
				{
					arrayInitializerExpression.CreateExpressions[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			Debug.Assert(assignmentExpression != null);
			Debug.Assert(assignmentExpression.Left != null);
			Debug.Assert(assignmentExpression.Right != null);
			this.nodeStack.Push(assignmentExpression.Left);
			assignmentExpression.Left.AcceptVisitor(this, data);
			assignmentExpression.Left = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(assignmentExpression.Right);
			assignmentExpression.Right.AcceptVisitor(this, data);
			assignmentExpression.Right = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitAttribute(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
		{
			Debug.Assert(attribute != null);
			Debug.Assert(attribute.PositionalArguments != null);
			Debug.Assert(attribute.NamedArguments != null);
			for (int i = 0; i < attribute.PositionalArguments.Count; i++)
			{
				Expression o = attribute.PositionalArguments[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					attribute.PositionalArguments.RemoveAt(i--);
				}
				else
				{
					attribute.PositionalArguments[i] = o;
				}
			}
			for (int i = 0; i < attribute.NamedArguments.Count; i++)
			{
				NamedArgumentExpression o2 = attribute.NamedArguments[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (NamedArgumentExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					attribute.NamedArguments.RemoveAt(i--);
				}
				else
				{
					attribute.NamedArguments[i] = o2;
				}
			}
			return null;
		}

		public virtual object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			Debug.Assert(attributeSection != null);
			Debug.Assert(attributeSection.Attributes != null);
			for (int i = 0; i < attributeSection.Attributes.Count; i++)
			{
				AIMS.Libraries.Scripting.NRefactory.Ast.Attribute o = attributeSection.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AIMS.Libraries.Scripting.NRefactory.Ast.Attribute)this.nodeStack.Pop();
				if (o == null)
				{
					attributeSection.Attributes.RemoveAt(i--);
				}
				else
				{
					attributeSection.Attributes[i] = o;
				}
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
			this.nodeStack.Push(binaryOperatorExpression.Left);
			binaryOperatorExpression.Left.AcceptVisitor(this, data);
			binaryOperatorExpression.Left = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(binaryOperatorExpression.Right);
			binaryOperatorExpression.Right.AcceptVisitor(this, data);
			binaryOperatorExpression.Right = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			Debug.Assert(blockStatement != null);
			for (int i = 0; i < blockStatement.Children.Count; i++)
			{
				INode o = blockStatement.Children[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = this.nodeStack.Pop();
				if (o == null)
				{
					blockStatement.Children.RemoveAt(i--);
				}
				else
				{
					blockStatement.Children[i] = o;
				}
			}
			return null;
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
			this.nodeStack.Push(caseLabel.Label);
			caseLabel.Label.AcceptVisitor(this, data);
			caseLabel.Label = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(caseLabel.ToExpression);
			caseLabel.ToExpression.AcceptVisitor(this, data);
			caseLabel.ToExpression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitCastExpression(CastExpression castExpression, object data)
		{
			Debug.Assert(castExpression != null);
			Debug.Assert(castExpression.CastTo != null);
			Debug.Assert(castExpression.Expression != null);
			this.nodeStack.Push(castExpression.CastTo);
			castExpression.CastTo.AcceptVisitor(this, data);
			castExpression.CastTo = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(castExpression.Expression);
			castExpression.Expression.AcceptVisitor(this, data);
			castExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitCatchClause(CatchClause catchClause, object data)
		{
			Debug.Assert(catchClause != null);
			Debug.Assert(catchClause.TypeReference != null);
			Debug.Assert(catchClause.StatementBlock != null);
			Debug.Assert(catchClause.Condition != null);
			this.nodeStack.Push(catchClause.TypeReference);
			catchClause.TypeReference.AcceptVisitor(this, data);
			catchClause.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(catchClause.StatementBlock);
			catchClause.StatementBlock.AcceptVisitor(this, data);
			catchClause.StatementBlock = (Statement)this.nodeStack.Pop();
			this.nodeStack.Push(catchClause.Condition);
			catchClause.Condition.AcceptVisitor(this, data);
			catchClause.Condition = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			Debug.Assert(checkedExpression != null);
			Debug.Assert(checkedExpression.Expression != null);
			this.nodeStack.Push(checkedExpression.Expression);
			checkedExpression.Expression.AcceptVisitor(this, data);
			checkedExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			Debug.Assert(checkedStatement != null);
			Debug.Assert(checkedStatement.Block != null);
			this.nodeStack.Push(checkedStatement.Block);
			checkedStatement.Block.AcceptVisitor(this, data);
			checkedStatement.Block = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			Debug.Assert(classReferenceExpression != null);
			return null;
		}

		public virtual object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			Debug.Assert(compilationUnit != null);
			for (int i = 0; i < compilationUnit.Children.Count; i++)
			{
				INode o = compilationUnit.Children[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = this.nodeStack.Pop();
				if (o == null)
				{
					compilationUnit.Children.RemoveAt(i--);
				}
				else
				{
					compilationUnit.Children[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			Debug.Assert(conditionalExpression != null);
			Debug.Assert(conditionalExpression.Condition != null);
			Debug.Assert(conditionalExpression.TrueExpression != null);
			Debug.Assert(conditionalExpression.FalseExpression != null);
			this.nodeStack.Push(conditionalExpression.Condition);
			conditionalExpression.Condition.AcceptVisitor(this, data);
			conditionalExpression.Condition = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(conditionalExpression.TrueExpression);
			conditionalExpression.TrueExpression.AcceptVisitor(this, data);
			conditionalExpression.TrueExpression = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(conditionalExpression.FalseExpression);
			conditionalExpression.FalseExpression.AcceptVisitor(this, data);
			conditionalExpression.FalseExpression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			Debug.Assert(constructorDeclaration != null);
			Debug.Assert(constructorDeclaration.Attributes != null);
			Debug.Assert(constructorDeclaration.Parameters != null);
			Debug.Assert(constructorDeclaration.ConstructorInitializer != null);
			Debug.Assert(constructorDeclaration.Body != null);
			for (int i = 0; i < constructorDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = constructorDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					constructorDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					constructorDeclaration.Attributes[i] = o;
				}
			}
			for (int i = 0; i < constructorDeclaration.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = constructorDeclaration.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					constructorDeclaration.Parameters.RemoveAt(i--);
				}
				else
				{
					constructorDeclaration.Parameters[i] = o2;
				}
			}
			this.nodeStack.Push(constructorDeclaration.ConstructorInitializer);
			constructorDeclaration.ConstructorInitializer.AcceptVisitor(this, data);
			constructorDeclaration.ConstructorInitializer = (ConstructorInitializer)this.nodeStack.Pop();
			this.nodeStack.Push(constructorDeclaration.Body);
			constructorDeclaration.Body.AcceptVisitor(this, data);
			constructorDeclaration.Body = (BlockStatement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			Debug.Assert(constructorInitializer != null);
			Debug.Assert(constructorInitializer.Arguments != null);
			for (int i = 0; i < constructorInitializer.Arguments.Count; i++)
			{
				Expression o = constructorInitializer.Arguments[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					constructorInitializer.Arguments.RemoveAt(i--);
				}
				else
				{
					constructorInitializer.Arguments[i] = o;
				}
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
			for (int i = 0; i < declareDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = declareDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					declareDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					declareDeclaration.Attributes[i] = o;
				}
			}
			for (int i = 0; i < declareDeclaration.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = declareDeclaration.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					declareDeclaration.Parameters.RemoveAt(i--);
				}
				else
				{
					declareDeclaration.Parameters[i] = o2;
				}
			}
			this.nodeStack.Push(declareDeclaration.TypeReference);
			declareDeclaration.TypeReference.AcceptVisitor(this, data);
			declareDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			Debug.Assert(defaultValueExpression != null);
			Debug.Assert(defaultValueExpression.TypeReference != null);
			this.nodeStack.Push(defaultValueExpression.TypeReference);
			defaultValueExpression.TypeReference.AcceptVisitor(this, data);
			defaultValueExpression.TypeReference = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			Debug.Assert(delegateDeclaration != null);
			Debug.Assert(delegateDeclaration.Attributes != null);
			Debug.Assert(delegateDeclaration.ReturnType != null);
			Debug.Assert(delegateDeclaration.Parameters != null);
			Debug.Assert(delegateDeclaration.Templates != null);
			for (int i = 0; i < delegateDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = delegateDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					delegateDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					delegateDeclaration.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(delegateDeclaration.ReturnType);
			delegateDeclaration.ReturnType.AcceptVisitor(this, data);
			delegateDeclaration.ReturnType = (TypeReference)this.nodeStack.Pop();
			for (int i = 0; i < delegateDeclaration.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = delegateDeclaration.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					delegateDeclaration.Parameters.RemoveAt(i--);
				}
				else
				{
					delegateDeclaration.Parameters[i] = o2;
				}
			}
			for (int i = 0; i < delegateDeclaration.Templates.Count; i++)
			{
				TemplateDefinition o3 = delegateDeclaration.Templates[i];
				Debug.Assert(o3 != null);
				this.nodeStack.Push(o3);
				o3.AcceptVisitor(this, data);
				o3 = (TemplateDefinition)this.nodeStack.Pop();
				if (o3 == null)
				{
					delegateDeclaration.Templates.RemoveAt(i--);
				}
				else
				{
					delegateDeclaration.Templates[i] = o3;
				}
			}
			return null;
		}

		public virtual object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			Debug.Assert(destructorDeclaration != null);
			Debug.Assert(destructorDeclaration.Attributes != null);
			Debug.Assert(destructorDeclaration.Body != null);
			for (int i = 0; i < destructorDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = destructorDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					destructorDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					destructorDeclaration.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(destructorDeclaration.Body);
			destructorDeclaration.Body.AcceptVisitor(this, data);
			destructorDeclaration.Body = (BlockStatement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			Debug.Assert(directionExpression != null);
			Debug.Assert(directionExpression.Expression != null);
			this.nodeStack.Push(directionExpression.Expression);
			directionExpression.Expression.AcceptVisitor(this, data);
			directionExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			Debug.Assert(doLoopStatement != null);
			Debug.Assert(doLoopStatement.Condition != null);
			Debug.Assert(doLoopStatement.EmbeddedStatement != null);
			this.nodeStack.Push(doLoopStatement.Condition);
			doLoopStatement.Condition.AcceptVisitor(this, data);
			doLoopStatement.Condition = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(doLoopStatement.EmbeddedStatement);
			doLoopStatement.EmbeddedStatement.AcceptVisitor(this, data);
			doLoopStatement.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			Debug.Assert(elseIfSection != null);
			Debug.Assert(elseIfSection.Condition != null);
			Debug.Assert(elseIfSection.EmbeddedStatement != null);
			this.nodeStack.Push(elseIfSection.Condition);
			elseIfSection.Condition.AcceptVisitor(this, data);
			elseIfSection.Condition = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(elseIfSection.EmbeddedStatement);
			elseIfSection.EmbeddedStatement.AcceptVisitor(this, data);
			elseIfSection.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
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
			for (int i = 0; i < eraseStatement.Expressions.Count; i++)
			{
				Expression o = eraseStatement.Expressions[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					eraseStatement.Expressions.RemoveAt(i--);
				}
				else
				{
					eraseStatement.Expressions[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			Debug.Assert(errorStatement != null);
			Debug.Assert(errorStatement.Expression != null);
			this.nodeStack.Push(errorStatement.Expression);
			errorStatement.Expression.AcceptVisitor(this, data);
			errorStatement.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			Debug.Assert(eventAddRegion != null);
			Debug.Assert(eventAddRegion.Attributes != null);
			Debug.Assert(eventAddRegion.Block != null);
			Debug.Assert(eventAddRegion.Parameters != null);
			for (int i = 0; i < eventAddRegion.Attributes.Count; i++)
			{
				AttributeSection o = eventAddRegion.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					eventAddRegion.Attributes.RemoveAt(i--);
				}
				else
				{
					eventAddRegion.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(eventAddRegion.Block);
			eventAddRegion.Block.AcceptVisitor(this, data);
			eventAddRegion.Block = (BlockStatement)this.nodeStack.Pop();
			for (int i = 0; i < eventAddRegion.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = eventAddRegion.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					eventAddRegion.Parameters.RemoveAt(i--);
				}
				else
				{
					eventAddRegion.Parameters[i] = o2;
				}
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
			for (int i = 0; i < eventDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = eventDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					eventDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					eventDeclaration.Attributes[i] = o;
				}
			}
			for (int i = 0; i < eventDeclaration.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = eventDeclaration.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					eventDeclaration.Parameters.RemoveAt(i--);
				}
				else
				{
					eventDeclaration.Parameters[i] = o2;
				}
			}
			this.nodeStack.Push(eventDeclaration.TypeReference);
			eventDeclaration.TypeReference.AcceptVisitor(this, data);
			eventDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			for (int i = 0; i < eventDeclaration.InterfaceImplementations.Count; i++)
			{
				InterfaceImplementation o3 = eventDeclaration.InterfaceImplementations[i];
				Debug.Assert(o3 != null);
				this.nodeStack.Push(o3);
				o3.AcceptVisitor(this, data);
				o3 = (InterfaceImplementation)this.nodeStack.Pop();
				if (o3 == null)
				{
					eventDeclaration.InterfaceImplementations.RemoveAt(i--);
				}
				else
				{
					eventDeclaration.InterfaceImplementations[i] = o3;
				}
			}
			this.nodeStack.Push(eventDeclaration.AddRegion);
			eventDeclaration.AddRegion.AcceptVisitor(this, data);
			eventDeclaration.AddRegion = (EventAddRegion)this.nodeStack.Pop();
			this.nodeStack.Push(eventDeclaration.RemoveRegion);
			eventDeclaration.RemoveRegion.AcceptVisitor(this, data);
			eventDeclaration.RemoveRegion = (EventRemoveRegion)this.nodeStack.Pop();
			this.nodeStack.Push(eventDeclaration.RaiseRegion);
			eventDeclaration.RaiseRegion.AcceptVisitor(this, data);
			eventDeclaration.RaiseRegion = (EventRaiseRegion)this.nodeStack.Pop();
			this.nodeStack.Push(eventDeclaration.Initializer);
			eventDeclaration.Initializer.AcceptVisitor(this, data);
			eventDeclaration.Initializer = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			Debug.Assert(eventRaiseRegion != null);
			Debug.Assert(eventRaiseRegion.Attributes != null);
			Debug.Assert(eventRaiseRegion.Block != null);
			Debug.Assert(eventRaiseRegion.Parameters != null);
			for (int i = 0; i < eventRaiseRegion.Attributes.Count; i++)
			{
				AttributeSection o = eventRaiseRegion.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					eventRaiseRegion.Attributes.RemoveAt(i--);
				}
				else
				{
					eventRaiseRegion.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(eventRaiseRegion.Block);
			eventRaiseRegion.Block.AcceptVisitor(this, data);
			eventRaiseRegion.Block = (BlockStatement)this.nodeStack.Pop();
			for (int i = 0; i < eventRaiseRegion.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = eventRaiseRegion.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					eventRaiseRegion.Parameters.RemoveAt(i--);
				}
				else
				{
					eventRaiseRegion.Parameters[i] = o2;
				}
			}
			return null;
		}

		public virtual object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			Debug.Assert(eventRemoveRegion != null);
			Debug.Assert(eventRemoveRegion.Attributes != null);
			Debug.Assert(eventRemoveRegion.Block != null);
			Debug.Assert(eventRemoveRegion.Parameters != null);
			for (int i = 0; i < eventRemoveRegion.Attributes.Count; i++)
			{
				AttributeSection o = eventRemoveRegion.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					eventRemoveRegion.Attributes.RemoveAt(i--);
				}
				else
				{
					eventRemoveRegion.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(eventRemoveRegion.Block);
			eventRemoveRegion.Block.AcceptVisitor(this, data);
			eventRemoveRegion.Block = (BlockStatement)this.nodeStack.Pop();
			for (int i = 0; i < eventRemoveRegion.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = eventRemoveRegion.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					eventRemoveRegion.Parameters.RemoveAt(i--);
				}
				else
				{
					eventRemoveRegion.Parameters[i] = o2;
				}
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
			this.nodeStack.Push(expressionStatement.Expression);
			expressionStatement.Expression.AcceptVisitor(this, data);
			expressionStatement.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			Debug.Assert(fieldDeclaration != null);
			Debug.Assert(fieldDeclaration.Attributes != null);
			Debug.Assert(fieldDeclaration.TypeReference != null);
			Debug.Assert(fieldDeclaration.Fields != null);
			for (int i = 0; i < fieldDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = fieldDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					fieldDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					fieldDeclaration.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(fieldDeclaration.TypeReference);
			fieldDeclaration.TypeReference.AcceptVisitor(this, data);
			fieldDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			for (int i = 0; i < fieldDeclaration.Fields.Count; i++)
			{
				VariableDeclaration o2 = fieldDeclaration.Fields[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (VariableDeclaration)this.nodeStack.Pop();
				if (o2 == null)
				{
					fieldDeclaration.Fields.RemoveAt(i--);
				}
				else
				{
					fieldDeclaration.Fields[i] = o2;
				}
			}
			return null;
		}

		public virtual object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			Debug.Assert(fieldReferenceExpression != null);
			Debug.Assert(fieldReferenceExpression.TargetObject != null);
			this.nodeStack.Push(fieldReferenceExpression.TargetObject);
			fieldReferenceExpression.TargetObject.AcceptVisitor(this, data);
			fieldReferenceExpression.TargetObject = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			Debug.Assert(fixedStatement != null);
			Debug.Assert(fixedStatement.TypeReference != null);
			Debug.Assert(fixedStatement.PointerDeclarators != null);
			Debug.Assert(fixedStatement.EmbeddedStatement != null);
			this.nodeStack.Push(fixedStatement.TypeReference);
			fixedStatement.TypeReference.AcceptVisitor(this, data);
			fixedStatement.TypeReference = (TypeReference)this.nodeStack.Pop();
			for (int i = 0; i < fixedStatement.PointerDeclarators.Count; i++)
			{
				VariableDeclaration o = fixedStatement.PointerDeclarators[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (VariableDeclaration)this.nodeStack.Pop();
				if (o == null)
				{
					fixedStatement.PointerDeclarators.RemoveAt(i--);
				}
				else
				{
					fixedStatement.PointerDeclarators[i] = o;
				}
			}
			this.nodeStack.Push(fixedStatement.EmbeddedStatement);
			fixedStatement.EmbeddedStatement.AcceptVisitor(this, data);
			fixedStatement.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			Debug.Assert(foreachStatement != null);
			Debug.Assert(foreachStatement.TypeReference != null);
			Debug.Assert(foreachStatement.Expression != null);
			Debug.Assert(foreachStatement.NextExpression != null);
			Debug.Assert(foreachStatement.EmbeddedStatement != null);
			this.nodeStack.Push(foreachStatement.TypeReference);
			foreachStatement.TypeReference.AcceptVisitor(this, data);
			foreachStatement.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(foreachStatement.Expression);
			foreachStatement.Expression.AcceptVisitor(this, data);
			foreachStatement.Expression = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(foreachStatement.NextExpression);
			foreachStatement.NextExpression.AcceptVisitor(this, data);
			foreachStatement.NextExpression = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(foreachStatement.EmbeddedStatement);
			foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
			foreachStatement.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
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
			this.nodeStack.Push(forNextStatement.Start);
			forNextStatement.Start.AcceptVisitor(this, data);
			forNextStatement.Start = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(forNextStatement.End);
			forNextStatement.End.AcceptVisitor(this, data);
			forNextStatement.End = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(forNextStatement.Step);
			forNextStatement.Step.AcceptVisitor(this, data);
			forNextStatement.Step = (Expression)this.nodeStack.Pop();
			for (int i = 0; i < forNextStatement.NextExpressions.Count; i++)
			{
				Expression o = forNextStatement.NextExpressions[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					forNextStatement.NextExpressions.RemoveAt(i--);
				}
				else
				{
					forNextStatement.NextExpressions[i] = o;
				}
			}
			this.nodeStack.Push(forNextStatement.TypeReference);
			forNextStatement.TypeReference.AcceptVisitor(this, data);
			forNextStatement.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(forNextStatement.EmbeddedStatement);
			forNextStatement.EmbeddedStatement.AcceptVisitor(this, data);
			forNextStatement.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitForStatement(ForStatement forStatement, object data)
		{
			Debug.Assert(forStatement != null);
			Debug.Assert(forStatement.Initializers != null);
			Debug.Assert(forStatement.Condition != null);
			Debug.Assert(forStatement.Iterator != null);
			Debug.Assert(forStatement.EmbeddedStatement != null);
			for (int i = 0; i < forStatement.Initializers.Count; i++)
			{
				Statement o = forStatement.Initializers[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Statement)this.nodeStack.Pop();
				if (o == null)
				{
					forStatement.Initializers.RemoveAt(i--);
				}
				else
				{
					forStatement.Initializers[i] = o;
				}
			}
			this.nodeStack.Push(forStatement.Condition);
			forStatement.Condition.AcceptVisitor(this, data);
			forStatement.Condition = (Expression)this.nodeStack.Pop();
			for (int i = 0; i < forStatement.Iterator.Count; i++)
			{
				Statement o = forStatement.Iterator[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Statement)this.nodeStack.Pop();
				if (o == null)
				{
					forStatement.Iterator.RemoveAt(i--);
				}
				else
				{
					forStatement.Iterator[i] = o;
				}
			}
			this.nodeStack.Push(forStatement.EmbeddedStatement);
			forStatement.EmbeddedStatement.AcceptVisitor(this, data);
			forStatement.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			Debug.Assert(gotoCaseStatement != null);
			Debug.Assert(gotoCaseStatement.Expression != null);
			this.nodeStack.Push(gotoCaseStatement.Expression);
			gotoCaseStatement.Expression.AcceptVisitor(this, data);
			gotoCaseStatement.Expression = (Expression)this.nodeStack.Pop();
			return null;
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
			this.nodeStack.Push(ifElseStatement.Condition);
			ifElseStatement.Condition.AcceptVisitor(this, data);
			ifElseStatement.Condition = (Expression)this.nodeStack.Pop();
			for (int i = 0; i < ifElseStatement.TrueStatement.Count; i++)
			{
				Statement o = ifElseStatement.TrueStatement[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Statement)this.nodeStack.Pop();
				if (o == null)
				{
					ifElseStatement.TrueStatement.RemoveAt(i--);
				}
				else
				{
					ifElseStatement.TrueStatement[i] = o;
				}
			}
			for (int i = 0; i < ifElseStatement.FalseStatement.Count; i++)
			{
				Statement o = ifElseStatement.FalseStatement[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Statement)this.nodeStack.Pop();
				if (o == null)
				{
					ifElseStatement.FalseStatement.RemoveAt(i--);
				}
				else
				{
					ifElseStatement.FalseStatement[i] = o;
				}
			}
			for (int i = 0; i < ifElseStatement.ElseIfSections.Count; i++)
			{
				ElseIfSection o2 = ifElseStatement.ElseIfSections[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ElseIfSection)this.nodeStack.Pop();
				if (o2 == null)
				{
					ifElseStatement.ElseIfSections.RemoveAt(i--);
				}
				else
				{
					ifElseStatement.ElseIfSections[i] = o2;
				}
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
			for (int i = 0; i < indexerDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = indexerDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					indexerDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					indexerDeclaration.Attributes[i] = o;
				}
			}
			for (int i = 0; i < indexerDeclaration.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = indexerDeclaration.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					indexerDeclaration.Parameters.RemoveAt(i--);
				}
				else
				{
					indexerDeclaration.Parameters[i] = o2;
				}
			}
			for (int i = 0; i < indexerDeclaration.InterfaceImplementations.Count; i++)
			{
				InterfaceImplementation o3 = indexerDeclaration.InterfaceImplementations[i];
				Debug.Assert(o3 != null);
				this.nodeStack.Push(o3);
				o3.AcceptVisitor(this, data);
				o3 = (InterfaceImplementation)this.nodeStack.Pop();
				if (o3 == null)
				{
					indexerDeclaration.InterfaceImplementations.RemoveAt(i--);
				}
				else
				{
					indexerDeclaration.InterfaceImplementations[i] = o3;
				}
			}
			this.nodeStack.Push(indexerDeclaration.TypeReference);
			indexerDeclaration.TypeReference.AcceptVisitor(this, data);
			indexerDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(indexerDeclaration.GetRegion);
			indexerDeclaration.GetRegion.AcceptVisitor(this, data);
			indexerDeclaration.GetRegion = (PropertyGetRegion)this.nodeStack.Pop();
			this.nodeStack.Push(indexerDeclaration.SetRegion);
			indexerDeclaration.SetRegion.AcceptVisitor(this, data);
			indexerDeclaration.SetRegion = (PropertySetRegion)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			Debug.Assert(indexerExpression != null);
			Debug.Assert(indexerExpression.TargetObject != null);
			Debug.Assert(indexerExpression.Indexes != null);
			this.nodeStack.Push(indexerExpression.TargetObject);
			indexerExpression.TargetObject.AcceptVisitor(this, data);
			indexerExpression.TargetObject = (Expression)this.nodeStack.Pop();
			for (int i = 0; i < indexerExpression.Indexes.Count; i++)
			{
				Expression o = indexerExpression.Indexes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					indexerExpression.Indexes.RemoveAt(i--);
				}
				else
				{
					indexerExpression.Indexes[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			Debug.Assert(innerClassTypeReference != null);
			Debug.Assert(innerClassTypeReference.GenericTypes != null);
			Debug.Assert(innerClassTypeReference.BaseType != null);
			for (int i = 0; i < innerClassTypeReference.GenericTypes.Count; i++)
			{
				TypeReference o = innerClassTypeReference.GenericTypes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (TypeReference)this.nodeStack.Pop();
				if (o == null)
				{
					innerClassTypeReference.GenericTypes.RemoveAt(i--);
				}
				else
				{
					innerClassTypeReference.GenericTypes[i] = o;
				}
			}
			this.nodeStack.Push(innerClassTypeReference.BaseType);
			innerClassTypeReference.BaseType.AcceptVisitor(this, data);
			innerClassTypeReference.BaseType = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			Debug.Assert(interfaceImplementation != null);
			Debug.Assert(interfaceImplementation.InterfaceType != null);
			this.nodeStack.Push(interfaceImplementation.InterfaceType);
			interfaceImplementation.InterfaceType.AcceptVisitor(this, data);
			interfaceImplementation.InterfaceType = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Debug.Assert(invocationExpression != null);
			Debug.Assert(invocationExpression.TargetObject != null);
			Debug.Assert(invocationExpression.Arguments != null);
			Debug.Assert(invocationExpression.TypeArguments != null);
			this.nodeStack.Push(invocationExpression.TargetObject);
			invocationExpression.TargetObject.AcceptVisitor(this, data);
			invocationExpression.TargetObject = (Expression)this.nodeStack.Pop();
			for (int i = 0; i < invocationExpression.Arguments.Count; i++)
			{
				Expression o = invocationExpression.Arguments[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					invocationExpression.Arguments.RemoveAt(i--);
				}
				else
				{
					invocationExpression.Arguments[i] = o;
				}
			}
			for (int i = 0; i < invocationExpression.TypeArguments.Count; i++)
			{
				TypeReference o2 = invocationExpression.TypeArguments[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (TypeReference)this.nodeStack.Pop();
				if (o2 == null)
				{
					invocationExpression.TypeArguments.RemoveAt(i--);
				}
				else
				{
					invocationExpression.TypeArguments[i] = o2;
				}
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
			this.nodeStack.Push(localVariableDeclaration.TypeReference);
			localVariableDeclaration.TypeReference.AcceptVisitor(this, data);
			localVariableDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			for (int i = 0; i < localVariableDeclaration.Variables.Count; i++)
			{
				VariableDeclaration o = localVariableDeclaration.Variables[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (VariableDeclaration)this.nodeStack.Pop();
				if (o == null)
				{
					localVariableDeclaration.Variables.RemoveAt(i--);
				}
				else
				{
					localVariableDeclaration.Variables[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitLockStatement(LockStatement lockStatement, object data)
		{
			Debug.Assert(lockStatement != null);
			Debug.Assert(lockStatement.LockExpression != null);
			Debug.Assert(lockStatement.EmbeddedStatement != null);
			this.nodeStack.Push(lockStatement.LockExpression);
			lockStatement.LockExpression.AcceptVisitor(this, data);
			lockStatement.LockExpression = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(lockStatement.EmbeddedStatement);
			lockStatement.EmbeddedStatement.AcceptVisitor(this, data);
			lockStatement.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
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
			for (int i = 0; i < methodDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = methodDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					methodDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					methodDeclaration.Attributes[i] = o;
				}
			}
			for (int i = 0; i < methodDeclaration.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = methodDeclaration.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					methodDeclaration.Parameters.RemoveAt(i--);
				}
				else
				{
					methodDeclaration.Parameters[i] = o2;
				}
			}
			this.nodeStack.Push(methodDeclaration.TypeReference);
			methodDeclaration.TypeReference.AcceptVisitor(this, data);
			methodDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(methodDeclaration.Body);
			methodDeclaration.Body.AcceptVisitor(this, data);
			methodDeclaration.Body = (BlockStatement)this.nodeStack.Pop();
			for (int i = 0; i < methodDeclaration.InterfaceImplementations.Count; i++)
			{
				InterfaceImplementation o3 = methodDeclaration.InterfaceImplementations[i];
				Debug.Assert(o3 != null);
				this.nodeStack.Push(o3);
				o3.AcceptVisitor(this, data);
				o3 = (InterfaceImplementation)this.nodeStack.Pop();
				if (o3 == null)
				{
					methodDeclaration.InterfaceImplementations.RemoveAt(i--);
				}
				else
				{
					methodDeclaration.InterfaceImplementations[i] = o3;
				}
			}
			for (int i = 0; i < methodDeclaration.Templates.Count; i++)
			{
				TemplateDefinition o4 = methodDeclaration.Templates[i];
				Debug.Assert(o4 != null);
				this.nodeStack.Push(o4);
				o4.AcceptVisitor(this, data);
				o4 = (TemplateDefinition)this.nodeStack.Pop();
				if (o4 == null)
				{
					methodDeclaration.Templates.RemoveAt(i--);
				}
				else
				{
					methodDeclaration.Templates[i] = o4;
				}
			}
			return null;
		}

		public virtual object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			Debug.Assert(namedArgumentExpression != null);
			Debug.Assert(namedArgumentExpression.Expression != null);
			this.nodeStack.Push(namedArgumentExpression.Expression);
			namedArgumentExpression.Expression.AcceptVisitor(this, data);
			namedArgumentExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			Debug.Assert(namespaceDeclaration != null);
			for (int i = 0; i < namespaceDeclaration.Children.Count; i++)
			{
				INode o = namespaceDeclaration.Children[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = this.nodeStack.Pop();
				if (o == null)
				{
					namespaceDeclaration.Children.RemoveAt(i--);
				}
				else
				{
					namespaceDeclaration.Children[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			Debug.Assert(objectCreateExpression != null);
			Debug.Assert(objectCreateExpression.CreateType != null);
			Debug.Assert(objectCreateExpression.Parameters != null);
			this.nodeStack.Push(objectCreateExpression.CreateType);
			objectCreateExpression.CreateType.AcceptVisitor(this, data);
			objectCreateExpression.CreateType = (TypeReference)this.nodeStack.Pop();
			for (int i = 0; i < objectCreateExpression.Parameters.Count; i++)
			{
				Expression o = objectCreateExpression.Parameters[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					objectCreateExpression.Parameters.RemoveAt(i--);
				}
				else
				{
					objectCreateExpression.Parameters[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			Debug.Assert(onErrorStatement != null);
			Debug.Assert(onErrorStatement.EmbeddedStatement != null);
			this.nodeStack.Push(onErrorStatement.EmbeddedStatement);
			onErrorStatement.EmbeddedStatement.AcceptVisitor(this, data);
			onErrorStatement.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
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
			for (int i = 0; i < operatorDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = operatorDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					operatorDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					operatorDeclaration.Attributes[i] = o;
				}
			}
			for (int i = 0; i < operatorDeclaration.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = operatorDeclaration.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					operatorDeclaration.Parameters.RemoveAt(i--);
				}
				else
				{
					operatorDeclaration.Parameters[i] = o2;
				}
			}
			this.nodeStack.Push(operatorDeclaration.TypeReference);
			operatorDeclaration.TypeReference.AcceptVisitor(this, data);
			operatorDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(operatorDeclaration.Body);
			operatorDeclaration.Body.AcceptVisitor(this, data);
			operatorDeclaration.Body = (BlockStatement)this.nodeStack.Pop();
			for (int i = 0; i < operatorDeclaration.InterfaceImplementations.Count; i++)
			{
				InterfaceImplementation o3 = operatorDeclaration.InterfaceImplementations[i];
				Debug.Assert(o3 != null);
				this.nodeStack.Push(o3);
				o3.AcceptVisitor(this, data);
				o3 = (InterfaceImplementation)this.nodeStack.Pop();
				if (o3 == null)
				{
					operatorDeclaration.InterfaceImplementations.RemoveAt(i--);
				}
				else
				{
					operatorDeclaration.InterfaceImplementations[i] = o3;
				}
			}
			for (int i = 0; i < operatorDeclaration.Templates.Count; i++)
			{
				TemplateDefinition o4 = operatorDeclaration.Templates[i];
				Debug.Assert(o4 != null);
				this.nodeStack.Push(o4);
				o4.AcceptVisitor(this, data);
				o4 = (TemplateDefinition)this.nodeStack.Pop();
				if (o4 == null)
				{
					operatorDeclaration.Templates.RemoveAt(i--);
				}
				else
				{
					operatorDeclaration.Templates[i] = o4;
				}
			}
			for (int i = 0; i < operatorDeclaration.ReturnTypeAttributes.Count; i++)
			{
				AttributeSection o = operatorDeclaration.ReturnTypeAttributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					operatorDeclaration.ReturnTypeAttributes.RemoveAt(i--);
				}
				else
				{
					operatorDeclaration.ReturnTypeAttributes[i] = o;
				}
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
			for (int i = 0; i < parameterDeclarationExpression.Attributes.Count; i++)
			{
				AttributeSection o = parameterDeclarationExpression.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					parameterDeclarationExpression.Attributes.RemoveAt(i--);
				}
				else
				{
					parameterDeclarationExpression.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(parameterDeclarationExpression.TypeReference);
			parameterDeclarationExpression.TypeReference.AcceptVisitor(this, data);
			parameterDeclarationExpression.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(parameterDeclarationExpression.DefaultValue);
			parameterDeclarationExpression.DefaultValue.AcceptVisitor(this, data);
			parameterDeclarationExpression.DefaultValue = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			Debug.Assert(parenthesizedExpression != null);
			Debug.Assert(parenthesizedExpression.Expression != null);
			this.nodeStack.Push(parenthesizedExpression.Expression);
			parenthesizedExpression.Expression.AcceptVisitor(this, data);
			parenthesizedExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			Debug.Assert(pointerReferenceExpression != null);
			Debug.Assert(pointerReferenceExpression.TargetObject != null);
			this.nodeStack.Push(pointerReferenceExpression.TargetObject);
			pointerReferenceExpression.TargetObject.AcceptVisitor(this, data);
			pointerReferenceExpression.TargetObject = (Expression)this.nodeStack.Pop();
			return null;
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
			for (int i = 0; i < propertyDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = propertyDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					propertyDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					propertyDeclaration.Attributes[i] = o;
				}
			}
			for (int i = 0; i < propertyDeclaration.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = propertyDeclaration.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					propertyDeclaration.Parameters.RemoveAt(i--);
				}
				else
				{
					propertyDeclaration.Parameters[i] = o2;
				}
			}
			for (int i = 0; i < propertyDeclaration.InterfaceImplementations.Count; i++)
			{
				InterfaceImplementation o3 = propertyDeclaration.InterfaceImplementations[i];
				Debug.Assert(o3 != null);
				this.nodeStack.Push(o3);
				o3.AcceptVisitor(this, data);
				o3 = (InterfaceImplementation)this.nodeStack.Pop();
				if (o3 == null)
				{
					propertyDeclaration.InterfaceImplementations.RemoveAt(i--);
				}
				else
				{
					propertyDeclaration.InterfaceImplementations[i] = o3;
				}
			}
			this.nodeStack.Push(propertyDeclaration.TypeReference);
			propertyDeclaration.TypeReference.AcceptVisitor(this, data);
			propertyDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(propertyDeclaration.GetRegion);
			propertyDeclaration.GetRegion.AcceptVisitor(this, data);
			propertyDeclaration.GetRegion = (PropertyGetRegion)this.nodeStack.Pop();
			this.nodeStack.Push(propertyDeclaration.SetRegion);
			propertyDeclaration.SetRegion.AcceptVisitor(this, data);
			propertyDeclaration.SetRegion = (PropertySetRegion)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			Debug.Assert(propertyGetRegion != null);
			Debug.Assert(propertyGetRegion.Attributes != null);
			Debug.Assert(propertyGetRegion.Block != null);
			for (int i = 0; i < propertyGetRegion.Attributes.Count; i++)
			{
				AttributeSection o = propertyGetRegion.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					propertyGetRegion.Attributes.RemoveAt(i--);
				}
				else
				{
					propertyGetRegion.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(propertyGetRegion.Block);
			propertyGetRegion.Block.AcceptVisitor(this, data);
			propertyGetRegion.Block = (BlockStatement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			Debug.Assert(propertySetRegion != null);
			Debug.Assert(propertySetRegion.Attributes != null);
			Debug.Assert(propertySetRegion.Block != null);
			Debug.Assert(propertySetRegion.Parameters != null);
			for (int i = 0; i < propertySetRegion.Attributes.Count; i++)
			{
				AttributeSection o = propertySetRegion.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					propertySetRegion.Attributes.RemoveAt(i--);
				}
				else
				{
					propertySetRegion.Attributes[i] = o;
				}
			}
			this.nodeStack.Push(propertySetRegion.Block);
			propertySetRegion.Block.AcceptVisitor(this, data);
			propertySetRegion.Block = (BlockStatement)this.nodeStack.Pop();
			for (int i = 0; i < propertySetRegion.Parameters.Count; i++)
			{
				ParameterDeclarationExpression o2 = propertySetRegion.Parameters[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (ParameterDeclarationExpression)this.nodeStack.Pop();
				if (o2 == null)
				{
					propertySetRegion.Parameters.RemoveAt(i--);
				}
				else
				{
					propertySetRegion.Parameters[i] = o2;
				}
			}
			return null;
		}

		public virtual object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			Debug.Assert(raiseEventStatement != null);
			Debug.Assert(raiseEventStatement.Arguments != null);
			for (int i = 0; i < raiseEventStatement.Arguments.Count; i++)
			{
				Expression o = raiseEventStatement.Arguments[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Expression)this.nodeStack.Pop();
				if (o == null)
				{
					raiseEventStatement.Arguments.RemoveAt(i--);
				}
				else
				{
					raiseEventStatement.Arguments[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			Debug.Assert(reDimStatement != null);
			Debug.Assert(reDimStatement.ReDimClauses != null);
			for (int i = 0; i < reDimStatement.ReDimClauses.Count; i++)
			{
				InvocationExpression o = reDimStatement.ReDimClauses[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (InvocationExpression)this.nodeStack.Pop();
				if (o == null)
				{
					reDimStatement.ReDimClauses.RemoveAt(i--);
				}
				else
				{
					reDimStatement.ReDimClauses[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			Debug.Assert(removeHandlerStatement != null);
			Debug.Assert(removeHandlerStatement.EventExpression != null);
			Debug.Assert(removeHandlerStatement.HandlerExpression != null);
			this.nodeStack.Push(removeHandlerStatement.EventExpression);
			removeHandlerStatement.EventExpression.AcceptVisitor(this, data);
			removeHandlerStatement.EventExpression = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(removeHandlerStatement.HandlerExpression);
			removeHandlerStatement.HandlerExpression.AcceptVisitor(this, data);
			removeHandlerStatement.HandlerExpression = (Expression)this.nodeStack.Pop();
			return null;
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
			this.nodeStack.Push(returnStatement.Expression);
			returnStatement.Expression.AcceptVisitor(this, data);
			returnStatement.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			Debug.Assert(sizeOfExpression != null);
			Debug.Assert(sizeOfExpression.TypeReference != null);
			this.nodeStack.Push(sizeOfExpression.TypeReference);
			sizeOfExpression.TypeReference.AcceptVisitor(this, data);
			sizeOfExpression.TypeReference = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			Debug.Assert(stackAllocExpression != null);
			Debug.Assert(stackAllocExpression.TypeReference != null);
			Debug.Assert(stackAllocExpression.Expression != null);
			this.nodeStack.Push(stackAllocExpression.TypeReference);
			stackAllocExpression.TypeReference.AcceptVisitor(this, data);
			stackAllocExpression.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(stackAllocExpression.Expression);
			stackAllocExpression.Expression.AcceptVisitor(this, data);
			stackAllocExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
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
			for (int i = 0; i < switchSection.SwitchLabels.Count; i++)
			{
				CaseLabel o = switchSection.SwitchLabels[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (CaseLabel)this.nodeStack.Pop();
				if (o == null)
				{
					switchSection.SwitchLabels.RemoveAt(i--);
				}
				else
				{
					switchSection.SwitchLabels[i] = o;
				}
			}
			for (int i = 0; i < switchSection.Children.Count; i++)
			{
				INode o2 = switchSection.Children[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = this.nodeStack.Pop();
				if (o2 == null)
				{
					switchSection.Children.RemoveAt(i--);
				}
				else
				{
					switchSection.Children[i] = o2;
				}
			}
			return null;
		}

		public virtual object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			Debug.Assert(switchStatement != null);
			Debug.Assert(switchStatement.SwitchExpression != null);
			Debug.Assert(switchStatement.SwitchSections != null);
			this.nodeStack.Push(switchStatement.SwitchExpression);
			switchStatement.SwitchExpression.AcceptVisitor(this, data);
			switchStatement.SwitchExpression = (Expression)this.nodeStack.Pop();
			for (int i = 0; i < switchStatement.SwitchSections.Count; i++)
			{
				SwitchSection o = switchStatement.SwitchSections[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (SwitchSection)this.nodeStack.Pop();
				if (o == null)
				{
					switchStatement.SwitchSections.RemoveAt(i--);
				}
				else
				{
					switchStatement.SwitchSections[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			Debug.Assert(templateDefinition != null);
			Debug.Assert(templateDefinition.Attributes != null);
			Debug.Assert(templateDefinition.Bases != null);
			for (int i = 0; i < templateDefinition.Attributes.Count; i++)
			{
				AttributeSection o = templateDefinition.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					templateDefinition.Attributes.RemoveAt(i--);
				}
				else
				{
					templateDefinition.Attributes[i] = o;
				}
			}
			for (int i = 0; i < templateDefinition.Bases.Count; i++)
			{
				TypeReference o2 = templateDefinition.Bases[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (TypeReference)this.nodeStack.Pop();
				if (o2 == null)
				{
					templateDefinition.Bases.RemoveAt(i--);
				}
				else
				{
					templateDefinition.Bases[i] = o2;
				}
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
			this.nodeStack.Push(throwStatement.Expression);
			throwStatement.Expression.AcceptVisitor(this, data);
			throwStatement.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			Debug.Assert(tryCatchStatement != null);
			Debug.Assert(tryCatchStatement.StatementBlock != null);
			Debug.Assert(tryCatchStatement.CatchClauses != null);
			Debug.Assert(tryCatchStatement.FinallyBlock != null);
			this.nodeStack.Push(tryCatchStatement.StatementBlock);
			tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
			tryCatchStatement.StatementBlock = (Statement)this.nodeStack.Pop();
			for (int i = 0; i < tryCatchStatement.CatchClauses.Count; i++)
			{
				CatchClause o = tryCatchStatement.CatchClauses[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (CatchClause)this.nodeStack.Pop();
				if (o == null)
				{
					tryCatchStatement.CatchClauses.RemoveAt(i--);
				}
				else
				{
					tryCatchStatement.CatchClauses[i] = o;
				}
			}
			this.nodeStack.Push(tryCatchStatement.FinallyBlock);
			tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
			tryCatchStatement.FinallyBlock = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			Debug.Assert(typeDeclaration != null);
			Debug.Assert(typeDeclaration.Attributes != null);
			Debug.Assert(typeDeclaration.BaseTypes != null);
			Debug.Assert(typeDeclaration.Templates != null);
			for (int i = 0; i < typeDeclaration.Attributes.Count; i++)
			{
				AttributeSection o = typeDeclaration.Attributes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (AttributeSection)this.nodeStack.Pop();
				if (o == null)
				{
					typeDeclaration.Attributes.RemoveAt(i--);
				}
				else
				{
					typeDeclaration.Attributes[i] = o;
				}
			}
			for (int i = 0; i < typeDeclaration.BaseTypes.Count; i++)
			{
				TypeReference o2 = typeDeclaration.BaseTypes[i];
				Debug.Assert(o2 != null);
				this.nodeStack.Push(o2);
				o2.AcceptVisitor(this, data);
				o2 = (TypeReference)this.nodeStack.Pop();
				if (o2 == null)
				{
					typeDeclaration.BaseTypes.RemoveAt(i--);
				}
				else
				{
					typeDeclaration.BaseTypes[i] = o2;
				}
			}
			for (int i = 0; i < typeDeclaration.Templates.Count; i++)
			{
				TemplateDefinition o3 = typeDeclaration.Templates[i];
				Debug.Assert(o3 != null);
				this.nodeStack.Push(o3);
				o3.AcceptVisitor(this, data);
				o3 = (TemplateDefinition)this.nodeStack.Pop();
				if (o3 == null)
				{
					typeDeclaration.Templates.RemoveAt(i--);
				}
				else
				{
					typeDeclaration.Templates[i] = o3;
				}
			}
			for (int i = 0; i < typeDeclaration.Children.Count; i++)
			{
				INode o4 = typeDeclaration.Children[i];
				Debug.Assert(o4 != null);
				this.nodeStack.Push(o4);
				o4.AcceptVisitor(this, data);
				o4 = this.nodeStack.Pop();
				if (o4 == null)
				{
					typeDeclaration.Children.RemoveAt(i--);
				}
				else
				{
					typeDeclaration.Children[i] = o4;
				}
			}
			return null;
		}

		public virtual object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			Debug.Assert(typeOfExpression != null);
			Debug.Assert(typeOfExpression.TypeReference != null);
			this.nodeStack.Push(typeOfExpression.TypeReference);
			typeOfExpression.TypeReference.AcceptVisitor(this, data);
			typeOfExpression.TypeReference = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			Debug.Assert(typeOfIsExpression != null);
			Debug.Assert(typeOfIsExpression.Expression != null);
			Debug.Assert(typeOfIsExpression.TypeReference != null);
			this.nodeStack.Push(typeOfIsExpression.Expression);
			typeOfIsExpression.Expression.AcceptVisitor(this, data);
			typeOfIsExpression.Expression = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(typeOfIsExpression.TypeReference);
			typeOfIsExpression.TypeReference.AcceptVisitor(this, data);
			typeOfIsExpression.TypeReference = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitTypeReference(TypeReference typeReference, object data)
		{
			Debug.Assert(typeReference != null);
			Debug.Assert(typeReference.GenericTypes != null);
			for (int i = 0; i < typeReference.GenericTypes.Count; i++)
			{
				TypeReference o = typeReference.GenericTypes[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (TypeReference)this.nodeStack.Pop();
				if (o == null)
				{
					typeReference.GenericTypes.RemoveAt(i--);
				}
				else
				{
					typeReference.GenericTypes[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			Debug.Assert(typeReferenceExpression != null);
			Debug.Assert(typeReferenceExpression.TypeReference != null);
			this.nodeStack.Push(typeReferenceExpression.TypeReference);
			typeReferenceExpression.TypeReference.AcceptVisitor(this, data);
			typeReferenceExpression.TypeReference = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			Debug.Assert(unaryOperatorExpression != null);
			Debug.Assert(unaryOperatorExpression.Expression != null);
			this.nodeStack.Push(unaryOperatorExpression.Expression);
			unaryOperatorExpression.Expression.AcceptVisitor(this, data);
			unaryOperatorExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			Debug.Assert(uncheckedExpression != null);
			Debug.Assert(uncheckedExpression.Expression != null);
			this.nodeStack.Push(uncheckedExpression.Expression);
			uncheckedExpression.Expression.AcceptVisitor(this, data);
			uncheckedExpression.Expression = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			Debug.Assert(uncheckedStatement != null);
			Debug.Assert(uncheckedStatement.Block != null);
			this.nodeStack.Push(uncheckedStatement.Block);
			uncheckedStatement.Block.AcceptVisitor(this, data);
			uncheckedStatement.Block = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			Debug.Assert(unsafeStatement != null);
			Debug.Assert(unsafeStatement.Block != null);
			this.nodeStack.Push(unsafeStatement.Block);
			unsafeStatement.Block.AcceptVisitor(this, data);
			unsafeStatement.Block = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitUsing(Using @using, object data)
		{
			Debug.Assert(@using != null);
			Debug.Assert(@using.Alias != null);
			this.nodeStack.Push(@using.Alias);
			@using.Alias.AcceptVisitor(this, data);
			@using.Alias = (TypeReference)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			Debug.Assert(usingDeclaration != null);
			Debug.Assert(usingDeclaration.Usings != null);
			for (int i = 0; i < usingDeclaration.Usings.Count; i++)
			{
				Using o = usingDeclaration.Usings[i];
				Debug.Assert(o != null);
				this.nodeStack.Push(o);
				o.AcceptVisitor(this, data);
				o = (Using)this.nodeStack.Pop();
				if (o == null)
				{
					usingDeclaration.Usings.RemoveAt(i--);
				}
				else
				{
					usingDeclaration.Usings[i] = o;
				}
			}
			return null;
		}

		public virtual object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			Debug.Assert(usingStatement != null);
			Debug.Assert(usingStatement.ResourceAcquisition != null);
			Debug.Assert(usingStatement.EmbeddedStatement != null);
			this.nodeStack.Push(usingStatement.ResourceAcquisition);
			usingStatement.ResourceAcquisition.AcceptVisitor(this, data);
			usingStatement.ResourceAcquisition = (Statement)this.nodeStack.Pop();
			this.nodeStack.Push(usingStatement.EmbeddedStatement);
			usingStatement.EmbeddedStatement.AcceptVisitor(this, data);
			usingStatement.EmbeddedStatement = (Statement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			Debug.Assert(variableDeclaration != null);
			Debug.Assert(variableDeclaration.Initializer != null);
			Debug.Assert(variableDeclaration.TypeReference != null);
			Debug.Assert(variableDeclaration.FixedArrayInitialization != null);
			this.nodeStack.Push(variableDeclaration.Initializer);
			variableDeclaration.Initializer.AcceptVisitor(this, data);
			variableDeclaration.Initializer = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(variableDeclaration.TypeReference);
			variableDeclaration.TypeReference.AcceptVisitor(this, data);
			variableDeclaration.TypeReference = (TypeReference)this.nodeStack.Pop();
			this.nodeStack.Push(variableDeclaration.FixedArrayInitialization);
			variableDeclaration.FixedArrayInitialization.AcceptVisitor(this, data);
			variableDeclaration.FixedArrayInitialization = (Expression)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitWithStatement(WithStatement withStatement, object data)
		{
			Debug.Assert(withStatement != null);
			Debug.Assert(withStatement.Expression != null);
			Debug.Assert(withStatement.Body != null);
			this.nodeStack.Push(withStatement.Expression);
			withStatement.Expression.AcceptVisitor(this, data);
			withStatement.Expression = (Expression)this.nodeStack.Pop();
			this.nodeStack.Push(withStatement.Body);
			withStatement.Body.AcceptVisitor(this, data);
			withStatement.Body = (BlockStatement)this.nodeStack.Pop();
			return null;
		}

		public virtual object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			Debug.Assert(yieldStatement != null);
			Debug.Assert(yieldStatement.Statement != null);
			this.nodeStack.Push(yieldStatement.Statement);
			yieldStatement.Statement.AcceptVisitor(this, data);
			yieldStatement.Statement = (Statement)this.nodeStack.Pop();
			return null;
		}
	}
}

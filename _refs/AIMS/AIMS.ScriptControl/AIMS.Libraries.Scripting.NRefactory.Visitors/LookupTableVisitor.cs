using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public class LookupTableVisitor : AbstractAstVisitor
	{
		private Dictionary<string, List<LocalLookupVariable>> variables;

		private SupportedLanguage language;

		private List<WithStatement> withStatements = new List<WithStatement>();

		private Stack<Location> endLocationStack = new Stack<Location>();

		public Dictionary<string, List<LocalLookupVariable>> Variables
		{
			get
			{
				return this.variables;
			}
		}

		public List<WithStatement> WithStatements
		{
			get
			{
				return this.withStatements;
			}
		}

		private Location CurrentEndLocation
		{
			get
			{
				return (this.endLocationStack.Count == 0) ? Location.Empty : this.endLocationStack.Peek();
			}
		}

		public LookupTableVisitor(SupportedLanguage language)
		{
			this.language = language;
			if (language == SupportedLanguage.VBNet)
			{
				this.variables = new Dictionary<string, List<LocalLookupVariable>>(StringComparer.InvariantCultureIgnoreCase);
			}
			else
			{
				this.variables = new Dictionary<string, List<LocalLookupVariable>>(StringComparer.InvariantCulture);
			}
		}

		public void AddVariable(TypeReference typeRef, string name, Location startPos, Location endPos, bool isConst)
		{
			if (name != null && name.Length != 0)
			{
				List<LocalLookupVariable> list;
				if (!this.variables.ContainsKey(name))
				{
					list = (this.variables[name] = new List<LocalLookupVariable>());
				}
				else
				{
					list = this.variables[name];
				}
				list.Add(new LocalLookupVariable(typeRef, startPos, endPos, isConst));
			}
		}

		public override object VisitWithStatement(WithStatement withStatement, object data)
		{
			this.withStatements.Add(withStatement);
			return base.VisitWithStatement(withStatement, data);
		}

		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			this.endLocationStack.Push(blockStatement.EndLocation);
			base.VisitBlockStatement(blockStatement, data);
			this.endLocationStack.Pop();
			return null;
		}

		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			for (int i = 0; i < localVariableDeclaration.Variables.Count; i++)
			{
				VariableDeclaration varDecl = localVariableDeclaration.Variables[i];
				this.AddVariable(localVariableDeclaration.GetTypeForVariable(i), varDecl.Name, localVariableDeclaration.StartLocation, this.CurrentEndLocation, (localVariableDeclaration.Modifier & Modifiers.Const) == Modifiers.Const);
			}
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}

		public override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			foreach (ParameterDeclarationExpression p in anonymousMethodExpression.Parameters)
			{
				this.AddVariable(p.TypeReference, p.ParameterName, anonymousMethodExpression.StartLocation, anonymousMethodExpression.EndLocation, false);
			}
			return base.VisitAnonymousMethodExpression(anonymousMethodExpression, data);
		}

		public override object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			object result;
			if (forNextStatement.EmbeddedStatement.EndLocation.IsEmpty)
			{
				result = base.VisitForNextStatement(forNextStatement, data);
			}
			else
			{
				this.endLocationStack.Push(forNextStatement.EmbeddedStatement.EndLocation);
				base.VisitForNextStatement(forNextStatement, data);
				this.endLocationStack.Pop();
				result = null;
			}
			return result;
		}

		public override object VisitForStatement(ForStatement forStatement, object data)
		{
			object result;
			if (forStatement.EmbeddedStatement.EndLocation.IsEmpty)
			{
				result = base.VisitForStatement(forStatement, data);
			}
			else
			{
				this.endLocationStack.Push(forStatement.EmbeddedStatement.EndLocation);
				base.VisitForStatement(forStatement, data);
				this.endLocationStack.Pop();
				result = null;
			}
			return result;
		}

		public override object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			object result;
			if (usingStatement.EmbeddedStatement.EndLocation.IsEmpty)
			{
				result = base.VisitUsingStatement(usingStatement, data);
			}
			else
			{
				this.endLocationStack.Push(usingStatement.EmbeddedStatement.EndLocation);
				base.VisitUsingStatement(usingStatement, data);
				this.endLocationStack.Pop();
				result = null;
			}
			return result;
		}

		public override object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			object result;
			if (this.language == SupportedLanguage.VBNet)
			{
				result = this.VisitBlockStatement(switchSection, data);
			}
			else
			{
				result = base.VisitSwitchSection(switchSection, data);
			}
			return result;
		}

		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			this.AddVariable(foreachStatement.TypeReference, foreachStatement.VariableName, foreachStatement.StartLocation, foreachStatement.EndLocation, false);
			if (foreachStatement.Expression != null)
			{
				foreachStatement.Expression.AcceptVisitor(this, data);
			}
			object result;
			if (foreachStatement.EmbeddedStatement == null)
			{
				result = data;
			}
			else
			{
				result = foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
			}
			return result;
		}

		public override object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			object result;
			if (tryCatchStatement == null)
			{
				result = data;
			}
			else
			{
				if (tryCatchStatement.StatementBlock != null)
				{
					tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
				}
				if (tryCatchStatement.CatchClauses != null)
				{
					foreach (CatchClause catchClause in tryCatchStatement.CatchClauses)
					{
						if (catchClause != null)
						{
							if (catchClause.TypeReference != null && catchClause.VariableName != null)
							{
								this.AddVariable(catchClause.TypeReference, catchClause.VariableName, catchClause.StatementBlock.StartLocation, catchClause.StatementBlock.EndLocation, false);
							}
							catchClause.StatementBlock.AcceptVisitor(this, data);
						}
					}
				}
				if (tryCatchStatement.FinallyBlock != null)
				{
					result = tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
				}
				else
				{
					result = data;
				}
			}
			return result;
		}
	}
}

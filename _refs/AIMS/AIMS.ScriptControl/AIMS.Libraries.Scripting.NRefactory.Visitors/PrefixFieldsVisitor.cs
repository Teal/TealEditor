using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public class PrefixFieldsVisitor : AbstractAstVisitor
	{
		private List<VariableDeclaration> fields;

		private List<string> curBlock = new List<string>();

		private Stack<List<string>> blocks = new Stack<List<string>>();

		private string prefix;

		public PrefixFieldsVisitor(List<VariableDeclaration> fields, string prefix)
		{
			this.fields = fields;
			this.prefix = prefix;
		}

		public void Run(INode typeDeclaration)
		{
			typeDeclaration.AcceptVisitor(this, null);
			foreach (VariableDeclaration decl in this.fields)
			{
				decl.Name = this.prefix + decl.Name;
			}
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			this.Push();
			object result = base.VisitTypeDeclaration(typeDeclaration, data);
			this.Pop();
			return result;
		}

		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			this.Push();
			object result = base.VisitBlockStatement(blockStatement, data);
			this.Pop();
			return result;
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			this.Push();
			object result = base.VisitMethodDeclaration(methodDeclaration, data);
			this.Pop();
			return result;
		}

		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			this.Push();
			object result = base.VisitPropertyDeclaration(propertyDeclaration, data);
			this.Pop();
			return result;
		}

		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			this.Push();
			object result = base.VisitConstructorDeclaration(constructorDeclaration, data);
			this.Pop();
			return result;
		}

		private void Push()
		{
			this.blocks.Push(this.curBlock);
			this.curBlock = new List<string>();
		}

		private void Pop()
		{
			this.curBlock = this.blocks.Pop();
		}

		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			object result;
			if (this.fields.Contains(variableDeclaration))
			{
				result = null;
			}
			else
			{
				this.curBlock.Add(variableDeclaration.Name);
				result = base.VisitVariableDeclaration(variableDeclaration, data);
			}
			return result;
		}

		public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			this.curBlock.Add(parameterDeclarationExpression.ParameterName);
			return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
		}

		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			this.curBlock.Add(foreachStatement.VariableName);
			return base.VisitForeachStatement(foreachStatement, data);
		}

		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string name = identifierExpression.Identifier;
			foreach (VariableDeclaration var in this.fields)
			{
				if (var.Name == name && !this.IsLocal(name))
				{
					identifierExpression.Identifier = this.prefix + name;
					break;
				}
			}
			return base.VisitIdentifierExpression(identifierExpression, data);
		}

		public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			if (fieldReferenceExpression.TargetObject is ThisReferenceExpression)
			{
				string name = fieldReferenceExpression.FieldName;
				foreach (VariableDeclaration var in this.fields)
				{
					if (var.Name == name)
					{
						fieldReferenceExpression.FieldName = this.prefix + name;
						break;
					}
				}
			}
			return base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		private bool IsLocal(string name)
		{
			bool result;
			foreach (List<string> block in this.blocks)
			{
				if (block.Contains(name))
				{
					result = true;
					return result;
				}
			}
			result = this.curBlock.Contains(name);
			return result;
		}
	}
}

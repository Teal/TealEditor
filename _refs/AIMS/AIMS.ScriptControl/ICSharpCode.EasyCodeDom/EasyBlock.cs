using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public sealed class EasyBlock
	{
		private readonly CodeStatementCollection csc;

		public EasyBlock(CodeStatementCollection csc)
		{
			this.csc = csc;
		}

		public CodeMethodReturnStatement Return(CodeExpression expr)
		{
			CodeMethodReturnStatement st = new CodeMethodReturnStatement(expr);
			this.csc.Add(st);
			return st;
		}

		public CodeAssignStatement Assign(CodeExpression lhs, CodeExpression rhs)
		{
			CodeAssignStatement st = new CodeAssignStatement(lhs, rhs);
			this.csc.Add(st);
			return st;
		}

		public CodeExpressionStatement Add(CodeExpression expr)
		{
			CodeExpressionStatement st = new CodeExpressionStatement(expr);
			this.csc.Add(st);
			return st;
		}

		public CodeStatement Add(CodeStatement st)
		{
			this.csc.Add(st);
			return st;
		}

		public CodeExpressionStatement InvokeMethod(CodeExpression target, string name, params CodeExpression[] arguments)
		{
			return this.Add(new CodeMethodInvokeExpression(target, name, arguments));
		}

		public CodeVariableDeclarationStatement DeclareVariable(Type type, string name)
		{
			return this.DeclareVariable(Easy.TypeRef(type), name);
		}

		public CodeVariableDeclarationStatement DeclareVariable(CodeTypeReference type, string name)
		{
			CodeVariableDeclarationStatement st = new CodeVariableDeclarationStatement(type, name);
			this.csc.Add(st);
			return st;
		}
	}
}

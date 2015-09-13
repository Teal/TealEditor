using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public sealed class EasyExpression
	{
		private readonly CodeExpression expr;

		public EasyExpression(CodeExpression expr)
		{
			this.expr = expr;
		}

		public static implicit operator CodeExpression(EasyExpression expr)
		{
			return expr.expr;
		}

		public EasyExpression InvokeMethod(string name, params CodeExpression[] arguments)
		{
			return new EasyExpression(new CodeMethodInvokeExpression(this.expr, name, arguments));
		}

		public EasyExpression CastTo(Type type)
		{
			return this.CastTo(Easy.TypeRef(type));
		}

		public EasyExpression CastTo(CodeTypeReference type)
		{
			return new EasyExpression(new CodeCastExpression(type, this.expr));
		}

		public EasyExpression Index(params CodeExpression[] indices)
		{
			return new EasyExpression(new CodeIndexerExpression(this.expr, indices));
		}

		public EasyExpression Field(string name)
		{
			return new EasyExpression(new CodeFieldReferenceExpression(this.expr, name));
		}

		public EasyExpression Property(string name)
		{
			return new EasyExpression(new CodePropertyReferenceExpression(this.expr, name));
		}
	}
}

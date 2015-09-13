using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public class EasyMethod : CodeMemberMethod
	{
		private EasyBlock body;

		public EasyBlock Body
		{
			get
			{
				return this.body;
			}
		}

		public EasyMethod()
		{
			this.body = new EasyBlock(base.Statements);
		}

		public EasyMethod(CodeTypeReference type, string name) : this()
		{
			base.ReturnType = type;
			base.Name = name;
		}

		public CodeAttributeDeclaration AddAttribute(Type type, params CodeExpression[] arguments)
		{
			return Easy.AddAttribute(base.CustomAttributes, Easy.TypeRef(type), arguments);
		}

		public CodeAttributeDeclaration AddAttribute(CodeTypeReference type, params CodeExpression[] arguments)
		{
			return Easy.AddAttribute(base.CustomAttributes, type, arguments);
		}

		public CodeParameterDeclarationExpression AddParameter(Type type, string name)
		{
			return this.AddParameter(Easy.TypeRef(type), name);
		}

		public CodeParameterDeclarationExpression AddParameter(CodeTypeReference type, string name)
		{
			CodeParameterDeclarationExpression cpde = new CodeParameterDeclarationExpression(type, name);
			base.Parameters.Add(cpde);
			return cpde;
		}
	}
}

using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public class EasyField : CodeMemberField
	{
		public EasyField()
		{
		}

		public EasyField(CodeTypeReference type, string name) : base(type, name)
		{
		}

		public CodeAttributeDeclaration AddAttribute(Type type, params CodeExpression[] arguments)
		{
			return Easy.AddAttribute(base.CustomAttributes, Easy.TypeRef(type), arguments);
		}

		public CodeAttributeDeclaration AddAttribute(CodeTypeReference type, params CodeExpression[] arguments)
		{
			return Easy.AddAttribute(base.CustomAttributes, type, arguments);
		}
	}
}

using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public class EasyProperty : CodeMemberProperty
	{
		private EasyBlock getter;

		private EasyBlock setter;

		public EasyBlock Getter
		{
			get
			{
				return this.getter;
			}
		}

		public EasyBlock Setter
		{
			get
			{
				return this.setter;
			}
		}

		public EasyProperty()
		{
			this.getter = new EasyBlock(base.GetStatements);
			this.setter = new EasyBlock(base.SetStatements);
		}

		public EasyProperty(CodeTypeReference type, string name) : this()
		{
			base.Type = type;
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
	}
}

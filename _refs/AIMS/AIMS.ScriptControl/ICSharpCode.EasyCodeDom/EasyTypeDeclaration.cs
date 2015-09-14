using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public class EasyTypeDeclaration : CodeTypeDeclaration
	{
		public EasyTypeDeclaration()
		{
		}

		public EasyTypeDeclaration(string name) : base(name)
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

		public EasyField AddField(Type type, string name)
		{
			return this.AddField(Easy.TypeRef(type), name);
		}

		public EasyField AddField(CodeTypeReference type, string name)
		{
			EasyField f = new EasyField(type, name);
			base.Members.Add(f);
			return f;
		}

		public EasyProperty AddProperty(Type type, string name)
		{
			return this.AddProperty(Easy.TypeRef(type), name);
		}

		public EasyProperty AddProperty(CodeTypeReference type, string name)
		{
			EasyProperty p = new EasyProperty(type, name);
			base.Members.Add(p);
			if (!base.IsInterface)
			{
				p.Attributes = (MemberAttributes)24578;
			}
			return p;
		}

		public EasyProperty AddProperty(CodeMemberField field, string name)
		{
			EasyProperty p = this.AddProperty(field.Type, name);
			p.Getter.Return(new CodeVariableReferenceExpression(field.Name));
			p.Attributes |= (field.Attributes & MemberAttributes.Static);
			return p;
		}

		public EasyMethod AddMethod(string name)
		{
			return this.AddMethod(Easy.TypeRef(typeof(void)), name);
		}

		public EasyMethod AddMethod(Type type, string name)
		{
			return this.AddMethod(Easy.TypeRef(type), name);
		}

		public EasyMethod AddMethod(CodeTypeReference type, string name)
		{
			EasyMethod p = new EasyMethod(type, name);
			base.Members.Add(p);
			if (!base.IsInterface)
			{
				p.Attributes = (MemberAttributes)24578;
			}
			return p;
		}
	}
}

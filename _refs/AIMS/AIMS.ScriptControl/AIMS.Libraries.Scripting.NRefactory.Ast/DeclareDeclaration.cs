using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class DeclareDeclaration : ParametrizedNode
	{
		private string alias;

		private string library;

		private CharsetModifier charset;

		private TypeReference typeReference;

		public string Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.alias = (value ?? "");
			}
		}

		public string Library
		{
			get
			{
				return this.library;
			}
			set
			{
				this.library = (value ?? "");
			}
		}

		public CharsetModifier Charset
		{
			get
			{
				return this.charset;
			}
			set
			{
				this.charset = value;
			}
		}

		public TypeReference TypeReference
		{
			get
			{
				return this.typeReference;
			}
			set
			{
				this.typeReference = (value ?? TypeReference.Null);
			}
		}

		public DeclareDeclaration(string name, Modifiers modifier, TypeReference typeReference, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes, string library, string alias, CharsetModifier charset) : base(modifier, attributes, name, parameters)
		{
			this.TypeReference = typeReference;
			this.Library = library;
			this.Alias = alias;
			this.Charset = charset;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitDeclareDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[DeclareDeclaration Alias={0} Library={1} Charset={2} TypeReference={3} Name={4} Parameters={5} Attributes={6} Modifier={7}]", new object[]
			{
				this.Alias,
				this.Library,
				this.Charset,
				this.TypeReference,
				base.Name,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}

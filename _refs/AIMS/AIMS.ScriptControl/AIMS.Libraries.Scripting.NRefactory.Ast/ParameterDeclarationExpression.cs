using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ParameterDeclarationExpression : Expression
	{
		private List<AttributeSection> attributes;

		private string parameterName;

		private TypeReference typeReference;

		private ParameterModifiers paramModifier;

		private Expression defaultValue;

		public List<AttributeSection> Attributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = (value ?? new List<AttributeSection>());
			}
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
			set
			{
				this.parameterName = (string.IsNullOrEmpty(value) ? "?" : value);
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

		public ParameterModifiers ParamModifier
		{
			get
			{
				return this.paramModifier;
			}
			set
			{
				this.paramModifier = value;
			}
		}

		public Expression DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = (value ?? Expression.Null);
				if (!this.defaultValue.IsNull)
				{
					this.defaultValue.Parent = this;
				}
			}
		}

		public ParameterDeclarationExpression(TypeReference typeReference, string parameterName)
		{
			this.TypeReference = typeReference;
			this.ParameterName = parameterName;
			this.attributes = new List<AttributeSection>();
			this.defaultValue = Expression.Null;
		}

		public ParameterDeclarationExpression(TypeReference typeReference, string parameterName, ParameterModifiers paramModifier)
		{
			this.TypeReference = typeReference;
			this.ParameterName = parameterName;
			this.ParamModifier = paramModifier;
			this.attributes = new List<AttributeSection>();
			this.defaultValue = Expression.Null;
		}

		public ParameterDeclarationExpression(TypeReference typeReference, string parameterName, ParameterModifiers paramModifier, Expression defaultValue)
		{
			this.TypeReference = typeReference;
			this.ParameterName = parameterName;
			this.ParamModifier = paramModifier;
			this.DefaultValue = defaultValue;
			this.attributes = new List<AttributeSection>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitParameterDeclarationExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ParameterDeclarationExpression Attributes={0} ParameterName={1} TypeReference={2} ParamModifier={3} DefaultValue={4}]", new object[]
			{
				AbstractNode.GetCollectionString(this.Attributes),
				this.ParameterName,
				this.TypeReference,
				this.ParamModifier,
				this.DefaultValue
			});
		}
	}
}

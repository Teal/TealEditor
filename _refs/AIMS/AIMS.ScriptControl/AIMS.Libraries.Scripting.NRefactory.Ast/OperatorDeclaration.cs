using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class OperatorDeclaration : MethodDeclaration
	{
		private ConversionType conversionType;

		private List<AttributeSection> returnTypeAttributes;

		private OverloadableOperatorType overloadableOperator;

		public ConversionType ConversionType
		{
			get
			{
				return this.conversionType;
			}
			set
			{
				this.conversionType = value;
			}
		}

		public List<AttributeSection> ReturnTypeAttributes
		{
			get
			{
				return this.returnTypeAttributes;
			}
			set
			{
				this.returnTypeAttributes = (value ?? new List<AttributeSection>());
			}
		}

		public OverloadableOperatorType OverloadableOperator
		{
			get
			{
				return this.overloadableOperator;
			}
			set
			{
				this.overloadableOperator = value;
			}
		}

		public bool IsConversionOperator
		{
			get
			{
				return this.conversionType != ConversionType.None;
			}
		}

		public OperatorDeclaration(Modifiers modifier, List<AttributeSection> attributes, List<ParameterDeclarationExpression> parameters, TypeReference typeReference, ConversionType conversionType) : base(null, modifier, typeReference, parameters, attributes)
		{
			this.ConversionType = conversionType;
			this.returnTypeAttributes = new List<AttributeSection>();
		}

		public OperatorDeclaration(Modifiers modifier, List<AttributeSection> attributes, List<ParameterDeclarationExpression> parameters, TypeReference typeReference, OverloadableOperatorType overloadableOperator) : base(null, modifier, typeReference, parameters, attributes)
		{
			this.OverloadableOperator = overloadableOperator;
			this.returnTypeAttributes = new List<AttributeSection>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitOperatorDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[OperatorDeclaration ConversionType={0} ReturnTypeAttributes={1} OverloadableOperator={2} TypeReference={3} Body={4} HandlesClause={5} InterfaceImplementations={6} Templates={7} Name={8} Parameters={9} Attributes={10} Modifier={11}]", new object[]
			{
				this.ConversionType,
				AbstractNode.GetCollectionString(this.ReturnTypeAttributes),
				this.OverloadableOperator,
				base.TypeReference,
				base.Body,
				AbstractNode.GetCollectionString(base.HandlesClause),
				AbstractNode.GetCollectionString(base.InterfaceImplementations),
				AbstractNode.GetCollectionString(base.Templates),
				base.Name,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}

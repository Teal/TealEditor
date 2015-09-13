using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class IndexerDeclaration : AttributedNode
	{
		private List<ParameterDeclarationExpression> parameters;

		private List<InterfaceImplementation> interfaceImplementations;

		private TypeReference typeReference;

		private Location bodyStart;

		private Location bodyEnd;

		private PropertyGetRegion getRegion;

		private PropertySetRegion setRegion;

		public List<ParameterDeclarationExpression> Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = (value ?? new List<ParameterDeclarationExpression>());
			}
		}

		public List<InterfaceImplementation> InterfaceImplementations
		{
			get
			{
				return this.interfaceImplementations;
			}
			set
			{
				this.interfaceImplementations = (value ?? new List<InterfaceImplementation>());
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

		public Location BodyStart
		{
			get
			{
				return this.bodyStart;
			}
			set
			{
				this.bodyStart = value;
			}
		}

		public Location BodyEnd
		{
			get
			{
				return this.bodyEnd;
			}
			set
			{
				this.bodyEnd = value;
			}
		}

		public PropertyGetRegion GetRegion
		{
			get
			{
				return this.getRegion;
			}
			set
			{
				this.getRegion = (value ?? PropertyGetRegion.Null);
			}
		}

		public PropertySetRegion SetRegion
		{
			get
			{
				return this.setRegion;
			}
			set
			{
				this.setRegion = (value ?? PropertySetRegion.Null);
			}
		}

		public bool HasSetRegion
		{
			get
			{
				return !this.setRegion.IsNull;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.HasGetRegion && !this.HasSetRegion;
			}
		}

		public bool IsWriteOnly
		{
			get
			{
				return !this.HasGetRegion && this.HasSetRegion;
			}
		}

		public bool HasGetRegion
		{
			get
			{
				return !this.getRegion.IsNull;
			}
		}

		public IndexerDeclaration(Modifiers modifier, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes) : base(attributes)
		{
			base.Modifier = modifier;
			this.Parameters = parameters;
			this.interfaceImplementations = new List<InterfaceImplementation>();
			this.typeReference = TypeReference.Null;
			this.bodyStart = Location.Empty;
			this.bodyEnd = Location.Empty;
			this.getRegion = PropertyGetRegion.Null;
			this.setRegion = PropertySetRegion.Null;
		}

		public IndexerDeclaration(TypeReference typeReference, List<ParameterDeclarationExpression> parameters, Modifiers modifier, List<AttributeSection> attributes) : base(attributes)
		{
			this.TypeReference = typeReference;
			this.Parameters = parameters;
			base.Modifier = modifier;
			this.interfaceImplementations = new List<InterfaceImplementation>();
			this.bodyStart = Location.Empty;
			this.bodyEnd = Location.Empty;
			this.getRegion = PropertyGetRegion.Null;
			this.setRegion = PropertySetRegion.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitIndexerDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[IndexerDeclaration Parameters={0} InterfaceImplementations={1} TypeReference={2} BodyStart={3} BodyEnd={4} GetRegion={5} SetRegion={6} Attributes={7} Modifier={8}]", new object[]
			{
				AbstractNode.GetCollectionString(this.Parameters),
				AbstractNode.GetCollectionString(this.InterfaceImplementations),
				this.TypeReference,
				this.BodyStart,
				this.BodyEnd,
				this.GetRegion,
				this.SetRegion,
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}

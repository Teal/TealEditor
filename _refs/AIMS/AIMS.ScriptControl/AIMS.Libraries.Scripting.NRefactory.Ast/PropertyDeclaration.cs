using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class PropertyDeclaration : ParametrizedNode
	{
		private List<InterfaceImplementation> interfaceImplementations;

		private TypeReference typeReference;

		private Location bodyStart;

		private Location bodyEnd;

		private PropertyGetRegion getRegion;

		private PropertySetRegion setRegion;

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

		public bool HasGetRegion
		{
			get
			{
				return !this.getRegion.IsNull;
			}
		}

		public bool IsWriteOnly
		{
			get
			{
				return !this.HasGetRegion && this.HasSetRegion;
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

		public PropertyDeclaration(Modifiers modifier, List<AttributeSection> attributes, string name, List<ParameterDeclarationExpression> parameters) : base(modifier, attributes, name, parameters)
		{
			this.interfaceImplementations = new List<InterfaceImplementation>();
			this.typeReference = TypeReference.Null;
			this.bodyStart = Location.Empty;
			this.bodyEnd = Location.Empty;
			this.getRegion = PropertyGetRegion.Null;
			this.setRegion = PropertySetRegion.Null;
		}

		internal PropertyDeclaration(string name, TypeReference typeReference, Modifiers modifier, List<AttributeSection> attributes) : this(modifier, attributes, name, null)
		{
			this.TypeReference = typeReference;
			if ((modifier & Modifiers.ReadOnly) != Modifiers.ReadOnly)
			{
				this.SetRegion = new PropertySetRegion(null, null);
			}
			if ((modifier & Modifiers.WriteOnly) != Modifiers.WriteOnly)
			{
				this.GetRegion = new PropertyGetRegion(null, null);
			}
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitPropertyDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[PropertyDeclaration InterfaceImplementations={0} TypeReference={1} BodyStart={2} BodyEnd={3} GetRegion={4} SetRegion={5} Name={6} Parameters={7} Attributes={8} Modifier={9}]", new object[]
			{
				AbstractNode.GetCollectionString(this.InterfaceImplementations),
				this.TypeReference,
				this.BodyStart,
				this.BodyEnd,
				this.GetRegion,
				this.SetRegion,
				base.Name,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}

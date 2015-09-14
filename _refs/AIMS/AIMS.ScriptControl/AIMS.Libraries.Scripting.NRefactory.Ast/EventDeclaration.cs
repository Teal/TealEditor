using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class EventDeclaration : ParametrizedNode
	{
		private TypeReference typeReference;

		private List<InterfaceImplementation> interfaceImplementations;

		private EventAddRegion addRegion;

		private EventRemoveRegion removeRegion;

		private EventRaiseRegion raiseRegion;

		private Location bodyStart;

		private Location bodyEnd;

		private Expression initializer;

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

		public EventAddRegion AddRegion
		{
			get
			{
				return this.addRegion;
			}
			set
			{
				this.addRegion = (value ?? EventAddRegion.Null);
			}
		}

		public EventRemoveRegion RemoveRegion
		{
			get
			{
				return this.removeRegion;
			}
			set
			{
				this.removeRegion = (value ?? EventRemoveRegion.Null);
			}
		}

		public EventRaiseRegion RaiseRegion
		{
			get
			{
				return this.raiseRegion;
			}
			set
			{
				this.raiseRegion = (value ?? EventRaiseRegion.Null);
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

		public Expression Initializer
		{
			get
			{
				return this.initializer;
			}
			set
			{
				this.initializer = (value ?? Expression.Null);
				if (!this.initializer.IsNull)
				{
					this.initializer.Parent = this;
				}
			}
		}

		public bool HasRemoveRegion
		{
			get
			{
				return !this.removeRegion.IsNull;
			}
		}

		public bool HasRaiseRegion
		{
			get
			{
				return !this.raiseRegion.IsNull;
			}
		}

		public bool HasAddRegion
		{
			get
			{
				return !this.addRegion.IsNull;
			}
		}

		public EventDeclaration(TypeReference typeReference, string name, Modifiers modifier, List<AttributeSection> attributes, List<ParameterDeclarationExpression> parameters) : base(modifier, attributes, name, parameters)
		{
			this.TypeReference = typeReference;
			this.interfaceImplementations = new List<InterfaceImplementation>();
			this.addRegion = EventAddRegion.Null;
			this.removeRegion = EventRemoveRegion.Null;
			this.raiseRegion = EventRaiseRegion.Null;
			this.bodyStart = Location.Empty;
			this.bodyEnd = Location.Empty;
			this.initializer = Expression.Null;
		}

		public EventDeclaration(TypeReference typeReference, Modifiers modifier, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes, string name, List<InterfaceImplementation> interfaceImplementations) : base(modifier, attributes, name, parameters)
		{
			this.TypeReference = typeReference;
			this.InterfaceImplementations = interfaceImplementations;
			this.addRegion = EventAddRegion.Null;
			this.removeRegion = EventRemoveRegion.Null;
			this.raiseRegion = EventRaiseRegion.Null;
			this.bodyStart = Location.Empty;
			this.bodyEnd = Location.Empty;
			this.initializer = Expression.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitEventDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[EventDeclaration TypeReference={0} InterfaceImplementations={1} AddRegion={2} RemoveRegion={3} RaiseRegion={4} BodyStart={5} BodyEnd={6} Initializer={7} Name={8} Parameters={9} Attributes={10} Modifier={11}]", new object[]
			{
				this.TypeReference,
				AbstractNode.GetCollectionString(this.InterfaceImplementations),
				this.AddRegion,
				this.RemoveRegion,
				this.RaiseRegion,
				this.BodyStart,
				this.BodyEnd,
				this.Initializer,
				base.Name,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}

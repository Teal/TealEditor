using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public abstract class AbstractMember : AbstractNamedEntity, IMember, IDecoration, IComparable, ICloneable
	{
		private IReturnType returnType;

		private DomRegion region;

		private DomRegion bodyRegion;

		private List<ExplicitInterfaceImplementation> interfaceImplementations;

		private IReturnType declaringTypeReference;

		public virtual DomRegion Region
		{
			get
			{
				return this.region;
			}
			set
			{
				this.region = value;
			}
		}

		public virtual DomRegion BodyRegion
		{
			get
			{
				return this.bodyRegion;
			}
			protected set
			{
				this.bodyRegion = value;
			}
		}

		public virtual IReturnType ReturnType
		{
			get
			{
				return this.returnType;
			}
			set
			{
				this.returnType = value;
			}
		}

		public virtual IReturnType DeclaringTypeReference
		{
			get
			{
				return this.declaringTypeReference ?? base.DeclaringType.DefaultReturnType;
			}
			set
			{
				this.declaringTypeReference = value;
			}
		}

		public IList<ExplicitInterfaceImplementation> InterfaceImplementations
		{
			get
			{
				List<ExplicitInterfaceImplementation> arg_19_0;
				if ((arg_19_0 = this.interfaceImplementations) == null)
				{
					arg_19_0 = (this.interfaceImplementations = new List<ExplicitInterfaceImplementation>());
				}
				return arg_19_0;
			}
		}

		public AbstractMember(IClass declaringType, string name) : base(declaringType, name)
		{
		}

		public abstract IMember Clone();

		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}
}

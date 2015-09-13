using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultEvent : AbstractMember, IEvent, IMember, IDecoration, IComparable, ICloneable
	{
		private IMethod addMethod;

		private IMethod removeMethod;

		private IMethod raiseMethod;

		public override string DocumentationTag
		{
			get
			{
				return "E:" + this.DotNetName;
			}
		}

		public virtual IMethod AddMethod
		{
			get
			{
				return this.addMethod;
			}
			protected set
			{
				this.addMethod = value;
			}
		}

		public virtual IMethod RemoveMethod
		{
			get
			{
				return this.removeMethod;
			}
			protected set
			{
				this.removeMethod = value;
			}
		}

		public virtual IMethod RaiseMethod
		{
			get
			{
				return this.raiseMethod;
			}
			protected set
			{
				this.raiseMethod = value;
			}
		}

		public override IMember Clone()
		{
			DefaultEvent de = new DefaultEvent(base.Name, this.ReturnType, base.Modifiers, this.Region, this.BodyRegion, base.DeclaringType);
			foreach (ExplicitInterfaceImplementation eii in base.InterfaceImplementations)
			{
				de.InterfaceImplementations.Add(eii.Clone());
			}
			return de;
		}

		public DefaultEvent(IClass declaringType, string name) : base(declaringType, name)
		{
		}

		public DefaultEvent(string name, IReturnType type, ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region = region;
			this.BodyRegion = bodyRegion;
			base.Modifiers = m;
			if (base.Modifiers == ModifierEnum.None)
			{
				base.Modifiers = ModifierEnum.Private;
			}
		}

		public virtual int CompareTo(IEvent value)
		{
			int cmp;
			int result;
			if (0 != (cmp = base.CompareTo(value)))
			{
				result = cmp;
			}
			else if (base.FullyQualifiedName != null)
			{
				result = base.FullyQualifiedName.CompareTo(value.FullyQualifiedName);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		int IComparable.CompareTo(object value)
		{
			return this.CompareTo((IEvent)value);
		}
	}
}

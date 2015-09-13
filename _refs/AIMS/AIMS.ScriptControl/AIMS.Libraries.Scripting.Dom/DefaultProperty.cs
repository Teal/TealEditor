using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultProperty : AbstractMember, IProperty, IMethodOrProperty, IMember, IDecoration, IComparable, ICloneable
	{
		private const byte indexerFlag = 1;

		private const byte getterFlag = 2;

		private const byte setterFlag = 4;

		private const byte extensionFlag = 8;

		private DomRegion getterRegion = DomRegion.Empty;

		private DomRegion setterRegion = DomRegion.Empty;

		private IList<IParameter> parameters = null;

		internal byte accessFlags;

		public bool IsIndexer
		{
			get
			{
				return (this.accessFlags & 1) == 1;
			}
			set
			{
				if (value)
				{
					this.accessFlags |= 1;
				}
				else
				{
					this.accessFlags &= 254;
				}
			}
		}

		public bool CanGet
		{
			get
			{
				return (this.accessFlags & 2) == 2;
			}
			set
			{
				if (value)
				{
					this.accessFlags |= 2;
				}
				else
				{
					this.accessFlags &= 253;
				}
			}
		}

		public bool CanSet
		{
			get
			{
				return (this.accessFlags & 4) == 4;
			}
			set
			{
				if (value)
				{
					this.accessFlags |= 4;
				}
				else
				{
					this.accessFlags &= 251;
				}
			}
		}

		public bool IsExtensionMethod
		{
			get
			{
				return (this.accessFlags & 8) == 8;
			}
			set
			{
				if (value)
				{
					this.accessFlags |= 8;
				}
				else
				{
					this.accessFlags &= 247;
				}
			}
		}

		public override string DocumentationTag
		{
			get
			{
				return "P:" + this.DotNetName;
			}
		}

		public virtual IList<IParameter> Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new List<IParameter>();
				}
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		public DomRegion GetterRegion
		{
			get
			{
				return this.getterRegion;
			}
			set
			{
				this.getterRegion = value;
			}
		}

		public DomRegion SetterRegion
		{
			get
			{
				return this.setterRegion;
			}
			set
			{
				this.setterRegion = value;
			}
		}

		public override IMember Clone()
		{
			DefaultProperty p = new DefaultProperty(base.Name, this.ReturnType, base.Modifiers, this.Region, this.BodyRegion, base.DeclaringType);
			p.parameters = DefaultParameter.Clone(this.Parameters);
			p.accessFlags = this.accessFlags;
			foreach (ExplicitInterfaceImplementation eii in base.InterfaceImplementations)
			{
				p.InterfaceImplementations.Add(eii.Clone());
			}
			return p;
		}

		public DefaultProperty(IClass declaringType, string name) : base(declaringType, name)
		{
		}

		public DefaultProperty(string name, IReturnType type, ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region = region;
			this.BodyRegion = bodyRegion;
			base.Modifiers = m;
		}

		public virtual int CompareTo(IProperty value)
		{
			int result;
			if (base.FullyQualifiedName != null)
			{
				int cmp = base.FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0)
				{
					result = cmp;
					return result;
				}
			}
			result = DiffUtility.Compare<IParameter>(this.Parameters, value.Parameters);
			return result;
		}

		int IComparable.CompareTo(object value)
		{
			return this.CompareTo((IProperty)value);
		}
	}
}

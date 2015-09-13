using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultField : AbstractMember, IField, IMember, IDecoration, IComparable, ICloneable
	{
		public class LocalVariableField : DefaultField
		{
			public override bool IsLocalVariable
			{
				get
				{
					return true;
				}
			}

			public LocalVariableField(IReturnType type, string name, DomRegion region, IClass callingClass) : base(type, name, ModifierEnum.None, region, callingClass)
			{
			}
		}

		public class ParameterField : DefaultField
		{
			public override bool IsParameter
			{
				get
				{
					return true;
				}
			}

			public ParameterField(IReturnType type, string name, DomRegion region, IClass callingClass) : base(type, name, ModifierEnum.None, region, callingClass)
			{
			}
		}

		public override string DocumentationTag
		{
			get
			{
				return "F:" + this.DotNetName;
			}
		}

		public virtual bool IsLocalVariable
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsParameter
		{
			get
			{
				return false;
			}
		}

		public DefaultField(IClass declaringType, string name) : base(declaringType, name)
		{
		}

		public DefaultField(IReturnType type, string name, ModifierEnum m, DomRegion region, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region = region;
			base.Modifiers = m;
		}

		public override IMember Clone()
		{
			return new DefaultField(this.ReturnType, base.Name, base.Modifiers, this.Region, base.DeclaringType);
		}

		public virtual int CompareTo(IField field)
		{
			int cmp = base.CompareTo(field);
			int result;
			if (cmp != 0)
			{
				result = cmp;
			}
			else if (base.FullyQualifiedName != null)
			{
				result = base.FullyQualifiedName.CompareTo(field.FullyQualifiedName);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		int IComparable.CompareTo(object value)
		{
			return this.CompareTo((IField)value);
		}
	}
}

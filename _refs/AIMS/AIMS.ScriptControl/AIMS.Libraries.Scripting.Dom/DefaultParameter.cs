using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultParameter : IParameter, IComparable
	{
		public static readonly IList<IParameter> EmptyParameterList = new List<IParameter>().AsReadOnly();

		private string name;

		private string documentation;

		private IReturnType returnType;

		private ParameterModifiers modifier;

		private DomRegion region;

		private IList<IAttribute> attributes;

		public DomRegion Region
		{
			get
			{
				return this.region;
			}
		}

		public bool IsOut
		{
			get
			{
				return (byte)(this.modifier & ParameterModifiers.Out) == 2;
			}
		}

		public bool IsRef
		{
			get
			{
				return (byte)(this.modifier & ParameterModifiers.Ref) == 4;
			}
		}

		public bool IsParams
		{
			get
			{
				return (byte)(this.modifier & ParameterModifiers.Params) == 8;
			}
		}

		public bool IsOptional
		{
			get
			{
				return (byte)(this.modifier & ParameterModifiers.Optional) == 16;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
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

		public virtual IList<IAttribute> Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new List<IAttribute>();
				}
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		public virtual ParameterModifiers Modifiers
		{
			get
			{
				return this.modifier;
			}
			set
			{
				this.modifier = value;
			}
		}

		public string Documentation
		{
			get
			{
				return this.documentation;
			}
			set
			{
				this.documentation = value;
			}
		}

		protected DefaultParameter(string name)
		{
			this.Name = name;
		}

		public DefaultParameter(IParameter p)
		{
			this.name = p.Name;
			this.region = p.Region;
			this.modifier = p.Modifiers;
			this.returnType = p.ReturnType;
		}

		public DefaultParameter(string name, IReturnType type, DomRegion region) : this(name)
		{
			this.returnType = type;
			this.region = region;
		}

		public static List<IParameter> Clone(IList<IParameter> l)
		{
			List<IParameter> r = new List<IParameter>(l.Count);
			for (int i = 0; i < l.Count; i++)
			{
				r.Add(new DefaultParameter(l[i]));
			}
			return r;
		}

		public virtual int CompareTo(IParameter value)
		{
			int result;
			if (value == null)
			{
				result = -1;
			}
			else if (object.Equals(this.ReturnType, value.ReturnType))
			{
				result = 0;
			}
			else
			{
				int r = string.Compare(this.Name, value.Name);
				if (r != 0)
				{
					result = r;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		int IComparable.CompareTo(object value)
		{
			return this.CompareTo(value as IParameter);
		}
	}
}

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public abstract class AbstractReturnType : IReturnType
	{
		private string fullyQualifiedName = null;

		public virtual int TypeParameterCount
		{
			get
			{
				return 0;
			}
		}

		public virtual string FullyQualifiedName
		{
			get
			{
				string empty;
				if (this.fullyQualifiedName == null)
				{
					empty = string.Empty;
				}
				else
				{
					empty = this.fullyQualifiedName;
				}
				return empty;
			}
			set
			{
				this.fullyQualifiedName = value;
			}
		}

		public virtual string Name
		{
			get
			{
				string result;
				if (this.FullyQualifiedName == null)
				{
					result = null;
				}
				else
				{
					int index = this.FullyQualifiedName.LastIndexOf('.');
					result = ((index < 0) ? this.FullyQualifiedName : this.FullyQualifiedName.Substring(index + 1));
				}
				return result;
			}
		}

		public virtual string Namespace
		{
			get
			{
				string result;
				if (this.FullyQualifiedName == null)
				{
					result = null;
				}
				else
				{
					int index = this.FullyQualifiedName.LastIndexOf('.');
					result = ((index < 0) ? string.Empty : this.FullyQualifiedName.Substring(0, index));
				}
				return result;
			}
		}

		public virtual string DotNetName
		{
			get
			{
				return this.FullyQualifiedName;
			}
		}

		public virtual bool IsDefaultReturnType
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsArrayReturnType
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGenericReturnType
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsConstructedReturnType
		{
			get
			{
				return false;
			}
		}

		public abstract IClass GetUnderlyingClass();

		public abstract List<IMethod> GetMethods();

		public abstract List<IProperty> GetProperties();

		public abstract List<IField> GetFields();

		public abstract List<IEvent> GetEvents();

		public override bool Equals(object o)
		{
			IReturnType rt = o as IReturnType;
			return rt != null && rt.IsDefaultReturnType && DefaultReturnType.Equals(this, rt);
		}

		public override int GetHashCode()
		{
			return this.fullyQualifiedName.GetHashCode();
		}

		public virtual ArrayReturnType CastToArrayReturnType()
		{
			throw new InvalidCastException("Cannot cast " + this.ToString() + " to expected type.");
		}

		public virtual GenericReturnType CastToGenericReturnType()
		{
			throw new InvalidCastException("Cannot cast " + this.ToString() + " to expected type.");
		}

		public virtual ConstructedReturnType CastToConstructedReturnType()
		{
			throw new InvalidCastException("Cannot cast " + this.ToString() + " to expected type.");
		}
	}
}

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public abstract class ProxyReturnType : IReturnType
	{
		private bool busy = false;

		public abstract IReturnType BaseType
		{
			get;
		}

		public virtual string FullyQualifiedName
		{
			get
			{
				IReturnType baseType = this.BaseType;
				string tmp = (baseType != null && this.TryEnter()) ? baseType.FullyQualifiedName : "?";
				this.busy = false;
				return tmp;
			}
		}

		public virtual string Name
		{
			get
			{
				IReturnType baseType = this.BaseType;
				string tmp = (baseType != null && this.TryEnter()) ? baseType.Name : "?";
				this.busy = false;
				return tmp;
			}
		}

		public virtual string Namespace
		{
			get
			{
				IReturnType baseType = this.BaseType;
				string tmp = (baseType != null && this.TryEnter()) ? baseType.Namespace : "?";
				this.busy = false;
				return tmp;
			}
		}

		public virtual string DotNetName
		{
			get
			{
				IReturnType baseType = this.BaseType;
				string tmp = (baseType != null && this.TryEnter()) ? baseType.DotNetName : "?";
				this.busy = false;
				return tmp;
			}
		}

		public virtual int TypeParameterCount
		{
			get
			{
				IReturnType baseType = this.BaseType;
				int tmp = (baseType != null && this.TryEnter()) ? baseType.TypeParameterCount : 0;
				this.busy = false;
				return tmp;
			}
		}

		public virtual bool IsDefaultReturnType
		{
			get
			{
				IReturnType baseType = this.BaseType;
				bool tmp = baseType != null && this.TryEnter() && baseType.IsDefaultReturnType;
				this.busy = false;
				return tmp;
			}
		}

		public virtual bool IsArrayReturnType
		{
			get
			{
				IReturnType baseType = this.BaseType;
				bool tmp = baseType != null && this.TryEnter() && baseType.IsArrayReturnType;
				this.busy = false;
				return tmp;
			}
		}

		public virtual bool IsGenericReturnType
		{
			get
			{
				IReturnType baseType = this.BaseType;
				bool tmp = baseType != null && this.TryEnter() && baseType.IsGenericReturnType;
				this.busy = false;
				return tmp;
			}
		}

		public virtual bool IsConstructedReturnType
		{
			get
			{
				IReturnType baseType = this.BaseType;
				bool tmp = baseType != null && this.TryEnter() && baseType.IsConstructedReturnType;
				this.busy = false;
				return tmp;
			}
		}

		private bool TryEnter()
		{
			bool result;
			if (this.busy)
			{
				this.PrintTryEnterWarning();
				result = false;
			}
			else
			{
				this.busy = true;
				result = true;
			}
			return result;
		}

		private void PrintTryEnterWarning()
		{
			LoggingService.Info("TryEnter failed on " + this.ToString());
		}

		public virtual IClass GetUnderlyingClass()
		{
			IReturnType baseType = this.BaseType;
			IClass tmp = (baseType != null && this.TryEnter()) ? baseType.GetUnderlyingClass() : null;
			this.busy = false;
			return tmp;
		}

		public virtual List<IMethod> GetMethods()
		{
			IReturnType baseType = this.BaseType;
			List<IMethod> tmp = (baseType != null && this.TryEnter()) ? baseType.GetMethods() : new List<IMethod>();
			this.busy = false;
			return tmp;
		}

		public virtual List<IProperty> GetProperties()
		{
			IReturnType baseType = this.BaseType;
			List<IProperty> tmp = (baseType != null && this.TryEnter()) ? baseType.GetProperties() : new List<IProperty>();
			this.busy = false;
			return tmp;
		}

		public virtual List<IField> GetFields()
		{
			IReturnType baseType = this.BaseType;
			List<IField> tmp = (baseType != null && this.TryEnter()) ? baseType.GetFields() : new List<IField>();
			this.busy = false;
			return tmp;
		}

		public virtual List<IEvent> GetEvents()
		{
			IReturnType baseType = this.BaseType;
			List<IEvent> tmp = (baseType != null && this.TryEnter()) ? baseType.GetEvents() : new List<IEvent>();
			this.busy = false;
			return tmp;
		}

		public virtual ArrayReturnType CastToArrayReturnType()
		{
			IReturnType baseType = this.BaseType;
			if (baseType != null && this.TryEnter())
			{
				ArrayReturnType temp = baseType.CastToArrayReturnType();
				this.busy = false;
				return temp;
			}
			throw new InvalidCastException("Cannot cast " + this.ToString() + " to expected type.");
		}

		public virtual GenericReturnType CastToGenericReturnType()
		{
			IReturnType baseType = this.BaseType;
			if (baseType != null && this.TryEnter())
			{
				GenericReturnType temp = baseType.CastToGenericReturnType();
				this.busy = false;
				return temp;
			}
			throw new InvalidCastException("Cannot cast " + this.ToString() + " to expected type.");
		}

		public virtual ConstructedReturnType CastToConstructedReturnType()
		{
			IReturnType baseType = this.BaseType;
			if (baseType != null && this.TryEnter())
			{
				ConstructedReturnType temp = baseType.CastToConstructedReturnType();
				this.busy = false;
				return temp;
			}
			throw new InvalidCastException("Cannot cast " + this.ToString() + " to expected type.");
		}
	}
}

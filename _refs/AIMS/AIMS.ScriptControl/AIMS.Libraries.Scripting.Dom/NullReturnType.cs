using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class NullReturnType : AbstractReturnType
	{
		public static readonly NullReturnType Instance = new NullReturnType();

		public override bool IsDefaultReturnType
		{
			get
			{
				return false;
			}
		}

		public override bool Equals(object o)
		{
			return o is NullReturnType;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override IClass GetUnderlyingClass()
		{
			return null;
		}

		public override List<IMethod> GetMethods()
		{
			return new List<IMethod>();
		}

		public override List<IProperty> GetProperties()
		{
			return new List<IProperty>();
		}

		public override List<IField> GetFields()
		{
			return new List<IField>();
		}

		public override List<IEvent> GetEvents()
		{
			return new List<IEvent>();
		}
	}
}

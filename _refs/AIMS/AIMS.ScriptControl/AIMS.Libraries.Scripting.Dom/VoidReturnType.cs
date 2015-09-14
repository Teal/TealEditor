using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class VoidReturnType : AbstractReturnType
	{
		public static readonly VoidReturnType Instance = new VoidReturnType();

		private VoidReturnType()
		{
			this.FullyQualifiedName = VoidClass.VoidName;
		}

		public override IClass GetUnderlyingClass()
		{
			return VoidClass.Instance;
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

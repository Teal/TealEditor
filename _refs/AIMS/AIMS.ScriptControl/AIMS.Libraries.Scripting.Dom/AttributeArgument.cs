using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public struct AttributeArgument
	{
		public readonly IReturnType Type;

		public readonly object Value;

		public AttributeArgument(IReturnType type, object value)
		{
			this.Type = type;
			this.Value = value;
		}
	}
}

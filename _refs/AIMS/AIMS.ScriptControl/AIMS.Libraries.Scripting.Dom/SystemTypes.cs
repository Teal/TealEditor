using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class SystemTypes
	{
		public readonly IReturnType Void = VoidReturnType.Instance;

		public readonly IReturnType Object;

		public readonly IReturnType Delegate;

		public readonly IReturnType ValueType;

		public readonly IReturnType Enum;

		public readonly IReturnType Boolean;

		public readonly IReturnType Int32;

		public readonly IReturnType String;

		public readonly IReturnType Array;

		public readonly IReturnType Attribute;

		public readonly IReturnType Type;

		public readonly IReturnType AsyncCallback;

		public readonly IReturnType IAsyncResult;

		private IProjectContent pc;

		public SystemTypes(IProjectContent pc)
		{
			this.pc = pc;
			this.Object = this.CreateFromName("System.Object");
			this.Delegate = this.CreateFromName("System.Delegate");
			this.ValueType = this.CreateFromName("System.ValueType");
			this.Enum = this.CreateFromName("System.Enum");
			this.Boolean = this.CreateFromName("System.Boolean");
			this.Int32 = this.CreateFromName("System.Int32");
			this.String = this.CreateFromName("System.String");
			this.Array = this.CreateFromName("System.Array");
			this.Attribute = this.CreateFromName("System.Attribute");
			this.Type = this.CreateFromName("System.Type");
			this.AsyncCallback = this.CreateFromName("System.AsyncCallback");
			this.IAsyncResult = this.CreateFromName("System.IAsyncResult");
		}

		private IReturnType CreateFromName(string name)
		{
			IClass c = this.pc.GetClass(name, 0);
			IReturnType result;
			if (c != null)
			{
				result = c.DefaultReturnType;
			}
			else
			{
				LoggingService.Warn("SystemTypes.CreateFromName could not find " + name);
				result = VoidReturnType.Instance;
			}
			return result;
		}

		public IReturnType CreatePrimitive(Type type)
		{
			if (type.HasElementType || type.ContainsGenericParameters)
			{
				throw new ArgumentException("Only primitive types are supported.");
			}
			return this.CreateFromName(type.FullName);
		}
	}
}

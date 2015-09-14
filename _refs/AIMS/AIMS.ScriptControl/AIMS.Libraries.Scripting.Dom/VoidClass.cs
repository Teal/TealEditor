using System;

namespace AIMS.Libraries.Scripting.Dom
{
	internal sealed class VoidClass : DefaultClass
	{
		internal static readonly string VoidName = typeof(void).FullName;

		public static readonly VoidClass Instance = new VoidClass();

		private VoidClass() : base(DefaultCompilationUnit.DummyCompilationUnit, VoidClass.VoidName)
		{
		}

		protected override IReturnType CreateDefaultReturnType()
		{
			return VoidReturnType.Instance;
		}
	}
}

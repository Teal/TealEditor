using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	[Flags]
	public enum ParameterModifiers
	{
		None = 0,
		In = 1,
		Out = 2,
		Ref = 4,
		Params = 8,
		Optional = 16
	}
}

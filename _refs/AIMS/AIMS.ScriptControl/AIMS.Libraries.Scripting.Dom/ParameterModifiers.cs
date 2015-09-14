using System;

namespace AIMS.Libraries.Scripting.Dom
{
	[Flags]
	[Serializable]
	public enum ParameterModifiers : byte
	{
		None = 0,
		In = 1,
		Out = 2,
		Ref = 4,
		Params = 8,
		Optional = 16
	}
}

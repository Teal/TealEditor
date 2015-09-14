using System;

namespace AIMS.Libraries.Scripting.Dom
{
	[Flags]
	public enum ModifierEnum
	{
		None = 0,
		Private = 1,
		Internal = 2,
		Protected = 4,
		Public = 8,
		Dim = 16,
		Abstract = 16,
		Virtual = 32,
		Sealed = 64,
		Static = 128,
		Override = 256,
		Readonly = 512,
		Const = 1024,
		New = 2048,
		Partial = 4096,
		Extern = 8192,
		Volatile = 16384,
		Unsafe = 32768,
		Overloads = 65536,
		WithEvents = 131072,
		Default = 262144,
		Fixed = 524288,
		Synthetic = 2097152,
		ProtectedAndInternal = 6,
		VisibilityMask = 15
	}
}

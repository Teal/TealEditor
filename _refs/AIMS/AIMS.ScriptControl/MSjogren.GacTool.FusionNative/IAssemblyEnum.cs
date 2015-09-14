using System;
using System.Runtime.InteropServices;

namespace MSjogren.GacTool.FusionNative
{
	[Guid("21B8916C-F28E-11D2-A473-00C04F8EF448"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IAssemblyEnum
	{
		[PreserveSig]
		int GetNextAssembly(out IApplicationContext ppAppCtx, out IAssemblyName ppName, uint dwFlags);

		[PreserveSig]
		int Reset();

		[PreserveSig]
		int Clone(out IAssemblyEnum ppEnum);
	}
}

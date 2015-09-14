using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MSjogren.GacTool.FusionNative
{
	[Guid("CD193BC0-B4BC-11D2-9833-00C04FC31D2E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IAssemblyName
	{
		[PreserveSig]
		int Set(uint PropertyId, IntPtr pvProperty, uint cbProperty);

		[PreserveSig]
		int Get(uint PropertyId, IntPtr pvProperty, ref uint pcbProperty);

		[PreserveSig]
		int Finalize();

		[PreserveSig]
		int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szDisplayName, ref uint pccDisplayName, uint dwDisplayFlags);

		[PreserveSig]
		int BindToObject(object refIID, object pAsmBindSink, IApplicationContext pApplicationContext, [MarshalAs(UnmanagedType.LPWStr)] string szCodeBase, long llFlags, int pvReserved, uint cbReserved, out int ppv);

		[PreserveSig]
		int GetName(ref uint lpcwBuffer, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder pwzName);

		[PreserveSig]
		int GetVersion(out uint pdwVersionHi, out uint pdwVersionLow);

		[PreserveSig]
		int IsEqual(IAssemblyName pName, uint dwCmpFlags);

		[PreserveSig]
		int Clone(out IAssemblyName pName);
	}
}

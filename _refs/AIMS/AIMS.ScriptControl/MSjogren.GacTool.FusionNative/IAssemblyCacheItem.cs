using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace MSjogren.GacTool.FusionNative
{
	[Guid("9E3AAEB4-D1CD-11D2-BAB9-00C04F8ECEAE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IAssemblyCacheItem
	{
		void CreateStream([MarshalAs(UnmanagedType.LPWStr)] string pszName, uint dwFormat, uint dwFlags, uint dwMaxSize, out IStream ppStream);

		void IsNameEqual(IAssemblyName pName);

		void Commit(uint dwFlags);

		void MarkAssemblyVisible(uint dwFlags);
	}
}

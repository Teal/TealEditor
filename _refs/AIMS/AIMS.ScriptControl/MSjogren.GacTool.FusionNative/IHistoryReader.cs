using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MSjogren.GacTool.FusionNative
{
	[Guid("1D23DF4D-A1E2-4B8B-93D6-6EA3DC285A54"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IHistoryReader
	{
		[PreserveSig]
		int GetFilePath([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder wzFilePath, ref uint pdwSize);

		[PreserveSig]
		int GetApplicationName([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder wzAppName, ref uint pdwSize);

		[PreserveSig]
		int GetEXEModulePath([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder wzExePath, ref uint pdwSize);

		void GetNumActivations(out uint pdwNumActivations);

		void GetActivationDate(uint dwIdx, out long pftDate);

		[PreserveSig]
		int GetRunTimeVersion(ref long pftActivationDate, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder wzRunTimeVersion, ref uint pdwSize);

		void GetNumAssemblies(ref long pftActivationDate, out uint pdwNumAsms);

		void GetHistoryAssembly(ref long pftActivationDate, uint dwIdx, [MarshalAs(UnmanagedType.IUnknown)] out object ppHistAsm);
	}
}

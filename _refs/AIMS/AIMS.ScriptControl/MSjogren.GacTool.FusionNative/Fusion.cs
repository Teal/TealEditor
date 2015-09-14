using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MSjogren.GacTool.FusionNative
{
	internal class Fusion
	{
		[DllImport("fusion.dll", CharSet = CharSet.Auto)]
		internal static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, uint dwReserved);

		[DllImport("fusion.dll", CharSet = CharSet.Auto)]
		internal static extern int CreateAssemblyEnum(out IAssemblyEnum ppEnum, IApplicationContext pAppCtx, IAssemblyName pName, uint dwFlags, int pvReserved);

		[DllImport("fusion.dll", CharSet = CharSet.Auto)]
		internal static extern int CreateAssemblyNameObject(out IAssemblyName ppName, string szAssemblyName, uint dwFlags, int pvReserved);

		[DllImport("fusion.dll", CharSet = CharSet.Auto)]
		internal static extern int CreateHistoryReader(string wzFilePath, out IHistoryReader ppHistReader);

		[DllImport("fusion.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetHistoryFileDirectory([MarshalAs(UnmanagedType.LPWStr)] StringBuilder wzDir, ref uint pdwSize);

		[DllImport("fusion.dll")]
		internal static extern int NukeDownloadedCache();

		[DllImport("fusion.dll")]
		internal static extern int CreateApplicationContext(out IApplicationContext ppAppContext, uint dw);

		[DllImport("fusion.dll")]
		internal static extern int GetCachePath(uint flags, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder wzDir, ref uint pdwSize);

		public static string GetGacPath()
		{
			StringBuilder b = new StringBuilder(260);
			uint tmp = 260u;
			Fusion.GetCachePath(8u, b, ref tmp);
			return b.ToString();
		}

		[DllImport("shfusion.dll", CharSet = CharSet.Unicode)]
		internal static extern uint PolicyManager(IntPtr hWndParent, string pwzFullyQualifiedAppPath, string pwzAppName, int dwFlags);
	}
}

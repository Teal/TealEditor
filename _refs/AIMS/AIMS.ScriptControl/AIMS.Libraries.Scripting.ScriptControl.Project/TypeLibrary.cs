using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public class TypeLibrary
	{
		private enum RegKind
		{
			Default,
			Register,
			None
		}

		private string name;

		private string description;

		private string path;

		private string guid;

		private string version;

		private string lcid;

		private bool isolated = false;

		public string Guid
		{
			get
			{
				return this.guid;
			}
		}

		public bool Isolated
		{
			get
			{
				return this.isolated;
			}
		}

		public string Lcid
		{
			get
			{
				return this.lcid;
			}
		}

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					this.name = this.GetTypeLibName();
				}
				return this.name;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		public string Version
		{
			get
			{
				return this.version;
			}
		}

		public int VersionMajor
		{
			get
			{
				int result;
				if (this.version == null)
				{
					result = -1;
				}
				else
				{
					string[] ver = this.version.Split(new char[]
					{
						'.'
					});
					result = ((ver.Length == 0) ? -1 : TypeLibrary.GetVersion(ver[0]));
				}
				return result;
			}
		}

		public int VersionMinor
		{
			get
			{
				int result;
				if (this.version == null)
				{
					result = -1;
				}
				else
				{
					string[] ver = this.version.Split(new char[]
					{
						'.'
					});
					result = ((ver.Length < 2) ? -1 : TypeLibrary.GetVersion(ver[1]));
				}
				return result;
			}
		}

		public string WrapperTool
		{
			get
			{
				return "tlbimp";
			}
		}

		public static IEnumerable<TypeLibrary> Libraries
		{
			get
			{
				return new TypeLibrary.<get_Libraries>d__0(-2);
			}
		}

		public static TypeLibrary Create(RegistryKey typeLibKey)
		{
			string[] versions = typeLibKey.GetSubKeyNames();
			TypeLibrary result;
			if (versions.Length > 0)
			{
				TypeLibrary lib = new TypeLibrary();
				lib.version = versions[versions.Length - 1];
				RegistryKey versionKey = typeLibKey.OpenSubKey(lib.version);
				lib.description = (string)versionKey.GetValue(null);
				lib.path = TypeLibrary.GetTypeLibPath(versionKey, ref lib.lcid);
				lib.guid = System.IO.Path.GetFileName(typeLibKey.Name);
				result = lib;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private static string GetTypeLibPath(RegistryKey versionKey, ref string lcid)
		{
			string[] subkeys = versionKey.GetSubKeyNames();
			string result2;
			if (subkeys == null || subkeys.Length == 0)
			{
				result2 = null;
			}
			else
			{
				for (int i = 0; i < subkeys.Length; i++)
				{
					int result;
					if (int.TryParse(subkeys[i], out result))
					{
						lcid = subkeys[i];
						RegistryKey NullKey = versionKey.OpenSubKey(subkeys[i]);
						string[] subsubkeys = NullKey.GetSubKeyNames();
						RegistryKey win32Key = NullKey.OpenSubKey("win32");
						result2 = ((win32Key == null || win32Key.GetValue(null) == null) ? null : TypeLibrary.GetTypeLibPath(win32Key.GetValue(null).ToString()));
						return result2;
					}
				}
				result2 = null;
			}
			return result2;
		}

		private static int GetVersion(string s)
		{
			int version;
			int result;
			if (int.TryParse(s, out version))
			{
				result = version;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private string GetTypeLibName()
		{
			string name = null;
			int typeLibLcid;
			if (this.guid != null && this.lcid != null && int.TryParse(this.lcid, out typeLibLcid))
			{
				Guid typeLibGuid = new Guid(this.guid);
				name = TypeLibrary.GetTypeLibNameFromGuid(ref typeLibGuid, (short)this.VersionMajor, (short)this.VersionMinor, typeLibLcid);
			}
			if (name == null)
			{
				name = TypeLibrary.GetTypeLibNameFromFile(this.path);
			}
			string result;
			if (name != null)
			{
				result = name;
			}
			else
			{
				result = this.description;
			}
			return result;
		}

		private static string GetTypeLibPath(string fileName)
		{
			string result;
			if (fileName != null)
			{
				int index = fileName.LastIndexOf('\\');
				if (index > 0 && index + 1 < fileName.Length)
				{
					if (char.IsDigit(fileName[index + 1]))
					{
						result = fileName.Substring(0, index);
						return result;
					}
				}
			}
			result = fileName;
			return result;
		}

		private static string GetTypeLibNameFromFile(string fileName)
		{
			string result;
			if (fileName != null && fileName.Length > 0 && File.Exists(fileName))
			{
				ITypeLib typeLib;
				if (TypeLibrary.LoadTypeLibEx(fileName, TypeLibrary.RegKind.None, out typeLib) == 0)
				{
					try
					{
						result = Marshal.GetTypeLibName(typeLib);
						return result;
					}
					finally
					{
						Marshal.ReleaseComObject(typeLib);
					}
				}
			}
			result = null;
			return result;
		}

		private static string GetTypeLibNameFromGuid(ref Guid guid, short versionMajor, short versionMinor, int lcid)
		{
			ITypeLib typeLib;
			string result;
			if (TypeLibrary.LoadRegTypeLib(ref guid, versionMajor, versionMinor, lcid, out typeLib) == 0)
			{
				try
				{
					result = Marshal.GetTypeLibName(typeLib);
					return result;
				}
				finally
				{
					Marshal.ReleaseComObject(typeLib);
				}
			}
			result = null;
			return result;
		}

		[DllImport("oleaut32.dll")]
		private static extern int LoadTypeLibEx([MarshalAs(UnmanagedType.BStr)] string szFile, TypeLibrary.RegKind regkind, out ITypeLib pptlib);

		[DllImport("oleaut32.dll")]
		private static extern int LoadRegTypeLib(ref Guid rguid, short wVerMajor, short wVerMinor, int lcid, out ITypeLib pptlib);
	}
}

using MSjogren.GacTool.FusionNative;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom
{
	public static class GacInterop
	{
		public sealed class AssemblyListEntry
		{
			public readonly string FullName;

			public readonly string Name;

			public readonly string Version;

			internal AssemblyListEntry(string fullName)
			{
				this.FullName = fullName;
				string[] info = fullName.Split(new char[]
				{
					','
				});
				this.Name = info[0];
				this.Version = info[1].Substring(info[1].LastIndexOf('=') + 1);
			}
		}

		private static string cachedGacPath;

		public static string GacRootPath
		{
			get
			{
				if (GacInterop.cachedGacPath == null)
				{
					GacInterop.cachedGacPath = Fusion.GetGacPath();
				}
				return GacInterop.cachedGacPath;
			}
		}

		public static List<GacInterop.AssemblyListEntry> GetAssemblyList()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			List<GacInterop.AssemblyListEntry> i = new List<GacInterop.AssemblyListEntry>();
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2u, 0);
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0u) == 0)
			{
				uint nChars = 0u;
				assemblyName.GetDisplayName(null, ref nChars, 0u);
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0u);
				i.Add(new GacInterop.AssemblyListEntry(sb.ToString()));
			}
			return i;
		}

		public static GacAssemblyName FindBestMatchingAssemblyName(string name)
		{
			return GacInterop.FindBestMatchingAssemblyName(new GacAssemblyName(name));
		}

		public static GacAssemblyName FindBestMatchingAssemblyName(GacAssemblyName name)
		{
			string version = name.Version;
			string publicKey = name.PublicKey;
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName;
			Fusion.CreateAssemblyNameObject(out assemblyName, name.Name, 0u, 0);
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, assemblyName, 2u, 0);
			List<string> names = new List<string>();
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0u) == 0)
			{
				uint nChars = 0u;
				assemblyName.GetDisplayName(null, ref nChars, 0u);
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0u);
				string fullName = sb.ToString();
				if (publicKey != null)
				{
					string[] info = fullName.Split(new char[]
					{
						','
					});
					if (publicKey != info[3].Substring(info[3].LastIndexOf('=') + 1))
					{
						continue;
					}
				}
				names.Add(fullName);
			}
			GacAssemblyName result;
			if (names.Count == 0)
			{
				result = null;
			}
			else
			{
				string best = null;
				Version bestVersion = null;
				string[] info;
				if (version != null)
				{
					Version requiredVersion = new Version(version);
					for (int i = 0; i < names.Count; i++)
					{
						info = names[i].Split(new char[]
						{
							','
						});
						Version currentVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
						if (currentVersion.CompareTo(requiredVersion) >= 0)
						{
							if (best == null || currentVersion.CompareTo(bestVersion) < 0)
							{
								bestVersion = currentVersion;
								best = names[i];
							}
						}
					}
					if (best != null)
					{
						result = new GacAssemblyName(best);
						return result;
					}
				}
				best = names[0];
				info = names[0].Split(new char[]
				{
					','
				});
				bestVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
				for (int i = 1; i < names.Count; i++)
				{
					info = names[i].Split(new char[]
					{
						','
					});
					Version currentVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
					if (currentVersion.CompareTo(bestVersion) > 0)
					{
						bestVersion = currentVersion;
						best = names[i];
					}
				}
				result = new GacAssemblyName(best);
			}
			return result;
		}
	}
}

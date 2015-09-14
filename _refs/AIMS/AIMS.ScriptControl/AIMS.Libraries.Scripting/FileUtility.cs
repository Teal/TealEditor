using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AIMS.Libraries.Scripting
{
	public static class FileUtility
	{
		private const string fileNameRegEx = "^([a-zA-Z]:)?[^:]+$";

		private static readonly char[] separators = new char[]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar,
			Path.VolumeSeparatorChar
		};

		private static string applicationRootPath = AppDomain.CurrentDomain.BaseDirectory;

		public static int MaxPathLength = 260;

		public static string ApplicationRootPath
		{
			get
			{
				return FileUtility.applicationRootPath;
			}
			set
			{
				FileUtility.applicationRootPath = value;
			}
		}

		public static string NETFrameworkInstallRoot
		{
			get
			{
				string result;
				using (RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework"))
				{
					object o = installRootKey.GetValue("InstallRoot");
					result = ((o == null) ? string.Empty : o.ToString());
				}
				return result;
			}
		}

		public static string NetSdkInstallRoot
		{
			get
			{
				string val = string.Empty;
				RegistryKey sdkRootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v6.0");
				if (sdkRootKey != null)
				{
					object o = sdkRootKey.GetValue("InstallationFolder");
					val = ((o == null) ? string.Empty : o.ToString());
					sdkRootKey.Close();
				}
				if (val.Length == 0)
				{
					RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework");
					if (installRootKey != null)
					{
						object o = installRootKey.GetValue("sdkInstallRootv2.0");
						val = ((o == null) ? string.Empty : o.ToString());
						installRootKey.Close();
					}
				}
				return val;
			}
		}

		public static string Combine(params string[] paths)
		{
			string result2;
			if (paths == null || paths.Length == 0)
			{
				result2 = string.Empty;
			}
			else
			{
				string result = paths[0];
				for (int i = 1; i < paths.Length; i++)
				{
					result = Path.Combine(result, paths[i]);
				}
				result2 = result;
			}
			return result2;
		}

		public static bool IsUrl(string path)
		{
			return path.IndexOf(':') >= 2;
		}

		public static string GetCommonBaseDirectory(string dir1, string dir2)
		{
			string result2;
			if (dir1 == null || dir2 == null)
			{
				result2 = null;
			}
			else if (FileUtility.IsUrl(dir1) || FileUtility.IsUrl(dir2))
			{
				result2 = null;
			}
			else
			{
				dir1 = Path.GetFullPath(dir1);
				dir2 = Path.GetFullPath(dir2);
				string[] aPath = dir1.Split(new char[]
				{
					Path.DirectorySeparatorChar,
					Path.AltDirectorySeparatorChar
				});
				string[] bPath = dir2.Split(new char[]
				{
					Path.DirectorySeparatorChar,
					Path.AltDirectorySeparatorChar
				});
				StringBuilder result = new StringBuilder();
				int indx;
				for (indx = 0; indx < Math.Min(bPath.Length, aPath.Length); indx++)
				{
					if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
					if (result.Length > 0)
					{
						result.Append(Path.DirectorySeparatorChar);
					}
					result.Append(aPath[indx]);
				}
				if (indx == 0)
				{
					result2 = null;
				}
				else
				{
					result2 = result.ToString();
				}
			}
			return result2;
		}

		public static string GetRelativePath(string baseDirectoryPath, string absPath)
		{
			string result;
			if (FileUtility.IsUrl(absPath) || FileUtility.IsUrl(baseDirectoryPath))
			{
				result = absPath;
			}
			else
			{
				try
				{
					baseDirectoryPath = Path.GetFullPath(baseDirectoryPath.TrimEnd(new char[]
					{
						Path.DirectorySeparatorChar,
						Path.AltDirectorySeparatorChar
					}));
					absPath = Path.GetFullPath(absPath);
				}
				catch (Exception ex)
				{
					throw new ArgumentException(string.Concat(new string[]
					{
						"GetRelativePath error '",
						baseDirectoryPath,
						"' -> '",
						absPath,
						"'"
					}), ex);
				}
				string[] bPath = baseDirectoryPath.Split(FileUtility.separators);
				string[] aPath = absPath.Split(FileUtility.separators);
				int indx;
				for (indx = 0; indx < Math.Min(bPath.Length, aPath.Length); indx++)
				{
					if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
				}
				if (indx == 0)
				{
					result = absPath;
				}
				else
				{
					StringBuilder erg = new StringBuilder();
					if (indx != bPath.Length)
					{
						for (int i = indx; i < bPath.Length; i++)
						{
							erg.Append("..");
							erg.Append(Path.DirectorySeparatorChar);
						}
					}
					erg.Append(string.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
					result = erg.ToString();
				}
			}
			return result;
		}

		public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
		{
			return Path.GetFullPath(Path.Combine(baseDirectoryPath, relPath));
		}

		public static bool IsEqualFileName(string fileName1, string fileName2)
		{
			bool result;
			if (string.IsNullOrEmpty(fileName1) || string.IsNullOrEmpty(fileName2))
			{
				result = false;
			}
			else
			{
				char lastChar = fileName1[fileName1.Length - 1];
				if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
				{
					fileName1 = fileName1.Substring(0, fileName1.Length - 1);
				}
				lastChar = fileName2[fileName2.Length - 1];
				if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
				{
					fileName2 = fileName2.Substring(0, fileName2.Length - 1);
				}
				try
				{
					if (fileName1.Length < 2 || fileName1[1] != ':' || fileName1.IndexOf("/.") >= 0 || fileName1.IndexOf("\\.") >= 0)
					{
						fileName1 = Path.GetFullPath(fileName1);
					}
					if (fileName2.Length < 2 || fileName2[1] != ':' || fileName2.IndexOf("/.") >= 0 || fileName2.IndexOf("\\.") >= 0)
					{
						fileName2 = Path.GetFullPath(fileName2);
					}
				}
				catch (Exception)
				{
				}
				result = string.Equals(fileName1, fileName2, StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
		{
			bool result;
			try
			{
				baseDirectory = Path.GetFullPath(baseDirectory).ToUpperInvariant();
				testDirectory = Path.GetFullPath(testDirectory).ToUpperInvariant();
				baseDirectory = baseDirectory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				testDirectory = testDirectory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				if (baseDirectory[baseDirectory.Length - 1] != Path.DirectorySeparatorChar)
				{
					baseDirectory += Path.DirectorySeparatorChar;
				}
				if (testDirectory[testDirectory.Length - 1] != Path.DirectorySeparatorChar)
				{
					testDirectory += Path.DirectorySeparatorChar;
				}
				result = testDirectory.StartsWith(baseDirectory);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static string RenameBaseDirectory(string fileName, string oldDirectory, string newDirectory)
		{
			fileName = Path.GetFullPath(fileName);
			oldDirectory = Path.GetFullPath(oldDirectory.TrimEnd(new char[]
			{
				Path.DirectorySeparatorChar,
				Path.AltDirectorySeparatorChar
			}));
			newDirectory = Path.GetFullPath(newDirectory.TrimEnd(new char[]
			{
				Path.DirectorySeparatorChar,
				Path.AltDirectorySeparatorChar
			}));
			string result;
			if (FileUtility.IsBaseDirectory(oldDirectory, fileName))
			{
				if (fileName.Length == oldDirectory.Length)
				{
					result = newDirectory;
				}
				else
				{
					result = Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
				}
			}
			else
			{
				result = fileName;
			}
			return result;
		}

		public static void DeepCopy(string sourceDirectory, string destinationDirectory, bool overwrite)
		{
			if (!Directory.Exists(destinationDirectory))
			{
				Directory.CreateDirectory(destinationDirectory);
			}
			string[] array = Directory.GetFiles(sourceDirectory);
			for (int i = 0; i < array.Length; i++)
			{
				string fileName = array[i];
				File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
			}
			array = Directory.GetDirectories(sourceDirectory);
			for (int i = 0; i < array.Length; i++)
			{
				string directoryName = array[i];
				FileUtility.DeepCopy(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
			}
		}

		public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
		{
			List<string> collection = new List<string>();
			FileUtility.SearchDirectory(directory, filemask, collection, searchSubdirectories, ignoreHidden);
			return collection;
		}

		public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			return FileUtility.SearchDirectory(directory, filemask, searchSubdirectories, true);
		}

		public static List<string> SearchDirectory(string directory, string filemask)
		{
			return FileUtility.SearchDirectory(directory, filemask, true, true);
		}

		private static void SearchDirectory(string directory, string filemask, List<string> collection, bool searchSubdirectories, bool ignoreHidden)
		{
			bool isExtMatch = Regex.IsMatch(filemask, "^\\*\\..{3}$");
			string ext = null;
			string[] file = Directory.GetFiles(directory, filemask);
			if (isExtMatch)
			{
				ext = filemask.Remove(0, 1);
			}
			string[] array = file;
			for (int i = 0; i < array.Length; i++)
			{
				string f = array[i];
				if (!ignoreHidden || (File.GetAttributes(f) & FileAttributes.Hidden) != FileAttributes.Hidden)
				{
					if (!isExtMatch || !(Path.GetExtension(f) != ext))
					{
						collection.Add(f);
					}
				}
			}
			if (searchSubdirectories)
			{
				string[] dir = Directory.GetDirectories(directory);
				array = dir;
				for (int i = 0; i < array.Length; i++)
				{
					string d = array[i];
					if (!ignoreHidden || (File.GetAttributes(d) & FileAttributes.Hidden) != FileAttributes.Hidden)
					{
						FileUtility.SearchDirectory(d, filemask, collection, searchSubdirectories, ignoreHidden);
					}
				}
			}
		}

		public static bool IsValidFileName(string fileName)
		{
			bool result;
			if (fileName == null || fileName.Length == 0 || fileName.Length >= FileUtility.MaxPathLength)
			{
				result = false;
			}
			else if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
			{
				result = false;
			}
			else if (fileName.IndexOf('?') >= 0 || fileName.IndexOf('*') >= 0)
			{
				result = false;
			}
			else if (!Regex.IsMatch(fileName, "^([a-zA-Z]:)?[^:]+$"))
			{
				result = false;
			}
			else
			{
				string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
				if (nameWithoutExtension != null)
				{
					nameWithoutExtension = nameWithoutExtension.ToUpperInvariant();
				}
				if (nameWithoutExtension == "CON" || nameWithoutExtension == "PRN" || nameWithoutExtension == "AUX" || nameWithoutExtension == "NUL")
				{
					result = false;
				}
				else
				{
					char ch = (nameWithoutExtension.Length == 4) ? nameWithoutExtension[3] : '\0';
					result = ((!nameWithoutExtension.StartsWith("COM") && !nameWithoutExtension.StartsWith("LPT")) || !char.IsDigit(ch));
				}
			}
			return result;
		}

		public static bool IsValidDirectoryName(string name)
		{
			return FileUtility.IsValidFileName(name) && name.IndexOfAny(new char[]
			{
				Path.AltDirectorySeparatorChar,
				Path.DirectorySeparatorChar
			}) < 0 && name.Trim(new char[]
			{
				' '
			}).Length != 0;
		}

		public static bool TestFileExists(string filename)
		{
			return File.Exists(filename);
		}

		public static bool IsDirectory(string filename)
		{
			bool result;
			if (!Directory.Exists(filename))
			{
				result = false;
			}
			else
			{
				FileAttributes attr = File.GetAttributes(filename);
				result = ((attr & FileAttributes.Directory) != (FileAttributes)0);
			}
			return result;
		}

		private static bool MatchN(string src, int srcidx, string pattern, int patidx)
		{
			int patlen = pattern.Length;
			int srclen = src.Length;
			bool result;
			while (patidx != patlen)
			{
				char next_char = pattern[patidx++];
				if (next_char == '?')
				{
					if (srcidx == src.Length)
					{
						result = false;
						return result;
					}
					srcidx++;
				}
				else if (next_char != '*')
				{
					if (srcidx == src.Length || src[srcidx] != next_char)
					{
						result = false;
						return result;
					}
					srcidx++;
				}
				else
				{
					if (patidx == pattern.Length)
					{
						result = true;
						return result;
					}
					while (srcidx < srclen)
					{
						if (FileUtility.MatchN(src, srcidx, pattern, patidx))
						{
							result = true;
							return result;
						}
						srcidx++;
					}
					result = false;
					return result;
				}
			}
			result = (srcidx == srclen);
			return result;
		}

		private static bool Match(string src, string pattern)
		{
			bool result;
			if (pattern[0] == '*')
			{
				int i = pattern.Length;
				int j = src.Length;
				while (--i > 0)
				{
					if (pattern[i] == '*')
					{
						result = FileUtility.MatchN(src, 0, pattern, 0);
						return result;
					}
					if (j-- == 0)
					{
						result = false;
						return result;
					}
					if (pattern[i] != src[j] && pattern[i] != '?')
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			else
			{
				result = FileUtility.MatchN(src, 0, pattern, 0);
			}
			return result;
		}

		public static bool MatchesPattern(string filename, string pattern)
		{
			filename = filename.ToUpper();
			pattern = pattern.ToUpper();
			string[] patterns = pattern.Split(new char[]
			{
				';'
			});
			string[] array = patterns;
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				string p = array[i];
				if (FileUtility.Match(filename, p))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}

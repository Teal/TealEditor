using System;
using System.IO;
using System.Reflection;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class ReflectionLoader : MarshalByRefObject
	{
		private string lookupDirectory;

		public override string ToString()
		{
			return "ReflectionLoader in " + AppDomain.CurrentDomain.FriendlyName;
		}

		public static Assembly ReflectionLoadGacAssembly(string partialName, bool reflectionOnly)
		{
			Assembly result;
			if (reflectionOnly)
			{
				GacAssemblyName name = GacInterop.FindBestMatchingAssemblyName(partialName);
				if (name == null)
				{
					result = null;
				}
				else
				{
					result = Assembly.ReflectionOnlyLoad(name.FullName);
				}
			}
			else
			{
				result = Assembly.LoadWithPartialName(partialName);
			}
			return result;
		}

		public string LoadAndCreateDatabase(string fileName, string include, string cacheDirectory)
		{
			string result;
			try
			{
				ReflectionProjectContent content = this.LoadProjectContent(fileName, include, new ProjectContentRegistry());
				if (content == null)
				{
					result = null;
				}
				else
				{
					result = new DomPersistence(cacheDirectory, null).SaveProjectContent(content);
				}
			}
			catch (Exception ex)
			{
				if (ex is FileLoadException)
				{
					LoggingService.Info(ex);
				}
				else
				{
					LoggingService.Error(ex);
				}
				throw;
			}
			return result;
		}

		private ReflectionProjectContent LoadProjectContent(string fileName, string include, ProjectContentRegistry registry)
		{
			fileName = Path.GetFullPath(fileName);
			LoggingService.Debug("Trying to load " + fileName);
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(this.AssemblyResolve);
			this.lookupDirectory = Path.GetDirectoryName(fileName);
			ReflectionProjectContent result;
			try
			{
				if (File.Exists(fileName))
				{
					Assembly assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
					result = new ReflectionProjectContent(assembly, fileName, registry);
				}
				else
				{
					Assembly assembly = ReflectionLoader.ReflectionLoadGacAssembly(include, true);
					if (assembly == null)
					{
						throw new FileLoadException("Assembly not found.");
					}
					result = new ReflectionProjectContent(assembly, registry);
				}
			}
			catch (BadImageFormatException ex)
			{
				LoggingService.Warn("BadImageFormat: " + include);
				throw new FileLoadException(ex.Message, ex);
			}
			finally
			{
				AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= new ResolveEventHandler(this.AssemblyResolve);
				this.lookupDirectory = null;
			}
			return result;
		}

		private Assembly AssemblyResolve(object sender, ResolveEventArgs e)
		{
			AssemblyName name = new AssemblyName(e.Name);
			LoggingService.Debug("ProjectContentRegistry.AssemblyResolve " + e.Name);
			string path = Path.Combine(this.lookupDirectory, name.Name);
			Assembly result;
			if (File.Exists(path + ".dll"))
			{
				result = Assembly.ReflectionOnlyLoadFrom(path + ".dll");
			}
			else if (File.Exists(path + ".exe"))
			{
				result = Assembly.ReflectionOnlyLoadFrom(path + ".exe");
			}
			else if (File.Exists(path))
			{
				result = Assembly.ReflectionOnlyLoadFrom(path);
			}
			else
			{
				try
				{
					LoggingService.Debug("AssemblyResolve trying ReflectionOnlyLoad");
					result = Assembly.ReflectionOnlyLoad(e.Name);
				}
				catch (FileNotFoundException)
				{
					LoggingService.Warn("AssemblyResolve: ReflectionOnlyLoad failed for " + e.Name);
					GacAssemblyName fixedName = GacInterop.FindBestMatchingAssemblyName(e.Name);
					LoggingService.Info("AssemblyResolve: FixedName: " + fixedName);
					result = Assembly.ReflectionOnlyLoad(fixedName.FullName);
				}
			}
			return result;
		}
	}
}

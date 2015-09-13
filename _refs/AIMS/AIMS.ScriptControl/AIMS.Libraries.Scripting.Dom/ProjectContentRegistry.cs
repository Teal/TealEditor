using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace AIMS.Libraries.Scripting.Dom
{
	public class ProjectContentRegistry : IDisposable
	{
		internal DomPersistence persistence;

		private Dictionary<string, IProjectContent> contents = new Dictionary<string, IProjectContent>(StringComparer.InvariantCultureIgnoreCase);

		protected Dictionary<string, string> redirectedAssemblyNames = new Dictionary<string, string>();

		private ReflectionProjectContent mscorlibContent;

		public virtual IProjectContent Mscorlib
		{
			get
			{
				IProjectContent result;
				if (this.mscorlibContent != null)
				{
					result = this.mscorlibContent;
				}
				else
				{
					lock (this.contents)
					{
						if (this.contents.ContainsKey("mscorlib"))
						{
							this.mscorlibContent = (ReflectionProjectContent)this.contents["mscorlib"];
							result = this.contents["mscorlib"];
						}
						else
						{
							int time = LoggingService.IsDebugEnabled ? Environment.TickCount : 0;
							LoggingService.Debug("Loading PC for mscorlib...");
							if (this.persistence != null)
							{
								this.mscorlibContent = this.persistence.LoadProjectContentByAssemblyName(ProjectContentRegistry.MscorlibAssembly.FullName);
								if (this.mscorlibContent != null)
								{
									if (time != 0)
									{
										LoggingService.Debug("Loaded mscorlib from cache in " + (Environment.TickCount - time) + " ms");
									}
								}
							}
							if (this.mscorlibContent == null)
							{
								this.mscorlibContent = new ReflectionProjectContent(ProjectContentRegistry.MscorlibAssembly, this);
								if (time != 0)
								{
									LoggingService.Debug("Loaded mscorlib with Reflection in " + (Environment.TickCount - time) + " ms");
								}
								if (this.persistence != null)
								{
									this.persistence.SaveProjectContent(this.mscorlibContent);
									LoggingService.Debug("Saved mscorlib to cache");
								}
							}
							this.contents["mscorlib"] = this.mscorlibContent;
							this.contents[this.mscorlibContent.AssemblyFullName] = this.mscorlibContent;
							this.contents[this.mscorlibContent.AssemblyLocation] = this.mscorlibContent;
							lock (this.redirectedAssemblyNames)
							{
								this.redirectedAssemblyNames.Add("mscorlib", this.mscorlibContent.AssemblyFullName);
							}
							result = this.mscorlibContent;
						}
					}
				}
				return result;
			}
		}

		public static Assembly MscorlibAssembly
		{
			get
			{
				return typeof(object).Assembly;
			}
		}

		public static Assembly SystemAssembly
		{
			get
			{
				return typeof(Uri).Assembly;
			}
		}

		public virtual void Dispose()
		{
			List<IProjectContent> list;
			lock (this.contents)
			{
				list = new List<IProjectContent>(this.contents.Values);
				this.contents.Clear();
			}
			foreach (IProjectContent pc in list)
			{
				pc.Dispose();
			}
		}

		public DomPersistence ActivatePersistence(string cacheDirectory)
		{
			if (cacheDirectory == null)
			{
				throw new ArgumentNullException("cacheDirectory");
			}
			DomPersistence result;
			if (this.persistence != null && cacheDirectory == this.persistence.CacheDirectory)
			{
				result = this.persistence;
			}
			else
			{
				this.persistence = new DomPersistence(cacheDirectory, this);
				result = this.persistence;
			}
			return result;
		}

		protected ReflectionProjectContent ReflectionLoadProjectContent(string filename, string include)
		{
			bool tempPersistence;
			DomPersistence persistence;
			ReflectionProjectContent result;
			if (this.persistence == null)
			{
				tempPersistence = true;
				persistence = new DomPersistence(Path.GetTempPath(), this);
			}
			else
			{
				tempPersistence = false;
				persistence = this.persistence;
				ReflectionProjectContent pc = persistence.LoadProjectContentByAssemblyName(filename);
				if (pc != null)
				{
					result = pc;
					return result;
				}
				pc = persistence.LoadProjectContentByAssemblyName(include);
				if (pc != null)
				{
					result = pc;
					return result;
				}
			}
			AppDomainSetup setup = new AppDomainSetup();
			setup.DisallowCodeDownload = true;
			setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
			AppDomain domain = AppDomain.CreateDomain("AssemblyLoadingDomain", AppDomain.CurrentDomain.Evidence, setup);
			string database;
			try
			{
				object o = domain.CreateInstanceAndUnwrap(typeof(ReflectionLoader).Assembly.FullName, typeof(ReflectionLoader).FullName);
				ReflectionLoader loader = (ReflectionLoader)o;
				database = loader.LoadAndCreateDatabase(filename, include, persistence.CacheDirectory);
			}
			catch (FileLoadException e)
			{
				database = null;
				HostCallback.ShowAssemblyLoadErrorInternal(filename, include, e.Message);
			}
			catch (Exception e2)
			{
				database = null;
				HostCallback.ShowError("Error loading code-completion information for " + include + " from " + filename, e2);
				throw e2;
			}
			finally
			{
				AppDomain.Unload(domain);
			}
			if (database == null)
			{
				LoggingService.Debug("AppDomain finished but returned null...");
				result = null;
			}
			else
			{
				LoggingService.Debug("AppDomain finished, loading cache...");
				try
				{
					result = persistence.LoadProjectContent(database);
				}
				finally
				{
					if (tempPersistence)
					{
						try
						{
							File.Delete(database);
						}
						catch
						{
						}
					}
				}
			}
			return result;
		}

		public void RunLocked(ThreadStart method)
		{
			lock (this.contents)
			{
				method();
			}
		}

		public virtual ICollection<IProjectContent> GetLoadedProjectContents()
		{
			ICollection<IProjectContent> result;
			lock (this.contents)
			{
				result = new List<IProjectContent>(this.contents.Values);
			}
			return result;
		}

		[Obsolete("Use DomAssemblyName instead of AssemblyName")]
		public IProjectContent GetExistingProjectContent(AssemblyName assembly)
		{
			return this.GetExistingProjectContent(assembly.FullName, assembly.FullName);
		}

		public IProjectContent GetExistingProjectContent(DomAssemblyName assembly)
		{
			return this.GetExistingProjectContent(assembly.FullName, assembly.FullName);
		}

		public virtual IProjectContent GetExistingProjectContent(string itemInclude, string itemFileName)
		{
			if (itemFileName == itemInclude)
			{
				string shortName = itemInclude;
				int pos = shortName.IndexOf(',');
				if (pos > 0)
				{
					shortName = shortName.Substring(0, pos);
				}
				lock (this.redirectedAssemblyNames)
				{
					if (this.redirectedAssemblyNames.ContainsKey(shortName))
					{
						itemFileName = this.redirectedAssemblyNames[shortName];
						itemInclude = shortName;
					}
				}
			}
			IProjectContent result;
			lock (this.contents)
			{
				if (this.contents.ContainsKey(itemFileName))
				{
					result = this.contents[itemFileName];
					return result;
				}
				if (this.contents.ContainsKey(itemInclude))
				{
					result = this.contents[itemInclude];
					return result;
				}
			}
			result = null;
			return result;
		}

		public virtual IProjectContent GetProjectContentForReference(string itemInclude, string itemFileName)
		{
			IProjectContent result;
			lock (this.contents)
			{
				IProjectContent pc = this.GetExistingProjectContent(itemInclude, itemFileName);
				if (pc != null)
				{
					result = pc;
				}
				else
				{
					LoggingService.Debug("Loading PC for " + itemInclude);
					string shortName = itemInclude;
					int pos = shortName.IndexOf(',');
					if (pos > 0)
					{
						shortName = shortName.Substring(0, pos);
					}
					HostCallback.BeginAssemblyLoad(shortName);
					int time = Environment.TickCount;
					try
					{
						pc = this.LoadProjectContent(itemInclude, itemFileName);
					}
					catch (Exception ex)
					{
						HostCallback.ShowAssemblyLoadErrorInternal(itemFileName, itemInclude, "Error loading assembly:\n" + ex.ToString());
						throw ex;
					}
					finally
					{
						LoggingService.Debug(string.Format("Loaded {0} in {1}ms", itemInclude, Environment.TickCount - time));
						HostCallback.FinishAssemblyLoad();
					}
					if (pc != null)
					{
						ReflectionProjectContent reflectionProjectContent = pc as ReflectionProjectContent;
						if (reflectionProjectContent != null && reflectionProjectContent.AssemblyFullName != null)
						{
							this.contents[reflectionProjectContent.AssemblyFullName] = pc;
						}
						this.contents[itemInclude] = pc;
						this.contents[itemFileName] = pc;
					}
					result = pc;
				}
			}
			return result;
		}

		protected virtual IProjectContent LoadProjectContent(string itemInclude, string itemFileName)
		{
			string shortName = itemInclude;
			int pos = shortName.IndexOf(',');
			if (pos > 0)
			{
				shortName = shortName.Substring(0, pos);
			}
			Assembly assembly = ProjectContentRegistry.GetDefaultAssembly(shortName);
			ReflectionProjectContent pc = null;
			if (assembly != null)
			{
				if (this.persistence != null)
				{
					pc = this.persistence.LoadProjectContentByAssemblyName(assembly.FullName);
				}
				if (pc == null)
				{
					pc = new ReflectionProjectContent(assembly, this);
					if (this.persistence != null)
					{
						this.persistence.SaveProjectContent(pc);
					}
				}
				if (pc != null)
				{
					lock (this.redirectedAssemblyNames)
					{
						this.redirectedAssemblyNames[shortName] = pc.AssemblyFullName;
					}
				}
			}
			else if (pc == null)
			{
				pc = this.ReflectionLoadProjectContent(itemFileName, itemInclude);
			}
			return pc;
		}

		private static string GetVersion__Token(GacAssemblyName asmName)
		{
			StringBuilder b = new StringBuilder(asmName.Version.ToString());
			b.Append("__");
			b.Append(asmName.PublicKey);
			return b.ToString();
		}

		private static Assembly GetDefaultAssembly(string shortName)
		{
			Assembly result;
			switch (shortName)
			{
			case "System":
				result = ProjectContentRegistry.SystemAssembly;
				return result;
			case "System.Xml":
			case "System.XML":
				result = typeof(XmlReader).Assembly;
				return result;
			case "System.Data":
			case "System.Design":
			case "System.Drawing":
			case "System.Web.Services":
			case "System.Windows.Forms":
			case "System.Web":
			case "System.ServiceProcess":
			case "System.Security":
			case "System.Runtime.Remoting":
			case "System.Messaging":
			case "System.Management":
			case "System.Drawing.Design":
			case "System.Deployment":
			case "System.Configuration":
			case "Microsoft.VisualBasic":
				result = ReflectionLoader.ReflectionLoadGacAssembly(shortName, false);
				return result;
			}
			result = null;
			return result;
		}

		public void UnloadProjectContent(IProjectContent pc)
		{
			if (pc == null)
			{
				throw new ArgumentNullException("pc");
			}
			LoggingService.Debug("ProjectContentRegistry.UnloadProjectContent: " + pc);
			lock (this.contents)
			{
				List<string> keys = new List<string>();
				foreach (KeyValuePair<string, IProjectContent> pair in this.contents)
				{
					if (pair.Value == pc)
					{
						keys.Add(pair.Key);
					}
				}
				foreach (string key in keys)
				{
					this.contents.Remove(key);
				}
			}
			pc.Dispose();
		}
	}
}

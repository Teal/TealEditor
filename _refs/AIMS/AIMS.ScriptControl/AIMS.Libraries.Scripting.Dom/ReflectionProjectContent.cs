using AIMS.Libraries.Scripting.Dom.ReflectionLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AIMS.Libraries.Scripting.Dom
{
	public class ReflectionProjectContent : DefaultProjectContent
	{
		private string assemblyFullName;

		private DomAssemblyName[] referencedAssemblyNames;

		private ICompilationUnit assemblyCompilationUnit;

		private string assemblyLocation;

		private ProjectContentRegistry registry;

		private DateTime assemblyFileLastWriteTime;

		private bool initialized = false;

		private List<DomAssemblyName> missingNames;

		public string AssemblyLocation
		{
			get
			{
				return this.assemblyLocation;
			}
		}

		public string AssemblyFullName
		{
			get
			{
				return this.assemblyFullName;
			}
		}

		[Obsolete("This property always returns an empty array! Use ReferencedAssemblyNames instead!")]
		public AssemblyName[] ReferencedAssemblies
		{
			get
			{
				return new AssemblyName[0];
			}
		}

		public IList<DomAssemblyName> ReferencedAssemblyNames
		{
			get
			{
				return Array.AsReadOnly<DomAssemblyName>(this.referencedAssemblyNames);
			}
		}

		public ICompilationUnit AssemblyCompilationUnit
		{
			get
			{
				return this.assemblyCompilationUnit;
			}
		}

		public override bool IsUpToDate
		{
			get
			{
				DateTime newWriteTime;
				bool result;
				try
				{
					newWriteTime = File.GetLastWriteTimeUtc(this.assemblyLocation);
				}
				catch (Exception ex)
				{
					LoggingService.Warn(ex);
					result = true;
					return result;
				}
				result = (this.assemblyFileLastWriteTime == newWriteTime);
				return result;
			}
		}

		public ReflectionProjectContent(Assembly assembly, ProjectContentRegistry registry) : this(assembly, assembly.Location, registry)
		{
		}

		public ReflectionProjectContent(Assembly assembly, string assemblyLocation, ProjectContentRegistry registry) : this(assembly.FullName, assemblyLocation, DomAssemblyName.Convert(assembly.GetReferencedAssemblies()), registry)
		{
			Type[] exportedTypes = assembly.GetExportedTypes();
			for (int i = 0; i < exportedTypes.Length; i++)
			{
				Type type = exportedTypes[i];
				string name = type.FullName;
				if (name.IndexOf('+') < 0)
				{
					base.AddClassToNamespaceListInternal(new ReflectionClass(this.assemblyCompilationUnit, type, name, null));
				}
			}
			this.InitializeSpecialClasses();
		}

		[Obsolete("Use DomAssemblyName instead of AssemblyName!")]
		public ReflectionProjectContent(string assemblyFullName, string assemblyLocation, AssemblyName[] referencedAssemblies, ProjectContentRegistry registry) : this(assemblyFullName, assemblyLocation, DomAssemblyName.Convert(referencedAssemblies), registry)
		{
		}

		public ReflectionProjectContent(string assemblyFullName, string assemblyLocation, DomAssemblyName[] referencedAssemblies, ProjectContentRegistry registry)
		{
			if (assemblyFullName == null)
			{
				throw new ArgumentNullException("assemblyFullName");
			}
			if (assemblyLocation == null)
			{
				throw new ArgumentNullException("assemblyLocation");
			}
			if (registry == null)
			{
				throw new ArgumentNullException("registry");
			}
			this.registry = registry;
			this.assemblyFullName = assemblyFullName;
			this.referencedAssemblyNames = referencedAssemblies;
			this.assemblyLocation = assemblyLocation;
			this.assemblyCompilationUnit = new DefaultCompilationUnit(this);
			try
			{
				this.assemblyFileLastWriteTime = File.GetLastWriteTimeUtc(assemblyLocation);
			}
			catch (Exception ex)
			{
				LoggingService.Warn(ex);
			}
			string fileName = DefaultProjectContent.LookupLocalizedXmlDoc(assemblyLocation);
			if (fileName == null)
			{
				foreach (string testDirectory in XmlDoc.XmlDocLookupDirectories)
				{
					fileName = DefaultProjectContent.LookupLocalizedXmlDoc(Path.Combine(testDirectory, Path.GetFileName(assemblyLocation)));
					if (fileName != null)
					{
						break;
					}
				}
			}
			if (fileName != null && registry.persistence != null)
			{
				base.XmlDoc = XmlDoc.Load(fileName, Path.Combine(registry.persistence.CacheDirectory, "XmlDoc"));
			}
		}

		public void InitializeSpecialClasses()
		{
			if (base.GetClassInternal(VoidClass.VoidName, 0, base.Language) != null)
			{
				base.AddClassToNamespaceList(VoidClass.Instance);
			}
		}

		public void InitializeReferences()
		{
			bool changed = false;
			if (this.initialized)
			{
				if (this.missingNames != null)
				{
					for (int i = 0; i < this.missingNames.Count; i++)
					{
						IProjectContent content = this.registry.GetExistingProjectContent(this.missingNames[i]);
						if (content != null)
						{
							changed = true;
							lock (base.ReferencedContents)
							{
								base.ReferencedContents.Add(content);
							}
							this.missingNames.RemoveAt(i--);
						}
					}
					if (this.missingNames.Count == 0)
					{
						this.missingNames = null;
					}
				}
			}
			else
			{
				this.initialized = true;
				DomAssemblyName[] array = this.referencedAssemblyNames;
				for (int j = 0; j < array.Length; j++)
				{
					DomAssemblyName name = array[j];
					IProjectContent content = this.registry.GetExistingProjectContent(name);
					if (content != null)
					{
						changed = true;
						lock (base.ReferencedContents)
						{
							base.ReferencedContents.Add(content);
						}
					}
					else
					{
						if (this.missingNames == null)
						{
							this.missingNames = new List<DomAssemblyName>();
						}
						this.missingNames.Add(name);
					}
				}
			}
			if (changed)
			{
				this.OnReferencedContentsChanged(EventArgs.Empty);
			}
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}]", base.GetType().Name, this.assemblyFullName);
		}
	}
}

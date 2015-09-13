using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public class ReferenceProjectItem : ProjectItem
	{
		[Browsable(false)]
		public string HintPath
		{
			get
			{
				return base.GetEvaluatedMetadata("HintPath");
			}
			set
			{
				base.SetEvaluatedMetadata("HintPath", value);
			}
		}

		public string Aliases
		{
			get
			{
				return base.GetEvaluatedMetadata<string>("Aliases", "global");
			}
			set
			{
				base.SetEvaluatedMetadata("Aliases", value);
			}
		}

		public bool SpecificVersion
		{
			get
			{
				return base.GetEvaluatedMetadata<bool>("SpecificVersion", false);
			}
			set
			{
				base.SetEvaluatedMetadata<bool>("SpecificVersion", value);
			}
		}

		public bool Private
		{
			get
			{
				return base.GetEvaluatedMetadata<bool>("Private", !this.IsGacReference);
			}
			set
			{
				base.SetEvaluatedMetadata<bool>("Private", value);
			}
		}

		[ReadOnly(true)]
		public string Name
		{
			get
			{
				AssemblyName assemblyName = this.GetAssemblyName(base.Include);
				string result;
				if (assemblyName != null)
				{
					result = assemblyName.Name;
				}
				else
				{
					result = base.Include;
				}
				return result;
			}
		}

		[ReadOnly(true)]
		public Version Version
		{
			get
			{
				AssemblyName assemblyName = this.GetAssemblyName(base.Include);
				Version result;
				if (assemblyName != null)
				{
					result = assemblyName.Version;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		[ReadOnly(true)]
		public string Culture
		{
			get
			{
				AssemblyName assemblyName = this.GetAssemblyName(base.Include);
				string result;
				if (assemblyName != null && assemblyName.CultureInfo != null)
				{
					result = assemblyName.CultureInfo.Name;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		[ReadOnly(true)]
		public string PublicKeyToken
		{
			get
			{
				AssemblyName assemblyName = this.GetAssemblyName(base.Include);
				string result;
				if (assemblyName != null)
				{
					byte[] bytes = assemblyName.GetPublicKeyToken();
					if (bytes != null)
					{
						StringBuilder token = new StringBuilder();
						byte[] array = bytes;
						for (int i = 0; i < array.Length; i++)
						{
							byte b = array[i];
							token.Append(b.ToString("x2"));
						}
						result = token.ToString();
						return result;
					}
				}
				result = null;
				return result;
			}
		}

		[ReadOnly(true)]
		public override string FileName
		{
			get
			{
				string result;
				if (base.Project != null)
				{
					string projectDir = base.Project.Directory;
					string hintPath = this.HintPath;
					try
					{
						if (hintPath != null && hintPath.Length > 0)
						{
							result = FileUtility.GetAbsolutePath(projectDir, hintPath);
							return result;
						}
						string name = FileUtility.GetAbsolutePath(projectDir, base.Include);
						if (File.Exists(name))
						{
							result = name;
							return result;
						}
						if (File.Exists(name + ".dll"))
						{
							result = name + ".dll";
							return result;
						}
						if (File.Exists(name + ".exe"))
						{
							result = name + ".exe";
							return result;
						}
					}
					catch
					{
					}
				}
				result = base.Include;
				return result;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public bool IsGacReference
		{
			get
			{
				return !Path.IsPathRooted(this.FileName);
			}
		}

		protected ReferenceProjectItem(IProject project, ItemType itemType) : base(project, itemType)
		{
		}

		public ReferenceProjectItem(IProject project) : base(project, ItemType.Reference)
		{
		}

		public ReferenceProjectItem(IProject project, string include) : base(project, ItemType.Reference, include)
		{
		}

		private AssemblyName GetAssemblyName(string include)
		{
			AssemblyName result;
			try
			{
				if (base.ItemType == ItemType.Reference)
				{
					result = new AssemblyName(include);
					return result;
				}
			}
			catch (ArgumentException)
			{
			}
			result = null;
			return result;
		}
	}
}

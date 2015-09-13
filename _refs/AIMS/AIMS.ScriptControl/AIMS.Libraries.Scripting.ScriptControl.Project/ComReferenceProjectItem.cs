using System;
using System.ComponentModel;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public sealed class ComReferenceProjectItem : ReferenceProjectItem
	{
		[ReadOnly(true)]
		public string Guid
		{
			get
			{
				return base.GetEvaluatedMetadata("Guid");
			}
			set
			{
				base.SetEvaluatedMetadata("Guid", value);
			}
		}

		[ReadOnly(true)]
		public int VersionMajor
		{
			get
			{
				return base.GetEvaluatedMetadata<int>("VersionMajor", 1);
			}
			set
			{
				base.SetEvaluatedMetadata<int>("VersionMajor", value);
			}
		}

		[ReadOnly(true)]
		public int VersionMinor
		{
			get
			{
				return base.GetEvaluatedMetadata<int>("VersionMinor", 0);
			}
			set
			{
				base.SetEvaluatedMetadata<int>("VersionMinor", value);
			}
		}

		[ReadOnly(true)]
		public string Lcid
		{
			get
			{
				return base.GetEvaluatedMetadata("Lcid");
			}
			set
			{
				base.SetEvaluatedMetadata("Lcid", value);
			}
		}

		[ReadOnly(true)]
		public string FilePath
		{
			get
			{
				return base.GetEvaluatedMetadata("FilePath");
			}
			set
			{
				base.SetEvaluatedMetadata("FilePath", value);
			}
		}

		[ReadOnly(true)]
		public string WrapperTool
		{
			get
			{
				return base.GetEvaluatedMetadata("WrapperTool");
			}
			set
			{
				base.SetEvaluatedMetadata("WrapperTool", value);
			}
		}

		[ReadOnly(true)]
		public bool Isolated
		{
			get
			{
				return base.GetEvaluatedMetadata<bool>("Isolated", false);
			}
			set
			{
				base.SetEvaluatedMetadata<bool>("Isolated", value);
			}
		}

		public override string FileName
		{
			get
			{
				string result;
				try
				{
					if (base.Project != null && base.Project.OutputAssemblyFullPath != null)
					{
						string outputFolder = Path.GetDirectoryName(base.Project.OutputAssemblyFullPath);
						string interopFileName = Path.Combine(outputFolder, "Interop." + base.Include + ".dll");
						if (File.Exists(interopFileName))
						{
							result = interopFileName;
							return result;
						}
						interopFileName = ComReferenceProjectItem.GetActiveXInteropFileName(outputFolder, base.Include);
						if (File.Exists(interopFileName))
						{
							result = interopFileName;
							return result;
						}
						interopFileName = ComReferenceProjectItem.GetActiveXInteropFileName(outputFolder, base.Include);
						if (File.Exists(interopFileName))
						{
							result = interopFileName;
							return result;
						}
					}
				}
				catch (Exception)
				{
				}
				result = base.Include;
				return result;
			}
			set
			{
			}
		}

		public ComReferenceProjectItem(IProject project, TypeLibrary library) : base(project, ItemType.COMReference)
		{
			base.Include = library.Name;
			this.Guid = library.Guid;
			this.VersionMajor = library.VersionMajor;
			this.VersionMinor = library.VersionMinor;
			this.Lcid = library.Lcid;
			this.WrapperTool = library.WrapperTool;
			this.Isolated = library.Isolated;
			this.FilePath = library.Path;
		}

		private static string GetActiveXInteropFileName(string outputFolder, string include)
		{
			string result;
			if (include.ToLowerInvariant().StartsWith("ax"))
			{
				result = Path.Combine(outputFolder, "AxInterop." + include.Substring(2) + ".dll");
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}

using System;
using System.ComponentModel;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public class FileProjectItem : ProjectItem
	{
		public string BuildAction
		{
			get
			{
				return base.ItemType.ItemName;
			}
			set
			{
				base.ItemType = new ItemType(value);
			}
		}

		public CopyToOutputDirectory CopyToOutputDirectory
		{
			get
			{
				return base.GetEvaluatedMetadata<CopyToOutputDirectory>("CopyToOutputDirectory", CopyToOutputDirectory.Never);
			}
			set
			{
				base.SetEvaluatedMetadata<CopyToOutputDirectory>("CopyToOutputDirectory", value);
			}
		}

		public string CustomTool
		{
			get
			{
				return base.GetEvaluatedMetadata("Generator");
			}
			set
			{
				base.SetEvaluatedMetadata("Generator", value);
			}
		}

		public string CustomToolNamespace
		{
			get
			{
				return base.GetEvaluatedMetadata("CustomToolNamespace");
			}
			set
			{
				base.SetEvaluatedMetadata("CustomToolNamespace", value);
			}
		}

		[Browsable(false)]
		public string DependentUpon
		{
			get
			{
				return base.GetEvaluatedMetadata("DependentUpon");
			}
			set
			{
				base.SetEvaluatedMetadata("DependentUpon", value);
			}
		}

		[Browsable(false)]
		public string SubType
		{
			get
			{
				return base.GetEvaluatedMetadata("SubType");
			}
			set
			{
				base.SetEvaluatedMetadata("SubType", value);
			}
		}

		[Browsable(false)]
		public bool IsLink
		{
			get
			{
				return base.HasMetadata("Link") || !FileUtility.IsBaseDirectory(base.Project.Directory, this.FileName);
			}
		}

		[Browsable(false)]
		public string VirtualName
		{
			get
			{
				string result;
				if (base.HasMetadata("Link"))
				{
					result = base.GetEvaluatedMetadata("Link");
				}
				else if (FileUtility.IsBaseDirectory(base.Project.Directory, this.FileName))
				{
					result = base.Include;
				}
				else
				{
					result = Path.GetFileName(base.Include);
				}
				return result;
			}
		}

		public FileProjectItem(IProject project, ItemType itemType, string include) : base(project, itemType, include)
		{
		}

		public FileProjectItem(IProject project, ItemType itemType) : base(project, itemType)
		{
		}
	}
}

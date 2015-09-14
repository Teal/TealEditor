using System;
using System.ComponentModel;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public sealed class WebReferenceUrl : ProjectItem
	{
		[ReadOnly(true)]
		public string UpdateFromURL
		{
			get
			{
				return base.GetEvaluatedMetadata("UpdateFromURL");
			}
			set
			{
				base.SetEvaluatedMetadata("UpdateFromURL", value);
			}
		}

		[Browsable(false)]
		public string ServiceLocationURL
		{
			get
			{
				return base.GetEvaluatedMetadata("ServiceLocationURL");
			}
			set
			{
				base.SetEvaluatedMetadata("ServiceLocationURL", value);
			}
		}

		[Browsable(false)]
		public string CachedDynamicPropName
		{
			get
			{
				return base.GetEvaluatedMetadata("CachedDynamicPropName");
			}
			set
			{
				base.SetEvaluatedMetadata("CachedDynamicPropName", value);
			}
		}

		[Browsable(false)]
		public string CachedAppSettingsObjectName
		{
			get
			{
				return base.GetEvaluatedMetadata("CachedAppSettingsObjectName");
			}
			set
			{
				base.SetEvaluatedMetadata("CachedAppSettingsObjectName", value);
			}
		}

		[Browsable(false)]
		public string CachedSettingsPropName
		{
			get
			{
				return base.GetEvaluatedMetadata("CachedSettingsPropName");
			}
			set
			{
				base.SetEvaluatedMetadata("CachedSettingsPropName", value);
			}
		}

		[Browsable(false)]
		public string Namespace
		{
			get
			{
				string ns = base.GetEvaluatedMetadata("Namespace");
				string result;
				if (ns.Length > 0)
				{
					result = ns;
				}
				else
				{
					result = base.Project.RootNamespace;
				}
				return result;
			}
			set
			{
				base.SetEvaluatedMetadata("Namespace", value);
			}
		}

		[Browsable(false)]
		public string RelPath
		{
			get
			{
				return base.GetEvaluatedMetadata("RelPath");
			}
			set
			{
				base.SetEvaluatedMetadata("RelPath", value);
			}
		}

		[ReadOnly(true)]
		public string UrlBehavior
		{
			get
			{
				return base.GetEvaluatedMetadata("UrlBehavior");
			}
			set
			{
				base.SetEvaluatedMetadata("UrlBehavior", value);
			}
		}

		public override string FileName
		{
			get
			{
				string result;
				if (base.Project != null && this.RelPath != null)
				{
					result = Path.Combine(base.Project.Directory, this.RelPath.Trim(new char[]
					{
						'\\'
					}));
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				if (base.Project != null)
				{
					this.RelPath = FileUtility.GetRelativePath(base.Project.Directory, value);
				}
			}
		}

		public WebReferenceUrl(IProject project) : base(project, ItemType.WebReferenceUrl)
		{
			this.UrlBehavior = "Static";
		}
	}
}

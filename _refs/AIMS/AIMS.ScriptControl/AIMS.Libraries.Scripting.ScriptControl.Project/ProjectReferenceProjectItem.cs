using System;
using System.ComponentModel;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public class ProjectReferenceProjectItem : ReferenceProjectItem
	{
		private IProject referencedProject;

		[Browsable(false)]
		public IProject ReferencedProject
		{
			get
			{
				if (this.referencedProject == null)
				{
					this.referencedProject = ScriptControl.GetProject();
				}
				return this.referencedProject;
			}
		}

		[ReadOnly(true)]
		public string ProjectGuid
		{
			get
			{
				return base.GetEvaluatedMetadata("Project");
			}
			set
			{
				base.SetEvaluatedMetadata("Project", value);
			}
		}

		[ReadOnly(true)]
		public string ProjectName
		{
			get
			{
				return base.GetEvaluatedMetadata("Name");
			}
			set
			{
				base.SetEvaluatedMetadata("Name", value);
			}
		}

		public ProjectReferenceProjectItem(IProject project, IProject referenceTo) : base(project, ItemType.ProjectReference)
		{
			base.Include = FileUtility.GetRelativePath(project.Directory, referenceTo.FileName);
			this.referencedProject = referenceTo;
		}
	}
}

using System;
using System.ComponentModel;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public sealed class WebReferencesProjectItem : FileProjectItem
	{
		[Browsable(false)]
		public string Directory
		{
			get
			{
				return Path.Combine(base.Project.Directory, base.Include).Trim(new char[]
				{
					'\\',
					'/'
				});
			}
		}

		public WebReferencesProjectItem(IProject project) : base(project, ItemType.WebReferences)
		{
		}
	}
}

using System;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public sealed class ImportProjectItem : ProjectItem
	{
		public ImportProjectItem(IProject project, string include) : base(project, ItemType.Import, include)
		{
		}
	}
}

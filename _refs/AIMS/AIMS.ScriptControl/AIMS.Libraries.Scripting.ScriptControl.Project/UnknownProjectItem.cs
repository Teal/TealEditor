using System;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public sealed class UnknownProjectItem : ProjectItem
	{
		internal UnknownProjectItem(IProject project, string itemType, string include) : this(project, itemType, include, false)
		{
		}

		internal UnknownProjectItem(IProject project, string itemType, string include, bool treatIncludeAsLiteral) : base(project, new ItemType(itemType), include, treatIncludeAsLiteral)
		{
		}
	}
}

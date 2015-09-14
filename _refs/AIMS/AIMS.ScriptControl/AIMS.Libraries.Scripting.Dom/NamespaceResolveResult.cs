using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.Dom
{
	public class NamespaceResolveResult : ResolveResult
	{
		private string name;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public NamespaceResolveResult(IClass callingClass, IMember callingMember, string name) : base(callingClass, callingMember, null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
		}

		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			return projectContent.GetNamespaceContents(this.name);
		}
	}
}

using AIMS.Libraries.Scripting.ScriptControl.Project;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class WebReferenceChanges
	{
		private List<ProjectItem> newItems = new List<ProjectItem>();

		private List<ProjectItem> itemsRemoved = new List<ProjectItem>();

		public List<ProjectItem> NewItems
		{
			get
			{
				return this.newItems;
			}
		}

		public List<ProjectItem> ItemsRemoved
		{
			get
			{
				return this.itemsRemoved;
			}
		}

		public bool Changed
		{
			get
			{
				return this.itemsRemoved.Count > 0 || this.newItems.Count > 0;
			}
		}
	}
}

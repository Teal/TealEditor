using System;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class ExplorerLabelEditEventArgs : EventArgs
	{
		public string OldName = "";

		public string NewName = "";

		public bool Cancel = false;

		public ExplorerLabelEditEventArgs(string newName, string oldName)
		{
			this.OldName = oldName;
			this.NewName = newName;
			this.Cancel = false;
		}
	}
}

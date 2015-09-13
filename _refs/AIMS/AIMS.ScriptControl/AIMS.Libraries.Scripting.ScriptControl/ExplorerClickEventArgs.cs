using System;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class ExplorerClickEventArgs : EventArgs
	{
		public string FileName = "";

		public ExplorerClickEventArgs(string fileName)
		{
			this.FileName = fileName;
		}
	}
}

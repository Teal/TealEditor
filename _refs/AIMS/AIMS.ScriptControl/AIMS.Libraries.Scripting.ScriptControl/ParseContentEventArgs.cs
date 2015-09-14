using System;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class ParseContentEventArgs : EventArgs
	{
		public string FileName = "";

		public string Content = "";

		public int Column = 0;

		public int Line = 0;

		public ParseContentEventArgs(string fileName, string content)
		{
			this.FileName = fileName;
			this.Content = content;
		}
	}
}

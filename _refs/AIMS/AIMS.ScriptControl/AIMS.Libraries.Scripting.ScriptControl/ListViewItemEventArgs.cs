using System;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class ListViewItemEventArgs : EventArgs
	{
		public string FileName = "";

		public int ColumnNo = 0;

		public int LineNo = 0;

		public ListViewItemEventArgs(string fileName, int lineNo, int colNo)
		{
			this.FileName = fileName;
			this.LineNo = lineNo;
			this.ColumnNo = colNo;
		}
	}
}

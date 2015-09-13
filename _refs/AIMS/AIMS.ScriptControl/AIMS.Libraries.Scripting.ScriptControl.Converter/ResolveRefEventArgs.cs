using System;

namespace AIMS.Libraries.Scripting.ScriptControl.Converter
{
	public class ResolveRefEventArgs : EventArgs
	{
		public string Message = "";

		public ResolveRefEventArgs(string message)
		{
			this.Message = message;
		}
	}
}

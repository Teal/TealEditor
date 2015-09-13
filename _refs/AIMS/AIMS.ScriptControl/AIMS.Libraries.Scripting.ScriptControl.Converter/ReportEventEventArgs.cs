using System;
using System.Runtime.InteropServices;

namespace AIMS.Libraries.Scripting.ScriptControl.Converter
{
	public class ReportEventEventArgs : EventArgs
	{
		public ImporterEventKind EventKind;

		public int EventCode;

		public string EventMsg;

		public ReportEventEventArgs(ImporterEventKind eventKind, int eventCode, string eventMsg)
		{
			this.EventKind = eventKind;
			this.EventCode = eventCode;
			this.EventMsg = eventMsg;
		}
	}
}

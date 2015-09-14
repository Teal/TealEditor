using AIMS.Libraries.Scripting.Dom;
using System;

namespace AIMS.Libraries.Scripting.ScriptControl.Parser
{
	internal class ProjectContentItem
	{
		private string filename = "";

		private string content = "";

		private bool isopened = false;

		private ParseInformation parseInfo = null;

		public string FileName
		{
			get
			{
				return this.filename;
			}
			set
			{
				this.filename = value;
			}
		}

		public bool IsOpened
		{
			get
			{
				return this.isopened;
			}
			set
			{
				this.isopened = value;
			}
		}

		public string Contents
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}

		public ParseInformation ParsedContents
		{
			get
			{
				return this.parseInfo;
			}
			set
			{
				this.parseInfo = value;
			}
		}

		public ProjectContentItem(string fileName) : this(fileName, "", false)
		{
		}

		public ProjectContentItem(string fileName, bool Isopened) : this(fileName, "", Isopened)
		{
		}

		public ProjectContentItem(string fileName, string filecontent) : this(fileName, filecontent, false)
		{
		}

		public ProjectContentItem(string fileName, string filecontent, bool Isopened)
		{
			this.filename = fileName;
			this.content = filecontent;
			this.isopened = Isopened;
		}
	}
}

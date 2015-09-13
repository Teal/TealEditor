using System;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	public class Errors
	{
		private int count = 0;

		public ErrorCodeProc SynErr;

		public ErrorCodeProc SemErr;

		public ErrorMsgProc Error;

		public int LineNo = 0;

		public int ColumnNo = 0;

		private StringBuilder errorText = new StringBuilder();

		public string ErrorOutput
		{
			get
			{
				return this.errorText.ToString();
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public Errors()
		{
			this.SynErr = new ErrorCodeProc(this.DefaultCodeError);
			this.SemErr = new ErrorCodeProc(this.DefaultCodeError);
			this.Error = new ErrorMsgProc(this.DefaultMsgError);
		}

		private void DefaultCodeError(int line, int col, int n)
		{
			this.LineNo = line;
			this.ColumnNo = col;
			this.errorText.AppendLine(string.Format("{0}", n));
			this.count++;
		}

		private void DefaultMsgError(int line, int col, string s)
		{
			this.LineNo = line;
			this.ColumnNo = col;
			this.errorText.AppendLine(string.Format("{0}", s));
			this.count++;
		}
	}
}

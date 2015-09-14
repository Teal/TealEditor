using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public class PreprocessingDirective : AbstractSpecial
	{
		private string cmd;

		private string arg;

		public string Cmd
		{
			get
			{
				return this.cmd;
			}
			set
			{
				this.cmd = value;
			}
		}

		public string Arg
		{
			get
			{
				return this.arg;
			}
			set
			{
				this.arg = value;
			}
		}

		public static void VBToCSharp(IList<ISpecial> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is PreprocessingDirective)
				{
					list[i] = PreprocessingDirective.VBToCSharp((PreprocessingDirective)list[i]);
				}
			}
		}

		public static PreprocessingDirective VBToCSharp(PreprocessingDirective dir)
		{
			string cmd = dir.Cmd;
			string arg = dir.Arg;
			if (cmd.Equals("#End", StringComparison.InvariantCultureIgnoreCase))
			{
				if (arg.ToLowerInvariant().StartsWith("region"))
				{
					cmd = "#endregion";
					arg = "";
				}
				else if ("if".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
				{
					cmd = "#endif";
					arg = "";
				}
			}
			else if (cmd.Equals("#Region", StringComparison.InvariantCultureIgnoreCase))
			{
				cmd = "#region";
			}
			else if (cmd.Equals("#If", StringComparison.InvariantCultureIgnoreCase))
			{
				if (arg.ToLowerInvariant().EndsWith(" then"))
				{
					arg = arg.Substring(0, arg.Length - 5);
				}
			}
			return new PreprocessingDirective(cmd, arg, dir.StartPosition, dir.EndPosition);
		}

		public static void CSharpToVB(List<ISpecial> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is PreprocessingDirective)
				{
					list[i] = PreprocessingDirective.CSharpToVB((PreprocessingDirective)list[i]);
				}
			}
		}

		public static PreprocessingDirective CSharpToVB(PreprocessingDirective dir)
		{
			string cmd = dir.Cmd;
			string arg = dir.Arg;
			string text = cmd;
			if (text != null)
			{
				if (!(text == "#region"))
				{
					if (!(text == "#endregion"))
					{
						if (!(text == "#endif"))
						{
							if (text == "#if")
							{
								arg += " Then";
							}
						}
						else
						{
							cmd = "#End";
							arg = "If";
						}
					}
					else
					{
						cmd = "#End";
						arg = "Region";
					}
				}
				else
				{
					cmd = "#Region";
					if (!arg.StartsWith("\""))
					{
						arg = "\"" + arg.Trim() + "\"";
					}
				}
			}
			if (cmd.Length > 1)
			{
				cmd = cmd.Substring(0, 2).ToUpperInvariant() + cmd.Substring(2);
			}
			return new PreprocessingDirective(cmd, arg, dir.StartPosition, dir.EndPosition);
		}

		public override string ToString()
		{
			return string.Format("[PreProcessingDirective: Cmd = {0}, Arg = {1}]", this.Cmd, this.Arg);
		}

		public PreprocessingDirective(string cmd, string arg, Location start, Location end) : base(start, end)
		{
			this.cmd = cmd;
			this.arg = arg;
		}

		public override object AcceptVisitor(ISpecialVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}

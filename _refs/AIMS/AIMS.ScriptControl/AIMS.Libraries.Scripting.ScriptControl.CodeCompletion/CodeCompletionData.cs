using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class CodeCompletionData : ICompletionData, IComparable
	{
		private IAmbience ambience;

		private AutoListIcons imageIndex;

		private int overloads;

		private string text;

		private string description;

		private string documentation;

		private IClass c;

		private IMember member;

		private bool convertedDocumentation = false;

		private double priority;

		private string dotnetName;

		internal static Regex whitespace = new Regex("\\s+");

		public IClass Class
		{
			get
			{
				return this.c;
			}
		}

		public IMember Member
		{
			get
			{
				return this.member;
			}
		}

		public int Overloads
		{
			get
			{
				return this.overloads;
			}
			set
			{
				this.overloads = value;
			}
		}

		public double Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
			}
		}

		public AutoListIcons ImageIndex
		{
			get
			{
				return this.imageIndex;
			}
			set
			{
				this.imageIndex = value;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		public string Description
		{
			get
			{
				string result;
				if (this.description.Length == 0 && (this.documentation == null || this.documentation.Length == 0))
				{
					result = "";
				}
				else
				{
					if (!this.convertedDocumentation && this.documentation != null)
					{
						this.convertedDocumentation = true;
						this.documentation = CodeCompletionData.GetDocumentation(this.documentation);
					}
					result = this.description + ((this.overloads > 0) ? (" Overloads " + this.overloads.ToString()) : string.Empty) + "\n" + this.documentation;
				}
				return result;
			}
			set
			{
				this.description = value;
			}
		}

		private void GetPriority(string dotnetName)
		{
			this.dotnetName = dotnetName;
			this.priority = CodeCompletionDataUsageCache.GetPriority(dotnetName, true);
		}

		public CodeCompletionData(string s, string desc, AutoListIcons imageIndex)
		{
			this.ambience = ProjectParser.CurrentAmbience;
			this.description = desc;
			this.documentation = string.Empty;
			this.text = s;
			this.imageIndex = imageIndex;
			this.GetPriority(s);
		}

		public CodeCompletionData(IClass c)
		{
			this.ambience = ProjectParser.CurrentAmbience;
			this.c = c;
			this.imageIndex = ScriptControl.GetIcon(c);
			this.ambience.ConversionFlags = ConversionFlags.None;
			this.text = this.ambience.Convert(c);
			this.ambience.ConversionFlags = (ConversionFlags.UseFullyQualifiedNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowReturnType);
			this.description = this.ambience.Convert(c);
			this.documentation = c.Documentation;
			this.GetPriority(c.DotNetName);
		}

		public CodeCompletionData(IMethod method)
		{
			this.member = method;
			this.ambience = ProjectParser.CurrentAmbience;
			this.ambience.ConversionFlags = (ConversionFlags.ShowParameterNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowReturnType);
			this.imageIndex = ScriptControl.GetIcon(method);
			this.text = method.Name;
			this.description = this.ambience.Convert(method);
			this.documentation = method.Documentation;
			this.GetPriority(method.DotNetName);
		}

		public CodeCompletionData(IField field)
		{
			this.member = field;
			this.ambience = ProjectParser.CurrentAmbience;
			this.ambience.ConversionFlags = (ConversionFlags.ShowParameterNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowReturnType);
			this.imageIndex = ScriptControl.GetIcon(field);
			this.text = field.Name;
			this.description = this.ambience.Convert(field);
			this.documentation = field.Documentation;
			this.GetPriority(field.DotNetName);
		}

		public CodeCompletionData(IProperty property)
		{
			this.member = property;
			this.ambience = ProjectParser.CurrentAmbience;
			this.ambience.ConversionFlags = (ConversionFlags.ShowParameterNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowReturnType);
			this.imageIndex = ScriptControl.GetIcon(property);
			this.text = property.Name;
			this.description = this.ambience.Convert(property);
			this.documentation = property.Documentation;
			this.GetPriority(property.DotNetName);
		}

		public CodeCompletionData(IEvent e)
		{
			this.member = e;
			this.ambience = ProjectParser.CurrentAmbience;
			this.ambience.ConversionFlags = (ConversionFlags.ShowParameterNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowReturnType);
			this.imageIndex = ScriptControl.GetIcon(e);
			this.text = e.Name;
			this.description = this.ambience.Convert(e);
			this.documentation = e.Documentation;
			this.GetPriority(e.DotNetName);
		}

		public bool InsertAction(EditViewControl textArea, char ch)
		{
			if (this.dotnetName != null)
			{
				CodeCompletionDataUsageCache.IncrementUsage(this.dotnetName);
			}
			bool result;
			if (this.c != null && this.text.Length > this.c.Name.Length)
			{
				textArea.InsertText(this.text.Substring(0, this.c.Name.Length + 1));
				TextPoint start = textArea.Caret.Position;
				int pos = this.text.IndexOf(',');
				TextPoint end;
				if (pos < 0)
				{
					textArea.InsertText(this.text.Substring(this.c.Name.Length + 1));
					end = textArea.Caret.Position;
					end.X--;
				}
				else
				{
					textArea.InsertText(this.text.Substring(this.c.Name.Length + 1, pos - this.c.Name.Length - 1));
					end = textArea.Caret.Position;
					textArea.InsertText(this.text.Substring(pos));
				}
				textArea.Caret.Position = start;
				textArea.Selection.SelStart = start.X;
				textArea.Selection.SelEnd = end.X;
				if (!char.IsLetterOrDigit(ch))
				{
					result = true;
					return result;
				}
			}
			else
			{
				textArea.InsertText(this.text);
			}
			result = false;
			return result;
		}

		public static string GetDocumentation(string doc)
		{
			StringReader reader = new StringReader("<docroot>" + doc + "</docroot>");
			XmlTextReader xml = new XmlTextReader(reader);
			StringBuilder ret = new StringBuilder();
			string result;
			try
			{
				xml.Read();
				do
				{
					if (xml.NodeType == XmlNodeType.Element)
					{
						string elname = xml.Name.ToLowerInvariant();
						string text = elname;
						switch (text)
						{
						case "filterpriority":
							xml.Skip();
							break;
						case "remarks":
							ret.Append(Environment.NewLine);
							ret.Append("Remarks:");
							ret.Append(Environment.NewLine);
							break;
						case "example":
							ret.Append(Environment.NewLine);
							ret.Append("Example:");
							ret.Append(Environment.NewLine);
							break;
						case "exception":
							ret.Append(Environment.NewLine);
							ret.Append(CodeCompletionData.GetCref(xml["cref"]));
							ret.Append(": ");
							break;
						case "returns":
							ret.Append(Environment.NewLine);
							ret.Append("Returns: ");
							break;
						case "see":
							ret.Append(CodeCompletionData.GetCref(xml["cref"]));
							ret.Append(xml["langword"]);
							break;
						case "seealso":
							ret.Append(Environment.NewLine);
							ret.Append("See also: ");
							ret.Append(CodeCompletionData.GetCref(xml["cref"]));
							break;
						case "paramref":
							ret.Append(xml["name"]);
							break;
						case "param":
							ret.Append(Environment.NewLine);
							ret.Append(CodeCompletionData.whitespace.Replace(xml["name"].Trim(), " "));
							ret.Append(": ");
							break;
						case "value":
							ret.Append(Environment.NewLine);
							ret.Append("Value: ");
							ret.Append(Environment.NewLine);
							break;
						case "br":
						case "para":
							ret.Append(Environment.NewLine);
							break;
						}
					}
					else if (xml.NodeType == XmlNodeType.Text)
					{
						ret.Append(CodeCompletionData.whitespace.Replace(xml.Value, " "));
					}
				}
				while (xml.Read());
			}
			catch
			{
				result = doc;
				return result;
			}
			result = ret.ToString();
			return result;
		}

		private static string GetCref(string cref)
		{
			string result;
			if (cref == null || cref.Trim().Length == 0)
			{
				result = "";
			}
			else if (cref.Length < 2)
			{
				result = cref;
			}
			else if (cref.Substring(1, 1) == ":")
			{
				result = cref.Substring(2, cref.Length - 2);
			}
			else
			{
				result = cref;
			}
			return result;
		}

		public int CompareTo(object obj)
		{
			int result;
			if (obj == null || !(obj is CodeCompletionData))
			{
				result = -1;
			}
			else
			{
				result = this.text.CompareTo(((CodeCompletionData)obj).text);
			}
			return result;
		}
	}
}

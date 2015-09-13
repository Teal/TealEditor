using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.PrettyPrinter;
using System;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public class CSharpCodeGenerator : NRefactoryCodeGenerator
	{
		internal static readonly CSharpCodeGenerator Instance = new CSharpCodeGenerator();

		public override IOutputAstVisitor CreateOutputVisitor()
		{
			CSharpOutputVisitor v = new CSharpOutputVisitor();
			PrettyPrintOptions pOpt = v.Options;
			BraceStyle braceStyle;
			if (base.Options.BracesOnSameLine)
			{
				braceStyle = BraceStyle.EndOfLine;
			}
			else
			{
				braceStyle = BraceStyle.NextLine;
			}
			pOpt.StatementBraceStyle = braceStyle;
			pOpt.EventAddBraceStyle = braceStyle;
			pOpt.EventRemoveBraceStyle = braceStyle;
			pOpt.PropertyBraceStyle = braceStyle;
			pOpt.PropertyGetBraceStyle = braceStyle;
			pOpt.PropertySetBraceStyle = braceStyle;
			pOpt.IndentationChar = base.Options.IndentString[0];
			pOpt.IndentSize = base.Options.IndentString.Length;
			pOpt.TabSize = base.Options.IndentString.Length;
			return v;
		}

		public override void InsertCodeAtEnd(DomRegion region, IDocument document, params AbstractNode[] nodes)
		{
			string beginLineIndentation = base.GetIndentation(document, region.BeginLine);
			int insertionLine = region.EndLine - 1;
			IDocumentLine endLine = document.GetLine(region.EndLine);
			string endLineText = endLine.Text;
			int originalPos = region.EndColumn - 2;
			int pos = originalPos;
			if (pos >= endLineText.Length || endLineText[pos] != '}')
			{
				LoggingService.Warn(string.Concat(new object[]
				{
					"CSharpCodeGenerator.InsertCodeAtEnd: position is invalid (not pointing to '}') endLineText=",
					endLineText,
					", pos=",
					pos
				}));
			}
			else
			{
				for (pos--; pos >= 0; pos--)
				{
					if (!char.IsWhiteSpace(endLineText[pos]))
					{
						pos++;
						if (pos < originalPos)
						{
							document.Remove(endLine.Offset + pos, originalPos - pos);
						}
						document.Insert(endLine.Offset + pos, Environment.NewLine + beginLineIndentation);
						insertionLine++;
						pos = region.BeginColumn - 1;
						if (region.BeginLine == region.EndLine && pos >= 1 && pos < endLineText.Length)
						{
							pos = (originalPos = endLineText.IndexOf('{', pos));
							if (pos >= 0 && pos < region.EndColumn - 1)
							{
								originalPos++;
								for (pos++; pos < endLineText.Length; pos++)
								{
									if (!char.IsWhiteSpace(endLineText[pos]))
									{
										if (originalPos < pos)
										{
											document.Remove(endLine.Offset + originalPos, pos - originalPos);
										}
										document.Insert(endLine.Offset + originalPos, Environment.NewLine + beginLineIndentation + '\t');
										insertionLine++;
										break;
									}
								}
							}
						}
						break;
					}
				}
			}
			base.InsertCodeAfter(insertionLine, document, beginLineIndentation + base.Options.IndentString, nodes);
		}
	}
}

using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.PrettyPrinter;
using System;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public abstract class NRefactoryCodeGenerator : CodeGenerator
	{
		public abstract IOutputAstVisitor CreateOutputVisitor();

		public override string GenerateCode(AbstractNode node, string indentation)
		{
			IOutputAstVisitor visitor = this.CreateOutputVisitor();
			int indentCount = 0;
			for (int i = 0; i < indentation.Length; i++)
			{
				char c = indentation[i];
				if (c == '\t')
				{
					indentCount += 4;
				}
				else
				{
					indentCount++;
				}
			}
			visitor.OutputFormatter.IndentationLevel = indentCount / 4;
			if (node is Statement)
			{
				visitor.OutputFormatter.Indent();
			}
			node.AcceptVisitor(visitor, null);
			string text = visitor.Text;
			if (node is Statement && !text.EndsWith("\n"))
			{
				text += Environment.NewLine;
			}
			return text;
		}
	}
}

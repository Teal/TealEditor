using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.PrettyPrinter;
using System;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public class VBNetCodeGenerator : NRefactoryCodeGenerator
	{
		internal static readonly VBNetCodeGenerator Instance = new VBNetCodeGenerator();

		public override IOutputAstVisitor CreateOutputVisitor()
		{
			VBNetOutputVisitor v = new VBNetOutputVisitor();
			VBNetPrettyPrintOptions pOpt = v.Options;
			pOpt.IndentationChar = base.Options.IndentString[0];
			pOpt.IndentSize = base.Options.IndentString.Length;
			pOpt.TabSize = base.Options.IndentString.Length;
			return v;
		}

		public override PropertyDeclaration CreateProperty(IField field, bool createGetter, bool createSetter)
		{
			string propertyName = this.GetPropertyName(field.Name);
			if (string.Equals(propertyName, field.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				if (HostCallback.RenameMember(field, "m_" + field.Name))
				{
					field = new DefaultField(field.ReturnType, "m_" + field.Name, field.Modifiers, field.Region, field.DeclaringType);
				}
			}
			return base.CreateProperty(field, createGetter, createSetter);
		}
	}
}

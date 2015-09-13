using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	[Obsolete("Use VBNetConstructsConvertVisitor + ToCSharpConvertVisitor instead")]
	public class VBNetToCSharpConvertVisitor : VBNetConstructsConvertVisitor
	{
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			compilationUnit.AcceptVisitor(new ToCSharpConvertVisitor(), data);
			return null;
		}
	}
}

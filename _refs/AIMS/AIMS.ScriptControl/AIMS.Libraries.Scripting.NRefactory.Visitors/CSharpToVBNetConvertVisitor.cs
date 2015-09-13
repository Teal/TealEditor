using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	[Obsolete("Use CSharpConstructsVisitor + ToVBNetConvertVisitor instead")]
	public class CSharpToVBNetConvertVisitor : CSharpConstructsVisitor
	{
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			compilationUnit.AcceptVisitor(new ToVBNetConvertVisitor(), data);
			return null;
		}
	}
}

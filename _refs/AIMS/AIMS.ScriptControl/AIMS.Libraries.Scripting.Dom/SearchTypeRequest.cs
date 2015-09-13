using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public struct SearchTypeRequest
	{
		public readonly string Name;

		public readonly int TypeParameterCount;

		public readonly ICompilationUnit CurrentCompilationUnit;

		public readonly IClass CurrentType;

		public readonly int CaretLine;

		public readonly int CaretColumn;

		public SearchTypeRequest(string name, int typeParameterCount, IClass currentType, int caretLine, int caretColumn)
		{
			if (currentType == null)
			{
				throw new ArgumentNullException("currentType");
			}
			this.Name = name;
			this.TypeParameterCount = typeParameterCount;
			this.CurrentCompilationUnit = currentType.CompilationUnit;
			this.CurrentType = currentType;
			this.CaretLine = caretLine;
			this.CaretColumn = caretColumn;
		}

		public SearchTypeRequest(string name, int typeParameterCount, IClass currentType, ICompilationUnit currentCompilationUnit, int caretLine, int caretColumn)
		{
			if (currentCompilationUnit == null)
			{
				throw new ArgumentNullException("currentCompilationUnit");
			}
			this.Name = name;
			this.TypeParameterCount = typeParameterCount;
			this.CurrentCompilationUnit = currentCompilationUnit;
			this.CurrentType = currentType;
			this.CaretLine = caretLine;
			this.CaretColumn = caretColumn;
		}
	}
}

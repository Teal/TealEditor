using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class ParseInformation
	{
		private ICompilationUnit validCompilationUnit;

		private ICompilationUnit dirtyCompilationUnit;

		public ICompilationUnit ValidCompilationUnit
		{
			get
			{
				return this.validCompilationUnit;
			}
			set
			{
				this.validCompilationUnit = value;
			}
		}

		public ICompilationUnit DirtyCompilationUnit
		{
			get
			{
				return this.dirtyCompilationUnit;
			}
			set
			{
				this.dirtyCompilationUnit = value;
			}
		}

		public ICompilationUnit BestCompilationUnit
		{
			get
			{
				return (this.validCompilationUnit == null) ? this.dirtyCompilationUnit : this.validCompilationUnit;
			}
		}

		public ICompilationUnit MostRecentCompilationUnit
		{
			get
			{
				return (this.dirtyCompilationUnit == null) ? this.validCompilationUnit : this.dirtyCompilationUnit;
			}
		}
	}
}

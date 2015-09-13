using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public abstract class RefactoringProvider
	{
		private class DummyRefactoringProvider : RefactoringProvider
		{
			public override bool IsEnabledForFile(string fileName)
			{
				return false;
			}
		}

		public static readonly RefactoringProvider DummyProvider = new RefactoringProvider.DummyRefactoringProvider();

		public virtual bool SupportsFindUnusedUsingDeclarations
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportsCreateNewFileLikeExisting
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportsGetFullCodeRangeForType
		{
			get
			{
				return false;
			}
		}

		public abstract bool IsEnabledForFile(string fileName);

		public virtual IList<IUsing> FindUnusedUsingDeclarations(string fileName, string fileContent, ICompilationUnit compilationUnit)
		{
			throw new NotSupportedException();
		}

		public virtual string CreateNewFileLikeExisting(string existingFileContent, string codeForNewType)
		{
			throw new NotSupportedException();
		}

		public virtual DomRegion GetFullCodeRangeForType(string fileContent, IClass type)
		{
			throw new NotSupportedException();
		}
	}
}

using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class ClassFinder
	{
		private int caretLine;

		private int caretColumn;

		private ICompilationUnit cu;

		private IClass callingClass;

		private IProjectContent projectContent;

		public IClass CallingClass
		{
			get
			{
				return this.callingClass;
			}
		}

		public IProjectContent ProjectContent
		{
			get
			{
				return this.projectContent;
			}
		}

		public LanguageProperties Language
		{
			get
			{
				return this.projectContent.Language;
			}
		}

		public ClassFinder(string fileName, string fileContent, int offset)
		{
			this.caretLine = 0;
			this.caretColumn = 0;
			for (int i = 0; i < offset; i++)
			{
				if (fileContent[i] == '\n')
				{
					this.caretLine++;
					this.caretColumn = 0;
				}
				else
				{
					this.caretColumn++;
				}
			}
			this.Init(fileName);
		}

		public ClassFinder(string fileName, int caretLineNumber, int caretColumn)
		{
			this.caretLine = caretLineNumber;
			this.caretColumn = caretColumn;
			this.Init(fileName);
		}

		public ClassFinder(IMember classMember) : this(classMember.DeclaringType, classMember.Region.BeginLine, classMember.Region.BeginColumn)
		{
		}

		public ClassFinder(IClass callingClass, int caretLine, int caretColumn)
		{
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
			this.callingClass = callingClass;
			this.cu = callingClass.CompilationUnit;
			this.projectContent = this.cu.ProjectContent;
			if (this.projectContent == null)
			{
				throw new ArgumentException("callingClass must have a project content!");
			}
		}

		public ClassFinder(IClass callingClass, IMember callingMember, int caretLine, int caretColumn) : this(callingClass, caretLine, caretColumn)
		{
		}

		private void Init(string fileName)
		{
			ParseInformation parseInfo = HostCallback.GetParseInformation(fileName);
			if (parseInfo != null)
			{
				this.cu = parseInfo.MostRecentCompilationUnit;
			}
			if (this.cu != null)
			{
				this.callingClass = this.cu.GetInnermostClass(this.caretLine, this.caretColumn);
				this.projectContent = this.cu.ProjectContent;
			}
			else
			{
				this.projectContent = HostCallback.GetCurrentProjectContent();
			}
			if (this.projectContent == null)
			{
				throw new ArgumentException("projectContent not found!");
			}
		}

		public IClass GetClass(string fullName, int typeParameterCount)
		{
			return this.projectContent.GetClass(fullName, typeParameterCount);
		}

		public IReturnType SearchType(string name, int typeParameterCount)
		{
			return this.projectContent.SearchType(new SearchTypeRequest(name, typeParameterCount, this.callingClass, this.cu, this.caretLine, this.caretColumn)).Result;
		}

		public string SearchNamespace(string name)
		{
			return this.projectContent.SearchNamespace(name, this.callingClass, this.cu, this.caretLine, this.caretColumn);
		}
	}
}

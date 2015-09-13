using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface ICompilationUnit
	{
		string FileName
		{
			get;
			set;
		}

		bool ErrorsDuringCompile
		{
			get;
			set;
		}

		object Tag
		{
			get;
			set;
		}

		IProjectContent ProjectContent
		{
			get;
		}

		List<IUsing> Usings
		{
			get;
		}

		List<IAttribute> Attributes
		{
			get;
		}

		List<IClass> Classes
		{
			get;
		}

		List<IComment> MiscComments
		{
			get;
		}

		List<IComment> DokuComments
		{
			get;
		}

		List<TagComment> TagComments
		{
			get;
		}

		List<FoldingRegion> FoldingRegions
		{
			get;
		}

		IClass GetInnermostClass(int caretLine, int caretColumn);

		List<IClass> GetOuterClasses(int caretLine, int caretColumn);
	}
}

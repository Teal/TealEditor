using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultCompilationUnit : ICompilationUnit
	{
		public static readonly ICompilationUnit DummyCompilationUnit = new DefaultCompilationUnit(DefaultProjectContent.DummyProjectContent);

		private List<IUsing> usings = new List<IUsing>();

		private List<IClass> classes = new List<IClass>();

		private List<IAttribute> attributes = new List<IAttribute>();

		private List<FoldingRegion> foldingRegions = new List<FoldingRegion>();

		private List<TagComment> tagComments = new List<TagComment>();

		private bool errorsDuringCompile = false;

		private object tag = null;

		private string fileName = null;

		private IProjectContent projectContent;

		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		public IProjectContent ProjectContent
		{
			[DebuggerStepThrough]
			get
			{
				return this.projectContent;
			}
		}

		public bool ErrorsDuringCompile
		{
			get
			{
				return this.errorsDuringCompile;
			}
			set
			{
				this.errorsDuringCompile = value;
			}
		}

		public object Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		public virtual List<IUsing> Usings
		{
			get
			{
				return this.usings;
			}
		}

		public virtual List<IAttribute> Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		public virtual List<IClass> Classes
		{
			get
			{
				return this.classes;
			}
		}

		public List<FoldingRegion> FoldingRegions
		{
			get
			{
				return this.foldingRegions;
			}
		}

		public virtual List<IComment> MiscComments
		{
			get
			{
				return null;
			}
		}

		public virtual List<IComment> DokuComments
		{
			get
			{
				return null;
			}
		}

		public virtual List<TagComment> TagComments
		{
			get
			{
				return this.tagComments;
			}
		}

		public DefaultCompilationUnit(IProjectContent projectContent)
		{
			Debug.Assert(projectContent != null);
			this.projectContent = projectContent;
		}

		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			IClass result;
			foreach (IClass c in this.Classes)
			{
				if (c != null && c.Region.IsInside(caretLine, caretColumn))
				{
					result = c.GetInnermostClass(caretLine, caretColumn);
					return result;
				}
			}
			result = null;
			return result;
		}

		public List<IClass> GetOuterClasses(int caretLine, int caretColumn)
		{
			List<IClass> classes = new List<IClass>();
			IClass innerMostClass = this.GetInnermostClass(caretLine, caretColumn);
			foreach (IClass c in this.Classes)
			{
				if (c != null && c.Region.IsInside(caretLine, caretColumn))
				{
					if (c != innerMostClass)
					{
						this.GetOuterClasses(classes, c, caretLine, caretColumn);
						if (!classes.Contains(c))
						{
							classes.Add(c);
						}
					}
					break;
				}
			}
			return classes;
		}

		private void GetOuterClasses(List<IClass> classes, IClass curClass, int caretLine, int caretColumn)
		{
			if (curClass != null && curClass.InnerClasses.Count > 0)
			{
				IClass innerMostClass = this.GetInnermostClass(caretLine, caretColumn);
				foreach (IClass c in curClass.InnerClasses)
				{
					if (c != null && c.Region.IsInside(caretLine, caretColumn))
					{
						if (c != innerMostClass)
						{
							this.GetOuterClasses(classes, c, caretLine, caretColumn);
							if (!classes.Contains(c))
							{
								classes.Add(c);
							}
						}
						break;
					}
				}
			}
		}

		public override string ToString()
		{
			return string.Format("[CompilationUnit: classes = {0}, fileName = {1}]", this.classes.Count, this.fileName);
		}
	}
}

using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.Dom
{
	public class TypeResolveResult : ResolveResult
	{
		private IClass resolvedClass;

		public IClass ResolvedClass
		{
			get
			{
				return this.resolvedClass;
			}
		}

		public TypeResolveResult(IClass callingClass, IMember callingMember, IClass resolvedClass) : base(callingClass, callingMember, resolvedClass.DefaultReturnType)
		{
			this.resolvedClass = resolvedClass;
		}

		public TypeResolveResult(IClass callingClass, IMember callingMember, IReturnType resolvedType, IClass resolvedClass) : base(callingClass, callingMember, resolvedType)
		{
			this.resolvedClass = resolvedClass;
		}

		public TypeResolveResult(IClass callingClass, IMember callingMember, IReturnType resolvedType) : base(callingClass, callingMember, resolvedType)
		{
			this.resolvedClass = resolvedType.GetUnderlyingClass();
		}

		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			ArrayList ar = base.GetCompletionData(projectContent.Language, true);
			if (this.resolvedClass != null)
			{
				foreach (IClass baseClass in this.resolvedClass.ClassInheritanceTree)
				{
					ar.AddRange(baseClass.InnerClasses);
				}
			}
			return ar;
		}

		public override FilePosition GetDefinitionPosition()
		{
			FilePosition result;
			if (this.resolvedClass == null)
			{
				result = FilePosition.Empty;
			}
			else
			{
				ICompilationUnit cu = this.resolvedClass.CompilationUnit;
				if (cu == null || cu.FileName == null || cu.FileName.Length == 0)
				{
					result = FilePosition.Empty;
				}
				else
				{
					DomRegion reg = this.resolvedClass.Region;
					if (!reg.IsEmpty)
					{
						result = new FilePosition(cu.FileName, reg.BeginLine, reg.BeginColumn);
					}
					else
					{
						result = new FilePosition(cu.FileName);
					}
				}
			}
			return result;
		}
	}
}

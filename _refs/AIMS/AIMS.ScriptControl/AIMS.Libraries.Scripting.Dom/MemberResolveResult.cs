using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class MemberResolveResult : ResolveResult
	{
		private IMember resolvedMember;

		public IMember ResolvedMember
		{
			get
			{
				return this.resolvedMember;
			}
		}

		public MemberResolveResult(IClass callingClass, IMember callingMember, IMember resolvedMember) : base(callingClass, callingMember, resolvedMember.ReturnType)
		{
			if (resolvedMember == null)
			{
				throw new ArgumentNullException("resolvedMember");
			}
			this.resolvedMember = resolvedMember;
		}

		public override FilePosition GetDefinitionPosition()
		{
			return MemberResolveResult.GetDefinitionPosition(this.resolvedMember);
		}

		internal static FilePosition GetDefinitionPosition(IMember resolvedMember)
		{
			IClass declaringType = resolvedMember.DeclaringType;
			FilePosition result;
			if (declaringType == null)
			{
				result = FilePosition.Empty;
			}
			else
			{
				ICompilationUnit cu = declaringType.CompilationUnit;
				if (cu == null)
				{
					result = FilePosition.Empty;
				}
				else if (cu.FileName == null || cu.FileName.Length == 0)
				{
					result = FilePosition.Empty;
				}
				else
				{
					DomRegion reg = resolvedMember.Region;
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

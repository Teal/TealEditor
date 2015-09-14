using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.Dom
{
	public class IntegerLiteralResolveResult : ResolveResult
	{
		public IntegerLiteralResolveResult(IClass callingClass, IMember callingMember, IReturnType systemInt32) : base(callingClass, callingMember, systemInt32)
		{
		}

		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			return null;
		}
	}
}

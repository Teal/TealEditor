using System;
using System.Collections;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class MixedResolveResult : ResolveResult
	{
		private ResolveResult primaryResult;

		private ResolveResult secondaryResult;

		public ResolveResult PrimaryResult
		{
			get
			{
				return this.primaryResult;
			}
		}

		public IEnumerable<ResolveResult> Results
		{
			get
			{
				yield return this.primaryResult;
				yield return this.secondaryResult;
				yield break;
			}
		}

		public TypeResolveResult TypeResult
		{
			get
			{
				TypeResolveResult result;
				if (this.primaryResult is TypeResolveResult)
				{
					result = (TypeResolveResult)this.primaryResult;
				}
				else if (this.secondaryResult is TypeResolveResult)
				{
					result = (TypeResolveResult)this.secondaryResult;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public MixedResolveResult(ResolveResult primaryResult, ResolveResult secondaryResult) : base(primaryResult.CallingClass, primaryResult.CallingMember, primaryResult.ResolvedType)
		{
			this.primaryResult = primaryResult;
			this.secondaryResult = secondaryResult;
		}

		public override FilePosition GetDefinitionPosition()
		{
			return this.primaryResult.GetDefinitionPosition();
		}

		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			ArrayList result = this.primaryResult.GetCompletionData(projectContent);
			ArrayList result2 = this.secondaryResult.GetCompletionData(projectContent);
			ArrayList result3;
			if (result == null)
			{
				result3 = result2;
			}
			else if (result2 == null)
			{
				result3 = result;
			}
			else
			{
				foreach (object o in result2)
				{
					if (!result.Contains(o))
					{
						result.Add(o);
					}
				}
				result3 = result;
			}
			return result3;
		}
	}
}

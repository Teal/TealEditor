using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public struct SearchTypeResult
	{
		public static readonly SearchTypeResult Empty = new SearchTypeResult(null);

		private readonly IReturnType result;

		private readonly IUsing usedUsing;

		public IReturnType Result
		{
			get
			{
				return this.result;
			}
		}

		public IUsing UsedUsing
		{
			get
			{
				return this.usedUsing;
			}
		}

		public SearchTypeResult(IReturnType result)
		{
			this = new SearchTypeResult(result, null);
		}

		public SearchTypeResult(IReturnType result, IUsing usedUsing)
		{
			this.result = result;
			this.usedUsing = usedUsing;
		}
	}
}

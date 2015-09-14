using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public class LocalLookupVariable
	{
		private TypeReference typeRef;

		private Location startPos;

		private Location endPos;

		private bool isConst;

		public TypeReference TypeRef
		{
			get
			{
				return this.typeRef;
			}
		}

		public Location StartPos
		{
			get
			{
				return this.startPos;
			}
		}

		public Location EndPos
		{
			get
			{
				return this.endPos;
			}
		}

		public bool IsConst
		{
			get
			{
				return this.isConst;
			}
		}

		public LocalLookupVariable(TypeReference typeRef, Location startPos, Location endPos, bool isConst)
		{
			this.typeRef = typeRef;
			this.startPos = startPos;
			this.endPos = endPos;
			this.isConst = isConst;
		}
	}
}

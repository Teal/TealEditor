using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullTypeReference : TypeReference
	{
		private static NullTypeReference nullTypeReference = new NullTypeReference();

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		public static NullTypeReference Instance
		{
			get
			{
				return NullTypeReference.nullTypeReference;
			}
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		private NullTypeReference()
		{
		}

		public override string ToString()
		{
			return string.Format("[NullTypeReference]", new object[0]);
		}
	}
}

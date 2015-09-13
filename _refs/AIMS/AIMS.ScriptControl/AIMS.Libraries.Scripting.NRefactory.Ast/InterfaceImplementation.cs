using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class InterfaceImplementation : AbstractNode
	{
		private TypeReference interfaceType;

		private string memberName;

		public TypeReference InterfaceType
		{
			get
			{
				return this.interfaceType;
			}
			set
			{
				this.interfaceType = (value ?? TypeReference.Null);
			}
		}

		public string MemberName
		{
			get
			{
				return this.memberName;
			}
			set
			{
				this.memberName = (string.IsNullOrEmpty(value) ? "?" : value);
			}
		}

		public InterfaceImplementation(TypeReference interfaceType, string memberName)
		{
			this.InterfaceType = interfaceType;
			this.MemberName = memberName;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitInterfaceImplementation(this, data);
		}

		public override string ToString()
		{
			return string.Format("[InterfaceImplementation InterfaceType={0} MemberName={1}]", this.InterfaceType, this.MemberName);
		}
	}
}

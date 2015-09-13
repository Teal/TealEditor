using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class ExplicitInterfaceImplementation : IEquatable<ExplicitInterfaceImplementation>
	{
		private readonly IReturnType interfaceReference;

		private readonly string memberName;

		public IReturnType InterfaceReference
		{
			get
			{
				return this.interfaceReference;
			}
		}

		public string MemberName
		{
			get
			{
				return this.memberName;
			}
		}

		public ExplicitInterfaceImplementation(IReturnType interfaceReference, string memberName)
		{
			this.interfaceReference = interfaceReference;
			this.memberName = memberName;
		}

		public ExplicitInterfaceImplementation Clone()
		{
			return this;
		}

		public override int GetHashCode()
		{
			return this.interfaceReference.GetHashCode() ^ this.memberName.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ExplicitInterfaceImplementation);
		}

		public bool Equals(ExplicitInterfaceImplementation other)
		{
			return other != null && this.interfaceReference == other.interfaceReference && this.memberName == other.memberName;
		}
	}
}

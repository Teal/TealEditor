using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class MethodResolveResult : ResolveResult
	{
		private string name;

		private IReturnType containingType;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public IReturnType ContainingType
		{
			get
			{
				return this.containingType;
			}
		}

		public MethodResolveResult(IClass callingClass, IMember callingMember, IReturnType containingType, string name) : base(callingClass, callingMember, null)
		{
			if (containingType == null)
			{
				throw new ArgumentNullException("containingType");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.containingType = containingType;
			this.name = name;
		}

		public IMethod GetMethodIfSingleOverload()
		{
			List<IMethod> methods = this.containingType.GetMethods();
			methods = methods.FindAll((IMethod m) => m.Name == this.Name);
			IMethod result;
			if (methods.Count == 1)
			{
				result = methods[0];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public override FilePosition GetDefinitionPosition()
		{
			IMethod i = this.GetMethodIfSingleOverload();
			FilePosition definitionPosition;
			if (i != null)
			{
				definitionPosition = MemberResolveResult.GetDefinitionPosition(i);
			}
			else
			{
				definitionPosition = base.GetDefinitionPosition();
			}
			return definitionPosition;
		}
	}
}

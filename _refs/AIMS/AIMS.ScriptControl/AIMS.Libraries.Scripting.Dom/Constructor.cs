using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class Constructor : DefaultMethod
	{
		public Constructor(ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType) : base("#ctor", declaringType.DefaultReturnType, m, region, bodyRegion, declaringType)
		{
		}

		public Constructor(ModifierEnum m, IReturnType returnType, IClass declaringType) : base("#ctor", returnType, m, DomRegion.Empty, DomRegion.Empty, declaringType)
		{
		}

		public static Constructor CreateDefault(IClass c)
		{
			return new Constructor(ModifierEnum.Public, c.Region, c.Region, c)
			{
				Documentation = "Default constructor of " + c.Name
			};
		}
	}
}

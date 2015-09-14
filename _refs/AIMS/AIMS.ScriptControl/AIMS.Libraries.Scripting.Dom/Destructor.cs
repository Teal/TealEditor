using System;

namespace AIMS.Libraries.Scripting.Dom
{
	[Serializable]
	public class Destructor : DefaultMethod
	{
		public Destructor(DomRegion region, DomRegion bodyRegion, IClass declaringType) : base("#dtor", null, ModifierEnum.None, region, bodyRegion, declaringType)
		{
		}
	}
}

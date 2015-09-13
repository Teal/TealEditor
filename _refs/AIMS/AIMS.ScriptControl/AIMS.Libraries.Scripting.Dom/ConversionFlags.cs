using System;

namespace AIMS.Libraries.Scripting.Dom
{
	[Flags]
	public enum ConversionFlags
	{
		None = 0,
		ShowParameterNames = 1,
		ShowAccessibility = 16,
		UseFullyQualifiedNames = 2,
		ShowModifiers = 4,
		ShowInheritanceList = 8,
		IncludeHTMLMarkup = 32,
		QualifiedNamesOnlyForReturnTypes = 128,
		IncludeBodies = 256,
		ShowReturnType = 512,
		StandardConversionFlags = 519,
		All = 543
	}
}

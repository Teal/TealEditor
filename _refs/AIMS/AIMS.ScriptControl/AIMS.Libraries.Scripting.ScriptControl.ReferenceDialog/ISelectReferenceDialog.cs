using System;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public interface ISelectReferenceDialog
	{
		void AddReference(ReferenceType referenceType, string referenceName, string referenceLocation, object tag);
	}
}

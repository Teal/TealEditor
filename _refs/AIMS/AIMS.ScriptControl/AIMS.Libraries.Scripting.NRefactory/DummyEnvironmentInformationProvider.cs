using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
	internal class DummyEnvironmentInformationProvider : IEnvironmentInformationProvider
	{
		public bool HasField(string fullTypeName, string fieldName)
		{
			return false;
		}
	}
}

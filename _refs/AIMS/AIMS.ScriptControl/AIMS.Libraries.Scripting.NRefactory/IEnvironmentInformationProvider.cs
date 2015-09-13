using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public interface IEnvironmentInformationProvider
	{
		bool HasField(string fullTypeName, string fieldName);
	}
}

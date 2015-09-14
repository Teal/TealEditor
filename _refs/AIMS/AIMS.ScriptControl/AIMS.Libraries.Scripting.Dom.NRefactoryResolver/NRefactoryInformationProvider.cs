using AIMS.Libraries.Scripting.NRefactory;
using System;

namespace AIMS.Libraries.Scripting.Dom.NRefactoryResolver
{
	public class NRefactoryInformationProvider : IEnvironmentInformationProvider
	{
		private IProjectContent pc;

		private IClass callingClass;

		public NRefactoryInformationProvider(IProjectContent pc, IClass callingClass)
		{
			this.pc = pc;
			this.callingClass = callingClass;
		}

		public bool HasField(string fullTypeName, string fieldName)
		{
			IClass c = this.pc.GetClass(fullTypeName);
			bool result;
			if (c == null)
			{
				result = false;
			}
			else
			{
				foreach (IField field in c.DefaultReturnType.GetFields())
				{
					if (field.Name == fieldName)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}
	}
}

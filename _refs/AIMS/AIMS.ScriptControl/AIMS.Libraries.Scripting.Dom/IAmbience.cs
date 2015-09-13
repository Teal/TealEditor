using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IAmbience
	{
		ConversionFlags ConversionFlags
		{
			get;
			set;
		}

		string Convert(ModifierEnum modifier);

		string Convert(IClass c);

		string ConvertEnd(IClass c);

		string Convert(IField field);

		string Convert(IProperty property);

		string Convert(IEvent e);

		string Convert(IMethod m);

		string ConvertEnd(IMethod m);

		string Convert(IParameter param);

		string Convert(IReturnType returnType);

		string WrapAttribute(string attribute);

		string WrapComment(string comment);

		string GetIntrinsicTypeName(string dotNetTypeName);
	}
}

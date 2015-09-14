using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IReturnType
	{
		string FullyQualifiedName
		{
			get;
		}

		string Name
		{
			get;
		}

		string Namespace
		{
			get;
		}

		string DotNetName
		{
			get;
		}

		int TypeParameterCount
		{
			get;
		}

		bool IsDefaultReturnType
		{
			get;
		}

		bool IsArrayReturnType
		{
			get;
		}

		bool IsGenericReturnType
		{
			get;
		}

		bool IsConstructedReturnType
		{
			get;
		}

		IClass GetUnderlyingClass();

		List<IMethod> GetMethods();

		List<IProperty> GetProperties();

		List<IField> GetFields();

		List<IEvent> GetEvents();

		ArrayReturnType CastToArrayReturnType();

		GenericReturnType CastToGenericReturnType();

		ConstructedReturnType CastToConstructedReturnType();
	}
}

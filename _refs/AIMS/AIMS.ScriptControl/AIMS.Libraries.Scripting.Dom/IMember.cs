using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IMember : IDecoration, IComparable, ICloneable
	{
		string FullyQualifiedName
		{
			get;
		}

		IReturnType DeclaringTypeReference
		{
			get;
			set;
		}

		DomRegion Region
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

		IReturnType ReturnType
		{
			get;
			set;
		}

		DomRegion BodyRegion
		{
			get;
		}

		IList<ExplicitInterfaceImplementation> InterfaceImplementations
		{
			get;
		}
	}
}

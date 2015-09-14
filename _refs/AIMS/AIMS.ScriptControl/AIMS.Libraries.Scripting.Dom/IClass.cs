using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IClass : IDecoration, IComparable
	{
		string FullyQualifiedName
		{
			get;
			set;
		}

		IReturnType DefaultReturnType
		{
			get;
		}

		string DotNetName
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

		ClassType ClassType
		{
			get;
		}

		IProjectContent ProjectContent
		{
			get;
		}

		ICompilationUnit CompilationUnit
		{
			get;
		}

		DomRegion Region
		{
			get;
		}

		DomRegion BodyRegion
		{
			get;
		}

		List<IReturnType> BaseTypes
		{
			get;
		}

		List<IClass> InnerClasses
		{
			get;
		}

		List<IField> Fields
		{
			get;
		}

		List<IProperty> Properties
		{
			get;
		}

		List<IMethod> Methods
		{
			get;
		}

		List<IEvent> Events
		{
			get;
		}

		IList<ITypeParameter> TypeParameters
		{
			get;
		}

		IEnumerable<IClass> ClassInheritanceTree
		{
			get;
		}

		IClass BaseClass
		{
			get;
		}

		IReturnType BaseType
		{
			get;
		}

		bool HasPublicOrInternalStaticMembers
		{
			get;
		}

		bool HasExtensionMethods
		{
			get;
		}

		bool IsPartial
		{
			get;
			set;
		}

		IReturnType GetBaseType(int index);

		IClass GetCompoundClass();

		IClass GetInnermostClass(int caretLine, int caretColumn);

		List<IClass> GetAccessibleTypes(IClass callingClass);

		IMember SearchMember(string memberName, LanguageProperties language);

		bool IsTypeInInheritanceTree(IClass possibleBaseClass);
	}
}

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IDecoration : IComparable
	{
		IClass DeclaringType
		{
			get;
		}

		ModifierEnum Modifiers
		{
			get;
			set;
		}

		IList<IAttribute> Attributes
		{
			get;
		}

		string Documentation
		{
			get;
		}

		bool IsAbstract
		{
			get;
		}

		bool IsSealed
		{
			get;
		}

		bool IsStatic
		{
			get;
		}

		bool IsConst
		{
			get;
		}

		bool IsVirtual
		{
			get;
		}

		bool IsPublic
		{
			get;
		}

		bool IsProtected
		{
			get;
		}

		bool IsPrivate
		{
			get;
		}

		bool IsInternal
		{
			get;
		}

		bool IsReadonly
		{
			get;
		}

		bool IsProtectedAndInternal
		{
			get;
		}

		bool IsProtectedOrInternal
		{
			get;
		}

		bool IsOverride
		{
			get;
		}

		bool IsOverridable
		{
			get;
		}

		bool IsNew
		{
			get;
		}

		bool IsSynthetic
		{
			get;
		}

		object UserData
		{
			get;
			set;
		}

		bool IsAccessible(IClass callingClass, bool isClassInInheritanceTree);

		bool MustBeShown(IClass callingClass, bool showStatic, bool isClassInInheritanceTree);
	}
}

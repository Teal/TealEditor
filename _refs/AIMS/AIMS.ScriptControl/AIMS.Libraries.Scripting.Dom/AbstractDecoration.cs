using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public abstract class AbstractDecoration : IDecoration, IComparable
	{
		private ModifierEnum modifiers = ModifierEnum.None;

		private IList<IAttribute> attributes = null;

		private IClass declaringType;

		private object userData = null;

		private string documentation;

		public IClass DeclaringType
		{
			get
			{
				return this.declaringType;
			}
		}

		public object UserData
		{
			get
			{
				return this.userData;
			}
			set
			{
				this.userData = value;
			}
		}

		public ModifierEnum Modifiers
		{
			get
			{
				return this.modifiers;
			}
			set
			{
				this.modifiers = value;
			}
		}

		public IList<IAttribute> Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new List<IAttribute>();
				}
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		public string Documentation
		{
			get
			{
				string xmlDocumentation;
				if (this.documentation == null)
				{
					string documentationTag = this.DocumentationTag;
					if (documentationTag != null)
					{
						IProjectContent pc = null;
						if (this is IClass)
						{
							pc = ((IClass)this).ProjectContent;
						}
						else if (this.declaringType != null)
						{
							pc = this.declaringType.ProjectContent;
						}
						if (pc != null)
						{
							xmlDocumentation = pc.GetXmlDocumentation(documentationTag);
							return xmlDocumentation;
						}
					}
				}
				xmlDocumentation = this.documentation;
				return xmlDocumentation;
			}
			set
			{
				this.documentation = value;
			}
		}

		public abstract string DocumentationTag
		{
			get;
		}

		public bool IsAbstract
		{
			get
			{
				return (this.modifiers & ModifierEnum.Dim) == ModifierEnum.Dim;
			}
		}

		public bool IsSealed
		{
			get
			{
				return (this.modifiers & ModifierEnum.Sealed) == ModifierEnum.Sealed;
			}
		}

		public bool IsStatic
		{
			get
			{
				return (this.modifiers & ModifierEnum.Static) == ModifierEnum.Static || this.IsConst;
			}
		}

		public bool IsConst
		{
			get
			{
				return (this.modifiers & ModifierEnum.Const) == ModifierEnum.Const;
			}
		}

		public bool IsVirtual
		{
			get
			{
				return (this.modifiers & ModifierEnum.Virtual) == ModifierEnum.Virtual;
			}
		}

		public bool IsPublic
		{
			get
			{
				return (this.modifiers & ModifierEnum.Public) == ModifierEnum.Public;
			}
		}

		public bool IsProtected
		{
			get
			{
				return (this.modifiers & ModifierEnum.Protected) == ModifierEnum.Protected;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return (this.modifiers & ModifierEnum.Private) == ModifierEnum.Private;
			}
		}

		public bool IsInternal
		{
			get
			{
				return (this.modifiers & ModifierEnum.Internal) == ModifierEnum.Internal;
			}
		}

		public bool IsProtectedAndInternal
		{
			get
			{
				return (this.modifiers & ModifierEnum.ProtectedAndInternal) == ModifierEnum.ProtectedAndInternal;
			}
		}

		public bool IsProtectedOrInternal
		{
			get
			{
				return this.IsProtected || this.IsInternal;
			}
		}

		public bool IsReadonly
		{
			get
			{
				return (this.modifiers & ModifierEnum.Readonly) == ModifierEnum.Readonly;
			}
		}

		public bool IsOverride
		{
			get
			{
				return (this.modifiers & ModifierEnum.Override) == ModifierEnum.Override;
			}
		}

		public bool IsOverridable
		{
			get
			{
				return (this.IsOverride || this.IsVirtual || this.IsAbstract) && !this.IsSealed;
			}
		}

		public bool IsNew
		{
			get
			{
				return (this.modifiers & ModifierEnum.New) == ModifierEnum.New;
			}
		}

		public bool IsSynthetic
		{
			get
			{
				return (this.modifiers & ModifierEnum.Synthetic) == ModifierEnum.Synthetic;
			}
		}

		public AbstractDecoration(IClass declaringType)
		{
			this.declaringType = declaringType;
		}

		private bool IsInnerClass(IClass c, IClass possibleInnerClass)
		{
			bool result;
			foreach (IClass inner in c.InnerClasses)
			{
				if (inner.FullyQualifiedName == possibleInnerClass.FullyQualifiedName)
				{
					result = true;
					return result;
				}
				if (this.IsInnerClass(inner, possibleInnerClass))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public bool IsAccessible(IClass callingClass, bool isClassInInheritanceTree)
		{
			return this.IsInternal || this.IsPublic || (isClassInInheritanceTree && this.IsProtected) || (callingClass != null && (this.DeclaringType.FullyQualifiedName == callingClass.FullyQualifiedName || this.IsInnerClass(this.DeclaringType, callingClass)));
		}

		public bool MustBeShown(IClass callingClass, bool showStatic, bool isClassInInheritanceTree)
		{
			return this.DeclaringType.ClassType == ClassType.Enum || ((showStatic || !this.IsStatic) && (!showStatic || this.IsStatic || this.IsConst) && this.IsAccessible(callingClass, isClassInInheritanceTree));
		}

		public virtual int CompareTo(IDecoration value)
		{
			return this.Modifiers - value.Modifiers;
		}

		int IComparable.CompareTo(object value)
		{
			return this.CompareTo((IDecoration)value);
		}
	}
}

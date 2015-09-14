using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultClass : AbstractNamedEntity, IClass, IDecoration, IComparable
	{
		private const byte hasPublicOrInternalStaticMembersFlag = 2;

		private const byte hasExtensionMethodsFlag = 4;

		private ClassType classType;

		private DomRegion region;

		private DomRegion bodyRegion;

		private ICompilationUnit compilationUnit;

		private List<IReturnType> baseTypes = null;

		private List<IClass> innerClasses = null;

		private List<IField> fields = null;

		private List<IProperty> properties = null;

		private List<IMethod> methods = null;

		private List<IEvent> events = null;

		private IList<ITypeParameter> typeParameters = null;

		private byte flags;

		private IReturnType defaultReturnType;

		private List<IClass> inheritanceTreeCache;

		protected bool UseInheritanceCache = false;

		private IReturnType cachedBaseType;

		internal byte Flags
		{
			get
			{
				if (this.flags == 0)
				{
					this.flags = 1;
					foreach (IMember i in this.Fields)
					{
						if (i.IsStatic && (i.IsPublic || i.IsInternal))
						{
							this.flags |= 2;
						}
					}
					foreach (IProperty j in this.Properties)
					{
						if (j.IsStatic && (j.IsPublic || j.IsInternal))
						{
							this.flags |= 2;
						}
						if (j.IsExtensionMethod)
						{
							this.flags |= 4;
						}
					}
					foreach (IMethod k in this.Methods)
					{
						if (k.IsStatic && (k.IsPublic || k.IsInternal))
						{
							this.flags |= 2;
						}
						if (k.IsExtensionMethod)
						{
							this.flags |= 4;
						}
					}
					foreach (IMember i in this.Events)
					{
						if (i.IsStatic && (i.IsPublic || i.IsInternal))
						{
							this.flags |= 2;
						}
					}
					foreach (IClass c in this.InnerClasses)
					{
						if (c.IsPublic || c.IsInternal)
						{
							this.flags |= 2;
						}
					}
				}
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public bool HasPublicOrInternalStaticMembers
		{
			get
			{
				return (this.Flags & 2) == 2;
			}
		}

		public bool HasExtensionMethods
		{
			get
			{
				return (this.Flags & 4) == 4;
			}
		}

		public IReturnType DefaultReturnType
		{
			get
			{
				if (this.defaultReturnType == null)
				{
					this.defaultReturnType = this.CreateDefaultReturnType();
				}
				return this.defaultReturnType;
			}
		}

		public bool IsPartial
		{
			get
			{
				return (base.Modifiers & ModifierEnum.Partial) == ModifierEnum.Partial;
			}
			set
			{
				if (value)
				{
					base.Modifiers |= ModifierEnum.Partial;
				}
				else
				{
					base.Modifiers &= ~ModifierEnum.Partial;
				}
				this.defaultReturnType = null;
			}
		}

		public ICompilationUnit CompilationUnit
		{
			[DebuggerStepThrough]
			get
			{
				return this.compilationUnit;
			}
		}

		public IProjectContent ProjectContent
		{
			[DebuggerStepThrough]
			get
			{
				return this.CompilationUnit.ProjectContent;
			}
		}

		public ClassType ClassType
		{
			get
			{
				return this.classType;
			}
			set
			{
				this.classType = value;
			}
		}

		public DomRegion Region
		{
			get
			{
				return this.region;
			}
			set
			{
				this.region = value;
			}
		}

		public DomRegion BodyRegion
		{
			get
			{
				return this.bodyRegion;
			}
			set
			{
				this.bodyRegion = value;
			}
		}

		public override string DotNetName
		{
			get
			{
				string result;
				if (this.typeParameters == null || this.typeParameters.Count == 0)
				{
					result = base.FullyQualifiedName;
				}
				else
				{
					result = base.FullyQualifiedName + "`" + this.typeParameters.Count;
				}
				return result;
			}
		}

		public override string DocumentationTag
		{
			get
			{
				return "T:" + this.DotNetName;
			}
		}

		public List<IReturnType> BaseTypes
		{
			get
			{
				if (this.baseTypes == null)
				{
					this.baseTypes = new List<IReturnType>();
				}
				return this.baseTypes;
			}
		}

		public virtual List<IClass> InnerClasses
		{
			get
			{
				if (this.innerClasses == null)
				{
					this.innerClasses = new List<IClass>();
				}
				return this.innerClasses;
			}
		}

		public virtual List<IField> Fields
		{
			get
			{
				if (this.fields == null)
				{
					this.fields = new List<IField>();
				}
				return this.fields;
			}
		}

		public virtual List<IProperty> Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new List<IProperty>();
				}
				return this.properties;
			}
		}

		public virtual List<IMethod> Methods
		{
			get
			{
				if (this.methods == null)
				{
					this.methods = new List<IMethod>();
				}
				return this.methods;
			}
		}

		public virtual List<IEvent> Events
		{
			get
			{
				if (this.events == null)
				{
					this.events = new List<IEvent>();
				}
				return this.events;
			}
		}

		public virtual IList<ITypeParameter> TypeParameters
		{
			get
			{
				if (this.typeParameters == null)
				{
					this.typeParameters = new List<ITypeParameter>();
				}
				return this.typeParameters;
			}
			set
			{
				this.typeParameters = value;
			}
		}

		public IEnumerable<IClass> ClassInheritanceTree
		{
			get
			{
				IEnumerable<IClass> result;
				if (this.inheritanceTreeCache != null)
				{
					result = this.inheritanceTreeCache;
				}
				else
				{
					List<IClass> visitedList = new List<IClass>();
					Queue<IReturnType> typesToVisit = new Queue<IReturnType>();
					bool enqueuedLastBaseType = false;
					IClass currentClass = this;
					IReturnType nextType;
					do
					{
						if (currentClass != null)
						{
							if (!visitedList.Contains(currentClass))
							{
								visitedList.Add(currentClass);
								foreach (IReturnType type in currentClass.BaseTypes)
								{
									typesToVisit.Enqueue(type);
								}
							}
						}
						if (typesToVisit.Count > 0)
						{
							nextType = typesToVisit.Dequeue();
						}
						else
						{
							nextType = (enqueuedLastBaseType ? null : this.GetBaseTypeByClassType());
							enqueuedLastBaseType = true;
						}
						if (nextType != null)
						{
							currentClass = nextType.GetUnderlyingClass();
						}
					}
					while (nextType != null);
					if (this.UseInheritanceCache)
					{
						this.inheritanceTreeCache = visitedList;
					}
					result = visitedList;
				}
				return result;
			}
		}

		protected override bool CanBeSubclass
		{
			get
			{
				return true;
			}
		}

		public IReturnType BaseType
		{
			get
			{
				if (this.cachedBaseType == null)
				{
					foreach (IReturnType baseType in this.BaseTypes)
					{
						IClass baseClass = baseType.GetUnderlyingClass();
						if (baseClass != null && baseClass.ClassType == this.ClassType)
						{
							this.cachedBaseType = baseType;
							break;
						}
					}
				}
				IReturnType baseTypeByClassType;
				if (this.cachedBaseType == null)
				{
					baseTypeByClassType = this.GetBaseTypeByClassType();
				}
				else
				{
					baseTypeByClassType = this.cachedBaseType;
				}
				return baseTypeByClassType;
			}
		}

		public IClass BaseClass
		{
			get
			{
				IClass result;
				foreach (IReturnType baseType in this.BaseTypes)
				{
					IClass baseClass = baseType.GetUnderlyingClass();
					if (baseClass != null && baseClass.ClassType == this.ClassType)
					{
						result = baseClass;
						return result;
					}
				}
				switch (this.ClassType)
				{
				case ClassType.Class:
					if (base.FullyQualifiedName != "System.Object")
					{
						result = this.ProjectContent.SystemTypes.Object.GetUnderlyingClass();
						return result;
					}
					break;
				case ClassType.Enum:
					result = this.ProjectContent.SystemTypes.Enum.GetUnderlyingClass();
					return result;
				case ClassType.Struct:
					result = this.ProjectContent.SystemTypes.ValueType.GetUnderlyingClass();
					return result;
				case ClassType.Delegate:
					result = this.ProjectContent.SystemTypes.Delegate.GetUnderlyingClass();
					return result;
				}
				result = null;
				return result;
			}
		}

		public DefaultClass(ICompilationUnit compilationUnit, string fullyQualifiedName) : base(null)
		{
			this.compilationUnit = compilationUnit;
			base.FullyQualifiedName = fullyQualifiedName;
		}

		public DefaultClass(ICompilationUnit compilationUnit, IClass declaringType) : base(declaringType)
		{
			this.compilationUnit = compilationUnit;
		}

		public DefaultClass(ICompilationUnit compilationUnit, ClassType classType, ModifierEnum modifiers, DomRegion region, IClass declaringType) : base(declaringType)
		{
			this.compilationUnit = compilationUnit;
			this.region = region;
			this.classType = classType;
			base.Modifiers = modifiers;
		}

		protected virtual IReturnType CreateDefaultReturnType()
		{
			IReturnType result;
			if (this.IsPartial)
			{
				result = new GetClassReturnType(this.ProjectContent, base.FullyQualifiedName, this.TypeParameters.Count);
			}
			else
			{
				result = new DefaultReturnType(this);
			}
			return result;
		}

		public IClass GetCompoundClass()
		{
			return this.DefaultReturnType.GetUnderlyingClass() ?? this;
		}

		protected override void OnFullyQualifiedNameChanged(EventArgs e)
		{
			base.OnFullyQualifiedNameChanged(e);
			GetClassReturnType rt = this.defaultReturnType as GetClassReturnType;
			if (rt != null)
			{
				rt.SetFullyQualifiedName(base.FullyQualifiedName);
			}
		}

		public virtual int CompareTo(IClass value)
		{
			int cmp;
			int result;
			if (0 != (cmp = base.CompareTo(value)))
			{
				result = cmp;
			}
			else if (base.FullyQualifiedName != null)
			{
				cmp = base.FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0)
				{
					result = cmp;
				}
				else
				{
					result = this.TypeParameters.Count - value.TypeParameters.Count;
				}
			}
			else
			{
				result = -1;
			}
			return result;
		}

		int IComparable.CompareTo(object o)
		{
			return this.CompareTo((IClass)o);
		}

		public IReturnType GetBaseType(int index)
		{
			return this.BaseTypes[index];
		}

		private IReturnType GetBaseTypeByClassType()
		{
			IReturnType result;
			switch (this.ClassType)
			{
			case ClassType.Class:
			case ClassType.Interface:
				if (base.FullyQualifiedName != "System.Object")
				{
					result = this.ProjectContent.SystemTypes.Object;
					return result;
				}
				break;
			case ClassType.Enum:
				result = this.ProjectContent.SystemTypes.Enum;
				return result;
			case ClassType.Struct:
				result = this.ProjectContent.SystemTypes.ValueType;
				return result;
			case ClassType.Delegate:
				result = this.ProjectContent.SystemTypes.Delegate;
				return result;
			}
			result = null;
			return result;
		}

		public bool IsTypeInInheritanceTree(IClass possibleBaseClass)
		{
			bool result;
			if (possibleBaseClass == null)
			{
				result = false;
			}
			else
			{
				foreach (IClass baseClass in this.ClassInheritanceTree)
				{
					if (possibleBaseClass.FullyQualifiedName == baseClass.FullyQualifiedName)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public IMember SearchMember(string memberName, LanguageProperties language)
		{
			IMember result;
			if (memberName == null || memberName.Length == 0)
			{
				result = null;
			}
			else
			{
				StringComparer cmp = language.NameComparer;
				foreach (IProperty p in this.Properties)
				{
					if (cmp.Equals(p.Name, memberName))
					{
						result = p;
						return result;
					}
				}
				foreach (IEvent e in this.Events)
				{
					if (cmp.Equals(e.Name, memberName))
					{
						result = e;
						return result;
					}
				}
				foreach (IField f in this.Fields)
				{
					if (cmp.Equals(f.Name, memberName))
					{
						result = f;
						return result;
					}
				}
				foreach (IMethod i in this.Methods)
				{
					if (cmp.Equals(i.Name, memberName))
					{
						result = i;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			IClass result;
			foreach (IClass c in this.InnerClasses)
			{
				if (c != null && c.Region.IsInside(caretLine, caretColumn))
				{
					result = c.GetInnermostClass(caretLine, caretColumn);
					return result;
				}
			}
			result = this;
			return result;
		}

		public List<IClass> GetAccessibleTypes(IClass callingClass)
		{
			List<IClass> types = new List<IClass>();
			List<IClass> visitedTypes = new List<IClass>();
			IClass currentClass = this;
			while (!visitedTypes.Contains(currentClass))
			{
				visitedTypes.Add(currentClass);
				bool isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(currentClass);
				foreach (IClass c in currentClass.InnerClasses)
				{
					if (c.IsAccessible(callingClass, isClassInInheritanceTree))
					{
						types.Add(c);
					}
				}
				currentClass = currentClass.BaseClass;
				if (currentClass == null)
				{
					return types;
				}
			}
			return types;
		}
	}
}

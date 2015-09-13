using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultTypeParameter : ITypeParameter
	{
		public static readonly IList<ITypeParameter> EmptyTypeParameterList = new List<ITypeParameter>().AsReadOnly();

		private string name;

		private IMethod method;

		private IClass targetClass;

		private int index;

		private List<IReturnType> constraints = new List<IReturnType>();

		private bool hasConstructableConstraint = false;

		private bool hasReferenceTypeConstraint = false;

		private bool hasValueTypeConstraint = false;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public IMethod Method
		{
			get
			{
				return this.method;
			}
		}

		public IClass Class
		{
			get
			{
				return this.targetClass;
			}
		}

		public IList<IReturnType> Constraints
		{
			get
			{
				return this.constraints;
			}
		}

		public IList<IAttribute> Attributes
		{
			get
			{
				return DefaultAttribute.EmptyAttributeList;
			}
		}

		public bool HasConstructableConstraint
		{
			get
			{
				return this.hasConstructableConstraint;
			}
			set
			{
				this.hasConstructableConstraint = value;
			}
		}

		public bool HasReferenceTypeConstraint
		{
			get
			{
				return this.hasReferenceTypeConstraint;
			}
			set
			{
				this.hasReferenceTypeConstraint = value;
			}
		}

		public bool HasValueTypeConstraint
		{
			get
			{
				return this.hasValueTypeConstraint;
			}
			set
			{
				this.hasValueTypeConstraint = value;
			}
		}

		public DefaultTypeParameter(IMethod method, string name, int index)
		{
			this.method = method;
			this.targetClass = method.DeclaringType;
			this.name = name;
			this.index = index;
		}

		public DefaultTypeParameter(IMethod method, Type type)
		{
			this.method = method;
			this.targetClass = method.DeclaringType;
			this.name = type.Name;
			this.index = type.GenericParameterPosition;
		}

		public DefaultTypeParameter(IClass targetClass, string name, int index)
		{
			this.targetClass = targetClass;
			this.name = name;
			this.index = index;
		}

		public DefaultTypeParameter(IClass targetClass, Type type)
		{
			this.targetClass = targetClass;
			this.name = type.Name;
			this.index = type.GenericParameterPosition;
		}

		public override bool Equals(object obj)
		{
			DefaultTypeParameter tp = obj as DefaultTypeParameter;
			bool result;
			if (tp == null)
			{
				result = false;
			}
			else if (tp.index != this.index)
			{
				result = false;
			}
			else if (tp.name != this.name)
			{
				result = false;
			}
			else if (tp.hasConstructableConstraint != this.hasConstructableConstraint)
			{
				result = false;
			}
			else if (tp.hasReferenceTypeConstraint != this.hasReferenceTypeConstraint)
			{
				result = false;
			}
			else if (tp.hasValueTypeConstraint != this.hasValueTypeConstraint)
			{
				result = false;
			}
			else
			{
				if (tp.method != this.method)
				{
					if (tp.method == null || this.method == null)
					{
						result = false;
						return result;
					}
					if (tp.method.FullyQualifiedName == this.method.FullyQualifiedName)
					{
						result = false;
						return result;
					}
				}
				else if (tp.targetClass.FullyQualifiedName == this.targetClass.FullyQualifiedName)
				{
					result = false;
					return result;
				}
				result = true;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}]", base.GetType().Name, this.name);
		}

		public static DefaultClass GetDummyClassForTypeParameter(ITypeParameter p)
		{
			DefaultClass c = new DefaultClass(p.Class.CompilationUnit, p.Name);
			if (p.Method != null)
			{
				c.Region = new DomRegion(p.Method.Region.BeginLine, p.Method.Region.BeginColumn);
			}
			else
			{
				c.Region = new DomRegion(p.Class.Region.BeginLine, p.Class.Region.BeginColumn);
			}
			c.Modifiers = ModifierEnum.Public;
			if (p.HasValueTypeConstraint)
			{
				c.ClassType = ClassType.Struct;
			}
			else if (p.HasConstructableConstraint)
			{
				c.ClassType = ClassType.Class;
			}
			else
			{
				c.ClassType = ClassType.Interface;
			}
			return c;
		}
	}
}

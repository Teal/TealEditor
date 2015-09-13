using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class GenericReturnType : ProxyReturnType
	{
		private ITypeParameter typeParameter;

		[CompilerGenerated]
		private static Predicate<IMethod> <>9__CachedAnonymousMethodDelegate1;

		public ITypeParameter TypeParameter
		{
			get
			{
				return this.typeParameter;
			}
		}

		public override string FullyQualifiedName
		{
			get
			{
				return this.typeParameter.Name;
			}
		}

		public override string Name
		{
			get
			{
				return this.typeParameter.Name;
			}
		}

		public override string Namespace
		{
			get
			{
				return "";
			}
		}

		public override string DotNetName
		{
			get
			{
				string result;
				if (this.typeParameter.Method != null)
				{
					result = "``" + this.typeParameter.Index;
				}
				else
				{
					result = "`" + this.typeParameter.Index;
				}
				return result;
			}
		}

		public override IReturnType BaseType
		{
			get
			{
				int count = this.typeParameter.Constraints.Count;
				IReturnType result;
				if (count == 0)
				{
					result = this.typeParameter.Class.ProjectContent.SystemTypes.Object;
				}
				else if (count == 1)
				{
					result = this.typeParameter.Constraints[0];
				}
				else
				{
					result = new CombinedReturnType(this.typeParameter.Constraints, this.FullyQualifiedName, this.Name, this.Namespace, this.DotNetName);
				}
				return result;
			}
		}

		public override bool IsDefaultReturnType
		{
			get
			{
				return false;
			}
		}

		public override bool IsArrayReturnType
		{
			get
			{
				return false;
			}
		}

		public override bool IsConstructedReturnType
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericReturnType
		{
			get
			{
				return true;
			}
		}

		public override bool Equals(object o)
		{
			IReturnType rt = o as IReturnType;
			return rt != null && rt.IsGenericReturnType && this.typeParameter.Equals(rt.CastToGenericReturnType().typeParameter);
		}

		public override int GetHashCode()
		{
			return this.typeParameter.GetHashCode();
		}

		public GenericReturnType(ITypeParameter typeParameter)
		{
			if (typeParameter == null)
			{
				throw new ArgumentNullException("typeParameter");
			}
			this.typeParameter = typeParameter;
		}

		public override IClass GetUnderlyingClass()
		{
			return null;
		}

		public override List<IMethod> GetMethods()
		{
			List<IMethod> list = base.GetMethods();
			if (list != null)
			{
				list.RemoveAll((IMethod m) => m.IsStatic || m.IsConstructor);
				if (this.typeParameter.HasConstructableConstraint || this.typeParameter.HasValueTypeConstraint)
				{
					list.Add(new Constructor(ModifierEnum.Public, this, DefaultTypeParameter.GetDummyClassForTypeParameter(this.typeParameter)));
				}
			}
			return list;
		}

		public override string ToString()
		{
			return string.Format("[GenericReturnType: {0}]", this.typeParameter);
		}

		public override GenericReturnType CastToGenericReturnType()
		{
			return this;
		}
	}
}

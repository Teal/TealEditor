using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom
{
	[Serializable]
	public class DefaultMethod : AbstractMember, IMethod, IMethodOrProperty, IMember, IDecoration, IComparable, ICloneable
	{
		private IList<IParameter> parameters = null;

		private IList<ITypeParameter> typeParameters = null;

		private bool isExtensionMethod;

		private string documentationTag;

		public bool IsExtensionMethod
		{
			get
			{
				return this.isExtensionMethod;
			}
			set
			{
				this.isExtensionMethod = value;
			}
		}

		public override string DotNetName
		{
			get
			{
				string result;
				if (this.typeParameters == null || this.typeParameters.Count == 0)
				{
					result = base.DotNetName;
				}
				else
				{
					result = base.DotNetName + "``" + this.typeParameters.Count;
				}
				return result;
			}
		}

		public override string DocumentationTag
		{
			get
			{
				if (this.documentationTag == null)
				{
					string dotnetName = this.DotNetName;
					StringBuilder b = new StringBuilder("M:", dotnetName.Length + 2);
					b.Append(dotnetName);
					IList<IParameter> paras = this.Parameters;
					if (paras.Count > 0)
					{
						b.Append('(');
						for (int i = 0; i < paras.Count; i++)
						{
							if (i > 0)
							{
								b.Append(',');
							}
							IReturnType rt = paras[i].ReturnType;
							if (rt != null)
							{
								b.Append(rt.DotNetName);
							}
						}
						b.Append(')');
					}
					this.documentationTag = b.ToString();
				}
				return this.documentationTag;
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

		public virtual IList<IParameter> Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new List<IParameter>();
				}
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		public virtual bool IsConstructor
		{
			get
			{
				return this.ReturnType == null || base.Name == "#ctor";
			}
		}

		public override IMember Clone()
		{
			DefaultMethod p = new DefaultMethod(base.Name, this.ReturnType, base.Modifiers, this.Region, this.BodyRegion, base.DeclaringType);
			p.parameters = DefaultParameter.Clone(this.Parameters);
			p.typeParameters = this.typeParameters;
			p.documentationTag = this.DocumentationTag;
			p.isExtensionMethod = this.isExtensionMethod;
			foreach (ExplicitInterfaceImplementation eii in base.InterfaceImplementations)
			{
				p.InterfaceImplementations.Add(eii.Clone());
			}
			return p;
		}

		public DefaultMethod(IClass declaringType, string name) : base(declaringType, name)
		{
		}

		public DefaultMethod(string name, IReturnType type, ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region = region;
			this.BodyRegion = bodyRegion;
			base.Modifiers = m;
		}

		public override string ToString()
		{
			return string.Format("[AbstractMethod: FullyQualifiedName={0}, ReturnType = {1}, IsConstructor={2}, Modifier={3}]", new object[]
			{
				base.FullyQualifiedName,
				this.ReturnType,
				this.IsConstructor,
				base.Modifiers
			});
		}

		public virtual int CompareTo(IMethod value)
		{
			int cmp;
			int result;
			if (base.FullyQualifiedName != null)
			{
				cmp = base.FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0)
				{
					result = cmp;
					return result;
				}
			}
			cmp = this.TypeParameters.Count - value.TypeParameters.Count;
			if (cmp != 0)
			{
				result = cmp;
			}
			else
			{
				result = DiffUtility.Compare<IParameter>(this.Parameters, value.Parameters);
			}
			return result;
		}

		int IComparable.CompareTo(object value)
		{
			int result;
			if (value == null)
			{
				result = 0;
			}
			else
			{
				result = this.CompareTo((IMethod)value);
			}
			return result;
		}
	}
}

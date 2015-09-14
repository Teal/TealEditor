using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class ConstructedReturnType : ProxyReturnType
	{
		private IList<IReturnType> typeArguments;

		private IReturnType baseType;

		public IList<IReturnType> TypeArguments
		{
			get
			{
				return this.typeArguments;
			}
		}

		public override IReturnType BaseType
		{
			get
			{
				return this.baseType;
			}
		}

		public IReturnType UnboundType
		{
			get
			{
				return this.baseType;
			}
		}

		public override string DotNetName
		{
			get
			{
				string baseName = this.baseType.DotNetName;
				int pos = baseName.LastIndexOf('`');
				StringBuilder b;
				if (pos < 0)
				{
					b = new StringBuilder(baseName);
				}
				else
				{
					b = new StringBuilder(baseName, 0, pos, pos + 20);
				}
				b.Append('{');
				for (int i = 0; i < this.typeArguments.Count; i++)
				{
					if (i > 0)
					{
						b.Append(',');
					}
					if (this.typeArguments[i] != null)
					{
						b.Append(this.typeArguments[i].DotNetName);
					}
				}
				b.Append('}');
				return b.ToString();
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
				return true;
			}
		}

		public override bool IsGenericReturnType
		{
			get
			{
				return false;
			}
		}

		public ConstructedReturnType(IReturnType baseType, IList<IReturnType> typeArguments)
		{
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			if (typeArguments == null)
			{
				throw new ArgumentNullException("typeArguments");
			}
			this.typeArguments = typeArguments;
			this.baseType = baseType;
		}

		public override bool Equals(object o)
		{
			IReturnType rt = o as IReturnType;
			return rt != null && this.DotNetName == rt.DotNetName;
		}

		public override int GetHashCode()
		{
			int code = this.baseType.GetHashCode();
			foreach (IReturnType t in this.typeArguments)
			{
				if (t != null)
				{
					code ^= t.GetHashCode();
				}
			}
			return code;
		}

		private bool CheckReturnType(IReturnType t)
		{
			bool result;
			if (t == null)
			{
				result = false;
			}
			else if (t.IsGenericReturnType)
			{
				result = (t.CastToGenericReturnType().TypeParameter.Method == null);
			}
			else if (t.IsArrayReturnType)
			{
				result = this.CheckReturnType(t.CastToArrayReturnType().ArrayElementType);
			}
			else if (t.IsConstructedReturnType)
			{
				foreach (IReturnType para in t.CastToConstructedReturnType().TypeArguments)
				{
					if (this.CheckReturnType(para))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool CheckParameters(IList<IParameter> l)
		{
			bool result;
			foreach (IParameter p in l)
			{
				if (this.CheckReturnType(p.ReturnType))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public static IReturnType TranslateType(IReturnType input, IList<IReturnType> typeParameters, bool convertForMethod)
		{
			IReturnType result;
			if (typeParameters == null || typeParameters.Count == 0)
			{
				result = input;
			}
			else
			{
				if (input.IsGenericReturnType)
				{
					GenericReturnType rt = input.CastToGenericReturnType();
					if (convertForMethod ? (rt.TypeParameter.Method != null) : (rt.TypeParameter.Method == null))
					{
						if (rt.TypeParameter.Index < typeParameters.Count)
						{
							IReturnType newType = typeParameters[rt.TypeParameter.Index];
							if (newType != null)
							{
								result = newType;
								return result;
							}
						}
					}
				}
				else if (input.IsArrayReturnType)
				{
					ArrayReturnType arInput = input.CastToArrayReturnType();
					IReturnType e = arInput.ArrayElementType;
					IReturnType t = ConstructedReturnType.TranslateType(e, typeParameters, convertForMethod);
					if (e != t && t != null)
					{
						result = new ArrayReturnType(arInput.ProjectContent, t, arInput.ArrayDimensions);
						return result;
					}
				}
				else if (input.IsConstructedReturnType)
				{
					ConstructedReturnType cinput = input.CastToConstructedReturnType();
					List<IReturnType> para = new List<IReturnType>(cinput.TypeArguments.Count);
					foreach (IReturnType argument in cinput.TypeArguments)
					{
						para.Add(ConstructedReturnType.TranslateType(argument, typeParameters, convertForMethod));
					}
					result = new ConstructedReturnType(cinput.UnboundType, para);
					return result;
				}
				result = input;
			}
			return result;
		}

		private IReturnType TranslateType(IReturnType input)
		{
			return ConstructedReturnType.TranslateType(input, this.typeArguments, false);
		}

		public override List<IMethod> GetMethods()
		{
			List<IMethod> i = this.baseType.GetMethods();
			for (int j = 0; j < i.Count; j++)
			{
				if (this.CheckReturnType(i[j].ReturnType) || this.CheckParameters(i[j].Parameters))
				{
					i[j] = (IMethod)i[j].Clone();
					if (i[j].DeclaringType == this.baseType.GetUnderlyingClass())
					{
						i[j].DeclaringTypeReference = this;
					}
					i[j].ReturnType = this.TranslateType(i[j].ReturnType);
					for (int k = 0; k < i[j].Parameters.Count; k++)
					{
						i[j].Parameters[k].ReturnType = this.TranslateType(i[j].Parameters[k].ReturnType);
					}
				}
			}
			return i;
		}

		public override List<IProperty> GetProperties()
		{
			List<IProperty> i = this.baseType.GetProperties();
			for (int j = 0; j < i.Count; j++)
			{
				if (this.CheckReturnType(i[j].ReturnType) || this.CheckParameters(i[j].Parameters))
				{
					i[j] = (IProperty)i[j].Clone();
					if (i[j].DeclaringType == this.baseType.GetUnderlyingClass())
					{
						i[j].DeclaringTypeReference = this;
					}
					i[j].ReturnType = this.TranslateType(i[j].ReturnType);
					for (int k = 0; k < i[j].Parameters.Count; k++)
					{
						i[j].Parameters[k].ReturnType = this.TranslateType(i[j].Parameters[k].ReturnType);
					}
				}
			}
			return i;
		}

		public override List<IField> GetFields()
		{
			List<IField> i = this.baseType.GetFields();
			for (int j = 0; j < i.Count; j++)
			{
				if (this.CheckReturnType(i[j].ReturnType))
				{
					i[j] = (IField)i[j].Clone();
					if (i[j].DeclaringType == this.baseType.GetUnderlyingClass())
					{
						i[j].DeclaringTypeReference = this;
					}
					i[j].ReturnType = this.TranslateType(i[j].ReturnType);
				}
			}
			return i;
		}

		public override List<IEvent> GetEvents()
		{
			List<IEvent> i = this.baseType.GetEvents();
			for (int j = 0; j < i.Count; j++)
			{
				if (this.CheckReturnType(i[j].ReturnType))
				{
					i[j] = (IEvent)i[j].Clone();
					if (i[j].DeclaringType == this.baseType.GetUnderlyingClass())
					{
						i[j].DeclaringTypeReference = this;
					}
					i[j].ReturnType = this.TranslateType(i[j].ReturnType);
				}
			}
			return i;
		}

		public override string ToString()
		{
			string r = "[ConstructedReturnType: ";
			r += this.baseType;
			r += "<";
			for (int i = 0; i < this.typeArguments.Count; i++)
			{
				if (i > 0)
				{
					r += ",";
				}
				if (this.typeArguments[i] != null)
				{
					r += this.typeArguments[i];
				}
			}
			return r + ">]";
		}

		public override ConstructedReturnType CastToConstructedReturnType()
		{
			return this;
		}
	}
}

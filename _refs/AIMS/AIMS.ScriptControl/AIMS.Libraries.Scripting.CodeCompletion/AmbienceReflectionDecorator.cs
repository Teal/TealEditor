using AIMS.Libraries.Scripting.Dom;
using System;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
	public class AmbienceReflectionDecorator : IAmbience
	{
		private IAmbience conv;

		public ConversionFlags ConversionFlags
		{
			get
			{
				return this.conv.ConversionFlags;
			}
			set
			{
				this.conv.ConversionFlags = value;
			}
		}

		public string Convert(ModifierEnum modifier)
		{
			return this.conv.Convert(modifier);
		}

		public string Convert(IClass c)
		{
			return this.conv.Convert(c);
		}

		public string ConvertEnd(IClass c)
		{
			return this.conv.ConvertEnd(c);
		}

		public string Convert(IField field)
		{
			return this.conv.Convert(field);
		}

		public string Convert(IProperty property)
		{
			return this.conv.Convert(property);
		}

		public string Convert(IEvent e)
		{
			return this.conv.Convert(e);
		}

		public string Convert(IMethod m)
		{
			return this.conv.Convert(m);
		}

		public string ConvertEnd(IMethod m)
		{
			return this.conv.ConvertEnd(m);
		}

		public string Convert(IParameter param)
		{
			return this.conv.Convert(param);
		}

		public string Convert(IReturnType returnType)
		{
			return this.conv.Convert(returnType);
		}

		public AmbienceReflectionDecorator(IAmbience conv)
		{
			if (conv == null)
			{
				throw new ArgumentNullException("conv");
			}
			this.conv = conv;
		}

		public string WrapAttribute(string attribute)
		{
			return this.conv.WrapAttribute(attribute);
		}

		public string WrapComment(string comment)
		{
			return this.conv.WrapComment(comment);
		}

		public string GetIntrinsicTypeName(string dotNetTypeName)
		{
			return this.conv.GetIntrinsicTypeName(dotNetTypeName);
		}
	}
}

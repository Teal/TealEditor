using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom.CSharp
{
	public class CSharpAmbience : AbstractAmbience
	{
		private static CSharpAmbience instance = new CSharpAmbience();

		public static IDictionary<string, string> TypeConversionTable
		{
			get
			{
				return TypeReference.PrimitiveTypesCSharpReverse;
			}
		}

		public static CSharpAmbience Instance
		{
			get
			{
				return CSharpAmbience.instance;
			}
		}

		private bool ModifierIsSet(ModifierEnum modifier, ModifierEnum query)
		{
			return (modifier & query) == query;
		}

		public override string Convert(ModifierEnum modifier)
		{
			string result;
			if (base.ShowAccessibility)
			{
				if (this.ModifierIsSet(modifier, ModifierEnum.Public))
				{
					result = "public ";
					return result;
				}
				if (this.ModifierIsSet(modifier, ModifierEnum.Private))
				{
					result = "private ";
					return result;
				}
				if (this.ModifierIsSet(modifier, ModifierEnum.ProtectedAndInternal))
				{
					result = "protected internal ";
					return result;
				}
				if (this.ModifierIsSet(modifier, ModifierEnum.Internal))
				{
					result = "internal ";
					return result;
				}
				if (this.ModifierIsSet(modifier, ModifierEnum.Protected))
				{
					result = "protected ";
					return result;
				}
			}
			result = string.Empty;
			return result;
		}

		private string GetModifier(IDecoration decoration)
		{
			string ret = "";
			if (base.IncludeHTMLMarkup)
			{
				ret += "<i>";
			}
			if (decoration.IsStatic)
			{
				ret += "static ";
			}
			else if (decoration.IsSealed)
			{
				ret += "sealed ";
			}
			else if (decoration.IsVirtual)
			{
				ret += "virtual ";
			}
			else if (decoration.IsOverride)
			{
				ret += "override ";
			}
			else if (decoration.IsNew)
			{
				ret += "new ";
			}
			if (base.IncludeHTMLMarkup)
			{
				ret += "</i>";
			}
			return ret;
		}

		public override string Convert(IClass c)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(this.Convert(c.Modifiers));
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<i>");
			}
			if (base.ShowModifiers)
			{
				if (c.IsSealed)
				{
					switch (c.ClassType)
					{
					case AIMS.Libraries.Scripting.Dom.ClassType.Enum:
					case AIMS.Libraries.Scripting.Dom.ClassType.Struct:
					case AIMS.Libraries.Scripting.Dom.ClassType.Delegate:
						goto IL_8A;
					}
					builder.Append("sealed ");
					IL_8A:;
				}
				else if (c.IsAbstract && c.ClassType != AIMS.Libraries.Scripting.Dom.ClassType.Interface)
				{
					builder.Append("abstract ");
				}
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</i>");
			}
			if (base.ShowModifiers)
			{
				switch (c.ClassType)
				{
				case AIMS.Libraries.Scripting.Dom.ClassType.Class:
				case AIMS.Libraries.Scripting.Dom.ClassType.Module:
					builder.Append("class");
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Enum:
					builder.Append("enum");
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Interface:
					builder.Append("interface");
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Struct:
					builder.Append("struct");
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Delegate:
					builder.Append("delegate");
					break;
				}
				builder.Append(' ');
			}
			if (base.ShowReturnType && c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Delegate)
			{
				foreach (IMethod i in c.Methods)
				{
					if (!(i.Name != "Invoke"))
					{
						builder.Append(this.Convert(i.ReturnType));
						builder.Append(' ');
					}
				}
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<b>");
			}
			if (base.UseFullyQualifiedMemberNames)
			{
				builder.Append(c.FullyQualifiedName);
			}
			else
			{
				builder.Append(c.Name);
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</b>");
			}
			if (c.TypeParameters.Count > 0)
			{
				builder.Append('<');
				for (int j = 0; j < c.TypeParameters.Count; j++)
				{
					if (j > 0)
					{
						builder.Append(", ");
					}
					builder.Append(c.TypeParameters[j].Name);
				}
				builder.Append('>');
			}
			if (base.ShowReturnType && c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Delegate)
			{
				builder.Append(" (");
				if (base.IncludeHTMLMarkup)
				{
					builder.Append("<br>");
				}
				foreach (IMethod i in c.Methods)
				{
					if (!(i.Name != "Invoke"))
					{
						for (int j = 0; j < i.Parameters.Count; j++)
						{
							if (base.IncludeHTMLMarkup)
							{
								builder.Append("&nbsp;&nbsp;&nbsp;");
							}
							builder.Append(this.Convert(i.Parameters[j]));
							if (j + 1 < i.Parameters.Count)
							{
								builder.Append(", ");
							}
							if (base.IncludeHTMLMarkup)
							{
								builder.Append("<br>");
							}
						}
					}
				}
				builder.Append(')');
			}
			else if (base.ShowInheritanceList)
			{
				if (c.BaseTypes.Count > 0)
				{
					builder.Append(" : ");
					for (int j = 0; j < c.BaseTypes.Count; j++)
					{
						builder.Append(c.BaseTypes[j]);
						if (j + 1 < c.BaseTypes.Count)
						{
							builder.Append(", ");
						}
					}
				}
			}
			if (base.IncludeBodies)
			{
				builder.Append("\n{");
			}
			return builder.ToString();
		}

		public override string ConvertEnd(IClass c)
		{
			return "}";
		}

		public override string Convert(IField field)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(this.Convert(field.Modifiers));
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<i>");
			}
			if (base.ShowModifiers)
			{
				if (field.IsConst)
				{
					builder.Append("const ");
				}
				else if (field.IsStatic)
				{
					builder.Append("static ");
				}
				if (field.IsReadonly)
				{
					builder.Append("readonly ");
				}
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</i>");
			}
			if (field.ReturnType != null && base.ShowReturnType)
			{
				builder.Append(this.Convert(field.ReturnType));
				builder.Append(' ');
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<b>");
			}
			if (base.UseFullyQualifiedMemberNames)
			{
				builder.Append(field.FullyQualifiedName);
			}
			else
			{
				builder.Append(field.Name);
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</b>");
			}
			if (base.IncludeBodies)
			{
				builder.Append(";");
			}
			return builder.ToString();
		}

		public override string Convert(IProperty property)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(this.Convert(property.Modifiers));
			if (base.ShowModifiers)
			{
				builder.Append(this.GetModifier(property));
			}
			if (property.ReturnType != null && base.ShowReturnType)
			{
				builder.Append(this.Convert(property.ReturnType));
				builder.Append(' ');
			}
			if (property.IsIndexer)
			{
				if (property.DeclaringType != null)
				{
					if (base.UseFullyQualifiedMemberNames)
					{
						builder.Append(property.DeclaringType.FullyQualifiedName);
					}
					else
					{
						builder.Append(property.DeclaringType.Name);
					}
					builder.Append('.');
				}
				builder.Append("this");
			}
			else
			{
				if (base.IncludeHTMLMarkup)
				{
					builder.Append("<b>");
				}
				if (base.UseFullyQualifiedMemberNames)
				{
					builder.Append(property.FullyQualifiedName);
				}
				else
				{
					builder.Append(property.Name);
				}
				if (base.IncludeHTMLMarkup)
				{
					builder.Append("</b>");
				}
			}
			if (property.Parameters.Count > 0)
			{
				builder.Append(property.IsIndexer ? '[' : '(');
				if (base.IncludeHTMLMarkup)
				{
					builder.Append("<br>");
				}
				for (int i = 0; i < property.Parameters.Count; i++)
				{
					if (base.IncludeHTMLMarkup)
					{
						builder.Append("&nbsp;&nbsp;&nbsp;");
					}
					builder.Append(this.Convert(property.Parameters[i]));
					if (i + 1 < property.Parameters.Count)
					{
						builder.Append(", ");
					}
					if (base.IncludeHTMLMarkup)
					{
						builder.Append("<br>");
					}
				}
				builder.Append(property.IsIndexer ? ']' : ')');
			}
			if (base.IncludeBodies)
			{
				builder.Append(" { ");
				if (property.CanGet)
				{
					builder.Append("get; ");
				}
				if (property.CanSet)
				{
					builder.Append("set; ");
				}
				builder.Append(" } ");
			}
			return builder.ToString();
		}

		public override string Convert(IEvent e)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(this.Convert(e.Modifiers));
			if (base.ShowModifiers)
			{
				builder.Append(this.GetModifier(e));
			}
			builder.Append("event ");
			if (e.ReturnType != null && base.ShowReturnType)
			{
				builder.Append(this.Convert(e.ReturnType));
				builder.Append(' ');
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<b>");
			}
			if (base.UseFullyQualifiedMemberNames)
			{
				builder.Append(e.FullyQualifiedName);
			}
			else
			{
				builder.Append(e.Name);
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</b>");
			}
			if (base.IncludeBodies)
			{
				builder.Append(";");
			}
			return builder.ToString();
		}

		public override string Convert(IMethod m)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(this.Convert(m.Modifiers));
			if (base.ShowModifiers)
			{
				builder.Append(this.GetModifier(m));
			}
			if (m.ReturnType != null && base.ShowReturnType)
			{
				builder.Append(this.Convert(m.ReturnType));
				builder.Append(' ');
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<b>");
			}
			if (m.IsConstructor)
			{
				if (m.DeclaringType != null)
				{
					builder.Append(m.DeclaringType.Name);
				}
				else
				{
					builder.Append(m.Name);
				}
			}
			else if (base.UseFullyQualifiedMemberNames)
			{
				builder.Append(m.FullyQualifiedName);
			}
			else
			{
				builder.Append(m.Name);
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</b>");
			}
			if (m.TypeParameters.Count > 0)
			{
				builder.Append('<');
				for (int i = 0; i < m.TypeParameters.Count; i++)
				{
					if (i > 0)
					{
						builder.Append(", ");
					}
					builder.Append(m.TypeParameters[i].Name);
				}
				builder.Append('>');
			}
			builder.Append("(");
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<br>");
			}
			for (int i = 0; i < m.Parameters.Count; i++)
			{
				if (base.IncludeHTMLMarkup)
				{
					builder.Append("&nbsp;&nbsp;&nbsp;");
				}
				builder.Append(this.Convert(m.Parameters[i]));
				if (i + 1 < m.Parameters.Count)
				{
					builder.Append(", ");
				}
				if (base.IncludeHTMLMarkup)
				{
					builder.Append("<br>");
				}
			}
			builder.Append(')');
			if (base.IncludeBodies)
			{
				if (m.DeclaringType != null)
				{
					if (m.DeclaringType.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Interface)
					{
						builder.Append(";");
					}
					else
					{
						builder.Append(" {");
					}
				}
				else
				{
					builder.Append(" {");
				}
			}
			return builder.ToString();
		}

		public override string ConvertEnd(IMethod m)
		{
			return "}";
		}

		public override string Convert(IReturnType returnType)
		{
			string result;
			if (returnType == null)
			{
				result = string.Empty;
			}
			else
			{
				StringBuilder builder = new StringBuilder();
				string fullName = returnType.FullyQualifiedName;
				string shortName;
				if (fullName != null && CSharpAmbience.TypeConversionTable.TryGetValue(fullName, out shortName))
				{
					builder.Append(shortName);
				}
				else if (base.UseFullyQualifiedNames)
				{
					builder.Append(fullName);
				}
				else
				{
					builder.Append(returnType.Name);
				}
				this.UnpackNestedType(builder, returnType);
				result = builder.ToString();
			}
			return result;
		}

		private void UnpackNestedType(StringBuilder builder, IReturnType returnType)
		{
			if (returnType.IsArrayReturnType)
			{
				builder.Append('[');
				int dimensions = returnType.CastToArrayReturnType().ArrayDimensions;
				for (int i = 1; i < dimensions; i++)
				{
					builder.Append(',');
				}
				builder.Append(']');
				this.UnpackNestedType(builder, returnType.CastToArrayReturnType().ArrayElementType);
			}
			else if (returnType.IsConstructedReturnType)
			{
				this.UnpackNestedType(builder, returnType.CastToConstructedReturnType().UnboundType);
				builder.Append('<');
				IList<IReturnType> ta = returnType.CastToConstructedReturnType().TypeArguments;
				for (int i = 0; i < ta.Count; i++)
				{
					if (i > 0)
					{
						builder.Append(", ");
					}
					builder.Append(this.Convert(ta[i]));
				}
				builder.Append('>');
			}
		}

		public override string Convert(IParameter param)
		{
			StringBuilder builder = new StringBuilder();
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<i>");
			}
			if (param.IsRef)
			{
				builder.Append("ref ");
			}
			else if (param.IsOut)
			{
				builder.Append("out ");
			}
			else if (param.IsParams)
			{
				builder.Append("params ");
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</i>");
			}
			builder.Append(this.Convert(param.ReturnType));
			if (base.ShowParameterNames)
			{
				builder.Append(' ');
				builder.Append(param.Name);
			}
			return builder.ToString();
		}

		public override string WrapAttribute(string attribute)
		{
			return "[" + attribute + "]";
		}

		public override string WrapComment(string comment)
		{
			return "// " + comment;
		}

		public override string GetIntrinsicTypeName(string dotNetTypeName)
		{
			string shortName;
			string result;
			if (CSharpAmbience.TypeConversionTable.TryGetValue(dotNetTypeName, out shortName))
			{
				result = shortName;
			}
			else
			{
				result = dotNetTypeName;
			}
			return result;
		}
	}
}

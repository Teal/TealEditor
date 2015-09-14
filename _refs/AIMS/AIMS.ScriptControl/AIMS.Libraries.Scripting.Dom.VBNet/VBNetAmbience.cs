using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom.VBNet
{
	public class VBNetAmbience : AbstractAmbience
	{
		private static VBNetAmbience instance = new VBNetAmbience();

		public static IDictionary<string, string> TypeConversionTable
		{
			get
			{
				return TypeReference.PrimitiveTypesVBReverse;
			}
		}

		public static VBNetAmbience Instance
		{
			get
			{
				return VBNetAmbience.instance;
			}
		}

		private string GetModifier(IDecoration decoration)
		{
			StringBuilder builder = new StringBuilder();
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<i>");
			}
			if (decoration.IsStatic)
			{
				builder.Append("Shared ");
			}
			if (decoration.IsAbstract)
			{
				builder.Append("MustOverride ");
			}
			else if (decoration.IsSealed)
			{
				builder.Append("NotOverridable ");
			}
			else if (decoration.IsVirtual)
			{
				builder.Append("Overridable ");
			}
			else if (decoration.IsOverride)
			{
				builder.Append("Overrides ");
			}
			else if (decoration.IsNew)
			{
				builder.Append("Shadows ");
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</i>");
			}
			return builder.ToString();
		}

		public override string Convert(ModifierEnum modifier)
		{
			StringBuilder builder = new StringBuilder();
			if (base.ShowAccessibility)
			{
				if ((modifier & ModifierEnum.Public) == ModifierEnum.Public)
				{
					builder.Append("Public");
				}
				else if ((modifier & ModifierEnum.Private) == ModifierEnum.Private)
				{
					builder.Append("Private");
				}
				else if ((modifier & ModifierEnum.ProtectedAndInternal) == ModifierEnum.ProtectedAndInternal)
				{
					builder.Append("Protected Friend");
				}
				else if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal)
				{
					builder.Append("Friend");
				}
				else if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected)
				{
					builder.Append("Protected");
				}
				builder.Append(' ');
			}
			return builder.ToString();
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
					if (c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Class)
					{
						builder.Append("NotInheritable ");
					}
				}
				else if (c.IsAbstract && c.ClassType != AIMS.Libraries.Scripting.Dom.ClassType.Interface)
				{
					builder.Append("MustInherit ");
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
					builder.Append("Class");
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Enum:
					builder.Append("Enum");
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Interface:
					builder.Append("Interface");
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Struct:
					builder.Append("Structure");
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Delegate:
					builder.Append("Delegate ");
					if (base.ShowReturnType)
					{
						foreach (IMethod i in c.Methods)
						{
							if (!(i.Name != "Invoke"))
							{
								if (i.ReturnType == null || i.ReturnType.FullyQualifiedName == "System.Void")
								{
									builder.Append("Sub");
								}
								else
								{
									builder.Append("Function");
								}
							}
						}
					}
					break;
				case AIMS.Libraries.Scripting.Dom.ClassType.Module:
					builder.Append("Module");
					break;
				}
				builder.Append(' ');
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
				builder.Append("(Of ");
				for (int j = 0; j < c.TypeParameters.Count; j++)
				{
					if (j > 0)
					{
						builder.Append(", ");
					}
					builder.Append(c.TypeParameters[j].Name);
				}
				builder.Append(')');
			}
			if (base.ShowReturnType && c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Delegate)
			{
				builder.Append("(");
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
				builder.Append(")");
				foreach (IMethod i in c.Methods)
				{
					if (!(i.Name != "Invoke"))
					{
						if (i.ReturnType != null && !(i.ReturnType.FullyQualifiedName == "System.Void"))
						{
							if (base.ShowReturnType)
							{
								builder.Append(" As ");
								builder.Append(this.Convert(i.ReturnType));
							}
						}
					}
				}
			}
			else if (base.ShowInheritanceList)
			{
				if (c.BaseTypes.Count > 0)
				{
					builder.Append(" Inherits ");
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
			return builder.ToString();
		}

		public override string ConvertEnd(IClass c)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("End ");
			switch (c.ClassType)
			{
			case AIMS.Libraries.Scripting.Dom.ClassType.Class:
				builder.Append("Class");
				break;
			case AIMS.Libraries.Scripting.Dom.ClassType.Enum:
				builder.Append("Enum");
				break;
			case AIMS.Libraries.Scripting.Dom.ClassType.Interface:
				builder.Append("Interface");
				break;
			case AIMS.Libraries.Scripting.Dom.ClassType.Struct:
				builder.Append("Structure");
				break;
			case AIMS.Libraries.Scripting.Dom.ClassType.Delegate:
				builder.Append("Delegate");
				break;
			case AIMS.Libraries.Scripting.Dom.ClassType.Module:
				builder.Append("Module");
				break;
			}
			return builder.ToString();
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
					builder.Append("Const ");
				}
				else if (field.IsStatic)
				{
					builder.Append("Shared ");
				}
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</i>");
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
			if (field.ReturnType != null && base.ShowReturnType)
			{
				builder.Append(" As ");
				builder.Append(this.Convert(field.ReturnType));
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
			if (property.IsIndexer)
			{
				builder.Append("Default ");
			}
			if (property.CanGet && !property.CanSet)
			{
				builder.Append("ReadOnly ");
			}
			if (property.CanSet && !property.CanGet)
			{
				builder.Append("WriteOnly ");
			}
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
			if (property.Parameters.Count > 0)
			{
				builder.Append("(");
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
				builder.Append(')');
			}
			if (property.ReturnType != null && base.ShowReturnType)
			{
				builder.Append(" As ");
				builder.Append(this.Convert(property.ReturnType));
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
			builder.Append("Event ");
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
			if (e.ReturnType != null && base.ShowReturnType)
			{
				builder.Append(" As ");
				builder.Append(this.Convert(e.ReturnType));
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
			if (base.ShowReturnType)
			{
				if (m.ReturnType == null || m.ReturnType.FullyQualifiedName == "System.Void")
				{
					builder.Append("Sub ");
				}
				else
				{
					builder.Append("Function ");
				}
			}
			string dispName = base.UseFullyQualifiedMemberNames ? m.FullyQualifiedName : m.Name;
			if (m.Name == "#ctor" || m.Name == "#cctor" || m.IsConstructor)
			{
				dispName = "New";
			}
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("<b>");
			}
			builder.Append(dispName);
			if (base.IncludeHTMLMarkup)
			{
				builder.Append("</b>");
			}
			if (m.TypeParameters.Count > 0)
			{
				builder.Append("(Of ");
				for (int i = 0; i < m.TypeParameters.Count; i++)
				{
					if (i > 0)
					{
						builder.Append(", ");
					}
					builder.Append(m.TypeParameters[i].Name);
				}
				builder.Append(')');
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
			if (base.ShowReturnType && m.ReturnType != null && m.ReturnType.FullyQualifiedName != "System.Void")
			{
				builder.Append(" As ");
				builder.Append(this.Convert(m.ReturnType));
			}
			return builder.ToString();
		}

		public override string ConvertEnd(IMethod m)
		{
			string result;
			if (m.ReturnType == null || m.ReturnType.FullyQualifiedName == "System.Void")
			{
				result = "End Sub";
			}
			else
			{
				result = "End Function";
			}
			return result;
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
				if (fullName != null && VBNetAmbience.TypeConversionTable.TryGetValue(fullName, out shortName))
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
				builder.Append('(');
				int dimensions = returnType.CastToArrayReturnType().ArrayDimensions;
				for (int i = 1; i < dimensions; i++)
				{
					builder.Append(',');
				}
				builder.Append(')');
				this.UnpackNestedType(builder, returnType.CastToArrayReturnType().ArrayElementType);
			}
			else if (returnType.IsConstructedReturnType)
			{
				this.UnpackNestedType(builder, returnType.CastToConstructedReturnType().UnboundType);
				builder.Append("(Of ");
				IList<IReturnType> ta = returnType.CastToConstructedReturnType().TypeArguments;
				for (int i = 0; i < ta.Count; i++)
				{
					if (i > 0)
					{
						builder.Append(", ");
					}
					builder.Append(this.Convert(ta[i]));
				}
				builder.Append(')');
			}
		}

		public override string Convert(IParameter param)
		{
			StringBuilder builder = new StringBuilder();
			if (base.ShowParameterNames)
			{
				if (base.IncludeHTMLMarkup)
				{
					builder.Append("<i>");
				}
				if (param.IsOptional)
				{
					builder.Append("Optional ");
				}
				if (param.IsRef || param.IsOut)
				{
					builder.Append("ByRef ");
				}
				else if (param.IsParams)
				{
					builder.Append("ParamArray ");
				}
				if (base.IncludeHTMLMarkup)
				{
					builder.Append("</i>");
				}
				builder.Append(param.Name);
				builder.Append(" As ");
			}
			builder.Append(this.Convert(param.ReturnType));
			return builder.ToString();
		}

		public override string WrapAttribute(string attribute)
		{
			return "<" + attribute + ">";
		}

		public override string WrapComment(string comment)
		{
			return "' " + comment;
		}

		public override string GetIntrinsicTypeName(string dotNetTypeName)
		{
			string shortName;
			string result;
			if (VBNetAmbience.TypeConversionTable.TryGetValue(dotNetTypeName, out shortName))
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

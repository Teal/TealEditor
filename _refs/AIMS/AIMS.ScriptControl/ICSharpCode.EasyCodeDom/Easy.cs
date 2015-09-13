using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public static class Easy
	{
		public static EasyExpression This
		{
			get
			{
				return new EasyExpression(new CodeThisReferenceExpression());
			}
		}

		public static EasyExpression Base
		{
			get
			{
				return new EasyExpression(new CodeBaseReferenceExpression());
			}
		}

		public static EasyExpression Value
		{
			get
			{
				return new EasyExpression(new CodePropertySetValueReferenceExpression());
			}
		}

		public static EasyExpression Null
		{
			get
			{
				return new EasyExpression(new CodePrimitiveExpression(null));
			}
		}

		public static CodeTypeReference TypeRef(Type type)
		{
			return new CodeTypeReference(type, CodeTypeReferenceOptions.GlobalReference);
		}

		public static CodeTypeReference TypeRef(CodeTypeDeclaration type)
		{
			return new CodeTypeReference(type.Name);
		}

		public static CodeTypeReference TypeRef(string typeName, params string[] typeArguments)
		{
			CodeTypeReference tr = new CodeTypeReference(typeName);
			for (int i = 0; i < typeArguments.Length; i++)
			{
				string ta = typeArguments[i];
				tr.TypeArguments.Add(ta);
			}
			return tr;
		}

		public static EasyExpression Prim(object literalValue)
		{
			EasyExpression result;
			if (literalValue is Enum)
			{
				result = Easy.Type(literalValue.GetType()).Field(literalValue.ToString());
			}
			else
			{
				result = new EasyExpression(new CodePrimitiveExpression(literalValue));
			}
			return result;
		}

		public static EasyExpression Type(Type type)
		{
			return Easy.Type(Easy.TypeRef(type));
		}

		public static EasyExpression Type(CodeTypeReference type)
		{
			return new EasyExpression(new CodeTypeReferenceExpression(type));
		}

		public static EasyExpression Type(string type)
		{
			return Easy.Type(new CodeTypeReference(type));
		}

		public static EasyExpression TypeOf(Type type)
		{
			return Easy.TypeOf(Easy.TypeRef(type));
		}

		public static EasyExpression TypeOf(CodeTypeReference type)
		{
			return new EasyExpression(new CodeTypeOfExpression(type));
		}

		public static EasyExpression New(Type type, params CodeExpression[] arguments)
		{
			return Easy.New(Easy.TypeRef(type), arguments);
		}

		public static EasyExpression New(CodeTypeReference type, params CodeExpression[] arguments)
		{
			return new EasyExpression(new CodeObjectCreateExpression(type, arguments));
		}

		public static EasyExpression Var(string name)
		{
			return new EasyExpression(new CodeVariableReferenceExpression(name));
		}

		public static EasyExpression Binary(CodeExpression left, CodeBinaryOperatorType op, CodeExpression right)
		{
			return new EasyExpression(new CodeBinaryOperatorExpression(left, op, right));
		}

		public static void AddSummary(CodeTypeMember member, string summary)
		{
			member.Comments.Add(new CodeCommentStatement("<summary>", true));
			member.Comments.Add(new CodeCommentStatement(summary, true));
			member.Comments.Add(new CodeCommentStatement("</summary>", true));
		}

		internal static CodeAttributeDeclaration AddAttribute(CodeAttributeDeclarationCollection col, CodeTypeReference type, CodeExpression[] arguments)
		{
			CodeAttributeArgument[] attributeArguments = new CodeAttributeArgument[arguments.Length];
			for (int i = 0; i < arguments.Length; i++)
			{
				attributeArguments[i] = new CodeAttributeArgument(arguments[i]);
			}
			CodeAttributeDeclaration cad = new CodeAttributeDeclaration(type, attributeArguments);
			col.Add(cad);
			return cad;
		}
	}
}

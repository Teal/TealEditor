using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Security.Permissions;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class CodeDomVerboseOutputGenerator : CodeGenerator
	{
		protected override string NullToken
		{
			get
			{
				return "[NULL]";
			}
		}

		protected override void OutputType(CodeTypeReference typeRef)
		{
			base.Output.Write("[CodeTypeReference: {0}", typeRef.BaseType);
			if (typeRef.ArrayRank > 0)
			{
				base.Output.Write(" Rank:" + typeRef.ArrayRank);
			}
			base.Output.Write("]");
		}

		protected override void GenerateArrayCreateExpression(CodeArrayCreateExpression e)
		{
			base.Output.Write("[CodeArrayCreateExpression: {0}]", e.ToString());
		}

		protected override void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e)
		{
			base.Output.Write("[CodeBaseReferenceExpression: {0}]", e.ToString());
		}

		protected override void GenerateCastExpression(CodeCastExpression e)
		{
			base.Output.Write("[CodeCastExpression: {0}]", e.ToString());
		}

		protected override void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e)
		{
			base.Output.Write("[CodeDelegateCreateExpression: {0}]", e.ToString());
		}

		protected override void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
		{
			base.Output.Write("[CodeFieldReferenceExpression: Name={0}, Target=", e.FieldName);
			base.GenerateExpression(e.TargetObject);
			base.Output.Write("]");
		}

		protected override void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e)
		{
			base.Output.Write("[CodeMethodReferenceExpression: Name={0}, Target=", e.MethodName);
			base.GenerateExpression(e.TargetObject);
			base.Output.Write("]");
		}

		protected override void GenerateEventReferenceExpression(CodeEventReferenceExpression e)
		{
			base.Output.Write("[CodeEventReferenceExpression: Name={0}, Target=", e.EventName);
			base.GenerateExpression(e.TargetObject);
			base.Output.Write("]");
		}

		protected override void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e)
		{
			base.Output.Write("[CodeArgumentReferenceExpression: {0}]", e.ToString());
		}

		protected override void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e)
		{
			base.Output.Write("[CodeVariableReferenceExpression: Name={0}]", e.VariableName);
		}

		protected override void GenerateIndexerExpression(CodeIndexerExpression e)
		{
			base.Output.Write("[CodeIndexerExpression: {0}]", e.ToString());
		}

		protected override void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e)
		{
			base.Output.Write("[CodeArrayIndexerExpression: {0}]", e.ToString());
		}

		protected override void GenerateSnippetExpression(CodeSnippetExpression e)
		{
			base.Output.Write("[CodeSnippetExpression: {0}]", e.ToString());
		}

		protected override void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
		{
			base.Output.Write("[CodeMethodInvokeExpression: Method=");
			this.GenerateMethodReferenceExpression(e.Method);
			base.Output.Write(", Parameters=");
			bool first = true;
			foreach (CodeExpression expr in e.Parameters)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					base.Output.Write(", ");
				}
				base.GenerateExpression(expr);
			}
			base.Output.Write("]");
		}

		protected override void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e)
		{
			base.Output.Write("[CodeDelegateInvokeExpression: {0}]", e.ToString());
		}

		protected override void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
		{
			base.Output.Write("[CodeObjectCreateExpression: Type={0}, Parameters=", e.CreateType.BaseType);
			bool first = true;
			foreach (CodeExpression expr in e.Parameters)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					base.Output.Write(", ");
				}
				base.GenerateExpression(expr);
			}
			base.Output.Write("]");
		}

		protected override void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e)
		{
			base.Output.Write("[CodePropertyReferenceExpression: Name={0}, Target=", e.PropertyName);
			base.GenerateExpression(e.TargetObject);
			base.Output.Write("]");
		}

		protected override void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e)
		{
			base.Output.Write("[CodePropertySetValueReferenceExpression: {0}]", e.ToString());
		}

		protected override void GenerateThisReferenceExpression(CodeThisReferenceExpression e)
		{
			base.Output.Write("[CodeThisReferenceExpression]");
		}

		protected override void GenerateExpressionStatement(CodeExpressionStatement e)
		{
			base.Output.Write("[CodeExpressionStatement:");
			base.GenerateExpression(e.Expression);
			base.Output.WriteLine("]");
		}

		protected override void GenerateIterationStatement(CodeIterationStatement e)
		{
			base.Output.WriteLine("[CodeIterationStatement: {0}]", e.ToString());
		}

		protected override void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e)
		{
			base.Output.WriteLine("[CodeThrowExceptionStatement: {0}]", e.ToString());
		}

		protected override void GenerateComment(CodeComment e)
		{
			base.Output.WriteLine("[CodeComment: {0}]", e.ToString());
		}

		protected override void GenerateMethodReturnStatement(CodeMethodReturnStatement e)
		{
			base.Output.WriteLine("[CodeMethodReturnStatement: {0}]", e.ToString());
		}

		protected override void GenerateConditionStatement(CodeConditionStatement e)
		{
			base.Output.WriteLine("[GenerateConditionStatement: {0}]", e.ToString());
		}

		protected override void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e)
		{
			base.Output.WriteLine("[CodeTryCatchFinallyStatement: {0}]", e.ToString());
		}

		protected override void GenerateAssignStatement(CodeAssignStatement e)
		{
			base.Output.Write("[CodeAssignStatement: Left=");
			base.GenerateExpression(e.Left);
			base.Output.Write(", Right=");
			base.GenerateExpression(e.Right);
			base.Output.WriteLine("]");
		}

		protected override void GenerateAttachEventStatement(CodeAttachEventStatement e)
		{
			base.Output.WriteLine("[CodeAttachEventStatement: {0}]", e.ToString());
		}

		protected override void GenerateRemoveEventStatement(CodeRemoveEventStatement e)
		{
			base.Output.WriteLine("[CodeRemoveEventStatement: {0}]", e.ToString());
		}

		protected override void GenerateGotoStatement(CodeGotoStatement e)
		{
			base.Output.WriteLine("[CodeGotoStatement: {0}]", e.ToString());
		}

		protected override void GenerateLabeledStatement(CodeLabeledStatement e)
		{
			base.Output.WriteLine("[CodeLabeledStatement: {0}]", e.ToString());
		}

		protected override void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e)
		{
			base.Output.WriteLine("[CodeVariableDeclarationStatement: {0}]", e.ToString());
		}

		protected override void GenerateLinePragmaStart(CodeLinePragma e)
		{
			base.Output.WriteLine("[CodeLinePragma: {0}]", e.ToString());
		}

		protected override void GenerateLinePragmaEnd(CodeLinePragma e)
		{
			base.Output.WriteLine("[CodeLinePragma: {0}]", e.ToString());
		}

		protected override void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c)
		{
			base.Output.WriteLine("[CodeMemberEvent: {0}]", e.ToString());
		}

		protected override void GenerateField(CodeMemberField e)
		{
			base.Output.Write("[CodeMemberField: Name={0}, Type=", e.Name);
			base.Output.Write(e.Type.BaseType);
			base.Output.WriteLine("]");
		}

		protected override void GenerateSnippetMember(CodeSnippetTypeMember e)
		{
			base.Output.WriteLine("[CodeSnippetTypeMember: {0}]", e.ToString());
		}

		protected override void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c)
		{
			base.Output.WriteLine("[CodeEntryPointMethod: {0}]", e.ToString());
		}

		public void PublicGenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o)
		{
			((ICodeGenerator)this).GenerateCodeFromStatement(e, w, o);
		}

		protected override void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c)
		{
			base.Output.WriteLine("[CodeMemberMethod: Name={0}, Parameterns={1}]", e.Name, e.Parameters.Count);
			base.Indent++;
			base.GenerateStatements(e.Statements);
			base.Indent--;
		}

		protected override void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c)
		{
			base.Output.WriteLine("[CodeMemberProperty : {0}]", e.ToString());
		}

		protected override void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c)
		{
			base.Output.WriteLine("[CodeConstructor : {0}]", e.ToString());
			base.Indent++;
			base.GenerateStatements(e.Statements);
			base.Indent--;
		}

		protected override void GenerateTypeConstructor(CodeTypeConstructor e)
		{
			base.Output.WriteLine("[CodeTypeConstructor : {0}]", e.ToString());
		}

		protected override void GenerateTypeStart(CodeTypeDeclaration e)
		{
			base.Output.WriteLine("[CodeTypeDeclaration : {0}]", e.ToString());
		}

		protected override void GenerateTypeEnd(CodeTypeDeclaration e)
		{
			base.Output.WriteLine("[CodeTypeDeclaration: {0}]", e.ToString());
		}

		protected override void GenerateNamespaceStart(CodeNamespace e)
		{
			base.Output.WriteLine("[CodeNamespaceStart: {0}]", e.ToString());
		}

		protected override void GenerateNamespaceEnd(CodeNamespace e)
		{
			base.Output.WriteLine("[CodeNamespaceEnd: {0}]", e.ToString());
		}

		protected override void GenerateNamespaceImport(CodeNamespaceImport e)
		{
			base.Output.WriteLine("[CodeNamespaceImport: {0}]", e.ToString());
		}

		protected override void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes)
		{
			base.Output.WriteLine("[CodeAttributeDeclarationCollection: {0}]", attributes.ToString());
		}

		protected override void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes)
		{
			base.Output.WriteLine("[CodeAttributeDeclarationCollection: {0}]", attributes.ToString());
		}

		protected override bool Supports(GeneratorSupport support)
		{
			return true;
		}

		protected override bool IsValidIdentifier(string value)
		{
			return true;
		}

		protected override string CreateEscapedIdentifier(string value)
		{
			return value;
		}

		protected override string CreateValidIdentifier(string value)
		{
			return value;
		}

		protected override string GetTypeOutput(CodeTypeReference value)
		{
			return value.ToString();
		}

		protected override string QuoteSnippetString(string value)
		{
			return "\"" + value + "\"";
		}
	}
}

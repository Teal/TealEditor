using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
	public class ToCSharpConvertVisitor : AbstractAstTransformer
	{
		private sealed class ReplaceWithAccessTransformer : AbstractAstTransformer
		{
			private readonly Expression replaceWith;

			public ReplaceWithAccessTransformer(Expression replaceWith)
			{
				this.replaceWith = replaceWith;
			}

			public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
			{
				object result;
				if (fieldReferenceExpression.TargetObject.IsNull)
				{
					fieldReferenceExpression.TargetObject = this.replaceWith;
					result = null;
				}
				else
				{
					result = base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
				}
				return result;
			}

			public override object VisitWithStatement(WithStatement withStatement, object data)
			{
				return withStatement.Expression.AcceptVisitor(this, data);
			}
		}

		public override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			if (!eventDeclaration.HasAddRegion && !eventDeclaration.HasRaiseRegion && !eventDeclaration.HasRemoveRegion)
			{
				if (eventDeclaration.TypeReference.IsNull)
				{
					DelegateDeclaration dd = new DelegateDeclaration(eventDeclaration.Modifier, null);
					dd.Name = eventDeclaration.Name + "EventHandler";
					dd.Parameters = eventDeclaration.Parameters;
					dd.ReturnType = new TypeReference("System.Void");
					dd.Parent = eventDeclaration.Parent;
					eventDeclaration.Parameters = null;
					int index = eventDeclaration.Parent.Children.IndexOf(eventDeclaration);
					eventDeclaration.Parent.Children.Insert(index + 1, dd);
					eventDeclaration.TypeReference = new TypeReference(dd.Name);
				}
			}
			return base.VisitEventDeclaration(eventDeclaration, data);
		}

		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
			if ((localVariableDeclaration.Modifier & Modifiers.Static) == Modifiers.Static)
			{
				INode parent = localVariableDeclaration.Parent;
				while (parent != null && !ToCSharpConvertVisitor.IsTypeLevel(parent))
				{
					parent = parent.Parent;
				}
				if (parent != null)
				{
					INode type = parent.Parent;
					if (type != null)
					{
						int pos = type.Children.IndexOf(parent);
						if (pos >= 0)
						{
							FieldDeclaration field = new FieldDeclaration(null);
							field.TypeReference = localVariableDeclaration.TypeReference;
							field.Modifier = Modifiers.Static;
							field.Fields = localVariableDeclaration.Variables;
							new PrefixFieldsVisitor(field.Fields, "static_" + ToCSharpConvertVisitor.GetTypeLevelEntityName(parent) + "_").Run(parent);
							type.Children.Insert(pos + 1, field);
							base.RemoveCurrentNode();
						}
					}
				}
			}
			return null;
		}

		public override object VisitWithStatement(WithStatement withStatement, object data)
		{
			withStatement.Body.AcceptVisitor(new ToCSharpConvertVisitor.ReplaceWithAccessTransformer(withStatement.Expression), data);
			base.VisitWithStatement(withStatement, data);
			base.ReplaceCurrentNode(withStatement.Body);
			return null;
		}

		private static bool IsTypeLevel(INode node)
		{
			return node is MethodDeclaration || node is PropertyDeclaration || node is EventDeclaration || node is OperatorDeclaration || node is FieldDeclaration;
		}

		private static string GetTypeLevelEntityName(INode node)
		{
			string name;
			if (node is ParametrizedNode)
			{
				name = ((ParametrizedNode)node).Name;
			}
			else
			{
				if (!(node is FieldDeclaration))
				{
					throw new ArgumentException();
				}
				name = ((FieldDeclaration)node).Fields[0].Name;
			}
			return name;
		}
	}
}

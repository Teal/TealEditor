using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public sealed class CSharpOutputVisitor : IOutputAstVisitor, IAstVisitor
	{
		private Errors errors = new Errors();

		private CSharpOutputFormatter outputFormatter;

		private PrettyPrintOptions prettyPrintOptions = new PrettyPrintOptions();

		private NodeTracker nodeTracker;

		private bool printFullSystemType;

		private TypeDeclaration currentType = null;

		public string Text
		{
			get
			{
				return this.outputFormatter.Text;
			}
		}

		public Errors Errors
		{
			get
			{
				return this.errors;
			}
		}

		AbstractPrettyPrintOptions IOutputAstVisitor.Options
		{
			get
			{
				return this.prettyPrintOptions;
			}
		}

		public PrettyPrintOptions Options
		{
			get
			{
				return this.prettyPrintOptions;
			}
		}

		public IOutputFormatter OutputFormatter
		{
			get
			{
				return this.outputFormatter;
			}
		}

		public NodeTracker NodeTracker
		{
			get
			{
				return this.nodeTracker;
			}
		}

		public CSharpOutputVisitor()
		{
			this.outputFormatter = new CSharpOutputFormatter(this.prettyPrintOptions);
			this.nodeTracker = new NodeTracker(this);
		}

		private void Error(INode node, string message)
		{
			this.outputFormatter.PrintText(" // ERROR: " + message + Environment.NewLine);
			this.errors.Error(node.StartLocation.Y, node.StartLocation.X, message);
		}

		private void NotSupported(INode node)
		{
			this.Error(node, "Not supported in C#: " + node.GetType().Name);
		}

		public object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			this.nodeTracker.TrackedVisitChildren(compilationUnit, data);
			this.outputFormatter.EndFile();
			return null;
		}

		private static string ConvertTypeString(string typeString)
		{
			string primitiveType;
			string result;
			if (TypeReference.PrimitiveTypesCSharpReverse.TryGetValue(typeString, out primitiveType))
			{
				result = primitiveType;
			}
			else
			{
				result = typeString;
			}
			return result;
		}

		private void PrintTemplates(List<TemplateDefinition> templates)
		{
			if (templates.Count != 0)
			{
				this.outputFormatter.PrintToken(23);
				for (int i = 0; i < templates.Count; i++)
				{
					if (i > 0)
					{
						this.PrintFormattedComma();
					}
					this.outputFormatter.PrintIdentifier(templates[i].Name);
				}
				this.outputFormatter.PrintToken(22);
			}
		}

		public object VisitTypeReference(TypeReference typeReference, object data)
		{
			if (typeReference == TypeReference.ClassConstraint)
			{
				this.outputFormatter.PrintToken(58);
			}
			else if (typeReference == TypeReference.StructConstraint)
			{
				this.outputFormatter.PrintToken(108);
			}
			else if (typeReference == TypeReference.NewConstraint)
			{
				this.outputFormatter.PrintToken(88);
				this.outputFormatter.PrintToken(20);
				this.outputFormatter.PrintToken(21);
			}
			else
			{
				this.PrintTypeReferenceWithoutArray(typeReference);
				if (typeReference.IsArrayType)
				{
					this.PrintArrayRank(typeReference.RankSpecifier, 0);
				}
			}
			return null;
		}

		private void PrintArrayRank(int[] rankSpecifier, int startRankIndex)
		{
			for (int i = startRankIndex; i < rankSpecifier.Length; i++)
			{
				this.outputFormatter.PrintToken(18);
				if (this.prettyPrintOptions.SpacesWithinBrackets)
				{
					this.outputFormatter.Space();
				}
				for (int j = 0; j < rankSpecifier[i]; j++)
				{
					this.outputFormatter.PrintToken(14);
				}
				if (this.prettyPrintOptions.SpacesWithinBrackets)
				{
					this.outputFormatter.Space();
				}
				this.outputFormatter.PrintToken(19);
			}
		}

		private void PrintTypeReferenceWithoutArray(TypeReference typeReference)
		{
			if (typeReference.IsGlobal)
			{
				this.outputFormatter.PrintText("global::");
			}
			if (typeReference.Type == null || typeReference.Type.Length == 0)
			{
				this.outputFormatter.PrintText("void");
			}
			else if (typeReference.SystemType == "System.Nullable" && typeReference.GenericTypes != null && typeReference.GenericTypes.Count == 1 && !typeReference.IsGlobal)
			{
				this.nodeTracker.TrackedVisit(typeReference.GenericTypes[0], null);
				this.outputFormatter.PrintText("?");
			}
			else
			{
				if (typeReference.SystemType.Length > 0)
				{
					if (this.printFullSystemType || typeReference.IsGlobal)
					{
						this.outputFormatter.PrintIdentifier(typeReference.SystemType);
					}
					else
					{
						this.outputFormatter.PrintText(CSharpOutputVisitor.ConvertTypeString(typeReference.SystemType));
					}
				}
				else
				{
					this.outputFormatter.PrintText(typeReference.Type);
				}
				if (typeReference.GenericTypes != null && typeReference.GenericTypes.Count > 0)
				{
					this.outputFormatter.PrintToken(23);
					this.AppendCommaSeparatedList<TypeReference>(typeReference.GenericTypes);
					this.outputFormatter.PrintToken(22);
				}
			}
			for (int i = 0; i < typeReference.PointerNestingLevel; i++)
			{
				this.outputFormatter.PrintToken(6);
			}
		}

		public object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			this.nodeTracker.TrackedVisit(innerClassTypeReference.BaseType, data);
			this.outputFormatter.PrintToken(15);
			return this.VisitTypeReference(innerClassTypeReference, data);
		}

		private void VisitAttributes(ICollection attributes, object data)
		{
			if (attributes != null && attributes.Count > 0)
			{
				foreach (AttributeSection section in attributes)
				{
					this.nodeTracker.TrackedVisit(section, data);
				}
			}
		}

		private void PrintFormattedComma()
		{
			if (this.prettyPrintOptions.SpacesBeforeComma)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(14);
			if (this.prettyPrintOptions.SpacesAfterComma)
			{
				this.outputFormatter.Space();
			}
		}

		public object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(18);
			if (this.prettyPrintOptions.SpacesWithinBrackets)
			{
				this.outputFormatter.Space();
			}
			if (!string.IsNullOrEmpty(attributeSection.AttributeTarget))
			{
				this.outputFormatter.PrintText(attributeSection.AttributeTarget);
				this.outputFormatter.PrintToken(9);
				this.outputFormatter.Space();
			}
			Debug.Assert(attributeSection.Attributes != null);
			for (int i = 0; i < attributeSection.Attributes.Count; i++)
			{
				this.nodeTracker.TrackedVisit(attributeSection.Attributes[i], data);
				if (i + 1 < attributeSection.Attributes.Count)
				{
					this.PrintFormattedComma();
				}
			}
			if (this.prettyPrintOptions.SpacesWithinBrackets)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(19);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitAttribute(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
		{
			this.outputFormatter.PrintIdentifier(attribute.Name);
			this.outputFormatter.PrintToken(20);
			this.AppendCommaSeparatedList<Expression>(attribute.PositionalArguments);
			if (attribute.NamedArguments != null && attribute.NamedArguments.Count > 0)
			{
				if (attribute.PositionalArguments.Count > 0)
				{
					this.PrintFormattedComma();
				}
				for (int i = 0; i < attribute.NamedArguments.Count; i++)
				{
					this.nodeTracker.TrackedVisit(attribute.NamedArguments[i], data);
					if (i + 1 < attribute.NamedArguments.Count)
					{
						this.PrintFormattedComma();
					}
				}
			}
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			this.outputFormatter.PrintIdentifier(namedArgumentExpression.Name);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(3);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(namedArgumentExpression.Expression, data);
			return null;
		}

		public object VisitUsing(Using @using, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(120);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(@using.Name);
			if (@using.IsAlias)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(3);
				this.outputFormatter.Space();
				this.printFullSystemType = true;
				this.nodeTracker.TrackedVisit(@using.Alias, data);
				this.printFullSystemType = false;
			}
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			foreach (Using u in usingDeclaration.Usings)
			{
				this.nodeTracker.TrackedVisit(u, data);
			}
			return null;
		}

		public object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(87);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(namespaceDeclaration.Name);
			this.outputFormatter.BeginBrace(this.prettyPrintOptions.NamespaceBraceStyle);
			this.nodeTracker.TrackedVisitChildren(namespaceDeclaration, data);
			this.outputFormatter.EndBrace();
			return null;
		}

		private void OutputEnumMembers(TypeDeclaration typeDeclaration, object data)
		{
			for (int i = 0; i < typeDeclaration.Children.Count; i++)
			{
				FieldDeclaration fieldDeclaration = (FieldDeclaration)typeDeclaration.Children[i];
				this.nodeTracker.BeginNode(fieldDeclaration);
				VariableDeclaration f = fieldDeclaration.Fields[0];
				this.VisitAttributes(fieldDeclaration.Attributes, data);
				this.outputFormatter.Indent();
				this.outputFormatter.PrintIdentifier(f.Name);
				if (f.Initializer != null && !f.Initializer.IsNull)
				{
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(3);
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(f.Initializer, data);
				}
				if (i < typeDeclaration.Children.Count - 1)
				{
					this.outputFormatter.PrintToken(14);
				}
				this.outputFormatter.NewLine();
				this.nodeTracker.EndNode(fieldDeclaration);
			}
		}

		public object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			this.VisitAttributes(typeDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(typeDeclaration.Modifier);
			switch (typeDeclaration.Type)
			{
			case ClassType.Interface:
				this.outputFormatter.PrintToken(82);
				break;
			case ClassType.Struct:
				this.outputFormatter.PrintToken(108);
				break;
			case ClassType.Enum:
				this.outputFormatter.PrintToken(67);
				break;
			default:
				this.outputFormatter.PrintToken(58);
				break;
			}
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(typeDeclaration.Name);
			this.PrintTemplates(typeDeclaration.Templates);
			if (typeDeclaration.BaseTypes != null && typeDeclaration.BaseTypes.Count > 0)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(9);
				this.outputFormatter.Space();
				for (int i = 0; i < typeDeclaration.BaseTypes.Count; i++)
				{
					if (i > 0)
					{
						this.PrintFormattedComma();
					}
					this.nodeTracker.TrackedVisit(typeDeclaration.BaseTypes[i], data);
				}
			}
			foreach (TemplateDefinition templateDefinition in typeDeclaration.Templates)
			{
				this.nodeTracker.TrackedVisit(templateDefinition, data);
			}
			switch (typeDeclaration.Type)
			{
			case ClassType.Interface:
				this.outputFormatter.BeginBrace(this.prettyPrintOptions.InterfaceBraceStyle);
				break;
			case ClassType.Struct:
				this.outputFormatter.BeginBrace(this.prettyPrintOptions.StructBraceStyle);
				break;
			case ClassType.Enum:
				this.outputFormatter.BeginBrace(this.prettyPrintOptions.EnumBraceStyle);
				break;
			default:
				this.outputFormatter.BeginBrace(this.prettyPrintOptions.ClassBraceStyle);
				break;
			}
			TypeDeclaration oldType = this.currentType;
			this.currentType = typeDeclaration;
			if (typeDeclaration.Type == ClassType.Enum)
			{
				this.OutputEnumMembers(typeDeclaration, data);
			}
			else
			{
				this.nodeTracker.TrackedVisitChildren(typeDeclaration, data);
			}
			this.currentType = oldType;
			this.outputFormatter.EndBrace();
			return null;
		}

		public object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			object result;
			if (templateDefinition.Bases.Count == 0)
			{
				result = null;
			}
			else
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintText("where");
				this.outputFormatter.Space();
				this.outputFormatter.PrintIdentifier(templateDefinition.Name);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(9);
				this.outputFormatter.Space();
				for (int i = 0; i < templateDefinition.Bases.Count; i++)
				{
					this.nodeTracker.TrackedVisit(templateDefinition.Bases[i], data);
					if (i + 1 < templateDefinition.Bases.Count)
					{
						this.PrintFormattedComma();
					}
				}
				result = null;
			}
			return result;
		}

		public object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			this.VisitAttributes(delegateDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(delegateDeclaration.Modifier);
			this.outputFormatter.PrintToken(63);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(delegateDeclaration.ReturnType, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(delegateDeclaration.Name);
			this.PrintTemplates(delegateDeclaration.Templates);
			if (this.prettyPrintOptions.BeforeDelegateDeclarationParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(delegateDeclaration.Parameters);
			this.outputFormatter.PrintToken(21);
			foreach (TemplateDefinition templateDefinition in delegateDeclaration.Templates)
			{
				this.nodeTracker.TrackedVisit(templateDefinition, data);
			}
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			this.NotSupported(optionDeclaration);
			return null;
		}

		public object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			if (!fieldDeclaration.TypeReference.IsNull)
			{
				this.VisitAttributes(fieldDeclaration.Attributes, data);
				this.outputFormatter.Indent();
				this.OutputModifier(fieldDeclaration.Modifier);
				this.nodeTracker.TrackedVisit(fieldDeclaration.TypeReference, data);
				this.outputFormatter.Space();
				this.AppendCommaSeparatedList<VariableDeclaration>(fieldDeclaration.Fields);
				this.outputFormatter.PrintToken(11);
				this.outputFormatter.NewLine();
			}
			else
			{
				for (int i = 0; i < fieldDeclaration.Fields.Count; i++)
				{
					this.VisitAttributes(fieldDeclaration.Attributes, data);
					this.outputFormatter.Indent();
					this.OutputModifier(fieldDeclaration.Modifier);
					this.nodeTracker.TrackedVisit(fieldDeclaration.GetTypeForField(i), data);
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(fieldDeclaration.Fields[i], data);
					this.outputFormatter.PrintToken(11);
					this.outputFormatter.NewLine();
				}
			}
			return null;
		}

		public object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			this.outputFormatter.PrintIdentifier(variableDeclaration.Name);
			if (!variableDeclaration.FixedArrayInitialization.IsNull)
			{
				this.outputFormatter.PrintToken(18);
				this.nodeTracker.TrackedVisit(variableDeclaration.FixedArrayInitialization, data);
				this.outputFormatter.PrintToken(19);
			}
			if (!variableDeclaration.Initializer.IsNull)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(3);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(variableDeclaration.Initializer, data);
			}
			return null;
		}

		public object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			this.VisitAttributes(propertyDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			propertyDeclaration.Modifier &= ~Modifiers.ReadOnly;
			this.OutputModifier(propertyDeclaration.Modifier);
			this.nodeTracker.TrackedVisit(propertyDeclaration.TypeReference, data);
			this.outputFormatter.Space();
			if (propertyDeclaration.InterfaceImplementations.Count > 0)
			{
				this.nodeTracker.TrackedVisit(propertyDeclaration.InterfaceImplementations[0].InterfaceType, data);
				this.outputFormatter.PrintToken(15);
			}
			this.outputFormatter.PrintIdentifier(propertyDeclaration.Name);
			this.outputFormatter.BeginBrace(this.prettyPrintOptions.PropertyBraceStyle);
			this.nodeTracker.TrackedVisit(propertyDeclaration.GetRegion, data);
			this.nodeTracker.TrackedVisit(propertyDeclaration.SetRegion, data);
			this.outputFormatter.EndBrace();
			return null;
		}

		public object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			this.VisitAttributes(propertyGetRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(propertyGetRegion.Modifier);
			this.outputFormatter.PrintText("get");
			this.OutputBlockAllowInline(propertyGetRegion.Block, this.prettyPrintOptions.PropertyGetBraceStyle);
			return null;
		}

		public object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			this.VisitAttributes(propertySetRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(propertySetRegion.Modifier);
			this.outputFormatter.PrintText("set");
			this.OutputBlockAllowInline(propertySetRegion.Block, this.prettyPrintOptions.PropertySetBraceStyle);
			return null;
		}

		public object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			this.VisitAttributes(eventDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(eventDeclaration.Modifier);
			this.outputFormatter.PrintToken(68);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(eventDeclaration.TypeReference, data);
			this.outputFormatter.Space();
			if (eventDeclaration.InterfaceImplementations.Count > 0)
			{
				this.nodeTracker.TrackedVisit(eventDeclaration.InterfaceImplementations[0].InterfaceType, data);
				this.outputFormatter.PrintToken(15);
			}
			this.outputFormatter.PrintIdentifier(eventDeclaration.Name);
			if (!eventDeclaration.Initializer.IsNull)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(3);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(eventDeclaration.Initializer, data);
			}
			if (eventDeclaration.AddRegion.IsNull && eventDeclaration.RemoveRegion.IsNull)
			{
				this.outputFormatter.PrintToken(11);
				this.outputFormatter.NewLine();
			}
			else
			{
				this.outputFormatter.BeginBrace(this.prettyPrintOptions.PropertyBraceStyle);
				this.nodeTracker.TrackedVisit(eventDeclaration.AddRegion, data);
				this.nodeTracker.TrackedVisit(eventDeclaration.RemoveRegion, data);
				this.outputFormatter.EndBrace();
			}
			return null;
		}

		public object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			this.VisitAttributes(eventAddRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintText("add");
			this.OutputBlockAllowInline(eventAddRegion.Block, this.prettyPrintOptions.EventAddBraceStyle);
			return null;
		}

		public object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			this.VisitAttributes(eventRemoveRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintText("remove");
			this.OutputBlockAllowInline(eventRemoveRegion.Block, this.prettyPrintOptions.EventRemoveBraceStyle);
			return null;
		}

		public object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			this.NotSupported(eventRaiseRegion);
			return null;
		}

		public object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			this.VisitAttributes(parameterDeclarationExpression.Attributes, data);
			this.OutputModifier(parameterDeclarationExpression.ParamModifier, parameterDeclarationExpression);
			this.nodeTracker.TrackedVisit(parameterDeclarationExpression.TypeReference, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(parameterDeclarationExpression.ParameterName);
			return null;
		}

		public object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			this.VisitAttributes(methodDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(methodDeclaration.Modifier);
			this.nodeTracker.TrackedVisit(methodDeclaration.TypeReference, data);
			this.outputFormatter.Space();
			if (methodDeclaration.InterfaceImplementations.Count > 0)
			{
				this.nodeTracker.TrackedVisit(methodDeclaration.InterfaceImplementations[0].InterfaceType, data);
				this.outputFormatter.PrintToken(15);
			}
			this.outputFormatter.PrintIdentifier(methodDeclaration.Name);
			this.PrintMethodDeclaration(methodDeclaration);
			return null;
		}

		private void PrintMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			this.PrintTemplates(methodDeclaration.Templates);
			if (this.prettyPrintOptions.BeforeMethodDeclarationParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(methodDeclaration.Parameters);
			this.outputFormatter.PrintToken(21);
			foreach (TemplateDefinition templateDefinition in methodDeclaration.Templates)
			{
				this.nodeTracker.TrackedVisit(templateDefinition, null);
			}
			this.OutputBlock(methodDeclaration.Body, this.prettyPrintOptions.MethodBraceStyle);
		}

		public object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			this.VisitAttributes(operatorDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(operatorDeclaration.Modifier);
			if (operatorDeclaration.IsConversionOperator)
			{
				if (operatorDeclaration.ConversionType == ConversionType.Implicit)
				{
					this.outputFormatter.PrintToken(79);
				}
				else
				{
					this.outputFormatter.PrintToken(69);
				}
			}
			else
			{
				this.nodeTracker.TrackedVisit(operatorDeclaration.TypeReference, data);
			}
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(91);
			this.outputFormatter.Space();
			if (operatorDeclaration.IsConversionOperator)
			{
				this.nodeTracker.TrackedVisit(operatorDeclaration.TypeReference, data);
			}
			else
			{
				switch (operatorDeclaration.OverloadableOperator)
				{
				case OverloadableOperatorType.Add:
					this.outputFormatter.PrintToken(4);
					goto IL_34C;
				case OverloadableOperatorType.Subtract:
					this.outputFormatter.PrintToken(5);
					goto IL_34C;
				case OverloadableOperatorType.Multiply:
					this.outputFormatter.PrintToken(6);
					goto IL_34C;
				case OverloadableOperatorType.Divide:
				case OverloadableOperatorType.DivideInteger:
					this.outputFormatter.PrintToken(7);
					goto IL_34C;
				case OverloadableOperatorType.Modulus:
					this.outputFormatter.PrintToken(8);
					goto IL_34C;
				case OverloadableOperatorType.Concat:
					this.outputFormatter.PrintToken(4);
					goto IL_34C;
				case OverloadableOperatorType.Not:
					this.outputFormatter.PrintToken(24);
					goto IL_34C;
				case OverloadableOperatorType.BitNot:
					this.outputFormatter.PrintToken(27);
					goto IL_34C;
				case OverloadableOperatorType.BitwiseAnd:
					this.outputFormatter.PrintToken(28);
					goto IL_34C;
				case OverloadableOperatorType.BitwiseOr:
					this.outputFormatter.PrintToken(29);
					goto IL_34C;
				case OverloadableOperatorType.ExclusiveOr:
					this.outputFormatter.PrintToken(30);
					goto IL_34C;
				case OverloadableOperatorType.ShiftLeft:
					this.outputFormatter.PrintToken(37);
					goto IL_34C;
				case OverloadableOperatorType.ShiftRight:
					this.outputFormatter.PrintToken(22);
					this.outputFormatter.PrintToken(22);
					goto IL_34C;
				case OverloadableOperatorType.GreaterThan:
					this.outputFormatter.PrintToken(22);
					goto IL_34C;
				case OverloadableOperatorType.GreaterThanOrEqual:
					this.outputFormatter.PrintToken(35);
					goto IL_34C;
				case OverloadableOperatorType.Equality:
					this.outputFormatter.PrintToken(33);
					goto IL_34C;
				case OverloadableOperatorType.InEquality:
					this.outputFormatter.PrintToken(34);
					goto IL_34C;
				case OverloadableOperatorType.LessThan:
					this.outputFormatter.PrintToken(23);
					goto IL_34C;
				case OverloadableOperatorType.LessThanOrEqual:
					this.outputFormatter.PrintToken(36);
					goto IL_34C;
				case OverloadableOperatorType.Increment:
					this.outputFormatter.PrintToken(31);
					goto IL_34C;
				case OverloadableOperatorType.Decrement:
					this.outputFormatter.PrintToken(32);
					goto IL_34C;
				case OverloadableOperatorType.IsTrue:
					this.outputFormatter.PrintToken(112);
					goto IL_34C;
				case OverloadableOperatorType.IsFalse:
					this.outputFormatter.PrintToken(71);
					goto IL_34C;
				case OverloadableOperatorType.Like:
					this.outputFormatter.PrintText("Like");
					goto IL_34C;
				case OverloadableOperatorType.Power:
					this.outputFormatter.PrintText("Power");
					goto IL_34C;
				}
				this.Error(operatorDeclaration, operatorDeclaration.OverloadableOperator.ToString() + " is not supported as overloadable operator");
				IL_34C:;
			}
			this.PrintMethodDeclaration(operatorDeclaration);
			return null;
		}

		public object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			throw new InvalidOperationException();
		}

		public object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			this.VisitAttributes(constructorDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(constructorDeclaration.Modifier);
			if (this.currentType != null)
			{
				this.outputFormatter.PrintIdentifier(this.currentType.Name);
			}
			else
			{
				this.outputFormatter.PrintIdentifier(constructorDeclaration.Name);
			}
			if (this.prettyPrintOptions.BeforeConstructorDeclarationParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(constructorDeclaration.Parameters);
			this.outputFormatter.PrintToken(21);
			this.nodeTracker.TrackedVisit(constructorDeclaration.ConstructorInitializer, data);
			this.OutputBlock(constructorDeclaration.Body, this.prettyPrintOptions.ConstructorBraceStyle);
			return null;
		}

		public object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			object result;
			if (constructorInitializer.IsNull)
			{
				result = null;
			}
			else
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(9);
				this.outputFormatter.Space();
				if (constructorInitializer.ConstructorInitializerType == ConstructorInitializerType.Base)
				{
					this.outputFormatter.PrintToken(50);
				}
				else
				{
					this.outputFormatter.PrintToken(110);
				}
				this.outputFormatter.PrintToken(20);
				this.AppendCommaSeparatedList<Expression>(constructorInitializer.Arguments);
				this.outputFormatter.PrintToken(21);
				result = null;
			}
			return result;
		}

		public object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			this.VisitAttributes(indexerDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(indexerDeclaration.Modifier);
			this.nodeTracker.TrackedVisit(indexerDeclaration.TypeReference, data);
			this.outputFormatter.Space();
			if (indexerDeclaration.InterfaceImplementations.Count > 0)
			{
				this.nodeTracker.TrackedVisit(indexerDeclaration.InterfaceImplementations[0].InterfaceType, data);
				this.outputFormatter.PrintToken(15);
			}
			this.outputFormatter.PrintToken(110);
			this.outputFormatter.PrintToken(18);
			if (this.prettyPrintOptions.SpacesWithinBrackets)
			{
				this.outputFormatter.Space();
			}
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(indexerDeclaration.Parameters);
			if (this.prettyPrintOptions.SpacesWithinBrackets)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(19);
			this.outputFormatter.NewLine();
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(16);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisit(indexerDeclaration.GetRegion, data);
			this.nodeTracker.TrackedVisit(indexerDeclaration.SetRegion, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(17);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			this.VisitAttributes(destructorDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(destructorDeclaration.Modifier);
			this.outputFormatter.PrintToken(27);
			if (this.currentType != null)
			{
				this.outputFormatter.PrintIdentifier(this.currentType.Name);
			}
			else
			{
				this.outputFormatter.PrintIdentifier(destructorDeclaration.Name);
			}
			if (this.prettyPrintOptions.BeforeConstructorDeclarationParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.outputFormatter.PrintToken(21);
			this.OutputBlock(destructorDeclaration.Body, this.prettyPrintOptions.DestructorBraceStyle);
			return null;
		}

		public object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			this.NotSupported(declareDeclaration);
			return null;
		}

		private void OutputBlock(BlockStatement blockStatement, BraceStyle braceStyle)
		{
			this.nodeTracker.BeginNode(blockStatement);
			if (blockStatement.IsNull)
			{
				this.outputFormatter.PrintToken(11);
				this.outputFormatter.NewLine();
			}
			else
			{
				this.outputFormatter.BeginBrace(braceStyle);
				using (List<INode>.Enumerator enumerator = blockStatement.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Statement stmt = (Statement)enumerator.Current;
						this.outputFormatter.Indent();
						if (stmt is BlockStatement)
						{
							this.nodeTracker.TrackedVisit(stmt, BraceStyle.EndOfLine);
						}
						else
						{
							this.nodeTracker.TrackedVisit(stmt, null);
						}
						if (!this.outputFormatter.LastCharacterIsNewLine)
						{
							this.outputFormatter.NewLine();
						}
					}
				}
				this.outputFormatter.EndBrace();
			}
			this.nodeTracker.EndNode(blockStatement);
		}

		private void OutputBlockAllowInline(BlockStatement blockStatement, BraceStyle braceStyle)
		{
			this.OutputBlockAllowInline(blockStatement, braceStyle, true);
		}

		private void OutputBlockAllowInline(BlockStatement blockStatement, BraceStyle braceStyle, bool useNewLine)
		{
			if (!blockStatement.IsNull && (blockStatement.Children.Count == 0 || (blockStatement.Children.Count == 1 && (blockStatement.Children[0] is ExpressionStatement || blockStatement.Children[0] is ReturnStatement))))
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(16);
				this.outputFormatter.Space();
				if (blockStatement.Children.Count != 0)
				{
					bool doIndent = this.outputFormatter.DoIndent;
					bool doNewLine = this.outputFormatter.DoNewLine;
					this.outputFormatter.DoIndent = false;
					this.outputFormatter.DoNewLine = false;
					this.nodeTracker.TrackedVisit(blockStatement.Children[0], null);
					this.outputFormatter.DoIndent = doIndent;
					this.outputFormatter.DoNewLine = doNewLine;
					this.outputFormatter.Space();
				}
				this.outputFormatter.PrintToken(17);
				if (useNewLine)
				{
					this.outputFormatter.NewLine();
				}
			}
			else
			{
				this.OutputBlock(blockStatement, braceStyle);
			}
		}

		public object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			object result;
			if (this.outputFormatter.TextLength == 0)
			{
				using (List<INode>.Enumerator enumerator = blockStatement.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Statement stmt = (Statement)enumerator.Current;
						this.outputFormatter.Indent();
						this.nodeTracker.TrackedVisit(stmt, null);
						if (!this.outputFormatter.LastCharacterIsNewLine)
						{
							this.outputFormatter.NewLine();
						}
					}
				}
				result = null;
			}
			else
			{
				if (data is BraceStyle)
				{
					this.OutputBlock(blockStatement, (BraceStyle)data);
				}
				else
				{
					this.OutputBlock(blockStatement, BraceStyle.NextLine);
				}
				result = null;
			}
			return result;
		}

		public object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			this.nodeTracker.TrackedVisit(addHandlerStatement.EventExpression, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(38);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(addHandlerStatement.HandlerExpression, data);
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			this.nodeTracker.TrackedVisit(removeHandlerStatement.EventExpression, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(39);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(removeHandlerStatement.HandlerExpression, data);
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			this.outputFormatter.PrintToken(78);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(20);
			this.outputFormatter.PrintIdentifier(raiseEventStatement.EventName);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(34);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(89);
			this.outputFormatter.PrintToken(21);
			this.outputFormatter.BeginBrace(BraceStyle.EndOfLine);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintIdentifier(raiseEventStatement.EventName);
			this.outputFormatter.PrintToken(20);
			this.AppendCommaSeparatedList<Expression>(raiseEventStatement.Arguments);
			this.outputFormatter.PrintToken(21);
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.NewLine();
			this.outputFormatter.EndBrace();
			return null;
		}

		public object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			for (int i = 0; i < eraseStatement.Expressions.Count; i++)
			{
				if (i > 0)
				{
					this.outputFormatter.NewLine();
					this.outputFormatter.Indent();
				}
				this.nodeTracker.TrackedVisit(eraseStatement.Expressions[i], data);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(3);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(89);
				this.outputFormatter.PrintToken(11);
			}
			return null;
		}

		public object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			this.NotSupported(errorStatement);
			return null;
		}

		public object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			this.NotSupported(onErrorStatement);
			return null;
		}

		public object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			this.NotSupported(reDimStatement);
			return null;
		}

		public object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			this.nodeTracker.TrackedVisit(expressionStatement.Expression, data);
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			for (int i = 0; i < localVariableDeclaration.Variables.Count; i++)
			{
				VariableDeclaration v = localVariableDeclaration.Variables[i];
				if (i > 0)
				{
					this.outputFormatter.NewLine();
					this.outputFormatter.Indent();
				}
				this.OutputModifier(localVariableDeclaration.Modifier);
				this.nodeTracker.TrackedVisit(localVariableDeclaration.GetTypeForVariable(i) ?? new TypeReference("object"), data);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(v, data);
				this.outputFormatter.PrintToken(11);
			}
			return null;
		}

		public object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			Debug.Assert(yieldStatement != null);
			Debug.Assert(yieldStatement.Statement != null);
			this.outputFormatter.PrintText("yield");
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(yieldStatement.Statement, data);
			return null;
		}

		public object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			this.outputFormatter.PrintToken(100);
			if (!returnStatement.Expression.IsNull)
			{
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(returnStatement.Expression, data);
			}
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			this.outputFormatter.PrintToken(78);
			if (this.prettyPrintOptions.IfParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(ifElseStatement.Condition, data);
			this.outputFormatter.PrintToken(21);
			this.PrintIfSection(ifElseStatement.TrueStatement);
			foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections)
			{
				this.nodeTracker.TrackedVisit(elseIfSection, data);
			}
			if (ifElseStatement.HasElseStatements)
			{
				this.outputFormatter.Indent();
				this.outputFormatter.PrintToken(66);
				this.PrintIfSection(ifElseStatement.FalseStatement);
			}
			return null;
		}

		private void PrintIfSection(List<Statement> statements)
		{
			if (statements.Count != 1 || !(statements[0] is BlockStatement))
			{
				this.outputFormatter.Space();
			}
			if (statements.Count != 1)
			{
				this.outputFormatter.PrintToken(16);
			}
			foreach (Statement stmt in statements)
			{
				this.nodeTracker.TrackedVisit(stmt, null);
			}
			if (statements.Count != 1)
			{
				this.outputFormatter.PrintToken(17);
			}
			if (statements.Count != 1 || !(statements[0] is BlockStatement))
			{
				this.outputFormatter.Space();
			}
		}

		public object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			this.outputFormatter.PrintToken(66);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(78);
			if (this.prettyPrintOptions.IfParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(elseIfSection.Condition, data);
			this.outputFormatter.PrintToken(21);
			this.WriteEmbeddedStatement(elseIfSection.EmbeddedStatement);
			return null;
		}

		public object VisitForStatement(ForStatement forStatement, object data)
		{
			this.outputFormatter.PrintToken(75);
			if (this.prettyPrintOptions.ForParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.outputFormatter.DoIndent = false;
			this.outputFormatter.DoNewLine = false;
			this.outputFormatter.EmitSemicolon = false;
			for (int i = 0; i < forStatement.Initializers.Count; i++)
			{
				INode node = forStatement.Initializers[i];
				this.nodeTracker.TrackedVisit(node, data);
				if (i + 1 < forStatement.Initializers.Count)
				{
					this.outputFormatter.PrintToken(14);
				}
			}
			this.outputFormatter.EmitSemicolon = true;
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.EmitSemicolon = false;
			if (!forStatement.Condition.IsNull)
			{
				if (this.prettyPrintOptions.SpacesAfterSemicolon)
				{
					this.outputFormatter.Space();
				}
				this.nodeTracker.TrackedVisit(forStatement.Condition, data);
			}
			this.outputFormatter.EmitSemicolon = true;
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.EmitSemicolon = false;
			if (forStatement.Iterator != null && forStatement.Iterator.Count > 0)
			{
				if (this.prettyPrintOptions.SpacesAfterSemicolon)
				{
					this.outputFormatter.Space();
				}
				for (int i = 0; i < forStatement.Iterator.Count; i++)
				{
					INode node = forStatement.Iterator[i];
					this.nodeTracker.TrackedVisit(node, data);
					if (i + 1 < forStatement.Iterator.Count)
					{
						this.outputFormatter.PrintToken(14);
					}
				}
			}
			this.outputFormatter.PrintToken(21);
			this.outputFormatter.EmitSemicolon = true;
			this.outputFormatter.DoNewLine = true;
			this.outputFormatter.DoIndent = true;
			this.WriteEmbeddedStatement(forStatement.EmbeddedStatement);
			return null;
		}

		private void WriteEmbeddedStatement(Statement statement)
		{
			if (statement is BlockStatement)
			{
				this.nodeTracker.TrackedVisit(statement, this.prettyPrintOptions.StatementBraceStyle);
			}
			else
			{
				this.outputFormatter.IndentationLevel++;
				this.outputFormatter.NewLine();
				this.nodeTracker.TrackedVisit(statement, null);
				this.outputFormatter.NewLine();
				this.outputFormatter.IndentationLevel--;
			}
		}

		public object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			this.outputFormatter.PrintIdentifier(labelStatement.Label);
			this.outputFormatter.PrintToken(9);
			return null;
		}

		public object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			this.outputFormatter.PrintToken(77);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(gotoStatement.Label);
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			this.outputFormatter.PrintToken(109);
			if (this.prettyPrintOptions.SwitchParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(switchStatement.SwitchExpression, data);
			this.outputFormatter.PrintToken(21);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(16);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			foreach (SwitchSection section in switchStatement.SwitchSections)
			{
				this.nodeTracker.TrackedVisit(section, data);
			}
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(17);
			return null;
		}

		public object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			foreach (CaseLabel label in switchSection.SwitchLabels)
			{
				this.nodeTracker.TrackedVisit(label, data);
			}
			this.outputFormatter.IndentationLevel++;
			using (List<INode>.Enumerator enumerator2 = switchSection.Children.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Statement stmt = (Statement)enumerator2.Current;
					this.outputFormatter.Indent();
					this.nodeTracker.TrackedVisit(stmt, data);
					this.outputFormatter.NewLine();
				}
			}
			if (switchSection.Children.Count == 0 || (!(switchSection.Children[switchSection.Children.Count - 1] is BreakStatement) && !(switchSection.Children[switchSection.Children.Count - 1] is ContinueStatement) && !(switchSection.Children[switchSection.Children.Count - 1] is ReturnStatement)))
			{
				this.outputFormatter.Indent();
				this.outputFormatter.PrintToken(52);
				this.outputFormatter.PrintToken(11);
				this.outputFormatter.NewLine();
			}
			this.outputFormatter.IndentationLevel--;
			return null;
		}

		public object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			this.outputFormatter.Indent();
			if (caseLabel.IsDefault)
			{
				this.outputFormatter.PrintToken(62);
			}
			else
			{
				this.outputFormatter.PrintToken(54);
				this.outputFormatter.Space();
				if (caseLabel.BinaryOperatorType != BinaryOperatorType.None)
				{
					this.Error(caseLabel, string.Format("Case labels with binary operators are unsupported : {0}", caseLabel.BinaryOperatorType));
				}
				this.nodeTracker.TrackedVisit(caseLabel.Label, data);
			}
			this.outputFormatter.PrintToken(9);
			if (!caseLabel.ToExpression.IsNull)
			{
				PrimitiveExpression pl = caseLabel.Label as PrimitiveExpression;
				PrimitiveExpression pt = caseLabel.ToExpression as PrimitiveExpression;
				if (pl != null && pt != null && pl.Value is int && pt.Value is int)
				{
					int plv = (int)pl.Value;
					int prv = (int)pt.Value;
					if (plv < prv && plv + 12 > prv)
					{
						for (int i = plv + 1; i <= prv; i++)
						{
							this.outputFormatter.NewLine();
							this.outputFormatter.Indent();
							this.outputFormatter.PrintToken(54);
							this.outputFormatter.Space();
							this.outputFormatter.PrintText(i.ToString(NumberFormatInfo.InvariantInfo));
							this.outputFormatter.PrintToken(9);
						}
					}
					else
					{
						this.outputFormatter.PrintText(" // TODO: to ");
						this.nodeTracker.TrackedVisit(caseLabel.ToExpression, data);
					}
				}
				else
				{
					this.outputFormatter.PrintText(" // TODO: to ");
					this.nodeTracker.TrackedVisit(caseLabel.ToExpression, data);
				}
			}
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			this.outputFormatter.PrintToken(52);
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitStopStatement(StopStatement stopStatement, object data)
		{
			this.outputFormatter.PrintText("System.Diagnostics.Debugger.Break()");
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			this.NotSupported(resumeStatement);
			return null;
		}

		public object VisitEndStatement(EndStatement endStatement, object data)
		{
			this.outputFormatter.PrintText("System.Environment.Exit(0)");
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			this.outputFormatter.PrintToken(60);
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			this.outputFormatter.PrintToken(77);
			this.outputFormatter.Space();
			if (gotoCaseStatement.IsDefaultCase)
			{
				this.outputFormatter.PrintToken(62);
			}
			else
			{
				this.outputFormatter.PrintToken(54);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(gotoCaseStatement.Expression, data);
			}
			this.outputFormatter.PrintToken(11);
			return null;
		}

		private void PrintLoopCheck(DoLoopStatement doLoopStatement)
		{
			this.outputFormatter.PrintToken(124);
			if (this.prettyPrintOptions.WhileParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			if (doLoopStatement.ConditionType == ConditionType.Until)
			{
				this.outputFormatter.PrintToken(24);
				this.outputFormatter.PrintToken(20);
			}
			if (doLoopStatement.Condition.IsNull)
			{
				this.outputFormatter.PrintToken(112);
			}
			else
			{
				this.nodeTracker.TrackedVisit(doLoopStatement.Condition, null);
			}
			if (doLoopStatement.ConditionType == ConditionType.Until)
			{
				this.outputFormatter.PrintToken(21);
			}
			this.outputFormatter.PrintToken(21);
		}

		public object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			if (doLoopStatement.ConditionPosition == ConditionPosition.None)
			{
				this.Error(doLoopStatement, string.Format("Unknown condition position for loop : {0}.", doLoopStatement));
			}
			if (doLoopStatement.ConditionPosition == ConditionPosition.Start)
			{
				this.PrintLoopCheck(doLoopStatement);
			}
			else
			{
				this.outputFormatter.PrintToken(64);
			}
			this.WriteEmbeddedStatement(doLoopStatement.EmbeddedStatement);
			if (doLoopStatement.ConditionPosition == ConditionPosition.End)
			{
				this.outputFormatter.Indent();
				this.PrintLoopCheck(doLoopStatement);
				this.outputFormatter.PrintToken(11);
				this.outputFormatter.NewLine();
			}
			return null;
		}

		public object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			this.outputFormatter.PrintToken(76);
			if (this.prettyPrintOptions.ForeachParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(foreachStatement.TypeReference, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(foreachStatement.VariableName);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(80);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(foreachStatement.Expression, data);
			this.outputFormatter.PrintToken(21);
			this.WriteEmbeddedStatement(foreachStatement.EmbeddedStatement);
			return null;
		}

		public object VisitLockStatement(LockStatement lockStatement, object data)
		{
			this.outputFormatter.PrintToken(85);
			if (this.prettyPrintOptions.LockParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(lockStatement.LockExpression, data);
			this.outputFormatter.PrintToken(21);
			this.WriteEmbeddedStatement(lockStatement.EmbeddedStatement);
			return null;
		}

		public object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			this.outputFormatter.PrintToken(120);
			if (this.prettyPrintOptions.UsingParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.outputFormatter.DoIndent = false;
			this.outputFormatter.DoNewLine = false;
			this.outputFormatter.EmitSemicolon = false;
			this.nodeTracker.TrackedVisit(usingStatement.ResourceAcquisition, data);
			this.outputFormatter.DoIndent = true;
			this.outputFormatter.DoNewLine = true;
			this.outputFormatter.EmitSemicolon = true;
			this.outputFormatter.PrintToken(21);
			this.WriteEmbeddedStatement(usingStatement.EmbeddedStatement);
			return null;
		}

		public object VisitWithStatement(WithStatement withStatement, object data)
		{
			this.NotSupported(withStatement);
			return null;
		}

		public object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			this.outputFormatter.PrintToken(113);
			this.WriteEmbeddedStatement(tryCatchStatement.StatementBlock);
			foreach (CatchClause catchClause in tryCatchStatement.CatchClauses)
			{
				this.nodeTracker.TrackedVisit(catchClause, data);
			}
			if (!tryCatchStatement.FinallyBlock.IsNull)
			{
				this.outputFormatter.Indent();
				this.outputFormatter.PrintToken(72);
				this.WriteEmbeddedStatement(tryCatchStatement.FinallyBlock);
			}
			return null;
		}

		public object VisitCatchClause(CatchClause catchClause, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(55);
			if (!catchClause.TypeReference.IsNull)
			{
				if (this.prettyPrintOptions.CatchParentheses)
				{
					this.outputFormatter.Space();
				}
				this.outputFormatter.PrintToken(20);
				this.outputFormatter.PrintIdentifier(catchClause.TypeReference.Type);
				if (catchClause.VariableName.Length > 0)
				{
					this.outputFormatter.Space();
					this.outputFormatter.PrintIdentifier(catchClause.VariableName);
				}
				this.outputFormatter.PrintToken(21);
			}
			this.WriteEmbeddedStatement(catchClause.StatementBlock);
			return null;
		}

		public object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			this.outputFormatter.PrintToken(111);
			if (!throwStatement.Expression.IsNull)
			{
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(throwStatement.Expression, data);
			}
			this.outputFormatter.PrintToken(11);
			return null;
		}

		public object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			this.outputFormatter.PrintToken(73);
			if (this.prettyPrintOptions.FixedParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(fixedStatement.TypeReference, data);
			this.outputFormatter.Space();
			this.AppendCommaSeparatedList<VariableDeclaration>(fixedStatement.PointerDeclarators);
			this.outputFormatter.PrintToken(21);
			this.WriteEmbeddedStatement(fixedStatement.EmbeddedStatement);
			return null;
		}

		public object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			this.outputFormatter.PrintToken(118);
			this.WriteEmbeddedStatement(unsafeStatement.Block);
			return null;
		}

		public object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			this.outputFormatter.PrintToken(57);
			this.WriteEmbeddedStatement(checkedStatement.Block);
			return null;
		}

		public object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			this.outputFormatter.PrintToken(117);
			this.WriteEmbeddedStatement(uncheckedStatement.Block);
			return null;
		}

		public object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			if (exitStatement.ExitType == ExitType.Function || exitStatement.ExitType == ExitType.Sub || exitStatement.ExitType == ExitType.Property)
			{
				this.outputFormatter.PrintToken(100);
			}
			else
			{
				this.outputFormatter.PrintToken(52);
			}
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.PrintText(" // TODO: might not be correct. Was : Exit " + exitStatement.ExitType);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			this.outputFormatter.PrintToken(75);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(20);
			if (!forNextStatement.TypeReference.IsNull)
			{
				this.nodeTracker.TrackedVisit(forNextStatement.TypeReference, data);
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintIdentifier(forNextStatement.VariableName);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(3);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(forNextStatement.Start, data);
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(forNextStatement.VariableName);
			this.outputFormatter.Space();
			PrimitiveExpression pe = forNextStatement.Step as PrimitiveExpression;
			if ((pe == null || !(pe.Value is int) || (int)pe.Value >= 0) && !(forNextStatement.Step is UnaryOperatorExpression))
			{
				this.outputFormatter.PrintToken(36);
			}
			else
			{
				this.outputFormatter.PrintToken(35);
			}
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(forNextStatement.End, data);
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(forNextStatement.VariableName);
			if (forNextStatement.Step.IsNull)
			{
				this.outputFormatter.PrintToken(31);
			}
			else
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(38);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(forNextStatement.Step, data);
			}
			this.outputFormatter.PrintToken(21);
			this.WriteEmbeddedStatement(forNextStatement.EmbeddedStatement);
			return null;
		}

		public object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			this.NotSupported(classReferenceExpression);
			return null;
		}

		private static string ConvertCharLiteral(char ch)
		{
			string result;
			if (ch == '\'')
			{
				result = "\\'";
			}
			else
			{
				result = CSharpOutputVisitor.ConvertChar(ch);
			}
			return result;
		}

		private static string ConvertChar(char ch)
		{
			char c = ch;
			string result;
			switch (c)
			{
			case '\0':
				result = "\\0";
				return result;
			case '\u0001':
			case '\u0002':
			case '\u0003':
			case '\u0004':
			case '\u0005':
			case '\u0006':
				break;
			case '\a':
				result = "\\a";
				return result;
			case '\b':
				result = "\\b";
				return result;
			case '\t':
				result = "\\t";
				return result;
			case '\n':
				result = "\\n";
				return result;
			case '\v':
				result = "\\v";
				return result;
			case '\f':
				result = "\\f";
				return result;
			case '\r':
				result = "\\r";
				return result;
			default:
				if (c == '\\')
				{
					result = "\\\\";
					return result;
				}
				break;
			}
			if (char.IsControl(ch))
			{
				result = "\\u" + (int)ch;
			}
			else
			{
				result = ch.ToString();
			}
			return result;
		}

		private static string ConvertString(string str)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				char ch = str[i];
				if (ch == '"')
				{
					sb.Append("\\\"");
				}
				else
				{
					sb.Append(CSharpOutputVisitor.ConvertChar(ch));
				}
			}
			return sb.ToString();
		}

		public object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			object result;
			if (primitiveExpression.Value == null)
			{
				this.outputFormatter.PrintToken(89);
				result = null;
			}
			else
			{
				object val = primitiveExpression.Value;
				if (val is bool)
				{
					if ((bool)val)
					{
						this.outputFormatter.PrintToken(112);
					}
					else
					{
						this.outputFormatter.PrintToken(71);
					}
					result = null;
				}
				else if (val is string)
				{
					this.outputFormatter.PrintText('"' + CSharpOutputVisitor.ConvertString(val.ToString()) + '"');
					result = null;
				}
				else if (val is char)
				{
					this.outputFormatter.PrintText("'" + CSharpOutputVisitor.ConvertCharLiteral((char)val) + "'");
					result = null;
				}
				else if (val is decimal)
				{
					this.outputFormatter.PrintText(((decimal)val).ToString(NumberFormatInfo.InvariantInfo) + "m");
					result = null;
				}
				else if (val is float)
				{
					this.outputFormatter.PrintText(((float)val).ToString(NumberFormatInfo.InvariantInfo) + "f");
					result = null;
				}
				else if (val is double)
				{
					string text = ((double)val).ToString(NumberFormatInfo.InvariantInfo);
					if (text.IndexOf('.') < 0 && text.IndexOf('E') < 0)
					{
						this.outputFormatter.PrintText(text + ".0");
					}
					else
					{
						this.outputFormatter.PrintText(text);
					}
					result = null;
				}
				else
				{
					if (val is IFormattable)
					{
						this.outputFormatter.PrintText(((IFormattable)val).ToString(null, NumberFormatInfo.InvariantInfo));
						if (val is uint || val is ulong)
						{
							this.outputFormatter.PrintText("u");
						}
						if (val is long || val is ulong)
						{
							this.outputFormatter.PrintText("l");
						}
					}
					else
					{
						this.outputFormatter.PrintText(val.ToString());
					}
					result = null;
				}
			}
			return result;
		}

		private static bool IsNullLiteralExpression(Expression expr)
		{
			PrimitiveExpression pe = expr as PrimitiveExpression;
			return pe != null && pe.Value == null;
		}

		public object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			BinaryOperatorType op = binaryOperatorExpression.Op;
			object result;
			if (op != BinaryOperatorType.Power)
			{
				switch (op)
				{
				case BinaryOperatorType.ReferenceEquality:
				case BinaryOperatorType.ReferenceInequality:
					if (!CSharpOutputVisitor.IsNullLiteralExpression(binaryOperatorExpression.Left) && !CSharpOutputVisitor.IsNullLiteralExpression(binaryOperatorExpression.Right))
					{
						if (binaryOperatorExpression.Op == BinaryOperatorType.ReferenceInequality)
						{
							this.outputFormatter.PrintToken(24);
						}
						this.outputFormatter.PrintText("object.ReferenceEquals");
						if (this.prettyPrintOptions.BeforeMethodCallParentheses)
						{
							this.outputFormatter.Space();
						}
						this.outputFormatter.PrintToken(20);
						this.nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
						this.PrintFormattedComma();
						this.nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
						this.outputFormatter.PrintToken(21);
						result = null;
						return result;
					}
					break;
				}
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
				switch (binaryOperatorExpression.Op)
				{
				case BinaryOperatorType.BitwiseAnd:
					if (this.prettyPrintOptions.AroundBitwiseOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(28);
					if (this.prettyPrintOptions.AroundBitwiseOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.BitwiseOr:
					if (this.prettyPrintOptions.AroundBitwiseOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(29);
					if (this.prettyPrintOptions.AroundBitwiseOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.LogicalAnd:
					if (this.prettyPrintOptions.AroundLogicalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(25);
					if (this.prettyPrintOptions.AroundLogicalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.LogicalOr:
					if (this.prettyPrintOptions.AroundLogicalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(26);
					if (this.prettyPrintOptions.AroundLogicalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.ExclusiveOr:
					if (this.prettyPrintOptions.AroundBitwiseOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(30);
					if (this.prettyPrintOptions.AroundBitwiseOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.GreaterThan:
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(22);
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.GreaterThanOrEqual:
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(35);
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.Equality:
				case BinaryOperatorType.ReferenceEquality:
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(33);
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.InEquality:
				case BinaryOperatorType.ReferenceInequality:
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(34);
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.LessThan:
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(23);
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.LessThanOrEqual:
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(36);
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.Add:
				case BinaryOperatorType.Concat:
					if (this.prettyPrintOptions.AroundAdditiveOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(4);
					if (this.prettyPrintOptions.AroundAdditiveOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.Subtract:
					if (this.prettyPrintOptions.AroundAdditiveOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(5);
					if (this.prettyPrintOptions.AroundAdditiveOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.Multiply:
					if (this.prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(6);
					if (this.prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.Divide:
				case BinaryOperatorType.DivideInteger:
					if (this.prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(7);
					if (this.prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.Modulus:
					if (this.prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(8);
					if (this.prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.ShiftLeft:
					if (this.prettyPrintOptions.AroundShiftOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(37);
					if (this.prettyPrintOptions.AroundShiftOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.ShiftRight:
					if (this.prettyPrintOptions.AroundShiftOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(22);
					this.outputFormatter.PrintToken(22);
					if (this.prettyPrintOptions.AroundShiftOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				case BinaryOperatorType.NullCoalescing:
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					this.outputFormatter.PrintToken(13);
					if (this.prettyPrintOptions.AroundRelationalOperatorParentheses)
					{
						this.outputFormatter.Space();
					}
					goto IL_847;
				}
				this.Error(binaryOperatorExpression, string.Format("Unknown binary operator {0}", binaryOperatorExpression.Op));
				result = null;
				return result;
				IL_847:
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
				result = null;
			}
			else
			{
				this.outputFormatter.PrintText("Math.Pow");
				if (this.prettyPrintOptions.BeforeMethodCallParentheses)
				{
					this.outputFormatter.Space();
				}
				this.outputFormatter.PrintToken(20);
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
				this.PrintFormattedComma();
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
				this.outputFormatter.PrintToken(21);
				result = null;
			}
			return result;
		}

		public object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(parenthesizedExpression.Expression, data);
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			this.nodeTracker.TrackedVisit(invocationExpression.TargetObject, data);
			if (invocationExpression.TypeArguments != null && invocationExpression.TypeArguments.Count > 0)
			{
				this.outputFormatter.PrintToken(23);
				this.AppendCommaSeparatedList<TypeReference>(invocationExpression.TypeArguments);
				this.outputFormatter.PrintToken(22);
			}
			if (this.prettyPrintOptions.BeforeMethodCallParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.AppendCommaSeparatedList<Expression>(invocationExpression.Arguments);
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			this.outputFormatter.PrintIdentifier(identifierExpression.Identifier);
			return null;
		}

		public object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			this.nodeTracker.TrackedVisit(typeReferenceExpression.TypeReference, data);
			return null;
		}

		public object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			object result;
			switch (unaryOperatorExpression.Op)
			{
			case UnaryOperatorType.Not:
				this.outputFormatter.PrintToken(24);
				break;
			case UnaryOperatorType.BitNot:
				this.outputFormatter.PrintToken(27);
				break;
			case UnaryOperatorType.Minus:
				this.outputFormatter.PrintToken(5);
				break;
			case UnaryOperatorType.Plus:
				this.outputFormatter.PrintToken(4);
				break;
			case UnaryOperatorType.Increment:
				this.outputFormatter.PrintToken(31);
				break;
			case UnaryOperatorType.Decrement:
				this.outputFormatter.PrintToken(32);
				break;
			case UnaryOperatorType.PostIncrement:
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				this.outputFormatter.PrintToken(31);
				result = null;
				return result;
			case UnaryOperatorType.PostDecrement:
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				this.outputFormatter.PrintToken(32);
				result = null;
				return result;
			case UnaryOperatorType.Star:
				this.outputFormatter.PrintToken(6);
				break;
			case UnaryOperatorType.BitWiseAnd:
				this.outputFormatter.PrintToken(28);
				break;
			default:
				this.Error(unaryOperatorExpression, string.Format("Unknown unary operator {0}", unaryOperatorExpression.Op));
				result = null;
				return result;
			}
			this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
			result = null;
			return result;
		}

		public object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			this.nodeTracker.TrackedVisit(assignmentExpression.Left, data);
			if (this.prettyPrintOptions.AroundAssignmentParentheses)
			{
				this.outputFormatter.Space();
			}
			object result;
			switch (assignmentExpression.Op)
			{
			case AssignmentOperatorType.Assign:
				this.outputFormatter.PrintToken(3);
				goto IL_1C5;
			case AssignmentOperatorType.Add:
				this.outputFormatter.PrintToken(38);
				goto IL_1C5;
			case AssignmentOperatorType.Subtract:
				this.outputFormatter.PrintToken(39);
				goto IL_1C5;
			case AssignmentOperatorType.Multiply:
				this.outputFormatter.PrintToken(40);
				goto IL_1C5;
			case AssignmentOperatorType.Divide:
			case AssignmentOperatorType.DivideInteger:
				this.outputFormatter.PrintToken(41);
				goto IL_1C5;
			case AssignmentOperatorType.Modulus:
				this.outputFormatter.PrintToken(42);
				goto IL_1C5;
			case AssignmentOperatorType.Power:
				this.outputFormatter.PrintToken(3);
				if (this.prettyPrintOptions.AroundAssignmentParentheses)
				{
					this.outputFormatter.Space();
				}
				this.VisitBinaryOperatorExpression(new BinaryOperatorExpression(assignmentExpression.Left, BinaryOperatorType.Power, assignmentExpression.Right), data);
				result = null;
				return result;
			case AssignmentOperatorType.ShiftLeft:
				this.outputFormatter.PrintToken(46);
				goto IL_1C5;
			case AssignmentOperatorType.ShiftRight:
				this.outputFormatter.PrintToken(22);
				this.outputFormatter.PrintToken(35);
				goto IL_1C5;
			case AssignmentOperatorType.BitwiseAnd:
				this.outputFormatter.PrintToken(43);
				goto IL_1C5;
			case AssignmentOperatorType.BitwiseOr:
				this.outputFormatter.PrintToken(44);
				goto IL_1C5;
			case AssignmentOperatorType.ExclusiveOr:
				this.outputFormatter.PrintToken(45);
				goto IL_1C5;
			}
			this.Error(assignmentExpression, string.Format("Unknown assignment operator {0}", assignmentExpression.Op));
			result = null;
			return result;
			IL_1C5:
			if (this.prettyPrintOptions.AroundAssignmentParentheses)
			{
				this.outputFormatter.Space();
			}
			this.nodeTracker.TrackedVisit(assignmentExpression.Right, data);
			result = null;
			return result;
		}

		public object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			this.outputFormatter.PrintToken(104);
			if (this.prettyPrintOptions.SizeOfParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(sizeOfExpression.TypeReference, data);
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			this.outputFormatter.PrintToken(114);
			if (this.prettyPrintOptions.TypeOfParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(typeOfExpression.TypeReference, data);
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			this.outputFormatter.PrintToken(62);
			if (this.prettyPrintOptions.TypeOfParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(defaultValueExpression.TypeReference, data);
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			this.nodeTracker.TrackedVisit(typeOfIsExpression.Expression, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(84);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(typeOfIsExpression.TypeReference, data);
			return null;
		}

		public object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			return this.nodeTracker.TrackedVisit(addressOfExpression.Expression, data);
		}

		public object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			this.outputFormatter.PrintToken(63);
			if (anonymousMethodExpression.Parameters.Count > 0 || anonymousMethodExpression.HasParameterList)
			{
				this.outputFormatter.PrintToken(20);
				this.AppendCommaSeparatedList<ParameterDeclarationExpression>(anonymousMethodExpression.Parameters);
				this.outputFormatter.PrintToken(21);
			}
			this.OutputBlockAllowInline(anonymousMethodExpression.Body, this.prettyPrintOptions.MethodBraceStyle, false);
			return null;
		}

		public object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			this.outputFormatter.PrintToken(57);
			if (this.prettyPrintOptions.CheckedParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(checkedExpression.Expression, data);
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			this.outputFormatter.PrintToken(117);
			if (this.prettyPrintOptions.UncheckedParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.nodeTracker.TrackedVisit(uncheckedExpression.Expression, data);
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			this.nodeTracker.TrackedVisit(pointerReferenceExpression.TargetObject, data);
			this.outputFormatter.PrintToken(47);
			this.outputFormatter.PrintIdentifier(pointerReferenceExpression.Identifier);
			return null;
		}

		public object VisitCastExpression(CastExpression castExpression, object data)
		{
			if (castExpression.CastType == CastType.TryCast)
			{
				this.nodeTracker.TrackedVisit(castExpression.Expression, data);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(49);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(castExpression.CastTo, data);
			}
			else
			{
				this.outputFormatter.PrintToken(20);
				this.nodeTracker.TrackedVisit(castExpression.CastTo, data);
				this.outputFormatter.PrintToken(21);
				if (this.prettyPrintOptions.SpacesAfterTypecast)
				{
					this.outputFormatter.Space();
				}
				this.nodeTracker.TrackedVisit(castExpression.Expression, data);
			}
			return null;
		}

		public object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			this.outputFormatter.PrintToken(105);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(stackAllocExpression.TypeReference, data);
			this.outputFormatter.PrintToken(18);
			if (this.prettyPrintOptions.SpacesWithinBrackets)
			{
				this.outputFormatter.Space();
			}
			this.nodeTracker.TrackedVisit(stackAllocExpression.Expression, data);
			if (this.prettyPrintOptions.SpacesWithinBrackets)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(19);
			return null;
		}

		public object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			this.nodeTracker.TrackedVisit(indexerExpression.TargetObject, data);
			this.outputFormatter.PrintToken(18);
			if (this.prettyPrintOptions.SpacesWithinBrackets)
			{
				this.outputFormatter.Space();
			}
			this.AppendCommaSeparatedList<Expression>(indexerExpression.Indexes);
			if (this.prettyPrintOptions.SpacesWithinBrackets)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(19);
			return null;
		}

		public object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			this.outputFormatter.PrintToken(110);
			return null;
		}

		public object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			this.outputFormatter.PrintToken(50);
			return null;
		}

		public object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(objectCreateExpression.CreateType, data);
			if (this.prettyPrintOptions.NewParentheses)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(20);
			this.AppendCommaSeparatedList<Expression>(objectCreateExpression.Parameters);
			this.outputFormatter.PrintToken(21);
			return null;
		}

		public object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.PrintTypeReferenceWithoutArray(arrayCreateExpression.CreateType);
			if (arrayCreateExpression.Arguments.Count > 0)
			{
				this.outputFormatter.PrintToken(18);
				if (this.prettyPrintOptions.SpacesWithinBrackets)
				{
					this.outputFormatter.Space();
				}
				for (int i = 0; i < arrayCreateExpression.Arguments.Count; i++)
				{
					if (i > 0)
					{
						this.PrintFormattedComma();
					}
					this.nodeTracker.TrackedVisit(arrayCreateExpression.Arguments[i], data);
				}
				if (this.prettyPrintOptions.SpacesWithinBrackets)
				{
					this.outputFormatter.Space();
				}
				this.outputFormatter.PrintToken(19);
				this.PrintArrayRank(arrayCreateExpression.CreateType.RankSpecifier, 1);
			}
			else
			{
				this.PrintArrayRank(arrayCreateExpression.CreateType.RankSpecifier, 0);
			}
			if (!arrayCreateExpression.ArrayInitializer.IsNull)
			{
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(arrayCreateExpression.ArrayInitializer, data);
			}
			return null;
		}

		public object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			Expression target = fieldReferenceExpression.TargetObject;
			if (target is BinaryOperatorExpression || target is CastExpression)
			{
				this.outputFormatter.PrintToken(20);
			}
			this.nodeTracker.TrackedVisit(target, data);
			if (target is BinaryOperatorExpression || target is CastExpression)
			{
				this.outputFormatter.PrintToken(21);
			}
			this.outputFormatter.PrintToken(15);
			this.outputFormatter.PrintIdentifier(fieldReferenceExpression.FieldName);
			return null;
		}

		public object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			switch (directionExpression.FieldDirection)
			{
			case FieldDirection.Out:
				this.outputFormatter.PrintToken(92);
				this.outputFormatter.Space();
				break;
			case FieldDirection.Ref:
				this.outputFormatter.PrintToken(99);
				this.outputFormatter.Space();
				break;
			}
			this.nodeTracker.TrackedVisit(directionExpression.Expression, data);
			return null;
		}

		public object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			this.outputFormatter.PrintToken(16);
			this.AppendCommaSeparatedList<Expression>(arrayInitializerExpression.CreateExpressions);
			this.outputFormatter.PrintToken(17);
			return null;
		}

		public object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			this.nodeTracker.TrackedVisit(conditionalExpression.Condition, data);
			if (this.prettyPrintOptions.ConditionalOperatorBeforeConditionSpace)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(12);
			if (this.prettyPrintOptions.ConditionalOperatorAfterConditionSpace)
			{
				this.outputFormatter.Space();
			}
			this.nodeTracker.TrackedVisit(conditionalExpression.TrueExpression, data);
			if (this.prettyPrintOptions.ConditionalOperatorBeforeSeparatorSpace)
			{
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(9);
			if (this.prettyPrintOptions.ConditionalOperatorAfterSeparatorSpace)
			{
				this.outputFormatter.Space();
			}
			this.nodeTracker.TrackedVisit(conditionalExpression.FalseExpression, data);
			return null;
		}

		private void OutputModifier(ParameterModifiers modifier, INode node)
		{
			switch (modifier)
			{
			case ParameterModifiers.None:
			case ParameterModifiers.In:
				return;
			case ParameterModifiers.Out:
				this.outputFormatter.PrintToken(92);
				this.outputFormatter.Space();
				return;
			case ParameterModifiers.In | ParameterModifiers.Out:
			case ParameterModifiers.In | ParameterModifiers.Ref:
			case ParameterModifiers.Out | ParameterModifiers.Ref:
			case ParameterModifiers.In | ParameterModifiers.Out | ParameterModifiers.Ref:
				break;
			case ParameterModifiers.Ref:
				this.outputFormatter.PrintToken(99);
				this.outputFormatter.Space();
				return;
			case ParameterModifiers.Params:
				this.outputFormatter.PrintToken(94);
				this.outputFormatter.Space();
				return;
			default:
				if (modifier == ParameterModifiers.Optional)
				{
					this.Error(node, string.Format("Optional parameters aren't supported in C#", new object[0]));
					return;
				}
				break;
			}
			this.Error(node, string.Format("Unsupported modifier : {0}", modifier));
		}

		private void OutputModifier(Modifiers modifier)
		{
			ArrayList tokenList = new ArrayList();
			if ((modifier & Modifiers.Unsafe) != Modifiers.None)
			{
				tokenList.Add(118);
			}
			if ((modifier & Modifiers.Public) != Modifiers.None)
			{
				tokenList.Add(97);
			}
			if ((modifier & Modifiers.Private) != Modifiers.None)
			{
				tokenList.Add(95);
			}
			if ((modifier & Modifiers.Protected) != Modifiers.None)
			{
				tokenList.Add(96);
			}
			if ((modifier & Modifiers.Static) != Modifiers.None)
			{
				tokenList.Add(106);
			}
			if ((modifier & Modifiers.Internal) != Modifiers.None)
			{
				tokenList.Add(83);
			}
			if ((modifier & Modifiers.Override) != Modifiers.None)
			{
				tokenList.Add(93);
			}
			if ((modifier & Modifiers.Dim) != Modifiers.None)
			{
				tokenList.Add(48);
			}
			if ((modifier & Modifiers.Virtual) != Modifiers.None)
			{
				tokenList.Add(121);
			}
			if ((modifier & Modifiers.New) != Modifiers.None)
			{
				tokenList.Add(88);
			}
			if ((modifier & Modifiers.Sealed) != Modifiers.None)
			{
				tokenList.Add(102);
			}
			if ((modifier & Modifiers.Extern) != Modifiers.None)
			{
				tokenList.Add(70);
			}
			if ((modifier & Modifiers.Const) != Modifiers.None)
			{
				tokenList.Add(59);
			}
			if ((modifier & Modifiers.ReadOnly) != Modifiers.None)
			{
				tokenList.Add(98);
			}
			if ((modifier & Modifiers.Volatile) != Modifiers.None)
			{
				tokenList.Add(123);
			}
			if ((modifier & Modifiers.Fixed) != Modifiers.None)
			{
				tokenList.Add(73);
			}
			this.outputFormatter.PrintTokenList(tokenList);
			if ((modifier & Modifiers.Partial) != Modifiers.None)
			{
				this.outputFormatter.PrintText("partial ");
			}
		}

		public void AppendCommaSeparatedList<T>(ICollection<T> list) where T : class, INode
		{
			if (list != null)
			{
				int i = 0;
				foreach (T node in list)
				{
					this.nodeTracker.TrackedVisit(node, null);
					if (i + 1 < list.Count)
					{
						this.PrintFormattedComma();
					}
					if ((i + 1) % 10 == 0)
					{
						this.outputFormatter.NewLine();
						this.outputFormatter.Indent();
					}
					i++;
				}
			}
		}
	}
}

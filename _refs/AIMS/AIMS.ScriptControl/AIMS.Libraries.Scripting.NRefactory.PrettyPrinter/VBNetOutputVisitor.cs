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
	public class VBNetOutputVisitor : IOutputAstVisitor, IAstVisitor
	{
		private Errors errors = new Errors();

		private VBNetOutputFormatter outputFormatter;

		private VBNetPrettyPrintOptions prettyPrintOptions = new VBNetPrettyPrintOptions();

		private NodeTracker nodeTracker;

		private TypeDeclaration currentType;

		private bool printFullSystemType;

		private Stack<int> exitTokenStack = new Stack<int>();

		private TypeReference currentVariableType;

		private TypeReference currentEventType = null;

		private bool isUsingResourceAcquisition;

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

		public VBNetPrettyPrintOptions Options
		{
			get
			{
				return this.prettyPrintOptions;
			}
		}

		public NodeTracker NodeTracker
		{
			get
			{
				return this.nodeTracker;
			}
		}

		public IOutputFormatter OutputFormatter
		{
			get
			{
				return this.outputFormatter;
			}
		}

		public VBNetOutputVisitor()
		{
			this.outputFormatter = new VBNetOutputFormatter(this.prettyPrintOptions);
			this.nodeTracker = new NodeTracker(this);
		}

		private void Error(string text, Location position)
		{
			this.errors.Error(position.Y, position.X, text);
		}

		private void UnsupportedNode(INode node)
		{
			this.Error(node.GetType().Name + " is unsupported", node.StartLocation);
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
			TypeReference.PrimitiveTypesVBReverse.TryGetValue(typeString, out primitiveType);
			return primitiveType;
		}

		public object VisitTypeReference(TypeReference typeReference, object data)
		{
			if (typeReference == TypeReference.ClassConstraint)
			{
				this.outputFormatter.PrintToken(67);
			}
			else if (typeReference == TypeReference.StructConstraint)
			{
				this.outputFormatter.PrintToken(166);
			}
			else if (typeReference == TypeReference.NewConstraint)
			{
				this.outputFormatter.PrintToken(127);
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

		private void PrintTypeReferenceWithoutArray(TypeReference typeReference)
		{
			if (typeReference.IsGlobal)
			{
				this.outputFormatter.PrintToken(198);
				this.outputFormatter.PrintToken(10);
			}
			if (typeReference.Type == null || typeReference.Type.Length == 0)
			{
				this.outputFormatter.PrintText("Void");
			}
			else if (this.printFullSystemType || typeReference.IsGlobal)
			{
				this.outputFormatter.PrintIdentifier(typeReference.SystemType);
			}
			else
			{
				string shortTypeName = VBNetOutputVisitor.ConvertTypeString(typeReference.SystemType);
				if (shortTypeName != null)
				{
					this.outputFormatter.PrintText(shortTypeName);
				}
				else
				{
					this.outputFormatter.PrintIdentifier(typeReference.Type);
				}
			}
			if (typeReference.GenericTypes != null && typeReference.GenericTypes.Count > 0)
			{
				this.outputFormatter.PrintToken(24);
				this.outputFormatter.PrintToken(200);
				this.outputFormatter.Space();
				this.AppendCommaSeparatedList<TypeReference>(typeReference.GenericTypes);
				this.outputFormatter.PrintToken(25);
			}
			for (int i = 0; i < typeReference.PointerNestingLevel; i++)
			{
				this.outputFormatter.PrintToken(16);
			}
		}

		private void PrintArrayRank(int[] rankSpecifier, int startRank)
		{
			for (int i = startRank; i < rankSpecifier.Length; i++)
			{
				this.outputFormatter.PrintToken(24);
				for (int j = 0; j < rankSpecifier[i]; j++)
				{
					this.outputFormatter.PrintToken(12);
				}
				this.outputFormatter.PrintToken(25);
			}
		}

		public object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			this.nodeTracker.TrackedVisit(innerClassTypeReference.BaseType, data);
			this.outputFormatter.PrintToken(10);
			return this.VisitTypeReference(innerClassTypeReference, data);
		}

		public object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintText("<");
			if (attributeSection.AttributeTarget != null && attributeSection.AttributeTarget.Length > 0)
			{
				this.outputFormatter.PrintIdentifier(attributeSection.AttributeTarget);
				this.outputFormatter.PrintToken(13);
				this.outputFormatter.Space();
			}
			Debug.Assert(attributeSection.Attributes != null);
			this.AppendCommaSeparatedList<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute>(attributeSection.Attributes);
			this.outputFormatter.PrintText(">");
			if ("assembly".Equals(attributeSection.AttributeTarget, StringComparison.InvariantCultureIgnoreCase) || "module".Equals(attributeSection.AttributeTarget, StringComparison.InvariantCultureIgnoreCase))
			{
				this.outputFormatter.NewLine();
			}
			else
			{
				this.outputFormatter.PrintLineContinuation();
			}
			return null;
		}

		public object VisitAttribute(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
		{
			this.outputFormatter.PrintIdentifier(attribute.Name);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<Expression>(attribute.PositionalArguments);
			if (attribute.NamedArguments != null && attribute.NamedArguments.Count > 0)
			{
				if (attribute.PositionalArguments.Count > 0)
				{
					this.outputFormatter.PrintToken(12);
					this.outputFormatter.Space();
				}
				this.AppendCommaSeparatedList<NamedArgumentExpression>(attribute.NamedArguments);
			}
			this.outputFormatter.PrintToken(25);
			return null;
		}

		public object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			this.outputFormatter.PrintIdentifier(namedArgumentExpression.Name);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(13);
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(namedArgumentExpression.Expression, data);
			return null;
		}

		public object VisitUsing(Using @using, object data)
		{
			Debug.Fail("Should never be called. The usings should be handled in Visit(UsingDeclaration)");
			return null;
		}

		public object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(108);
			this.outputFormatter.Space();
			for (int i = 0; i < usingDeclaration.Usings.Count; i++)
			{
				this.outputFormatter.PrintIdentifier(usingDeclaration.Usings[i].Name);
				if (usingDeclaration.Usings[i].IsAlias)
				{
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(11);
					this.outputFormatter.Space();
					this.printFullSystemType = true;
					this.nodeTracker.TrackedVisit(usingDeclaration.Usings[i].Alias, data);
					this.printFullSystemType = false;
				}
				if (i + 1 < usingDeclaration.Usings.Count)
				{
					this.outputFormatter.PrintToken(12);
					this.outputFormatter.Space();
				}
			}
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(126);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(namespaceDeclaration.Name);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisitChildren(namespaceDeclaration, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(126);
			this.outputFormatter.NewLine();
			return null;
		}

		private static int GetTypeToken(TypeDeclaration typeDeclaration)
		{
			int result;
			switch (typeDeclaration.Type)
			{
			case ClassType.Class:
				result = 67;
				return result;
			case ClassType.Interface:
				result = 112;
				return result;
			case ClassType.Struct:
				result = 166;
				return result;
			case ClassType.Enum:
				result = 90;
				return result;
			}
			result = 67;
			return result;
		}

		private void PrintTemplates(List<TemplateDefinition> templates)
		{
			if (templates != null && templates.Count > 0)
			{
				this.outputFormatter.PrintToken(24);
				this.outputFormatter.PrintToken(200);
				this.outputFormatter.Space();
				this.AppendCommaSeparatedList<TemplateDefinition>(templates);
				this.outputFormatter.PrintToken(25);
			}
		}

		public object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			this.VisitAttributes(typeDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(typeDeclaration.Modifier, true);
			int typeToken = VBNetOutputVisitor.GetTypeToken(typeDeclaration);
			this.outputFormatter.PrintToken(typeToken);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(typeDeclaration.Name);
			this.PrintTemplates(typeDeclaration.Templates);
			if (typeDeclaration.Type == ClassType.Enum && typeDeclaration.BaseTypes != null && typeDeclaration.BaseTypes.Count > 0)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				foreach (TypeReference baseTypeRef in typeDeclaration.BaseTypes)
				{
					this.nodeTracker.TrackedVisit(baseTypeRef, data);
				}
			}
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			if (typeDeclaration.BaseTypes != null && typeDeclaration.Type != ClassType.Enum)
			{
				foreach (TypeReference baseTypeRef in typeDeclaration.BaseTypes)
				{
					this.outputFormatter.Indent();
					string baseType = baseTypeRef.Type;
					if (baseType.IndexOf('.') >= 0)
					{
						baseType = baseType.Substring(baseType.LastIndexOf('.') + 1);
					}
					bool baseTypeIsInterface = baseType.Length >= 2 && baseType[0] == 'I' && char.IsUpper(baseType[1]);
					if (!baseTypeIsInterface || typeDeclaration.Type == ClassType.Interface)
					{
						this.outputFormatter.PrintToken(110);
					}
					else
					{
						this.outputFormatter.PrintToken(107);
					}
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(baseTypeRef, data);
					this.outputFormatter.NewLine();
				}
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
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(typeToken);
			this.outputFormatter.NewLine();
			return null;
		}

		private void OutputEnumMembers(TypeDeclaration typeDeclaration, object data)
		{
			using (List<INode>.Enumerator enumerator = typeDeclaration.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FieldDeclaration fieldDeclaration = (FieldDeclaration)enumerator.Current;
					this.nodeTracker.BeginNode(fieldDeclaration);
					VariableDeclaration f = fieldDeclaration.Fields[0];
					this.VisitAttributes(fieldDeclaration.Attributes, data);
					this.outputFormatter.Indent();
					this.outputFormatter.PrintIdentifier(f.Name);
					if (f.Initializer != null && !f.Initializer.IsNull)
					{
						this.outputFormatter.Space();
						this.outputFormatter.PrintToken(11);
						this.outputFormatter.Space();
						this.nodeTracker.TrackedVisit(f.Initializer, data);
					}
					this.outputFormatter.NewLine();
					this.nodeTracker.EndNode(fieldDeclaration);
				}
			}
		}

		public object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			this.outputFormatter.PrintIdentifier(templateDefinition.Name);
			if (templateDefinition.Bases.Count > 0)
			{
				this.outputFormatter.PrintText(" As ");
				if (templateDefinition.Bases.Count == 1)
				{
					this.nodeTracker.TrackedVisit(templateDefinition.Bases[0], data);
				}
				else
				{
					this.outputFormatter.PrintToken(22);
					this.AppendCommaSeparatedList<TypeReference>(templateDefinition.Bases);
					this.outputFormatter.PrintToken(23);
				}
			}
			return null;
		}

		public object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			this.VisitAttributes(delegateDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(delegateDeclaration.Modifier, true);
			this.outputFormatter.PrintToken(80);
			this.outputFormatter.Space();
			bool isFunction = delegateDeclaration.ReturnType.Type != "void";
			if (isFunction)
			{
				this.outputFormatter.PrintToken(100);
				this.outputFormatter.Space();
			}
			else
			{
				this.outputFormatter.PrintToken(167);
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintIdentifier(delegateDeclaration.Name);
			this.PrintTemplates(delegateDeclaration.Templates);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(delegateDeclaration.Parameters);
			this.outputFormatter.PrintToken(25);
			if (isFunction)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(delegateDeclaration.ReturnType, data);
			}
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			this.outputFormatter.PrintToken(136);
			this.outputFormatter.Space();
			switch (optionDeclaration.OptionType)
			{
			case OptionType.Explicit:
				this.outputFormatter.PrintToken(95);
				this.outputFormatter.Space();
				if (!optionDeclaration.OptionValue)
				{
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(134);
				}
				break;
			case OptionType.Strict:
				this.outputFormatter.PrintToken(164);
				if (!optionDeclaration.OptionValue)
				{
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(134);
				}
				break;
			case OptionType.CompareBinary:
				this.outputFormatter.PrintToken(70);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(51);
				break;
			case OptionType.CompareText:
				this.outputFormatter.PrintToken(70);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(169);
				break;
			}
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			this.VisitAttributes(fieldDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			if (fieldDeclaration.Modifier == Modifiers.None)
			{
				this.outputFormatter.PrintToken(145);
				this.outputFormatter.Space();
			}
			else if (fieldDeclaration.Modifier == Modifiers.Dim)
			{
				this.outputFormatter.PrintToken(81);
				this.outputFormatter.Space();
			}
			else
			{
				this.OutputModifier(fieldDeclaration.Modifier);
			}
			this.currentVariableType = fieldDeclaration.TypeReference;
			this.AppendCommaSeparatedList<VariableDeclaration>(fieldDeclaration.Fields);
			this.currentVariableType = null;
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			this.outputFormatter.PrintIdentifier(variableDeclaration.Name);
			TypeReference varType = this.currentVariableType;
			if (varType != null && varType.IsNull)
			{
				varType = null;
			}
			if (varType == null && !variableDeclaration.TypeReference.IsNull)
			{
				varType = variableDeclaration.TypeReference;
			}
			object result;
			if (varType != null)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				ObjectCreateExpression init = variableDeclaration.Initializer as ObjectCreateExpression;
				if (init != null && TypeReference.AreEqualReferences(init.CreateType, varType))
				{
					this.nodeTracker.TrackedVisit(variableDeclaration.Initializer, data);
					result = null;
					return result;
				}
				this.nodeTracker.TrackedVisit(varType, data);
			}
			if (!variableDeclaration.Initializer.IsNull)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(11);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(variableDeclaration.Initializer, data);
			}
			result = null;
			return result;
		}

		public object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			this.VisitAttributes(propertyDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(propertyDeclaration.Modifier);
			if ((propertyDeclaration.Modifier & (Modifiers.ReadOnly | Modifiers.WriteOnly)) == Modifiers.None)
			{
				if (propertyDeclaration.IsReadOnly)
				{
					this.outputFormatter.PrintToken(150);
					this.outputFormatter.Space();
				}
				else if (propertyDeclaration.IsWriteOnly)
				{
					this.outputFormatter.PrintToken(184);
					this.outputFormatter.Space();
				}
			}
			this.outputFormatter.PrintToken(146);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(propertyDeclaration.Name);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(propertyDeclaration.Parameters);
			this.outputFormatter.PrintToken(25);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(48);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(propertyDeclaration.TypeReference, data);
			this.PrintInterfaceImplementations(propertyDeclaration.InterfaceImplementations);
			this.outputFormatter.NewLine();
			if (!this.IsAbstract(propertyDeclaration))
			{
				this.outputFormatter.IndentationLevel++;
				this.exitTokenStack.Push(146);
				this.nodeTracker.TrackedVisit(propertyDeclaration.GetRegion, data);
				this.nodeTracker.TrackedVisit(propertyDeclaration.SetRegion, data);
				this.exitTokenStack.Pop();
				this.outputFormatter.IndentationLevel--;
				this.outputFormatter.Indent();
				this.outputFormatter.PrintToken(88);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(146);
				this.outputFormatter.NewLine();
			}
			return null;
		}

		public object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			this.VisitAttributes(propertyGetRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(propertyGetRegion.Modifier);
			this.outputFormatter.PrintToken(101);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisit(propertyGetRegion.Block, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(101);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			this.VisitAttributes(propertySetRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(propertySetRegion.Modifier);
			this.outputFormatter.PrintToken(156);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisit(propertySetRegion.Block, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(156);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			bool customEvent = eventDeclaration.HasAddRegion || eventDeclaration.HasRemoveRegion;
			this.VisitAttributes(eventDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(eventDeclaration.Modifier);
			if (customEvent)
			{
				this.outputFormatter.PrintText("Custom");
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(93);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(eventDeclaration.Name);
			if (eventDeclaration.Parameters.Count > 0)
			{
				this.outputFormatter.PrintToken(24);
				this.AppendCommaSeparatedList<ParameterDeclarationExpression>(eventDeclaration.Parameters);
				this.outputFormatter.PrintToken(25);
			}
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(48);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(eventDeclaration.TypeReference, data);
			this.PrintInterfaceImplementations(eventDeclaration.InterfaceImplementations);
			if (!eventDeclaration.Initializer.IsNull)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(11);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(eventDeclaration.Initializer, data);
			}
			this.outputFormatter.NewLine();
			if (customEvent)
			{
				this.outputFormatter.IndentationLevel++;
				this.currentEventType = eventDeclaration.TypeReference;
				this.exitTokenStack.Push(167);
				this.nodeTracker.TrackedVisit(eventDeclaration.AddRegion, data);
				this.nodeTracker.TrackedVisit(eventDeclaration.RemoveRegion, data);
				this.exitTokenStack.Pop();
				this.outputFormatter.IndentationLevel--;
				this.outputFormatter.Indent();
				this.outputFormatter.PrintToken(88);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(93);
				this.outputFormatter.NewLine();
			}
			return null;
		}

		private void PrintInterfaceImplementations(IList<InterfaceImplementation> list)
		{
			if (list != null && list.Count != 0)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(107);
				for (int i = 0; i < list.Count; i++)
				{
					if (i > 0)
					{
						this.outputFormatter.PrintToken(12);
					}
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(list[i].InterfaceType, null);
					this.outputFormatter.PrintToken(10);
					this.outputFormatter.PrintIdentifier(list[i].MemberName);
				}
			}
		}

		public object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			this.VisitAttributes(eventAddRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintText("AddHandler(");
			if (eventAddRegion.Parameters.Count == 0)
			{
				this.outputFormatter.PrintToken(55);
				this.outputFormatter.Space();
				this.outputFormatter.PrintIdentifier("value");
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(this.currentEventType, data);
			}
			else
			{
				this.AppendCommaSeparatedList<ParameterDeclarationExpression>(eventAddRegion.Parameters);
			}
			this.outputFormatter.PrintToken(25);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisit(eventAddRegion.Block, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintText("AddHandler");
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			this.VisitAttributes(eventRemoveRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintText("RemoveHandler");
			this.outputFormatter.PrintToken(24);
			if (eventRemoveRegion.Parameters.Count == 0)
			{
				this.outputFormatter.PrintToken(55);
				this.outputFormatter.Space();
				this.outputFormatter.PrintIdentifier("value");
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(this.currentEventType, data);
			}
			else
			{
				this.AppendCommaSeparatedList<ParameterDeclarationExpression>(eventRemoveRegion.Parameters);
			}
			this.outputFormatter.PrintToken(25);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisit(eventRemoveRegion.Block, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintText("RemoveHandler");
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			this.VisitAttributes(eventRaiseRegion.Attributes, data);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintText("RaiseEvent");
			this.outputFormatter.PrintToken(24);
			if (eventRaiseRegion.Parameters.Count == 0)
			{
				this.outputFormatter.PrintToken(55);
				this.outputFormatter.Space();
				this.outputFormatter.PrintIdentifier("value");
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(this.currentEventType, data);
			}
			else
			{
				this.AppendCommaSeparatedList<ParameterDeclarationExpression>(eventRaiseRegion.Parameters);
			}
			this.outputFormatter.PrintToken(25);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisit(eventRaiseRegion.Block, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintText("RaiseEvent");
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			this.VisitAttributes(parameterDeclarationExpression.Attributes, data);
			this.OutputModifier(parameterDeclarationExpression.ParamModifier, parameterDeclarationExpression.StartLocation);
			this.outputFormatter.PrintIdentifier(parameterDeclarationExpression.ParameterName);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(48);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(parameterDeclarationExpression.TypeReference, data);
			return null;
		}

		public object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			this.VisitAttributes(methodDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(methodDeclaration.Modifier);
			bool isSub = methodDeclaration.TypeReference.IsNull || methodDeclaration.TypeReference.SystemType == "System.Void";
			if (isSub)
			{
				this.outputFormatter.PrintToken(167);
			}
			else
			{
				this.outputFormatter.PrintToken(100);
			}
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(methodDeclaration.Name);
			this.PrintTemplates(methodDeclaration.Templates);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(methodDeclaration.Parameters);
			this.outputFormatter.PrintToken(25);
			if (!isSub)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(methodDeclaration.TypeReference, data);
			}
			this.PrintInterfaceImplementations(methodDeclaration.InterfaceImplementations);
			this.outputFormatter.NewLine();
			if (!this.IsAbstract(methodDeclaration))
			{
				this.nodeTracker.BeginNode(methodDeclaration.Body);
				this.outputFormatter.IndentationLevel++;
				this.exitTokenStack.Push(isSub ? 167 : 100);
				methodDeclaration.Body.AcceptVisitor(this, data);
				this.exitTokenStack.Pop();
				this.outputFormatter.IndentationLevel--;
				this.outputFormatter.Indent();
				this.outputFormatter.PrintToken(88);
				this.outputFormatter.Space();
				if (isSub)
				{
					this.outputFormatter.PrintToken(167);
				}
				else
				{
					this.outputFormatter.PrintToken(100);
				}
				this.outputFormatter.NewLine();
				this.nodeTracker.EndNode(methodDeclaration.Body);
			}
			return null;
		}

		public object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			throw new InvalidOperationException();
		}

		private bool IsAbstract(AttributedNode node)
		{
			return (node.Modifier & Modifiers.Dim) == Modifiers.Dim || (this.currentType != null && this.currentType.Type == ClassType.Interface);
		}

		public object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			this.VisitAttributes(constructorDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(constructorDeclaration.Modifier);
			this.outputFormatter.PrintToken(167);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(127);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(constructorDeclaration.Parameters);
			this.outputFormatter.PrintToken(25);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.exitTokenStack.Push(167);
			this.nodeTracker.TrackedVisit(constructorDeclaration.ConstructorInitializer, data);
			this.nodeTracker.TrackedVisit(constructorDeclaration.Body, data);
			this.exitTokenStack.Pop();
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(167);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			this.outputFormatter.Indent();
			if (constructorInitializer.ConstructorInitializerType == ConstructorInitializerType.This)
			{
				this.outputFormatter.PrintToken(119);
			}
			else
			{
				this.outputFormatter.PrintToken(124);
			}
			this.outputFormatter.PrintToken(10);
			this.outputFormatter.PrintToken(127);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<Expression>(constructorInitializer.Arguments);
			this.outputFormatter.PrintToken(25);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			this.VisitAttributes(indexerDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(indexerDeclaration.Modifier);
			this.outputFormatter.PrintToken(79);
			this.outputFormatter.Space();
			if (indexerDeclaration.IsReadOnly)
			{
				this.outputFormatter.PrintToken(150);
				this.outputFormatter.Space();
			}
			else if (indexerDeclaration.IsWriteOnly)
			{
				this.outputFormatter.PrintToken(184);
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(146);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier("Item");
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(indexerDeclaration.Parameters);
			this.outputFormatter.PrintToken(25);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(48);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(indexerDeclaration.TypeReference, data);
			this.PrintInterfaceImplementations(indexerDeclaration.InterfaceImplementations);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.exitTokenStack.Push(146);
			this.nodeTracker.TrackedVisit(indexerDeclaration.GetRegion, data);
			this.nodeTracker.TrackedVisit(indexerDeclaration.SetRegion, data);
			this.exitTokenStack.Pop();
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(146);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintText("Protected Overrides Sub Finalize()");
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.exitTokenStack.Push(167);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(174);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisit(destructorDeclaration.Body, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(97);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintText("MyBase.Finalize()");
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(174);
			this.outputFormatter.NewLine();
			this.exitTokenStack.Pop();
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(167);
			this.outputFormatter.NewLine();
			return null;
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
					this.outputFormatter.PrintToken(202);
				}
				else
				{
					this.outputFormatter.PrintToken(201);
				}
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(187);
			this.outputFormatter.Space();
			int op = -1;
			switch (operatorDeclaration.OverloadableOperator)
			{
			case OverloadableOperatorType.Add:
				op = 14;
				break;
			case OverloadableOperatorType.Subtract:
				op = 15;
				break;
			case OverloadableOperatorType.Multiply:
				op = 16;
				break;
			case OverloadableOperatorType.Divide:
				op = 17;
				break;
			case OverloadableOperatorType.Modulus:
				op = 120;
				break;
			case OverloadableOperatorType.Concat:
				op = 19;
				break;
			case OverloadableOperatorType.Not:
				op = 129;
				break;
			case OverloadableOperatorType.BitNot:
				op = 129;
				break;
			case OverloadableOperatorType.BitwiseAnd:
				op = 45;
				break;
			case OverloadableOperatorType.BitwiseOr:
				op = 138;
				break;
			case OverloadableOperatorType.ExclusiveOr:
				op = 185;
				break;
			case OverloadableOperatorType.ShiftLeft:
				op = 31;
				break;
			case OverloadableOperatorType.ShiftRight:
				op = 32;
				break;
			case OverloadableOperatorType.GreaterThan:
				op = 26;
				break;
			case OverloadableOperatorType.GreaterThanOrEqual:
				op = 29;
				break;
			case OverloadableOperatorType.Equality:
				op = 11;
				break;
			case OverloadableOperatorType.InEquality:
				op = 28;
				break;
			case OverloadableOperatorType.LessThan:
				op = 27;
				break;
			case OverloadableOperatorType.LessThanOrEqual:
				op = 30;
				break;
			case OverloadableOperatorType.Increment:
				this.Error("Increment operator is not supported in Visual Basic", operatorDeclaration.StartLocation);
				break;
			case OverloadableOperatorType.Decrement:
				this.Error("Decrement operator is not supported in Visual Basic", operatorDeclaration.StartLocation);
				break;
			case OverloadableOperatorType.IsTrue:
				this.outputFormatter.PrintText("IsTrue");
				break;
			case OverloadableOperatorType.IsFalse:
				this.outputFormatter.PrintText("IsFalse");
				break;
			case OverloadableOperatorType.Like:
				op = 116;
				break;
			case OverloadableOperatorType.Power:
				op = 20;
				break;
			case OverloadableOperatorType.CType:
				op = 75;
				break;
			case OverloadableOperatorType.DivideInteger:
				op = 18;
				break;
			}
			if (operatorDeclaration.IsConversionOperator)
			{
				this.outputFormatter.PrintToken(75);
			}
			else if (op != -1)
			{
				this.outputFormatter.PrintToken(op);
			}
			this.PrintTemplates(operatorDeclaration.Templates);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(operatorDeclaration.Parameters);
			this.outputFormatter.PrintToken(25);
			if (!operatorDeclaration.TypeReference.IsNull)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(operatorDeclaration.TypeReference, data);
			}
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			this.nodeTracker.TrackedVisit(operatorDeclaration.Body, data);
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(187);
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			this.VisitAttributes(declareDeclaration.Attributes, data);
			this.outputFormatter.Indent();
			this.OutputModifier(declareDeclaration.Modifier);
			this.outputFormatter.PrintToken(78);
			this.outputFormatter.Space();
			switch (declareDeclaration.Charset)
			{
			case CharsetModifier.Auto:
				this.outputFormatter.PrintToken(50);
				this.outputFormatter.Space();
				break;
			case CharsetModifier.Unicode:
				this.outputFormatter.PrintToken(176);
				this.outputFormatter.Space();
				break;
			case CharsetModifier.Ansi:
				this.outputFormatter.PrintToken(47);
				this.outputFormatter.Space();
				break;
			}
			bool isVoid = declareDeclaration.TypeReference.IsNull || declareDeclaration.TypeReference.SystemType == "System.Void";
			if (isVoid)
			{
				this.outputFormatter.PrintToken(167);
			}
			else
			{
				this.outputFormatter.PrintToken(100);
			}
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(declareDeclaration.Name);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(115);
			this.outputFormatter.Space();
			this.outputFormatter.PrintText('"' + VBNetOutputVisitor.ConvertString(declareDeclaration.Library) + '"');
			this.outputFormatter.Space();
			if (declareDeclaration.Alias.Length > 0)
			{
				this.outputFormatter.PrintToken(44);
				this.outputFormatter.Space();
				this.outputFormatter.PrintText('"' + VBNetOutputVisitor.ConvertString(declareDeclaration.Alias) + '"');
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<ParameterDeclarationExpression>(declareDeclaration.Parameters);
			this.outputFormatter.PrintToken(25);
			if (!isVoid)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(declareDeclaration.TypeReference, data);
			}
			this.outputFormatter.NewLine();
			return null;
		}

		public object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			this.VisitStatementList(blockStatement.Children);
			return null;
		}

		private void PrintIndentedBlock(Statement stmt)
		{
			this.outputFormatter.IndentationLevel++;
			if (stmt is BlockStatement)
			{
				this.nodeTracker.TrackedVisit(stmt, null);
			}
			else
			{
				this.outputFormatter.Indent();
				this.nodeTracker.TrackedVisit(stmt, null);
				this.outputFormatter.NewLine();
			}
			this.outputFormatter.IndentationLevel--;
		}

		private void PrintIndentedBlock(IEnumerable statements)
		{
			this.outputFormatter.IndentationLevel++;
			this.VisitStatementList(statements);
			this.outputFormatter.IndentationLevel--;
		}

		private void VisitStatementList(IEnumerable statements)
		{
			foreach (Statement stmt in statements)
			{
				if (stmt is BlockStatement)
				{
					this.nodeTracker.TrackedVisit(stmt, null);
				}
				else
				{
					this.outputFormatter.Indent();
					this.nodeTracker.TrackedVisit(stmt, null);
					this.outputFormatter.NewLine();
				}
			}
		}

		public object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			this.outputFormatter.PrintToken(42);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(addHandlerStatement.EventExpression, data);
			this.outputFormatter.PrintToken(12);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(addHandlerStatement.HandlerExpression, data);
			return null;
		}

		public object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			this.outputFormatter.PrintToken(152);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(removeHandlerStatement.EventExpression, data);
			this.outputFormatter.PrintToken(12);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(removeHandlerStatement.HandlerExpression, data);
			return null;
		}

		public object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			this.outputFormatter.PrintToken(149);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(raiseEventStatement.EventName);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<Expression>(raiseEventStatement.Arguments);
			this.outputFormatter.PrintToken(25);
			return null;
		}

		public object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			this.outputFormatter.PrintToken(91);
			this.outputFormatter.Space();
			this.AppendCommaSeparatedList<Expression>(eraseStatement.Expressions);
			return null;
		}

		public object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			this.outputFormatter.PrintToken(92);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(errorStatement.Expression, data);
			return null;
		}

		public object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			this.outputFormatter.PrintToken(135);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(92);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(onErrorStatement.EmbeddedStatement, data);
			return null;
		}

		public object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			this.outputFormatter.PrintToken(151);
			this.outputFormatter.Space();
			if (reDimStatement.IsPreserve)
			{
				this.outputFormatter.PrintToken(144);
				this.outputFormatter.Space();
			}
			this.AppendCommaSeparatedList<InvocationExpression>(reDimStatement.ReDimClauses);
			return null;
		}

		public object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			this.nodeTracker.TrackedVisit(expressionStatement.Expression, data);
			return null;
		}

		public object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			if (localVariableDeclaration.Modifier != Modifiers.None)
			{
				this.OutputModifier(localVariableDeclaration.Modifier);
			}
			if (!this.isUsingResourceAcquisition)
			{
				if ((localVariableDeclaration.Modifier & Modifiers.Const) == Modifiers.None)
				{
					this.outputFormatter.PrintToken(81);
				}
				this.outputFormatter.Space();
			}
			this.currentVariableType = localVariableDeclaration.TypeReference;
			this.AppendCommaSeparatedList<VariableDeclaration>(localVariableDeclaration.Variables);
			this.currentVariableType = null;
			return null;
		}

		public object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			this.outputFormatter.NewLine();
			return null;
		}

		public virtual object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			this.UnsupportedNode(yieldStatement);
			return null;
		}

		public object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			this.outputFormatter.PrintToken(154);
			if (!returnStatement.Expression.IsNull)
			{
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(returnStatement.Expression, data);
			}
			return null;
		}

		public object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			this.outputFormatter.PrintToken(106);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(ifElseStatement.Condition, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(170);
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(ifElseStatement.TrueStatement);
			foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections)
			{
				this.nodeTracker.TrackedVisit(elseIfSection, data);
			}
			if (ifElseStatement.HasElseStatements)
			{
				this.outputFormatter.Indent();
				this.outputFormatter.PrintToken(86);
				this.outputFormatter.NewLine();
				this.PrintIndentedBlock(ifElseStatement.FalseStatement);
			}
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(106);
			return null;
		}

		public object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			this.outputFormatter.PrintToken(87);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(elseIfSection.Condition, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(170);
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(elseIfSection.EmbeddedStatement);
			return null;
		}

		public object VisitForStatement(ForStatement forStatement, object data)
		{
			this.exitTokenStack.Push(181);
			bool isFirstLine = true;
			foreach (INode node in forStatement.Initializers)
			{
				if (!isFirstLine)
				{
					this.outputFormatter.Indent();
				}
				isFirstLine = false;
				this.nodeTracker.TrackedVisit(node, data);
				this.outputFormatter.NewLine();
			}
			if (!isFirstLine)
			{
				this.outputFormatter.Indent();
			}
			this.outputFormatter.PrintToken(181);
			this.outputFormatter.Space();
			if (forStatement.Condition.IsNull)
			{
				this.outputFormatter.PrintToken(173);
			}
			else
			{
				this.nodeTracker.TrackedVisit(forStatement.Condition, data);
			}
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(forStatement.EmbeddedStatement);
			this.PrintIndentedBlock(forStatement.Iterator);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(181);
			this.exitTokenStack.Pop();
			return null;
		}

		public object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			this.outputFormatter.PrintIdentifier(labelStatement.Label);
			this.outputFormatter.PrintToken(13);
			return null;
		}

		public object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			this.outputFormatter.PrintToken(104);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(gotoStatement.Label);
			return null;
		}

		public object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			this.exitTokenStack.Push(155);
			this.outputFormatter.PrintToken(155);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(57);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(switchStatement.SwitchExpression, data);
			this.outputFormatter.NewLine();
			this.outputFormatter.IndentationLevel++;
			foreach (SwitchSection section in switchStatement.SwitchSections)
			{
				this.nodeTracker.TrackedVisit(section, data);
			}
			this.outputFormatter.IndentationLevel--;
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(155);
			this.exitTokenStack.Pop();
			return null;
		}

		public object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(57);
			this.outputFormatter.Space();
			this.AppendCommaSeparatedList<CaseLabel>(switchSection.SwitchLabels);
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(switchSection.Children);
			return null;
		}

		public object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			if (caseLabel.IsDefault)
			{
				this.outputFormatter.PrintToken(86);
			}
			else
			{
				if (caseLabel.BinaryOperatorType != BinaryOperatorType.None)
				{
					switch (caseLabel.BinaryOperatorType)
					{
					case BinaryOperatorType.GreaterThan:
						this.outputFormatter.PrintToken(26);
						break;
					case BinaryOperatorType.GreaterThanOrEqual:
						this.outputFormatter.PrintToken(29);
						break;
					case BinaryOperatorType.Equality:
						this.outputFormatter.PrintToken(11);
						break;
					case BinaryOperatorType.InEquality:
						this.outputFormatter.PrintToken(27);
						this.outputFormatter.PrintToken(26);
						break;
					case BinaryOperatorType.LessThan:
						this.outputFormatter.PrintToken(27);
						break;
					case BinaryOperatorType.LessThanOrEqual:
						this.outputFormatter.PrintToken(30);
						break;
					}
					this.outputFormatter.Space();
				}
				this.nodeTracker.TrackedVisit(caseLabel.Label, data);
				if (!caseLabel.ToExpression.IsNull)
				{
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(172);
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(caseLabel.ToExpression, data);
				}
			}
			return null;
		}

		public object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			this.outputFormatter.PrintToken(94);
			if (this.exitTokenStack.Count > 0)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(this.exitTokenStack.Peek());
			}
			return null;
		}

		public object VisitStopStatement(StopStatement stopStatement, object data)
		{
			this.outputFormatter.PrintToken(163);
			return null;
		}

		public object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			this.outputFormatter.PrintToken(153);
			this.outputFormatter.Space();
			if (resumeStatement.IsResumeNext)
			{
				this.outputFormatter.PrintToken(128);
			}
			else
			{
				this.outputFormatter.PrintIdentifier(resumeStatement.LabelName);
			}
			return null;
		}

		public object VisitEndStatement(EndStatement endStatement, object data)
		{
			this.outputFormatter.PrintToken(88);
			return null;
		}

		public object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			this.outputFormatter.PrintToken(186);
			this.outputFormatter.Space();
			switch (continueStatement.ContinueType)
			{
			case ContinueType.Do:
				this.outputFormatter.PrintToken(83);
				break;
			case ContinueType.For:
				this.outputFormatter.PrintToken(98);
				break;
			case ContinueType.While:
				this.outputFormatter.PrintToken(181);
				break;
			default:
				this.outputFormatter.PrintToken(this.exitTokenStack.Peek());
				break;
			}
			return null;
		}

		public object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			this.outputFormatter.PrintText("goto case ");
			if (gotoCaseStatement.IsDefaultCase)
			{
				this.outputFormatter.PrintText("default");
			}
			else
			{
				this.nodeTracker.TrackedVisit(gotoCaseStatement.Expression, null);
			}
			return null;
		}

		public object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			if (doLoopStatement.ConditionPosition == ConditionPosition.None)
			{
				this.Error(string.Format("Unknown condition position for loop : {0}.", doLoopStatement), doLoopStatement.StartLocation);
			}
			if (doLoopStatement.ConditionPosition == ConditionPosition.Start)
			{
				switch (doLoopStatement.ConditionType)
				{
				case ConditionType.Until:
					this.exitTokenStack.Push(83);
					this.outputFormatter.PrintToken(83);
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(181);
					break;
				case ConditionType.While:
					this.exitTokenStack.Push(181);
					this.outputFormatter.PrintToken(181);
					break;
				case ConditionType.DoWhile:
					this.exitTokenStack.Push(83);
					this.outputFormatter.PrintToken(83);
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(181);
					break;
				default:
					throw new InvalidOperationException();
				}
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(doLoopStatement.Condition, null);
			}
			else
			{
				this.exitTokenStack.Push(83);
				this.outputFormatter.PrintToken(83);
			}
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(doLoopStatement.EmbeddedStatement);
			this.outputFormatter.Indent();
			if (doLoopStatement.ConditionPosition == ConditionPosition.Start && doLoopStatement.ConditionType == ConditionType.While)
			{
				this.outputFormatter.PrintToken(88);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(181);
			}
			else
			{
				this.outputFormatter.PrintToken(118);
			}
			if (doLoopStatement.ConditionPosition == ConditionPosition.End && !doLoopStatement.Condition.IsNull)
			{
				this.outputFormatter.Space();
				switch (doLoopStatement.ConditionType)
				{
				case ConditionType.Until:
					this.outputFormatter.PrintToken(177);
					break;
				case ConditionType.While:
				case ConditionType.DoWhile:
					this.outputFormatter.PrintToken(181);
					break;
				}
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(doLoopStatement.Condition, null);
			}
			this.exitTokenStack.Pop();
			return null;
		}

		public object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			this.exitTokenStack.Push(98);
			this.outputFormatter.PrintToken(98);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(85);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(foreachStatement.VariableName);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(48);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(foreachStatement.TypeReference, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(109);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(foreachStatement.Expression, data);
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(foreachStatement.EmbeddedStatement);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(128);
			if (!foreachStatement.NextExpression.IsNull)
			{
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(foreachStatement.NextExpression, data);
			}
			this.exitTokenStack.Pop();
			return null;
		}

		public object VisitLockStatement(LockStatement lockStatement, object data)
		{
			this.outputFormatter.PrintToken(168);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(lockStatement.LockExpression, data);
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(lockStatement.EmbeddedStatement);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(168);
			return null;
		}

		public object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			this.outputFormatter.PrintToken(188);
			this.outputFormatter.Space();
			this.isUsingResourceAcquisition = true;
			this.nodeTracker.TrackedVisit(usingStatement.ResourceAcquisition, data);
			this.isUsingResourceAcquisition = false;
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(usingStatement.EmbeddedStatement);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(188);
			return null;
		}

		public object VisitWithStatement(WithStatement withStatement, object data)
		{
			this.outputFormatter.PrintToken(182);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(withStatement.Expression, data);
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(withStatement.Body);
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(182);
			return null;
		}

		public object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			this.exitTokenStack.Push(174);
			this.outputFormatter.PrintToken(174);
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(tryCatchStatement.StatementBlock);
			foreach (CatchClause catchClause in tryCatchStatement.CatchClauses)
			{
				this.nodeTracker.TrackedVisit(catchClause, data);
			}
			if (!tryCatchStatement.FinallyBlock.IsNull)
			{
				this.outputFormatter.Indent();
				this.outputFormatter.PrintToken(97);
				this.outputFormatter.NewLine();
				this.PrintIndentedBlock(tryCatchStatement.FinallyBlock);
			}
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(88);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(174);
			this.exitTokenStack.Pop();
			return null;
		}

		public object VisitCatchClause(CatchClause catchClause, object data)
		{
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(58);
			if (!catchClause.TypeReference.IsNull)
			{
				this.outputFormatter.Space();
				if (catchClause.VariableName.Length > 0)
				{
					this.outputFormatter.PrintIdentifier(catchClause.VariableName);
				}
				else
				{
					this.outputFormatter.PrintIdentifier("generatedExceptionName");
				}
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.outputFormatter.PrintIdentifier(catchClause.TypeReference.Type);
			}
			if (!catchClause.Condition.IsNull)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(180);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(catchClause.Condition, data);
			}
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(catchClause.StatementBlock);
			return null;
		}

		public object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			this.outputFormatter.PrintToken(171);
			if (!throwStatement.Expression.IsNull)
			{
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(throwStatement.Expression, data);
			}
			return null;
		}

		public object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			this.UnsupportedNode(fixedStatement);
			return this.nodeTracker.TrackedVisit(fixedStatement.EmbeddedStatement, data);
		}

		public object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			this.UnsupportedNode(unsafeStatement);
			return this.nodeTracker.TrackedVisit(unsafeStatement.Block, data);
		}

		public object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			this.UnsupportedNode(checkedStatement);
			return this.nodeTracker.TrackedVisit(checkedStatement.Block, data);
		}

		public object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			this.UnsupportedNode(uncheckedStatement);
			return this.nodeTracker.TrackedVisit(uncheckedStatement.Block, data);
		}

		public object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			this.outputFormatter.PrintToken(94);
			if (exitStatement.ExitType != ExitType.None)
			{
				this.outputFormatter.Space();
				switch (exitStatement.ExitType)
				{
				case ExitType.Sub:
					this.outputFormatter.PrintToken(167);
					break;
				case ExitType.Function:
					this.outputFormatter.PrintToken(100);
					break;
				case ExitType.Property:
					this.outputFormatter.PrintToken(146);
					break;
				case ExitType.Do:
					this.outputFormatter.PrintToken(83);
					break;
				case ExitType.For:
					this.outputFormatter.PrintToken(98);
					break;
				case ExitType.While:
					this.outputFormatter.PrintToken(181);
					break;
				case ExitType.Select:
					this.outputFormatter.PrintToken(155);
					break;
				case ExitType.Try:
					this.outputFormatter.PrintToken(174);
					break;
				default:
					this.Error(string.Format("Unsupported exit type : {0}", exitStatement.ExitType), exitStatement.StartLocation);
					break;
				}
			}
			return null;
		}

		public object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			this.exitTokenStack.Push(98);
			this.outputFormatter.PrintToken(98);
			this.outputFormatter.Space();
			this.outputFormatter.PrintIdentifier(forNextStatement.VariableName);
			if (!forNextStatement.TypeReference.IsNull)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(48);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(forNextStatement.TypeReference, data);
			}
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(11);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(forNextStatement.Start, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(172);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(forNextStatement.End, data);
			if (!forNextStatement.Step.IsNull)
			{
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(162);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(forNextStatement.Step, data);
			}
			this.outputFormatter.NewLine();
			this.PrintIndentedBlock(forNextStatement.EmbeddedStatement);
			this.outputFormatter.Indent();
			this.outputFormatter.PrintToken(128);
			if (forNextStatement.NextExpressions.Count > 0)
			{
				this.outputFormatter.Space();
				this.AppendCommaSeparatedList<Expression>(forNextStatement.NextExpressions);
			}
			this.exitTokenStack.Pop();
			return null;
		}

		public object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			this.outputFormatter.PrintToken(125);
			return null;
		}

		private static string ConvertCharLiteral(char ch)
		{
			string result;
			if (char.IsControl(ch))
			{
				result = "Chr(" + (int)ch + ")";
			}
			else if (ch == '"')
			{
				result = "\"\"\"\"C";
			}
			else
			{
				result = "\"" + ch.ToString() + "\"C";
			}
			return result;
		}

		private static string ConvertString(string str)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				char ch = str[i];
				if (char.IsControl(ch))
				{
					sb.Append("\" & Chr(" + (int)ch + ") & \"");
				}
				else if (ch == '"')
				{
					sb.Append("\"\"");
				}
				else
				{
					sb.Append(ch);
				}
			}
			return sb.ToString();
		}

		public object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			object val = primitiveExpression.Value;
			object result;
			if (val == null)
			{
				this.outputFormatter.PrintToken(130);
				result = null;
			}
			else if (val is bool)
			{
				if ((bool)primitiveExpression.Value)
				{
					this.outputFormatter.PrintToken(173);
				}
				else
				{
					this.outputFormatter.PrintToken(96);
				}
				result = null;
			}
			else if (val is string)
			{
				this.outputFormatter.PrintText('"' + VBNetOutputVisitor.ConvertString((string)val) + '"');
				result = null;
			}
			else if (val is char)
			{
				this.outputFormatter.PrintText(VBNetOutputVisitor.ConvertCharLiteral((char)primitiveExpression.Value));
				result = null;
			}
			else if (val is decimal)
			{
				this.outputFormatter.PrintText(((decimal)primitiveExpression.Value).ToString(NumberFormatInfo.InvariantInfo) + "D");
				result = null;
			}
			else if (val is float)
			{
				this.outputFormatter.PrintText(((float)primitiveExpression.Value).ToString(NumberFormatInfo.InvariantInfo) + "F");
				result = null;
			}
			else
			{
				if (val is IFormattable)
				{
					this.outputFormatter.PrintText(((IFormattable)val).ToString(null, NumberFormatInfo.InvariantInfo));
				}
				else
				{
					this.outputFormatter.PrintText(val.ToString());
				}
				result = null;
			}
			return result;
		}

		public object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			int op = 0;
			object result;
			switch (binaryOperatorExpression.Op)
			{
			case BinaryOperatorType.BitwiseAnd:
				op = 45;
				break;
			case BinaryOperatorType.BitwiseOr:
				op = 138;
				break;
			case BinaryOperatorType.LogicalAnd:
				op = 46;
				break;
			case BinaryOperatorType.LogicalOr:
				op = 139;
				break;
			case BinaryOperatorType.ExclusiveOr:
				op = 185;
				break;
			case BinaryOperatorType.GreaterThan:
				op = 26;
				break;
			case BinaryOperatorType.GreaterThanOrEqual:
				op = 29;
				break;
			case BinaryOperatorType.Equality:
				op = 11;
				break;
			case BinaryOperatorType.InEquality:
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(27);
				this.outputFormatter.PrintToken(26);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
				result = null;
				return result;
			case BinaryOperatorType.LessThan:
				op = 27;
				break;
			case BinaryOperatorType.LessThanOrEqual:
				op = 30;
				break;
			case BinaryOperatorType.Add:
				op = 14;
				break;
			case BinaryOperatorType.Subtract:
				op = 15;
				break;
			case BinaryOperatorType.Multiply:
				op = 16;
				break;
			case BinaryOperatorType.Divide:
				op = 17;
				break;
			case BinaryOperatorType.Modulus:
				op = 120;
				break;
			case BinaryOperatorType.ShiftLeft:
				op = 31;
				break;
			case BinaryOperatorType.ShiftRight:
				op = 32;
				break;
			case BinaryOperatorType.ReferenceEquality:
				op = 113;
				break;
			case BinaryOperatorType.ReferenceInequality:
				op = 189;
				break;
			case BinaryOperatorType.NullCoalescing:
				this.outputFormatter.PrintText("IIf(");
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
				this.outputFormatter.PrintText(" Is Nothing, ");
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
				this.outputFormatter.PrintToken(12);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
				this.outputFormatter.PrintToken(25);
				result = null;
				return result;
			}
			this.nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(op);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
			result = null;
			return result;
		}

		public object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			this.outputFormatter.PrintToken(24);
			this.nodeTracker.TrackedVisit(parenthesizedExpression.Expression, data);
			this.outputFormatter.PrintToken(25);
			return null;
		}

		public object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			this.nodeTracker.TrackedVisit(invocationExpression.TargetObject, data);
			if (invocationExpression.TypeArguments != null && invocationExpression.TypeArguments.Count > 0)
			{
				this.outputFormatter.PrintToken(24);
				this.outputFormatter.PrintToken(200);
				this.outputFormatter.Space();
				this.AppendCommaSeparatedList<TypeReference>(invocationExpression.TypeArguments);
				this.outputFormatter.PrintToken(25);
			}
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<Expression>(invocationExpression.Arguments);
			this.outputFormatter.PrintToken(25);
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
			case UnaryOperatorType.BitNot:
				this.outputFormatter.PrintToken(129);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				result = null;
				break;
			case UnaryOperatorType.Minus:
				this.outputFormatter.PrintToken(15);
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				result = null;
				break;
			case UnaryOperatorType.Plus:
				this.outputFormatter.PrintToken(14);
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				result = null;
				break;
			case UnaryOperatorType.Increment:
				this.outputFormatter.PrintText("System.Threading.Interlocked.Increment(");
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				this.outputFormatter.PrintText(")");
				result = null;
				break;
			case UnaryOperatorType.Decrement:
				this.outputFormatter.PrintText("System.Threading.Interlocked.Decrement(");
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				this.outputFormatter.PrintText(")");
				result = null;
				break;
			case UnaryOperatorType.PostIncrement:
				this.outputFormatter.PrintText("System.Math.Max(System.Threading.Interlocked.Increment(");
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				this.outputFormatter.PrintText("),");
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				this.outputFormatter.PrintText(" - 1)");
				result = null;
				break;
			case UnaryOperatorType.PostDecrement:
				this.outputFormatter.PrintText("System.Math.Max(System.Threading.Interlocked.Decrement(");
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				this.outputFormatter.PrintText("),");
				this.nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
				this.outputFormatter.PrintText(" + 1)");
				result = null;
				break;
			case UnaryOperatorType.Star:
				this.outputFormatter.PrintToken(16);
				result = null;
				break;
			case UnaryOperatorType.BitWiseAnd:
				this.outputFormatter.PrintToken(43);
				result = null;
				break;
			default:
				this.Error("unknown unary operator: " + unaryOperatorExpression.Op.ToString(), unaryOperatorExpression.StartLocation);
				result = null;
				break;
			}
			return result;
		}

		public object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			int op = 0;
			bool unsupportedOpAssignment = false;
			object result;
			switch (assignmentExpression.Op)
			{
			case AssignmentOperatorType.Assign:
				op = 11;
				break;
			case AssignmentOperatorType.Add:
				op = 33;
				if (VBNetOutputVisitor.IsEventHandlerCreation(assignmentExpression.Right))
				{
					this.outputFormatter.PrintToken(42);
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(assignmentExpression.Left, data);
					this.outputFormatter.PrintToken(12);
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(43);
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(VBNetOutputVisitor.GetEventHandlerMethod(assignmentExpression.Right), data);
					result = null;
					return result;
				}
				break;
			case AssignmentOperatorType.Subtract:
				op = 35;
				if (VBNetOutputVisitor.IsEventHandlerCreation(assignmentExpression.Right))
				{
					this.outputFormatter.PrintToken(152);
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(assignmentExpression.Left, data);
					this.outputFormatter.PrintToken(12);
					this.outputFormatter.Space();
					this.outputFormatter.PrintToken(43);
					this.outputFormatter.Space();
					this.nodeTracker.TrackedVisit(VBNetOutputVisitor.GetEventHandlerMethod(assignmentExpression.Right), data);
					result = null;
					return result;
				}
				break;
			case AssignmentOperatorType.Multiply:
				op = 36;
				break;
			case AssignmentOperatorType.Divide:
				op = 37;
				break;
			case AssignmentOperatorType.Modulus:
				op = 120;
				unsupportedOpAssignment = true;
				break;
			case AssignmentOperatorType.ShiftLeft:
				op = 39;
				break;
			case AssignmentOperatorType.ShiftRight:
				op = 40;
				break;
			case AssignmentOperatorType.BitwiseAnd:
				op = 45;
				unsupportedOpAssignment = true;
				break;
			case AssignmentOperatorType.BitwiseOr:
				op = 138;
				unsupportedOpAssignment = true;
				break;
			case AssignmentOperatorType.ExclusiveOr:
				op = 185;
				unsupportedOpAssignment = true;
				break;
			}
			this.nodeTracker.TrackedVisit(assignmentExpression.Left, data);
			this.outputFormatter.Space();
			if (unsupportedOpAssignment)
			{
				this.outputFormatter.PrintToken(11);
				this.outputFormatter.Space();
				this.nodeTracker.TrackedVisit(assignmentExpression.Left, data);
				this.outputFormatter.Space();
			}
			this.outputFormatter.PrintToken(op);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(assignmentExpression.Right, data);
			result = null;
			return result;
		}

		public object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			this.UnsupportedNode(sizeOfExpression);
			return null;
		}

		public object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			this.outputFormatter.PrintToken(102);
			this.outputFormatter.PrintToken(24);
			this.nodeTracker.TrackedVisit(typeOfExpression.TypeReference, data);
			this.outputFormatter.PrintToken(25);
			return null;
		}

		public object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			this.outputFormatter.PrintToken(130);
			return null;
		}

		public object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			this.outputFormatter.PrintToken(175);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(typeOfIsExpression.Expression, data);
			this.outputFormatter.Space();
			this.outputFormatter.PrintToken(113);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(typeOfIsExpression.TypeReference, data);
			return null;
		}

		public object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			this.outputFormatter.PrintToken(43);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(addressOfExpression.Expression, data);
			return null;
		}

		public object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			this.UnsupportedNode(anonymousMethodExpression);
			return null;
		}

		public object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			this.UnsupportedNode(checkedExpression);
			return this.nodeTracker.TrackedVisit(checkedExpression.Expression, data);
		}

		public object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			this.UnsupportedNode(uncheckedExpression);
			return this.nodeTracker.TrackedVisit(uncheckedExpression.Expression, data);
		}

		public object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			this.UnsupportedNode(pointerReferenceExpression);
			return null;
		}

		public object VisitCastExpression(CastExpression castExpression, object data)
		{
			object result;
			if (castExpression.CastType == CastType.Cast)
			{
				result = this.PrintCast(82, castExpression);
			}
			else if (castExpression.CastType == CastType.TryCast)
			{
				result = this.PrintCast(199, castExpression);
			}
			else
			{
				string systemType = castExpression.CastTo.SystemType;
				if (systemType != null)
				{
					if (__Class1.__member2 == null)
					{
						__Class1.__member2 = new Dictionary<string, int>(16)
						{
							{
								"System.Boolean",
								0
							},
							{
								"System.Byte",
								1
							},
							{
								"System.SByte",
								2
							},
							{
								"System.Char",
								3
							},
							{
								"System.DateTime",
								4
							},
							{
								"System.Decimal",
								5
							},
							{
								"System.Double",
								6
							},
							{
								"System.Int16",
								7
							},
							{
								"System.Int32",
								8
							},
							{
								"System.Int64",
								9
							},
							{
								"System.UInt16",
								10
							},
							{
								"System.UInt32",
								11
							},
							{
								"System.UInt64",
								12
							},
							{
								"System.Object",
								13
							},
							{
								"System.Single",
								14
							},
							{
								"System.String",
								15
							}
						};
					}
					int num;
					if (__Class1.__member2.TryGetValue(systemType, out num))
					{
						switch (num)
						{
						case 0:
							this.outputFormatter.PrintToken(59);
							break;
						case 1:
							this.outputFormatter.PrintToken(60);
							break;
						case 2:
							this.outputFormatter.PrintToken(194);
							break;
						case 3:
							this.outputFormatter.PrintToken(61);
							break;
						case 4:
							this.outputFormatter.PrintToken(62);
							break;
						case 5:
							this.outputFormatter.PrintToken(64);
							break;
						case 6:
							this.outputFormatter.PrintToken(63);
							break;
						case 7:
							this.outputFormatter.PrintToken(72);
							break;
						case 8:
							this.outputFormatter.PrintToken(66);
							break;
						case 9:
							this.outputFormatter.PrintToken(68);
							break;
						case 10:
							this.outputFormatter.PrintToken(195);
							break;
						case 11:
							this.outputFormatter.PrintToken(66);
							break;
						case 12:
							this.outputFormatter.PrintToken(68);
							break;
						case 13:
							this.outputFormatter.PrintToken(69);
							break;
						case 14:
							this.outputFormatter.PrintToken(73);
							break;
						case 15:
							this.outputFormatter.PrintToken(74);
							break;
						default:
							goto IL_2B4;
						}
						this.outputFormatter.PrintToken(24);
						this.nodeTracker.TrackedVisit(castExpression.Expression, data);
						this.outputFormatter.PrintToken(25);
						result = null;
						return result;
					}
				}
				IL_2B4:
				result = this.PrintCast(75, castExpression);
			}
			return result;
		}

		private object PrintCast(int castToken, CastExpression castExpression)
		{
			this.outputFormatter.PrintToken(castToken);
			this.outputFormatter.PrintToken(24);
			this.nodeTracker.TrackedVisit(castExpression.Expression, null);
			this.outputFormatter.PrintToken(12);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(castExpression.CastTo, null);
			this.outputFormatter.PrintToken(25);
			return null;
		}

		public object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			this.UnsupportedNode(stackAllocExpression);
			return null;
		}

		public object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			this.nodeTracker.TrackedVisit(indexerExpression.TargetObject, data);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<Expression>(indexerExpression.Indexes);
			this.outputFormatter.PrintToken(25);
			return null;
		}

		public object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			this.outputFormatter.PrintToken(119);
			return null;
		}

		public object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			this.outputFormatter.PrintToken(124);
			return null;
		}

		public object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			this.outputFormatter.PrintToken(127);
			this.outputFormatter.Space();
			this.nodeTracker.TrackedVisit(objectCreateExpression.CreateType, data);
			this.outputFormatter.PrintToken(24);
			this.AppendCommaSeparatedList<Expression>(objectCreateExpression.Parameters);
			this.outputFormatter.PrintToken(25);
			return null;
		}

		public object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			this.outputFormatter.PrintToken(127);
			this.outputFormatter.Space();
			this.PrintTypeReferenceWithoutArray(arrayCreateExpression.CreateType);
			if (arrayCreateExpression.Arguments.Count > 0)
			{
				this.outputFormatter.PrintToken(24);
				this.AppendCommaSeparatedList<Expression>(arrayCreateExpression.Arguments);
				this.outputFormatter.PrintToken(25);
				this.PrintArrayRank(arrayCreateExpression.CreateType.RankSpecifier, 1);
			}
			else
			{
				this.PrintArrayRank(arrayCreateExpression.CreateType.RankSpecifier, 0);
			}
			this.outputFormatter.Space();
			if (arrayCreateExpression.ArrayInitializer.IsNull)
			{
				this.outputFormatter.PrintToken(22);
				this.outputFormatter.PrintToken(23);
			}
			else
			{
				this.nodeTracker.TrackedVisit(arrayCreateExpression.ArrayInitializer, data);
			}
			return null;
		}

		public object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			this.outputFormatter.PrintToken(22);
			this.AppendCommaSeparatedList<Expression>(arrayInitializerExpression.CreateExpressions);
			this.outputFormatter.PrintToken(23);
			return null;
		}

		public object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			this.nodeTracker.TrackedVisit(fieldReferenceExpression.TargetObject, data);
			this.outputFormatter.PrintToken(10);
			this.outputFormatter.PrintIdentifier(fieldReferenceExpression.FieldName);
			return null;
		}

		public object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			this.nodeTracker.TrackedVisit(directionExpression.Expression, data);
			return null;
		}

		public object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			this.outputFormatter.PrintText("IIf");
			this.outputFormatter.PrintToken(24);
			this.nodeTracker.TrackedVisit(conditionalExpression.Condition, data);
			this.outputFormatter.PrintToken(12);
			this.nodeTracker.TrackedVisit(conditionalExpression.TrueExpression, data);
			this.outputFormatter.PrintToken(12);
			this.nodeTracker.TrackedVisit(conditionalExpression.FalseExpression, data);
			this.outputFormatter.PrintToken(25);
			return null;
		}

		private void OutputModifier(ParameterModifiers modifier, Location position)
		{
			switch (modifier)
			{
			case ParameterModifiers.None:
			case ParameterModifiers.In:
				this.outputFormatter.PrintToken(55);
				goto IL_B1;
			case ParameterModifiers.Out:
				this.Error("Out parameter converted to ByRef", position);
				this.outputFormatter.PrintToken(53);
				goto IL_B1;
			case ParameterModifiers.In | ParameterModifiers.Out:
			case ParameterModifiers.In | ParameterModifiers.Ref:
			case ParameterModifiers.Out | ParameterModifiers.Ref:
			case ParameterModifiers.In | ParameterModifiers.Out | ParameterModifiers.Ref:
				break;
			case ParameterModifiers.Ref:
				this.outputFormatter.PrintToken(53);
				goto IL_B1;
			case ParameterModifiers.Params:
				this.outputFormatter.PrintToken(143);
				goto IL_B1;
			default:
				if (modifier == ParameterModifiers.Optional)
				{
					this.outputFormatter.PrintToken(137);
					goto IL_B1;
				}
				break;
			}
			this.Error(string.Format("Unsupported modifier : {0}", modifier), position);
			IL_B1:
			this.outputFormatter.Space();
		}

		private void OutputModifier(Modifiers modifier)
		{
			this.OutputModifier(modifier, false);
		}

		private void OutputModifier(Modifiers modifier, bool forTypeDecl)
		{
			if ((modifier & Modifiers.Public) == Modifiers.Public)
			{
				this.outputFormatter.PrintToken(148);
				this.outputFormatter.Space();
			}
			else if ((modifier & Modifiers.Private) == Modifiers.Private)
			{
				this.outputFormatter.PrintToken(145);
				this.outputFormatter.Space();
			}
			else if ((modifier & (Modifiers.Internal | Modifiers.Protected)) == (Modifiers.Internal | Modifiers.Protected))
			{
				this.outputFormatter.PrintToken(147);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(99);
				this.outputFormatter.Space();
			}
			else if ((modifier & Modifiers.Internal) == Modifiers.Internal)
			{
				this.outputFormatter.PrintToken(99);
				this.outputFormatter.Space();
			}
			else if ((modifier & Modifiers.Protected) == Modifiers.Protected)
			{
				this.outputFormatter.PrintToken(147);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.Static) == Modifiers.Static)
			{
				this.outputFormatter.PrintToken(158);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.Virtual) == Modifiers.Virtual)
			{
				this.outputFormatter.PrintToken(141);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.Dim) == Modifiers.Dim)
			{
				this.outputFormatter.PrintToken(forTypeDecl ? 122 : 123);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.Override) == Modifiers.Override)
			{
				this.outputFormatter.PrintToken(140);
				this.outputFormatter.Space();
				this.outputFormatter.PrintToken(142);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.New) == Modifiers.New)
			{
				this.outputFormatter.PrintToken(157);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.Sealed) == Modifiers.Sealed)
			{
				this.outputFormatter.PrintToken(forTypeDecl ? 131 : 132);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.ReadOnly) == Modifiers.ReadOnly)
			{
				this.outputFormatter.PrintToken(150);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.WriteOnly) == Modifiers.WriteOnly)
			{
				this.outputFormatter.PrintToken(184);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.Const) == Modifiers.Const)
			{
				this.outputFormatter.PrintToken(71);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.Partial) == Modifiers.Partial)
			{
				this.outputFormatter.PrintToken(203);
				this.outputFormatter.Space();
			}
			if ((modifier & Modifiers.Extern) == Modifiers.Extern)
			{
			}
			if ((modifier & Modifiers.Volatile) == Modifiers.Volatile)
			{
				this.Error("'Volatile' modifier not convertable", Location.Empty);
			}
			if ((modifier & Modifiers.Unsafe) == Modifiers.Unsafe)
			{
				this.Error("'Unsafe' modifier not convertable", Location.Empty);
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
						this.outputFormatter.PrintToken(12);
						this.outputFormatter.Space();
						if ((i + 1) % 6 == 0)
						{
							this.outputFormatter.PrintLineContinuation();
							this.outputFormatter.Indent();
							this.outputFormatter.PrintText("\t");
						}
					}
					i++;
				}
			}
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

		private static bool IsEventHandlerCreation(Expression expr)
		{
			bool result;
			if (expr is ObjectCreateExpression)
			{
				ObjectCreateExpression oce = (ObjectCreateExpression)expr;
				if (oce.Parameters.Count == 1)
				{
					result = oce.CreateType.SystemType.EndsWith("Handler");
					return result;
				}
			}
			result = false;
			return result;
		}

		private static Expression GetEventHandlerMethod(Expression expr)
		{
			ObjectCreateExpression oce = (ObjectCreateExpression)expr;
			return oce.Parameters[0];
		}
	}
}

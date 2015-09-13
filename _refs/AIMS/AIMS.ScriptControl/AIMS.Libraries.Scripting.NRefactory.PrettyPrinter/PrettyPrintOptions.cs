using System;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public class PrettyPrintOptions : AbstractPrettyPrintOptions
	{
		private BraceStyle namespaceBraceStyle = BraceStyle.NextLine;

		private BraceStyle classBraceStyle = BraceStyle.NextLine;

		private BraceStyle interfaceBraceStyle = BraceStyle.NextLine;

		private BraceStyle structBraceStyle = BraceStyle.NextLine;

		private BraceStyle enumBraceStyle = BraceStyle.NextLine;

		private BraceStyle constructorBraceStyle = BraceStyle.NextLine;

		private BraceStyle destructorBraceStyle = BraceStyle.NextLine;

		private BraceStyle methodBraceStyle = BraceStyle.NextLine;

		private BraceStyle propertyBraceStyle = BraceStyle.EndOfLine;

		private BraceStyle propertyGetBraceStyle = BraceStyle.EndOfLine;

		private BraceStyle propertySetBraceStyle = BraceStyle.EndOfLine;

		private BraceStyle eventAddBraceStyle = BraceStyle.EndOfLine;

		private BraceStyle eventRemoveBraceStyle = BraceStyle.EndOfLine;

		private BraceStyle statementBraceStyle = BraceStyle.EndOfLine;

		private bool beforeMethodCallParentheses = false;

		private bool beforeDelegateDeclarationParentheses = false;

		private bool beforeMethodDeclarationParentheses = false;

		private bool beforeConstructorDeclarationParentheses = false;

		private bool ifParentheses = true;

		private bool whileParentheses = true;

		private bool forParentheses = true;

		private bool foreachParentheses = true;

		private bool catchParentheses = true;

		private bool switchParentheses = true;

		private bool lockParentheses = true;

		private bool usingParentheses = true;

		private bool fixedParentheses = true;

		private bool sizeOfParentheses = false;

		private bool typeOfParentheses = false;

		private bool checkedParentheses = false;

		private bool uncheckedParentheses = false;

		private bool newParentheses = false;

		private bool aroundAssignmentParentheses = true;

		private bool aroundLogicalOperatorParentheses = true;

		private bool aroundEqualityOperatorParentheses = true;

		private bool aroundRelationalOperatorParentheses = true;

		private bool aroundBitwiseOperatorParentheses = true;

		private bool aroundAdditiveOperatorParentheses = true;

		private bool aroundMultiplicativeOperatorParentheses = true;

		private bool aroundShiftOperatorParentheses = true;

		private bool conditionalOperatorBeforeConditionSpace = true;

		private bool conditionalOperatorAfterConditionSpace = true;

		private bool conditionalOperatorBeforeSeparatorSpace = true;

		private bool conditionalOperatorAfterSeparatorSpace = true;

		private bool spacesWithinBrackets = false;

		private bool spacesAfterComma = true;

		private bool spacesBeforeComma = false;

		private bool spacesAfterSemicolon = true;

		private bool spacesAfterTypecast = false;

		public BraceStyle StatementBraceStyle
		{
			get
			{
				return this.statementBraceStyle;
			}
			set
			{
				this.statementBraceStyle = value;
			}
		}

		public BraceStyle NamespaceBraceStyle
		{
			get
			{
				return this.namespaceBraceStyle;
			}
			set
			{
				this.namespaceBraceStyle = value;
			}
		}

		public BraceStyle ClassBraceStyle
		{
			get
			{
				return this.classBraceStyle;
			}
			set
			{
				this.classBraceStyle = value;
			}
		}

		public BraceStyle InterfaceBraceStyle
		{
			get
			{
				return this.interfaceBraceStyle;
			}
			set
			{
				this.interfaceBraceStyle = value;
			}
		}

		public BraceStyle StructBraceStyle
		{
			get
			{
				return this.structBraceStyle;
			}
			set
			{
				this.structBraceStyle = value;
			}
		}

		public BraceStyle EnumBraceStyle
		{
			get
			{
				return this.enumBraceStyle;
			}
			set
			{
				this.enumBraceStyle = value;
			}
		}

		public BraceStyle ConstructorBraceStyle
		{
			get
			{
				return this.constructorBraceStyle;
			}
			set
			{
				this.constructorBraceStyle = value;
			}
		}

		public BraceStyle DestructorBraceStyle
		{
			get
			{
				return this.destructorBraceStyle;
			}
			set
			{
				this.destructorBraceStyle = value;
			}
		}

		public BraceStyle MethodBraceStyle
		{
			get
			{
				return this.methodBraceStyle;
			}
			set
			{
				this.methodBraceStyle = value;
			}
		}

		public BraceStyle PropertyBraceStyle
		{
			get
			{
				return this.propertyBraceStyle;
			}
			set
			{
				this.propertyBraceStyle = value;
			}
		}

		public BraceStyle PropertyGetBraceStyle
		{
			get
			{
				return this.propertyGetBraceStyle;
			}
			set
			{
				this.propertyGetBraceStyle = value;
			}
		}

		public BraceStyle PropertySetBraceStyle
		{
			get
			{
				return this.propertySetBraceStyle;
			}
			set
			{
				this.propertySetBraceStyle = value;
			}
		}

		public BraceStyle EventAddBraceStyle
		{
			get
			{
				return this.eventAddBraceStyle;
			}
			set
			{
				this.eventAddBraceStyle = value;
			}
		}

		public BraceStyle EventRemoveBraceStyle
		{
			get
			{
				return this.eventRemoveBraceStyle;
			}
			set
			{
				this.eventRemoveBraceStyle = value;
			}
		}

		public bool CheckedParentheses
		{
			get
			{
				return this.checkedParentheses;
			}
			set
			{
				this.checkedParentheses = value;
			}
		}

		public bool NewParentheses
		{
			get
			{
				return this.newParentheses;
			}
			set
			{
				this.newParentheses = value;
			}
		}

		public bool SizeOfParentheses
		{
			get
			{
				return this.sizeOfParentheses;
			}
			set
			{
				this.sizeOfParentheses = value;
			}
		}

		public bool TypeOfParentheses
		{
			get
			{
				return this.typeOfParentheses;
			}
			set
			{
				this.typeOfParentheses = value;
			}
		}

		public bool UncheckedParentheses
		{
			get
			{
				return this.uncheckedParentheses;
			}
			set
			{
				this.uncheckedParentheses = value;
			}
		}

		public bool BeforeConstructorDeclarationParentheses
		{
			get
			{
				return this.beforeConstructorDeclarationParentheses;
			}
			set
			{
				this.beforeConstructorDeclarationParentheses = value;
			}
		}

		public bool BeforeDelegateDeclarationParentheses
		{
			get
			{
				return this.beforeDelegateDeclarationParentheses;
			}
			set
			{
				this.beforeDelegateDeclarationParentheses = value;
			}
		}

		public bool BeforeMethodCallParentheses
		{
			get
			{
				return this.beforeMethodCallParentheses;
			}
			set
			{
				this.beforeMethodCallParentheses = value;
			}
		}

		public bool BeforeMethodDeclarationParentheses
		{
			get
			{
				return this.beforeMethodDeclarationParentheses;
			}
			set
			{
				this.beforeMethodDeclarationParentheses = value;
			}
		}

		public bool IfParentheses
		{
			get
			{
				return this.ifParentheses;
			}
			set
			{
				this.ifParentheses = value;
			}
		}

		public bool WhileParentheses
		{
			get
			{
				return this.whileParentheses;
			}
			set
			{
				this.whileParentheses = value;
			}
		}

		public bool ForeachParentheses
		{
			get
			{
				return this.foreachParentheses;
			}
			set
			{
				this.foreachParentheses = value;
			}
		}

		public bool LockParentheses
		{
			get
			{
				return this.lockParentheses;
			}
			set
			{
				this.lockParentheses = value;
			}
		}

		public bool UsingParentheses
		{
			get
			{
				return this.usingParentheses;
			}
			set
			{
				this.usingParentheses = value;
			}
		}

		public bool CatchParentheses
		{
			get
			{
				return this.catchParentheses;
			}
			set
			{
				this.catchParentheses = value;
			}
		}

		public bool FixedParentheses
		{
			get
			{
				return this.fixedParentheses;
			}
			set
			{
				this.fixedParentheses = value;
			}
		}

		public bool SwitchParentheses
		{
			get
			{
				return this.switchParentheses;
			}
			set
			{
				this.switchParentheses = value;
			}
		}

		public bool ForParentheses
		{
			get
			{
				return this.forParentheses;
			}
			set
			{
				this.forParentheses = value;
			}
		}

		public bool AroundAdditiveOperatorParentheses
		{
			get
			{
				return this.aroundAdditiveOperatorParentheses;
			}
			set
			{
				this.aroundAdditiveOperatorParentheses = value;
			}
		}

		public bool AroundAssignmentParentheses
		{
			get
			{
				return this.aroundAssignmentParentheses;
			}
			set
			{
				this.aroundAssignmentParentheses = value;
			}
		}

		public bool AroundBitwiseOperatorParentheses
		{
			get
			{
				return this.aroundBitwiseOperatorParentheses;
			}
			set
			{
				this.aroundBitwiseOperatorParentheses = value;
			}
		}

		public bool AroundEqualityOperatorParentheses
		{
			get
			{
				return this.aroundEqualityOperatorParentheses;
			}
			set
			{
				this.aroundEqualityOperatorParentheses = value;
			}
		}

		public bool AroundLogicalOperatorParentheses
		{
			get
			{
				return this.aroundLogicalOperatorParentheses;
			}
			set
			{
				this.aroundLogicalOperatorParentheses = value;
			}
		}

		public bool AroundMultiplicativeOperatorParentheses
		{
			get
			{
				return this.aroundMultiplicativeOperatorParentheses;
			}
			set
			{
				this.aroundMultiplicativeOperatorParentheses = value;
			}
		}

		public bool AroundRelationalOperatorParentheses
		{
			get
			{
				return this.aroundRelationalOperatorParentheses;
			}
			set
			{
				this.aroundRelationalOperatorParentheses = value;
			}
		}

		public bool AroundShiftOperatorParentheses
		{
			get
			{
				return this.aroundShiftOperatorParentheses;
			}
			set
			{
				this.aroundShiftOperatorParentheses = value;
			}
		}

		public bool ConditionalOperatorAfterConditionSpace
		{
			get
			{
				return this.conditionalOperatorAfterConditionSpace;
			}
			set
			{
				this.conditionalOperatorAfterConditionSpace = value;
			}
		}

		public bool ConditionalOperatorAfterSeparatorSpace
		{
			get
			{
				return this.conditionalOperatorAfterSeparatorSpace;
			}
			set
			{
				this.conditionalOperatorAfterSeparatorSpace = value;
			}
		}

		public bool ConditionalOperatorBeforeConditionSpace
		{
			get
			{
				return this.conditionalOperatorBeforeConditionSpace;
			}
			set
			{
				this.conditionalOperatorBeforeConditionSpace = value;
			}
		}

		public bool ConditionalOperatorBeforeSeparatorSpace
		{
			get
			{
				return this.conditionalOperatorBeforeSeparatorSpace;
			}
			set
			{
				this.conditionalOperatorBeforeSeparatorSpace = value;
			}
		}

		public bool SpacesAfterComma
		{
			get
			{
				return this.spacesAfterComma;
			}
			set
			{
				this.spacesAfterComma = value;
			}
		}

		public bool SpacesAfterSemicolon
		{
			get
			{
				return this.spacesAfterSemicolon;
			}
			set
			{
				this.spacesAfterSemicolon = value;
			}
		}

		public bool SpacesAfterTypecast
		{
			get
			{
				return this.spacesAfterTypecast;
			}
			set
			{
				this.spacesAfterTypecast = value;
			}
		}

		public bool SpacesBeforeComma
		{
			get
			{
				return this.spacesBeforeComma;
			}
			set
			{
				this.spacesBeforeComma = value;
			}
		}

		public bool SpacesWithinBrackets
		{
			get
			{
				return this.spacesWithinBrackets;
			}
			set
			{
				this.spacesWithinBrackets = value;
			}
		}
	}
}

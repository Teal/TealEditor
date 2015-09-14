using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class OptionDeclaration : AbstractNode
	{
		private OptionType optionType;

		private bool optionValue;

		public OptionType OptionType
		{
			get
			{
				return this.optionType;
			}
			set
			{
				this.optionType = value;
			}
		}

		public bool OptionValue
		{
			get
			{
				return this.optionValue;
			}
			set
			{
				this.optionValue = value;
			}
		}

		public OptionDeclaration(OptionType optionType, bool optionValue)
		{
			this.OptionType = optionType;
			this.OptionValue = optionValue;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitOptionDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[OptionDeclaration OptionType={0} OptionValue={1}]", this.OptionType, this.OptionValue);
		}
	}
}

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public abstract class EventAddRemoveRegion : AttributedNode, INullable
	{
		private BlockStatement block;

		private List<ParameterDeclarationExpression> parameters;

		public BlockStatement Block
		{
			get
			{
				return this.block;
			}
			set
			{
				this.block = (value ?? BlockStatement.Null);
				if (!this.block.IsNull)
				{
					this.block.Parent = this;
				}
			}
		}

		public List<ParameterDeclarationExpression> Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = (value ?? new List<ParameterDeclarationExpression>());
			}
		}

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		protected EventAddRemoveRegion(List<AttributeSection> attributes) : base(attributes)
		{
			this.block = BlockStatement.Null;
			this.parameters = new List<ParameterDeclarationExpression>();
		}
	}
}

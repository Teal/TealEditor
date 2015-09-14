using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public abstract class PropertyGetSetRegion : AttributedNode, INullable
	{
		private BlockStatement block;

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

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		protected PropertyGetSetRegion(BlockStatement block, List<AttributeSection> attributes) : base(attributes)
		{
			this.Block = block;
		}
	}
}

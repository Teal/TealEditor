using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class ArrayReturnType : ProxyReturnType
	{
		public class ArrayIndexer : DefaultProperty
		{
			public ArrayIndexer(IReturnType elementType, IClass systemArray) : base("Indexer", elementType, ModifierEnum.Public, DomRegion.Empty, DomRegion.Empty, systemArray)
			{
				base.IsIndexer = true;
			}
		}

		private IReturnType elementType;

		private int dimensions;

		private IProjectContent pc;

		internal IProjectContent ProjectContent
		{
			get
			{
				return this.pc;
			}
		}

		public IReturnType ArrayElementType
		{
			get
			{
				return this.elementType;
			}
		}

		public int ArrayDimensions
		{
			get
			{
				return this.dimensions;
			}
		}

		public override string FullyQualifiedName
		{
			get
			{
				return this.elementType.FullyQualifiedName;
			}
		}

		public override string Name
		{
			get
			{
				return this.elementType.Name;
			}
		}

		public override string DotNetName
		{
			get
			{
				return this.AppendArrayString(this.elementType.DotNetName);
			}
		}

		public override IReturnType BaseType
		{
			get
			{
				return this.pc.SystemTypes.Array;
			}
		}

		public override bool IsDefaultReturnType
		{
			get
			{
				return false;
			}
		}

		public override bool IsArrayReturnType
		{
			get
			{
				return true;
			}
		}

		public override bool IsConstructedReturnType
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericReturnType
		{
			get
			{
				return false;
			}
		}

		public ArrayReturnType(IProjectContent pc, IReturnType elementType, int dimensions)
		{
			if (pc == null)
			{
				throw new ArgumentNullException("pc");
			}
			if (dimensions <= 0)
			{
				throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
			}
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			this.pc = pc;
			this.elementType = elementType;
			this.dimensions = dimensions;
		}

		public override bool Equals(object o)
		{
			IReturnType rt = o as IReturnType;
			bool result;
			if (rt == null || !rt.IsArrayReturnType)
			{
				result = false;
			}
			else
			{
				ArrayReturnType art = rt.CastToArrayReturnType();
				result = (art.ArrayDimensions == this.dimensions && this.elementType.Equals(art.ArrayElementType));
			}
			return result;
		}

		public override int GetHashCode()
		{
			return 2 * this.elementType.GetHashCode() + 27 * this.dimensions;
		}

		public override List<IProperty> GetProperties()
		{
			List<IProperty> i = base.GetProperties();
			ArrayReturnType.ArrayIndexer property = new ArrayReturnType.ArrayIndexer(this.elementType, this.BaseType.GetUnderlyingClass());
			IReturnType int32 = this.pc.SystemTypes.Int32;
			for (int j = 0; j < this.dimensions; j++)
			{
				property.Parameters.Add(new DefaultParameter("index", int32, DomRegion.Empty));
			}
			i.Add(property);
			return i;
		}

		private string AppendArrayString(string a)
		{
			StringBuilder b = new StringBuilder(a, a.Length + 1 + this.dimensions);
			b.Append('[');
			for (int i = 1; i < this.dimensions; i++)
			{
				b.Append(',');
			}
			b.Append(']');
			return b.ToString();
		}

		public override string ToString()
		{
			return string.Format("[ArrayReturnType: {0}, dimensions={1}]", this.elementType, this.AppendArrayString(""));
		}

		public override ArrayReturnType CastToArrayReturnType()
		{
			return this;
		}
	}
}

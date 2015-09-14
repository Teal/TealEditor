using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultAttribute : IAttribute, IComparable
	{
		public static readonly IList<IAttribute> EmptyAttributeList = new List<IAttribute>().AsReadOnly();

		private string name;

		private List<AttributeArgument> positionalArguments;

		private SortedList<string, AttributeArgument> namedArguments;

		private AttributeTarget attributeTarget;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public AttributeTarget AttributeTarget
		{
			get
			{
				return this.attributeTarget;
			}
			set
			{
				this.attributeTarget = value;
			}
		}

		public List<AttributeArgument> PositionalArguments
		{
			get
			{
				return this.positionalArguments;
			}
		}

		public SortedList<string, AttributeArgument> NamedArguments
		{
			get
			{
				return this.namedArguments;
			}
		}

		public DefaultAttribute(string name) : this(name, AttributeTarget.None)
		{
		}

		public DefaultAttribute(string name, AttributeTarget attributeTarget)
		{
			this.name = name;
			this.attributeTarget = attributeTarget;
			this.positionalArguments = new List<AttributeArgument>();
			this.namedArguments = new SortedList<string, AttributeArgument>();
		}

		public DefaultAttribute(string name, AttributeTarget attributeTarget, List<AttributeArgument> positionalArguments, SortedList<string, AttributeArgument> namedArguments)
		{
			this.name = name;
			this.attributeTarget = attributeTarget;
			this.positionalArguments = positionalArguments;
			this.namedArguments = namedArguments;
		}

		public virtual int CompareTo(IAttribute value)
		{
			return this.Name.CompareTo(value.Name);
		}

		int IComparable.CompareTo(object value)
		{
			return this.CompareTo((IAttribute)value);
		}
	}
}

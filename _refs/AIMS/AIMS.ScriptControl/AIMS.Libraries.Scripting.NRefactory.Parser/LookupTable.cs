using System;
using System.Globalization;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	internal class LookupTable
	{
		private class Node
		{
			public string word;

			public int val;

			public LookupTable.Node[] leaf = new LookupTable.Node[256];

			public Node(int val, string word)
			{
				this.word = word;
				this.val = val;
			}
		}

		private LookupTable.Node root = new LookupTable.Node(-1, null);

		private bool casesensitive;

		private int length;

		public int Count
		{
			get
			{
				return this.length;
			}
		}

		public int this[string keyword]
		{
			get
			{
				LookupTable.Node next = this.root;
				if (!this.casesensitive)
				{
					keyword = keyword.ToUpper(CultureInfo.InvariantCulture);
				}
				int i = 0;
				int result;
				while (i < keyword.Length)
				{
					int index = (int)(keyword[i] % 'Ā');
					next = next.leaf[index];
					if (next == null)
					{
						result = -1;
					}
					else
					{
						if (!(keyword == next.word))
						{
							i++;
							continue;
						}
						result = next.val;
					}
					return result;
				}
				result = -1;
				return result;
			}
			set
			{
				LookupTable.Node node = this.root;
				LookupTable.Node next = this.root;
				if (!this.casesensitive)
				{
					keyword = keyword.ToUpper(CultureInfo.InvariantCulture);
				}
				this.length++;
				for (int i = 0; i < keyword.Length; i++)
				{
					int index = (int)(keyword[i] % 'Ā');
					bool d = keyword[i] == '\\';
					next = next.leaf[index];
					if (next == null)
					{
						node.leaf[index] = new LookupTable.Node(value, keyword);
						break;
					}
					if (next.word != null && next.word.Length != i)
					{
						string tmpword = next.word;
						int tmpval = next.val;
						next.val = -1;
						next.word = null;
						this[tmpword] = tmpval;
					}
					if (i == keyword.Length - 1)
					{
						next.word = keyword;
						next.val = value;
						break;
					}
					node = next;
				}
			}
		}

		public LookupTable(bool casesensitive)
		{
			this.casesensitive = casesensitive;
		}
	}
}

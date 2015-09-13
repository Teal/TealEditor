using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultUsing : IUsing
	{
		private DomRegion region;

		private IProjectContent projectContent;

		private List<string> usings = new List<string>();

		private SortedList<string, IReturnType> aliases = null;

		public DomRegion Region
		{
			get
			{
				return this.region;
			}
		}

		public List<string> Usings
		{
			get
			{
				return this.usings;
			}
		}

		public SortedList<string, IReturnType> Aliases
		{
			get
			{
				return this.aliases;
			}
		}

		public bool HasAliases
		{
			get
			{
				return this.aliases != null && this.aliases.Count > 0;
			}
		}

		public DefaultUsing(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
		}

		public DefaultUsing(IProjectContent projectContent, DomRegion region) : this(projectContent)
		{
			this.region = region;
		}

		public void AddAlias(string alias, IReturnType type)
		{
			if (this.aliases == null)
			{
				this.aliases = new SortedList<string, IReturnType>();
			}
			this.aliases.Add(alias, type);
		}

		public string SearchNamespace(string partialNamespaceName)
		{
			string result;
			if (this.HasAliases)
			{
				foreach (KeyValuePair<string, IReturnType> entry in this.aliases)
				{
					if (entry.Value.IsDefaultReturnType)
					{
						string aliasString = entry.Key;
						if (this.projectContent.Language.NameComparer.Equals(partialNamespaceName, aliasString))
						{
							string nsName = entry.Value.FullyQualifiedName;
							if (this.projectContent.NamespaceExists(nsName))
							{
								result = nsName;
								return result;
							}
						}
						if (partialNamespaceName.Length > aliasString.Length)
						{
							if (this.projectContent.Language.NameComparer.Equals(partialNamespaceName.Substring(0, aliasString.Length + 1), aliasString + "."))
							{
								string nsName = entry.Value.FullyQualifiedName + partialNamespaceName.Remove(0, aliasString.Length);
								if (this.projectContent.NamespaceExists(nsName))
								{
									result = nsName;
									return result;
								}
							}
						}
					}
				}
			}
			if (this.projectContent.Language.ImportNamespaces)
			{
				foreach (string str in this.usings)
				{
					string possibleNamespace = str + "." + partialNamespaceName;
					if (this.projectContent.NamespaceExists(possibleNamespace))
					{
						result = possibleNamespace;
						return result;
					}
				}
			}
			result = null;
			return result;
		}

		public IEnumerable<IReturnType> SearchType(string partialTypeName, int typeParameterCount)
		{
            //DefaultUsing.__Class2 ___m = new DefaultUsing.__Class2(-2);
            //___m.<>4__this = this;
            //___m.<>3__partialTypeName = partialTypeName;
            //___m.<>3__typeParameterCount = typeParameterCount;
            //return ___m;
            return null;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("[DefaultUsing: ");
			foreach (string str in this.usings)
			{
				builder.Append(str);
				builder.Append(", ");
			}
			if (this.HasAliases)
			{
				foreach (KeyValuePair<string, IReturnType> p in this.aliases)
				{
					builder.Append(p.Key);
					builder.Append("=");
					builder.Append(p.Value.ToString());
					builder.Append(", ");
				}
			}
			builder.Length -= 2;
			builder.Append("]");
			return builder.ToString();
		}
	}
}

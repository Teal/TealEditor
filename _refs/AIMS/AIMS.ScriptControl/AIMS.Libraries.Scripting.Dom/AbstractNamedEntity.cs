using System;
using System.Diagnostics;

namespace AIMS.Libraries.Scripting.Dom
{
	public abstract class AbstractNamedEntity : AbstractDecoration
	{
		private static char[] nameDelimiters = new char[]
		{
			'.',
			'+'
		};

		private string fullyQualifiedName = null;

		private string name = null;

		private string nspace = null;

		public string FullyQualifiedName
		{
			get
			{
				string empty;
				if (this.fullyQualifiedName == null)
				{
					if (this.name == null || this.nspace == null)
					{
						empty = string.Empty;
						return empty;
					}
					this.fullyQualifiedName = this.nspace + '.' + this.name;
				}
				empty = this.fullyQualifiedName;
				return empty;
			}
			set
			{
				if (!(this.fullyQualifiedName == value))
				{
					this.fullyQualifiedName = value;
					this.name = null;
					this.nspace = null;
					this.OnFullyQualifiedNameChanged(EventArgs.Empty);
				}
			}
		}

		public virtual string DotNetName
		{
			get
			{
				string result;
				if (base.DeclaringType != null)
				{
					result = base.DeclaringType.DotNetName + "." + this.Name;
				}
				else
				{
					result = this.FullyQualifiedName;
				}
				return result;
			}
		}

		public string Name
		{
			get
			{
				if (this.name == null && this.FullyQualifiedName != null)
				{
					int lastIndex;
					if (this.CanBeSubclass)
					{
						lastIndex = this.FullyQualifiedName.LastIndexOfAny(AbstractNamedEntity.nameDelimiters);
					}
					else
					{
						lastIndex = this.FullyQualifiedName.LastIndexOf('.');
					}
					if (lastIndex < 0)
					{
						this.name = this.FullyQualifiedName;
					}
					else
					{
						this.name = this.FullyQualifiedName.Substring(lastIndex + 1);
					}
				}
				return this.name;
			}
		}

		public string Namespace
		{
			get
			{
				if (this.nspace == null && this.FullyQualifiedName != null)
				{
					int lastIndex = this.FullyQualifiedName.LastIndexOf('.');
					if (lastIndex < 0)
					{
						this.nspace = string.Empty;
					}
					else
					{
						this.nspace = this.FullyQualifiedName.Substring(0, lastIndex);
					}
				}
				return this.nspace;
			}
		}

		protected virtual bool CanBeSubclass
		{
			get
			{
				return false;
			}
		}

		protected virtual void OnFullyQualifiedNameChanged(EventArgs e)
		{
		}

		public AbstractNamedEntity(IClass declaringType) : base(declaringType)
		{
		}

		public AbstractNamedEntity(IClass declaringType, string name) : base(declaringType)
		{
			Debug.Assert(declaringType != null);
			this.name = name;
			this.nspace = declaringType.FullyQualifiedName;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}]", base.GetType().Name, this.FullyQualifiedName);
		}
	}
}

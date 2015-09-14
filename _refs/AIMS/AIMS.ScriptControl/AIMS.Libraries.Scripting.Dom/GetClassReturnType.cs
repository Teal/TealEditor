using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class GetClassReturnType : ProxyReturnType
	{
		private IProjectContent content;

		private string fullName;

		private string shortName;

		private int typeParameterCount;

		public override bool IsDefaultReturnType
		{
			get
			{
				return true;
			}
		}

		public override int TypeParameterCount
		{
			get
			{
				return this.typeParameterCount;
			}
		}

		public override IReturnType BaseType
		{
			get
			{
				IClass c = this.content.GetClass(this.fullName, this.typeParameterCount);
				return (c != null) ? c.DefaultReturnType : null;
			}
		}

		public override string FullyQualifiedName
		{
			get
			{
				return this.fullName;
			}
		}

		public override string Name
		{
			get
			{
				return this.shortName;
			}
		}

		public override string Namespace
		{
			get
			{
				string tmp = base.Namespace;
				string result;
				if (tmp == "?")
				{
					if (this.fullName.IndexOf('.') > 0)
					{
						result = this.fullName.Substring(0, this.fullName.LastIndexOf('.'));
					}
					else
					{
						result = "";
					}
				}
				else
				{
					result = tmp;
				}
				return result;
			}
		}

		public override string DotNetName
		{
			get
			{
				string tmp = base.DotNetName;
				string result;
				if (tmp == "?")
				{
					result = this.fullName;
				}
				else
				{
					result = tmp;
				}
				return result;
			}
		}

		public GetClassReturnType(IProjectContent content, string fullName, int typeParameterCount)
		{
			this.content = content;
			this.typeParameterCount = typeParameterCount;
			this.SetFullyQualifiedName(fullName);
		}

		public override bool Equals(object o)
		{
			IReturnType rt = o as IReturnType;
			return rt != null && rt.IsDefaultReturnType && DefaultReturnType.Equals(this, rt);
		}

		public override int GetHashCode()
		{
			return this.content.GetHashCode() ^ this.fullName.GetHashCode() ^ this.typeParameterCount * 5;
		}

		public void SetFullyQualifiedName(string fullName)
		{
			if (fullName == null)
			{
				throw new ArgumentNullException("fullName");
			}
			this.fullName = fullName;
			int pos = fullName.LastIndexOf('.');
			if (pos < 0)
			{
				this.shortName = fullName;
			}
			else
			{
				this.shortName = fullName.Substring(pos + 1);
			}
		}

		public override string ToString()
		{
			return string.Format("[GetClassReturnType: {0}]", this.fullName);
		}
	}
}

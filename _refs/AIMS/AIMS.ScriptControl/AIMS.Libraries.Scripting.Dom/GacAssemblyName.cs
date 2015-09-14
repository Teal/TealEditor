using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class GacAssemblyName : IEquatable<GacAssemblyName>
	{
		private readonly string fullName;

		private readonly string[] info;

		public string Name
		{
			get
			{
				return this.info[0];
			}
		}

		public string Version
		{
			get
			{
				return (this.info.Length > 1) ? this.info[1].Substring(this.info[1].LastIndexOf('=') + 1) : null;
			}
		}

		public string PublicKey
		{
			get
			{
				return (this.info.Length > 3) ? this.info[3].Substring(this.info[3].LastIndexOf('=') + 1) : null;
			}
		}

		public string FullName
		{
			get
			{
				return this.fullName;
			}
		}

		public GacAssemblyName(string fullName)
		{
			if (fullName == null)
			{
				throw new ArgumentNullException("fullName");
			}
			this.fullName = fullName;
			this.info = fullName.Split(new char[]
			{
				','
			});
		}

		public override string ToString()
		{
			return this.fullName;
		}

		public bool Equals(GacAssemblyName other)
		{
			return other != null && this.fullName == other.fullName;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as GacAssemblyName);
		}

		public override int GetHashCode()
		{
			return this.fullName.GetHashCode();
		}
	}
}

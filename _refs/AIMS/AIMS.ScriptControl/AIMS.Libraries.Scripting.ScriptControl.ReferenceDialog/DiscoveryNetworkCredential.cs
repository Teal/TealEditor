using System;
using System.Net;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class DiscoveryNetworkCredential : NetworkCredential
	{
		public const string DefaultAuthenticationType = "Default";

		private string authenticationType = string.Empty;

		public string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
		}

		public bool IsDefaultAuthenticationType
		{
			get
			{
				return string.Compare(this.authenticationType, "Default", true) == 0;
			}
		}

		public DiscoveryNetworkCredential(string userName, string password, string domain, string authenticationType) : base(userName, password, domain)
		{
			this.authenticationType = authenticationType;
		}

		public DiscoveryNetworkCredential(NetworkCredential credential, string authenticationType) : this(credential.UserName, credential.Password, credential.Domain, authenticationType)
		{
		}
	}
}

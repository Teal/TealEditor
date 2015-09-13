using System;
using System.Net;
using System.Text;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class HttpAuthenticationHeader
	{
		private string[] authenticationSchemes;

		public string AuthenticationType
		{
			get
			{
				string result;
				if (this.HasAuthenticationSchemes)
				{
					int schemesAdded = 0;
					StringBuilder authenticationType = new StringBuilder();
					for (int i = 0; i < this.authenticationSchemes.Length; i++)
					{
						string scheme = this.authenticationSchemes[i];
						int index = scheme.IndexOf(' ');
						if (index > 0)
						{
							scheme = scheme.Substring(0, index);
						}
						if (schemesAdded > 0)
						{
							authenticationType.Append(",");
						}
						authenticationType.Append(scheme);
						schemesAdded++;
					}
					result = authenticationType.ToString();
				}
				else
				{
					result = string.Empty;
				}
				return result;
			}
		}

		private bool HasAuthenticationSchemes
		{
			get
			{
				return this.authenticationSchemes != null && this.authenticationSchemes.Length > 0;
			}
		}

		public HttpAuthenticationHeader(WebHeaderCollection headers)
		{
			this.authenticationSchemes = headers.GetValues("WWW-Authenticate");
		}

		public override string ToString()
		{
			string result;
			if (this.HasAuthenticationSchemes)
			{
				StringBuilder schemes = new StringBuilder();
				string[] array = this.authenticationSchemes;
				for (int i = 0; i < array.Length; i++)
				{
					string scheme = array[i];
					schemes.Append("WWW-Authenticate: ");
					schemes.Append(scheme);
					schemes.Append("\r\n");
				}
				result = schemes.ToString();
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}
	}
}

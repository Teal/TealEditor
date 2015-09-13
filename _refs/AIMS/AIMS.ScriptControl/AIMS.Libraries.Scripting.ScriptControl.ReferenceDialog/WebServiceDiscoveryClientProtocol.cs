using System;
using System.Net;
using System.Web.Services.Discovery;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class WebServiceDiscoveryClientProtocol : DiscoveryClientProtocol
	{
		private HttpWebResponse lastResponseReceived;

		public bool IsAuthenticationRequired
		{
			get
			{
				return this.lastResponseReceived != null && this.lastResponseReceived.StatusCode == HttpStatusCode.Unauthorized;
			}
		}

		public HttpAuthenticationHeader GetAuthenticationHeader()
		{
			HttpAuthenticationHeader result;
			if (this.lastResponseReceived != null)
			{
				result = new HttpAuthenticationHeader(this.lastResponseReceived.Headers);
			}
			else
			{
				result = null;
			}
			return result;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse response = base.GetWebResponse(request);
			this.lastResponseReceived = (response as HttpWebResponse);
			return response;
		}
	}
}

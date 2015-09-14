using System;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class AsyncDiscoveryState
	{
		private WebServiceDiscoveryClientProtocol protocol;

		private Uri uri;

		private DiscoveryNetworkCredential credential;

		public WebServiceDiscoveryClientProtocol Protocol
		{
			get
			{
				return this.protocol;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		public DiscoveryNetworkCredential Credential
		{
			get
			{
				return this.credential;
			}
		}

		public AsyncDiscoveryState(WebServiceDiscoveryClientProtocol protocol, Uri uri, DiscoveryNetworkCredential credential)
		{
			this.protocol = protocol;
			this.uri = uri;
			this.credential = credential;
		}
	}
}

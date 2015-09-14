using AIMS.Libraries.Scripting.ScriptControl.Project;
using AIMS.Libraries.Scripting.ScriptControl.Properties;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class AddWebReferenceDialog : Form
	{
		private delegate DiscoveryDocument DiscoverAnyAsync(string url);

		private delegate void DiscoveredWebServicesHandler(DiscoveryClientProtocol protocol);

		private delegate void AuthenticationHandler(Uri uri, string authenticationType);

		private WebServiceDiscoveryClientProtocol discoveryClientProtocol;

		private CredentialCache credentialCache = new CredentialCache();

		private string namespacePrefix = string.Empty;

		private Uri discoveryUri;

		private IProject project;

		private WebReference webReference;

		private ScriptLanguage _Language = ScriptLanguage.CSharp;

		private Label label2;

		private TextBox txtName;

		private StringCollection _CreatedFileNames = null;

		private Label namespaceLabel;

		private TextBox namespaceTextBox;

		private Button cancelButton;

		private Button addButton;

		private TextBox referenceNameTextBox;

		private Label referenceNameLabel;

		private TabPage webBrowserTabPage;

		private TabPage webServicesTabPage;

		private ToolStrip toolStrip;

		private WebBrowser webBrowser;

		private TabControl tabControl;

		private ToolStripButton goButton;

		private ToolStripComboBox urlComboBox;

		private ToolStripButton stopButton;

		private ToolStripButton refreshButton;

		private ToolStripButton forwardButton;

		private ToolStripButton backButton;

		private WebServicesView webServicesView;

		public string NamespacePrefix
		{
			get
			{
				return this.namespacePrefix;
			}
			set
			{
				this.namespacePrefix = value;
			}
		}

		public WebReference WebReference
		{
			get
			{
				return this.webReference;
			}
		}

		public string WebReferenceFileName
		{
			get
			{
				return this.txtName.Text;
			}
		}

		private bool IsValidNamespace
		{
			get
			{
				bool valid = false;
				if (this.namespaceTextBox.Text.Length > 0)
				{
					char ch = this.namespaceTextBox.Text[0];
					if (char.IsLetter(ch) || ch == '_')
					{
						valid = true;
						for (int i = 1; i < this.namespaceTextBox.Text.Length; i++)
						{
							ch = this.namespaceTextBox.Text[i];
							if (!char.IsLetterOrDigit(ch) && ch != '.' && ch != '_')
							{
								valid = false;
								break;
							}
						}
					}
				}
				return valid;
			}
		}

		private bool IsValidReferenceName
		{
			get
			{
				bool result;
				if (this.referenceNameTextBox.Text.Length > 0)
				{
					if (this.referenceNameTextBox.Text.IndexOf('\\') == -1)
					{
						if (!this.ContainsInvalidDirectoryChar(this.referenceNameTextBox.Text))
						{
							result = true;
							return result;
						}
					}
				}
				result = false;
				return result;
			}
		}

		public AddWebReferenceDialog(IProject project, ScriptLanguage Lang, StringCollection CreatedFileNames)
		{
			this._Language = Lang;
			this._CreatedFileNames = CreatedFileNames;
			this.InitializeComponent();
			this.txtName.Text = this.GenerateValidFileName();
			this.AddMruList();
			this.AddWebReferenceDialogResize(null, null);
			this.project = project;
		}

		private void InitializeComponent()
		{
			this.toolStrip = new ToolStrip();
			this.backButton = new ToolStripButton();
			this.forwardButton = new ToolStripButton();
			this.refreshButton = new ToolStripButton();
			this.stopButton = new ToolStripButton();
			this.urlComboBox = new ToolStripComboBox();
			this.goButton = new ToolStripButton();
			this.tabControl = new TabControl();
			this.webBrowserTabPage = new TabPage();
			this.webBrowser = new WebBrowser();
			this.webServicesTabPage = new TabPage();
			this.webServicesView = new WebServicesView();
			this.referenceNameLabel = new Label();
			this.referenceNameTextBox = new TextBox();
			this.addButton = new Button();
			this.cancelButton = new Button();
			this.namespaceTextBox = new TextBox();
			this.namespaceLabel = new Label();
			this.label2 = new Label();
			this.txtName = new TextBox();
			this.toolStrip.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.webBrowserTabPage.SuspendLayout();
			this.webServicesTabPage.SuspendLayout();
			base.SuspendLayout();
			this.toolStrip.CanOverflow = false;
			this.toolStrip.Items.AddRange(new ToolStripItem[]
			{
				this.backButton,
				this.forwardButton,
				this.refreshButton,
				this.stopButton,
				this.urlComboBox,
				this.goButton
			});
			this.toolStrip.Location = new Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new Size(543, 25);
			this.toolStrip.Stretch = true;
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "toolStrip";
			this.toolStrip.PreviewKeyDown += new PreviewKeyDownEventHandler(this.ToolStripPreviewKeyDown);
			this.toolStrip.Enter += new EventHandler(this.ToolStripEnter);
			this.toolStrip.Leave += new EventHandler(this.ToolStripLeave);
			this.backButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.backButton.Enabled = false;
			this.backButton.Image = Resources.Icons_16x16_BrowserBefore;
			this.backButton.ImageTransparentColor = Color.Magenta;
			this.backButton.Name = "backButton";
			this.backButton.Size = new Size(23, 22);
			this.backButton.Text = "Back";
			this.backButton.Click += new EventHandler(this.BackButtonClick);
			this.forwardButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.forwardButton.Enabled = false;
			this.forwardButton.Image = Resources.Icons_16x16_BrowserAfter;
			this.forwardButton.ImageTransparentColor = Color.Magenta;
			this.forwardButton.Name = "forwardButton";
			this.forwardButton.Size = new Size(23, 22);
			this.forwardButton.Text = "forward";
			this.forwardButton.Click += new EventHandler(this.ForwardButtonClick);
			this.refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.refreshButton.Image = Resources.Icons_16x16_BrowserRefresh;
			this.refreshButton.ImageTransparentColor = Color.Magenta;
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new Size(23, 22);
			this.refreshButton.Text = "Refresh";
			this.refreshButton.Click += new EventHandler(this.RefreshButtonClick);
			this.stopButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.stopButton.Enabled = false;
			this.stopButton.Image = Resources.Icons_16x16_BrowserCancel;
			this.stopButton.ImageTransparentColor = Color.Magenta;
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new Size(23, 22);
			this.stopButton.Text = "Stop";
			this.stopButton.ToolTipText = "Stop";
			this.stopButton.Click += new EventHandler(this.StopButtonClick);
			this.urlComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
			this.urlComboBox.AutoCompleteSource = AutoCompleteSource.AllUrl;
			this.urlComboBox.AutoSize = false;
			this.urlComboBox.FlatStyle = FlatStyle.Standard;
			this.urlComboBox.Name = "urlComboBox";
			this.urlComboBox.Size = new Size(361, 21);
			this.urlComboBox.KeyDown += new KeyEventHandler(this.UrlComboBoxKeyDown);
			this.urlComboBox.SelectedIndexChanged += new EventHandler(this.UrlComboBoxSelectedIndexChanged);
			this.goButton.Image = Resources.Icons_16x16_RunProgramIcon;
			this.goButton.ImageTransparentColor = Color.Magenta;
			this.goButton.Name = "goButton";
			this.goButton.Size = new Size(40, 22);
			this.goButton.Text = "Go";
			this.goButton.Click += new EventHandler(this.GoButtonClick);
			this.tabControl.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.tabControl.Controls.Add(this.webBrowserTabPage);
			this.tabControl.Controls.Add(this.webServicesTabPage);
			this.tabControl.Location = new Point(0, 28);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new Size(543, 181);
			this.tabControl.TabIndex = 1;
			this.webBrowserTabPage.Controls.Add(this.webBrowser);
			this.webBrowserTabPage.Location = new Point(4, 22);
			this.webBrowserTabPage.Name = "webBrowserTabPage";
			this.webBrowserTabPage.Padding = new Padding(3);
			this.webBrowserTabPage.Size = new Size(535, 155);
			this.webBrowserTabPage.TabIndex = 0;
			this.webBrowserTabPage.Text = "WSDL";
			this.webBrowserTabPage.UseVisualStyleBackColor = true;
			this.webBrowser.Dock = DockStyle.Fill;
			this.webBrowser.Location = new Point(3, 3);
			this.webBrowser.MinimumSize = new Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new Size(529, 149);
			this.webBrowser.TabIndex = 0;
			this.webBrowser.TabStop = false;
			this.webBrowser.CanGoForwardChanged += new EventHandler(this.WebBrowserCanGoForwardChanged);
			this.webBrowser.CanGoBackChanged += new EventHandler(this.WebBrowserCanGoBackChanged);
			this.webBrowser.Navigated += new WebBrowserNavigatedEventHandler(this.WebBrowserNavigated);
			this.webBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.WebBrowserNavigating);
			this.webServicesTabPage.Controls.Add(this.webServicesView);
			this.webServicesTabPage.Location = new Point(4, 22);
			this.webServicesTabPage.Name = "webServicesTabPage";
			this.webServicesTabPage.Padding = new Padding(3);
			this.webServicesTabPage.Size = new Size(535, 155);
			this.webServicesTabPage.TabIndex = 1;
			this.webServicesTabPage.Text = "Available Web Services";
			this.webServicesTabPage.UseVisualStyleBackColor = true;
			this.webServicesView.Dock = DockStyle.Fill;
			this.webServicesView.Location = new Point(3, 3);
			this.webServicesView.Name = "webServicesView";
			this.webServicesView.Size = new Size(529, 149);
			this.webServicesView.TabIndex = 0;
			this.referenceNameLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.referenceNameLabel.Location = new Point(-26, 263);
			this.referenceNameLabel.Name = "referenceNameLabel";
			this.referenceNameLabel.Size = new Size(20, 13);
			this.referenceNameLabel.TabIndex = 2;
			this.referenceNameLabel.Text = "&Reference Name:";
			this.referenceNameLabel.UseCompatibleTextRendering = true;
			this.referenceNameLabel.Visible = false;
			this.referenceNameTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.referenceNameTextBox.Location = new Point(0, 300);
			this.referenceNameTextBox.Name = "referenceNameTextBox";
			this.referenceNameTextBox.Size = new Size(12, 20);
			this.referenceNameTextBox.TabIndex = 4;
			this.referenceNameTextBox.Visible = false;
			this.addButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.addButton.Enabled = false;
			this.addButton.Location = new Point(468, 226);
			this.addButton.Name = "addButton";
			this.addButton.Size = new Size(73, 21);
			this.addButton.TabIndex = 6;
			this.addButton.Text = "&Add";
			this.addButton.UseCompatibleTextRendering = true;
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new EventHandler(this.AddButtonClick);
			this.cancelButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(468, 251);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(73, 21);
			this.cancelButton.TabIndex = 7;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseCompatibleTextRendering = true;
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new EventHandler(this.CancelButtonClick);
			this.namespaceTextBox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.namespaceTextBox.Location = new Point(129, 253);
			this.namespaceTextBox.Name = "namespaceTextBox";
			this.namespaceTextBox.Size = new Size(333, 20);
			this.namespaceTextBox.TabIndex = 5;
			this.namespaceLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.namespaceLabel.Location = new Point(11, 252);
			this.namespaceLabel.Name = "namespaceLabel";
			this.namespaceLabel.Size = new Size(128, 20);
			this.namespaceLabel.TabIndex = 3;
			this.namespaceLabel.Text = "&Namespace:";
			this.namespaceLabel.UseCompatibleTextRendering = true;
			this.label2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.label2.Location = new Point(11, 227);
			this.label2.Name = "label2";
			this.label2.Size = new Size(128, 20);
			this.label2.TabIndex = 9;
			this.label2.Text = "&File Name:";
			this.label2.UseCompatibleTextRendering = true;
			this.txtName.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.txtName.Location = new Point(129, 226);
			this.txtName.Name = "txtName";
			this.txtName.Size = new Size(333, 20);
			this.txtName.TabIndex = 3;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(543, 285);
			base.Controls.Add(this.txtName);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.namespaceTextBox);
			base.Controls.Add(this.namespaceLabel);
			base.Controls.Add(this.addButton);
			base.Controls.Add(this.referenceNameTextBox);
			base.Controls.Add(this.referenceNameLabel);
			base.Controls.Add(this.tabControl);
			base.Controls.Add(this.toolStrip);
			this.MinimumSize = new Size(300, 200);
			base.Name = "AddWebReferenceDialog";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Add Web Reference";
			base.Resize += new EventHandler(this.AddWebReferenceDialogResize);
			base.FormClosing += new FormClosingEventHandler(this.AddWebReferenceDialogFormClosing);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.webBrowserTabPage.ResumeLayout(false);
			this.webServicesTabPage.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void AddMruList()
		{
			try
			{
				RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Internet Explorer\\TypedURLs");
				if (key != null)
				{
					string[] valueNames = key.GetValueNames();
					for (int i = 0; i < valueNames.Length; i++)
					{
						string name = valueNames[i];
						this.urlComboBox.Items.Add((string)key.GetValue(name));
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private string GenerateValidFileName()
		{
			string baseName = "WebReference";
			bool found = false;
			int filecounter = 1;
			string filename = "";
			while (!found)
			{
				filename = baseName + filecounter.ToString() + ((this._Language == ScriptLanguage.CSharp) ? ".cs" : ".vb");
				if (this._CreatedFileNames == null)
				{
					break;
				}
				if (!this._CreatedFileNames.Contains(filename))
				{
					break;
				}
				filecounter++;
			}
			return filename;
		}

		private void ToolStripPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Tab)
			{
				if (this.goButton.Selected && e.Modifiers != Keys.Shift)
				{
					this.toolStrip.TabStop = true;
				}
				else if (this.backButton.Selected && e.Modifiers == Keys.Shift)
				{
					this.toolStrip.TabStop = true;
				}
			}
		}

		private void ToolStripEnter(object sender, EventArgs e)
		{
			this.toolStrip.TabStop = false;
		}

		private void ToolStripLeave(object sender, EventArgs e)
		{
			this.toolStrip.TabStop = true;
		}

		private void BackButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.webBrowser.GoBack();
			}
			catch (Exception)
			{
			}
		}

		private void ForwardButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.webBrowser.GoForward();
			}
			catch (Exception)
			{
			}
		}

		private void StopButtonClick(object sender, EventArgs e)
		{
			this.webBrowser.Stop();
			this.StopDiscovery();
			this.addButton.Enabled = false;
		}

		private void RefreshButtonClick(object sender, EventArgs e)
		{
			this.webBrowser.Refresh();
		}

		private void GoButtonClick(object sender, EventArgs e)
		{
			this.BrowseUrl(this.urlComboBox.Text);
		}

		private void BrowseUrl(string url)
		{
			this.webBrowser.Focus();
			this.webBrowser.Navigate(url);
		}

		private void CancelButtonClick(object sender, EventArgs e)
		{
			base.Close();
		}

		private void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			this.stopButton.Enabled = true;
			this.webServicesView.Clear();
		}

		private void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			this.Cursor = Cursors.Default;
			this.stopButton.Enabled = false;
			this.urlComboBox.Text = this.webBrowser.Url.ToString();
			this.StartDiscovery(e.Url);
		}

		private void WebBrowserCanGoForwardChanged(object sender, EventArgs e)
		{
			this.forwardButton.Enabled = this.webBrowser.CanGoForward;
		}

		private void WebBrowserCanGoBackChanged(object sender, EventArgs e)
		{
			this.backButton.Enabled = this.webBrowser.CanGoBack;
		}

		private string GetDefaultNamespace()
		{
			string result;
			if (this.namespacePrefix.Length > 0 && this.discoveryUri != null)
			{
				result = this.namespacePrefix + "." + this.discoveryUri.Host;
			}
			else if (this.discoveryUri != null)
			{
				result = this.discoveryUri.Host;
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private string GetReferenceName()
		{
			string result;
			if (this.discoveryUri != null)
			{
				result = this.discoveryUri.Host;
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private bool ContainsInvalidDirectoryChar(string item)
		{
			char[] invalidPathChars = Path.GetInvalidPathChars();
			bool result;
			for (int i = 0; i < invalidPathChars.Length; i++)
			{
				char ch = invalidPathChars[i];
				if (item.IndexOf(ch) >= 0)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private void StartDiscovery(Uri uri)
		{
			this.StartDiscovery(uri, new DiscoveryNetworkCredential(CredentialCache.DefaultNetworkCredentials, "Default"));
		}

		private void StartDiscovery(Uri uri, DiscoveryNetworkCredential credential)
		{
			this.StopDiscovery();
			this.discoveryUri = uri;
			AddWebReferenceDialog.DiscoverAnyAsync asyncDelegate = new AddWebReferenceDialog.DiscoverAnyAsync(this.discoveryClientProtocol.DiscoverAny);
			AsyncCallback callback = new AsyncCallback(this.DiscoveryCompleted);
			this.discoveryClientProtocol.Credentials = credential;
			IAsyncResult result = asyncDelegate.BeginInvoke(uri.AbsoluteUri, callback, new AsyncDiscoveryState(this.discoveryClientProtocol, uri, credential));
		}

		private void DiscoveryCompleted(IAsyncResult result)
		{
			AsyncDiscoveryState state = (AsyncDiscoveryState)result.AsyncState;
			WebServiceDiscoveryClientProtocol protocol = state.Protocol;
			bool wanted = false;
			lock (this)
			{
				wanted = object.ReferenceEquals(this.discoveryClientProtocol, protocol);
			}
			if (wanted)
			{
				AddWebReferenceDialog.DiscoveredWebServicesHandler handler = new AddWebReferenceDialog.DiscoveredWebServicesHandler(this.DiscoveredWebServices);
				try
				{
					AddWebReferenceDialog.DiscoverAnyAsync asyncDelegate = (AddWebReferenceDialog.DiscoverAnyAsync)((AsyncResult)result).AsyncDelegate;
					DiscoveryDocument doc = asyncDelegate.EndInvoke(result);
					if (!state.Credential.IsDefaultAuthenticationType)
					{
						this.AddCredential(state.Uri, state.Credential);
					}
					base.Invoke(handler, new object[]
					{
						protocol
					});
				}
				catch
				{
					if (protocol.IsAuthenticationRequired)
					{
						HttpAuthenticationHeader authHeader = protocol.GetAuthenticationHeader();
						AddWebReferenceDialog.AuthenticationHandler authHandler = new AddWebReferenceDialog.AuthenticationHandler(this.AuthenticateUser);
						base.Invoke(authHandler, new object[]
						{
							state.Uri,
							authHeader.AuthenticationType
						});
					}
					else
					{
						Delegate arg_113_1 = handler;
						object[] args = new object[1];
						base.Invoke(arg_113_1, args);
					}
				}
			}
		}

		private void StopDiscovery()
		{
			lock (this)
			{
				if (this.discoveryClientProtocol != null)
				{
					try
					{
						this.discoveryClientProtocol.Abort();
					}
					catch (NotImplementedException)
					{
					}
					catch (ObjectDisposedException)
					{
					}
					this.discoveryClientProtocol.Dispose();
				}
				this.discoveryClientProtocol = new WebServiceDiscoveryClientProtocol();
			}
		}

		private void AddWebReferenceDialogFormClosing(object sender, FormClosingEventArgs e)
		{
			this.StopDiscovery();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.urlComboBox.Focus();
		}

		private ServiceDescriptionCollection GetServiceDescriptions(DiscoveryClientProtocol protocol)
		{
			ServiceDescriptionCollection services = new ServiceDescriptionCollection();
			protocol.ResolveOneLevel();
			foreach (DictionaryEntry entry in protocol.References)
			{
				ContractReference contractRef = entry.Value as ContractReference;
				if (contractRef != null)
				{
					services.Add(contractRef.Contract);
				}
			}
			return services;
		}

		private void DiscoveredWebServices(DiscoveryClientProtocol protocol)
		{
			if (protocol != null)
			{
				this.addButton.Enabled = true;
				this.namespaceTextBox.Text = this.GetDefaultNamespace();
				this.referenceNameTextBox.Text = this.GetReferenceName();
				this.webServicesView.Add(this.GetServiceDescriptions(protocol));
				this.webReference = new WebReference(this.project, this.discoveryUri.AbsoluteUri, this.referenceNameTextBox.Text, this.namespaceTextBox.Text, protocol);
				this.tabControl.SelectedTab = this.webServicesTabPage;
			}
			else
			{
				this.webReference = null;
				this.addButton.Enabled = false;
				this.webServicesView.Clear();
			}
		}

		private void UrlComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			this.BrowseUrl(this.urlComboBox.Text);
		}

		private void UrlComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return && this.urlComboBox.Text.Length > 0)
			{
				this.BrowseUrl(this.urlComboBox.Text);
			}
		}

		private void AddWebReferenceDialogResize(object sender, EventArgs e)
		{
			int width = this.toolStrip.ClientSize.Width;
			foreach (ToolStripItem item in this.toolStrip.Items)
			{
				if (item != this.urlComboBox)
				{
					width -= item.Width + 8;
				}
			}
			this.urlComboBox.Width = width;
		}

		private void AddButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (!this.IsValidReferenceName)
				{
					MessageBox.Show("Invalid Reference Name Error");
				}
				else if (!this.IsValidNamespace)
				{
					MessageBox.Show("Invalid Namespace Error");
				}
				else
				{
					if (this._CreatedFileNames != null)
					{
						if (this._CreatedFileNames.Contains(this.txtName.Text))
						{
							MessageBox.Show(this, "A file with the name " + this.txtName.Text + " already exists in the current project. Please give a unique name to the item you are adding , or delete the existing item first.", "AIMS Script Editor");
							return;
						}
					}
					this.webReference.Name = this.referenceNameTextBox.Text;
					this.webReference.ProxyNamespace = this.namespaceTextBox.Text;
					base.DialogResult = DialogResult.OK;
					base.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void AuthenticateUser(Uri uri, string authenticationType)
		{
			DiscoveryNetworkCredential credential = (DiscoveryNetworkCredential)this.credentialCache.GetCredential(uri, authenticationType);
			if (credential != null)
			{
				this.StartDiscovery(uri, credential);
			}
			else
			{
				using (UserCredentialsDialog credentialsForm = new UserCredentialsDialog(uri.ToString(), authenticationType))
				{
					if (DialogResult.OK == credentialsForm.ShowDialog())
					{
						this.StartDiscovery(uri, credentialsForm.Credential);
					}
				}
			}
		}

		private void AddCredential(Uri uri, DiscoveryNetworkCredential credential)
		{
			NetworkCredential matchedCredential = this.credentialCache.GetCredential(uri, credential.AuthenticationType);
			if (matchedCredential != null)
			{
				this.credentialCache.Remove(uri, credential.AuthenticationType);
			}
			this.credentialCache.Add(uri, credential.AuthenticationType, credential);
		}
	}
}

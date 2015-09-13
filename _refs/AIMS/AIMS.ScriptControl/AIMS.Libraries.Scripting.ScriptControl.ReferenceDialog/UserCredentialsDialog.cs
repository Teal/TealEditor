using System;
using System.Drawing;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class UserCredentialsDialog : Form
	{
		private string authenticationType = string.Empty;

		private Label infoLabel;

		private TextBox passwordTextBox;

		private Label userNameLabel;

		private Button cancelButton;

		private Button okButton;

		private Label url;

		private TextBox domainTextBox;

		private TextBox userTextBox;

		private Label domainLabel;

		private Label passwordLabel;

		private Label urlLabel;

		public DiscoveryNetworkCredential Credential
		{
			get
			{
				return new DiscoveryNetworkCredential(this.userTextBox.Text, this.passwordTextBox.Text, this.domainTextBox.Text, this.authenticationType);
			}
		}

		public UserCredentialsDialog(string url, string authenticationType)
		{
			this.InitializeComponent();
			this.url.Text = url;
			this.authenticationType = authenticationType;
			this.AddStringResources();
		}

		private void InitializeComponent()
		{
			this.urlLabel = new Label();
			this.userNameLabel = new Label();
			this.passwordLabel = new Label();
			this.domainLabel = new Label();
			this.userTextBox = new TextBox();
			this.passwordTextBox = new TextBox();
			this.domainTextBox = new TextBox();
			this.url = new Label();
			this.okButton = new Button();
			this.cancelButton = new Button();
			this.infoLabel = new Label();
			base.SuspendLayout();
			this.urlLabel.Location = new Point(10, 59);
			this.urlLabel.Name = "urlLabel";
			this.urlLabel.Size = new Size(91, 23);
			this.urlLabel.TabIndex = 0;
			this.urlLabel.Text = "Url:";
			this.urlLabel.UseCompatibleTextRendering = true;
			this.userNameLabel.Location = new Point(10, 88);
			this.userNameLabel.Name = "userNameLabel";
			this.userNameLabel.Size = new Size(91, 23);
			this.userNameLabel.TabIndex = 1;
			this.userNameLabel.Text = "&User name:";
			this.userNameLabel.UseCompatibleTextRendering = true;
			this.passwordLabel.Location = new Point(10, 115);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new Size(91, 23);
			this.passwordLabel.TabIndex = 3;
			this.passwordLabel.Text = "&Password:";
			this.passwordLabel.UseCompatibleTextRendering = true;
			this.domainLabel.Location = new Point(10, 142);
			this.domainLabel.Name = "domainLabel";
			this.domainLabel.Size = new Size(91, 23);
			this.domainLabel.TabIndex = 5;
			this.domainLabel.Text = "&Domain:";
			this.domainLabel.UseCompatibleTextRendering = true;
			this.userTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.userTextBox.Location = new Point(93, 85);
			this.userTextBox.Name = "userTextBox";
			this.userTextBox.Size = new Size(187, 21);
			this.userTextBox.TabIndex = 2;
			this.passwordTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.passwordTextBox.Location = new Point(93, 112);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new Size(187, 21);
			this.passwordTextBox.TabIndex = 4;
			this.domainTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.domainTextBox.Location = new Point(93, 139);
			this.domainTextBox.Name = "domainTextBox";
			this.domainTextBox.Size = new Size(187, 21);
			this.domainTextBox.TabIndex = 6;
			this.url.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.url.BorderStyle = BorderStyle.Fixed3D;
			this.url.Location = new Point(93, 57);
			this.url.Name = "url";
			this.url.Size = new Size(187, 21);
			this.url.TabIndex = 9;
			this.url.UseCompatibleTextRendering = true;
			this.okButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.okButton.DialogResult = DialogResult.OK;
			this.okButton.Location = new Point(146, 166);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(64, 26);
			this.okButton.TabIndex = 7;
			this.okButton.Text = "OK";
			this.okButton.UseCompatibleTextRendering = true;
			this.okButton.UseVisualStyleBackColor = true;
			this.cancelButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(216, 166);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(64, 26);
			this.cancelButton.TabIndex = 8;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseCompatibleTextRendering = true;
			this.cancelButton.UseVisualStyleBackColor = true;
			this.infoLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.infoLabel.Location = new Point(12, 9);
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.Size = new Size(267, 48);
			this.infoLabel.TabIndex = 10;
			this.infoLabel.Text = "Please supply the credentials to access the specified url.";
			this.infoLabel.UseCompatibleTextRendering = true;
			base.AcceptButton = this.okButton;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new Size(292, 202);
			base.Controls.Add(this.infoLabel);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.url);
			base.Controls.Add(this.domainTextBox);
			base.Controls.Add(this.passwordTextBox);
			base.Controls.Add(this.userTextBox);
			base.Controls.Add(this.domainLabel);
			base.Controls.Add(this.passwordLabel);
			base.Controls.Add(this.userNameLabel);
			base.Controls.Add(this.urlLabel);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new Size(300, 236);
			base.Name = "UserCredentialsDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Discovery Credential";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void AddStringResources()
		{
			this.Text = "Dialog Title";
			this.infoLabel.Text = "Information";
			this.urlLabel.Text = "Url";
			this.userNameLabel.Text = "User Name";
			this.passwordLabel.Text = "Password";
			this.domainLabel.Text = "Domain";
			this.cancelButton.Text = "Cancel";
			this.okButton.Text = "Ok";
		}
	}
}

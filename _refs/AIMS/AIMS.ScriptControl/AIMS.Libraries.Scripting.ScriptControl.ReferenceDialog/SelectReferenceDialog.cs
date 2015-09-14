using AIMS.Libraries.Scripting.ScriptControl.Project;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class SelectReferenceDialog : Form, ISelectReferenceDialog
	{
		private ListView referencesListView;

		private Button selectButton;

		private Button removeButton;

		private TabPage gacTabPage;

		private TabPage webTabPage;

		private TabPage browserTabPage;

		private TabPage comTabPage;

		private Label referencesLabel;

		private ColumnHeader referenceHeader;

		private ColumnHeader typeHeader;

		private ColumnHeader locationHeader;

		private TabControl referenceTabControl;

		private Button okButton;

		private Button cancelButton;

		private Button helpButton;

		private Container components = null;

		private IProject configureProject;

		public ArrayList ReferenceInformations
		{
			get
			{
				ArrayList referenceInformations = new ArrayList();
				foreach (ListViewItem item in this.referencesListView.Items)
				{
					Debug.Assert(item.Tag != null);
					referenceInformations.Add(item.Tag);
				}
				return referenceInformations;
			}
		}

		public IProject ConfigureProject
		{
			get
			{
				return this.configureProject;
			}
			set
			{
				this.referencesListView.Items.Clear();
				this.configureProject = value;
			}
		}

		public SelectReferenceDialog(IProject configureProject)
		{
			this.configureProject = configureProject;
			this.InitializeComponent();
			this.gacTabPage.Controls.Add(new GacReferencePanel(this));
			this.browserTabPage.Controls.Add(new AssemblyReferencePanel(this));
			this.comTabPage.Controls.Add(new COMReferencePanel(this));
		}

		public void AddReference(ReferenceType referenceType, string referenceName, string referenceLocation, object tag)
		{
			foreach (ListViewItem item in this.referencesListView.Items)
			{
				if (referenceLocation == item.SubItems[2].Text && referenceName == item.Text)
				{
					return;
				}
			}
			ListViewItem newItem = new ListViewItem(new string[]
			{
				referenceName,
				referenceType.ToString(),
				referenceLocation
			});
			switch (referenceType)
			{
			case ReferenceType.Assembly:
				newItem.Tag = new ReferenceProjectItem(this.configureProject)
				{
					Include = Path.GetFileNameWithoutExtension(referenceLocation),
					HintPath = FileUtility.GetRelativePath(this.configureProject.Directory, referenceLocation),
					SpecificVersion = false
				};
				break;
			case ReferenceType.Typelib:
				newItem.Tag = new ComReferenceProjectItem(this.configureProject, (TypeLibrary)tag);
				break;
			case ReferenceType.Gac:
				newItem.Tag = new ReferenceProjectItem(this.configureProject, referenceLocation);
				break;
			case ReferenceType.Project:
				newItem.Tag = new ProjectReferenceProjectItem(this.configureProject, (IProject)tag);
				break;
			default:
				throw new NotSupportedException("Unknown reference type:" + referenceType);
			}
			this.referencesListView.Items.Add(newItem);
		}

		private void SelectReference(object sender, EventArgs e)
		{
			IReferencePanel refPanel = (IReferencePanel)this.referenceTabControl.SelectedTab.Controls[0];
			refPanel.AddReference();
		}

		private void OkButtonClick(object sender, EventArgs e)
		{
			if (this.referencesListView.Items.Count == 0)
			{
				this.SelectReference(sender, e);
			}
		}

		private void RemoveReference(object sender, EventArgs e)
		{
			ArrayList itemsToDelete = new ArrayList();
			foreach (ListViewItem item in this.referencesListView.SelectedItems)
			{
				itemsToDelete.Add(item);
			}
			foreach (ListViewItem item in itemsToDelete)
			{
				this.referencesListView.Items.Remove(item);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
				{
					this.components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.referenceTabControl = new TabControl();
			this.gacTabPage = new TabPage();
			this.comTabPage = new TabPage();
			this.webTabPage = new TabPage();
			this.browserTabPage = new TabPage();
			this.referencesListView = new ListView();
			this.referenceHeader = new ColumnHeader();
			this.typeHeader = new ColumnHeader();
			this.locationHeader = new ColumnHeader();
			this.selectButton = new Button();
			this.removeButton = new Button();
			this.referencesLabel = new Label();
			this.okButton = new Button();
			this.cancelButton = new Button();
			this.helpButton = new Button();
			this.referenceTabControl.SuspendLayout();
			base.SuspendLayout();
			this.referenceTabControl.Controls.Add(this.gacTabPage);
			this.referenceTabControl.Controls.Add(this.comTabPage);
			this.referenceTabControl.Controls.Add(this.webTabPage);
			this.referenceTabControl.Controls.Add(this.browserTabPage);
			this.referenceTabControl.Location = new Point(8, 8);
			this.referenceTabControl.Name = "referenceTabControl";
			this.referenceTabControl.SelectedIndex = 0;
			this.referenceTabControl.Size = new Size(472, 224);
			this.referenceTabControl.TabIndex = 0;
			this.gacTabPage.Location = new Point(4, 22);
			this.gacTabPage.Name = "gacTabPage";
			this.gacTabPage.Size = new Size(464, 198);
			this.gacTabPage.TabIndex = 0;
			this.gacTabPage.Text = ".Net";
			this.gacTabPage.UseVisualStyleBackColor = true;
			this.comTabPage.Location = new Point(4, 22);
			this.comTabPage.Name = "comTabPage";
			this.comTabPage.Size = new Size(464, 198);
			this.comTabPage.TabIndex = 2;
			this.comTabPage.Text = "COM";
			this.comTabPage.UseVisualStyleBackColor = true;
			this.webTabPage.Location = new Point(4, 22);
			this.webTabPage.Name = "webTabPage";
			this.webTabPage.Size = new Size(464, 198);
			this.webTabPage.TabIndex = 1;
			this.webTabPage.Text = "Web";
			this.webTabPage.UseVisualStyleBackColor = true;
			this.browserTabPage.Location = new Point(4, 22);
			this.browserTabPage.Name = "browserTabPage";
			this.browserTabPage.Size = new Size(464, 198);
			this.browserTabPage.TabIndex = 2;
			this.browserTabPage.Text = "Browse";
			this.browserTabPage.UseVisualStyleBackColor = true;
			this.referencesListView.Columns.AddRange(new ColumnHeader[]
			{
				this.referenceHeader,
				this.typeHeader,
				this.locationHeader
			});
			this.referencesListView.FullRowSelect = true;
			this.referencesListView.Location = new Point(8, 256);
			this.referencesListView.Name = "referencesListView";
			this.referencesListView.Size = new Size(472, 97);
			this.referencesListView.TabIndex = 3;
			this.referencesListView.UseCompatibleStateImageBehavior = false;
			this.referencesListView.View = View.Details;
			this.referenceHeader.Text = "Reference";
			this.referenceHeader.Width = 183;
			this.typeHeader.Text = "Type";
			this.typeHeader.Width = 57;
			this.locationHeader.Text = "Location";
			this.locationHeader.Width = 228;
			this.selectButton.FlatStyle = FlatStyle.System;
			this.selectButton.Location = new Point(488, 32);
			this.selectButton.Name = "selectButton";
			this.selectButton.Size = new Size(75, 23);
			this.selectButton.TabIndex = 1;
			this.selectButton.Text = "Select";
			this.selectButton.Click += new EventHandler(this.SelectReference);
			this.removeButton.FlatStyle = FlatStyle.System;
			this.removeButton.Location = new Point(488, 256);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new Size(75, 23);
			this.removeButton.TabIndex = 4;
			this.removeButton.Text = "Remove";
			this.removeButton.Click += new EventHandler(this.RemoveReference);
			this.referencesLabel.Location = new Point(8, 240);
			this.referencesLabel.Name = "referencesLabel";
			this.referencesLabel.Size = new Size(472, 16);
			this.referencesLabel.TabIndex = 2;
			this.referencesLabel.Text = "References";
			this.okButton.DialogResult = DialogResult.OK;
			this.okButton.Location = new Point(312, 368);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(75, 23);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "Ok";
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.FlatStyle = FlatStyle.System;
			this.cancelButton.Location = new Point(400, 368);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(75, 23);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			this.helpButton.FlatStyle = FlatStyle.System;
			this.helpButton.Location = new Point(488, 368);
			this.helpButton.Name = "helpButton";
			this.helpButton.Size = new Size(75, 23);
			this.helpButton.TabIndex = 7;
			this.helpButton.Text = "Help";
			base.AcceptButton = this.okButton;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new Size(570, 399);
			base.Controls.Add(this.helpButton);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.referencesLabel);
			base.Controls.Add(this.removeButton);
			base.Controls.Add(this.selectButton);
			base.Controls.Add(this.referencesListView);
			base.Controls.Add(this.referenceTabControl);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SelectReferenceDialog";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Add Reference to AIMS script";
			this.referenceTabControl.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}

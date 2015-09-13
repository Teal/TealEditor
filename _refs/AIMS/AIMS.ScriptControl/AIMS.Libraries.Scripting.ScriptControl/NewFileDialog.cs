using AIMS.Libraries.Scripting.ScriptControl.Project;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class NewFileDialog : Form
	{
		private IContainer components = null;

		private Panel pnlContainer;

		private Button cmdCancel;

		private Button cmdAdd;

		private TextBox txtName;

		private Label label1;

		private TextBox txtHint;

		private ListView listView1;

		private Label label2;

		private ImageList ilImages;

		private ScriptLanguage _Language = ScriptLanguage.CSharp;

		private StringCollection _CreatedFileNames = null;

		private SelectedItemType selectedItemType = SelectedItemType.None;

		public string FileName
		{
			get
			{
				string name = this.txtName.Text;
				string result;
				if (name.Length > 0)
				{
					if (name.LastIndexOf('.') > 0)
					{
						result = name;
					}
					else
					{
						result = name + ((this._Language == ScriptLanguage.CSharp) ? ".cs" : ".vb");
					}
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		public SelectedItemType SelectedItemType
		{
			get
			{
				return this.selectedItemType;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(NewFileDialog));
			this.pnlContainer = new Panel();
			this.label2 = new Label();
			this.cmdCancel = new Button();
			this.cmdAdd = new Button();
			this.txtName = new TextBox();
			this.label1 = new Label();
			this.txtHint = new TextBox();
			this.listView1 = new ListView();
			this.ilImages = new ImageList(this.components);
			this.pnlContainer.SuspendLayout();
			base.SuspendLayout();
			this.pnlContainer.Controls.Add(this.label2);
			this.pnlContainer.Controls.Add(this.cmdCancel);
			this.pnlContainer.Controls.Add(this.cmdAdd);
			this.pnlContainer.Controls.Add(this.txtName);
			this.pnlContainer.Controls.Add(this.label1);
			this.pnlContainer.Controls.Add(this.txtHint);
			this.pnlContainer.Controls.Add(this.listView1);
			this.pnlContainer.Dock = DockStyle.Fill;
			this.pnlContainer.Location = new Point(0, 0);
			this.pnlContainer.Name = "pnlContainer";
			this.pnlContainer.Size = new Size(324, 213);
			this.pnlContainer.TabIndex = 0;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(4, 4);
			this.label2.Name = "label2";
			this.label2.Size = new Size(56, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Templates";
			this.cmdCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cmdCancel.Location = new Point(266, 186);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new Size(55, 23);
			this.cmdCancel.TabIndex = 7;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.UseVisualStyleBackColor = true;
			this.cmdCancel.Click += new EventHandler(this.cmdCancel_Click);
			this.cmdAdd.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cmdAdd.Location = new Point(205, 186);
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.Size = new Size(55, 23);
			this.cmdAdd.TabIndex = 6;
			this.cmdAdd.Text = "Add";
			this.cmdAdd.UseVisualStyleBackColor = true;
			this.cmdAdd.Click += new EventHandler(this.cmdAdd_Click);
			this.txtName.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.txtName.Location = new Point(46, 186);
			this.txtName.Name = "txtName";
			this.txtName.Size = new Size(153, 20);
			this.txtName.TabIndex = 5;
			this.label1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(4, 189);
			this.label1.Name = "label1";
			this.label1.Size = new Size(35, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Name";
			this.txtHint.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.txtHint.BackColor = SystemColors.Control;
			this.txtHint.Location = new Point(3, 159);
			this.txtHint.Name = "txtHint";
			this.txtHint.Size = new Size(319, 20);
			this.txtHint.TabIndex = 3;
			this.listView1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.listView1.LargeImageList = this.ilImages;
			this.listView1.Location = new Point(3, 25);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowGroups = false;
			this.listView1.Size = new Size(319, 127);
			this.listView1.TabIndex = 2;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
			this.ilImages.ImageStream = (ImageListStreamer)resources.GetObject("ilImages.ImageStream");
			this.ilImages.TransparentColor = Color.Transparent;
			this.ilImages.Images.SetKeyName(0, "Code_ClassCS.ico");
			this.ilImages.Images.SetKeyName(1, "Code_CodeFileCS.ico");
			this.ilImages.Images.SetKeyName(2, "CodeClass.ico");
			this.ilImages.Images.SetKeyName(3, "Code_CodeFile.ico");
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(324, 213);
			base.Controls.Add(this.pnlContainer);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "NewFileDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = SizeGripStyle.Show;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Add New Item";
			this.pnlContainer.ResumeLayout(false);
			this.pnlContainer.PerformLayout();
			base.ResumeLayout(false);
		}

		public NewFileDialog(ScriptLanguage Lang, StringCollection CreatedFileNames)
		{
			this._Language = Lang;
			this._CreatedFileNames = CreatedFileNames;
			this.InitializeComponent();
			this.InitilizeListView(Lang);
		}

		private void InitilizeListView(ScriptLanguage Lang)
		{
			if (this._Language == ScriptLanguage.CSharp)
			{
				ListViewItem listViewItem = new ListViewItem("Class", "Code_ClassCS.ico");
				listViewItem.ToolTipText = "An empty class definiton.";
				ListViewItem listViewItem2 = new ListViewItem("Interface", "Code_CodeFileCS.ico");
				listViewItem2.ToolTipText = "An empty interface definition.";
				listViewItem.Selected = true;
				this.listView1.Items.AddRange(new ListViewItem[]
				{
					listViewItem,
					listViewItem2
				});
			}
			else
			{
				ListViewItem listViewItem = new ListViewItem("Class", "CodeClass.ico");
				listViewItem.ToolTipText = "An empty class definiton.";
				ListViewItem listViewItem2 = new ListViewItem("Interface", "Code_CodeFile.ico");
				listViewItem2.ToolTipText = "An empty interface definition.";
				listViewItem.Selected = true;
				this.listView1.Items.AddRange(new ListViewItem[]
				{
					listViewItem,
					listViewItem2
				});
			}
		}

		private string GenerateValidFileName(ListViewItem lvItem)
		{
			string baseName = "";
			string text = lvItem.Text;
			if (text != null)
			{
				if (!(text == "Class"))
				{
					if (text == "Interface")
					{
						baseName = "Interface";
						this.selectedItemType = SelectedItemType.Interface;
					}
				}
				else
				{
					baseName = "Class";
					this.selectedItemType = SelectedItemType.Class;
				}
			}
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

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.listView1.SelectedItems.Count > 0)
			{
				ListViewItem lvItem = this.listView1.SelectedItems[0];
				this.txtHint.Text = lvItem.ToolTipText;
				this.txtName.Text = this.GenerateValidFileName(lvItem);
			}
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			this.txtName.Text = "";
			this.selectedItemType = SelectedItemType.None;
			base.Hide();
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			if (this._CreatedFileNames != null)
			{
				if (this._CreatedFileNames.Contains(this.txtName.Text))
				{
					MessageBox.Show(this, "A file with the name " + this.txtName.Text + " already exists in the current project. Please give a unique name to the item you are adding , or delete the existing item first.", "AIMS Script Editor");
					return;
				}
			}
			base.Hide();
		}

		public static string GetDefaultNamespace(IProject project, string fileName)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			string relPath = FileUtility.GetRelativePath(project.Directory, Path.GetDirectoryName(project.Directory + fileName));
			string[] subdirs = relPath.Split(new char[]
			{
				Path.DirectorySeparatorChar,
				Path.AltDirectorySeparatorChar
			});
			StringBuilder standardNameSpace = new StringBuilder(project.RootNamespace);
			string[] array = subdirs;
			for (int i = 0; i < array.Length; i++)
			{
				string subdir = array[i];
				if (!(subdir == ".") && !(subdir == "..") && subdir.Length != 0)
				{
					if (!subdir.Equals("src", StringComparison.OrdinalIgnoreCase))
					{
						if (!subdir.Equals("source", StringComparison.OrdinalIgnoreCase))
						{
							standardNameSpace.Append('.');
							standardNameSpace.Append(NewFileDialog.GenerateValidClassOrNamespaceName(subdir, true));
						}
					}
				}
			}
			return standardNameSpace.ToString();
		}

		internal static string GenerateValidClassOrNamespaceName(string className, bool allowDot)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className");
			}
			className = className.Trim();
			string result;
			if (className.Length == 0)
			{
				result = string.Empty;
			}
			else
			{
				StringBuilder nameBuilder = new StringBuilder();
				if (className[0] != '_' && !char.IsLetter(className, 0))
				{
					nameBuilder.Append('_');
				}
				for (int idx = 0; idx < className.Length; idx++)
				{
					if (char.IsLetterOrDigit(className[idx]) || className[idx] == '_')
					{
						nameBuilder.Append(className[idx]);
					}
					else if (className[idx] == '.' && allowDot)
					{
						nameBuilder.Append('.');
					}
					else
					{
						nameBuilder.Append('_');
					}
				}
				result = nameBuilder.ToString();
			}
			return result;
		}
	}
}

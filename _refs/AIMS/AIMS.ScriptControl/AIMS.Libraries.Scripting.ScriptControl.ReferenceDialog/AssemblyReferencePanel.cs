using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class AssemblyReferencePanel : Panel, IReferencePanel
	{
		private ISelectReferenceDialog selectDialog;

		public AssemblyReferencePanel(ISelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			Button browseButton = new Button();
			browseButton.Location = new Point(10, 10);
			browseButton.Text = "Browse";
			browseButton.Click += new EventHandler(this.SelectReferenceDialog);
			browseButton.FlatStyle = FlatStyle.System;
			base.Controls.Add(browseButton);
		}

		private void SelectReferenceDialog(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag = new OpenFileDialog())
			{
				fdiag.AddExtension = true;
				fdiag.Filter = "AssemblyFiles|*.dll;*.exe|AllFiles}|*.*";
				fdiag.Multiselect = true;
				fdiag.CheckFileExists = true;
				if (fdiag.ShowDialog() == DialogResult.OK)
				{
					string[] fileNames = fdiag.FileNames;
					for (int i = 0; i < fileNames.Length; i++)
					{
						string file = fileNames[i];
						this.selectDialog.AddReference(ReferenceType.Assembly, Path.GetFileName(file), file, null);
					}
				}
			}
		}

		public void AddReference()
		{
			this.SelectReferenceDialog(null, null);
		}
	}
}

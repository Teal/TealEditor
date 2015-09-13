using AIMS.Libraries.Scripting.ScriptControl.Project;
using System;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class COMReferencePanel : ListView, IReferencePanel
	{
		private ISelectReferenceDialog selectDialog;

		private bool populated;

		public COMReferencePanel(ISelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			base.Sorting = SortOrder.Ascending;
			ColumnHeader nameHeader = new ColumnHeader();
			nameHeader.Text = "Global.Name";
			nameHeader.Width = 240;
			base.Columns.Add(nameHeader);
			ColumnHeader directoryHeader = new ColumnHeader();
			directoryHeader.Text = "Global.Path";
			directoryHeader.Width = 200;
			base.Columns.Add(directoryHeader);
			base.View = View.Details;
			this.Dock = DockStyle.Fill;
			base.FullRowSelect = true;
			base.ItemActivate += delegate
			{
				this.AddReference();
			};
		}

		public void AddReference()
		{
			foreach (ListViewItem item in base.SelectedItems)
			{
				TypeLibrary library = (TypeLibrary)item.Tag;
				this.selectDialog.AddReference(ReferenceType.Typelib, library.Name, library.Path, library);
			}
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (!this.populated && base.Visible)
			{
				this.populated = true;
				this.PopulateListView();
			}
		}

		private void PopulateListView()
		{
			foreach (TypeLibrary typeLib in TypeLibrary.Libraries)
			{
				ListViewItem newItem = new ListViewItem(new string[]
				{
					typeLib.Description,
					typeLib.Path
				});
				newItem.Tag = typeLib;
				base.Items.Add(newItem);
			}
		}
	}
}

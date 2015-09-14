using AIMS.Libraries.Scripting.Dom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class GacReferencePanel : UserControl, IReferencePanel
	{
		private class ColumnSorter : IComparer
		{
			private int column = 0;

			private bool asc = true;

			public int CurrentColumn
			{
				get
				{
					return this.column;
				}
				set
				{
					if (this.column == value)
					{
						this.asc = !this.asc;
					}
					else
					{
						this.column = value;
					}
				}
			}

			public int Compare(object x, object y)
			{
				ListViewItem rowA = (ListViewItem)x;
				ListViewItem rowB = (ListViewItem)y;
				int result = string.Compare(rowA.SubItems[this.CurrentColumn].Text, rowB.SubItems[this.CurrentColumn].Text);
				int result2;
				if (this.asc)
				{
					result2 = result;
				}
				else
				{
					result2 = result * -1;
				}
				return result2;
			}
		}

		protected ListView listView;

		private CheckBox chooseSpecificVersionCheckBox;

		private ISelectReferenceDialog selectDialog;

		private GacReferencePanel.ColumnSorter sorter;

		private ListViewItem[] fullItemList;

		private ListViewItem[] shortItemList;

		public GacReferencePanel(ISelectReferenceDialog selectDialog)
		{
			this.listView = new ListView();
			this.sorter = new GacReferencePanel.ColumnSorter();
			this.listView.ListViewItemSorter = this.sorter;
			this.selectDialog = selectDialog;
			ColumnHeader referenceHeader = new ColumnHeader();
			referenceHeader.Text = "Dialog.SelectReferenceDialog.GacReferencePanel.ReferenceHeader";
			referenceHeader.Width = 180;
			this.listView.Columns.Add(referenceHeader);
			this.listView.Sorting = SortOrder.Ascending;
			ColumnHeader versionHeader = new ColumnHeader();
			versionHeader.Text = "Dialog.SelectReferenceDialog.GacReferencePanel.VersionHeader";
			versionHeader.Width = 70;
			this.listView.Columns.Add(versionHeader);
			ColumnHeader pathHeader = new ColumnHeader();
			pathHeader.Text = "Global.Path";
			pathHeader.Width = 100;
			this.listView.Columns.Add(pathHeader);
			this.listView.View = View.Details;
			this.listView.FullRowSelect = true;
			this.listView.ItemActivate += delegate
			{
				this.AddReference();
			};
			this.listView.ColumnClick += new ColumnClickEventHandler(this.columnClick);
			this.listView.Dock = DockStyle.Fill;
			this.Dock = DockStyle.Fill;
			base.Controls.Add(this.listView);
			this.chooseSpecificVersionCheckBox = new CheckBox();
			this.chooseSpecificVersionCheckBox.Dock = DockStyle.Top;
			this.chooseSpecificVersionCheckBox.Text = "ChooseSpecificAssemblyVersion}";
			base.Controls.Add(this.chooseSpecificVersionCheckBox);
			this.chooseSpecificVersionCheckBox.CheckedChanged += delegate
			{
				this.listView.Items.Clear();
				if (this.chooseSpecificVersionCheckBox.Checked)
				{
					this.listView.Items.AddRange(this.fullItemList);
				}
				else
				{
					this.listView.Items.AddRange(this.shortItemList);
				}
			};
			this.PrintCache();
		}

		private void columnClick(object sender, ColumnClickEventArgs e)
		{
			if (e.Column < 2)
			{
				this.sorter.CurrentColumn = e.Column;
				this.listView.Sort();
			}
		}

		public void AddReference()
		{
			foreach (ListViewItem item in this.listView.SelectedItems)
			{
				this.selectDialog.AddReference(ReferenceType.Gac, item.Text, this.chooseSpecificVersionCheckBox.Checked ? item.Tag.ToString() : item.Text, null);
			}
		}

		private void PrintCache()
		{
			List<ListViewItem> itemList = this.GetCacheContent();
			this.fullItemList = itemList.ToArray();
			itemList.RemoveAll((ListViewItem item) => itemList.Exists((ListViewItem item2) => string.Equals(item.Text, item2.Text, StringComparison.OrdinalIgnoreCase) && new Version(item.SubItems[1].Text) < new Version(item2.SubItems[1].Text)));
			this.shortItemList = itemList.ToArray();
			this.listView.Items.AddRange(this.shortItemList);
		}

		protected virtual List<ListViewItem> GetCacheContent()
		{
			List<ListViewItem> itemList = new List<ListViewItem>();
			foreach (GacInterop.AssemblyListEntry asm in GacInterop.GetAssemblyList())
			{
				itemList.Add(new ListViewItem(new string[]
				{
					asm.Name,
					asm.Version
				})
				{
					Tag = asm.FullName
				});
			}
			return itemList;
		}

		void IReferencePanel.AddReference()
		{
			this.AddReference();
		}
	}
}

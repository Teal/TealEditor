using AIMS.Libraries.Forms.Docking;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class ErrorList : DockableWindow
	{
		private IContainer components = null;

		private ListView lvErrorList;

		private ColumnHeader colIcon;

		private ColumnHeader colNo;

		private ColumnHeader colDescription;

		private ColumnHeader colFile;

		private ColumnHeader colLine;

		private ColumnHeader colColumn;

		private ImageList imgList;

		public ScriptControl ParentSc;

		public event EventHandler<ListViewItemEventArgs> ItemDoubleClick
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ItemDoubleClick = (EventHandler<ListViewItemEventArgs>)Delegate.Combine(this.ItemDoubleClick, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ItemDoubleClick = (EventHandler<ListViewItemEventArgs>)Delegate.Remove(this.ItemDoubleClick, value);
			}
		}

		public int ParserErrorCount
		{
			get
			{
				return this.lvErrorList.Items.Count;
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(ErrorList));
			this.lvErrorList = new ListView();
			this.colIcon = new ColumnHeader();
			this.colNo = new ColumnHeader();
			this.colDescription = new ColumnHeader();
			this.colFile = new ColumnHeader();
			this.colLine = new ColumnHeader();
			this.colColumn = new ColumnHeader();
			this.imgList = new ImageList(this.components);
			base.SuspendLayout();
			this.lvErrorList.BorderStyle = BorderStyle.None;
			this.lvErrorList.Columns.AddRange(new ColumnHeader[]
			{
				this.colIcon,
				this.colNo,
				this.colDescription,
				this.colFile,
				this.colLine,
				this.colColumn
			});
			this.lvErrorList.Dock = DockStyle.Fill;
			this.lvErrorList.FullRowSelect = true;
			this.lvErrorList.GridLines = true;
			this.lvErrorList.LargeImageList = this.imgList;
			this.lvErrorList.Location = new Point(0, 0);
			this.lvErrorList.MultiSelect = false;
			this.lvErrorList.Name = "lvErrorList";
			this.lvErrorList.ShowGroups = false;
			this.lvErrorList.Size = new Size(458, 157);
			this.lvErrorList.SmallImageList = this.imgList;
			this.lvErrorList.StateImageList = this.imgList;
			this.lvErrorList.TabIndex = 0;
			this.lvErrorList.UseCompatibleStateImageBehavior = false;
			this.lvErrorList.View = View.Details;
			this.lvErrorList.DoubleClick += new EventHandler(this.lvErrorList_DoubleClick);
			this.lvErrorList.Resize += new EventHandler(this.lvErrorList_Resize);
			this.colIcon.Text = "";
			this.colIcon.Width = 16;
			this.colNo.Text = "";
			this.colNo.Width = 18;
			this.colDescription.Text = "Description";
			this.colDescription.Width = 166;
			this.colFile.Text = "File";
			this.colFile.Width = 96;
			this.colLine.Text = "Line";
			this.colLine.Width = 36;
			this.colColumn.Text = "Column";
			this.colColumn.Width = 54;
			this.imgList.ImageStream = (ImageListStreamer)resources.GetObject("imgList.ImageStream");
			this.imgList.TransparentColor = Color.Fuchsia;
			this.imgList.Images.SetKeyName(0, "ErrorList.bmp");
			this.imgList.Images.SetKeyName(1, "Output.bmp");
			this.imgList.Images.SetKeyName(2, "Warning.bmp");
			this.imgList.Images.SetKeyName(3, "Error.bmp");
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(458, 157);
			base.Controls.Add(this.lvErrorList);
			base.Icon = (Icon)resources.GetObject("$this.Icon");
			base.Name = "ErrorList";
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.Manual;
			base.TabText = "Error List";
			this.Text = "Error List";
			base.ResumeLayout(false);
		}

		public ErrorList(ScriptControl scriptControl)
		{
			this.ItemDoubleClick = null;
			this.ParentSc = null;
			base..ctor();
			this.ParentSc = scriptControl;
			this.InitializeComponent();
		}

		private void lvErrorList_Resize(object sender, EventArgs e)
		{
			this.lvErrorList.Columns[2].Width = this.lvErrorList.Width - (this.lvErrorList.Columns[0].Width + this.lvErrorList.Columns[1].Width + this.lvErrorList.Columns[3].Width + this.lvErrorList.Columns[4].Width + this.lvErrorList.Columns[5].Width);
		}

		public void ConvertToLanguage(ScriptLanguage OldLang, ScriptLanguage NewLanguage)
		{
			if (this.lvErrorList.Items.Count > 0)
			{
				string OldExt = (OldLang == ScriptLanguage.CSharp) ? ".cs" : ".vb";
				string NewExt = (NewLanguage == ScriptLanguage.CSharp) ? ".cs" : ".vb";
				foreach (ListViewItem lvExisting in this.lvErrorList.Items)
				{
					lvExisting.Tag = lvExisting.Tag.ToString().Replace(OldExt, NewExt);
					lvExisting.SubItems[3].Text = lvExisting.SubItems[3].Text.Replace(OldExt, NewExt);
				}
			}
		}

		public void ComilerErrors(Document doc, CompilerErrorCollection Errors)
		{
			foreach (ListViewItem lvExisting in this.lvErrorList.Items)
			{
				if (Convert.ToInt32(lvExisting.SubItems[1].Text) > 0)
				{
					string fileName = lvExisting.SubItems[3].Text;
					doc = this.ParentSc.ShowFile(fileName);
					if (doc != null)
					{
						doc.HighlightRemove(Convert.ToInt32("0" + lvExisting.SubItems[4].Text), Convert.ToInt32("0" + lvExisting.SubItems[5].Text));
					}
					lvExisting.Remove();
				}
			}
			if (Errors.HasErrors || Errors.HasWarnings)
			{
				foreach (CompilerError e in Errors)
				{
					int lineNo = e.Line - 1;
					int ColNo = e.Column - 1;
					lineNo = ((lineNo < 0) ? 0 : lineNo);
					ColNo = ((ColNo < 0) ? 0 : ColNo);
					string fileName = Path.GetFileName(e.FileName);
					ListViewItem lvItem;
					if (e.IsWarning)
					{
						lvItem = new ListViewItem(new string[]
						{
							"",
							"2",
							e.ErrorText,
							fileName,
							lineNo.ToString(),
							ColNo.ToString()
						});
						lvItem.StateImageIndex = 2;
					}
					else
					{
						lvItem = new ListViewItem(new string[]
						{
							"",
							"1",
							e.ErrorText,
							fileName,
							lineNo.ToString(),
							ColNo.ToString()
						});
						lvItem.StateImageIndex = 3;
					}
					lvItem.Tag = string.Concat(new string[]
					{
						fileName,
						"-",
						lineNo.ToString(),
						"-",
						ColNo.ToString()
					});
					this.lvErrorList.Items.Add(lvItem);
					doc = this.ParentSc.ShowFile(fileName);
					if (doc != null)
					{
						doc.HighlightError(lineNo, ColNo, e.IsWarning, e.ErrorText);
					}
				}
			}
			this.UpdateSummary();
		}

		public void ProjectErrors(Document doc, Errors p)
		{
			string msg = p.ErrorOutput.Trim();
			int lineNo = p.LineNo - 1;
			int ColNo = p.ColumnNo - 1;
			string fileName = doc.FileName;
			lineNo = ((lineNo < 0) ? 0 : lineNo);
			ColNo = ((ColNo < 0) ? 0 : ColNo);
			try
			{
				ListViewItem lvItem = new ListViewItem(new string[]
				{
					"",
					"0",
					msg,
					fileName,
					lineNo.ToString(),
					ColNo.ToString()
				});
				lvItem.Tag = string.Concat(new string[]
				{
					fileName,
					"-",
					lineNo.ToString(),
					"-",
					ColNo.ToString()
				});
				foreach (ListViewItem lvExisting in this.lvErrorList.Items)
				{
					if (lvExisting.Tag.ToString() == lvItem.Tag.ToString())
					{
						try
						{
							doc.HighlightRemove(Convert.ToInt32("0" + lvExisting.SubItems[4].Text), Convert.ToInt32("0" + lvExisting.SubItems[5].Text));
						}
						finally
						{
							lvExisting.Remove();
						}
					}
					if (Convert.ToInt32("0" + lvExisting.SubItems[1].Text) == 0 && lvExisting.SubItems[3].Text == fileName)
					{
						try
						{
							doc.HighlightRemove(Convert.ToInt32("0" + lvExisting.SubItems[4].Text), Convert.ToInt32("0" + lvExisting.SubItems[5].Text));
						}
						finally
						{
							lvExisting.Remove();
						}
					}
				}
				if (p.Count != 0)
				{
					doc.HighlightError(lineNo, ColNo, false, msg);
					lvItem.StateImageIndex = 3;
					this.lvErrorList.Items.Add(lvItem);
				}
			}
			finally
			{
				this.UpdateSummary();
			}
		}

		private void UpdateSummary()
		{
			if (this.lvErrorList.Items.Count > 0)
			{
				this.Text = "Error List (" + this.lvErrorList.Items.Count + ")";
			}
			else
			{
				this.Text = "Error List";
			}
			base.TabText = this.Text;
		}

		private void lvErrorList_DoubleClick(object sender, EventArgs e)
		{
			ListViewItem item = this.lvErrorList.SelectedItems[0];
			string FileName = item.SubItems[3].Text;
			int lineNo = Convert.ToInt32("0" + item.SubItems[4].Text);
			int colNo = Convert.ToInt32("0" + item.SubItems[5].Text);
			this.OnItemDoubleClick(new ListViewItemEventArgs(FileName, lineNo, colNo));
		}

		protected virtual void OnItemDoubleClick(ListViewItemEventArgs e)
		{
			if (this.ItemDoubleClick != null)
			{
				this.ItemDoubleClick(this, e);
			}
		}
	}
}

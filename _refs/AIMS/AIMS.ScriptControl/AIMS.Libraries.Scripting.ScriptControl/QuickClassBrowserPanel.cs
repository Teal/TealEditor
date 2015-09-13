using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class QuickClassBrowserPanel : UserControl
	{
		private class ComboBoxItem : IComparable
		{
			private object item;

			private string text;

			private int iconIndex;

			private bool isInCurrentPart;

			private string cachedString;

			public int IconIndex
			{
				get
				{
					return this.iconIndex;
				}
			}

			public object Item
			{
				get
				{
					return this.item;
				}
			}

			public bool IsInCurrentPart
			{
				get
				{
					return this.isInCurrentPart;
				}
			}

			public DomRegion ItemRegion
			{
				get
				{
					IClass classItem = this.item as IClass;
					DomRegion result;
					if (this.item is IClass)
					{
						result = ((IClass)this.item).Region;
					}
					else if (this.item is IMember)
					{
						result = ((IMember)this.item).Region;
					}
					else
					{
						result = DomRegion.Empty;
					}
					return result;
				}
			}

			public int Line
			{
				get
				{
					DomRegion r = this.ItemRegion;
					int result;
					if (r.IsEmpty)
					{
						result = 0;
					}
					else
					{
						result = r.BeginLine - 1;
					}
					return result;
				}
			}

			public int Column
			{
				get
				{
					DomRegion r = this.ItemRegion;
					int result;
					if (r.IsEmpty)
					{
						result = 0;
					}
					else
					{
						result = r.BeginColumn - 1;
					}
					return result;
				}
			}

			public int EndLine
			{
				get
				{
					DomRegion r = this.ItemRegion;
					int result;
					if (r.IsEmpty)
					{
						result = 0;
					}
					else
					{
						result = r.EndLine - 1;
					}
					return result;
				}
			}

			public ComboBoxItem(object item, string text, int iconIndex, bool isInCurrentPart)
			{
				this.item = item;
				this.text = text;
				this.iconIndex = iconIndex;
				this.isInCurrentPart = isInCurrentPart;
			}

			public bool IsInside(int lineNumber)
			{
				bool result;
				if (!this.isInCurrentPart)
				{
					result = false;
				}
				else
				{
					IClass classItem = this.item as IClass;
					if (classItem != null)
					{
						result = (!classItem.Region.IsEmpty && classItem.Region.BeginLine - 1 <= lineNumber && classItem.Region.EndLine - 1 >= lineNumber);
					}
					else
					{
						IMember member = this.item as IMember;
						if (member == null || member.Region.IsEmpty)
						{
							result = false;
						}
						else
						{
							bool isInside = member.Region.BeginLine - 1 <= lineNumber;
							if (member is IMethodOrProperty)
							{
								if (((IMethodOrProperty)member).BodyRegion.EndLine < 0)
								{
									result = (member.Region.BeginLine - 1 == lineNumber);
									return result;
								}
								isInside &= (lineNumber <= ((IMethodOrProperty)member).BodyRegion.EndLine - 1);
							}
							else
							{
								isInside &= (lineNumber <= member.Region.EndLine - 1);
							}
							result = isInside;
						}
					}
				}
				return result;
			}

			public int CompareItemTo(object obj)
			{
				QuickClassBrowserPanel.ComboBoxItem boxItem = (QuickClassBrowserPanel.ComboBoxItem)obj;
				int result;
				if (boxItem.Item is IComparable)
				{
					result = ((IComparable)boxItem.Item).CompareTo(this.item);
				}
				else if (boxItem.text != this.text || boxItem.Line != this.Line || boxItem.EndLine != this.EndLine || boxItem.iconIndex != this.iconIndex)
				{
					result = 1;
				}
				else
				{
					result = 0;
				}
				return result;
			}

			public override string ToString()
			{
				if (this.cachedString == null)
				{
					this.cachedString = this.ToStringInternal();
				}
				return this.cachedString;
			}

			private string ToStringInternal()
			{
				IAmbience ambience = ProjectParser.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
				string result;
				if (this.item is IMethod)
				{
					result = ambience.Convert((IMethod)this.item);
				}
				else if (this.item is IProperty)
				{
					result = ambience.Convert((IProperty)this.item);
				}
				else if (this.item is IField)
				{
					result = ambience.Convert((IField)this.item);
				}
				else if (this.item is IProperty)
				{
					result = ambience.Convert((IProperty)this.item);
				}
				else if (this.item is IEvent)
				{
					result = ambience.Convert((IEvent)this.item);
				}
				else
				{
					result = this.text;
				}
				return result;
			}

			public int CompareTo(object obj)
			{
				return this.ToString().CompareTo(obj.ToString());
			}
		}

		private ComboBox classComboBox;

		private ComboBox membersComboBox;

		private ICompilationUnit currentCompilationUnit;

		private CodeEditorControl textAreaControl;

		private bool autoselect = true;

		private bool membersComboBoxSelectedMember = false;

		private bool classComboBoxSelectedMember = false;

		private IClass lastClassInMembersComboBox;

		private static Font font = QuickClassBrowserPanel.font = new Font("Arial", 8.25f);

		private static StringFormat drawStringFormat = new StringFormat(StringFormatFlags.NoWrap);

		public CodeEditorControl Editor
		{
			get
			{
				return this.textAreaControl;
			}
			set
			{
				this.textAreaControl = value;
				this.textAreaControl.CaretChange += new EventHandler(this.CaretPositionChanged);
			}
		}

		public QuickClassBrowserPanel() : this(null)
		{
		}

		public QuickClassBrowserPanel(CodeEditorControl textAreaControl)
		{
			this.InitializeComponent();
			this.membersComboBox.MaxDropDownItems = 20;
			base.Dock = DockStyle.Top;
			if (textAreaControl != null)
			{
				this.Editor = textAreaControl;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.textAreaControl != null)
				{
					this.textAreaControl.CaretChange -= new EventHandler(this.CaretPositionChanged);
				}
				this.membersComboBox.Dispose();
				this.classComboBox.Dispose();
			}
			base.Dispose(disposing);
		}

		public void PopulateCombo()
		{
			try
			{
				ParseInformation parseInfo = ProjectParser.GetParseInformation(this.textAreaControl.FileName);
				if (parseInfo != null)
				{
					if (this.currentCompilationUnit != parseInfo.MostRecentCompilationUnit)
					{
						this.currentCompilationUnit = parseInfo.MostRecentCompilationUnit;
						if (this.currentCompilationUnit != null)
						{
							this.FillClassComboBox(true);
							this.FillMembersComboBox();
						}
					}
					this.UpdateClassComboBox();
					this.UpdateMembersComboBox();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void CaretPositionChanged(object sender, EventArgs e)
		{
			this.PopulateCombo();
		}

		private void UpdateMembersComboBox()
		{
			this.autoselect = false;
			try
			{
				if (this.currentCompilationUnit != null)
				{
					for (int i = 0; i < this.membersComboBox.Items.Count; i++)
					{
						if (((QuickClassBrowserPanel.ComboBoxItem)this.membersComboBox.Items[i]).IsInside(this.textAreaControl.ActiveViewControl.Caret.Position.Y))
						{
							if (this.membersComboBox.SelectedIndex != i)
							{
								this.membersComboBox.SelectedIndex = i;
							}
							if (!this.membersComboBoxSelectedMember)
							{
								this.membersComboBox.Refresh();
							}
							this.membersComboBoxSelectedMember = true;
							return;
						}
					}
				}
				this.membersComboBox.SelectedIndex = -1;
				if (this.membersComboBoxSelectedMember)
				{
					this.membersComboBox.Refresh();
					this.membersComboBoxSelectedMember = false;
				}
			}
			finally
			{
				this.autoselect = true;
			}
		}

		private void UpdateClassComboBox()
		{
			if (this.currentCompilationUnit == null)
			{
				this.currentCompilationUnit = ProjectParser.GetParseInformation(Path.GetFullPath(this.textAreaControl.FileName)).MostRecentCompilationUnit;
			}
			this.autoselect = false;
			try
			{
				if (this.currentCompilationUnit != null)
				{
					for (int i = 0; i < this.classComboBox.Items.Count; i++)
					{
						if (((QuickClassBrowserPanel.ComboBoxItem)this.classComboBox.Items[i]).IsInside(this.textAreaControl.ActiveViewControl.Caret.Position.Y))
						{
							bool innerClassContainsCaret = false;
							for (int j = i + 1; j < this.classComboBox.Items.Count; j++)
							{
								if (((QuickClassBrowserPanel.ComboBoxItem)this.classComboBox.Items[j]).IsInside(this.textAreaControl.ActiveViewControl.Caret.Position.Y))
								{
									innerClassContainsCaret = true;
									break;
								}
							}
							if (!innerClassContainsCaret)
							{
								if (this.classComboBox.SelectedIndex != i)
								{
									this.classComboBox.SelectedIndex = i;
									this.FillMembersComboBox();
								}
								if (!this.classComboBoxSelectedMember)
								{
									this.classComboBox.Refresh();
								}
								this.classComboBoxSelectedMember = true;
								return;
							}
						}
					}
				}
				if (this.classComboBoxSelectedMember)
				{
					this.classComboBox.Refresh();
					this.classComboBoxSelectedMember = false;
				}
			}
			finally
			{
				this.autoselect = true;
			}
		}

		private bool NeedtoUpdate(ArrayList items, ComboBox comboBox)
		{
			bool result;
			if (items.Count != comboBox.Items.Count)
			{
				result = true;
			}
			else
			{
				for (int i = 0; i < items.Count; i++)
				{
					QuickClassBrowserPanel.ComboBoxItem oldItem = (QuickClassBrowserPanel.ComboBoxItem)comboBox.Items[i];
					QuickClassBrowserPanel.ComboBoxItem newItem = (QuickClassBrowserPanel.ComboBoxItem)items[i];
					if (oldItem.GetType() != newItem.GetType())
					{
						result = true;
						return result;
					}
					if (newItem.CompareItemTo(oldItem) != 0)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		private void FillMembersComboBox()
		{
			IClass c = this.GetCurrentSelectedClass();
			if (c != null && this.lastClassInMembersComboBox != c)
			{
				this.lastClassInMembersComboBox = c;
				ArrayList items = new ArrayList();
				bool partialMode = false;
				IClass currentPart = c;
				if (c.IsPartial)
				{
					CompoundClass cc = c.GetCompoundClass() as CompoundClass;
					if (cc != null)
					{
						partialMode = true;
						c = cc;
					}
				}
				lock (c)
				{
					int lastIndex = 0;
					IComparer comparer = new Comparer(CultureInfo.InvariantCulture);
					foreach (IMethod i in c.Methods)
					{
						items.Add(new QuickClassBrowserPanel.ComboBoxItem(i, i.Name, (int)ScriptControl.GetIcon(i), !partialMode || currentPart.Methods.Contains(i)));
					}
					items.Sort(lastIndex, c.Methods.Count, comparer);
					lastIndex = items.Count;
					foreach (IProperty p in c.Properties)
					{
						items.Add(new QuickClassBrowserPanel.ComboBoxItem(p, p.Name, (int)ScriptControl.GetIcon(p), !partialMode || currentPart.Properties.Contains(p)));
					}
					items.Sort(lastIndex, c.Properties.Count, comparer);
					lastIndex = items.Count;
					foreach (IField f in c.Fields)
					{
						items.Add(new QuickClassBrowserPanel.ComboBoxItem(f, f.Name, (int)ScriptControl.GetIcon(f), !partialMode || currentPart.Fields.Contains(f)));
					}
					items.Sort(lastIndex, c.Fields.Count, comparer);
					lastIndex = items.Count;
					foreach (IEvent evt in c.Events)
					{
						items.Add(new QuickClassBrowserPanel.ComboBoxItem(evt, evt.Name, (int)ScriptControl.GetIcon(evt), !partialMode || currentPart.Events.Contains(evt)));
					}
					items.Sort(lastIndex, c.Events.Count, comparer);
					lastIndex = items.Count;
				}
				this.membersComboBox.BeginUpdate();
				this.membersComboBox.Items.Clear();
				this.membersComboBox.Items.AddRange(items.ToArray());
				this.membersComboBox.EndUpdate();
				this.UpdateMembersComboBox();
			}
		}

		private void AddClasses(ArrayList items, ICollection classes)
		{
			foreach (IClass c in classes)
			{
				items.Add(new QuickClassBrowserPanel.ComboBoxItem(c, c.FullyQualifiedName, (int)ScriptControl.GetIcon(c), true));
				this.AddClasses(items, c.InnerClasses);
			}
		}

		private void FillClassComboBox(bool isUpdateRequired)
		{
			ArrayList items = new ArrayList();
			this.AddClasses(items, this.currentCompilationUnit.Classes);
			if (isUpdateRequired)
			{
				this.classComboBox.BeginUpdate();
			}
			this.classComboBox.Items.Clear();
			this.membersComboBox.Items.Clear();
			this.classComboBox.Items.AddRange(items.ToArray());
			if (items.Count == 1)
			{
				try
				{
					this.autoselect = false;
					this.classComboBox.SelectedIndex = 0;
					this.FillMembersComboBox();
				}
				finally
				{
					this.autoselect = true;
				}
			}
			if (isUpdateRequired)
			{
				this.classComboBox.EndUpdate();
			}
			this.UpdateClassComboBox();
		}

		private void InitializeComponent()
		{
			this.membersComboBox = new ComboBox();
			this.classComboBox = new ComboBox();
			base.SuspendLayout();
			this.membersComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.membersComboBox.DrawMode = DrawMode.OwnerDrawVariable;
			this.membersComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.membersComboBox.Location = new Point(199, 4);
			this.membersComboBox.Name = "membersComboBox";
			this.membersComboBox.Size = new Size(316, 21);
			this.membersComboBox.TabIndex = 1;
			this.membersComboBox.DrawItem += new DrawItemEventHandler(this.ComboBoxDrawItem);
			this.membersComboBox.SelectedIndexChanged += new EventHandler(this.ComboBoxSelectedIndexChanged);
			this.membersComboBox.MeasureItem += new MeasureItemEventHandler(this.MeasureComboBoxItem);
			this.classComboBox.DrawMode = DrawMode.OwnerDrawVariable;
			this.classComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.classComboBox.Location = new Point(4, 4);
			this.classComboBox.Name = "classComboBox";
			this.classComboBox.Size = new Size(189, 21);
			this.classComboBox.Sorted = true;
			this.classComboBox.TabIndex = 0;
			this.classComboBox.DrawItem += new DrawItemEventHandler(this.ComboBoxDrawItem);
			this.classComboBox.SelectedIndexChanged += new EventHandler(this.ComboBoxSelectedIndexChanged);
			this.classComboBox.MeasureItem += new MeasureItemEventHandler(this.MeasureComboBoxItem);
			base.Controls.Add(this.membersComboBox);
			base.Controls.Add(this.classComboBox);
			base.Name = "QuickClassBrowserPanel";
			base.Size = new Size(520, 29);
			base.Resize += new EventHandler(this.QuickClassBrowserPanelResize);
			base.ResumeLayout(false);
		}

		public IClass GetCurrentSelectedClass()
		{
			IClass result;
			if (this.classComboBox.SelectedIndex >= 0)
			{
				result = (IClass)((QuickClassBrowserPanel.ComboBoxItem)this.classComboBox.Items[this.classComboBox.SelectedIndex]).Item;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			if (this.textAreaControl != null)
			{
				if (this.autoselect)
				{
					QuickClassBrowserPanel.ComboBoxItem item = (QuickClassBrowserPanel.ComboBoxItem)comboBox.Items[comboBox.SelectedIndex];
					if (item.IsInCurrentPart)
					{
						this.textAreaControl.ActiveViewControl.Caret.Position = new TextPoint(item.Column, item.Line);
						this.textAreaControl.ActiveViewControl.ScrollIntoView();
						this.textAreaControl.ActiveViewControl.Focus();
					}
					else
					{
						IMember i = item.Item as IMember;
						if (i != null)
						{
							string fileName = i.DeclaringType.CompilationUnit.FileName;
							if (fileName == this.textAreaControl.FileName)
							{
								this.textAreaControl.ActiveViewControl.Caret.Position = new TextPoint(item.Column, item.Line);
								this.textAreaControl.ActiveViewControl.ScrollIntoView();
								this.textAreaControl.ActiveViewControl.Focus();
							}
							else
							{
								Document doc = (Document)this.textAreaControl.Parent;
								ScriptControl sc = doc.ParentScriptControl;
								Document docnew = sc.ShowFile(fileName);
								if (docnew != null)
								{
									docnew.ParseContentsNow();
									docnew.Editor.ActiveViewControl.Caret.Position = new TextPoint(item.Column, item.Line);
									docnew.Editor.ActiveViewControl.ScrollIntoView();
									docnew.Editor.ActiveViewControl.Focus();
								}
								else
								{
									docnew = (Document)this.textAreaControl.Parent;
									docnew.ParseContentsNow();
									docnew.Editor.ActiveViewControl.Caret.Position = new TextPoint(item.Column, item.Line);
									docnew.Editor.ActiveViewControl.ScrollIntoView();
									docnew.Editor.ActiveViewControl.Focus();
								}
							}
						}
					}
					if (comboBox == this.classComboBox)
					{
						this.FillMembersComboBox();
						this.UpdateMembersComboBox();
					}
				}
			}
		}

		private void ComboBoxDrawItem(object sender, DrawItemEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			e.DrawBackground();
			if (this.textAreaControl != null)
			{
				if (e.Index >= 0)
				{
					QuickClassBrowserPanel.ComboBoxItem item = (QuickClassBrowserPanel.ComboBoxItem)comboBox.Items[e.Index];
					e.Graphics.DrawImageUnscaled(this.textAreaControl.AutoListIcons.Images[item.IconIndex], new Point(e.Bounds.X, e.Bounds.Y + (e.Bounds.Height - this.textAreaControl.AutoListIcons.ImageSize.Height) / 2));
					Rectangle drawingRect = new Rectangle(e.Bounds.X + this.textAreaControl.AutoListIcons.ImageSize.Width, e.Bounds.Y, e.Bounds.Width - this.textAreaControl.AutoListIcons.ImageSize.Width, e.Bounds.Height);
					Brush drawItemBrush = SystemBrushes.WindowText;
					if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
					{
						drawItemBrush = SystemBrushes.HighlightText;
					}
					if (!item.IsInCurrentPart)
					{
						drawItemBrush = SystemBrushes.ControlDark;
					}
					else if (e.State == DrawItemState.ComboBoxEdit && !item.IsInside(this.textAreaControl.ActiveViewControl.Caret.Position.Y))
					{
						drawItemBrush = SystemBrushes.ControlDark;
					}
					e.Graphics.DrawString(item.ToString(), QuickClassBrowserPanel.font, drawItemBrush, drawingRect, QuickClassBrowserPanel.drawStringFormat);
				}
				e.DrawFocusRectangle();
			}
		}

		private void QuickClassBrowserPanelResize(object sender, EventArgs e)
		{
			Size comboBoxSize = new Size(base.Width / 2 - 12, 21);
			this.classComboBox.Location = new Point(8, this.classComboBox.Bounds.Top);
			this.classComboBox.Size = comboBoxSize;
			this.membersComboBox.Location = new Point(this.classComboBox.Bounds.Right + 8, this.classComboBox.Bounds.Top);
			this.membersComboBox.Size = comboBoxSize;
		}

		private void MeasureComboBoxItem(object sender, MeasureItemEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			if (e.Index >= 0)
			{
				QuickClassBrowserPanel.ComboBoxItem item = (QuickClassBrowserPanel.ComboBoxItem)comboBox.Items[e.Index];
				SizeF size = e.Graphics.MeasureString(item.ToString(), QuickClassBrowserPanel.font);
				e.ItemWidth = (int)size.Width;
				if (this.textAreaControl == null)
				{
					e.ItemHeight = (int)Math.Max(size.Height, 16f);
				}
				else
				{
					e.ItemHeight = (int)Math.Max(size.Height, (float)this.textAreaControl.AutoListIcons.ImageSize.Height);
				}
			}
		}
	}
}

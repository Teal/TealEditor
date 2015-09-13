using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.Forms.Docking;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.ScriptControl.Converter;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using AIMS.Libraries.Scripting.ScriptControl.Project;
using AIMS.Libraries.Scripting.ScriptControl.Properties;
using AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class ScriptControl : UserControl
	{
		private IContainer components = null;

		private SyntaxDocument syntaxDocument1;

		private StatusStrip ScriptStatus;

		private ToolStripStatusLabel tsMessage;

		private ToolStripProgressBar tsProgress;

		private ToolTip CtrlToolTip;

		private ToolStripPanel BottomToolStripPanel;

		private ToolStripPanel TopToolStripPanel;

		private ToolStripPanel RightToolStripPanel;

		private ToolStripPanel LeftToolStripPanel;

		private ToolStripContentPanel ContentPanel;

		private ToolStripContainer toolStripContainer1;

		private ToolStrip toolStrip1;

		private ToolStripDropDownButton tsbSelectLanguage;

		private ToolStripMenuItem cNetToolStripMenuItemCSharp;

		private ToolStripMenuItem vBNetToolStripMenuItemVbNet;

		private ToolStripSeparator toolStripSeparator7;

		private ToolStripButton tsbNew;

		private ToolStripButton tsbSave;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton tsbCut;

		private ToolStripButton tsbCopy;

		private ToolStripButton tsbPaste;

		private ToolStripSeparator toolStripSeparator5;

		private ToolStripButton tsbComment;

		private ToolStripButton tsbUnComment;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripButton tsbUndo;

		private ToolStripButton tsbRedo;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripButton tsbBuild;

		private ToolStripButton tsbRun;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripButton tsbToggleBookmark;

		private ToolStripButton tsbPreBookmark;

		private ToolStripButton tsbNextBookmark;

		private ToolStripButton tsbDelAllBookmark;

		private ToolStripButton tsbDelallBreakPoints;

		private ToolStripSeparator toolStripSeparator6;

		private ToolStripButton tsbFind;

		private ToolStripButton tsbReplace;

		private ToolStripSeparator toolStripSeparator8;

		private ToolStripButton tsbErrorList;

		private ToolStripButton tsbSolutionExplorer;

		private DockContainer dockContainer1;

		private ToolStripStatusLabel tCursorPos;

		private ScriptLanguage _scriptLanguage;

		private ErrorList _winErrorList = null;

		private Output _winOutput = null;

		private ProjectExplorer _winProjExplorer = null;

		private static IProject m_AIMSProject = null;

		private WeakReference m_SelectRefDialog = null;

		private IDictionary<string, object> _RemoteVariables = null;

		public event EventHandler Execute
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Execute = (EventHandler)Delegate.Combine(this.Execute, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Execute = (EventHandler)Delegate.Remove(this.Execute, value);
			}
		}

		public ScriptLanguage ScriptLanguage
		{
			get
			{
				return this._scriptLanguage;
			}
			set
			{
				if (value == ScriptLanguage.CSharp)
				{
					this.tsbSelectLanguage.Image = Resources.VSProject_CSCodefile;
					this.tsbSelectLanguage.ImageTransparentColor = Color.Magenta;
				}
				else
				{
					this.tsbSelectLanguage.Image = Resources.VSProject_VBCodefile;
					this.tsbSelectLanguage.ImageTransparentColor = Color.Magenta;
				}
				this.ConvertToLanguage(this._scriptLanguage, value);
				this._scriptLanguage = value;
			}
		}

		public IDictionary<string, object> RemoteVariables
		{
			get
			{
				return this._RemoteVariables;
			}
		}

		public string DefaultNameSpace
		{
			get
			{
				return ScriptControl.m_AIMSProject.RootNamespace;
			}
		}

		public string DefaultClassName
		{
			get
			{
				return "Program";
			}
		}

		public string StartMethodName
		{
			get
			{
				return "Main";
			}
		}

		public string OutputAssemblyName
		{
			get
			{
				return ScriptControl.m_AIMSProject.OutputAssemblyFullPath;
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
			this.syntaxDocument1 = new SyntaxDocument(this.components);
			this.ScriptStatus = new StatusStrip();
			this.tsMessage = new ToolStripStatusLabel();
			this.tsProgress = new ToolStripProgressBar();
			this.tCursorPos = new ToolStripStatusLabel();
			this.CtrlToolTip = new ToolTip(this.components);
			this.BottomToolStripPanel = new ToolStripPanel();
			this.TopToolStripPanel = new ToolStripPanel();
			this.RightToolStripPanel = new ToolStripPanel();
			this.LeftToolStripPanel = new ToolStripPanel();
			this.ContentPanel = new ToolStripContentPanel();
			this.toolStripContainer1 = new ToolStripContainer();
			this.dockContainer1 = new DockContainer();
			this.toolStrip1 = new ToolStrip();
			this.tsbSelectLanguage = new ToolStripDropDownButton();
			this.cNetToolStripMenuItemCSharp = new ToolStripMenuItem();
			this.vBNetToolStripMenuItemVbNet = new ToolStripMenuItem();
			this.toolStripSeparator7 = new ToolStripSeparator();
			this.tsbNew = new ToolStripButton();
			this.tsbSave = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.tsbCut = new ToolStripButton();
			this.tsbCopy = new ToolStripButton();
			this.tsbPaste = new ToolStripButton();
			this.toolStripSeparator5 = new ToolStripSeparator();
			this.tsbComment = new ToolStripButton();
			this.tsbUnComment = new ToolStripButton();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.tsbUndo = new ToolStripButton();
			this.tsbRedo = new ToolStripButton();
			this.toolStripSeparator3 = new ToolStripSeparator();
			this.tsbBuild = new ToolStripButton();
			this.tsbRun = new ToolStripButton();
			this.toolStripSeparator4 = new ToolStripSeparator();
			this.tsbToggleBookmark = new ToolStripButton();
			this.tsbPreBookmark = new ToolStripButton();
			this.tsbNextBookmark = new ToolStripButton();
			this.tsbDelAllBookmark = new ToolStripButton();
			this.tsbDelallBreakPoints = new ToolStripButton();
			this.toolStripSeparator6 = new ToolStripSeparator();
			this.tsbFind = new ToolStripButton();
			this.tsbReplace = new ToolStripButton();
			this.toolStripSeparator8 = new ToolStripSeparator();
			this.tsbErrorList = new ToolStripButton();
			this.tsbSolutionExplorer = new ToolStripButton();
			this.ScriptStatus.SuspendLayout();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.syntaxDocument1.Lines = new string[]
			{
				""
			};
			this.syntaxDocument1.MaxUndoBufferSize = 1000;
			this.syntaxDocument1.Modified = false;
			this.syntaxDocument1.UndoStep = 0;
			this.ScriptStatus.Items.AddRange(new ToolStripItem[]
			{
				this.tsMessage,
				this.tsProgress,
				this.tCursorPos
			});
			this.ScriptStatus.Location = new Point(0, 413);
			this.ScriptStatus.Name = "ScriptStatus";
			this.ScriptStatus.Size = new Size(633, 22);
			this.ScriptStatus.SizingGrip = false;
			this.ScriptStatus.TabIndex = 1;
			this.ScriptStatus.Text = "statusStrip1";
			this.tsMessage.AutoSize = false;
			this.tsMessage.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsMessage.Name = "tsMessage";
			this.tsMessage.Size = new Size(618, 17);
			this.tsMessage.Spring = true;
			this.tsMessage.Text = "Please wait..";
			this.tsMessage.TextAlign = ContentAlignment.MiddleLeft;
			this.tsProgress.AutoSize = false;
			this.tsProgress.Name = "tsProgress";
			this.tsProgress.Size = new Size(200, 16);
			this.tsProgress.Style = ProgressBarStyle.Marquee;
			this.tsProgress.Visible = false;
			this.tCursorPos.Name = "tCursorPos";
			this.tCursorPos.Size = new Size(0, 17);
			this.tCursorPos.TextAlign = ContentAlignment.MiddleRight;
			this.tCursorPos.ToolTipText = "Cursor Position";
			this.BottomToolStripPanel.Location = new Point(0, 0);
			this.BottomToolStripPanel.Name = "BottomToolStripPanel";
			this.BottomToolStripPanel.Orientation = Orientation.Horizontal;
			this.BottomToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
			this.BottomToolStripPanel.Size = new Size(0, 0);
			this.TopToolStripPanel.Location = new Point(0, 0);
			this.TopToolStripPanel.Name = "TopToolStripPanel";
			this.TopToolStripPanel.Orientation = Orientation.Horizontal;
			this.TopToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
			this.TopToolStripPanel.Size = new Size(0, 0);
			this.RightToolStripPanel.Location = new Point(0, 0);
			this.RightToolStripPanel.Name = "RightToolStripPanel";
			this.RightToolStripPanel.Orientation = Orientation.Horizontal;
			this.RightToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
			this.RightToolStripPanel.Size = new Size(0, 0);
			this.LeftToolStripPanel.Location = new Point(0, 0);
			this.LeftToolStripPanel.Name = "LeftToolStripPanel";
			this.LeftToolStripPanel.Orientation = Orientation.Horizontal;
			this.LeftToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
			this.LeftToolStripPanel.Size = new Size(0, 0);
			this.ContentPanel.Size = new Size(589, 301);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.dockContainer1);
			this.toolStripContainer1.ContentPanel.Size = new Size(633, 388);
			this.toolStripContainer1.Dock = DockStyle.Fill;
			this.toolStripContainer1.Location = new Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new Size(633, 413);
			this.toolStripContainer1.TabIndex = 4;
			this.toolStripContainer1.Text = "toolStripContainer1";
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			this.dockContainer1.ActiveAutoHideContent = null;
			this.dockContainer1.Dock = DockStyle.Fill;
			this.dockContainer1.DocumentStyle = DocumentStyles.DockingWindow;
			this.dockContainer1.Font = new Font("Tahoma", 11f, FontStyle.Regular, GraphicsUnit.World, 0);
			this.dockContainer1.Location = new Point(0, 0);
			this.dockContainer1.Name = "dockContainer1";
			this.dockContainer1.Size = new Size(633, 388);
			this.dockContainer1.TabIndex = 0;
			this.toolStrip1.Dock = DockStyle.None;
			this.toolStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.tsbSelectLanguage,
				this.toolStripSeparator7,
				this.tsbNew,
				this.tsbSave,
				this.toolStripSeparator1,
				this.tsbCut,
				this.tsbCopy,
				this.tsbPaste,
				this.toolStripSeparator5,
				this.tsbComment,
				this.tsbUnComment,
				this.toolStripSeparator2,
				this.tsbUndo,
				this.tsbRedo,
				this.toolStripSeparator3,
				this.tsbBuild,
				this.tsbRun,
				this.toolStripSeparator4,
				this.tsbToggleBookmark,
				this.tsbPreBookmark,
				this.tsbNextBookmark,
				this.tsbDelAllBookmark,
				this.tsbDelallBreakPoints,
				this.toolStripSeparator6,
				this.tsbFind,
				this.tsbReplace,
				this.toolStripSeparator8,
				this.tsbErrorList,
				this.tsbSolutionExplorer
			});
			this.toolStrip1.Location = new Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(549, 25);
			this.toolStrip1.TabIndex = 4;
			this.tsbSelectLanguage.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbSelectLanguage.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.cNetToolStripMenuItemCSharp,
				this.vBNetToolStripMenuItemVbNet
			});
			this.tsbSelectLanguage.Image = Resources.VSProject_CSCodefile;
			this.tsbSelectLanguage.ImageTransparentColor = Color.Magenta;
			this.tsbSelectLanguage.Name = "tsbSelectLanguage";
			this.tsbSelectLanguage.Size = new Size(29, 22);
			this.tsbSelectLanguage.Text = "Select Language";
			this.cNetToolStripMenuItemCSharp.Image = Resources.VSProject_CSCodefile;
			this.cNetToolStripMenuItemCSharp.ImageTransparentColor = Color.Fuchsia;
			this.cNetToolStripMenuItemCSharp.Name = "cNetToolStripMenuItemCSharp";
			this.cNetToolStripMenuItemCSharp.Size = new Size(114, 22);
			this.cNetToolStripMenuItemCSharp.Text = "C# .Net";
			this.cNetToolStripMenuItemCSharp.ToolTipText = "Select C# as programming Language";
			this.cNetToolStripMenuItemCSharp.Click += new EventHandler(this.cNetToolStripMenuItemCSharp_Click);
			this.vBNetToolStripMenuItemVbNet.BackColor = Color.Transparent;
			this.vBNetToolStripMenuItemVbNet.Image = Resources.VSProject_VBCodefile;
			this.vBNetToolStripMenuItemVbNet.ImageTransparentColor = Color.Fuchsia;
			this.vBNetToolStripMenuItemVbNet.Name = "vBNetToolStripMenuItemVbNet";
			this.vBNetToolStripMenuItemVbNet.Size = new Size(114, 22);
			this.vBNetToolStripMenuItemVbNet.Text = "VB .Net";
			this.vBNetToolStripMenuItemVbNet.ToolTipText = "Select Vb.Net as programming Language";
			this.vBNetToolStripMenuItemVbNet.Click += new EventHandler(this.vBNetToolStripMenuItemVbNet_Click);
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new Size(6, 25);
			this.tsbNew.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbNew.Image = Resources.NewDocument;
			this.tsbNew.ImageTransparentColor = Color.Magenta;
			this.tsbNew.Name = "tsbNew";
			this.tsbNew.Size = new Size(23, 22);
			this.tsbNew.Text = "New";
			this.tsbNew.Click += new EventHandler(this.tsbNew_Click);
			this.tsbSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbSave.Image = Resources.Save;
			this.tsbSave.ImageTransparentColor = Color.Magenta;
			this.tsbSave.Name = "tsbSave";
			this.tsbSave.Size = new Size(23, 22);
			this.tsbSave.Text = "Save";
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(6, 25);
			this.tsbCut.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbCut.Image = Resources.Cut;
			this.tsbCut.ImageTransparentColor = Color.Magenta;
			this.tsbCut.Name = "tsbCut";
			this.tsbCut.Size = new Size(23, 22);
			this.tsbCut.Text = "Cut";
			this.tsbCut.Click += new EventHandler(this.tsbCut_Click);
			this.tsbCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbCopy.Image = Resources.Copy;
			this.tsbCopy.ImageTransparentColor = Color.Magenta;
			this.tsbCopy.Name = "tsbCopy";
			this.tsbCopy.Size = new Size(23, 22);
			this.tsbCopy.Text = "Copy";
			this.tsbCopy.Click += new EventHandler(this.tsbCopy_Click);
			this.tsbPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbPaste.Image = Resources.Paste;
			this.tsbPaste.ImageTransparentColor = Color.Magenta;
			this.tsbPaste.Name = "tsbPaste";
			this.tsbPaste.Size = new Size(23, 22);
			this.tsbPaste.Text = "Paste";
			this.tsbPaste.Click += new EventHandler(this.tsbPaste_Click);
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new Size(6, 25);
			this.tsbComment.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbComment.Image = Resources.CodeComment;
			this.tsbComment.ImageTransparentColor = Color.Magenta;
			this.tsbComment.Name = "tsbComment";
			this.tsbComment.Size = new Size(23, 22);
			this.tsbComment.Text = "Comment out the selected lines.";
			this.tsbComment.Click += new EventHandler(this.tsbComment_Click);
			this.tsbUnComment.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbUnComment.Image = Resources.CodeUnComment;
			this.tsbUnComment.ImageTransparentColor = Color.Magenta;
			this.tsbUnComment.Name = "tsbUnComment";
			this.tsbUnComment.Size = new Size(23, 22);
			this.tsbUnComment.Text = "Uncomment the selected lines.";
			this.tsbUnComment.Click += new EventHandler(this.tsbUnComment_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(6, 25);
			this.tsbUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbUndo.Image = Resources.Edit_Undo;
			this.tsbUndo.ImageTransparentColor = Color.Magenta;
			this.tsbUndo.Name = "tsbUndo";
			this.tsbUndo.Size = new Size(23, 22);
			this.tsbUndo.Text = "Undo";
			this.tsbUndo.Click += new EventHandler(this.tsbUndo_Click);
			this.tsbRedo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbRedo.Image = Resources.Edit_Redo;
			this.tsbRedo.ImageTransparentColor = Color.Magenta;
			this.tsbRedo.Name = "tsbRedo";
			this.tsbRedo.Size = new Size(23, 22);
			this.tsbRedo.Text = "Redo";
			this.tsbRedo.Click += new EventHandler(this.tsbRedo_Click);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new Size(6, 25);
			this.tsbBuild.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbBuild.Image = Resources.Build;
			this.tsbBuild.ImageTransparentColor = Color.Magenta;
			this.tsbBuild.Name = "tsbBuild";
			this.tsbBuild.Size = new Size(23, 22);
			this.tsbBuild.Text = "Build";
			this.tsbBuild.Click += new EventHandler(this.tsbBuild_Click);
			this.tsbRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbRun.Image = Resources.Run;
			this.tsbRun.ImageTransparentColor = Color.Magenta;
			this.tsbRun.Name = "tsbRun";
			this.tsbRun.Size = new Size(23, 22);
			this.tsbRun.Text = "Run";
			this.tsbRun.Click += new EventHandler(this.tsbRun_Click);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new Size(6, 25);
			this.tsbToggleBookmark.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbToggleBookmark.Image = Resources.BookMarkToggle;
			this.tsbToggleBookmark.ImageTransparentColor = Color.Magenta;
			this.tsbToggleBookmark.Name = "tsbToggleBookmark";
			this.tsbToggleBookmark.Size = new Size(23, 22);
			this.tsbToggleBookmark.Text = "Toggle Bookmark";
			this.tsbToggleBookmark.Click += new EventHandler(this.tsbToggleBookmark_Click);
			this.tsbPreBookmark.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbPreBookmark.Image = Resources.BookMarkPre;
			this.tsbPreBookmark.ImageTransparentColor = Color.Magenta;
			this.tsbPreBookmark.Name = "tsbPreBookmark";
			this.tsbPreBookmark.Size = new Size(23, 22);
			this.tsbPreBookmark.Text = "Move to previous bookmark.";
			this.tsbPreBookmark.Click += new EventHandler(this.tsbPreBookmark_Click);
			this.tsbNextBookmark.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbNextBookmark.Image = Resources.BookMarkNext;
			this.tsbNextBookmark.ImageTransparentColor = Color.Magenta;
			this.tsbNextBookmark.Name = "tsbNextBookmark";
			this.tsbNextBookmark.Size = new Size(23, 22);
			this.tsbNextBookmark.Text = "Move to next bookmark.";
			this.tsbNextBookmark.Click += new EventHandler(this.tsbNextBookmark_Click);
			this.tsbDelAllBookmark.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbDelAllBookmark.Image = Resources.BookMarkDelete;
			this.tsbDelAllBookmark.ImageTransparentColor = Color.Magenta;
			this.tsbDelAllBookmark.Name = "tsbDelAllBookmark";
			this.tsbDelAllBookmark.Size = new Size(23, 22);
			this.tsbDelAllBookmark.Text = "Delete all bookmarks.";
			this.tsbDelAllBookmark.Click += new EventHandler(this.tsbDelAllBookmark_Click);
			this.tsbDelallBreakPoints.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbDelallBreakPoints.Image = Resources.DelBreakpoints;
			this.tsbDelallBreakPoints.ImageTransparentColor = Color.Magenta;
			this.tsbDelallBreakPoints.Name = "tsbDelallBreakPoints";
			this.tsbDelallBreakPoints.Size = new Size(23, 22);
			this.tsbDelallBreakPoints.Text = "Delete all breakpoints.";
			this.tsbDelallBreakPoints.Click += new EventHandler(this.tsbDelallBreakPoints_Click);
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new Size(6, 25);
			this.tsbFind.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbFind.Image = Resources.Find;
			this.tsbFind.ImageTransparentColor = Color.Magenta;
			this.tsbFind.Name = "tsbFind";
			this.tsbFind.Size = new Size(23, 22);
			this.tsbFind.Text = "Find & Replace";
			this.tsbFind.Click += new EventHandler(this.tsbFind_Click);
			this.tsbReplace.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbReplace.Image = Resources.FindNext;
			this.tsbReplace.ImageTransparentColor = Color.Magenta;
			this.tsbReplace.Name = "tsbReplace";
			this.tsbReplace.Size = new Size(23, 22);
			this.tsbReplace.Text = "toolStripButton1";
			this.tsbReplace.Click += new EventHandler(this.tsbReplace_Click);
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new Size(6, 25);
			this.tsbErrorList.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbErrorList.Image = Resources.Output;
			this.tsbErrorList.ImageTransparentColor = Color.Magenta;
			this.tsbErrorList.Name = "tsbErrorList";
			this.tsbErrorList.Size = new Size(23, 22);
			this.tsbErrorList.Text = "Error List";
			this.tsbErrorList.Click += new EventHandler(this.tsbErrorList_Click);
			this.tsbSolutionExplorer.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbSolutionExplorer.Image = Resources.VSProjectExplorer;
			this.tsbSolutionExplorer.ImageTransparentColor = Color.Magenta;
			this.tsbSolutionExplorer.Name = "tsbSolutionExplorer";
			this.tsbSolutionExplorer.Size = new Size(23, 22);
			this.tsbSolutionExplorer.Text = "Project References";
			this.tsbSolutionExplorer.ToolTipText = "Project References";
			this.tsbSolutionExplorer.Click += new EventHandler(this.tsbSolutionExplorer_Click);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.toolStripContainer1);
			base.Controls.Add(this.ScriptStatus);
			this.DoubleBuffered = true;
			base.Name = "ScriptControl";
			base.Size = new Size(633, 435);
			this.ScriptStatus.ResumeLayout(false);
			this.ScriptStatus.PerformLayout();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected virtual void OnExecute()
		{
			if (this.Execute != null)
			{
				this.Execute(this, null);
			}
		}

		private void ConvertToLanguage(ScriptLanguage OldLang, ScriptLanguage NewLanguage)
		{
			this.ResetParserLanguage(NewLanguage);
			foreach (IDockableWindow docWin in this.dockContainer1.Contents)
			{
				if (docWin is Document)
				{
					Document doc = docWin as Document;
					this.DocumentEvents(doc, false);
					if (OldLang != NewLanguage)
					{
						doc.FileName = Path.GetFileNameWithoutExtension(doc.FileName) + ((NewLanguage == ScriptLanguage.CSharp) ? ".cs" : ".vb");
						if (NewLanguage == ScriptLanguage.CSharp)
						{
							doc.ScriptLanguage = NewLanguage;
							doc.Contents = ProjectParser.GetFileContents(doc.FileName);
						}
						else
						{
							doc.Contents = ProjectParser.GetFileContents(doc.FileName);
							doc.ScriptLanguage = NewLanguage;
						}
					}
					this.DocumentEvents(doc, true);
				}
			}
			if (this._winErrorList != null)
			{
				this._winErrorList.ConvertToLanguage(OldLang, NewLanguage);
			}
			this._winProjExplorer.Language = NewLanguage;
		}

		private string GetUserSrcCode()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("namespace " + this.DefaultNameSpace);
			sb.AppendLine("{");
			sb.AppendLine("\tpublic partial class " + this.DefaultClassName);
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tpublic int " + this.StartMethodName + "()");
			sb.AppendLine("\t\t{");
			sb.AppendLine("\t\t\t");
			sb.AppendLine("\t\t\treturn 0;");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			return sb.ToString();
		}

		private string GetAddObjectSrcCode()
		{
			StringBuilder sb = new StringBuilder();
			foreach (string key in this._RemoteVariables.Keys)
			{
				object obj = null;
				if (this._RemoteVariables.TryGetValue(key, out obj))
				{
					sb.Append("\t\tpublic static ");
					sb.Append(obj.GetType().BaseType.FullName);
					sb.Append(" ");
					sb.Append(key);
					sb.Append(" = null;");
					sb.AppendLine();
				}
			}
			sb.AppendLine();
			return sb.ToString();
		}

		public void AddObject(string Name, object Value)
		{
			try
			{
				this._RemoteVariables.Add(Name, Value);
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
		}

		private string GetSystemSrcCode()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("#region System Generated Source Code.Please do not change ...");
			sb.AppendLine("namespace " + this.DefaultNameSpace);
			sb.AppendLine("{");
			sb.AppendLine("\tusing System;");
			sb.AppendLine("\tusing System.Collections.Generic;");
			sb.AppendLine("\tusing System.Diagnostics;");
			sb.AppendLine("\tusing System.Reflection;");
			sb.AppendLine("\tpublic partial class " + this.DefaultClassName + " : MarshalByRefObject, IRun");
			sb.AppendLine("\t{");
			sb.AppendLine(this.GetAddObjectSrcCode());
			sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
			sb.AppendLine("\t\tvoid IRun.Initialize(IDictionary<string, object> Variables)");
			sb.AppendLine("\t\t{");
			sb.AppendLine("\t\t\tforeach (string name in Variables.Keys)");
			sb.AppendLine("\t\t\t{");
			sb.AppendLine("\t\t\t\tobject value = null;");
			sb.AppendLine("\t\t\t\ttry");
			sb.AppendLine("\t\t\t\t{");
			sb.AppendLine("\t\t\t\t\tVariables.TryGetValue(name, out value);");
			sb.AppendLine("\t\t\t\t\tFieldInfo fInfo = this.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static);");
			sb.AppendLine("\t\t\t\t\tfInfo.SetValue(this, value);");
			sb.AppendLine("\t\t\t\t}");
			sb.AppendLine("\t\t\t\tcatch(Exception ex)");
			sb.AppendLine("\t\t\t\t{");
			sb.AppendLine("\t\t\t\t\tthrow ex;");
			sb.AppendLine("\t\t\t\t}");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("");
			sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
			sb.AppendLine("\t\tobject IRun.Run(string StartMethod, params object[] Parameters)");
			sb.AppendLine("\t\t{");
			sb.AppendLine("\t\t\ttry");
			sb.AppendLine("\t\t\t{");
			sb.AppendLine("\t\t\t\tMethodInfo methodInfo = this.GetType().GetMethod(StartMethod,BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);");
			sb.AppendLine("\t\t\t\treturn methodInfo.Invoke(this, Parameters);");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t\tcatch (Exception ex)");
			sb.AppendLine("\t\t\t{");
			sb.AppendLine("\t\t\t\tthrow ex;");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("");
			sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
			sb.AppendLine("\t\tvoid IRun.Dispose(IDictionary<string, object> Variables)");
			sb.AppendLine("\t\t{");
			sb.AppendLine("\t\t\tforeach (string name in Variables.Keys)");
			sb.AppendLine("\t\t\t{");
			sb.AppendLine("\t\t\t\tobject value = null; ;");
			sb.AppendLine("\t\t\t\ttry");
			sb.AppendLine("\t\t\t\t{");
			sb.AppendLine("\t\t\t\t\tFieldInfo fInfo = this.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static);");
			sb.AppendLine("\t\t\t\t\tfInfo.SetValue(this, value);");
			sb.AppendLine("\t\t\t\t}");
			sb.AppendLine("\t\t\t\tcatch (Exception ex)");
			sb.AppendLine("\t\t\t\t{");
			sb.AppendLine("\t\t\t\t\tthrow ex;");
			sb.AppendLine("\t\t\t\t}");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			sb.AppendLine("#endregion");
			return sb.ToString();
		}

		public void StartEditor()
		{
			this.AddRefrence(new ReferenceProjectItem(ScriptControl.m_AIMSProject, "System"));
			this.AddRefrence(new ReferenceProjectItem(ScriptControl.m_AIMSProject, "System.Windows.Forms"));
			ProjectParser.ParseProjectContents("Program.Sys.cs", this.GetSystemSrcCode());
			Document doc = this.AddDocument("Program.cs");
			doc.Contents = this.GetUserSrcCode();
			doc.ParseContentsNow();
			doc.Editor.ActiveViewControl.Caret.Position = new TextPoint(0, 1);
		}

		public ScriptControl()
		{
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.InitializeComponent();
			this.InitilizeDocks();
			ProjectParser.Initilize(SupportedLanguage.CSharp);
			this.UpdateCutCopyToolbar();
			ScriptControl.m_AIMSProject = new DefaultProject();
			this._RemoteVariables = new Dictionary<string, object>();
		}

		private void ResetParserLanguage(ScriptLanguage lang)
		{
			if (lang == ScriptLanguage.CSharp)
			{
				ProjectParser.Language = SupportedLanguage.CSharp;
			}
			else
			{
				ProjectParser.Language = SupportedLanguage.VBNet;
			}
		}

		private void InitilizeDocks()
		{
			this._winErrorList = new ErrorList(this);
			this._winErrorList.Text = "Error List";
			this._winErrorList.Tag = "ERRORLIST";
			this._winErrorList.HideOnClose = true;
			this._winErrorList.Show(this.dockContainer1, DockState.DockBottomAutoHide);
			this._winProjExplorer = new ProjectExplorer();
			this._winProjExplorer.Text = "Solution Explorer";
			this._winProjExplorer.Tag = "SOLUTIONEXPLORER";
			this._winProjExplorer.HideOnClose = true;
			this._winProjExplorer.Show(this.dockContainer1, DockState.DockRightAutoHide);
			this._winOutput = new Output();
			this._winOutput.Text = "Output";
			this._winOutput.Tag = "OUTPUT";
			this._winOutput.HideOnClose = true;
			this._winOutput.Show(this.dockContainer1, DockState.DockBottomAutoHide);
			this.dockContainer1.ActiveDocumentChanged += new EventHandler(this.dockContainer1_ActiveDocumentChanged);
			this._winProjExplorer.FileClick += new EventHandler<ExplorerClickEventArgs>(this._winProjExplorer_FileClick);
			this._winProjExplorer.FileNameChanged += new EventHandler<ExplorerLabelEditEventArgs>(this._winProjExplorer_FileNameChanged);
			this._winProjExplorer.NewItemAdd += new EventHandler(this._winProjExplorer_NewItemAdd);
			this._winProjExplorer.FileItemDeleted += new EventHandler(this._winProjExplorer_FileItemDeleted);
			this._winErrorList.ItemDoubleClick += new EventHandler<ListViewItemEventArgs>(this._winErrorList_ItemDoubleClick);
			this._winProjExplorer.AddRefrenceItem += new EventHandler(this._winProjExplorer_AddRefrenceItem);
			this._winProjExplorer.AddWebRefrenceItem += new EventHandler(this._winProjExplorer_AddWebRefrenceItem);
		}

		public void AddRefrence(ProjectItem Reference)
		{
			TreeNode refNode = this._winProjExplorer.RefrenceNode;
			this.ConvertCOM(null, new ArrayList
			{
				Reference
			}, refNode);
		}

		private void _winProjExplorer_AddWebRefrenceItem(object sender, EventArgs e)
		{
			TreeNode t = (TreeNode)sender;
			StringCollection files = new StringCollection();
			foreach (string Name in ProjectParser.ProjectFiles.Keys)
			{
				files.Add(Name);
			}
			using (AddWebReferenceDialog refDialog = new AddWebReferenceDialog(ScriptControl.m_AIMSProject, this._scriptLanguage, files))
			{
				refDialog.NamespacePrefix = ScriptControl.m_AIMSProject.RootNamespace;
				if (refDialog.ShowDialog() == DialogResult.OK)
				{
					refDialog.WebReference.Name = WebReference.GetReferenceName(refDialog.WebReference.WebReferencesDirectory, refDialog.WebReference.Name);
					this.AddWebRefrenceToProject(t, refDialog.WebReference, refDialog.WebReferenceFileName);
				}
			}
		}

		private void AddWebRefrenceToProject(TreeNode node, WebReference webref, string fileName)
		{
			ProjectParser.ParseProjectContents(fileName, webref.GetSourceCode(), false);
			this._winProjExplorer.AddWebReference(fileName);
			ArrayList refItems = new ArrayList();
			foreach (ProjectItem item in webref.Items)
			{
				if (item is ReferenceProjectItem)
				{
					refItems.Add(item);
				}
			}
			if (refItems.Count > 0)
			{
				this.ConvertCOM(null, refItems, node);
			}
		}

		private void _winProjExplorer_AddRefrenceItem(object sender, EventArgs e)
		{
			TreeNode t = (TreeNode)sender;
			if (this.m_SelectRefDialog == null)
			{
				this.m_SelectRefDialog = new WeakReference(null);
			}
			if (!this.m_SelectRefDialog.IsAlive)
			{
				this.m_SelectRefDialog.Target = new SelectReferenceDialog(ScriptControl.m_AIMSProject);
			}
			SelectReferenceDialog selDialog = (SelectReferenceDialog)this.m_SelectRefDialog.Target;
			selDialog.ConfigureProject = ScriptControl.m_AIMSProject;
			if (selDialog.ShowDialog(base.ParentForm) == DialogResult.OK)
			{
				this.ConvertCOM(null, selDialog.ReferenceInformations, t);
			}
			this.m_SelectRefDialog.Target = selDialog;
		}

		private void ConvertCOM(object sender, ArrayList refrences, TreeNode node)
		{
			object[] param = new object[]
			{
				sender,
				refrences,
				node
			};
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ConvertCOMThread), param);
		}

		private void ConvertCOMThread(object stateInfo)
		{
			object[] param = (object[])stateInfo;
			object sender = param[0];
			ArrayList refrences = param[1] as ArrayList;
			TreeNode node = param[2] as TreeNode;
			base.BeginInvoke(new MethodInvoker(delegate
			{
				this.tsMessage.Text = "Please wait...";
			}));
			IEnumerator enumerator = refrences.GetEnumerator();
			try
			{
				ReferenceProjectItem reference;
				while (enumerator.MoveNext())
				{
					reference = (ReferenceProjectItem)enumerator.Current;
					try
					{
						if (reference.ItemType == ItemType.COMReference)
						{
							if (Path.IsPathRooted(reference.FileName))
							{
								ScriptControl.m_AIMSProject.AddProjectItem(reference);
							}
							else
							{
								ArrayList addedRefs = this.ImportCom(reference as ComReferenceProjectItem);
								IEnumerator enumerator2 = addedRefs.GetEnumerator();
								try
								{
									ReferenceProjectItem refs;
									while (enumerator2.MoveNext())
									{
										refs = (ReferenceProjectItem)enumerator2.Current;
										ScriptControl.m_AIMSProject.AddProjectItem(refs);
										base.BeginInvoke(new MethodInvoker(delegate
										{
											TreeNode refNode = node.Nodes.Add(refs.Name);
											refNode.ImageKey = "Reference.ico";
											refNode.Tag = NodeType.Reference;
										}));
									}
								}
								finally
								{
									IDisposable disposable = enumerator2 as IDisposable;
									if (disposable != null)
									{
										disposable.Dispose();
									}
								}
							}
						}
						else if (reference.ItemType == ItemType.Reference)
						{
							ScriptControl.m_AIMSProject.AddProjectItem(reference);
							base.BeginInvoke(new MethodInvoker(delegate
							{
								TreeNode refNode = node.Nodes.Add(reference.Name);
								refNode.ImageKey = "Reference.ico";
								refNode.Tag = NodeType.Reference;
							}));
						}
					}
					catch (Exception Ex)
					{
						MessageBox.Show(Ex.Message);
					}
					base.BeginInvoke(new MethodInvoker(delegate
					{
						this.tsMessage.Text = "Ready";
					}));
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private ArrayList ImportCom(ComReferenceProjectItem t)
		{
			ArrayList refrences = new ArrayList();
			refrences.Add(t);
			base.BeginInvoke(new MethodInvoker(delegate
			{
				this.tsMessage.Text = "Compiling COM component '" + t.Include + "' ...";
			}));
			TlbImp importer = new TlbImp(refrences);
			importer.ReportEvent += new EventHandler<ReportEventEventArgs>(this.importer_ReportEvent);
			importer.ResolveRef += new EventHandler<ResolveRefEventArgs>(this.importer_ResolveRef);
			string outputFolder = Path.GetDirectoryName(ScriptControl.m_AIMSProject.OutputAssemblyFullPath);
			string interopFileName = Path.Combine(outputFolder, "Interop." + t.Include + ".dll");
			string asmPath = interopFileName;
			importer.Import(asmPath, t.FilePath, t.Name);
			return refrences;
		}

		private void importer_ResolveRef(object sender, ResolveRefEventArgs e)
		{
			base.BeginInvoke(new MethodInvoker(delegate
			{
				this.tsMessage.Text = e.Message;
			}));
			base.BeginInvoke(new MethodInvoker(delegate
			{
				this._winOutput.AppendLine(e.Message);
			}));
		}

		private void importer_ReportEvent(object sender, ReportEventEventArgs e)
		{
			ScriptControl.<>c__DisplayClass16 <>c__DisplayClass = new ScriptControl.<>c__DisplayClass16();
			<>c__DisplayClass.e = e;
			<>c__DisplayClass.<>4__this = this;
			<>c__DisplayClass.msg = Environment.NewLine + "COM Importer Event ..." + Environment.NewLine;
			ScriptControl.<>c__DisplayClass16 expr_30 = <>c__DisplayClass;
			expr_30.msg = expr_30.msg + "Kind: " + <>c__DisplayClass.e.EventKind.ToString() + Environment.NewLine;
			ScriptControl.<>c__DisplayClass16 expr_60 = <>c__DisplayClass;
			object msg = expr_60.msg;
			expr_60.msg = string.Concat(new object[]
			{
				msg,
				"Code: ",
				<>c__DisplayClass.e.EventCode,
				Environment.NewLine
			});
			ScriptControl.<>c__DisplayClass16 expr_A1 = <>c__DisplayClass;
			expr_A1.msg = expr_A1.msg + "Message: " + <>c__DisplayClass.e.EventMsg;
			base.BeginInvoke(new MethodInvoker(delegate
			{
				<>c__DisplayClass.<>4__this.tsMessage.Text = <>c__DisplayClass.e.EventMsg;
			}));
			base.BeginInvoke(new MethodInvoker(delegate
			{
				<>c__DisplayClass.<>4__this._winOutput.AppendLine(<>c__DisplayClass.msg);
			}));
		}

		private void _winProjExplorer_FileItemDeleted(object sender, EventArgs e)
		{
			TreeNode node = (TreeNode)sender;
			Document doc = this.GetExistingFile(node.Text);
			if (doc != null)
			{
				doc.Close();
			}
			node.Remove();
			ProjectParser.RemoveContentFile(node.Text);
		}

		private void _winProjExplorer_NewItemAdd(object sender, EventArgs e)
		{
			this.AddNewItem();
		}

		private void _winProjExplorer_FileNameChanged(object sender, ExplorerLabelEditEventArgs e)
		{
			if (this.ValidateFileName(e.NewName))
			{
				string contents = ProjectParser.GetFileContents(e.OldName);
				ProjectParser.ProjectFiles.Remove(e.OldName);
				ProjectParser.ParseProjectContents(e.NewName, contents);
				Document doc = this.GetExistingFile(e.OldName);
				if (doc != null)
				{
					doc.Text = Path.GetFileNameWithoutExtension(e.NewName);
					doc.FileName = e.NewName;
				}
			}
			else
			{
				MessageBox.Show("File Name '" + e.NewName + "' already exists in the project.Please try other name.");
				e.Cancel = true;
			}
		}

		private bool ValidateFileName(string fileName)
		{
			string[] keys = new string[ProjectParser.ProjectFiles.Keys.Count];
			ProjectParser.ProjectFiles.Keys.CopyTo(keys, 0);
			bool result;
			for (int count = 0; count <= keys.Length - 1; count++)
			{
				if (keys[count].ToLower() == fileName.ToLower())
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}

		private void _winErrorList_ItemDoubleClick(object sender, ListViewItemEventArgs e)
		{
			System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
			tmr.Tick += new EventHandler(this.ShowIntoView);
			tmr.Interval = 500;
			Document doc = this.ShowFile(e.FileName);
			tmr.Tag = e;
			tmr.Start();
		}

		private void ShowIntoView(object sender, EventArgs e)
		{
			System.Windows.Forms.Timer tmr = (System.Windows.Forms.Timer)sender;
			tmr.Stop();
			ListViewItemEventArgs et = (ListViewItemEventArgs)tmr.Tag;
			Document doc = this.ShowFile(et.FileName);
			TextPoint t = new TextPoint(et.ColumnNo, et.LineNo);
			doc.Editor.ScrollIntoView(t);
			doc.Editor.Caret.SetPos(t);
		}

		private Document GetExistingFile(string FileName)
		{
			Document result;
			foreach (IDockableWindow docWin in this.dockContainer1.Contents)
			{
				if (docWin is Document)
				{
					Document doc = docWin as Document;
					if (doc.FileName == FileName)
					{
						result = doc;
						return result;
					}
				}
			}
			result = null;
			return result;
		}

		public Document ShowFile(string FileName)
		{
			Document doc;
			Document result;
			foreach (IDockableWindow docWin in this.dockContainer1.Contents)
			{
				if (docWin is Document)
				{
					doc = (docWin as Document);
					if (doc.FileName == FileName)
					{
						doc.Show(this.dockContainer1, DockState.Document);
						result = doc;
						return result;
					}
				}
			}
			doc = this.OpenDocument(FileName);
			if (doc != null)
			{
				doc.Activate();
				doc.Focus();
			}
			result = doc;
			return result;
		}

		private void _winProjExplorer_FileClick(object sender, ExplorerClickEventArgs e)
		{
			Document doc = this.ShowFile(e.FileName);
			if (doc != null)
			{
				doc.ParseContentsNow();
			}
		}

		private void dockContainer1_ActiveDocumentChanged(object sender, EventArgs e)
		{
			if (this.dockContainer1.ActiveDocument is Document)
			{
				Document doc = this.dockContainer1.ActiveDocument as Document;
				this._winProjExplorer.ActiveNode(doc.FileName);
			}
			this.UpdateCutCopyToolbar();
		}

		public Document AddDocument(string Name)
		{
			return this.AddDocument(Name, false);
		}

		public Document AddDocument(string Name, bool IsWebReference)
		{
			Document doc = new Document(this);
			doc.FileName = Name;
			doc.Text = Path.GetFileNameWithoutExtension(Name);
			doc.Tag = "USERDOCUMENT";
			doc.HideOnClose = false;
			doc.ScriptLanguage = this._scriptLanguage;
			this.DocumentEvents(doc, true);
			doc.Show(this.dockContainer1, DockState.Document);
			if (IsWebReference)
			{
				this._winProjExplorer.AddWebReference(Name);
			}
			else
			{
				this._winProjExplorer.AddFile(Name);
			}
			ProjectParser.ParseProjectContents(Name, "");
			return doc;
		}

		public Document OpenDocument(string Name)
		{
			string contents = ProjectParser.GetFileContents(Name);
			Document result;
			if (contents == string.Empty)
			{
				result = null;
			}
			else
			{
				Document doc = new Document(this);
				doc.FileName = Name;
				doc.Text = Path.GetFileNameWithoutExtension(Name);
				doc.Tag = "USERDOCUMENT";
				doc.HideOnClose = false;
				doc.ScriptLanguage = this._scriptLanguage;
				this.DocumentEvents(doc, true);
				doc.Show(this.dockContainer1, DockState.Document);
				doc.Contents = contents;
				ProjectParser.ParseProjectContents(Name, contents);
				result = doc;
			}
			return result;
		}

		private void DocumentEvents(Document doc, bool Enable)
		{
			if (Enable)
			{
				doc.ParseContent += new EventHandler<ParseContentEventArgs>(this.doc_ParseContent);
				doc.FormClosing += new FormClosingEventHandler(this.doc_FormClosing);
				doc.CaretChange += new EventHandler<EventArgs>(this.doc_CaretChange);
				doc.Editor.Selection.Change += new EventHandler(this.Selection_Change);
			}
			else
			{
				doc.ParseContent -= new EventHandler<ParseContentEventArgs>(this.doc_ParseContent);
				doc.FormClosing -= new FormClosingEventHandler(this.doc_FormClosing);
				doc.CaretChange -= new EventHandler<EventArgs>(this.doc_CaretChange);
				doc.Editor.Selection.Change -= new EventHandler(this.Selection_Change);
			}
		}

		private void Selection_Change(object sender, EventArgs e)
		{
			this.UpdateCutCopyToolbar();
		}

		private void doc_CaretChange(object sender, EventArgs e)
		{
			this.UpdateCutCopyToolbar();
			Caret c = ((Document)sender).Editor.ActiveViewControl.Caret;
			this.tCursorPos.Text = string.Concat(new object[]
			{
				"Ln ",
				c.Position.Y,
				", Col ",
				c.Position.X
			});
		}

		private void doc_ParseContent(object sender, ParseContentEventArgs e)
		{
			this.DoParsing(sender, e, true);
		}

		private void DoParsing(object sender, ParseContentEventArgs e, bool IsOpened)
		{
			object[] param = new object[]
			{
				sender,
				e,
				true
			};
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ParseContentThread), param);
		}

		private void ParseContentThread(object stateInfo)
		{
			object[] param = (object[])stateInfo;
			object sender = param[0];
			ParseContentEventArgs e = param[1] as ParseContentEventArgs;
			bool IsOpened = (bool)param[2];
			ParseInformation pi = ProjectParser.ParseProjectContents(e.FileName, e.Content, IsOpened);
			Errors errors = ProjectParser.LastParserErrors;
			Document doc = sender as Document;
			base.BeginInvoke(new MethodInvoker(delegate
			{
				this.UploadParserError(doc, errors);
			}));
		}

		private void UploadParserError(Document doc, Errors e)
		{
			if (this._winErrorList != null)
			{
				this._winErrorList.ProjectErrors(doc, e);
			}
		}

		private void doc_FormClosing(object sender, FormClosingEventArgs e)
		{
			Document doc = sender as Document;
			this.DocumentEvents(doc, false);
			ProjectParser.ProjectFiles[doc.FileName].IsOpened = false;
		}

		private void cNetToolStripMenuItemCSharp_Click(object sender, EventArgs e)
		{
			if (this._winErrorList.ParserErrorCount == 0)
			{
				if (this._scriptLanguage != ScriptLanguage.CSharp)
				{
					this.ScriptLanguage = ScriptLanguage.CSharp;
				}
			}
			else
			{
				MessageBox.Show("Remove Parsing errors before converting");
			}
		}

		private void vBNetToolStripMenuItemVbNet_Click(object sender, EventArgs e)
		{
			if (this._winErrorList.ParserErrorCount == 0)
			{
				if (this._scriptLanguage != ScriptLanguage.VBNET)
				{
					this.ScriptLanguage = ScriptLanguage.VBNET;
				}
			}
			else
			{
				MessageBox.Show("Remove Parsing errors before converting");
			}
		}

		private void tsbCut_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				view.Cut();
			}
			this.UpdateCutCopyToolbar();
		}

		private void tsbCopy_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				if (view.CanCopy)
				{
					view.Copy();
				}
			}
			this.UpdateCutCopyToolbar();
		}

		private EditViewControl GetCurrentView()
		{
			Document doc = this.dockContainer1.ActiveDocument as Document;
			EditViewControl result;
			if (doc != null)
			{
				result = doc.CurrentView;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void tsbPaste_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				if (view.CanPaste)
				{
					view.Paste();
				}
			}
			this.UpdateCutCopyToolbar();
		}

		private void tsbUndo_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				if (view.CanUndo)
				{
					view.Undo();
				}
			}
			this.UpdateCutCopyToolbar();
		}

		private void tsbRedo_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				if (view.CanRedo)
				{
					view.Redo();
				}
			}
			this.UpdateCutCopyToolbar();
		}

		private void tsbToggleBookmark_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				view.ToggleBookmark();
			}
		}

		private void tsbPreBookmark_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				view.GotoPreviousBookmark();
			}
		}

		private void tsbNextBookmark_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				view.GotoNextBookmark();
			}
		}

		private void tsbDelAllBookmark_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete all of the bookmark(s).", "AIMS Script Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				EditViewControl view = this.GetCurrentView();
				if (view != null)
				{
					view.Document.ClearBookmarks();
				}
			}
		}

		private void tsbDelallBreakPoints_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to remove all of the break point(s).", "AIMS Script Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				EditViewControl view = this.GetCurrentView();
				if (view != null)
				{
					view.Document.ClearBreakpoints();
				}
			}
		}

		private void tsbFind_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				view.ShowFind();
			}
		}

		private void tsbReplace_Click(object sender, EventArgs e)
		{
			EditViewControl view = this.GetCurrentView();
			if (view != null)
			{
				view.ShowReplace();
			}
		}

		private void tsbErrorList_Click(object sender, EventArgs e)
		{
			this._winErrorList.Show();
			this._winOutput.Show();
		}

		private void tsbBuild_Click(object sender, EventArgs e)
		{
			CompilerResults results = this.CompileScript();
			this.LoadComileErrors(results.Errors);
		}

		private CompilerResults CompileScript()
		{
			CodeDomProvider provider = ScriptControl.m_AIMSProject.LanguageProperties.CodeDomProvider;
			CompilerParameters parameters = new CompilerParameters();
			parameters.GenerateExecutable = false;
			parameters.GenerateInMemory = false;
			parameters.IncludeDebugInformation = true;
			parameters.OutputAssembly = ScriptControl.m_AIMSProject.OutputAssemblyFullPath;
			foreach (ProjectItem item in ScriptControl.m_AIMSProject.Items)
			{
				parameters.ReferencedAssemblies.Add(item.Include + ".dll");
			}
			parameters.ReferencedAssemblies.Add("AIMS.Scripting.ScriptRun.dll");
			string[] sourceCode = new string[ProjectParser.ProjectFiles.Count];
			int counter = 0;
			string tmpFilePath = Path.Combine(Path.GetDirectoryName(ScriptControl.m_AIMSProject.OutputAssemblyFullPath), "Temp");
			if (Directory.Exists(tmpFilePath))
			{
				Directory.Delete(tmpFilePath, true);
			}
			Directory.CreateDirectory(tmpFilePath);
			foreach (ProjectContentItem pcItem in ProjectParser.ProjectFiles.Values)
			{
				StreamWriter writer = new StreamWriter(Path.Combine(tmpFilePath, pcItem.FileName), false);
				writer.Write(pcItem.Contents);
				writer.Close();
				sourceCode[counter++] = Path.Combine(tmpFilePath, pcItem.FileName);
			}
			CompilerResults results = provider.CompileAssemblyFromFile(parameters, sourceCode);
			Directory.Delete(tmpFilePath, true);
			return results;
		}

		private void tsbRun_Click(object sender, EventArgs e)
		{
			this.OnExecute();
		}

		private void tsbSave_Click(object sender, EventArgs e)
		{
		}

		private void tsbNew_Click(object sender, EventArgs e)
		{
			this.AddNewItem();
		}

		private void AddNewItem()
		{
			this.dockContainer1.SuspendLayout();
			StringCollection files = new StringCollection();
			foreach (string Name in ProjectParser.ProjectFiles.Keys)
			{
				files.Add(Name);
			}
			NewFileDialog f = new NewFileDialog(this._scriptLanguage, files);
			f.ShowDialog(this.dockContainer1);
			string fileName = f.FileName;
			if (fileName.Length > 0)
			{
				Document doc = this.AddDocument(fileName);
				doc.Editor.ActiveViewControl.Document.Text = this.GetInitialContents(f, fileName);
				doc.ParseContentsNow();
				doc.Editor.ActiveViewControl.Caret.Position = new TextPoint(0, 1);
			}
			this.dockContainer1.ResumeLayout();
		}

		private string GetInitialContents(NewFileDialog f, string fileName)
		{
			string defNameSpace = NewFileDialog.GetDefaultNamespace(ScriptControl.m_AIMSProject, fileName);
			string defClassName = NewFileDialog.GenerateValidClassOrNamespaceName(Path.GetFileNameWithoutExtension(fileName), true);
			StringBuilder contents = new StringBuilder();
			if (this._scriptLanguage == ScriptLanguage.CSharp)
			{
				contents.AppendLine("#region Usings ...");
				contents.AppendLine("using System;");
				contents.AppendLine("using System.Collections.Generic;");
				contents.AppendLine("using System.Text;");
				contents.AppendLine("#endregion");
				contents.AppendLine("");
				contents.AppendLine("namespace " + defNameSpace);
				contents.AppendLine("{");
				contents.AppendLine("    " + ((f.SelectedItemType == SelectedItemType.Class) ? "class " : "interface ") + defClassName);
				contents.AppendLine("    {");
				contents.AppendLine("");
				contents.AppendLine("    }");
				contents.AppendLine("}");
			}
			else
			{
				contents.AppendLine("#Region Usings ...");
				contents.AppendLine("Imports System");
				contents.AppendLine("Imports System.Collections.Generic");
				contents.AppendLine("Imports System.Text");
				contents.AppendLine("#End Region");
				contents.AppendLine("");
				contents.AppendLine("Namespace " + defNameSpace);
				contents.AppendLine("    " + ((f.SelectedItemType == SelectedItemType.Class) ? "Class " : "Interface ") + defClassName);
				contents.AppendLine("");
				contents.AppendLine("    End " + ((f.SelectedItemType == SelectedItemType.Class) ? "Class" : "Interface"));
				contents.AppendLine("End Namespace");
			}
			return contents.ToString();
		}

		private void UpdateCutCopyToolbar()
		{
			EditViewControl ev = this.GetCurrentView();
			if (ev != null)
			{
				this.tsbComment.Enabled = true;
				this.tsbFind.Enabled = true;
				this.tsbReplace.Enabled = true;
				this.tsbToggleBookmark.Enabled = true;
				this.tsbUnComment.Enabled = true;
				this.tsbCut.Enabled = ev.Selection.IsValid;
				this.tsbCopy.Enabled = ev.Selection.IsValid;
				this.tsbPaste.Enabled = ev.CanPaste;
				this.tsbRedo.Enabled = ev.CanRedo;
				this.tsbUndo.Enabled = ev.CanUndo;
			}
			else
			{
				this.tsbCut.Enabled = false;
				this.tsbCopy.Enabled = false;
				this.tsbPaste.Enabled = false;
				this.tsbRedo.Enabled = false;
				this.tsbUndo.Enabled = false;
				this.tsbComment.Enabled = false;
				this.tsbFind.Enabled = false;
				this.tsbReplace.Enabled = false;
				this.tsbToggleBookmark.Enabled = false;
				this.tsbUnComment.Enabled = false;
			}
		}

		public void LoadComileErrors(CompilerErrorCollection Errors)
		{
			this._winErrorList.ComilerErrors(null, Errors);
		}

		public static AutoListIcons GetIcon(IClass c)
		{
			AutoListIcons imageIndex = AutoListIcons.iClass;
			switch (c.ClassType)
			{
			case ClassType.Enum:
				imageIndex = AutoListIcons.iEnum;
				break;
			case ClassType.Interface:
				imageIndex = AutoListIcons.iInterface;
				break;
			case ClassType.Struct:
				imageIndex = AutoListIcons.iStructure;
				break;
			case ClassType.Delegate:
				imageIndex = AutoListIcons.iDelegate;
				break;
			}
			return imageIndex + ScriptControl.GetModifierOffset(c.Modifiers);
		}

		public static AutoListIcons GetIcon(IMethod method)
		{
			return AutoListIcons.iMethod + ScriptControl.GetModifierOffset(method.Modifiers);
		}

		private static int GetModifierOffset(ModifierEnum modifier)
		{
			int result;
			if ((modifier & ModifierEnum.Public) == ModifierEnum.Public)
			{
				result = 0;
			}
			else if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected)
			{
				result = 3;
			}
			else if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal)
			{
				result = 4;
			}
			else
			{
				result = 2;
			}
			return result;
		}

		public static AutoListIcons GetIcon(IField field)
		{
			AutoListIcons result;
			if (field.IsConst)
			{
				result = AutoListIcons.iConstant;
			}
			else if (field.IsParameter)
			{
				result = AutoListIcons.iProperties;
			}
			else if (field.IsLocalVariable)
			{
				result = AutoListIcons.iField;
			}
			else
			{
				result = AutoListIcons.iField + ScriptControl.GetModifierOffset(field.Modifiers);
			}
			return result;
		}

		public static AutoListIcons GetIcon(IProperty property)
		{
			AutoListIcons result;
			if (property.IsIndexer)
			{
				result = AutoListIcons.iProperties + ScriptControl.GetModifierOffset(property.Modifiers);
			}
			else
			{
				result = AutoListIcons.iProperties + ScriptControl.GetModifierOffset(property.Modifiers);
			}
			return result;
		}

		public static AutoListIcons GetIcon(IEvent evt)
		{
			return AutoListIcons.iEvent + ScriptControl.GetModifierOffset(evt.Modifiers);
		}

		internal static IProject GetProject()
		{
			return ScriptControl.m_AIMSProject;
		}

		private void tsbComment_Click(object sender, EventArgs e)
		{
			this.tsbComment.Enabled = false;
			this.toolStrip1.Refresh();
			EditViewControl ev = this.GetCurrentView();
			if (ev != null)
			{
				this.CommentCode(ev, true);
			}
			this.tsbComment.Enabled = true;
		}

		private void CommentCode(EditViewControl ev, bool comment)
		{
			int startRow = Math.Min(ev.Selection.Bounds.FirstRow, ev.Selection.Bounds.LastRow);
			int lastRow = Math.Max(ev.Selection.Bounds.FirstRow, ev.Selection.Bounds.LastRow);
			int lastCol = ev.Document.VisibleRows[lastRow].Count;
			if (lastCol >= 0)
			{
				lastCol = Math.Max(lastCol, ev.Document.VisibleRows[lastRow].Expansion_EndChar);
			}
			else
			{
				lastCol = 0;
			}
			bool Changed = false;
			TextRange tr = new TextRange(0, startRow, lastCol, lastRow);
			TextRange trFinal = new TextRange(0, startRow, lastCol + 2, lastRow);
			string txt = ev.Document.GetRange(tr);
			string[] rows = txt.Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.None);
			string[] output = new string[rows.Length];
			ev.Selection.Bounds = tr;
			for (int count = 0; count <= lastRow - startRow; count++)
			{
				bool found = false;
				string rowText = rows[count];
				int startIndex = rowText.Length - rowText.TrimStart(null).Length;
				string wText = rowText.Substring(startIndex);
				if (comment)
				{
					if (wText.Length > 0)
					{
						wText = ((this._scriptLanguage == ScriptLanguage.CSharp) ? "//" : "'") + wText;
						found = true;
					}
				}
				else if (this._scriptLanguage == ScriptLanguage.CSharp)
				{
					if (wText.Length >= 2 && wText.Substring(0, 2) == "//")
					{
						if (wText.Length >= 3 && wText.Substring(2, 1) != "/")
						{
							wText = wText.Substring(2);
							found = true;
						}
						else if ((wText.Length > 3 && wText.Substring(0, 3) != "///") || (wText.Length > 4 && wText.Substring(0, 4) == "////"))
						{
							wText = wText.Substring(2);
							found = true;
						}
					}
				}
				else if (wText.Length >= 1 && wText.Substring(0, 1) == "'")
				{
					wText = wText.Substring(1);
					found = true;
				}
				if (found && rowText.Length > 0)
				{
					output[count] = rowText.Substring(0, startIndex) + wText;
					Changed = true;
				}
				else
				{
					output[count] = rowText;
				}
			}
			if (Changed)
			{
				string pReplacedData = string.Join("\r\n", output);
				ev.ReplaceSelection(pReplacedData);
				ev.Selection.Bounds = trFinal;
			}
		}

		private void tsbUnComment_Click(object sender, EventArgs e)
		{
			this.tsbUnComment.Enabled = false;
			this.toolStrip1.Refresh();
			EditViewControl ev = this.GetCurrentView();
			if (ev != null)
			{
				this.CommentCode(ev, false);
			}
			this.tsbUnComment.Enabled = true;
		}

		private void tsbSolutionExplorer_Click(object sender, EventArgs e)
		{
			this._winProjExplorer.Show();
		}
	}
}

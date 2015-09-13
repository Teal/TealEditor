using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.SyntaxFiles;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Forms.Docking;
using AIMS.Libraries.Scripting.CodeCompletion;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class Document : DockableWindow
	{
		private IContainer components = null;

		private CodeEditorControl CodeEditorCtrl;

		private SyntaxDocument syntaxDocument1;

		private string _FileName = "";

		private string _contents = "";

		private ScriptLanguage _scriptLanguage;

		private ScriptControl _Parent = null;

		private QuickClassBrowserPanel quickClassBrowserPanel = null;

		private Word lastErrorWord = null;

		private ICodeCompletionBinding[] codeCompletionBindings;

		private bool inHandleKeyPress;

		[CompilerGenerated]
		private static Func<IProjectContent> <>9__CachedAnonymousMethodDelegate5;

		[CompilerGenerated]
		private static Action<string, Exception> <>9__CachedAnonymousMethodDelegate6;

		[CompilerGenerated]
		private static Action<string> <>9__CachedAnonymousMethodDelegate7;

		[CompilerGenerated]
		private static Action<string, string, string> <>9__CachedAnonymousMethodDelegate8;

		public event EventHandler<EventArgs> CaretChange
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.CaretChange = (EventHandler<EventArgs>)Delegate.Combine(this.CaretChange, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.CaretChange = (EventHandler<EventArgs>)Delegate.Remove(this.CaretChange, value);
			}
		}

		public event EventHandler<ParseContentEventArgs> ParseContent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ParseContent = (EventHandler<ParseContentEventArgs>)Delegate.Combine(this.ParseContent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ParseContent = (EventHandler<ParseContentEventArgs>)Delegate.Remove(this.ParseContent, value);
			}
		}

		public ScriptControl ParentScriptControl
		{
			get
			{
				return this._Parent;
			}
		}

		public CodeEditorControl Editor
		{
			get
			{
				return this.CodeEditorCtrl;
			}
		}

		public QuickClassBrowserPanel QuickClassBrowserPanel
		{
			get
			{
				return this.quickClassBrowserPanel;
			}
		}

		public ICodeCompletionBinding[] CodeCompletionBindings
		{
			get
			{
				if (this.codeCompletionBindings == null)
				{
					try
					{
						this.codeCompletionBindings = this.GetCompletionBinding();
					}
					catch
					{
						this.codeCompletionBindings = new ICodeCompletionBinding[0];
					}
				}
				return this.codeCompletionBindings;
			}
		}

		public string Contents
		{
			get
			{
				return this._contents;
			}
			set
			{
				this.CodeEditorCtrl.ActiveViewControl.Document.Text = value;
			}
		}

		public string FileName
		{
			get
			{
				return this._FileName;
			}
			set
			{
				this._FileName = value;
				this.CodeEditorCtrl.ActiveViewControl.FileName = value;
				this.CodeEditorCtrl.FileName = value;
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
					CodeEditorSyntaxLoader.SetSyntax(this.CodeEditorCtrl, SyntaxLanguage.CSharp);
				}
				else
				{
					CodeEditorSyntaxLoader.SetSyntax(this.CodeEditorCtrl, SyntaxLanguage.VBNET);
				}
				this._scriptLanguage = value;
				this.GetCompletionBinding();
				this.CodeEditorCtrl.ScrollIntoView(0);
				this.OnCaretChange(null);
			}
		}

		public EditViewControl CurrentView
		{
			get
			{
				return this.CodeEditorCtrl.ActiveViewControl;
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
			LineMarginRender lineMarginRender = new LineMarginRender();
			this.CodeEditorCtrl = new CodeEditorControl();
			this.syntaxDocument1 = new SyntaxDocument(this.components);
			base.SuspendLayout();
			this.CodeEditorCtrl.ActiveView = ActiveView.BottomRight;
			this.CodeEditorCtrl.AutoListPosition = null;
			this.CodeEditorCtrl.AutoListSelectedText = "";
			this.CodeEditorCtrl.AutoListVisible = false;
			this.CodeEditorCtrl.BorderColor = Color.White;
			this.CodeEditorCtrl.BorderStyle = ControlBorderStyle.FixedSingle;
			this.CodeEditorCtrl.ChildBorderColor = SystemColors.Window;
			this.CodeEditorCtrl.ChildBorderStyle = ControlBorderStyle.None;
			this.CodeEditorCtrl.CopyAsRTF = false;
			this.CodeEditorCtrl.Dock = DockStyle.Fill;
			this.CodeEditorCtrl.FileName = null;
			this.CodeEditorCtrl.InfoTipCount = 1;
			this.CodeEditorCtrl.InfoTipPosition = null;
			this.CodeEditorCtrl.InfoTipSelectedIndex = 1;
			this.CodeEditorCtrl.InfoTipVisible = false;
			lineMarginRender.Bounds = new Rectangle(19, 0, 19, 16);
			this.CodeEditorCtrl.LineMarginRender = lineMarginRender;
			this.CodeEditorCtrl.Location = new Point(0, 0);
			this.CodeEditorCtrl.LockCursorUpdate = false;
			this.CodeEditorCtrl.Name = "CodeEditorCtrl";
			this.CodeEditorCtrl.Saved = false;
			this.CodeEditorCtrl.ShowScopeIndicator = false;
			this.CodeEditorCtrl.Size = new Size(454, 335);
			this.CodeEditorCtrl.SmoothScroll = false;
			this.CodeEditorCtrl.SplitView = false;
			this.CodeEditorCtrl.SplitviewH = -4;
			this.CodeEditorCtrl.SplitviewV = -4;
			this.CodeEditorCtrl.TabGuideColor = Color.FromArgb(222, 219, 214);
			this.CodeEditorCtrl.TabIndex = 0;
			this.CodeEditorCtrl.WhitespaceColor = SystemColors.ControlDark;
			this.syntaxDocument1.Lines = new string[]
			{
				""
			};
			this.syntaxDocument1.MaxUndoBufferSize = 1000;
			this.syntaxDocument1.Modified = false;
			this.syntaxDocument1.UndoStep = 0;
			this.BackColor = SystemColors.Window;
			base.ClientSize = new Size(454, 335);
			base.Controls.Add(this.CodeEditorCtrl);
			this.DoubleBuffered = true;
			base.Name = "Document";
			base.ResumeLayout(false);
		}

		public Document(ScriptControl Parent) : this()
		{
			this._Parent = Parent;
		}

		public Document()
		{
			this.InitializeComponent();
			this.CodeEditorCtrl.Document = this.syntaxDocument1;
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.CodeEditorCtrl.Indent = IndentStyle.LastRow;
			this.CodeEditorCtrl.LineNumberForeColor = Color.FromArgb(50, this.CodeEditorCtrl.LineNumberForeColor);
			this.CodeEditorCtrl.LineNumberBorderColor = Color.FromArgb(50, this.CodeEditorCtrl.LineNumberBorderColor);
			this.CodeEditorCtrl.TextChanged += new EventHandler(this.ActiveViewControl_TextChanged);
			this.CodeEditorCtrl.ActiveViewControl.KeyDown += new KeyEventHandler(this.ActiveViewControl_KeyDown);
			this.CodeEditorCtrl.ActiveViewControl.KeyPress += new KeyPressEventHandler(this.ActiveViewControl_KeyPress);
			this.CodeEditorCtrl.CaretChange += new EventHandler(this.CodeEditorCtrl_CaretChange);
			this.HostCallBackRegister();
			this.ShowQuickClassBrowserPanel();
		}

		private void CodeEditorCtrl_CaretChange(object sender, EventArgs e)
		{
			this.OnCaretChange(e);
		}

		protected virtual void OnCaretChange(EventArgs e)
		{
			if (this.CaretChange != null)
			{
				this.CaretChange(this, e);
			}
		}

		public void JumpToFilePosition(string fileName, int Line, int Column)
		{
			this.CodeEditorCtrl.ActiveViewControl.Caret.Position = new TextPoint(Column, Line);
		}

		private void ActiveViewControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = this.HandleKeyPress(e.KeyChar);
			if (!e.Handled && e.KeyChar == '.' && !this.CodeEditorCtrl.AutoListVisible)
			{
				string SearchWord = this.Editor.ActiveViewControl.Caret.CurrentWord.Text;
				string FoundWord = "";
				ICompletionDataProvider cdp = new CtrlSpaceCompletionDataProvider();
				ICompletionData[] completionData = cdp.GenerateCompletionData(this.FileName, this.Editor.ActiveViewControl, e.KeyChar);
				ICompletionData[] array = completionData;
				for (int i = 0; i < array.Length; i++)
				{
					ICompletionData data = array[i];
					if (SearchWord.ToLower() == data.Text.ToLower())
					{
						FoundWord = data.Text;
						break;
					}
				}
				if (FoundWord.Length > 0)
				{
					this.Editor.ActiveViewControl.Caret.CurrentWord.Text = FoundWord;
					e.Handled = this.HandleKeyPress(e.KeyChar);
				}
			}
		}

		private bool IsVariable(string varName)
		{
			return false;
		}

		private void ActiveViewControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control & e.KeyCode == Keys.Space)
			{
				this.CodeEditorCtrl.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(), '\0');
				e.Handled = true;
			}
		}

		private ICodeCompletionBinding[] GetCompletionBinding()
		{
			ICodeCompletionBinding[] bindings;
			if (this._scriptLanguage == ScriptLanguage.CSharp)
			{
				bindings = new ICodeCompletionBinding[]
				{
					new CSharpCompletionBinding()
				};
			}
			else
			{
				bindings = new ICodeCompletionBinding[]
				{
					new VBNetCompletionBinding()
				};
			}
			return bindings;
		}

		private bool HandleKeyPress(char ch)
		{
			bool result;
			if (this.inHandleKeyPress)
			{
				result = false;
			}
			else
			{
				this.inHandleKeyPress = true;
				try
				{
					if (CodeCompletionOptions.EnableCodeCompletion && !this.Editor.AutoListVisible)
					{
						ICodeCompletionBinding[] array = this.CodeCompletionBindings;
						for (int i = 0; i < array.Length; i++)
						{
							ICodeCompletionBinding ccBinding = array[i];
							if (ccBinding.HandleKeyPress(this.CodeEditorCtrl, ch))
							{
								result = false;
								return result;
							}
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
				finally
				{
					this.inHandleKeyPress = false;
				}
				result = false;
			}
			return result;
		}

		private bool IsInComment(EditViewControl editor)
		{
			CSharpExpressionFinder ef = new CSharpExpressionFinder(this.FileName);
			int cursor = editor.Caret.Offset - 1;
			return ef.FilterComments(this.Contents, ref cursor) == null;
		}

		private void ActiveViewControl_TextChanged(object sender, EventArgs e)
		{
			this._contents = this.CodeEditorCtrl.ActiveViewControl.Document.Text;
			ParseContentEventArgs eInfo = new ParseContentEventArgs(this.FileName, this._contents);
			this.OnParseContent(ref eInfo);
		}

		protected virtual void OnParseContent(ref ParseContentEventArgs e)
		{
			if (this.ParseContent != null)
			{
				this.ParseContent(this, e);
			}
		}

		public void HighlightRemove(int LineNo, int ColNo)
		{
			SyntaxDocument Doc = this.CodeEditorCtrl.ActiveViewControl.Document;
			Word curWord = Doc.GetWordFromPos(new TextPoint(ColNo, LineNo));
			if (curWord != null)
			{
				curWord.HasError = false;
				curWord.HasWarning = false;
				curWord.InfoTip = "";
			}
			if (this.lastErrorWord != null)
			{
				this.lastErrorWord.HasError = false;
				this.lastErrorWord.HasWarning = false;
				this.lastErrorWord.InfoTip = "";
			}
		}

		public void HighlightError(int LineNo, int ColNo, bool IsWarning, string error)
		{
			SyntaxDocument Doc = this.CodeEditorCtrl.ActiveViewControl.Document;
			Word curWord = Doc.GetWordFromPos(new TextPoint(ColNo, LineNo));
			if (curWord != null)
			{
				if (IsWarning)
				{
					curWord.HasWarning = true;
				}
				else
				{
					curWord.HasError = true;
				}
				curWord.InfoTip = error;
			}
			this.lastErrorWord = curWord;
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			this.CodeEditorCtrl.Focus();
			this.ParseContentsNow();
		}

		public void ParseContentsNow()
		{
			this.ActiveViewControl_TextChanged(null, null);
			this.quickClassBrowserPanel.PopulateCombo();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.CodeEditorCtrl.Focus();
		}

		public void HostCallBackRegister()
		{
			HostCallback.GetParseInformation = ((string fileName) => ProjectParser.GetParseInformation(this.FileName));
			HostCallback.GetCurrentProjectContent = (() => ProjectParser.CurrentProjectContent);
			HostCallback.ShowError = (Action<string, Exception>)Delegate.Combine(HostCallback.ShowError, new Action<string, Exception>(delegate(string message, Exception ex)
			{
				MessageBox.Show(message + Environment.NewLine + ex.ToString());
			}));
			HostCallback.ShowMessage = (Action<string>)Delegate.Combine(HostCallback.ShowMessage, new Action<string>(delegate(string message)
			{
				MessageBox.Show(message);
			}));
			HostCallback.ShowAssemblyLoadError = (Action<string, string, string>)Delegate.Combine(HostCallback.ShowAssemblyLoadError, new Action<string, string, string>(delegate(string fileName, string include, string message)
			{
				MessageBox.Show(string.Concat(new string[]
				{
					"Error loading code-completion information for ",
					include,
					" from ",
					fileName,
					":\r\n",
					message,
					"\r\n"
				}));
			}));
		}

		public void HostCallBackUnRegister()
		{
			HostCallback.GetParseInformation = null;
			HostCallback.GetCurrentProjectContent = null;
			HostCallback.ShowError = null;
			HostCallback.ShowMessage = null;
			HostCallback.ShowAssemblyLoadError = null;
		}

		private void ShowQuickClassBrowserPanel()
		{
			if (this.quickClassBrowserPanel == null)
			{
				this.quickClassBrowserPanel = new QuickClassBrowserPanel(this.CodeEditorCtrl);
				base.Controls.Add(this.quickClassBrowserPanel);
				this.quickClassBrowserPanel.BackColor = this.CodeEditorCtrl.GutterMarginColor;
				this.CodeEditorCtrl.BorderStyle = ControlBorderStyle.None;
				this.CodeEditorCtrl.ActiveViewControl.BorderColor = this.CodeEditorCtrl.GutterMarginBorderColor;
				this.CodeEditorCtrl.ActiveViewControl.BorderStyle = ControlBorderStyle.FixedSingle;
			}
		}

		private void RemoveQuickClassBrowserPanel()
		{
			if (this.quickClassBrowserPanel != null)
			{
				base.Controls.Remove(this.quickClassBrowserPanel);
				this.quickClassBrowserPanel.Dispose();
				this.quickClassBrowserPanel = null;
			}
		}
	}
}

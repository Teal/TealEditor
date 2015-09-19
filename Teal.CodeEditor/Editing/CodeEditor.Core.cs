using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor : Control {

        /// <summary>
        /// 获取或设置当前编辑器对应的文档。
        /// </summary>
        public Document document = new Document();

        /// <summary>
        /// 获取或设置当前编辑器的配置。
        /// </summary>
        public DocumentConfigs configs {
            get {
                return document.configs;
            }
            set {
                document.configs = value;
            }
        }

        /// <summary>
        /// 获取或设置当前编辑器的语法绑定。
        /// </summary>
        public SyntaxBinding syntaxBinding {
            get {
                return document.syntaxBinding;
            }
            set {
                document.syntaxBinding = value;
            }
        }

        /// <summary>
        /// 初始化 <see cref="CodeEditor"/> 类的新实例。
        /// </summary>
        public CodeEditor() {

            //            _synataxBinding.codeEditor = this;

            //            SuspendLayout();

            //            // 设置基本属性。
            //            base.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse | ControlStyles.Opaque | ControlStyles.Selectable | ControlStyles.ResizeRedraw, true);

            //            // 初始化分割按钮。
            //            _splitButton.MouseDown += splitButton_MouseDown;
            //            Controls.Add(_splitButton);

            //            // 初始化文档。
            //            _document.update += document_update;

            //            // 初始化布局。
            //            painter.tabWidth = painter.measureString(' ') * Configs.defaultTabWidth;
            //            _wrapIndentWidth = painter.measureString(' ') * Configs.defaultWrapIndentCount;

            //#pragma warning disable 162
            //            _documentMaxWidth = Configs.defaultWordWrap ? 0 : int.MaxValue;
            //#pragma warning restore 162
            //            _maxLineNumberInWidth = (int)Math.Pow(10, _lineNumbersWidth / painter.fontWidth) - _lineNumbersStart + 1;

            //            // 重新计算文档大小。
            //            updateDocumentSize();

            //            ResumeLayout(false);

            //            initDefaultActions();

            //            //_document.text = @"	01234 56789
            //            //01234 56789" + '\u0002' + "aaa";

            //            return;

            //            this.components = new Container();
            //            this.codeCompletionAction = new ThreadProc(this.CodeCompletionAction);
            //            this.codeCompletionStart = new ThreadProc(this.CodeCompletionStart);
            //            this.codeCompletionEnd = new ThreadProc(this.CodeCompletionEnd);
            //            this.innerTextSource = new Teal.CodeEditor.TextSource.TextSource();
            //            this.Source = this.innerTextSource;
            //            this.source = null;
            //            this.painter222 = new Painter();
            //            // return new GdiPlusPainter();
            //            this.gutter = new Gutter(this);
            //            this.displayLines = new DisplayStrings(this, this.Lines);
            //            this.margin = new Margin(this);
            //            this.selection = new Selection2(this);
            //            this.scrolling = new Scrolling(this);
            //            this.whiteSpace = new WhiteSpace(this);
            //            this.lineSeparator = new LineSeparator(this);
            //            this.lineStyles = new EditLineStyles(this);
            //            this.braces = new Braces(this);
            //            this.hyperText = new EditHyperText(this);
            //            this.outlining = new Outlining(this);
            //            this.spelling = new EditSpelling(this);
            //            this.keyList = new KeyList(this);
            //            this.macroRecords = new MacroKeyList();
            //            this.syntaxPaint = new EditSyntaxPaint(this.painter222, this);
            //            this.printing = this.CreatePrinting();
            //            this.syntaxSettings = new Teal.CodeEditor.Dialogs.SyntaxSettings();
            //            this.notifyEventArgs = new NotifyEventArgs();
            //            this.autoCorrectEventArgs = new AutoCorrectEventArgs();
            //            base.QueryContinueDrag += new QueryContinueDragEventHandler(this.QueryEndDrag);
            //            //  this.Font = new Font(FontFamily.GenericMonospace, 30f);
            //            this.Font = painter.font;
            //            this.painter222.font = this.Font;
            //            this.BackColor = Consts.DefaultControlBackColor;

        }

        /// <summary>
        /// 引发 <see cref="E:System.Windows.Forms.Control.Paint"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.PaintEventArgs"/>。</param>
        protected override void OnPaint(PaintEventArgs e) {
            document.draw(e.Graphics, e.ClipRectangle.Top, e.ClipRectangle.Bottom);

            return;
            //if (!e.ClipRectangle.IsEmpty) {
            //    this.painter222.BeginPaint(e.Graphics);
            //    try {
            //        this.PaintScrollRect(this.painter222);
            //        Rectangle clientRect = this.GetClientRect(true);
            //        Rectangle rect = clientRect;
            //        rect.Intersect(e.ClipRectangle);

            //        int num = 0;
            //        if (this.scrolling.ScrollByPixels) {
            //            if (this.painter222.lineHeight != 0) {
            //                num = this.scrolling.WindowOriginY / this.painter222.lineHeight;
            //                int num2 = this.scrolling.WindowOriginY % this.painter222.lineHeight;
            //                clientRect.Y -= num2;
            //                rect.Height += num2;
            //            }
            //        } else {
            //            num = this.scrolling.WindowOriginY;
            //        }

            //        this.syntaxPaint.PaintWindow(this.painter222, num, rect, clientRect.Location, 1f, 1f, this.scrolling.ScrollByPixels, false);
            //        if (this.Gutter.DrawLineBookmarks) {
            //            this.syntaxPaint.PaintLineBookMarks(this.painter222, this.ClientRect);
            //        }
            //    } finally {
            //        this.painter222.EndPaint();
            //    }
            //    this.syntaxPaint.NeedPaint = false;
            //    base.OnPaint(e);
            //}
        }

        // #region 继承的事件

        /// <summary>
        /// 获取或设置控件的默认光标。
        /// </summary>
        /// <returns>
        /// 一个 <see cref="T:System.Windows.Forms.Cursor"/> 类型的对象，表示当前默认光标。
        /// </returns>
        protected override Cursor DefaultCursor => Cursors.IBeam;

        //        #region 布局

        //        /// <summary>
        //        /// 获取控件的默认大小。
        //        /// </summary>
        //        /// <returns>
        //        /// 控件的默认 <see cref="T:System.Drawing.Size"/>。
        //        /// </returns>
        //        protected override Size DefaultSize {
        //            get {
        //                return new Size(Configs.defaultWidth, Configs.defaultHeight);
        //            }
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.Resize"/> 事件。
        //        /// </summary>
        //        /// <param name="e">一个 <see cref="T:System.EventArgs"/>，其中包含事件数据。</param>
        //        protected override void OnResize(EventArgs e) {
        //            return;
        //            bool flag = this.gutter.InvalidateLineNumberArea(false);
        //            base.OnResize(e);
        //            if (this.displayLines != null) {
        //                if (!flag && this.WordWrap && this.GetWrapMargin() != this.WrapMargin) {
        //                    this.UpdateWordWrap();
        //                }
        //                if (this.IsTransparent) {
        //                    base.Invalidate();
        //                }
        //                this.scrolling.UpdateScroll(true);
        //            }
        //        }

        //        #endregion

        //        #region 绘制

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.VisibleChanged"/> 事件。
        //        /// </summary>
        //        /// <param name="e">一个 <see cref="T:System.EventArgs"/>，其中包含事件数据。</param>
        //        protected override void OnVisibleChanged(EventArgs e) {
        //            base.OnVisibleChanged(e);
        //        }

        //        #endregion

        //        #region 鼠标

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.MouseWheel"/> 事件。
        //        /// </summary>
        //        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.MouseEventArgs"/>。</param>
        //        protected override void OnMouseWheel(MouseEventArgs e) {
        //            base.OnMouseWheel(e);
        //            if ((Control.ModifierKeys & Keys.Control) == Keys.None) {
        //                handleMouseScroll(e.Delta);
        //            }
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.MouseLeave"/> 事件。
        //        /// </summary>
        //        /// <param name="e">一个 <see cref="T:System.EventArgs"/>，其中包含事件数据。</param>
        //        protected override void OnMouseLeave(EventArgs e) {
        //            base.OnMouseLeave(e);
        //            this.DisableCodeCompletionTimer();
        //            this.mouseRange = null;
        //            this.snippetRange = null;
        //            this.ClearActiveOutlineRange();
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.MouseDown"/> 事件。
        //        /// </summary>
        //        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.MouseEventArgs"/>。</param>
        //        protected override void OnMouseDown(MouseEventArgs e) {
        //            Utility.mark(e.Location);

        //            moveCaret(e.X, e.Y, FoldBlockOperation.select);

        //            base.OnMouseDown(e);
        //            return;
        //            if (this.GetClientRect(true).Contains(new Point(e.X, e.Y))) {
        //                this.needStartDrag = false;
        //                HitTestInfo hitTestInfo = new HitTestInfo();
        //                this.gutter.GetHitTest(e.X, e.Y, hitTestInfo);
        //                bool flag = (hitTestInfo.HitTest & HitTest.OutlineArea) != HitTest.None || (hitTestInfo.HitTest & HitTest.LineNumber) != HitTest.None;
        //                bool flag2 = flag;
        //                bool lineEnd = false;
        //                Point position = this.ScreenToText(e.X, e.Y, ref lineEnd);
        //                if (base.CanFocus) {
        //                    base.Focus();
        //                    bool flag3 = e.Button != MouseButtons.Right || ((NavigateOptions.MoveOnRightButton & this.NavigateOptions) != NavigateOptions.None && !this.selection.IsPosInSelection(position));
        //                    bool flag4 = e.Button == MouseButtons.Left && (SelectionOptions.DisableSelection & this.selection.Options) == SelectionOptions.None;
        //                    bool flag5 = e.Button == MouseButtons.Left && e.Clicks == 2 && e.X >= this.gutter.DisplayWidth;
        //                    bool flag6 = e.Button == MouseButtons.Left && (Control.ModifierKeys & Keys.Control) != Keys.None;
        //                    if ((this.gutter.Options & GutterOptions.SelectLineOnClick) != GutterOptions.None) {
        //                        flag2 = (flag2 || (hitTestInfo.HitTest & HitTest.Gutter) != HitTest.None);
        //                    }
        //                    if ((SelectionOptions.SelectLineOnDblClick & this.selection.Options) == SelectionOptions.None && (SelectionOptions.SelectLineOnTripleClick & this.selection.Options) != SelectionOptions.None) {
        //                        if (e.Button == MouseButtons.Left && flag4 && e.X >= this.gutter.DisplayWidth) {
        //                            this.StartTripleClickTimer();
        //                            this.lbuttonClicks += e.Clicks;
        //                        }
        //                    }
        //                    bool flag7 = this.selection.SelectionState == SelectionState.None;
        //                    bool flag8 = flag7 && this.selection.IsPosInSelection(position) && (SelectionOptions.DisableDragging & this.selection.Options) == SelectionOptions.None;
        //                    bool flag9 = e.Button == MouseButtons.Left && (hitTestInfo.HitTest & HitTest.OutlineImage) != HitTest.None;

        //                    if (flag9) {
        //                        int outlineIndex = hitTestInfo.OutlineIndex;
        //                        if (this.outlining.IsExpanded(outlineIndex)) {
        //                            if (this.outlining.CanCollapse(outlineIndex)) {
        //                                this.outlining.Collapse(outlineIndex);
        //                            }
        //                            return;
        //                        }
        //                        if (this.outlining.IsCollapsed(outlineIndex)) {
        //                            if (this.outlining.CanExpand(outlineIndex)) {
        //                                this.outlining.Expand(outlineIndex);
        //                            }
        //                            return;
        //                        }
        //                    }
        //                    if (this.outlining.AllowOutlining && (this.outlining.OutlineOptions & OutlineOptions.DrawButtons) != OutlineOptions.None) {
        //                        if ((hitTestInfo.HitTest & HitTest.OutlineButton) != HitTest.None && hitTestInfo.OutlineRange != null) {
        //                            if (flag5) {
        //                                this.selection.Clear();
        //                                if (this.outlining.CanExpand(hitTestInfo.OutlineRange.StartPoint.Y)) {
        //                                    hitTestInfo.OutlineRange.Visible = true;
        //                                }
        //                                return;
        //                            }
        //                            if (e.Button == MouseButtons.Left) {
        //                                this.Position = position;
        //                                this.selection.SelectionState = SelectionState.Select;
        //                                this.Selection.SetSelection(SelectionType.Stream, hitTestInfo.OutlineRange.StartPoint, hitTestInfo.OutlineRange.EndPoint);
        //                                return;
        //                            }
        //                        }
        //                    }
        //                    if (this.dragMargin && (Control.ModifierKeys & Keys.Control) != Keys.None) {
        //                        this.margin.IsDragging = true;
        //                        return;
        //                    }
        //                    if (flag4 && !flag5 && (Control.ModifierKeys & Keys.Shift) != Keys.None) {
        //                        if (flag2) {
        //                            this.scrolling.WindowOriginX = 0;
        //                            this.selection.StartSelection();
        //                            this.selection.SelectionState = SelectionState.SelectLine;
        //                        } else {
        //                            if (this.selection.IsEmpty) {
        //                                this.selection.UpdateSelRange(this.Position, this.Position);
        //                            }
        //                            this.selection.SelectionState = SelectionState.Select;
        //                        }
        //                        this.selection.OnSelect(this, null);
        //                    } else {
        //                        if (flag3) {
        //                            this.displayLines.LineEnd = lineEnd;
        //                            this.selection.BeginUpdate();
        //                            this.displayLines.DisableUpdate();
        //                            try {
        //                                if ((flag4 || (e.Button == MouseButtons.Right && !this.selection.IsPosInSelection(position))) && !flag5 && flag7 && !flag8 && !flag2) {
        //                                    this.Selection.Clear();
        //                                }
        //                                this.Position = position;
        //                            } finally {
        //                                this.displayLines.EnableUpdate();
        //                                this.selection.EndUpdate();
        //                            }
        //                        }
        //                        if (flag4) {
        //                            if (this.lbuttonClicks >= 4) {
        //                                this.selection.SelectLine();
        //                                this.StopTripleClickTimer();
        //                                this.selection.SelectionState = SelectionState.SelectWord;
        //                            } else {
        //                                if (flag5 && (this.selection.Options & SelectionOptions.DeselectOnDblClick) == SelectionOptions.None) {
        //                                    if ((SelectionOptions.SelectLineOnDblClick & this.selection.Options) != SelectionOptions.None) {
        //                                        this.selection.SelectLine();
        //                                    } else {
        //                                        this.selection.UpdateSelStart(false);
        //                                        this.selection.SelectWord();
        //                                        this.selection.StartSelection();
        //                                    }
        //                                    this.selection.SelectionState = SelectionState.SelectWord;
        //                                } else {
        //                                    if (flag7) {
        //                                        if (flag2) {
        //                                            this.selection.UpdateSelStart(false);
        //                                            this.scrolling.WindowOriginX = 0;
        //                                            this.selection.SetSelection(SelectionType.Stream, new Rectangle(0, this.Position.Y, 0, 1));
        //                                            this.selection.StartSelection();
        //                                            this.selection.SelectionState = SelectionState.SelectLine;
        //                                        } else {
        //                                            if (flag6 && (hitTestInfo.HitTest & HitTest.Gutter) == HitTest.None && (this.Selection.Options & SelectionOptions.SelectWordOnCtrlClick) != SelectionOptions.None) {
        //                                                this.selection.UpdateSelStart(false);
        //                                                this.selection.SelectWord();
        //                                                this.selection.StartSelection();
        //                                                this.selection.SelectionState = SelectionState.SelectWord;
        //                                            } else {
        //                                                if (flag8) {
        //                                                    this.selection.EndSelection();
        //                                                    this.selection.SelectionState = SelectionState.Drag;
        //                                                    this.needStartDrag = true;
        //                                                    this.startDragPos = new Point(e.X, e.Y);
        //                                                } else {
        //                                                    this.selection.Clear();
        //                                                    this.selection.SelectionState = SelectionState.Select;
        //                                                    if ((this.NavigateOptions & NavigateOptions.BeyondEof) == NavigateOptions.None || position.Y < this.Lines.Count) {
        //                                                        this.selection.StartSelection();
        //                                                    }
        //                                                }
        //                                                this.selection.UpdateSelStart(false);
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        } else {
        //                            if (e.Button == MouseButtons.Right && this.selection.SelectionState != SelectionState.None) {
        //                                this.selection.EndSelection();
        //                            }
        //                        }
        //                    }
        //                }
        //                this.urlAtCursor = this.hyperText.IsUrlAtPoint(e.X, e.Y);
        //                if (!flag && (hitTestInfo.HitTest & HitTest.Gutter) != HitTest.None) {
        //                    if (e.Clicks == 1) {
        //                        this.gutter.OnClick(e);
        //                    } else {
        //                        this.gutter.OnDoubleClick(new EventArgs());
        //                    }
        //                    if ((hitTestInfo.HitTest & HitTest.BookMark) != HitTest.None) {
        //                        IBookMark bookMark = this.Source.BookMarks.FindBookMark(hitTestInfo.GutterImage, position.Y);
        //                        if (bookMark != null) {
        //                            if (e.Clicks == 1) {
        //                                this.Source.BookMarks.OnClick(bookMark);
        //                            } else {
        //                                this.Source.BookMarks.OnDoubleClick(bookMark);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.MouseUp"/> 事件。
        //        /// </summary>
        //        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.MouseEventArgs"/>。</param>
        //        protected override void OnMouseUp(MouseEventArgs e) {
        //            base.OnMouseUp(e);
        //            return;
        //            if (e.Button == MouseButtons.Left) {
        //                switch (this.selection.SelectionState) {
        //                    case SelectionState.None:
        //                        this.selection.Clear();
        //                        break;
        //                    case SelectionState.Drag:
        //                        if (this.needStartDrag) {
        //                            this.selection.Clear();
        //                        }
        //                        this.selection.SelectionState = SelectionState.None;
        //                        break;
        //                    case SelectionState.Select:
        //                    case SelectionState.SelectLine:
        //                        this.selection.EndSelection();
        //                        if (!this.selection.IsValidSelectionPoint(this.Position)) {
        //                            this.selection.Clear();
        //                        }
        //                        break;
        //                    case SelectionState.SelectWord:
        //                        this.selection.SelectionState = SelectionState.None;
        //                        this.selection.EndSelection();
        //                        break;
        //                }
        //                this.needStartDrag = false;
        //                if (this.urlAtCursor && this.selection.SelectionState == SelectionState.None && (Control.ModifierKeys & Keys.Control) != Keys.None) {
        //                    string url;
        //                    if (this.hyperText.IsUrlAtPoint(e.X, e.Y, out url)) {
        //                        this.hyperText.UrlJump(url);
        //                        return;
        //                    }
        //                }
        //                if (this.mouseBookMark != null && this.selection.SelectionState == SelectionState.None && (Control.ModifierKeys & Keys.Control) != Keys.None) {
        //                    string url = this.mouseBookMark.Url;
        //                    if (url != null && url != string.Empty) {
        //                        this.hyperText.UrlJump(url);
        //                        return;
        //                    }
        //                }
        //                if (this.margin.IsDragging) {
        //                    this.margin.Position = this.ScreenToDisplay(e.X, e.Y).X;
        //                    this.CancelDragging();
        //                }
        //            }
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.MouseMove"/> 事件。
        //        /// </summary>
        //        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.MouseEventArgs"/>。</param>
        //        protected override void OnMouseMove(MouseEventArgs e) {
        //            base.OnMouseMove(e);
        //            return;
        //            if (this.needStartDrag && (e.Button & MouseButtons.Left) != MouseButtons.None && this.IsSignificantMouseMove(new Point(e.X, e.Y), this.startDragPos)) {
        //                this.StartDragging();
        //            } else {
        //                Teal.CodeEditor.TextSource.TextSource textSource = this.Source;
        //                bool flag = false;
        //                Point position = this.ScreenToDisplay(e.X, e.Y);
        //                Point point = this.displayLines.DisplayPointToPoint(position.X, position.Y, ref flag);
        //                this.UpdateActiveOutlineRange();
        //                CodeSnippetRange codeSnippetRange = null;
        //                if (textSource.CodeSnippets.Count > 0) {
        //                    int index;
        //                    if (textSource.CodeSnippets.FindSnippet(point, false, out index) && textSource.CodeSnippets.IsFirstSnippet(index)) {
        //                        codeSnippetRange = textSource.CodeSnippets[index];
        //                    }
        //                }
        //                if (codeSnippetRange != this.snippetRange) {
        //                    if (this.codeCompletionHint != null) {
        //                        this.codeCompletionHint.Close(false);
        //                    }
        //                    this.DisableCodeCompletionTimer();
        //                    this.snippetRange = codeSnippetRange;
        //                    if (this.snippetRange != null && this.snippetRange.Tooltip != null && this.snippetRange.Tooltip != string.Empty) {
        //                        this.DoCodeToolTip(this.snippetRange.Tooltip, e.X, e.Y, this.snippetRange.StartPoint, false);
        //                        return;
        //                    }
        //                }
        //                if (codeSnippetRange == null) {
        //                    HitTestInfo hitTestInfo = new HitTestInfo();
        //                    this.gutter.GetHitTest(e.X, e.Y, hitTestInfo);
        //                    if (this.outlining.AllowOutlining && (OutlineOptions.ShowHints & this.Outlining.OutlineOptions) != OutlineOptions.None) {
        //                        OutlineRange outlineRange = null;
        //                        if ((hitTestInfo.HitTest & HitTest.OutlineButton) != HitTest.None) {
        //                            outlineRange = hitTestInfo.OutlineRange;
        //                        }
        //                        if (this.mouseRange != outlineRange) {
        //                            if (this.codeCompletionHint != null) {
        //                                this.codeCompletionHint.Close(false);
        //                            }
        //                            this.DisableCodeCompletionTimer();
        //                            this.mouseRange = outlineRange;
        //                            if (this.mouseRange != null) {
        //                                this.DoCodeToolTip(this.displayLines.GetOutlineHint(this.mouseRange), e.X, e.Y, this.mouseRange.StartPoint, false);
        //                                return;
        //                            }
        //                        }
        //                        if (outlineRange != null) {
        //                            return;
        //                        }
        //                    }
        //                    if (this.margin.AllowDrag && this.margin.Visible) {
        //                        if (this.margin.IsDragging) {
        //                            this.margin.DragTo(e.X, e.Y);
        //                            return;
        //                        }
        //                        bool flag2 = this.margin.Contains(e.X, e.Y);
        //                        if (this.dragMargin != flag2) {
        //                            if (this.margin.ShowHints) {
        //                                if (this.codeCompletionHint != null) {
        //                                    this.codeCompletionHint.Close(false);
        //                                }
        //                                this.DisableCodeCompletionTimer();
        //                            }
        //                            this.dragMargin = flag2;
        //                            if (this.dragMargin && this.margin.ShowHints) {
        //                                this.DoCodeToolTip(StringConsts.DefaultDragMarginHint, e.X, e.Y, Point.Empty, true);
        //                            }
        //                            return;
        //                        }
        //                        if (flag2 && this.margin.ShowHints) {
        //                            return;
        //                        }
        //                    }
        //                    if (this.syntaxPaint.SyntaxErrorsHints) {
        //                        SyntaxError syntaxError;
        //                        if (!this.IsMouseOnSyntaxError(e.X, e.Y, out syntaxError)) {
        //                            syntaxError = null;
        //                        }
        //                        if (this.mouseError != syntaxError) {
        //                            if (this.codeCompletionHint != null) {
        //                                this.codeCompletionHint.Close(false);
        //                            }
        //                            this.DisableCodeCompletionTimer();
        //                            this.mouseError = syntaxError;
        //                            if (this.mouseError != null) {
        //                                this.DoCodeToolTip(syntaxError.Description, e.X, e.Y, syntaxError.Position, true);
        //                                return;
        //                            }
        //                        }
        //                        if (syntaxError != null) {
        //                            return;
        //                        }
        //                    }
        //                    if ((this.gutter.Options & GutterOptions.PaintBookMarks) != GutterOptions.None) {
        //                        IBookMark bookMark = null;
        //                        if ((hitTestInfo.HitTest & HitTest.BookMark) != HitTest.None) {
        //                            bookMark = this.Source.BookMarks.FindBookMark((hitTestInfo.GutterImage == this.gutter.BookMarkImageIndex) ? int.MaxValue : hitTestInfo.GutterImage, point.Y);
        //                        }
        //                        if (bookMark != this.mouseBookMark) {
        //                            this.mouseBookMark = bookMark;
        //                            this.mouseBookMarkPt = new Point(e.X, e.Y);
        //                            if (this.gutter.ShowBookmarkHints) {
        //                                if (this.codeCompletionHint != null) {
        //                                    this.codeCompletionHint.Close(false);
        //                                }
        //                                this.DisableCodeCompletionTimer();
        //                                if (bookMark != null) {
        //                                    string text = bookMark.Description;
        //                                    string url = bookMark.Url;
        //                                    if (url != null && url != string.Empty) {
        //                                        text = text + ((text != null && text != string.Empty) ? "\r\n" : string.Empty) + string.Format(StringConsts.DefaultHyperTextHint, url);
        //                                    }
        //                                    if (!this.Source.BookMarks.OnShowTooltip(ref text, bookMark)) {
        //                                        if (text != string.Empty) {
        //                                            this.DoCodeToolTip(text, e.X, e.Y, new Point(0, point.Y), true);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (bookMark != null && this.gutter.ShowBookmarkHints) {
        //                            return;
        //                        }
        //                    }
        //                    if (this.hyperText.HighlightHyperText && this.hyperText.ShowHints) {
        //                        string text = string.Empty;
        //                        if (!this.hyperText.IsUrlAtPoint(e.X, e.Y, out text)) {
        //                            text = string.Empty;
        //                        }
        //                        if (text != this.mouseUrl) {
        //                            if (this.codeCompletionHint != null) {
        //                                this.codeCompletionHint.Close(false);
        //                            }
        //                            this.DisableCodeCompletionTimer();
        //                            this.mouseUrl = text;
        //                            if (this.mouseUrl != string.Empty && this.mouseUrl != null) {
        //                                this.mouseUrlPoint = position;
        //                                this.DoCodeToolTip(string.Format(StringConsts.DefaultHyperTextHint, this.mouseUrl), e.X, e.Y, this.displayLines.DisplayPointToPoint(position), true);
        //                            }
        //                        }
        //                        if (this.mouseUrl != string.Empty && this.mouseUrl != null) {
        //                            return;
        //                        }
        //                    }
        //                    if (this.Source.NeedQuickInfoTips()) {
        //                        Point point2 = this.GetInfoTipPos(point);
        //                        if (!this.infoTipPos.Equals(point2)) {
        //                            if (this.codeCompletionHint != null) {
        //                                this.codeCompletionHint.Close(false);
        //                            }
        //                            this.DisableCodeCompletionTimer();
        //                            this.infoTipPos = point2;
        //                            if (this.infoTipPos.Y >= 0 && this.infoTipPos.X >= 0) {
        //                                this.QuickInfo(this.codeCompletionArgs, this.infoTipPos, false);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        #endregion

        //        #region 拖动

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.DragLeave"/> 事件。
        //        /// </summary>
        //        /// <param name="e">一个 <see cref="T:System.EventArgs"/>，其中包含事件数据。</param>
        //        protected override void OnDragLeave(EventArgs e) {
        //            base.OnDragLeave(e);
        //            this.HideDragCaret();
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.DragEnter"/> 事件。
        //        /// </summary>
        //        /// <param name="drgevent">包含事件数据的 <see cref="T:System.Windows.Forms.DragEventArgs"/>。</param>
        //        protected override void OnDragEnter(DragEventArgs drgevent) {
        //            base.OnDragEnter(drgevent);
        //            this.DisplayDragCaret();
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.DragOver"/> 事件。
        //        /// </summary>
        //        /// <param name="drgevent">包含事件数据的 <see cref="T:System.Windows.Forms.DragEventArgs"/>。</param>
        //        protected override void OnDragOver(DragEventArgs drgevent) {
        //            drgevent.Effect = DragDropEffects.None;
        //            base.OnDragOver(drgevent);
        //            this.DisplayDragCaret();
        //            if (drgevent.Effect == DragDropEffects.None) {
        //                if (!this.Source.ReadOnly && drgevent.Data != null && (drgevent.Data.GetDataPresent(DataFormats.Text) || drgevent.Data.GetDataPresent(DataFormats.UnicodeText))) {
        //                    if ((Control.ModifierKeys & Keys.Control) != Keys.None) {
        //                        drgevent.Effect = DragDropEffects.Copy;
        //                    } else {
        //                        drgevent.Effect = DragDropEffects.Move;
        //                    }
        //                    this.MoveCaretOnDrag();
        //                } else {
        //                    drgevent.Effect = DragDropEffects.None;
        //                }
        //            }
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.DragDrop"/> 事件。
        //        /// </summary>
        //        /// <param name="drgevent">包含事件数据的 <see cref="T:System.Windows.Forms.DragEventArgs"/>。</param>
        //        protected override void OnDragDrop(DragEventArgs drgevent) {
        //            base.OnDragDrop(drgevent);
        //            this.HideDragCaret();
        //            if (drgevent.Effect != DragDropEffects.None) {
        //                this.needStartDrag = false;
        //                if (this.selection.SelectionState == SelectionState.Drag) {
        //                    if (!this.selection.Move(this.Position, (Control.ModifierKeys & Keys.Control) == Keys.None)) {
        //                        this.selection.Clear();
        //                    }
        //                    this.selection.SelectionState = SelectionState.None;
        //                    drgevent.Effect = DragDropEffects.None;
        //                } else {
        //                    object data = drgevent.Data.GetData(DataFormats.UnicodeText);
        //                    if (data == null) {
        //                        data = drgevent.Data.GetData(DataFormats.Text);
        //                    }
        //                    if (data != null) {
        //                        if ((this.selection.Options & SelectionOptions.ClearOnDrag) != SelectionOptions.None) {
        //                            this.selection.Delete();
        //                        } else {
        //                            this.selection.Clear();
        //                        }
        //                        this.selection.SelectedText = (string)data;
        //                        if ((this.selection.Options & SelectionOptions.DeselectOnDrag) != SelectionOptions.None) {
        //                            this.selection.Clear();
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        #endregion

        //        #region 焦点

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.GotFocus"/> 事件。
        //        /// </summary>
        //        /// <param name="e">一个 <see cref="T:System.EventArgs"/>，其中包含事件数据。</param>
        //        protected override void OnGotFocus(EventArgs e) {
        //            showCaret();
        //            //return;
        //            //if (this.Source != null) {
        //            //    this.Source.ActiveEdit = this;

        //            //        this.CreateCaret();
        //            //        this.UpdateCaret();
        //            //}
        //            //if (this.selection != null) {
        //            //    this.selection.UpdateSelection();
        //            //}
        //            base.OnGotFocus(e);
        //        }

        //        /// <summary>
        //        /// 引发 <see cref="E:System.Windows.Forms.Control.LostFocus"/> 事件。
        //        /// </summary>
        //        /// <param name="e">一个 <see cref="T:System.EventArgs"/>，其中包含事件数据。</param>
        //        protected override void OnLostFocus(EventArgs e) {
        //            if (!base.Disposing && !this.IsCodeCompletionWindowFocused) {
        //                blur();
        //                this.KillFocus();
        //                base.OnLostFocus(e);
        //            }
        //        }

        //        /// <summary>
        //        /// 强制当前控件失去焦点。
        //        /// </summary>
        //        public void blur() {
        //            hideCaret();
        //        }

        //        public virtual void KillFocus() {
        //            return;
        //            this.destroyCaret();
        //            this.UpdateSeparator();
        //            this.selection.UpdateSelection();
        //            this.CancelDragging();
        //            this.ClearCurrentBlock();
        //        }

        //        #endregion

        //        #endregion

    }

}

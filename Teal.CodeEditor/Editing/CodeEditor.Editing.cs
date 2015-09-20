using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        /// <summary>
        /// 对当前编辑器执行撤销操作。
        /// </summary>
        public void undo() {
            undoStack.undo(this);
        }

        /// <summary>
        /// 对当前编辑器执行恢复操作。
        /// </summary>
        public void redo() {
            undoStack.redo(this);
        }

        /// <summary>
        /// 判断当前是否可进行撤销操作。
        /// </summary>
        public bool canUndo => undoStack.canUndo;

        /// <summary>
        /// 判断当前是否可进行恢复操作。
        /// </summary>
        public bool canRedo => undoStack.canRedo;

        /// <summary>
        /// 添加一个撤销记录。
        /// </summary>
        /// <param name="op">要撤销的操作。</param>
        public void addUndo(UndoableOperation op) {
            //op.oldLine = _caretLine;
            //op.oldColumn = _caretColumn;
            undoStack.add(op);
        }

        /// <summary>
        /// 删除所有撤销记录。
        /// </summary>
        public void clearUndo() {
            undoStack.clear();
        }

        /// <summary>
        /// 删除指定条数以外的所有撤销记录。
        /// </summary>
        /// <param name="undoLimit">限制的最大撤销记录数。</param>
        public void clearUndo(int undoLimit) {
            undoStack.clear(undoLimit);
        }

        #region 内置事件

        /// <summary>
        /// 初始化默认事件列表。
        /// </summary>
        private void initDefaultActions() {

            // 控制。
            actions[Keys.Back] = backspace;
            actions[Keys.Control | Keys.Back] = backspaceWord;

            actions[Keys.Delete] = delete;
            actions[Keys.Control | Keys.Delete] = deleteWord;

            actions[Keys.Return] = breakLine;
            actions[Keys.Tab] = indent;
            actions[Keys.Shift | Keys.Tab] = unindent;

            // 剪贴板。
            actions[Keys.Control | Keys.C] = copy;
            actions[Keys.Control | Keys.Insert] = copy;
            actions[Keys.Control | Keys.V] = paste;
            actions[Keys.Shift | Keys.Insert] = paste;
            actions[Keys.Control | Keys.X] = cut;
            actions[Keys.Shift | Keys.Delete] = cut;

            // 撤销。
            actions[Keys.Control | Keys.Z] = undo;
            actions[Keys.Control | Keys.Y] = redo;
            actions[Keys.Alt | Keys.Back] = undo;

            // 其它。
            actions[Keys.Control | Keys.A] = selectAll;
            actions[Keys.Control | Keys.R] = dumpLine;
            actions[Keys.Escape] = clearSelections;

            // 方向键。
            actions[Keys.Left] = moveLeft;
            actions[Keys.Left | Keys.Shift] = selectLeft;
            actions[Keys.Left | Keys.Control] = moveWordLeft;
            actions[Keys.Left | Keys.Control | Keys.Shift] = selectWordLeft;

            actions[Keys.Right] = moveRight;
            actions[Keys.Right | Keys.Shift] = selectRight;
            actions[Keys.Right | Keys.Control] = moveWordRight;
            actions[Keys.Right | Keys.Control | Keys.Shift] = selectWordRight;

            actions[Keys.Up] = moveUp;
            actions[Keys.Up | Keys.Shift] = selectUp;
            actions[Keys.Up | Keys.Control] = scrollUp;

            actions[Keys.Down] = moveDown;
            actions[Keys.Down | Keys.Shift] = selectDown;
            actions[Keys.Down | Keys.Control] = scrollDown;

            actions[Keys.Insert] = toggleCaretMode;

            actions[Keys.Home] = moveToHome;
            actions[Keys.Home | Keys.Shift] = selectToHome;
            actions[Keys.End] = moveToEnd;
            actions[Keys.End | Keys.Shift] = selectToEnd;

            actions[Keys.PageUp] = movePageUp;
            actions[Keys.PageUp | Keys.Shift] = selectPageUp;
            actions[Keys.PageDown] = movePageDown;
            actions[Keys.PageDown | Keys.Shift] = selectPageDown;

            actions[Keys.Control | Keys.Divide] = toggleComment;

        }

        private void toggleComment() {

        }

        private void clearSelections() {

        }

        private void selectAll() {

        }

        private void dumpLine() {

        }

        private void scrollDown() {

        }

        private void scrollUp() {

        }

        /// <summary>
        /// 执行复制操作。
        /// </summary>
        public void copy() {
            Utility.mark();
        }

        /// <summary>
        /// 执行剪切操作。
        /// </summary>
        public void cut() {
            Utility.mark();
        }

        /// <summary>
        /// 执行粘贴操作。
        /// </summary>
        public void paste() {
            Utility.mark();
        }

        public void indent() {

        }

        public void unindent() {

        }

        /// <summary>
        /// 模拟用户输入一个字符。
        /// </summary>
        /// <param name="ch">要插入的字符。</param>
        public void input(char ch) {
            Utility.mark(ch);

            // 没有选区直接输入。
            if (_selections != null) {
                replaceSelections(ch.ToString(), false);
                return;
            }

            // 插入模式：插入字符。
            if (caretMode == CaretMode.insertMode || caretColumn >= document.lines[caretLine].length) {
                addUndo(new InsertCharOperation(caretLine, caretColumn, ch));
                document.insert(caretLine, caretColumn, ch);
                moveRight();
            } else {
                replace(caretLine, caretColumn, caretLine, caretColumn, caretLine, caretColumn + 1, ch.ToString());
            }

        }

        /// <summary>
        /// 执行一个 DELETE 操作。
        /// </summary>
        public void delete() {

            //if (_selections == null) {
            //    deleteChar(_caretVisualLine, _caretColumn - 1);
            //    return;
            //}

            backspace();
        }

        private void deleteWord() {

        }

        /// <summary>
        /// 执行一个退格键。
        /// </summary>
        public void backspace() {

            // 1. 删除选区。
            if (_selections != null) {
                replaceSelections(String.Empty, false);
                return;
            }

            var oldCaretLine = _caretLine;
            var oldCaretColumn = _caretColumn;

            switch (moveLeftCore()) {

                // 2. 现在在某字符前，删除字符。
                case MoveOperation.column:
                    addUndo(new BackCharOperation(oldCaretLine, oldCaretColumn, document.lines[_caretLine].chars[_caretColumn]));
                    document.delete(_caretLine, _caretColumn);
                    return;

                // 3. 现在行首，删除整行。
                case MoveOperation.line:
                    var nl = document.unbreakLine(oldCaretLine).newLine;
                    //addUndo(new UnbreakLineUndoableOperation(oldCaretLine, oldCaretColumn, document.lines[_caretLine].chars[_caretColumn]));
                    return;

                // 4. 现在在某折叠域前，删除折叠域。
                case MoveOperation.block:
                    return;

                // 5. 现在在最开头，不操作。
                default:
                    return;
            }

            //// 其他情况。
            ////addUndo(new DeleteBlockUndoableOperation() {
            ////    line = startLine,
            ////    column = startColumn,
            ////    value = _document.getText(startLine, startColumn, endLine, endColumn),
            ////    endLocation = new Point(endLine, endColumn)
            ////});
            //_document.deleteBlock(startLine, startColumn, endLine, endColumn);

            //delete(_caretVisualLine, _caretColumn, oldCaretLine, oldCaretColumn, oldCaretLine, oldCaretColumn);

            // 有选区先撤销选区。
        }

        private void backspaceWord() {

            if (_selections != null) {
                replaceSelections(String.Empty, false);
                return;
            }

            var oldCaretLine = caretLine;
            var oldCaretColumn = caretColumn;
            moveWordLeft();
            delete(oldCaretLine, oldCaretColumn, caretLine, caretColumn, oldCaretLine, oldCaretColumn);

        }

        #endregion

        #region 原生事件

        #region 键盘

        /// <summary>
        /// 获取所有键盘事件绑定。
        /// </summary>
        public readonly Dictionary<Keys, Action> actions = new Dictionary<Keys, Action>(30);

        /// <summary>
        /// 处理命令键。
        /// </summary>
        /// <returns>
        /// 如果字符已由控件处理，则为 true；否则为 false。
        /// </returns>
        /// <param name="msg">通过引用传递的 <see cref="T:System.Windows.Forms.Message"/>，它表示要处理的窗口消息。</param><param name="keyData"><see cref="T:System.Windows.Forms.Keys"/> 值之一，它表示要处理的键。</param>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {

            Action action;
            if (actions.TryGetValue(keyData, out action)) {
                action();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 确定指定的键是常规输入键还是需要预处理的特殊键。
        /// </summary>
        /// <returns>
        /// 如果指定的键是常规输入键，则为 true；否则为 false。
        /// </returns>
        /// <param name="keyData"><see cref="T:System.Windows.Forms.Keys"/> 值之一。</param>
        protected override bool IsInputKey(Keys keyData) {
            return true;
        }

        /// <summary>
        /// 处理对话框键。
        /// </summary>
        /// <returns>
        /// 如果键已由控件处理，则为 true；否则为 false。
        /// </returns>
        /// <param name="keyData"><see cref="T:System.Windows.Forms.Keys"/> 值之一，它表示要处理的键。</param>
        protected override bool ProcessDialogKey(Keys keyData) {
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 确定一个字符是否是控件可识别的输入字符。
        /// </summary>
        /// <returns>
        /// 如果字符应直接发送到控件且不必经过预处理，则为 true；否则为 false。
        /// </returns>
        /// <param name="charCode">要测试的字符。</param>
        protected override bool IsInputChar(char charCode) {
            return true;
        }

        /// <summary>
        /// 引发 <see cref="E:System.Windows.Forms.Control.KeyDown"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.KeyEventArgs"/>。</param>
        protected override void OnKeyDown(KeyEventArgs e) {
            Utility.mark(e.KeyCode);
            if (e.KeyCode == Keys.Escape) {
                this.CancelDragging();
            }
            this.keyProcessed = false;
            base.OnKeyDown(e);
            if (!e.Handled) {
                Keys keyData = e.KeyData;
                if (keyData != Keys.Tab) {
                    if (keyData == Keys.Return) {
                        this.lastKey = '\r';
                    }
                } else {
                    this.lastKey = '\t';
                }
                if (this.ProcessKey(e.KeyData)) {
                    this.keyProcessed = true;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 引发 <see cref="E:System.Windows.Forms.Control.KeyUp"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.KeyEventArgs"/>。</param>
        protected override void OnKeyUp(KeyEventArgs e) {
            this.CheckAutoCorrect();
            base.OnKeyUp(e);
            this.keyProcessed = false;
        }

        /// <summary>
        /// 引发 <see cref="E:System.Windows.Forms.Control.KeyPress"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.KeyPressEventArgs"/>。</param>
        protected override void OnKeyPress(KeyPressEventArgs e) {
            Utility.mark(e.KeyChar);

            if (this.keyProcessed) {
                this.keyProcessed = false;
                e.Handled = true;
            } else {
                base.OnKeyPress(e);
                if (!e.Handled) {
                    // if (!this.NeedIncrementalSearch(e.KeyChar)) {
                    if (this.keyState == 0 && e.KeyChar >= ' ') {
                        this.lastKey = e.KeyChar;
                        this.ProcessKeyPress(e.KeyChar);
                        //this.InternalProcessKey(e.KeyChar);
                        e.Handled = true;
                    }
                    //  }
                }
            }
        }

        /// <summary>
        /// 处理 Windows 消息。
        /// </summary>
        /// <param name="m">要处理的 Windows<see cref="T:System.Windows.Forms.Message"/>。</param>
        protected override void WndProc(ref Message m) {
            int num = m.Msg;
            if (num <= 198) {
                if (num <= 123) {
                    if (num != 32) {
                        if (num == 123) {
                            if (this.useDefaultMenu && this.ContextMenu == null) {
                                short num2 = (short)(m.LParam.ToInt32() & 65535);
                                short num3 = (short)(((long)m.LParam.ToInt32() & unchecked((long)((ulong)-65536))) >> 16);
                                Point point;
                                if (num2 == -1 && num3 == -1) {
                                    point = new Point(base.Bounds.X + 20, base.Bounds.Y + 20);
                                } else {
                                    point = base.PointToClient(new Point((int)num2, (int)num3));
                                }
                                this.PopupDefaultMenu(new Point(point.X, point.Y));
                                m.Result = (IntPtr)1;
                                return;
                            }
                        }
                    } else {
                        if (this.CheckCursor(base.PointToClient(Control.MousePosition))) {
                            m.Result = (IntPtr)1;
                            return;
                        }
                    }
                } else {
                    if (num == 133) {
                        base.WndProc(ref m);
                        if (this.BorderStyle == EditBorderStyle.System) {
                            if (XPThemes.CurrentTheme != XPThemeName.None && this.BorderColor != Color.Empty) {
                                IntPtr windowDC = OSUtils.GetWindowDC(base.Handle);
                                try {
                                    if (this.BorderColor != Color.Empty) {
                                        Graphics graphics = Graphics.FromHdc(windowDC);
                                        Pen pen = new Pen(this.BorderColor);
                                        try {
                                            graphics.DrawRectangle(pen, 0, 0, base.Width - 1, base.Height - 1);
                                        } finally {
                                            pen.Dispose();
                                            graphics.Dispose();
                                        }
                                    } else {
                                        OSUtils.ExcludeClipRect(windowDC, 2, 2, base.Width - 2, base.Height - 2);
                                        XPThemes.DrawEditBorder(windowDC, new Rectangle(0, 0, base.Width, base.Height));
                                    }
                                } finally {
                                    OSUtils.ReleaseDC(base.Handle, windowDC);
                                }
                            }
                        }
                        return;
                    }
                    if (num == 198) {
                        if (this.Source.CanUndo()) {
                            m.Result = (IntPtr)1;
                        }
                    }
                }
            } else {
                if (num <= 514) {
                    switch (num) {
                        case 276:
                            //if ((this.scrolling.Options & ScrollingOptions.SystemScrollbars) != ScrollingOptions.None) {
                            //    m.Result = (IntPtr)1;
                            //    this.scrolling.SystemScroll((int)OSUtils.LoWord(m.WParam), false);
                            //    return;
                            //}
                            break;
                        case 277:
                            //if ((this.scrolling.Options & ScrollingOptions.SystemScrollbars) != ScrollingOptions.None) {
                            //    m.Result = (IntPtr)1;
                            //    this.scrolling.SystemScroll((int)OSUtils.LoWord(m.WParam), true);
                            //    return;
                            //}
                            break;
                        default:
                            switch (num) {
                                case 513:
                                    base.Capture = true;
                                    break;
                                case 514:
                                    base.Capture = false;
                                    break;
                            }
                            break;
                    }
                } else {
                    switch (num) {
                        case 642:
                            num = m.WParam.ToInt32();
                            if (num == 8 || num == 11) {
                                if (this.NeedImeComposition()) {
                                    OSUtils.ImmSetCompositionFont(base.Handle, this.Font);
                                }
                            }
                            break;
                        case 643:
                            num = m.WParam.ToInt32();
                            if (num == 11) {
                                if (this.NeedImeComposition()) {
                                    OSUtils.UpdateCompositionWindow(base.Handle, this.TextToScreen(this.Position), m.LParam);
                                }
                            }
                            break;
                        default:
                            switch (num) {
                                case 768:
                                    if (this.selection.CanCut()) {
                                        this.selection.Cut();
                                    }
                                    break;
                                case 769:
                                    if (this.selection.CanCopy()) {
                                        this.selection.Copy();
                                    }
                                    break;
                                case 770:
                                    if (this.selection.CanPaste()) {
                                        this.selection.Paste();
                                    }
                                    break;
                                case 771:
                                    if (!this.selection.IsEmpty) {
                                        this.selection.Delete();
                                    }
                                    break;
                                case 772:
                                    if (this.Source.CanUndo()) {
                                        this.Source.Undo();
                                        m.Result = (IntPtr)1;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            base.WndProc(ref m);
            if (m.Msg == 269) {
                //if (this.NeedImeComposition()) {
                //    OSUtils.ImmSetCompositionWindow(base.Handle, this.TextToScreen(this.Position));
                //}
            }
        }

        #endregion


        #endregion

    }



}

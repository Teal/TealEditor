﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档视图。
    /// </summary>
    public sealed partial class DocumentView {

        /// <summary>
        /// 存储当前光标的所在行号。
        /// </summary>
        private int _caretLine;

        /// <summary>
        /// 存储当前光标的所在列号。
        /// </summary>
        private int _caretColumn;

        /// <summary>
        /// 将当前光标左移一位。
        /// </summary>
        public MoveOperation moveLeft() {

            //clearSelections();

            //MoveOperation op;

            //// 如果当前光标等于起始列，则切换到上一行。
            //if (_caretColumn == 0) {
            //    if (_caretLine == 0) {
            //        return MoveOperation.none;
            //    }

            //    _caretColumn = document.lines[--_caretLine].length;
            //    op = MoveOperation.line;
            //} else {
            //    var foldingRange = getFoldingRangeByEnd(_caretLine, _caretColumn);
            //    if (foldingRange != null && foldingRange.isFolded) {
            //        _caretLine = foldingRange.startLine;
            //        _caretColumn = foldingRange.startColumn;
            //        op = MoveOperation.block;
            //    } else {
            //        _caretColumn--;
            //        op = MoveOperation.column;
            //    }
            //}

            //updateCaret();

            // return op;

            return MoveOperation.none;
        }

        /// <summary>
        /// 执行删除操作。
        /// </summary>
        public void delete() {
            beginUpdate();

            if(hasSelections) {
                
            } else {
                
            }

            endUpdate();
        }

        /// <summary>
        /// 执行删除操作。
        /// </summary>
        public void input() {
            beginUpdate();

            if (hasSelections) {

            } else {

            }

            endUpdate();
        }

        ///// <summary>
        ///// 表示一个代码编辑器。
        ///// </summary>
        //public partial class CodeEditor {

        //    #region 光标坐标

        //    /// <summary>
        //    /// 当前光标的所在显示行号。
        //    /// </summary>
        //    private int _caretVisualLine;

        //    /// <summary>
        //    /// 当前光标换行之前的水平坐标。用于跨行移动时记住当前列。
        //    /// </summary>
        //    private int _caretDesiredLeft;

        //    /// <summary>
        //    /// 获取或设置当前光标的所在实际行号。
        //    /// </summary>updateCaret
        //    public int caretLine {
        //        get {
        //            return _caretLine;
        //        }
        //        set {
        //            if (_caretLine == value) {
        //                return;
        //            }
        //            _caretLine = value;
        //            updateCaret();
        //        }
        //    }

        //    /// <summary>
        //    /// 获取或设置当前光标的所在实际列号。
        //    /// </summary>
        //    public int caretColumn {
        //        get {
        //            return _caretColumn;
        //        }
        //        set {
        //            if (_caretColumn == value) {
        //                return;
        //            }
        //            _caretColumn = value;
        //            updateCaret();
        //        }
        //    }

        //    /// <summary>
        //    /// 获取或设置当前光标的所在实际行号和列号。
        //    /// </summary>
        //    public Point caretLocation {
        //        get {
        //            return new Point(caretLine, caretColumn);
        //        }
        //        set {
        //            setCaretLocation(value.Y, value.X);
        //        }
        //    }

        //    /// <summary>
        //    /// 获取或设置当前光标的所在坐标。
        //    /// </summary>
        //    public Point caretPosition {
        //        get {
        //            return getCaretPosition();
        //        }
        //        set {
        //            moveCaret(value.X, value.Y);
        //        }
        //    }

        //    /// <summary>
        //    /// 设置光标所在行列号。
        //    /// </summary>
        //    /// <param name="line"></param>
        //    /// <param name="column"></param>
        //    public void setCaretLocation(int line, int column) {
        //        Utility.mark(line, column);
        //        if (_caretLine != line || _caretColumn != column) {
        //            _caretLine = line;
        //            _caretColumn = column;
        //            _caretVisualLine = locationToVisualLine(line, column);
        //            updateCaret();
        //        }
        //    }

        //    /// <summary>
        //    /// 设置光标的核心函数。
        //    /// </summary>
        //    /// <param name="x"></param>
        //    /// <param name="y"></param>
        //    protected virtual void setCaretPositionCore(int x, int y) {
        //        _caretDesiredLeft = x;
        //        x -= Configs.caretWidth >> 1;
        //        setCaretPosition(x, y);
        //        if (caretPositionChanged != null) {
        //            caretPositionChanged(x, y);
        //        }
        //    }

        //    /// <summary>
        //    /// 光标位置改变后事件。
        //    /// </summary>
        //    public event Action<int, int> caretPositionChanged;

        //    #endregion

        //    #region 移动

        //    #region 移动一格

        ///// <summary>
        ///// 将当前光标左移一位。
        ///// </summary>
        //public void moveLeft() {
        //    moveLeftCore();
        //}

        ///// <summary>
        ///// 将当前光标右移一位。
        ///// </summary>
        //public void moveRight() {
        //    moveRightCore();
        //}

        //    /// <summary>
        //    /// 将当前光标左移一位并选中。
        //    /// </summary>
        //    public void selectLeft() {
        //        var oldCaretLine = _caretLine;
        //        var oldCaretColumn = _caretColumn;
        //        moveLeft();
        //        select(_caretLine, _caretColumn, oldCaretLine, oldCaretColumn);
        //    }

        //    /// <summary>
        //    /// 将当前光标右移一位并选中。
        //    /// </summary>
        //    public void selectRight() {
        //        var oldCaretLine = _caretLine;
        //        var oldCaretColumn = _caretColumn;
        //        moveRight();
        //        select(oldCaretLine, oldCaretColumn, _caretLine, _caretColumn);
        //    }

        //    /// <summary>
        //    /// 将当前光标右移一位。
        //    /// </summary>
        //    private MoveOperation moveRightCore() {

        //        clearSelections();

        //        MoveOperation op;

        //        // 如果当前光标等于起始列，则切换到上一行。
        //        if (_caretColumn == document.lines[_caretLine].length) {
        //            if (_caretLine == document.lines.Count - 1) {
        //                return MoveOperation.none;
        //            }

        //            _caretLine++;
        //            _caretColumn = 0;
        //            op = MoveOperation.line;
        //        } else {
        //            var foldingRange = getFoldBlockByStart(_caretLine, _caretColumn);
        //            if (foldingRange != null && foldingRange.isFolded) {
        //                _caretLine = foldingRange.startLine;
        //                _caretColumn = foldingRange.startColumn;
        //                op = MoveOperation.block;
        //            } else {
        //                _caretColumn--;
        //                op = MoveOperation.column;
        //            }
        //        }

        //        updateCaret();

        //        return op;

        //    }

        //    #endregion

        //    #region 移动单词

        //    /// <summary>
        //    /// 将当前光标左移一位单词。
        //    /// </summary>
        //    public void moveWordLeft() {
        //        var wordStart = synataxBinding.findPrevWordStart(caretLine, caretColumn);
        //        while (moveLeftCore() == MoveOperation.column && caretColumn > wordStart) ;
        //    }

        //    /// <summary>
        //    /// 将当前光标右移一位单词。
        //    /// </summary>
        //    public void moveWordRight() {
        //        var wordEnd = synataxBinding.findNextWordEnd(caretLine, caretColumn);
        //        while (moveRightCore() == MoveOperation.column && caretColumn < wordEnd) ;
        //    }

        //    /// <summary>
        //    /// 将当前光标左移一位单词并选中。
        //    /// </summary>
        //    public void selectWordLeft() {
        //        int oldCaretLine = caretLine;
        //        int oldCaretColumn = caretColumn;
        //        moveWordLeft();
        //        select(oldCaretLine, oldCaretColumn, caretLine, caretColumn);
        //    }

        //    /// <summary>
        //    /// 将当前光标右移一位单词并选中。
        //    /// </summary>
        //    public void selectWordRight() {
        //        int oldCaretLine = caretLine;
        //        int oldCaretColumn = caretColumn;
        //        moveWordRight();
        //        select(oldCaretLine, oldCaretColumn, caretLine, caretColumn);
        //    }

        //    #endregion

        //    #region 移动多行

        //    /// <summary>
        //    /// 将当前光标上移一位。
        //    /// </summary>
        //    public void moveUp() {
        //        moveVertical(1);
        //    }

        //    /// <summary>
        //    /// 将当前光标下移一位。
        //    /// </summary>
        //    public void moveDown() {
        //        moveVertical(-1);
        //    }

        //    public void moveToHome() {
        //        //moveTo(0, _caretVirtualColumn, false);
        //    }

        //    public void moveToEnd() {
        //    }

        //    public void movePageDown() {
        //        moveVertical(visibleLineCount);
        //    }

        //    public void movePageUp() {
        //        moveVertical(-visibleLineCount);
        //    }

        //    private void moveVertical(int delta) {
        //        var left = _caretDesiredLeft;
        //        Point p = caretPosition;
        //        p.Y += painter.lineHeight * delta;
        //        caretPosition = p;
        //        _caretDesiredLeft = left;
        //    }

        //    /// <summary>
        //    /// 将当前光标下移一位并选中。
        //    /// </summary>
        //    public void selectDown() {
        //        int caretLine = _caretLine;
        //        int caretColumn = _caretColumn;
        //        moveDown();
        //        select(caretLine, caretColumn, _caretLine, _caretColumn);
        //    }

        //    /// <summary>
        //    /// 将当前光标上移一位并选中。
        //    /// </summary>
        //    public void selectUp() {
        //        int caretLine = _caretLine;
        //        int caretColumn = _caretColumn;
        //        moveUp();
        //        select(_caretLine, _caretColumn, caretLine, caretColumn);
        //    }

        //    public void selectToHome() {
        //        int caretLine = _caretLine;
        //        int caretColumn = _caretColumn;
        //        moveToHome();
        //        select(caretLine, caretColumn, _caretLine, _caretColumn);
        //    }

        //    public void selectToEnd() {
        //        int caretLine = _caretLine;
        //        int caretColumn = _caretColumn;
        //        moveToEnd();
        //        select(caretLine, caretColumn, _caretLine, _caretColumn);
        //    }

        //    public void selectPageDown() {
        //        int caretLine = _caretLine;
        //        int caretColumn = _caretColumn;
        //        movePageDown();
        //        select(caretLine, caretColumn, _caretLine, _caretColumn);
        //    }

        //    public void selectPageUp() {
        //        int caretLine = _caretLine;
        //        int caretColumn = _caretColumn;
        //        movePageUp();
        //        select(caretLine, caretColumn, _caretLine, _caretColumn);
        //    }

        //    #endregion

        //    /// <summary>
        //    /// 移动光标的位置，并自动调整为字符位置。
        //    /// </summary>
        //    /// <param name="x"></param>
        //    /// <param name="y"></param>
        //    /// <param name="foldBlockOperation"></param>
        //    public void moveCaret(int x, int y, FoldBlockOperation foldBlockOperation = FoldBlockOperation.none) {

        //        FoldingRange foldingRange;
        //        positionToLocation(x, y, out _caretVisualLine, out _caretLine, out _caretColumn, out foldingRange);
        //        if (foldingRange != null) {
        //            switch (foldBlockOperation) {
        //                case FoldBlockOperation.none:
        //                    break;
        //                case FoldBlockOperation.select:
        //                    break;
        //                case FoldBlockOperation.expand:
        //                    break;
        //            }
        //        }

        //        updateCaret();

        //    }

        //    #endregion

        //    #region 拖动

        //    private void drawDragCaret(int x, int y) {

        //    }
        //    private void clearDragCaret(int x, int y) {

        //    }

        //    protected void PaintDragCaret(Point pt, bool erase) {
        //        if (!pt.Equals(this.hideCaretPoint)) {
        //            Rectangle rectangle = new Rectangle(pt.X - 1, pt.Y, 2, this.painter222.FontHeight - 1);
        //            if (erase) {
        //                rectangle.Width++;
        //                rectangle.Height++;
        //                base.Invalidate(rectangle);
        //            } else {
        //                Graphics graphics = base.CreateGraphics();
        //                try {
        //                    Pen pen = new Pen(this.Selection.IsPosInSelection(this.Position) ? EditConsts.DefaultSelectionDragColor : Consts.DefaultControlForeColor, 1f);
        //                    graphics.DrawRectangle(pen, rectangle);
        //                    pen.Dispose();
        //                } finally {
        //                    graphics.Dispose();
        //                }
        //            }
        //        }
        //    }



        //    #endregion

        //    #region 接口

        //    /// <summary>
        //    /// 为该控件创建句柄。
        //    /// </summary>
        //    /// <exception cref="T:System.ObjectDisposedException">对象处于被释放状态。</exception>
        //    protected override void CreateHandle() {
        //        base.CreateHandle();
        //        initCarat();
        //    }

        //    /// <summary>
        //    /// 初始化光标。
        //    /// </summary>
        //    private void initCarat() {
        //        createCaret(Configs.caretWidth);
        //    }

        //    /// <summary>
        //    /// 取消初始化光标。
        //    /// </summary>
        //    private void uninitCarat() {
        //        destroyCaret();
        //    }

        //    #endregion

        //    #region weidengki

        //    private bool dragCaret;

        //    private Point hideCaretPoint = new Point(-100, -100);

        //    public virtual void MoveCaretOnDrag() {
        //        Point point = base.PointToClient(Cursor.Position);
        //        if (!this.selection.ScrollIfNeeded(point)) {
        //            this.selection.BeginUpdate();
        //            this.displayLines.DisableUpdate();
        //            try {
        //                bool lineEnd = false;
        //                point = this.ScreenToText(point.X, point.Y, ref lineEnd);
        //                this.displayLines.LineEnd = lineEnd;
        //                this.Position = point;
        //            } finally {
        //                this.displayLines.EnableUpdate();
        //                this.selection.EndUpdate();
        //            }
        //        }
        //    }

        //    protected void UpdateCaretMode() {
        //        if (this.IsFocused) {
        //            this.destroyCaret();
        //            this.CreateCaret();
        //            this.UpdateCaret();
        //        }
        //    }

        //    #region 方法

        //    #endregion

        //    public virtual void CreateCaret() {
        //        if (base.IsHandleCreated) {
        //            Size caretSize = this.GetCaretSize(this.Position);
        //            Win32Api.CreateCaret(base.Handle, caretSize.Width, caretSize.Height);
        //            Win32Api.ShowCaret(base.Handle);
        //        }
        //    }

        //    public virtual void ShowCaret(int x, int y) {
        //        if (this.dragCaret) {
        //            this.PaintDragCaret(this.oldDragPoint, true);
        //            this.oldDragPoint = new Point(x, y);
        //            this.PaintDragCaret(this.oldDragPoint, false);
        //        } else {
        //            Win32Api.SetCaretPos(x, y);
        //            if (this.NeedImeComposition()) {
        //                OSUtils.ImmSetCompositionWindow(base.Handle, this.TextToScreen(this.Position));
        //            }
        //        }
        //    }

        //    private int getTopFromLine(int line) {
        //        return painter.lineHeight * line - _scrollTop;
        //    }

        //    public virtual void UpdateCaret() {
        //        if (this.IsFocused || this.dragCaret) {
        //            Point point = this.TextToScreen(this.Position);
        //            if (point.X < this.gutter.DisplayWidth || point.Y < 0) {
        //                this.DisplayCaretNowhere();
        //            } else {
        //                this.ShowCaret(point.X, point.Y);
        //            }

        //        }
        //        this.UpdateCurrentBlock();
        //        this.UpdateSeparator();
        //    }

        //    public virtual Size GetCaretSize(Point position) {
        //        Size result;
        //        if (this.Source.Overwrite) {
        //            result = new Size(this.painter222.FontWidth, this.painter222.FontHeight);
        //        } else {
        //            result = new Size(Configs.caretWidth, this.painter222.FontHeight);
        //        }
        //        return result;
        //    }

        //    public virtual void DisplayDragCaret() {
        //        if (!this.dragCaret) {
        //            if (base.IsHandleCreated && this.IsFocused) {
        //                this.DisplayCaretNowhere();
        //            }
        //            this.dragCaret = true;
        //            this.UpdateCaret();
        //        }
        //    }

        //    public virtual void HideDragCaret() {
        //        if (this.dragCaret) {
        //            this.PaintDragCaret(this.oldDragPoint, true);
        //            this.oldDragPoint = this.hideCaretPoint;
        //            this.dragCaret = false;
        //            this.UpdateCaret();
        //        }
        //    }

        //    protected void DisplayCaretNowhere() {
        //        Win32Api.SetCaretPos(this.hideCaretPoint.X, this.hideCaretPoint.Y);
        //    }

        //    #endregion

        //}

    }
}
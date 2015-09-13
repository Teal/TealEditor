using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        #region 文档区域

        /// <summary>
        /// 当前编辑器代码区域总体偏移数。
        /// </summary>
        private int _documentLeft = Configs.defaultGutterWidth + Configs.documentPaddingLeft;

        /// <summary>
        /// 当前编辑器代码区域（不含滚动条和边界）的实际宽度。
        /// </summary>
        private int _documentWidth;

        /// <summary>
        /// 当前编辑器代码区域（不含滚动条）的实际高度。
        /// </summary>
        private int _documentHeight;

        /// <summary>
        /// 获取当前编辑器代码区域和控件左边界的距离。
        /// </summary>
        public int documentLeft {
            get {
                return _documentLeft;
            }
        }

        /// <summary>
        /// 获取或设置当前编辑器代码区域（不含滚动条和边界）的实际宽度。
        /// </summary>
        public int documentWidth {
            get {
                return _documentWidth;
            }
        }

        /// <summary>
        /// 获取或设置当前编辑器代码区域（不含滚动条）的实际高度。
        /// </summary>
        public int documentHeight {
            get {
                return _documentHeight;
            }
        }

        /// <summary>
        /// 获取当前用户实际可显示的行数。
        /// </summary>
        public int visibleLineCount {
            get {
                return documentHeight / painter.lineHeight;
            }
        }

        /// <summary>
        /// 引发 <see cref="E:System.Windows.Forms.Control.SizeChanged"/> 事件。
        /// </summary>
        /// <param name="e">一个 <see cref="T:System.EventArgs"/>，其中包含事件数据。</param>
        protected override void OnSizeChanged(EventArgs e) {
            updateDocumentSize();
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// 当编辑器大小发生改变后重新计算相关属性。
        /// </summary>
        private void updateDocumentSize() {

            // 重新计算滚动条。
            updateScrollBarSizes();

            var baseSize = base.ClientSize;
            _documentWidth = Math.Max(baseSize.Width - documentLeft - (_vScrollBar != null && _vScrollBar.Visible ? _vScrollBar.Width : 0), 0);
            _documentHeight = Math.Max(baseSize.Height - (_hScrollBar != null && _hScrollBar.Visible ? _hScrollBar.Height : 0), 0);

            // 更新自动换行。
            if (wordWrap) {

                _documentMaxWidth = documentWidth - (_showAutoBreakMark ? Configs.autoBreakMarkWidth : 0);

                // 如果现在的文档宽度比最大的宽度小，需要重新布局。
                if (_maxWidthLine < 0 || _documentMaxWidth < visualLines[_maxWidthLine].right) {
                    updateLayout();
                }

            }

        }

        #endregion

        #region 特殊字符

        /// <summary>
        /// 存储是否显示空白字符。
        /// </summary>
        private bool _showWhitespaces;

        /// <summary>
        /// 存储是否显示换行符。
        /// </summary>
        private bool _showLineBreaks;

        /// <summary>
        /// 显示自动换行。
        /// </summary>
        private bool _showAutoBreakMark;

        /// <summary>
        /// 显示缩进线。
        /// </summary>
        private bool _showIndentGuide;

        /// <summary>
        /// 判断或设置是否显示空白字符。
        /// </summary>
        [Category("Appearance")]
        [Description("判断或设置是否显示空白字符。")]
        public bool showWhitespaces {
            get {
                return _showWhitespaces;
            }
            set {
                if (showWhitespaces != value) {
                    _showWhitespaces = value;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 判断或设置是否显示换行符。
        /// </summary>
        [Category("Appearance")]
        [Description("判断或设置是否显示换行符。")]
        public bool showLineBreaks {
            get {
                return _showLineBreaks;
            }
            set {
                if (showLineBreaks != value) {
                    _showLineBreaks = value;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 判断或设置是否显示全部不可见字符（包含空白、换行等）。
        /// </summary>
        [Category("Appearance")]
        [Description("判断或设置是否显示全部不可见字符（包含空白、换行等）。")]
        public bool showInvisibleChars {
            get {
                return showWhitespaces || showLineBreaks;
            }
            set {
                showWhitespaces = showLineBreaks = value;
            }
        }

        /// <summary>
        /// 判断或设置是否显示自动换行的标记。
        /// </summary>
        [Category("Appearance")]
        [Description("判断或设置是否显示自动换行的标记。")]
        public bool showAutoBreakMark {
            get {
                return _showAutoBreakMark;
            }
            set {
                if (showAutoBreakMark = value) {
                    _showAutoBreakMark = value;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 判断或设置是否显示缩进线。
        /// </summary>
        [Category("Appearance")]
        [Description("判断或设置是否显示缩进线。")]
        public bool showIndentGuide {
            get {
                return _showIndentGuide;
            }
            set {
                if (showAutoBreakMark = value) {
                    _showIndentGuide = value;
                    updateLayout();
                }
            }
        }

        #endregion

        #region 字体和颜色

        /// <summary>
        /// 获取用于绘制整个编辑器的绘图器。
        /// </summary>
        private readonly Painter painter = new Painter(new Font(configs.defaultFontName, configs.defaultFontSize));

        /// <summary>
        /// 存储当前的背景色。
        /// </summary>
        private uint _backColor = Configs.defaultBackColor;

        /// <summary>
        /// 存储当前的前景色。
        /// </summary>
        private uint _foreColor = Configs.defaultForeColor;

        /// <summary>
        /// 存储当前的选区背景色。
        /// </summary>
        private uint _selectionBackColor = Configs.defaultSelectionBackColor;

        /// <summary>
        /// 存储当前的未激活状态选区背景色。
        /// </summary>
        private uint _inactivedSelectionBackColor = Configs.defaultInactivedSelectionBackColor;

        /// <summary>
        /// 存储当前的选区边框色。
        /// </summary>
        private uint _selectionBorderColor = Configs.defaultInactivedSelectionBackColor;

        /// <summary>
        /// 获取或设置控件的前景色。
        /// </summary>
        /// <returns>
        /// 控件的前景 <see cref="T:System.Drawing.Color"/>。 默认为 <see cref="P:System.Windows.Forms.Control.DefaultForeColor"/> 属性的值。
        /// </returns>
        [Description("获取或设置控件的前景色。")]
        public override Color ForeColor {
            get {
                return Painter.fromRGB(_foreColor);
            }
            set {
                updateProperty(ref _foreColor, value);
            }
        }

        /// <summary>
        /// 获取或设置控件的背景色。
        /// </summary>
        /// <returns>
        /// 表示控件背景色的 <see cref="T:System.Drawing.Color"/>。 默认为 <see cref="P:System.Windows.Forms.Control.DefaultBackColor"/> 属性的值。
        /// </returns>
        [Description("获取或设置控件的背景色。")]
        public override Color BackColor {
            get {
                return Painter.fromRGB(_backColor);
            }
            set {
                updateProperty(ref _backColor, value);
            }
        }

        /// <summary>
        /// 获取或设置折叠域的前景色。
        /// </summary>
        [Description("获取或设置折叠域的前景色。")]
        public Color foldBlockForeColor {
            get {
                return Painter.fromRGB(_foldMarkerForeColor);
            }
            set {
                updateProperty(ref _foldMarkerForeColor, value);
            }
        }

        /// <summary>
        /// 获取或设置折叠域的边框色。
        /// </summary>
        [Description("获取或设置折叠域的边框色。")]
        public Color foldBlockBorderColor {
            get {
                return Painter.fromRGB(_foldMarkerBorderColor);
            }
            set {
                updateProperty(ref _foldMarkerBorderColor, value);
            }
        }

        /// <summary>
        /// 获取或设置折叠区域边框选中颜色。
        /// </summary>
        [Description("获取或设置折叠区域边框选中颜色。")]
        public Color foldBlockBorderSelectedColor {
            get {
                return Painter.fromRGB(_foldMarkerBorderSelectedColor);
            }
            set {
                updateProperty(ref _foldMarkerBorderSelectedColor, value);
            }
        }

        /// <summary>
        /// 获取或设置当前编辑的选区背景色。
        /// </summary>
        [Description("获取或设置当前编辑的选区背景色。")]
        public Color selectionBackColor {
            get {
                return Painter.fromRGB(_selectionBackColor);
            }
            set {
                updateProperty(ref _selectionBackColor, value);
            }
        }

        /// <summary>
        /// 获取或设置当前编辑的未激活状态选区背景色。
        /// </summary>
        [Description("获取或设置当前编辑的未激活状态选区背景色。")]
        public Color inactivedSelectionBackColor {
            get {
                return Painter.fromRGB(_selectionBackColor);
            }
            set {
                updateProperty(ref _inactivedSelectionBackColor, value);
            }
        }

        /// <summary>
        /// 获取或设置当前编辑的选区边框色。
        /// </summary>
        [Description("获取或设置当前编辑的选区边框色。")]
        public Color selectionBorderColor {
            get {
                return Painter.fromRGB(_selectionBorderColor);
            }
            set {
                updateProperty(ref _selectionBorderColor, value);
            }
        }

        /// <summary>
        /// 内部更新属性值并重绘界面。
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private void updateProperty(ref uint oldValue, Color newValue) {
            var value = Painter.toRGB(newValue);
            if (oldValue != value) {
                oldValue = value;
                updateLayout();
            }
        }

        #endregion

        #region 绘制

        /// <summary>
        /// 更新某个区间并使之重绘。
        /// </summary>
        /// <param name="startLine">要处理的起始行。</param>
        /// <param name="startColumn">要处理的起始行。</param>
        /// <param name="endLine">要处理的结束行。</param>
        /// <param name="endColumn">要处理的结束列。</param>
        public void invalidate(int startLine, int startColumn, int endLine, int endColumn) {

        }

        /// <summary>
        /// 引发 <see cref="E:System.Windows.Forms.Control.Paint"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Forms.PaintEventArgs"/>。</param>
        protected override void OnPaint(PaintEventArgs e) {
            Utility.mark(e.ClipRectangle);

            painter.beginPaint(e.Graphics);

            draw(e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Right, e.ClipRectangle.Bottom);

            painter.endPaint(e.Graphics);

            return;
            if (!e.ClipRectangle.IsEmpty) {
                this.painter222.BeginPaint(e.Graphics);
                try {
                    this.PaintScrollRect(this.painter222);
                    Rectangle clientRect = this.GetClientRect(true);
                    Rectangle rect = clientRect;
                    rect.Intersect(e.ClipRectangle);

                    int num = 0;
                    if (this.scrolling.ScrollByPixels) {
                        if (this.painter222.lineHeight != 0) {
                            num = this.scrolling.WindowOriginY / this.painter222.lineHeight;
                            int num2 = this.scrolling.WindowOriginY % this.painter222.lineHeight;
                            clientRect.Y -= num2;
                            rect.Height += num2;
                        }
                    } else {
                        num = this.scrolling.WindowOriginY;
                    }

                    this.syntaxPaint.PaintWindow(this.painter222, num, rect, clientRect.Location, 1f, 1f, this.scrolling.ScrollByPixels, false);
                    if (this.Gutter.DrawLineBookmarks) {
                        this.syntaxPaint.PaintLineBookMarks(this.painter222, this.ClientRect);
                    }
                } finally {
                    this.painter222.EndPaint();
                }
                this.syntaxPaint.NeedPaint = false;
                base.OnPaint(e);
            }
        }

        /// <summary>
        /// 重新绘制整个控件指定的区块。
        /// </summary>
        /// <param name="left">绘制的起始位置。</param>
        /// <param name="top">绘制的起始位置。</param>
        /// <param name="right">绘制的结束位置。</param>
        /// <param name="bottom">绘制的结束位置。</param>
        private void draw(int left, int top, int right, int bottom) {

            // 1. 根据 left 决定绘制的内容。

            int line = topToLine(top);

            // 绘制边界。
            drawGutter(visualStartLine, top, bottom);

            // 绘制所有行。
            drawLines(visualStartLine, top, bottom);

        }

        /// <summary>
        /// 在指定位置绘制所有行。
        /// </summary>
        /// <param name="visualStartLine">绘制的目标显示行。</param>
        /// <param name="top">绘制的上边距。</param>
        /// <param name="bottom">最大绘制的下边距。超过此区域停止绘制。</param>
        private void drawLines(int visualStartLine, int top, int bottom) {

            // 绘制背景。
            drawBackground(top, bottom);

            top -= _scrollTop;
            var baseLeft = documentLeft - _scrollLeft;

            // 绘制每行。
            int nextVisualLine;
            for (var visualLineCount = visualLines.Count; visualStartLine < visualLineCount && top < bottom; visualStartLine = nextVisualLine, top += painter.lineHeight) {

                var visualLine = visualLines[visualStartLine];

                // 判断下行是否和当前行属于相同实际行。
                nextVisualLine = visualStartLine + 1;
                var nextVisualLineIsSameLine = nextVisualLine < visualLineCount && visualLines[nextVisualLine].isWrapLine;

                // 绘制每个元素。
                var line = visualLine.startLine;
                var column = visualLine.startColumn;
                var left = baseLeft + visualLine.left;

                // 如果当前行包含折叠项，则先绘制折叠内容。
                for (var visualFoldBlock = visualLine.nextFoldBlock; visualFoldBlock != null; visualFoldBlock = visualFoldBlock.nextFoldBlock) {
                    drawTextBlock(line, column, visualFoldBlock.foldingRange.startColumn, left, top);
                    drawFoldBlock(visualFoldBlock, top);
                    line = visualFoldBlock.foldingRange.endLine;
                    column = visualFoldBlock.foldingRange.endColumn;
                    left = visualFoldBlock.right;
                }

                // 绘制最后一段文本。
                int x = drawTextBlock(line, column, nextVisualLineIsSameLine ? visualLines[nextVisualLine].startColumn : _document.lines[visualLine.startLine].length, left, top);

                // 绘制换行。
                if (_showAutoBreakMark && nextVisualLineIsSameLine) {
                    drawAutoBreaklineMark(top);
                } else if (_showLineBreaks) {
                    // todo
                    // drawControlChar(controlName, x, lastLeft, top);
                }

            }

        }

        /// <summary>
        /// 绘制背景。
        /// </summary>
        /// <param name="top">绘制的起始位置。</param>
        /// <param name="bottom">绘制的结束位置。</param>
        private void drawBackground(int top, int bottom) {
            painter.fillColor = _backColor;
            painter.fillRectangle(gutterWidth, top, gutterWidth + documentWidth, bottom);
        }

        /// <summary>
        /// 绘制指定文本。
        /// </summary>
        /// <param name="visualLineText">要绘制的文本。</param>
        /// <param name="top">绘制的上左边距。</param>
        private int drawTextBlock(int line, int startColumn, int endColumn, int left, int top) {
            var documentLine = _document.lines[line];
            var chars = documentLine.chars;
            var styles = documentLine.styles;
            var x = left - _scrollLeft;

            // 为加速输出，相同样式的文本一次性输出。
            var lastColumn = startColumn;
            var lastStyle = styles[lastColumn];
            var lastLeft = left;
            setCharStyles(lastStyle);

            for (int column = lastColumn; column < endColumn; column++) {

                char ch = chars[column];

                // 特殊处理。
                var controlName = Painter.getControlCharacterName(ch);
                if (controlName != null) {

                    // 输出之前的文本。
                    drawString(chars, lastColumn, column, lastLeft, top);
                    lastColumn = column + 1;

                    // TAB
                    if (ch == '\t') {
                        lastLeft = painter.alignTab(x);
                        if (_showWhitespaces) {
                            drawTab(x, lastLeft, top);
                        }
                        x = lastLeft;
                        continue;
                    }

                    // 绘制控制字符。
                    lastLeft = x + painter.measureString(ch);
                    drawControlChar(controlName, x, lastLeft, top);
                    x = lastLeft;
                    continue;
                }

                // 空白
                if (_showWhitespaces && (ch == ' ' || ch == '\u3000')) {

                    // 输出之前的文本。
                    drawString(chars, lastColumn, column, lastLeft, top);

                    drawWhitespace(ch, x, top);
                    lastLeft = x += painter.measureString(ch);
                    lastColumn = column + 1;
                    continue;
                }

                // 如果当前列样式发生改变，则输出之前的文本并更新样式。
                if (styles[column] != lastStyle) {
                    setCharStyles(lastStyle = styles[column]);
                    drawString(chars, lastColumn, column, lastLeft, top);

                    lastLeft = x;
                    lastColumn = column;

                }

                x += painter.measureString(ch);

            }

            // 绘制最后一段内容。
            if (lastColumn < endColumn) {
                drawString(chars, lastColumn, endColumn, lastLeft, top);
            }

            return x;

        }

        /// <summary>
        /// 使用指定的样式在指定位置绘制折叠的区域。
        /// </summary>
        /// <param name="style"></param>
        /// <param name="chars"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        private void drawFoldBlock(VisualFoldBlock visualLineFoldBlock, int top) {
            //if (selected) {
            //    _painter.foreColor = _collapsedSectionBorderColor;
            //    _painter.fillColor = _selectionBackColor;
            //    _painter.drawRectangle(left, top, right, bottom);
            //    _painter.fillRectangle(left, top, right, bottom);
            //} else {
            //    _painter.foreColor = _collapsedSectionBorderColor;
            //    _painter.drawRectangle(left, top, right, bottom);
            //}

            var right = visualLineFoldBlock.nextFoldBlock.left - _scrollLeft;

            painter.textColor = _foldMarkerForeColor;
            painter.drawString(visualLineFoldBlock.foldingRange.text, visualLineFoldBlock.left - _scrollLeft, top);

        }

        /// <summary>
        /// 设置当前字体的样式。
        /// </summary>
        /// <param name="style"></param>
        private void setCharStyles(CharStyles style) {
            painter.textColor = _foreColor;
        }

        /// <summary>
        /// 使用指定的样式在指定位置绘制指定的文本。
        /// </summary>
        /// <param name="style">绘制的文本样式。</param>
        /// <param name="chars"></param>
        /// <param name="startIndex"></param>
        /// <param name="column"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        private void drawString(char[] chars, int startIndex, int endIndex, int left, int top) {
            painter.drawString(chars, startIndex, endIndex - startIndex, left, top);
        }

        private void drawControlChar(string controlName, int left, int right, int top) {

            painter.fillColor = painter.textColor;
            painter.fillRoundRectangle(left, top + painter.leadingHeight + 4, right, top + painter.baselineHeight + 4);

            var oldTextColor = painter.textColor;
            painter.textColor = painter.backColor;
            painter.drawString(controlName, left, top);
            painter.textColor = oldTextColor;

        }

        private void drawTab(int left, int right, int top) {

        }

        private void drawWhitespace(char ch, int left, int top) {

        }

        /// <summary>
        /// 绘制自动换行标记。
        /// </summary>
        /// <param name="top"></param>
        private void drawAutoBreaklineMark(int top) {
            top -= painter.lineHeight;

        }

        #endregion

    }
}

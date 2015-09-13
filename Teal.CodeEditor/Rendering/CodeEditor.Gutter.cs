using System;
using System.ComponentModel;
using System.Drawing;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        #region 边界大小

        // 左边界 = 书签 + 行号 + 折叠

        /// <summary>
        /// 边界区域宽度。
        /// </summary>
        private int _gutterWidth = Configs.defaultGutterWidth;

        /// <summary>
        /// 存储书签区域的宽度。
        /// </summary>
        private int _bookmarksWidth = Configs.defaultBookmarksWidth;

        /// <summary>
        /// 存储行号区域的宽度。
        /// </summary>
        private int _lineNumbersWidth = Configs.defaultLineNumbersWidth;

        /// <summary>
        /// 当前行号宽度可以显示的最大行号。
        /// </summary>
        private int _maxLineNumberInWidth = -1;

        /// <summary>
        /// 存储由于行号变化导致自动扩充的行宽度。
        /// </summary>
        private int _lineNumberAutoMargin;

        /// <summary>
        /// 获取或设置边界区域的宽度。
        /// </summary>
        [Description("获取或设置边界区域的宽度。")]
        [DefaultValue(Configs.defaultGutterWidth)]
        public int gutterWidth {
            get {
                return _gutterWidth;
            }
            set {
                if (_gutterWidth != value) {
                    _gutterWidth = value;
                    _documentLeft = value + Configs.documentPaddingLeft;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置书签区域的宽度。
        /// </summary>
        [Description("获取或设置书签区域的宽度。")]
        [DefaultValue(Configs.defaultBookmarksWidth)]
        public int bookmarksWidth {
            get {
                return _bookmarksWidth;
            }
            set {
                if (_bookmarksWidth != value) {
                    _bookmarksWidth = value;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 判断或设置是否显示书签。
        /// </summary>
        [Description("判断或设置是否显示书签。")]
        [DefaultValue(Configs.defaultBookmarksWidth > 0)]
        public bool showBookmarks {
            get {
                return _bookmarksWidth > 0;
            }
            set {
                if (showBookmarks != value) {
                    _bookmarksWidth = ~_bookmarksWidth;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置行号区域的宽度。
        /// </summary>
        [Description("获取或设置行号区域的宽度。")]
        [DefaultValue(Configs.defaultLineNumbersWidth)]
        public int lineNumbersWidth {
            get {
                return _lineNumbersWidth;
            }
            set {
                if (_lineNumbersWidth != value) {
                    _lineNumbersWidth = value;
                    _maxLineNumberInWidth = (int)Math.Pow(10, _lineNumbersWidth / painter.fontWidth) - _lineNumbersStart + 1;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 判断或设置是否显示行号。
        /// </summary>
        [Description("判断或设置是否显示行号。")]
        [DefaultValue(Configs.defaultLineNumbersWidth > 0)]
        public bool showLineNumbers {
            get {
                return _lineNumbersWidth > 0;
            }
            set {
                if (showLineNumbers != value) {
                    _lineNumbersWidth = ~_lineNumbersWidth;
                    updateLayout();
                }
            }
        }

        ///// <summary>
        ///// 获取或设置大纲区域的宽度。
        ///// </summary>
        //[Description("获取或设置大纲区域的宽度。")]
        //[DefaultValue(Configs.defaultOutlinesWidth)]
        //public int outlinesWidth {
        //    get {
        //        return _outlinesWidth;
        //    }
        //    set {
        //        if (_outlinesWidth != value) {
        //            _outlinesWidth = value;
        //            updateUI();
        //        }
        //    }
        //}

        ///// <summary>
        ///// 判断或设置是否显示大纲。
        ///// </summary>
        //[Description("判断或设置是否显示大纲。")]
        //[DefaultValue(Configs.defaultOutlinesWidth > 0)]
        //public bool showOutlines {
        //    get {
        //        return _outlinesWidth > 0;
        //    }
        //    set {
        //        if (showOutlines != value) {
        //            _outlinesWidth = ~_outlinesWidth;
        //            updateUI();
        //        }
        //    }
        //}

        /// <summary>
        /// 判断或设置是否显示边界。
        /// </summary>
        [Description("判断或设置是否显示边界。")]
        [DefaultValue(CodeEditorConfigs.defaultGutterWidth > 0)]
        public bool showGutter {
            get {
                return showBookmarks || showLineNumbers || showFoldMarkers;
            }
            set {
                showBookmarks = showLineNumbers = showFoldMarkers = value;
            }
        }

        #endregion

        #region 绘制边界

        /// <summary>
        /// 存储第一行的行号。
        /// </summary>
        private int _lineNumbersStart = Configs.defaultLineNumbersStart;

        /// <summary>
        /// 存储左边界的文本颜色。
        /// </summary>
        private uint _gutterForeColor = Configs.defaultGutterForeColor;

        /// <summary>
        /// 存储左边界的背景颜色。
        /// </summary>
        private uint _gutterBackColor = Configs.defaultGutterBackColor;

        /// <summary>
        /// 获取或设置第一行的行号。
        /// </summary>
        [Description("获取或设置第一行的行号。")]
        [DefaultValue(Configs.defaultLineNumbersStart)]
        public int lineNumbersStart {
            get {
                return _lineNumbersStart;
            }
            set {
                if (_lineNumbersStart != value) {
                    _lineNumbersStart = value;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置边界区域的前景色。
        /// </summary>
        [Description("获取或设置边界区域的前景色。")]
        public Color gutterForeColor {
            get {
                return Painter.fromRGB(_gutterForeColor);
            }
            set {
                updateProperty(ref _gutterForeColor, value);
            }
        }

        /// <summary>
        /// 获取或设置边界区域的背景色。
        /// </summary>
        [Description("获取或设置边界区域的背景色。")]
        public Color gutterBackColor {
            get {
                return Painter.fromRGB(_gutterBackColor);
            }
            set {
                updateProperty(ref _gutterBackColor, value);
            }
        }

        /// <summary>
        /// 存储当前行号字符串缓存。
        /// </summary>
        private char[] _lineBuffer = new char[10];

        /// <summary>
        /// 将指定行号信息更新为当前行字符缓存。
        /// </summary>
        /// <param name="line"></param>
        private int updateLineBuffer(int line) {

            // 添加显示的起始行。
            line += _lineNumbersStart;

            if (line < 10) {
                _lineBuffer[0] = (char)(line + '0');
                return 1;
            }
            if (line < 100) {
                _lineBuffer[1] = (char)(line % 10 + '0');
                _lineBuffer[0] = (char)(line / 10 + '0');
                return 2;
            }
            if (line < 1000) {
                _lineBuffer[2] = (char)(line % 10 + '0');
                _lineBuffer[1] = (char)(line / 10 % 10 + '0');
                _lineBuffer[0] = (char)(line / 100 + '0');
                return 3;
            }
            if (line < 10000) {
                _lineBuffer[3] = (char)(line % 10 + '0');
                _lineBuffer[2] = (char)(line / 10 % 10 + '0');
                _lineBuffer[1] = (char)(line / 100 % 10 + '0');
                _lineBuffer[0] = (char)(line / 1000 + '0');
                return 4;
            }
            if (line < 100000) {
                _lineBuffer[4] = (char)(line % 10 + '0');
                _lineBuffer[3] = (char)(line / 10 % 10 + '0');
                _lineBuffer[2] = (char)(line / 100 % 10 + '0');
                _lineBuffer[1] = (char)(line / 1000 % 10 + '0');
                _lineBuffer[0] = (char)(line / 10000 + '0');
                return 5;
            }
            if (line < 1000000) {
                _lineBuffer[5] = (char)(line % 10 + '0');
                _lineBuffer[4] = (char)(line / 10 % 10 + '0');
                _lineBuffer[3] = (char)(line / 100 % 10 + '0');
                _lineBuffer[2] = (char)(line / 1000 % 10 + '0');
                _lineBuffer[1] = (char)(line / 10000 % 10 + '0');
                _lineBuffer[0] = (char)(line / 100000 + '0');
                return 6;
            }
            if (line < 10000000) {
                _lineBuffer[6] = (char)(line % 10 + '0');
                _lineBuffer[5] = (char)(line / 10 % 10 + '0');
                _lineBuffer[4] = (char)(line / 100 % 10 + '0');
                _lineBuffer[3] = (char)(line / 1000 % 10 + '0');
                _lineBuffer[2] = (char)(line / 10000 % 10 + '0');
                _lineBuffer[1] = (char)(line / 100000 % 10 + '0');
                _lineBuffer[0] = (char)(line / 1000000 + '0');
                return 7;
            }
            if (line < 100000000) {
                _lineBuffer[7] = (char)(line % 10 + '0');
                _lineBuffer[6] = (char)(line / 10 % 10 + '0');
                _lineBuffer[5] = (char)(line / 100 % 10 + '0');
                _lineBuffer[4] = (char)(line / 1000 % 10 + '0');
                _lineBuffer[3] = (char)(line / 10000 % 10 + '0');
                _lineBuffer[2] = (char)(line / 100000 % 10 + '0');
                _lineBuffer[1] = (char)(line / 1000000 % 10 + '0');
                _lineBuffer[0] = (char)(line / 10000000 + '0');
                return 8;
            }
            if (line < 1000000000) {
                _lineBuffer[8] = (char)(line % 10 + '0');
                _lineBuffer[7] = (char)(line / 10 % 10 + '0');
                _lineBuffer[6] = (char)(line / 100 % 10 + '0');
                _lineBuffer[5] = (char)(line / 1000 % 10 + '0');
                _lineBuffer[4] = (char)(line / 10000 % 10 + '0');
                _lineBuffer[3] = (char)(line / 100000 % 10 + '0');
                _lineBuffer[2] = (char)(line / 1000000 % 10 + '0');
                _lineBuffer[1] = (char)(line / 10000000 % 10 + '0');
                _lineBuffer[0] = (char)(line / 100000000 + '0');
                return 9;
            }
            _lineBuffer[9] = (char)(line % 10 + '0');
            _lineBuffer[8] = (char)(line / 10 % 10 + '0');
            _lineBuffer[7] = (char)(line / 100 % 10 + '0');
            _lineBuffer[6] = (char)(line / 1000 % 10 + '0');
            _lineBuffer[5] = (char)(line / 10000 % 10 + '0');
            _lineBuffer[4] = (char)(line / 100000 % 10 + '0');
            _lineBuffer[3] = (char)(line / 1000000 % 10 + '0');
            _lineBuffer[2] = (char)(line / 10000000 % 10 + '0');
            _lineBuffer[1] = (char)(line / 100000000 % 10 + '0');
            _lineBuffer[0] = (char)(line / 1000000000 + '0');
            return 10;
        }

        /// <summary>
        /// 检查当前行号宽度是否足够显示指定的行号。
        /// </summary>
        /// <param name="maxLine">显示的行号。</param>
        private void updateLineNumberWidth(int maxLine) {

            // 加速行号小于指定值时的处理。
            if (_lineNumberAutoMargin == 0 && maxLine < _maxLineNumberInWidth) {
                return;
            }

            var oldLineNumbersWidth = _lineNumbersWidth;

            // 恢复为原长。
            _lineNumbersWidth -= _lineNumberAutoMargin;
            var lineNumberWidth = painter.measureString(_lineBuffer, updateLineBuffer(maxLine));

            _lineNumberAutoMargin = lineNumberWidth - _lineNumbersWidth;

            if (_lineNumberAutoMargin >= 0) {
                _lineNumbersWidth += _lineNumberAutoMargin;
            } else {
                _lineNumberAutoMargin = 0;
            }

            // 更新整个界面。
            if (oldLineNumbersWidth != _lineNumbersWidth) {
                _gutterWidth += _lineNumbersWidth - oldLineNumbersWidth;
                _documentLeft = _gutterWidth + Configs.documentPaddingLeft;
                Invalidate();
            }
        }

        /// <summary>
        /// 绘制边界。
        /// </summary>
        /// <param name="top">需要绘制的上边界。</param>
        /// <param name="bottom">需要绘制的下边界。</param>
        private void drawGutter(int visualStartLine, int top, int bottom) {

            // 绘制边界背景。
            drawGutterBackground(top, bottom);

            for (; visualStartLine < visualLines.Count && top < bottom; visualStartLine++, top += painter.lineHeight) {

                var visualLine = visualLines[visualStartLine];
                if (visualLine.isWrapLine) {
                    continue;
                }

                drawGutter(visualLine, visualStartLine, top);

            }

        }

        /// <summary>
        /// 绘制边界背景。
        /// </summary>
        /// <param name="top">需要绘制的上边界。</param>
        /// <param name="bottom">需要绘制的下边界。</param>
        private void drawGutterBackground(int top, int bottom) {
            painter.fillColor = _gutterBackColor;
            painter.fillRectangle(0, top, gutterWidth, bottom);
        }

        /// <summary>
        /// 绘制边界。
        /// </summary>
        /// <param name="lineInfo">要绘制的行信息。</param>
        /// <param name="line">要绘制的行号。</param>
        /// <param name="top">需要绘制的上边界。</param>
        private void drawGutter(VisualLine visualLineObject, int visualLine, int top) {

            // 绘制行号。
            drawLineNumber(visualLineObject.startLine, top);

            //if (lineInfo != null) {

            //    // 绘制书签。
            //    drawBookmark(lineInfo, line, top);

            //    // 绘制行修改状态。
            //    drawLineModificator(lineInfo, line, top);

            //    // 绘制大纲。
            //    drawOutline(lineInfo, line, top);

            //}

        }

        private void drawBookmark(DocumentLine lineInfo, int line, int top) {

        }

        /// <summary>
        /// 在指定位置绘制行号。
        /// </summary>
        /// <param name="line">要绘制的行号。</param>
        /// <param name="top">绘制的上边距。</param>
        private void drawLineNumber(int line, int top) {
            painter.textColor = _gutterForeColor;
            int len = updateLineBuffer(line);
            painter.drawString(_lineBuffer, 0, len, bookmarksWidth + lineNumbersWidth - painter.measureString(_lineBuffer, len), top);
        }

        #endregion

    }

}

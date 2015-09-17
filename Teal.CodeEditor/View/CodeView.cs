using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码视图。
    /// </summary>
    public sealed class CodeView {

        #region 核心

        /// <summary>
        /// 获取用于绘制整个编辑器的绘图器。
        /// </summary>
        private readonly Painter _painter = new Painter(new Font(CodeEditorConfigs.defaultFontName, CodeEditorConfigs.defaultFontSize));

        /// <summary>
        /// 获取或设置当前视图的配置。
        /// </summary>
        public CodeEditorConfigs configs { get; set; } = new CodeEditorConfigs();

        private Document document { get; set; }

        #endregion

        #region 布局

        private void layoutLine(int line) {
            var layoutLine = new LayoutLine();
            var documentLine = document[line];

            // 获取 documentLine 的所有折叠域。



            // 获取 documentLine 的 segments

            var x = 0;

        }

        #endregion

        #region 绘图

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


        #endregion

        /// <summary>
        /// 存储目前最宽的行。如果值等于 -1 则需重新计算。
        /// </summary>
        private int _maxWidthLine = 0;

        #region 滚动条位置

        /// <summary>
        /// 获取或设置当前编辑器水平滚动的位置。
        /// </summary>
        public int scrollLeft {
            get {
                return _scrollLeft;
            }
            set {
                if (_scrollLeft != value) {
                    _scrollLeft = value;
                    //if (_hScrollBar != null) {
                    //    _hScrollBar.Value = value;
                    //}
                    //updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置当前编辑器垂直滚动的位置。
        /// </summary>
        public int scrollTop {
            get {
                return _scrollTop;
            }
            set {
                if (_scrollTop != value) {
                    _scrollTop = value;
                    //if (_vScrollBar != null) {
                    //    _vScrollBar.Value = value;
                    //}
                    //updateLayout();
                }
            }
        }

        ///// <summary>
        ///// 获取或设置当前编辑器滚动区域。
        ///// </summary>
        //public Point scrollPosition {
        //    get {
        //        return new Point(scrollLeft, scrollTop);
        //    }
        //    set {
        //        setScroll(value.X, value.Y);
        //    }
        //}

        ///// <summary>
        ///// 获取当前控件的滚动大小。
        ///// </summary>
        //public Size scrollSize {
        //    get {
        //        return new Size(scrollWidth, scrollHeight);
        //    }
        //}

        ///// <summary>
        ///// 获取或设置当前滚动的行号。
        ///// </summary>
        //public int scrollLine {
        //    get {
        //        return scrollTop / painter.lineHeight;
        //    }
        //    set {
        //        scrollTop = value * painter.lineHeight;
        //    }
        //}

        ///// <summary>
        ///// 获取或设置当前滚动的列号。
        ///// </summary>
        //public int scrollColumn {
        //    get {
        //        return scrollLeft / painter.fontWidth;
        //    }
        //    set {
        //        scrollLeft = value * painter.fontWidth;
        //    }
        //}

        ///// <summary>
        ///// 更新滚动条的位置。
        ///// </summary>
        //private void updateScrollPositions() {



        //}

        ///// <summary>
        ///// 滚动到指定位置。
        ///// </summary>
        ///// <param name="top"></param>
        ///// <param name="left"></param>
        //public void setScroll(int top, int left) {
        //    beginUpdate();
        //    scrollTop = top;
        //    scrollLeft = left;
        //    endUpdate();
        //}

        ///// <summary>
        ///// 平滑滚动到指定位置。
        ///// </summary>
        ///// <param name="top"></param>
        ///// <param name="left"></param>
        //public void scrollTo(int top, int left) {

        //}

        ///// <summary>
        ///// 平滑滚动到指定位置。
        ///// </summary>
        ///// <param name="deltaX"></param>
        ///// <param name="deltaY"></param>
        //public void scrollBy(int deltaX, int deltaY) {
        //    scrollTo(scrollLeft + deltaX, scrollTop + deltaY);
        //}

        #endregion

        /// <summary>
        /// 切换指定区域的折叠状态。
        /// </summary>
        /// <param name="line">折叠区域。</param>
        /// <returns>如果确实折叠行，则返回 true。</returns>
        public bool toggleFold(int line) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 确保指定位置未被折叠。
        /// </summary>
        /// <param name="line">要显示的行。</param>
        /// <param name="column">要显示的列。</param>
        /// <returns>如果已展开行则返回 true，否则返回 false。</returns>
        public bool ensureVisible(int line, int column) {
            throw new NotImplementedException();
        }

        //private FoldingRange getFoldingRange(int line, int column) {
        //    throw new NotImplementedException();
        //}

        //private FoldingRange getFoldBlockByStart(int line, int column) {
        //    throw new NotImplementedException();
        //}

        //private FoldingRange getFoldingRangeByEnd(int line, int column) {
        //    throw new NotImplementedException();
        //}

        //private FoldingRange getFoldingRangeByLine(int line) {
        //    throw new NotImplementedException();
        //}


        void t() {
            DocumentLine line;
            // line.segments.First().type.c

        }


    }

    /// <summary>
    /// 表示一个布局行。
    /// </summary>
    [DebuggerDisplay("line top={top}")]
    sealed class LayoutLine {

        /// <summary>
        /// 获取当前布局行对应的文档行。
        /// </summary>
        public DocumentLine line;

        /// <summary>
        /// 获取当前布局行对应文档行的第一列。
        /// </summary>
        public int column;

    }

    /// <summary>
    /// 指示一个位置的代码信息。
    /// </summary>
    public struct CodeLayoutInfo {

    }

}

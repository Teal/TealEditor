using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed partial class Document {

        /// <summary>
        /// 获取用于绘制整个编辑器的绘图器。
        /// </summary>
        private readonly Painter _painter = new Painter(new Font(DocumentConfigs.defaultFontName, DocumentConfigs.defaultFontSize));

        /// <summary>
        /// 存储布局需要的全部参数。
        /// </summary>
        private struct LayoutInfo {

            /// <summary>
            /// 当前绘制的行号。
            /// </summary>
            public int line;

            /// <summary>
            /// 当前绘制的列号。
            /// </summary>
            public int column;

            /// <summary>
            /// 当前绘制的上边距。
            /// </summary>
            public int top;

            /// <summary>
            /// 当前绘制的左边距。
            /// </summary>
            public int left;

        }

        /// <summary>
        /// 将当前文档绘制在指定图形上。
        /// </summary>
        /// <param name="graphics">要绘制的目标图形。</param>
        /// <param name="top">开始绘制的垂直位置。</param>
        /// <param name="bottom">结束绘制的垂直位置。</param>
        public void draw(Graphics graphics, int top, int bottom) {
            _painter.beginPaint(graphics);
            draw(0, top, bottom, 0, lines.length);
            _painter.endPaint(graphics);
        }

        /// <summary>
        /// 重绘指定区域内的行。
        /// </summary>
        /// <param name="left">开始绘制的水平位置。</param>
        /// <param name="top">开始绘制的垂直位置。</param>
        /// <param name="startLine">开始绘制的行。</param>
        /// <param name="bottom">结束绘制的垂直位置。</param>
        /// <param name="endLine">结束绘制的行。</param>
        private void draw(int left, int top, int bottom, int startLine, int endLine) {

            LayoutInfo layoutInfo;
            layoutInfo.line = startLine;
            layoutInfo.column = 0;
            layoutInfo.left = left;
            layoutInfo.top = top;

            // 如果存在对应的行，且当前行正在重绘区域内，则继续往下绘制。
            while (layoutInfo.line < endLine && layoutInfo.top < bottom) {
                var documentLine = lines.data[layoutInfo.line];

                // 确保行已经解析。
                if (!documentLine.parsed) {
                    parseSegments(layoutInfo.line);
                }

                // 绘制一行。

                // 获取当前行内已折叠的代码域。
                var block = documentLine.getCollapsedBlock();
                if (block != null && block.startColumn > layoutInfo.column) {

                    // 绘制 折叠域 之前的文本。
                    drawLine(ref layoutInfo, block.startColumn);

                    // 绘制 折叠域。
                    drawCollapsedBlock(ref layoutInfo, block);

                    // 更新行号。
                    layoutInfo.line = lines.indexOf(block.endLine, layoutInfo.line);
                    layoutInfo.column = block.endColumn;

                    Debug.Assert(layoutInfo.line >= 0, "block.endLine 指向了一个被删除的行。");

                    // 继续循环，绘制折叠域之后的内容。
                    continue;

                }

                // 当前行无折叠，直接绘制整行。
                drawLine(ref layoutInfo, documentLine.buffer.length);

                // 换行。
                layoutInfo.line++;
                layoutInfo.column = 0;
                layoutInfo.top += _painter.lineHeight;
                layoutInfo.left = 0;

            }
        }

        /// <summary>
        /// 重绘指定行的文本域，忽略折叠，检测自动换行。
        /// </summary>
        /// <param name="layoutInfo">布局相关的参数。</param>
        /// <param name="endColumn">当前绘制的结束列。</param>
        private void drawLine(ref LayoutInfo layoutInfo, int endColumn) {

            // NOTE: endColumn 必须是 segmentSplitter 所在位置。

            // 当前要绘制的行数据。
            var documentLine = lines.data[layoutInfo.line];
            var textData = documentLine.buffer.data;

            // 记录当前的最新值。
            var column = layoutInfo.column;
            var left = layoutInfo.left;

            // 记录当前绘制的片段进度。
            var segmentSplitterIndex = 0;

            // 1. 跳过 layoutInfo.column 之前的分割器，一般地，这些分割器是已被折叠的代码。
            for (; segmentSplitterIndex < documentLine.segments.length && documentLine.segments.data[segmentSplitterIndex].index < column; segmentSplitterIndex++)
                ;

            // 2. 绘制所有片段。
            for (; segmentSplitterIndex < documentLine.segments.length; segmentSplitterIndex++) {

                // 超出绘制范围停止绘制。
                if (column >= endColumn) {
                    break;
                }

                // 绘制单一片段。
                // 任务：从 layoutInfo.column 到 documentLine.segmentSplitterData[segmentSplitterIndex].index
                // 使用 documentLine.segmentSplitterData[segmentSplitterIndex].type 风格。
                // 考虑自动换行。

                var end = documentLine.segments.data[segmentSplitterIndex].index;

                // TODO: 绘制缩进线。
                if (column == 0) {

                }

                // 设置本次绘制的风格。
                setStyle(documentLine.segments.data[segmentSplitterIndex].type);

                // 一次绘制一个单词。

                for (; column < end; column++) {
                    var c = textData[column];
                    var controlName = Utility.getControlCharName(c);

                    // 碰到特殊字符则直接绘制。
                    if (controlName != null) {

                        // 首先绘制特殊字符之前的字符串。
                        _painter.drawString(textData, layoutInfo.column, column, layoutInfo.left, layoutInfo.top);
                        layoutInfo.left = left;
                        layoutInfo.column = column;

                        // 计算字符的宽度。
                        switch (c) {
                            case '\t':
                                left = alignTab(left);
                                break;
                            case ' ':
                                left += _painter.measureString(c);
                                break;
                            default:
                                left += _painter.measureString(controlName);
                                break;
                        }

                        // 如果超界，则在新行重新绘制。
                        if (left >= _offsetRight && layoutInfo.column > 0) {
                            drawWrap(layoutInfo.top, left);
                            layoutInfo.top += _painter.lineHeight;
                            layoutInfo.left = (documentLine.indentCount + configs.wrapIndentCount) * _painter.measureString(' ');

                            // 重新加上宽度。
                            switch (c) {
                                case '\t':
                                    left = alignTab(left);
                                    break;
                                case ' ':
                                    left += _painter.measureString(c);
                                    break;
                                default:
                                    left += _painter.measureString(controlName);
                                    break;
                            }

                        }

                        switch (c) {
                            case '\t':
                                drawTab(layoutInfo.top, layoutInfo.left);
                                break;
                            case ' ':
                                drawWhitespace(layoutInfo.top, layoutInfo.left);
                                break;
                            default:
                                drawControlChar(controlName, layoutInfo.top, layoutInfo.left);
                                break;
                        }

                    } else {

                        left += _painter.measureString(c);

                        // 如果超界，则返回搜索
                        if (left >= _offsetRight && layoutInfo.column > 0) {

                            // 获取合适的中断点。
                            var oldColumn = column;
                            column = getWrapPoint(textData, layoutInfo.column, oldColumn);

                            // 减去被中断的大小。
                            for (; oldColumn > column; oldColumn--) {
                                left -= _painter.measureString(textData[oldColumn]);
                            }

                            // 绘制之前的字符串。
                            _painter.drawString(textData, layoutInfo.column, column, layoutInfo.left, layoutInfo.top);
                            layoutInfo.left = left;
                            layoutInfo.column = column;

                            // 换行。
                            drawWrap(layoutInfo.top, left);
                            layoutInfo.top += _painter.lineHeight;
                            layoutInfo.left = (documentLine.indentCount + configs.wrapIndentCount) * _painter.measureString(' ');

                        }

                    }

                }

                // 绘制最后一个特殊字符或开头到结尾的字符串。
                _painter.drawString(textData, layoutInfo.column, column, layoutInfo.left, layoutInfo.top);
                layoutInfo.left = left;
                layoutInfo.column = column;
            }
        }

        private void setStyle(SegmentType type) {
            _painter.textColor = type.foreColor;
            _painter.backColor = type.backColor;
        }

        private void drawWrap(int top, int left) {

        }

        private void drawControlChar(string controlName, int left, int top) {
            _painter.drawString(controlName, left, top);
        }

        private void drawWhitespace(int top, int left) {

        }

        private void drawTab(int top, int left) {

        }

        /// <summary>
        /// 计算最合适的自动换行中断位置。如 "hello world" 的最合适中断点是 'w'。
        /// </summary>
        /// <param name="textData">当前的文本数据。</param>
        /// <param name="startIndex">开始处理的位置。</param>
        /// <param name="endIndex">结束处理的位置。</param>
        /// <returns>返回最合适的中端位置。</returns>
        private int getWrapPoint(string textData, int startIndex, int endIndex) {

            // 往前回溯寻找最合算的换行点。
            var wrapPoint = endIndex - 1;

            // 如果此字符是单词组成部分，则不强制分割单词，找到单词之前的首字符。
            for (; wrapPoint > startIndex && syntaxBinding.isWordPart(textData[wrapPoint]); wrapPoint--) {
                Debug.Assert(textData[wrapPoint] != '\t', "TAB 不能是标识符");
                Debug.Assert(textData[wrapPoint] != ' ', "空格不能是标识符");
            }

            // 无法找到合适的单词边界，则强行从行尾中端。
            return wrapPoint == startIndex ? endIndex - 1 : wrapPoint;
        }

        /// <summary>
        /// 计算加上 TAB 后新坐标。
        /// </summary>
        /// <param name="left">当前的左边距。</param>
        /// <returns>返回添加 TAB 后的左边距。</returns>
        private int alignTab(int left) {
            return configs.tabWidth <= 0 ? left - configs.tabWidth : (left / configs.tabWidth + 1) * configs.tabWidth;
        }

        /// <summary>
        /// 当前编辑器代码区域（不含滚动条和边界）的实际宽度。
        /// </summary>
        private int _clientWidth;

        /// <summary>
        /// 当前编辑器代码区域（不含滚动条）的实际高度。
        /// </summary>
        private int _clientHeight;

        private int _offsetTop;

        private int _offsetBottom;

        private int _offsetRight = 400;

        /// <summary>
        /// 重绘指定已折叠块进行布局，检测是否自动换行。
        /// </summary>
        /// <param name="layoutInfo">存储布局相关的参数。绘制结束后将更新位置。</param>
        /// <param name="block">当前的块。</param>
        private void drawCollapsedBlock(ref LayoutInfo layoutInfo, Block block) {

            // 获取折叠后显示的文本。假设无 '\t' 和 特殊字符。
            var collapsedText = getCollapsedText(block);

            // 计算添加折叠域后的新位置。
            var oldLeft = layoutInfo.left;
            layoutInfo.left = _painter.measureString(collapsedText);

            // 自动换行。
            if (layoutInfo.left >= _offsetRight && layoutInfo.column > 0) {
                drawWrap(layoutInfo.top, oldLeft);

                var collapsedTextWidth = layoutInfo.left - oldLeft;
                oldLeft = (lines.data[layoutInfo.line].indentCount + configs.wrapIndentCount) * _painter.measureString(' ');
                layoutInfo.top += _painter.lineHeight;
                layoutInfo.left = oldLeft + collapsedTextWidth;
            }

            _painter.drawString(collapsedText, oldLeft, layoutInfo.top);
        }

        private string getCollapsedText(Block block) {
            return "{...}";
        }
    }

}

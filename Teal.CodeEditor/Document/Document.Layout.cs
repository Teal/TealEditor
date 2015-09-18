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
        /// 重绘指定区域内的行。
        /// </summary>
        /// <param name="left">开始绘制的水平位置。</param>
        /// <param name="top">开始绘制的垂直位置。</param>
        /// <param name="startLine">开始绘制的行。</param>
        /// <param name="bottom">结束绘制的垂直位置。</param>
        /// <param name="endLine">结束绘制的行。</param>
        private void draw(int left, int top, int startLine, int bottom, int endLine) {

            LayoutInfo layoutInfo;
            layoutInfo.line = startLine;
            layoutInfo.column = 0;
            layoutInfo.left = left;
            layoutInfo.top = top;

            // 如果存在对应的行，且当前行正在重绘区域内，则继续往下绘制。
            while (layoutInfo.line < endLine && layoutInfo.top < bottom) {
                var documentLine = _lines[layoutInfo.line];

                // 绘制一行。

                // 获取当前行内已折叠的代码域。
                var block = documentLine.getCollapsedBlock();
                if (block != null && block.startColumn > layoutInfo.column) {

                    // 绘制 折叠域 之前的文本。
                    drawLine(ref layoutInfo, block.startColumn);

                    // 绘制 折叠域。
                    drawCollapsedBlock(ref layoutInfo, block);

                    // 更新行号。
                    layoutInfo.line = indexOf(block.endLine, block.startLine);
                    layoutInfo.column = block.endColumn;

                    Debug.Assert(layoutInfo.line >= 0, "block.endLine 指向了一个被删除的行。");

                    // 继续循环，绘制折叠域之后的内容。
                    continue;

                }

                // 当前行无折叠，直接绘制整行。
                drawLine(ref layoutInfo, documentLine.textLength);

                // 换行。
                layoutInfo.line++;
                layoutInfo.column = 0;
                layoutInfo.top += _painter.lineHeight;
                layoutInfo.left = 0;

            }
        }

        /// <summary>
        /// 重新绘制指定未折叠的文本域，绘制时可能自动换行。
        /// </summary>
        /// <param name="layoutInfo">布局相关的参数。</param>
        /// <param name="endColumn">当前需要布局的结束列。</param>
        private void drawLine(ref LayoutInfo layoutInfo, int endColumn) {

            var documentLine = _lines[layoutInfo.line];
            var textData = documentLine.textData;

            var segmentSplitterIndex = 0;

            // 1. 跳过 layoutInfo.column 之前的分割器，一般地，这些分割器是已被折叠的代码。
            for (; segmentSplitterIndex < documentLine.segmentSplitterCount && documentLine.segmentSplitterData[segmentSplitterIndex].index < layoutInfo.column; segmentSplitterIndex++)
                ;

            // 2. 绘制所有片段。
            for (; segmentSplitterIndex < documentLine.segmentSplitterCount; segmentSplitterIndex++) {

                // 超出绘制范围停止绘制。
                if (layoutInfo.column >= endColumn) {
                    break;
                }

                // TODO: 绘制缩进线。
                if (layoutInfo.column == 0) {

                }

                // 绘制单一片段。
                // 任务：从 layoutInfo.column 到 documentLine.segmentSplitterData[segmentSplitterIndex].index
                // 使用 documentLine.segmentSplitterData[segmentSplitterIndex].type 风格。
                // 考虑自动换行。

                var end = documentLine.segmentSplitterData[segmentSplitterIndex].index;
                int newLeft;

                // 设置本次绘制的风格。
                setStyle(documentLine.segmentSplitterData[segmentSplitterIndex].type);

            drawRest:

                // BUG: 需要优先绘制空格。

                // 计算本次可以绘制的最大列。
                var column = getMaxLayoutColumn(layoutInfo.left, textData, layoutInfo.column, end, out newLeft);

                // 绘制折行之前的内容。
                _painter.drawString(textData, layoutInfo.column, column, layoutInfo.left, layoutInfo.top);

                // 更新当前绘制状态。
                layoutInfo.column = column;
                layoutInfo.left = newLeft;

                // 判断是否存在换行。
                if (column < end) {

                    // TODO: 绘制自动换行标记。

                    // 换行。
                    layoutInfo.top += _painter.lineHeight;
                    layoutInfo.left = (documentLine.indentCount + configs.wrapIndentCount) * _painter.measureString(' ');

                    goto drawRest;

                }

            }

        }

        /// <summary>
        /// 获取指定文本在不换行的条件下所能布局的最大列。
        /// </summary>
        /// <param name="left">当前布局的水平坐标。</param>
        /// <param name="textData">要计算的文本。</param>
        /// <param name="startIndex">计算的开始索引。</param>
        /// <param name="endIndex">计算的结束索引。</param>
        /// <param name="newLeft">返回布局后的新左值。</param>
        /// <returns>返回最大布局的结束索引。</returns>
        private int getMaxLayoutColumn(int left, string textData, int startIndex, int endIndex, out int newLeft) {

            // 保存当前索引。
            var index = startIndex;

            for (; index < endIndex; index++) {

                // 追加当前字符宽度。
                left = alignChar(left, textData[index]);

                // 宽度合适继续布局下一个字符。
                if (left < _offsetRight)
                    continue;

                // 加上当前字符导致需要换行。往前回溯寻找最合算的换行点。
                var splitIndex = index - 1;

                // 如果此字符是单词组成部分，则不强制分割单词，找到单词之前的首字符。
                for (; splitIndex > startIndex && _syntaxBinding.isWordPart(textData[splitIndex]); splitIndex--) {
                    Debug.Assert(textData[splitIndex] != '\t', "TAB 不能是标识符");
                    Debug.Assert(textData[splitIndex] != ' ', "空格不能是标识符");
                }

                // 减去中断点之后字符的宽度。
                // 此处可以放心使用 _painter.measureString 因为
                // 中间不会出现 '\t' 。
                for (; index > splitIndex; index--) {
                    left -= _painter.measureString(textData[index]);
                }

                newLeft = left;
                return splitIndex;
            }

            newLeft = left;
            return index;

        }

        /// <summary>
        /// 计算加上指定字符后新坐标。
        /// </summary>
        /// <param name="left">当前的左边距。</param>
        /// <param name="ch">追加的字符。</param>
        /// <returns>返回添加指定字符后的新坐标。</returns>
        private int alignChar(int left, char ch) {
            return ch == '\t' ? alignTab(left) : (left + _painter.measureString(ch));
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

        private int _offsetRight;

        private void drawSegment(ref LayoutInfo layoutInfo, DocumentLine documentLine, Block parentBlock, int endIndex) {

        }

        /// <summary>
        /// 对指定折叠域进行布局，考虑可能自动换行。
        /// </summary>
        /// <param name="foldingRange">要布局的折叠域。</param>
        /// <param name="layoutInfo">布局相关的参数。</param>
        private void drawCollapsedBlock(ref LayoutInfo layoutInfo, Block block) {

            // 隐藏折叠区域之间的全部行。
            for (var line = layoutInfo.line; line <= foldingRange.endLine; line++) {
                _layoutLines[line].bottom = -1;
            }

            // 创建下一个折叠域。
            layoutInfo.layoutBreakPoint = layoutInfo.layoutBreakPoint.next = new LayoutFoldingBlock(foldingRange);

            var oldLeft = layoutInfo.left;

            // 计算折叠域的宽度。
            layoutInfo.left = painter.alignString(oldLeft, foldingRange.text);

            // 正常显示。
            if (layoutInfo.left < _documentMaxWidth || foldingRange.startColumn <= 0) {
                return;
            }

            // 如果加上折叠区域后的宽度超限，则当前行计算完成，折叠区域应换行显示。
            var foldWidth = layoutInfo.left - oldLeft;

            layoutWrap(ref layoutInfo);

            layoutInfo.left += foldWidth;

        }

    }

}

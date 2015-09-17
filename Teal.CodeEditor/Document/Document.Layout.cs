using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed partial class Document {

        public int drawLines() {

        }

        /// <summary>
        /// 当前展示的滚动 x 位置。
        /// </summary>
        private int _scrollLeft;

        /// <summary>
        /// 当前展示的滚动 y 位置。
        /// </summary>
        private int _scrollTop;

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

        private void draw() {

            LayoutInfo layoutInfo = new LayoutInfo();

            // 如果存在则绘制区域，则继续往下绘制。
            while (layoutInfo.top < _offsetBottom) {
                var documentLine = this[layoutInfo.line];

                // 获取当前行内已折叠的代码域。
                var block = documentLine.getCollapsedBlock();
                if (block != null && block.startColumn > layoutInfo.column) {

                    // 按照 [文本] [折叠域] [文本] 的方式绘制折叠域。

                    // 绘制 折叠域 之前的文本。
                    drawText(ref layoutInfo, block.startColumn);

                    // 绘制 折叠域。
                    drawBlock(ref layoutInfo, block);

                    // 更新行号。
                    layoutInfo.line = getLineNumber(block.endLine, block.startLine);
                    layoutInfo.column = block.endColumn;

                    Debug.Assert(layoutInfo.line >= 0, "block.endLine 指向了一个被删除的行。");

                    // 继续循环，绘制折叠域之后的内容。
                    continue;

                }

                // 当前行无折叠，直接绘制整行。
                drawText(ref layoutInfo, documentLine.length);

                // 换行。

                layoutInfo.line++;
                layoutInfo.column = 0;

                // 界面换行。
                layoutInfo.top += layoutInfo.painter.lineHeight;
                layoutInfo.left = 0;

            }
        }

        /// <summary>
        /// 对指定文本域进行布局，考虑可能自动换行。
        /// </summary>
        /// <param name="endColumn">当前需要布局的结束列。</param>
        /// <param name="layoutInfo">布局相关的参数。</param>
        private void drawText(ref LayoutInfo layoutInfo, int endColumn) {

            var chars = document.lines[layoutInfo.line].chars;

            // 计算当前能绘制的最大列。
            for (var column = layoutInfo.column; column < endColumn;) {

                // 保存之前的宽度。
                var oldLeft = layoutInfo.left;

                // 加上本字符宽度。
                if ((layoutInfo.left = painter.alignChar(oldLeft, chars[column])) < _documentMaxWidth || column <= layoutInfo.column) {
                    column++;
                    continue;
                }

                // 加上当前字符导致需要换行。

                // 从行尾开始找到合适的字符换行。
                for (var splitColumn = column - 1; splitColumn > layoutInfo.column; splitColumn--) {
                    // 如果此字符是单词组成部分，则不强制分割。
                    if (synataxBinding.isWordPart(chars[splitColumn])) {
                        Debug.Assert(chars[splitColumn] != '\t', "TAB 不能是标识符");
                        Debug.Assert(chars[splitColumn] != ' ', "空格不能是标识符");
                        continue;
                    }

                    // 由于拆分列变化，需要回退一些宽度值。
                    for (splitColumn++; --column > splitColumn;) {
                        layoutInfo.left -= painter.measureString(chars[column]);
                    }

                    break;
                }

                // 设置中断的列。
                layoutInfo.column = column;
                layoutInfo.left = oldLeft;

                // 从 column 拆分。
                layoutWrap(ref layoutInfo);

            }

            // 更新列。
            layoutInfo.column = endColumn;
        }

        /// <summary>
        /// 对指定折叠域进行布局，考虑可能自动换行。
        /// </summary>
        /// <param name="foldingRange">要布局的折叠域。</param>
        /// <param name="layoutInfo">布局相关的参数。</param>
        private void drawBlock(FoldingRange foldingRange, ref LayoutInfo layoutInfo) {

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

    /// <summary>
    /// 存储布局需要的全部参数。
    /// </summary>
    public struct LayoutInfo {

        public Document document;

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

}

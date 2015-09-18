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
        private readonly Painter _painter = new Painter(new Font(CodeEditorConfigs.defaultFontName, CodeEditorConfigs.defaultFontSize));

        /// <summary>
        /// 获取或设置当前视图的配置。
        /// </summary>
        public CodeEditorConfigs configs {
            get;
            set;
        } = new CodeEditorConfigs();

        private SyntaxBinding _syntaxBinding;

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

        private int _offsetRight;

        private void draw() {

            LayoutInfo layoutInfo = new LayoutInfo();

            // 如果存在对应的行，且当前正在绘制区域内，则继续往下绘制。
            while (layoutInfo.line < _linesLength && layoutInfo.top < _offsetBottom) {
                var documentLine = _lines[layoutInfo.line];

                // 绘制一行。

                // 首先把行分为多个块。每个块独立绘制。 
                // 有些块可能已折叠，则按折叠块绘制。

                foreach (var endBlock in documentLine.blocks) {

                }

                // 获取当前行内已折叠的代码域。
                var block = documentLine.getCollapsedBlock();
                if (block != null && block.startColumn > layoutInfo.column) {

                    // 绘制 折叠域 之前的文本。
                    drawUncollapsedText(ref layoutInfo, block.startColumn);

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
                drawUncollapsedText(ref layoutInfo, documentLine.textLength);

                // 换行。

                layoutInfo.line++;
                layoutInfo.column = 0;

                // 界面换行。
                layoutInfo.top += layoutInfo.painter.lineHeight;
                layoutInfo.left = 0;

            }
        }

        private void drawSegment(ref LayoutInfo layoutInfo, DocumentLine documentLine, Block parentBlock, int endIndex) {

        }

        private void drawSegment(Block parentBlock, int left, int top, string data, int startIndex, int endIndex) {

        }

        /// <summary>
        /// 对指定文本域进行布局，考虑可能自动换行。
        /// </summary>
        /// <param name="layoutInfo">布局相关的参数。</param>
        /// <param name="endColumn">当前需要布局的结束列。</param>
        private void layoutUncollapsedText(int left, string data, int startIndex, int endIndex) {





        }

        /// <summary>
        /// 计算指定区间的字符宽度。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private int measureWidth(int left, string data, int startIndex, int endIndex) {

        }

        /// <summary>
        /// 计算指定区间的字符宽度。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private int drawWord(int left, int top, string data, int startIndex, int endIndex, SegmentType type) {

        }

        /// <summary>
        /// 对指定未折叠的文本域进行布局，考虑可能自动换行。
        /// </summary>
        /// <param name="endColumn">当前需要布局的结束列。</param>
        /// <param name="layoutInfo">布局相关的参数。</param>
        private void drawUncollapsedText(ref LayoutInfo layoutInfo, int endColumn) {

            // 一行文本内可能包含多个需要高亮的单词。

            var documentLine = _lines[layoutInfo.line];

            var segmentIndex = 0;

            // 计算当前能绘制的最大列。
            for (var column = layoutInfo.column; column < endColumn;) {

                // 保存之前的宽度。
                var oldLeft = layoutInfo.left;

                // 加上本字符宽度。
                if ((layoutInfo.left = _painter.alignChar(oldLeft, documentLine.data[column])) < _offsetRight || column <= layoutInfo.column) {
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

        private void drawWords(int left, int top, DocumentLine documentLine, int startIndex, int endIndex) {

            var data = documentLine.data;
            var currentIndex = startIndex;

            // TODO: 遍历单词 性能优化。
            foreach (var word in documentLine.words) {

                // 不允许单词跨界。
                var start = Math.Max(startIndex, word.startIndex);
                var end = Math.Min(endIndex, word.endIndex);

                // 计算当前单词左边的距离。
                left += measureWidth(left, data, currentIndex, start);

                // 绘制本单词并计算本单词的距离。
                left += drawWord(left, top, data, start, end, word.type);

                // 更新当前绘制的索引。
                currentIndex = end;

            }

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

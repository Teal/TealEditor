using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档视图。
    /// </summary>
    public sealed partial class DocumentView {

        /// <summary>
        /// 存储当前编辑器的文档。
        /// </summary>
        private Document _document = new Document();

        /// <summary>
        /// 获取或设置当前文档的配置。
        /// </summary>
        public DocumentConfigs configs = new DocumentConfigs();



        #region 坐标计算

        /// <summary>
        /// 获取指定行列号当前显示的实际位置。
        /// </summary>
        /// <param name="line">要查找的行。</param>
        /// <param name="column">要查找的列。</param>
        /// <param name="left">返回显示的水平位置。</param>
        /// <param name="top">返回显示的垂直位置。</param>
        /// <param name="foldingRange">如果当前行列号已被折叠，则返回所在折叠域。否则返回 <c>null</c>。</param>
        public void locationToPosition(int line, int column, out int left, out int top, out FoldingRange foldingRange) {

            //foldingRange = null;

            //// 获取保存对应行布局信息的行。
            //Debug.Assert(line >= 0 && line < _layoutLines.Count, "行号超出索引");
            //var layoutLine = line;
            //while (_layoutLines[layoutLine].bottom == -1) {
            //    layoutLine--;
            //}

            //var layoutInfo = _layoutLines[layoutLine];

            //// 加速处理：当前行无折叠和自动换行。
            //if (layoutInfo.next == null) {
            //    Debug.Assert(layoutLine == line, "所有中断点都不包含对应行号");
            //    var chars = document.lines[line].chars;
            //    Debug.Assert(column <= document.lines[line].length, "列号超出索引");
            //    y = layoutInfo.bottom - painter.lineHeight - scrollTop;
            //    x = painter.alignChars(0, chars, 0, column) - scrollLeft + documentLeft;
            //    return;
            //}

            //// 获取上一行高度。
            //var lastLine = line;
            //if (lastLine > 0) {
            //    while (_layoutLines[--lastLine].bottom == -1)
            //        ;
            //}
            //y = _layoutLines[lastLine].bottom;

            //x = 0;
            //var startColumn = 0;
            //for (LayoutBreakPoint bp = layoutInfo; bp != null; bp = bp.next) {

            //    // 当前中断是最后一个中断点，或者当前行列号在当前中断点和下一个中断点之间。
            //    if (bp.next == null || (layoutLine == line && column < bp.next.startColumn)) {
            //        Debug.Assert(layoutLine == line, "所有中断点都不包含对应行号");
            //        var chars = document.lines[line].chars;
            //        Debug.Assert(column <= document.lines[line].length, "列号超出索引");
            //        x += painter.alignChars(0, chars, startColumn, column) - scrollLeft + documentLeft;
            //        return;
            //    }

            //    // 发现自动换行。
            //    if (bp.isWrapPoint) {
            //        y += painter.lineHeight;
            //        x = getWrapIndentWidth(line);
            //        startColumn = ((LayoutWrapPoint)bp).column;
            //        continue;
            //    }

            //    // 发现折叠。
            //    var bpf = (LayoutFoldingBlock)bp;
            //    if (bpf.foldingRange.contains(line, column)) {
            //        foldingRange = bpf.foldingRange;
            //        return;
            //    }

            //    // 切换到下一个折叠域。
            //    x += painter.alignString(x, bpf.foldingRange.text);
            //    layoutLine = bpf.foldingRange.endLine;
            //    startColumn = bpf.foldingRange.endColumn;

            //}

        }

        /// <summary>
        /// 获取指定位置当前正在显示的行列号。
        /// </summary>
        /// <param name="left">要查找的水平位置。</param>
        /// <param name="top">要查找的垂直位置。</param>
        /// <param name="line">返回对应的行。</param>
        /// <param name="column">返回对应的列。</param>
        /// <param name="foldingRange">如果指定位置是一个折叠域，则返回所在折叠域。否则返回 <c>null</c>。</param>
        public void positionToLocation(int left, int top, out int line, out int column, out FoldingRange foldingRange) {

            //foldingRange = null;

            //// 根据 y 大致确定所在行。
            //line = topToLine(y);

            //// 获取对应行的布局信息。
            //LayoutBreakPoint bp = _layoutLines[line];

            //// 用于始终保存当前的左边界值。
            //var left = 0;

            //// 追加滚动条。
            //x += scrollLeft - documentLeft;

            //// 加速处理：当前行无折叠和自动换行。
            //if (bp.next == null) {

            //    var documentLine = document.lines[line];

            //    // 加速处理：超过右边界。
            //    if (x >= ((LayoutLine)bp).right) {
            //        column = documentLine.length;
            //        return;
            //    }

            //    column = leftToColumn(ref left, documentLine.chars, 0, documentLine.length, x);
            //    return;
            //}

            //// 跳过自动换行找到当前所在行。

            //// 获取上一行高度。
            //var lastLine = line;
            //if (lastLine > 0) {
            //    while (_layoutLines[--lastLine].bottom == -1)
            //        ;
            //}

            //// 用于始终保存当前的上边界值。
            //var top = _layoutLines[lastLine].bottom;
            //while (top + painter.lineHeight <= y) {

            //    bp = bp.next;

            //    // 在当前行范围内，一定能找到合适的 DocumentLayoutWrapPoint, 
            //    // 使 top 在合理范围。如果找不到则说明当前 y 不属于当前行范围，
            //    // 则 getLayoutLine 不应该返回当前行。
            //    Debug.Assert(bp != null, "当前行缺少自动换行标记");

            //    if (bp.isWrapPoint) {
            //        left = getWrapIndentWidth(line);
            //        top += painter.lineHeight;
            //    } else {
            //        // DocumentLayoutWrapPoint 未存储 line 信息。
            //        // DocumentLayoutWrapPoint 的 line 和最近的 
            //        // DocumentLayoutLine 或 DocumentLayoutFoldingBlock
            //        // 相同。必须记住最近的 line 。
            //        line = ((LayoutFoldingBlock)bp).foldingRange.endLine;
            //    }

            //}

            //// 跳过折叠找到当前所在列。

            //// bp 现在可能是 DocumentLayoutLine 或 DocumentLayoutWrapPoint

            //// 用于始终保存当前的最小列。
            //var startColumn = bp.startColumn;

            //while (true) {

            //    var chars = document.lines[line].chars;

            //    // 如果当前中断点是最后一个，则搜索到末尾。
            //    if (bp.next == null) {
            //        column = leftToColumn(ref left, chars, startColumn, chars.Length, x);
            //        return;
            //    }

            //    // 判断 x 是否在当前中断点和下个中断点之间。
            //    column = leftToColumn(ref left, chars, startColumn, bp.next.startColumn, x);
            //    if (column < bp.next.startColumn) {
            //        return;
            //    }

            //    // 切换到下一个中断点。
            //    bp = bp.next;

            //    // 下一个中断点已换行，则停止搜索。
            //    if (bp.isWrapPoint) {
            //        return;
            //    }

            //    // bp 现在只能是 DocumentLayoutFoldingBlock

            //    var fbp = (LayoutFoldingBlock)bp;

            //    // 判断当前 x 是否在折叠域中。
            //    var oldLeft = left;
            //    if ((left = painter.alignString(oldLeft, fbp.foldingRange.text)) > x) {
            //        foldingRange = fbp.foldingRange;
            //        if (Utility.isCloserToAThanB(x, oldLeft, left)) {
            //            column = fbp.foldingRange.startColumn;
            //        } else {
            //            line = fbp.foldingRange.endLine;
            //            column = fbp.foldingRange.endColumn;
            //        }
            //        return;
            //    }

            //    // 切换到下一个折叠域。
            //    line = fbp.foldingRange.endLine;
            //    startColumn = fbp.foldingRange.endColumn;
            //}

        }

        /// <summary>
        /// 获取指定垂直坐标所显示的虚拟行号。
        /// </summary>
        /// <param name="top">要查找的垂直坐标。</param>
        /// <returns>返回显示行号。</returns>
        public int topToVisualLine(int top) {
            return (top + _scrollTop) / painter.lineHeight;
        }

        ///// <summary>
        ///// 根据垂直坐标获取获取布局信息中对应行号。
        ///// </summary>
        ///// <param name="y">要查找的垂直坐标。</param>
        ///// <returns>返回行号。</returns>
        //private int topToLine(int y) {

        //    // 大部分情况下，实际行号和显示行号相同。
        //    // 代码折叠可能导致显示行小于实际行号。
        //    // 代码自动换行可能导致显示行大于实际行号。

        //    // 从相同显示行开始查找以提高查找效率。
        //    // 找到最后一个 _layoutInfo[?].top <= y 的位置。

        //    // _layoutInfo 必须不空。
        //    Debug.Assert(_layoutLines.Count > 0);

        //    // 追加滚动条。
        //    y += scrollTop;

        //    // 在文档范围之上。
        //    if (y < _layoutLines[0].bottom) {
        //        return 0;
        //    }

        //    // 在文档范围之下。
        //    if (y >= _layoutLines[_layoutLines.Count - 1].bottom) {
        //        return _layoutLines.Count - 1;
        //    }

        //    // 大致确定所在行号，然后进行调整。
        //    var line = y / painter.lineHeight;

        //    // 找到 min(line) 满足 _layoutInfo[line].bottom <= y，被折叠的行不计算在内。
        //    // _layoutLines[0].bottom <= y，所以不需要额外判断 line >= 0
        //    while (_layoutLines[line].bottom > y || _layoutLines[line].bottom == -1) {
        //        line--;
        //    }

        //    // 找到 max(line) 满足 _layoutInfo[line].bottom > y，被折叠的行不计算在内。
        //    // _layoutLines[len - 1].bottom > y，所以不需要额外判断 line < len
        //    while (_layoutLines[line].bottom <= y) {
        //        line++;
        //    }

        //    // 忽略被折叠的行。
        //    // _layoutLines[0].bottom >= 0，所以不需要额外判断 line >= 0
        //    while (_layoutLines[line].bottom == -1) {
        //        line--;
        //    }

        //    return line;

        //}

        #endregion

    }

}

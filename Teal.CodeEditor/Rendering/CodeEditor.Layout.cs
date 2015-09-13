using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        #region TAB

        /// <summary>
        /// 获取或设置当前显示的 TAB 符的宽度。
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Configs.defaultTabWidth)]
        [Description("获取或设置当前显示的 TAB 符的宽度。")]
        public int tabWidth {
            get {
                return painter.tabWidth / painter.measureString(' ');
            }
            set {
                var newTabWidth = value * painter.measureString(' ');
                if (painter.tabWidth != newTabWidth) {
                    painter.tabWidth = newTabWidth;
                    updateLayout();
                }
            }
        }

        #endregion

        #region 自动换行

        /// <summary>
        /// 文档区域的最大宽度。
        /// </summary>
        private int _documentMaxWidth;

        /// <summary>
        /// 当自动换行后新行的缩进空格数。
        /// </summary>
        private int _wrapIndentWidth;

        /// <summary>
        /// 自动换行后新行是否自动继承上行的缩进。
        /// </summary>
        private bool _inheritIndents = Configs.defaultInheritIndents;

        /// <summary>
        /// 判断或设置是否自动换行。
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Configs.defaultWordWrap)]
        [Description("判断或设置是否自动换行。")]
        public bool wordWrap {
            get {
                return _documentMaxWidth != int.MaxValue;
            }
            set {
                if (wordWrap != value) {
                    if (value) {
                        _documentMaxWidth = documentWidth - (_showAutoBreakMark ? Configs.autoBreakMarkWidth : 0);
                    } else {
                        _documentMaxWidth = int.MaxValue;
                    }
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 判断或设置自动换行后新行是否自动继承上行的缩进。
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Configs.defaultInheritIndents)]
        [Description("判断或设置自动换行后新行是否自动继承上行的缩进。")]
        public bool inheritIndents {
            get {
                return _inheritIndents;
            }
            set {
                if (inheritIndents != value) {
                    _inheritIndents = value;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置当自动换行后新行的缩进空格数。
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Configs.defaultWrapIndentCount)]
        [Description("获取或设置当自动换行后新行的缩进空格数。")]
        public int wrapIndentWidth {
            get {
                return _wrapIndentWidth / painter.measureString(' ');
            }
            set {
                var newWrapIndentWidth = value * painter.measureString(' ');
                if (_wrapIndentWidth != newWrapIndentWidth) {
                    _wrapIndentWidth = newWrapIndentWidth;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取指定行自动换行的布局行缩进宽度。
        /// </summary>
        /// <param name="line">要获取的行。</param>
        /// <returns>返回自动换行缩进宽度。</returns>
        private int getWrapIndentWidth(int line) {

            int x = 0;

            // 计算继承当前行的缩进个数。
            if (inheritIndents) {

                // 忽略折叠。
                var documentLine = document.lines[line];

                for (var i = 0; i < documentLine.length; i++) {
                    char ch = documentLine.chars[i];
                    if (!synataxBinding.isWhitespace(ch)) {
                        break;
                    }
                    x = painter.alignChar(x, ch);
                }

            }

            // 尝试添加自动缩进。
            x += _wrapIndentWidth;

            // 如果添加自动缩进导致宽度超限，则不增加。
            if (x >= _documentMaxWidth) {
                x -= _wrapIndentWidth;
            }

            return x;
        }

        #endregion

        #region 字体

        /// <summary>
        /// 获取或设置控件显示的文字的字体。
        /// </summary>
        /// <returns>
        /// 要应用于由控件显示的文本的 <see cref="T:System.Drawing.Font"/>。 默认为 <see cref="P:System.Windows.Forms.Control.DefaultFont"/> 属性的值。
        /// </returns>
        [Description("获取或设置控件显示的文字的字体。")]
        public override Font Font {
            get {
                return painter.font;
            }
            set {
                base.Font = value;
            }
        }

        /// <summary>
        /// 引发 <see cref="E:System.Windows.Forms.Control.FontChanged"/> 事件。
        /// </summary>
        /// <param name="e">一个 <see cref="T:System.EventArgs"/>，其中包含事件数据。</param>
        protected override void OnFontChanged(EventArgs e) {
            Utility.mark(Font);

            // 重新计算一些依赖字体的变量。
            var oldTabWidth = tabWidth;
            var oldWrapIndentWidth = wrapIndentWidth;

            // 重新设置字体。
            painter.font = base.Font;

            // 重新计算一些依赖字体的变量。
            tabWidth = oldTabWidth;
            wrapIndentWidth = oldWrapIndentWidth;

            base.OnFontChanged(e);

        }

        #endregion

        #region 更新

        /// <summary>
        /// 用于计数是否需要更新控件。
        /// </summary>
        private byte _updateCount;

        /// <summary>
        /// 存储需要更新的最小行。
        /// </summary>
        private int _updatingMinStartLine = int.MaxValue;

        /// <summary>
        /// 存储需要更新的最大行。
        /// </summary>
        private int _updatingMaxEndLine = -1;

        /// <summary>
        /// 通知编辑器应该对所有文档进行重新布局。
        /// </summary>
        public void updateLayout() {
            if (document.lines.Count == 0) {
                return;
            }
            updateLayout(0, 0, document.lines.Count, document.lines[document.lines.Count - 1].length);
        }

        /// <summary>
        /// 通知编辑器应该对指定行区间进行重新布局。
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        public void updateLayout(int startLine, int startColumn, int endLine, int endColumn) {
            if (_updateCount > 0) {
                return;
            }
            document_update(startLine, startColumn, 0, endLine - startLine, 0, endColumn - startColumn);
        }

        /// <summary>
        /// 避免在调用 endUpdate() 方法之前描述控件。
        /// </summary>
        public void beginUpdate() {
            _updateCount++;
        }

        /// <summary>
        /// 在 beginUpdate() 方法挂起描述后，继续描述控件。
        /// </summary>
        public void endUpdate() {
            _updateCount--;
            updateLayout();
        }

        /// <summary>
        /// 当文档内容改变后进行重新布局。
        /// </summary>
        /// <param name="line">发生改变的行。</param>
        /// <param name="column">发生改变的列。</param>
        /// <param name="deleteLineCount">删除的行数。</param>
        /// <param name="insertLineCount">插入的行数。</param>
        /// <param name="deleteColumnCount">删除的列数。</param>
        /// <param name="insertColumnCount">插入的列数。</param>
        private void document_update(int line, int column, int deleteLineCount, int insertLineCount, int deleteColumnCount, int insertColumnCount) {
            Utility.mark(line, column, deleteLineCount, insertLineCount, deleteColumnCount, insertColumnCount);

            // 更新最大行号。
            updateLineNumberWidth(document.lines.Count);

            // 同步 _layoutInfo 以匹配改动。

            // 记录行改变之前的底边位置。
            // 如果行重新布局后底边发生变化，则需要对之后行全部重新布局。
            var oldBottom = _layoutLines[line].bottom;

            // 如果删除的行和插入的行相同，则无需更新 _layoutLines。
            var deltaLine = insertLineCount - deleteLineCount;
            if (deltaLine != 0) {
                if (deltaLine > 0) {
                    for (var i = 0; i < deltaLine; i++) {
                        _layoutLines.Insert(line, new LayoutLine());
                    }
                } else {
                    _layoutLines.RemoveRange(line, -deltaLine);
                }
            }

            // 目前 _layoutLines 和 document.lines 同步。

            // 对插入的新行重新布局。
            line -= deleteLineCount;

            LayoutInfo layoutInfo;
            layoutInfo.bottom = (line == 0 ? 0 : _layoutLines[line - 1].bottom) + painter.lineHeight;
            layoutInfo.needUpdateScroll = false;

            for (var endLine = line + insertLineCount; line <= endLine; line++) {
                layoutInfo.line = line;
                layoutInfo.left = layoutInfo.column = 0;
                layoutInfo.layoutBreakPoint = _layoutLines[line];

                // 绘制当前行。
                layoutLine(ref layoutInfo);

                // 绘制新行。
                layoutNewLine(ref layoutInfo);

            }

            // 高度发生变化，需要重写高度。
            if (_layoutLines[line].bottom != oldBottom) {
                var offset = _layoutLines[line].bottom - oldBottom;
                for (; line < _layoutLines.Count; line++) {
                    _layoutLines[line].bottom += offset;
                }
            } else {

            }

            updateCaret();
        }

        #endregion

        #region 布局逻辑

        /// <summary>
        /// 获取所有行的布局信息。
        /// </summary>
        private readonly List<LayoutLine> _layoutLines = new List<LayoutLine>() { new LayoutLine() };

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
            /// 当前绘制的左边距。
            /// </summary>
            public int left;

            /// <summary>
            /// 当前绘制的底边距离。
            /// </summary>
            public int bottom;

            /// <summary>
            /// 当前的中断点。
            /// </summary>
            public LayoutBreakPoint layoutBreakPoint;

            /// <summary>
            /// 标记是否需要更新滚动条。
            /// </summary>
            public bool needUpdateScroll;

        }

        /// <summary>
        /// 对指定行进行布局，考虑可能自动换行和折叠。
        /// </summary>
        /// <param name="layoutInfo">布局相关的参数。</param>
        private void layoutLine(ref LayoutInfo layoutInfo) {

            // 重置 right 以让 right 最后更新为全部布局行的最大值。
            _layoutLines[layoutInfo.line].right = 0;

            // 当前展示行可能已被折叠。
            FoldingRange foldingRange;
            while ((foldingRange = getFoldingRange(layoutInfo.line)) != null && foldingRange.isFolded) {

                // 首先布局折叠之前的内容。
                layoutTextBlock(foldingRange.startColumn, ref layoutInfo);

                // 布局折叠区域。
                layoutFlodBlock(foldingRange, ref layoutInfo);

                // 切换当前行到折叠结束为止。
                layoutInfo.line = foldingRange.endLine;
                layoutInfo.column = foldingRange.endColumn;

            }

            // 绘制剩下的内容。
            layoutTextBlock(document.lines[layoutInfo.line].length, ref layoutInfo);

        }

        /// <summary>
        /// 对指定文本域进行布局，考虑可能自动换行。
        /// </summary>
        /// <param name="endColumn">当前需要布局的结束列。</param>
        /// <param name="layoutInfo">布局相关的参数。</param>
        private void layoutTextBlock(int endColumn, ref LayoutInfo layoutInfo) {

            var chars = document.lines[layoutInfo.line].chars;

            // 计算当前能绘制的最大列。
            for (var column = layoutInfo.column; column < endColumn; ) {

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
                    for (splitColumn++; --column > splitColumn; ) {
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
        private void layoutFlodBlock(FoldingRange foldingRange, ref LayoutInfo layoutInfo) {

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

        /// <summary>
        /// 实现自动换行。
        /// </summary>
        /// <param name="layoutInfo">布局相关的参数。</param>
        private void layoutWrap(ref LayoutInfo layoutInfo) {

            // 添加中断点。
            layoutInfo.layoutBreakPoint = layoutInfo.layoutBreakPoint.next = new LayoutWrapPoint(layoutInfo.column);

            // 结束当前行布局。
            layoutNewLine(ref layoutInfo);

            // 自动换行，重新计算左边距。
            layoutInfo.left = getWrapIndentWidth(layoutInfo.line);

        }

        /// <summary>
        /// 实现换行。
        /// </summary>
        /// <param name="layoutInfo">布局相关的参数。</param>
        private void layoutNewLine(ref LayoutInfo layoutInfo) {

            // 记录最长的行以更新水平滚动条。
            if (_hScrollBar != null) {
                if (_maxWidthLine >= 0) {
                    if (layoutInfo.left >= _layoutLines[_maxWidthLine].right) {

                        // 如果当前行比最长行更长，则设置当前行是最长行。
                        _maxWidthLine = layoutInfo.line;

                        // 需要重新计算滚动条大小。
                        layoutInfo.needUpdateScroll = true;
                    } else if (_maxWidthLine == layoutInfo.line) {

                        // 如果当前行本身是最长的行，且新长度变小了，则无法确定目前的最长行。
                        _maxWidthLine = -1;

                        // 需要重新计算滚动条大小。
                        layoutInfo.needUpdateScroll = true;
                    }

                } else {

                    // 需要重新计算滚动条大小。
                    layoutInfo.needUpdateScroll = true;
                }
            }

            // 保存当前行右边距和下边距。
            var layoutLine = _layoutLines[layoutInfo.line];
            if (layoutLine.right < layoutInfo.left) {
                layoutLine.right = layoutInfo.left;
            }
            layoutLine.bottom = layoutInfo.bottom;

            // 切换新行。
            //layoutInfo.left = 0;
            layoutInfo.bottom += painter.lineHeight;
        }

        #endregion

        #region 坐标换算

        /// <summary>
        /// 根据行列号获取对应的显示坐标。
        /// </summary>
        /// <param name="line">要查找的行。</param>
        /// <param name="column">要查找的列。</param>
        /// <param name="x">输出对应的水平坐标。</param>
        /// <param name="y">输出对应的垂直坐标。</param>
        /// <param name="foldingRange">输出对应的折叠区域。如果不存在则返回 null。</param>
        public void locationToPosition(int line, int column, out int x, out int y, out FoldingRange foldingRange) {

            foldingRange = null;

            // 获取保存对应行布局信息的行。
            Debug.Assert(line >= 0 && line < _layoutLines.Count, "行号超出索引");
            var layoutLine = line;
            while (_layoutLines[layoutLine].bottom == -1) {
                layoutLine--;
            }

            var layoutInfo = _layoutLines[layoutLine];

            // 加速处理：当前行无折叠和自动换行。
            if (layoutInfo.next == null) {
                Debug.Assert(layoutLine == line, "所有中断点都不包含对应行号");
                var chars = document.lines[line].chars;
                Debug.Assert(column <= document.lines[line].length, "列号超出索引");
                y = layoutInfo.bottom - painter.lineHeight - scrollTop;
                x = painter.alignChars(0, chars, 0, column) - scrollLeft + documentLeft;
                return;
            }

            // 获取上一行高度。
            var lastLine = line;
            if (lastLine > 0) {
                while (_layoutLines[--lastLine].bottom == -1)
                    ;
            }
            y = _layoutLines[lastLine].bottom;

            x = 0;
            var startColumn = 0;
            for (LayoutBreakPoint bp = layoutInfo; bp != null; bp = bp.next) {

                // 当前中断是最后一个中断点，或者当前行列号在当前中断点和下一个中断点之间。
                if (bp.next == null || (layoutLine == line && column < bp.next.startColumn)) {
                    Debug.Assert(layoutLine == line, "所有中断点都不包含对应行号");
                    var chars = document.lines[line].chars;
                    Debug.Assert(column <= document.lines[line].length, "列号超出索引");
                    x += painter.alignChars(0, chars, startColumn, column) - scrollLeft + documentLeft;
                    return;
                }

                // 发现自动换行。
                if (bp.isWrapPoint) {
                    y += painter.lineHeight;
                    x = getWrapIndentWidth(line);
                    startColumn = ((LayoutWrapPoint)bp).column;
                    continue;
                }

                // 发现折叠。
                var bpf = (LayoutFoldingBlock)bp;
                if (bpf.foldingRange.contains(line, column)) {
                    foldingRange = bpf.foldingRange;
                    return;
                }

                // 切换到下一个折叠域。
                x += painter.alignString(x, bpf.foldingRange.text);
                layoutLine = bpf.foldingRange.endLine;
                startColumn = bpf.foldingRange.endColumn;

            }

        }

        /// <summary>
        /// 根据行列号显示坐标获取对应的行列号。
        /// </summary>
        /// <param name="x">要查找的水平坐标。</param>
        /// <param name="y">要查找的垂直坐标。</param>
        /// <param name="line">输出对应的行。</param>
        /// <param name="column">输出对应的列。</param>
        /// <param name="foldingRange">输出对应的折叠区域。如果不存在则返回 null。</param>
        public void positionToLocation(int x, int y, out int line, out int column, out FoldingRange foldingRange) {

            foldingRange = null;

            // 根据 y 大致确定所在行。
            line = topToLine(y);

            // 获取对应行的布局信息。
            LayoutBreakPoint bp = _layoutLines[line];

            // 用于始终保存当前的左边界值。
            var left = 0;

            // 追加滚动条。
            x += scrollLeft - documentLeft;

            // 加速处理：当前行无折叠和自动换行。
            if (bp.next == null) {

                var documentLine = document.lines[line];

                // 加速处理：超过右边界。
                if (x >= ((LayoutLine)bp).right) {
                    column = documentLine.length;
                    return;
                }

                column = leftToColumn(ref left, documentLine.chars, 0, documentLine.length, x);
                return;
            }

            // 跳过自动换行找到当前所在行。

            // 获取上一行高度。
            var lastLine = line;
            if (lastLine > 0) {
                while (_layoutLines[--lastLine].bottom == -1)
                    ;
            }

            // 用于始终保存当前的上边界值。
            var top = _layoutLines[lastLine].bottom;
            while (top + painter.lineHeight <= y) {

                bp = bp.next;

                // 在当前行范围内，一定能找到合适的 DocumentLayoutWrapPoint, 
                // 使 top 在合理范围。如果找不到则说明当前 y 不属于当前行范围，
                // 则 getLayoutLine 不应该返回当前行。
                Debug.Assert(bp != null, "当前行缺少自动换行标记");

                if (bp.isWrapPoint) {
                    left = getWrapIndentWidth(line);
                    top += painter.lineHeight;
                } else {
                    // DocumentLayoutWrapPoint 未存储 line 信息。
                    // DocumentLayoutWrapPoint 的 line 和最近的 
                    // DocumentLayoutLine 或 DocumentLayoutFoldingBlock
                    // 相同。必须记住最近的 line 。
                    line = ((LayoutFoldingBlock)bp).foldingRange.endLine;
                }

            }

            // 跳过折叠找到当前所在列。

            // bp 现在可能是 DocumentLayoutLine 或 DocumentLayoutWrapPoint

            // 用于始终保存当前的最小列。
            var startColumn = bp.startColumn;

            while (true) {

                var chars = document.lines[line].chars;

                // 如果当前中断点是最后一个，则搜索到末尾。
                if (bp.next == null) {
                    column = leftToColumn(ref left, chars, startColumn, chars.Length, x);
                    return;
                }

                // 判断 x 是否在当前中断点和下个中断点之间。
                column = leftToColumn(ref left, chars, startColumn, bp.next.startColumn, x);
                if (column < bp.next.startColumn) {
                    return;
                }

                // 切换到下一个中断点。
                bp = bp.next;

                // 下一个中断点已换行，则停止搜索。
                if (bp.isWrapPoint) {
                    return;
                }

                // bp 现在只能是 DocumentLayoutFoldingBlock

                var fbp = (LayoutFoldingBlock)bp;

                // 判断当前 x 是否在折叠域中。
                var oldLeft = left;
                if ((left = painter.alignString(oldLeft, fbp.foldingRange.text)) > x) {
                    foldingRange = fbp.foldingRange;
                    if (Utility.isCloserToAThanB(x, oldLeft, left)) {
                        column = fbp.foldingRange.startColumn;
                    } else {
                        line = fbp.foldingRange.endLine;
                        column = fbp.foldingRange.endColumn;
                    }
                    return;
                }

                // 切换到下一个折叠域。
                line = fbp.foldingRange.endLine;
                startColumn = fbp.foldingRange.endColumn;
            }

        }

        /// <summary>
        /// 根据垂直坐标获取获取布局信息中对应行号。
        /// </summary>
        /// <param name="y">要查找的垂直坐标。</param>
        /// <returns>返回行号。</returns>
        private int topToLine(int y) {

            // 大部分情况下，实际行号和显示行号相同。
            // 代码折叠可能导致显示行小于实际行号。
            // 代码自动换行可能导致显示行大于实际行号。

            // 从相同显示行开始查找以提高查找效率。
            // 找到最后一个 _layoutInfo[?].top <= y 的位置。

            // _layoutInfo 必须不空。
            Debug.Assert(_layoutLines.Count > 0);

            // 追加滚动条。
            y += scrollTop;

            // 在文档范围之上。
            if (y < _layoutLines[0].bottom) {
                return 0;
            }

            // 在文档范围之下。
            if (y >= _layoutLines[_layoutLines.Count - 1].bottom) {
                return _layoutLines.Count - 1;
            }

            // 大致确定所在行号，然后进行调整。
            var line = y / painter.lineHeight;

            // 找到 min(line) 满足 _layoutInfo[line].bottom <= y，被折叠的行不计算在内。
            // _layoutLines[0].bottom <= y，所以不需要额外判断 line >= 0
            while (_layoutLines[line].bottom > y || _layoutLines[line].bottom == -1) {
                line--;
            }

            // 找到 max(line) 满足 _layoutInfo[line].bottom > y，被折叠的行不计算在内。
            // _layoutLines[len - 1].bottom > y，所以不需要额外判断 line < len
            while (_layoutLines[line].bottom <= y) {
                line++;
            }

            // 忽略被折叠的行。
            // _layoutLines[0].bottom >= 0，所以不需要额外判断 line >= 0
            while (_layoutLines[line].bottom == -1) {
                line--;
            }

            return line;

        }

        /// <summary>
        /// 根据水平坐标获取获取对应的列号。
        /// </summary>
        /// <param name="left">当前的水平坐标位置，函数返回前更新为最新值。</param>
        /// <param name="chars">所有字符数组。</param>
        /// <param name="startColumn">字符数组的起始位置。</param>
        /// <param name="endColumn">字符数组的结束位置。</param>
        /// <param name="x">要查找的水平坐标。</param>
        /// <returns>返回对应的列表。</returns>
        private int leftToColumn(ref int left, char[] chars, int startColumn, int endColumn, int x) {

            // 当前行在 x 之前，则修复为第一列。
            if (left <= x) {
                return startColumn;
            }

            for (; startColumn < endColumn; startColumn++) {
                var oldLeft = left;
                if ((left = painter.alignChar(oldLeft, chars[startColumn])) > x) {
                    return Utility.isCloserToAThanB(x, oldLeft, left) ? startColumn : (startColumn + 1);
                }
            }
            return endColumn;
        }

        //#region locationTo


        /////// <summary>
        /////// 根据行列号获取获取对应的显示行号。
        /////// </summary>
        /////// <param name="line">要查找的行。</param>
        /////// <param name="column">要查找的列。</param>
        /////// <returns>返回显示行号。</returns>
        ////public int locationToVisualLine(int line, int column) {
        ////    return line;
        ////}

        /////// <summary>
        /////// 根据行列号获取对应的坐标。
        /////// </summary>
        /////// <param name="line">要查找的行。</param>
        /////// <param name="column">要查找的列。</param>
        /////// <returns>返回坐标。</returns>
        ////public Point locationToPosition(Point p) {
        ////    int x, y;
        ////    locationToPosition(p.Y, p.X, out x, out y);
        ////    return new Point(x, y);
        ////}

        //#endregion

        //#region positionTo

        /////// <summary>
        /////// 根据垂直坐标获取对应显示行号。
        /////// </summary>
        /////// <param name="top">要查找的坐标。</param>
        /////// <returns>返回显示行号。</returns>
        ////public int topToVisualLine(int top) {
        ////    return (top + scrollTop) / painter.lineHeight;
        ////}

        /////// <summary>
        /////// 根据行列号显示坐标获取对应的行列号。
        /////// </summary>
        /////// <param name="x">要查找的水平坐标。</param>
        /////// <param name="y">要查找的垂直坐标。</param>
        /////// <returns>返回行列号。</returns>
        ////public Point positionToLocation(Point p) {
        ////    int line, column;
        ////    FoldingRange foldingRange;
        ////    positionToLocation(p.X, p.Y, out line, out column, out foldingRange);
        ////    return new Point(column, line);
        ////}

        //#endregion

        ////#region visualLineTo

        /////// <summary>
        /////// 根据显示行号获取其垂直坐标。
        /////// </summary>
        /////// <param name="visualLine">显示的行号。</param>
        /////// <returns>返回垂直坐标。</returns>
        ////public int visualLineToTop(int visualLine) {
        ////    return painter.lineHeight * visualLine - scrollTop;
        ////}

        /////// <summary>
        /////// 根据显示行号获取其第一个字符的行列号。
        /////// </summary>
        /////// <param name="visualLine">显示的行号。</param>
        /////// <param name="line">输出对应的行。</param>
        /////// <param name="column">输出对应的列。</param>
        ////public void visualLineToLocation(int visualLine, out int line, out int column) {

        ////}

        ////#endregion

        //#region getVisualLine

        /////// <summary>
        /////// 根据文档行列号获取显示行列号。
        /////// </summary>
        /////// <param name="line"></param>
        /////// <param name="column"></param>
        /////// <param name="visualLine"></param>
        /////// <param name="visualColumn"></param>
        ////public void getVisualLocationFromDocumentLocation(int line, int column, out int visualLine, out int visualColumn) {
        ////    throw new NotImplementedException();
        ////}

        /////// <summary>
        /////// 根据显示行获取其第一个字符的文档行号。
        /////// </summary>
        /////// <param name="visualLine">显示的行。</param>
        /////// <returns>行号。</returns>
        ////public int getDocumentLocationFromVisualLocation(int visualLine) {
        ////    return visualLines[visualLine].line;
        ////}

        /////// <summary>
        /////// 根据显示行获取其第一个字符的文档列号。
        /////// </summary>
        /////// <param name="visualLine">显示的行。</param>
        /////// <returns>列。</returns>
        ////public int getDocumentColumnFromVisualLine(int visualLine) {
        ////    return visualLines[visualLine].column;
        ////}

        ////public int getDocumentLineFromVisualLocation(int visualLine, ref int visualColumn) {
        ////    var visualLineObject = visualLines[visualLine];
        ////    var lastLine = visualLineObject.startLine;
        ////    var lastColumn = visualLineObject.startColumn;

        ////    for (var foldBlock = visualLineObject.nextFoldBlock; foldBlock != null; foldBlock = foldBlock.nextFoldBlock) {

        ////        // 之前块到当前折叠域之前使用了 columnSpan。
        ////        var columnSpan = foldBlock.foldingRange.startColumn - lastColumn;
        ////        if (lastColumn < columnSpan) {
        ////            return lastLine;
        ////        }

        ////        lastColumn = foldBlock.foldingRange.endColumn;
        ////        lastLine = foldBlock.foldingRange.endLine;
        ////    }

        ////}

        //#endregion

        ////private int getLeftFromVisualLocation(int visualLine, int line, int colum) {

        ////    // line, column 已确保在文档范围中。

        ////    var visualLineObject = visualLines[visualLine];
        ////    var currentColumn = visualLineObject.startColumn;
        ////    var x = 0;

        ////    // 如果有折叠，根据行列定位。
        ////    for (var foldBlock = visualLineObject.nextFoldBlock; foldBlock != null; foldBlock = foldBlock.nextFoldBlock) {

        ////        // 当前文本在指定折叠域之前，则已成功定位到文本位置。
        ////        if (foldBlock.foldingRange.isAfter(line, colum)) {
        ////            break;
        ////        }

        ////        // 当前文本在指定折叠域外，需要继续查找下一个折叠域。
        ////        currentColumn = foldBlock.foldingRange.endColumn;
        ////        x = foldBlock.right;

        ////    }

        ////    // 计算从 currentColumn 到 column 的宽度。
        ////    for (var chars = document.lines[line].chars; currentColumn < colum; currentColumn++) {
        ////        x = painter.alignChar(x, chars[currentColumn]);
        ////    }

        ////    return x + documentLeft - _scrollLeft;

        ////}

        ////private void getEndLocation(int visualLine, out int endLine, out int endColumn) {
        ////    // 如果下一行和当前行是同一实际行，则从下行获取。
        ////    if (visualLine + 1 < visualLines.Count && visualLines[visualLine + 1].isWrapLine) {
        ////        endLine = visualLines[visualLine + 1].startLine;
        ////        endColumn = visualLines[visualLine + 1].startColumn;
        ////        return;
        ////    }

        ////    endLine = visualLines[visualLine].endLine;
        ////    endColumn = document.lines[endLine].length;
        ////}

        /////// <summary>
        /////// 获取指定位置显示的实际行号。
        /////// </summary>
        /////// <param name="top">要查找的行。</param>
        /////// <returns>返回显示行号。</returns>
        ////public int getVisualLocationFromPosition(int top) {
        ////    return (top + scrollTop) / _painter.lineHeight;
        ////}

        /////// <summary>
        /////// 获取指定坐标的实际行列号。
        /////// </summary>
        /////// <param name="x"></param>
        /////// <param name="y"></param>
        /////// <param name="line"></param>
        /////// <param name="column"></param>
        ////public void getLineColumnFromPosition(int x, int y, out int line, out int column, out bool isCollapsedSection) {

        ////    isCollapsedSection = false;

        ////    // 转为 displayLine 。
        ////    var displayLine = y / _painter.fontHeight;

        ////    // 超出屏幕边界。
        ////    if (displayLine < 0 || displayLine >= _visualLineCount) {
        ////        line = displayLine;
        ////        column = 0;
        ////        return;
        ////    }

        ////    line = _visualLines[displayLine].line;

        ////    var lineInfo = document.lines[line];

        ////    // 超出当前行宽度。
        ////    if (x >= _visualLines[displayLine].right) {
        ////        column = lineInfo.length;
        ////        return;
        ////    }

        ////    // 边界区域。
        ////    if (x < _gutterWidth) {
        ////        // todo: 计算边界范围。
        ////        column = -1;
        ////        return;
        ////    }

        ////    // 根据 x 计算列。

        ////    var currentColumn = _visualLines[displayLine].column;
        ////    var currentLeft = _gutterWidth;

        ////    // 如果当前行不是从第 0 列开始，说明是自动换行，计算自动换行空间。
        ////    if (currentColumn > 0) {

        ////        //var i = displayLine;

        ////        //for (; i > 0; i--) {
        ////        //    if (_displayLines[i - 1].line != line) {
        ////        //        break;
        ////        //    }
        ////        //}

        ////        //getIndentionWidth(lineInfo.chars, 0, _displayLines[i]);
        ////    }

        ////calcRest:

        ////    // 如果当前行已折叠。
        ////    if (lineInfo.collapseState == CollapseState.collapsed) {

        ////        // 搜索是否在折叠区域前文本。
        ////        column = getColumnByLeft(lineInfo, currentColumn, lineInfo.collapseStartColumn, ref currentLeft, x);

        ////        if (column >= 0) {
        ////            return;
        ////        }

        ////        // 搜索是否在折叠区域。
        ////        column = getCollapsedSectionByLeft(lineInfo, ref currentLeft, x);

        ////        if (column >= 0) {
        ////            isCollapsedSection = true;
        ////            return;
        ////        }

        ////        // 加上折叠区域。
        ////        line += lineInfo.collapseRowCount;
        ////        lineInfo = document.lines[line];
        ////        currentColumn = lineInfo.collapseEndColumn;

        ////        goto calcRest;

        ////    }

        ////    column = getColumnByLeft(lineInfo, currentColumn, lineInfo.length, ref currentLeft, x);

        ////}

        ////private int getColumnByLeft(LineInfo lineInfo, int startColumn, int endColumn, ref int currentLeft, int maxLeft) {

        ////    int left = currentLeft;
        ////    for (int i = startColumn; i < endColumn; i++) {

        ////        // 获取当前字符宽度。
        ////        var charWidth = getCharWidth(lineInfo.chars[i], left);
        ////        left += charWidth;

        ////        // 如果加上 left 后超过范围，说明当前列就是搜索的列。
        ////        if (left > maxLeft) {

        ////            // 根据在当前字符的位置区分当前列还是下一列。
        ////            if (left - maxLeft >= charWidth >> 1) {
        ////                return i;
        ////            }

        ////            return i + 1;

        ////        }

        ////    }

        ////    currentLeft = left;
        ////    return -1;
        ////}

        ////private int getCollapsedSectionByLeft(LineInfo lineInfo, ref int currentLeft, int maxLeft) {

        ////    var lastLeft = currentLeft;

        ////    // 计算折叠域的宽度。
        ////    int left = lastLeft;
        ////    for (var i = 0; i < lineInfo.collapsedText.Length; i++) {
        ////        left += getCharWidth(lineInfo.collapsedText[i], left);
        ////    }

        ////    if (left > maxLeft) {

        ////        if (left - maxLeft > (left - lastLeft) >> 1) {
        ////            return lineInfo.collapseStartColumn;
        ////        }

        ////        return lineInfo.collapseEndColumn;

        ////    }

        ////    currentLeft = left;
        ////    return -1;

        ////}

        #endregion

        //#region 工具函数

        /////// <summary>
        /////// 判断一个字符是否是单词的分隔符。
        /////// </summary>
        /////// <param name="ch"></param>
        /////// <returns></returns>
        ////public bool isDelimiter(char ch) {
        ////    return !char.IsLetterOrDigit(ch) && ch != '@' && ch != '$' && ch != '_';
        ////}

        //#endregion

    }

}

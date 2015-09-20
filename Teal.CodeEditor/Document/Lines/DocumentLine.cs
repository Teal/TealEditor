using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示文档中的一行。
    /// </summary>
    public sealed class DocumentLine {

        #region 字符

        /// <summary>
        /// 获取当前行的所有字符串缓存。
        /// </summary>
        public StringBuffer buffer;

        /// <summary>
        /// 获取或设置当前行的文本。
        /// </summary>
        public string text {
            get {
                return buffer.ToString();
            }
            set {
                buffer.set(value);
            }
        }

        /// <summary>
        /// 获取或设置当前行指定位置的字符。
        /// </summary>
        /// <param name="index">要获取的索引。</param>
        /// <returns>返回对应的字符。</returns>
        public char this[int index] {
            get {
                return buffer[index];
            }
            set {
                buffer[index] = value;
            }
        }

        /// <summary>
        /// 获取或设置当前行指定区域的子字符串。
        /// </summary>
        /// <param name="startIndex">要获取的开始索引。</param>
        /// <param name="endIndex">要获取的结束索引。</param>
        /// <returns>返回子字符串。</returns>
        public string this[int startIndex, int endIndex] {
            get {
                return buffer[startIndex, endIndex];
            }
            set {
                buffer[startIndex, endIndex] = value;
            }
        }

        /// <summary>
        /// 获取当前行的实际字符长度。
        /// </summary>
        public int textLength => buffer.length;

        /// <summary>
        /// 初始化 <see cref="DocumentLine"/> 类的新实例。
        /// </summary>
        /// <param name="value">初始化的值。</param>
        public DocumentLine(string value) {
            buffer = new StringBuffer(value);
            segments = new ArrayList<SegmentSplitter>(value.Length >> 4);
        }

        /// <summary>
        /// 初始化 <see cref="DocumentLine" /> 类的新实例。
        /// </summary>
        /// <param name="capacity">初始化容量。</param>
        public DocumentLine(int capacity = 16) {
            buffer = new StringBuffer(capacity);
            segments = new ArrayList<SegmentSplitter>(capacity >> 4);
        }

        /// <summary>
        /// 返回当前行的字符串形式。
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString() => buffer.ToString();

        #endregion

        #region 行属性

        /// <summary>
        /// 获取当前行和上一行之间的换行符。
        /// </summary>
        public string newLine {
            get {
                return Utility.newlineTypeToNewLine(newLineType);
            }
            set {
                newLineType = Utility.newlineToNewLineType(value);
            }
        }

        /// <summary>
        /// 获取当前行和上一行之间的换行符类型。
        /// </summary>
        public DocumentLineFlags newLineType {
            get {
                return flags & DocumentLineFlags.NEW_LINE_TYPE;
            }
            set {
                flags = (flags & ~DocumentLineFlags.NEW_LINE_TYPE) | value;
            }
        }

        /// <summary>
        /// 获取当前行的缩进长度。
        /// </summary>
        public int indentCount {
            get {
                for (var i = 0; i < buffer.length; i++) {
                    if (!Utility.isIndentChar(buffer.data[i])) {
                        return i;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// 获取或设置当前行的标记。
        /// </summary>
        public DocumentLineFlags flags;

        /////// <summary>
        /////// 获取或设置当前行的状态。
        /////// </summary>
        ////public DocumentLineFlags state {
        ////    get {
        ////        return flags & DocumentLineFlags.MODIFIE_STATE;
        ////    }
        ////    set {
        ////        flags = (flags & ~DocumentLineFlags.MODIFIE_STATE) | value;
        ////    }
        ////}

        #endregion

        #region 区块

        /// <summary>
        /// 存储当前行最后一个字符之后所属的块。其块所属可能属于当前行或之前行。
        /// </summary>
        private Block _endBlock;

        /// <summary>
        /// 获取属于从当前行开始的所有折叠块。
        /// </summary>
        public IEnumerable<Block> startBlocks {
            get {
                for (var block = _endBlock; block != null && block.startLine == this; block = block.parent) {
                    yield return block;
                }
            }
        }

        /// <summary>
        /// 获取属于从当前行结束的所有折叠块。
        /// </summary>
        public IEnumerable<Block> endBlocks {
            get {
                for (var block = _endBlock; block != null; block = block.parent) {
                    if (block.endLine == this) {
                        yield return block;
                    }
                }
            }
        }

        public IEnumerable<Block> blocks {
            get {
                return null;
            }
        }

        /// <summary>
        /// 获取从当前行开始的最后一个已折叠的块。
        /// </summary>
        /// <returns>返回已折叠的块。</returns>
        public Block getCollapsedBlock() {
            for (var block = _endBlock; block != null && block.startLine == this; block = block.parent) {
                if (block.collapsed) {
                    return block;
                }
            }
            return null;
        }

        #endregion

        #region 片段

        /// <summary>
        /// 获取当前行的所有片段分隔符。
        /// </summary>
        public ArrayList<SegmentSplitter> segments;

        ///// <summary>
        ///// 更新当前行的片段列表。
        ///// </summary>
        ///// <param name="parentBlockSegment">当前行所在的块。</param>
        //public void parseSegments(Block parentBlockSegment) {

        //    //// 清空片段列表。
        //    //_segmentLength = 1;
        //    //_segments[0].type = parentBlockSegment.type;
        //    //_segments[0].startIndex = -1;
        //    //_segments[0].endIndex = -1;

        //    parseSegments(ref parentBlockSegment, 0, _textLength);

        //}

        //private int parseSegments(ref Block parentBlockSegment, int startIndex, int endIndex) {
        //    while (startIndex < endIndex) {
        //        // 每次解析到当前块的结束。
        //        var lastStartIndex = startIndex;
        //        startIndex = parseSegment(ref parentBlockSegment, parentBlockSegment.type, startIndex, endIndex);
        //        if (startIndex < 0) {
        //            return lastStartIndex;
        //        }
        //    }
        //    return startIndex;
        //}

        ///// <summary>
        ///// 解析一个块级的子类型。
        ///// </summary>
        ///// <param name="parentBlockSegment">上级块级。</param>
        ///// <param name="parentSegmentType上级片段类型。</param>
        ///// <param name="startIndex">解析的开始位置。</param>
        ///// <param name="endIndex">解析的结束位置。</param>
        ///// <returns>返回当前块的关闭位置。如果当前块未关闭则返回 -1。</returns>
        //private int parseSegment(ref Block parentBlockSegment, SegmentType parentSegmentType, int startIndex, int endIndex) {
        //redo:

        //    // 1. 查找父块的结束标志。
        //    var parentBlockEnd = parentBlockSegment?.type.end.match(textData, startIndex, endIndex) ?? new PatternMatchResult(-1, -1);

        //    // 2. 查找子块开始标志。
        //    Word childSegment;
        //    childSegment.type = null;
        //    childSegment.startIndex = parentBlockEnd.success ? parentBlockEnd.startIndex : int.MaxValue;
        //    childSegment.endIndex = 0;

        //    // 遍历所有子片段类型，找到匹配的子片段类型。
        //    // 如果发现多个匹配项，则匹配位置最前的生效。
        //    // 子片段可能是完全匹配一个单词或仅仅匹配其开始标记。
        //    if (parentSegmentType.children != null) {
        //        foreach (var segmentType in parentSegmentType.children) {
        //            var matchResult = segmentType.start.match(textData, startIndex, endIndex);
        //            if (matchResult.success && matchResult.startIndex < childSegment.startIndex) {
        //                childSegment.type = segmentType;
        //                childSegment.startIndex = matchResult.startIndex;
        //                childSegment.endIndex = matchResult.endIndex;
        //            }
        //        }
        //    }

        //    // 3. 处理子块开始。
        //    if (childSegment.type != null) {

        //        // 添加片段信息。
        //        var segmentIndex = _segmentSplitterCount;
        //        Utility.appendArrayList(ref segmentSplitterData, ref _segmentSplitterCount);
        //        segmentSplitterData[segmentIndex] = childSegment;

        //        // 区分是块级片段还是内联片段。
        //        if (childSegment.type.isBlock) {

        //            // 创建一个子块。
        //            var block = new Block(parentBlockSegment, (SegmentSegmentType)childSegment.type, this);

        //            // 继续解析内部区块。
        //            var childBlockEnd = parseSegment(ref block, childSegment.type, childSegment.endIndex, endIndex);
        //            segmentSplitterData[segmentIndex].endIndex = childBlockEnd;
        //            if (childBlockEnd < 0) {
        //                if (!childSegment.type.isMultiLine) {
        //                    segmentSplitterData[segmentIndex].endIndex = endIndex;
        //                }
        //                return childBlockEnd;
        //            }

        //            // 跳过当前内部块继续解析剩余内容。
        //            startIndex = childBlockEnd;
        //            goto redo;
        //        } else {

        //            // 内联区块：childSegment.endIndex 表示当前区块的结束点。
        //            Block blockSegment = null;

        //            // 在内部继续查找子片段。
        //            parseSegment(ref blockSegment, childSegment.type, childSegment.startIndex, childSegment.endIndex);

        //            // 跳过当前内部块继续解析剩余内容。
        //            startIndex = childSegment.endIndex;
        //            goto redo;

        //        }

        //    }

        //    // 4. 处理父块结束。
        //    if (parentBlockEnd.success) {

        //        // 如果当前行自之前的行开始则插入一个片段。
        //        if (parentBlockSegment.startLine != this) {
        //            var segmentIndex = _segmentSplitterCount;
        //            Utility.prependArrayList(ref segmentSplitterData, ref _segmentSplitterCount);
        //            segmentSplitterData[0].type = parentSegmentType;
        //            segmentSplitterData[0].startIndex = -1;
        //            segmentSplitterData[0].endIndex = parentBlockEnd.endIndex;
        //        }

        //        // 设置块的结束行。
        //        parentBlockSegment.endLine = this;

        //        // 回溯到父级块。
        //        parentBlockSegment = parentBlockSegment.parent;

        //        // 返回结束行。
        //        return parentBlockEnd.endIndex;
        //    }

        //    // 5. 当前块未结束。
        //    return -1;

        //}

        ///// <summary>
        ///// 解析一个片段类型的子类型。
        ///// </summary>
        ///// <param name="parentSegmentType">上级片段类型。</param>
        ///// <param name="startIndex">解析的开始位置。</param>
        ///// <param name="endIndex">解析的结束位置。</param>
        ///// <returns>返回解析结束后的新结束位置。如果解析未成功则返回开始位置。</returns>
        //private int parseSegments(SegmentType parentSegmentType, int startIndex, int endIndex) {
        //    Segment childSegment;
        //    childSegment.type = null;
        //    childSegment.startIndex = int.MaxValue;
        //    childSegment.endIndex = int.MaxValue;

        //    // 遍历所有子片段类型，找到匹配的子片段类型。
        //    // 如果发现多个匹配项，则匹配位置最前的生效。
        //    // 子片段可能是完全匹配一个单词或仅仅匹配其开始标记。
        //    for (var i = 0; i < parentSegmentType.children.Length; i++) {
        //        var currentSegmentType = parentSegmentType.children[i];
        //        int startIndex, end;
        //        currentSegmentType.startIndex.match(textData, startIndex, endIndex, out startIndex, out end);
        //        if (startIndex > 0 && childSegment.startIndex > startIndex) {
        //            childSegment.type = currentSegmentType;
        //            childSegment.startIndex = startIndex;
        //            childSegment.endIndex = end;
        //        }
        //    }

        //    // 不匹配任何子片段。
        //    if (childSegment.type == null) {
        //        return startIndex;
        //    }

        //    // 添加片段信息。
        //    int segmentIndex = _segmentLength;
        //    Utilty.incArrayList(ref _segments, ref _segmentLength);
        //    _segments[segmentIndex] = childSegment;

        //    // 区分是块级片段还是内联片段。
        //    if (childSegment.type.isBlock) {
        //        // 块级区块：end 表示结束为止。

        //        // 创建一个区块。
        //        var BlockSegment = new BlockSegment();
        //        BlockSegment.startLine = this;
        //        BlockSegment.type = (SegmentSegmentType)childSegment.type;

        //        // 继续解析内部区块。
        //        int newEndIndex = parseSegment(BlockSegment, childSegment.endIndex, endIndex);

        //        // 同行片段不跨越行。
        //        if (newEndIndex <= childSegment.endIndex && !childSegment.type.isMultiLine) {
        //            newEndIndex = endIndex;
        //        }

        //        // 更新区块的结束位置。
        //        _segments[segmentIndex].endIndex = newEndIndex;

        //        return newEndIndex;

        //    }

        //    // 内联区块：childSegment.endIndex 表示当前区块的结束点。

        //    // 在内部继续查找子片段。
        //    parseSegments(childSegment.type, childSegment.startIndex, childSegment.endIndex);

        //    return childSegment.endIndex;

        //}

        #endregion

    }


    public struct DocumentLayoutInfo {

        /// <summary>
        /// 获取当前布局行第一个字符的垂直坐标。如果当前布局行被隐藏，则返回 -1。
        /// </summary>
        public int top;


    }

    ///// <summary>
    ///// 表示一个布局信息元素。
    ///// </summary>
    //public abstract class DocumentLayoutElement {

    //    /// <summary>
    //    /// 获取同个布局行的下一个中断点。
    //    /// </summary>
    //    public LayoutBreakPoint next;

    //    /// <summary>
    //    /// 获取当前中断点的起始列。
    //    /// </summary>
    //    public abstract int startColumn {
    //        get;
    //    }

    //    /// <summary>
    //    /// 判断当前中断点是否是自动换行中断点。
    //    /// </summary>
    //    public abstract bool isWrapPoint {
    //        get;
    //    }

    //}

    ///// <summary>
    ///// 表示一个片段类型。
    ///// </summary>
    //public enum SegmentType {

    //    /// <summary>
    //    /// 表示普通字符。
    //    /// </summary>
    //    word,

    //    /// <summary>
    //    /// 表示空格。
    //    /// </summary>
    //    space,

    //    /// <summary>
    //    /// 表示制表符。
    //    /// </summary>
    //    tab,

    //}

    /// <summary>
    /// 表示一个行内片段分割器。同一行的文本可能被多个分割器分成多个部分。
    /// </summary>
    public struct SegmentSplitter {

        /// <summary>
        /// 获取当前分割列号之前的样式。
        /// </summary>
        public SegmentType type;

        /// <summary>
        /// 获取当前分割的列号。
        /// </summary>
        public int index;

    }

    /// <summary>
    /// 表示一个行内的片段。
    /// </summary>
    [DebuggerStepThrough]
    public struct Segment {

        /// <summary>
        /// 获取当前片段的类型。
        /// </summary>
        public SegmentType type;

        /// <summary>
        /// 获取当前片段在行内的开始位置。
        /// </summary>
        public int startIndex;

        /// <summary>
        /// 获取当前片段在行内的结束位置。
        /// </summary>
        public int endIndex;

        public Segment(SegmentType type, int startIndex, int endIndex) {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.type = type;
        }

        public override string ToString() {
            return $"{startIndex}-{endIndex}: {type}";
        }
    }

    /// <summary>
    /// 表示一个代码块。
    /// </summary>
    public sealed class Block {

        /// <summary>
        /// 获取上一级代码块。
        /// </summary>
        public Block parent { get; internal set; }

        /// <summary>
        /// 获取当前代码块的类型。
        /// </summary>
        public SegmentSegmentType type { get; }

        /// <summary>
        /// 获取当前代码块的起始行。
        /// </summary>
        public DocumentLine startLine { get; internal set; }

        /// <summary>
        /// 获取当前代码块的起始列。
        /// </summary>
        public int startColumn;

        /// <summary>
        /// 获取当前代码块的结束行。
        /// </summary>
        public DocumentLine endLine { get; internal set; }

        /// <summary>
        /// 获取或设置当前块的折叠状态。
        /// </summary>
        public bool collapsed { get; internal set; }
        public int endColumn { get; internal set; }

        public Block(Block parent, SegmentSegmentType type, DocumentLine startLine) {
            this.parent = parent;
            this.type = type;
            this.startLine = startLine;
        }

        public override string ToString() {
            return $"{type}";
        }
    }

}

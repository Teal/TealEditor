﻿using System;
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
        /// <param name="newLine">当前行的换行符.</param>
        /// <param name="value">初始化的值。</param>
        public DocumentLine(DocumentLineFlags newLine, string value) {
            flags = newLine;
            buffer = new StringBuffer(value);
            segments = new ArrayList<SegmentSplitter>(value.Length >> 4);
        }

        /// <summary>
        /// 初始化 <see cref="DocumentLine"/> 类的新实例。
        /// </summary>
        /// <param name="newLine">当前行的换行符.</param>
        /// <param name="value">初始化的值。</param>
        /// <param name="startIndex"><paramref name="value"/> 中的起始位置。</param>
        /// <param name="length"><paramref name="value"/> 中的长度。</param>
        public DocumentLine(DocumentLineFlags newLine, string value, int startIndex, int length) {
            flags = newLine | DocumentLineFlags.modified;
            buffer = new StringBuffer(length + 2);
            buffer.append(value, startIndex, length);
            segments = new ArrayList<SegmentSplitter>(length >> 4);
        }

        /// <summary>
        /// 初始化 <see cref="DocumentLine" /> 类的新实例。
        /// </summary>
        /// <param name="newLine">当前行的换行符.</param>
        /// <param name="capacity">初始化容量。</param>
        public DocumentLine(DocumentLineFlags newLine, int capacity = DocumentConfigs.defaultLineCapacity) {
            flags = newLine | DocumentLineFlags.modified;
            buffer = new StringBuffer(capacity);
            segments = new ArrayList<SegmentSplitter>(capacity >> 4);
        }

        /// <summary>
        /// 初始化 <see cref="DocumentLine" /> 类的新实例。
        /// </summary>
        public DocumentLine()
                : this(DocumentLineFlags.newLineTypeWin, DocumentConfigs.defaultLineCapacity) { }

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

        /// <summary>
        /// 判断当前行是否已解析。
        /// </summary>
        public bool parsed {
            get {
                return (flags & DocumentLineFlags.parsed) != 0;
            }
            set {
                flags = value ? flags | DocumentLineFlags.parsed : flags & ~DocumentLineFlags.parsed;
            }
        }

        /// <summary>
        /// 获取或设置当前行的状态。
        /// </summary>
        public DocumentLineFlags modifyState {
            get {
                return flags & DocumentLineFlags.modifiedAndSaved;
            }
            set {
                flags = (flags & ~(DocumentLineFlags.modifiedAndSaved | DocumentLineFlags.parsed)) | value;
            }
        }

        #endregion

        #region 区块

        /// <summary>
        /// 存储当前行最后一个字符之后所属的块。其块所属可能属于当前行或之前行。
        /// </summary>
        private Block _endingBlock;

        /// <summary>
        /// 获取当前行最开始的块。
        /// </summary>
        /// <returns></returns>
        public Block startingBlock {
            get {
                var block = _endingBlock;
                if (block != null) {
                    for (; block.parent != null && block.parent.startLine == this; block = block.parent)
                        ;
                }
                return block;
            }
        }

        /// <summary>
        /// 获取当前行最结尾的块。
        /// </summary>
        /// <returns></returns>
        public Block endingBlock => _endingBlock;

        /// <summary>
        /// 获取属于从当前行开始的所有折叠块。
        /// </summary>
        public IEnumerable<Block> startBlocks {
            get {
                for (var block = _endingBlock; block != null && block.startLine == this; block = block.parent) {
                    yield return block;
                }
            }
        }

        /// <summary>
        /// 获取属于从当前行结束的所有折叠块。
        /// </summary>
        public IEnumerable<Block> endBlocks {
            get {
                for (var block = _endingBlock; block != null; block = block.parent) {
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
            for (var block = _endingBlock; block != null && block.startLine == this; block = block.parent) {
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

        /// <summary>
        /// 添加一个已找到的片段。
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="type"></param>
        private void addSegment(int startIndex, int endIndex, SegmentType type) {
            Console.WriteLine($"{startIndex}-{endIndex}: {type}");
        }

        /// <summary>
        /// 更新当前行的片段列表。
        /// </summary>
        /// <param name="parentBlockSegment">当前行所在的块。</param>
        public Block parseSegments(Block parentBlockSegment) {
            flags |= DocumentLineFlags.parsed;
            segments.clear();
            return _endingBlock = parseSegmentsCore(parentBlockSegment);
        }

        /// <summary>
        /// 解析一个块级的子类型。
        /// </summary>
        /// <param name="parentBlock">上级块级。</param>
        /// <param name="index">解析的开始位置。</param>
        /// <returns>返回当前块的关闭位置。如果当前块未关闭则返回 -1。</returns>
        private Block parseSegmentsCore(Block parentBlock, int index = 0) {

            // 解析当前行从 startIndex -> endIndex 所有片段的算法：
            // 1. 查找 parentBlockType 的结束点或 parentBlockType 中任何一个子 segmentType 开始点。
            // 2. 选取最靠前的 segmentType 。
            //    2.1 存储子 segmentType 。
            //    2.2 如果最靠前的是 parentBlockType，则：
            //        2.2.1 设置 Block 结束点
            //        2.2.2 退出。
            //    2.3 如果最靠前的是子 segmentType，则：
            //        2.3.2 如果子 segmentType 是 blockType 则：
            //            2.3.2.1 更新 parentBlockType 为子 segmentType 重复步骤 1。
            //        2.3.3 如果 segmentType 是 segmentType 则： 
            //            2.3.3.1 存储子 segmentType 。

            while (index < buffer.length) {

                var parentBlockType = parentBlock.type;

                // 1. 查找父块的结束标志。
                var matchResult = parentBlockType.end?.match(buffer.data, index, buffer.length) ?? (parentBlockType.isMultiLine ? new PatternMatchResult(-1, -1) : new PatternMatchResult(buffer.length, buffer.length));

                // 2. 查找子块开始标志。
                var childSegmentType = (SegmentType)null;
                var minStartIndex = matchResult.success ? matchResult.startIndex : int.MaxValue;

                // 遍历所有子片段类型，找到匹配的子片段类型。
                // 如果发现多个匹配项，则匹配位置最前的生效。
                // 子片段可能是完全匹配一个单词或仅仅匹配其开始标记。
                if (parentBlockType.children != null) {
                    foreach (var segmentType in parentBlockType.children) {
                        var r = segmentType.start.match(buffer.data, index, buffer.length);
                        if (r.success && r.startIndex < minStartIndex) {
                            childSegmentType = segmentType;
                            matchResult = r;
                            minStartIndex = r.startIndex;
                        }
                    }
                }

                // 3. 处理子片段的开始。
                if (childSegmentType != null) {

                    if (childSegmentType.isBlock) {
                        // 如果子片段是另一个块，则递归处理。
                        parentBlock = new Block(parentBlock, (BlockType)childSegmentType, this);
                    } else {
                        // 保存 子块.开始 -> 子块.结束 的样式。
                        addSegment(matchResult.startIndex, matchResult.endIndex, childSegmentType);
                    }

                    index = matchResult.endIndex;
                    continue;

                }

                // 4. 处理父块的结束。
                if (matchResult.success) {

                    // 保存 块.开始 -> 块.结束 的样式。
                    addSegment(parentBlock.startLine == this ? parentBlock.startColumn : 0, matchResult.endIndex, parentBlockType);

                    // 设置块的结束行。
                    parentBlock.endLine = this;
                    parentBlock.endColumn = matchResult.endIndex;

                    // 回溯到父级块。
                    parentBlock = parentBlock.parent;
                    index = matchResult.endIndex;

                    // 继续解析剩下的块
                    continue;
                }

                // 5. 剩下字符不满足任意部分，则处理为父块的最后部分。

                // 保存 块.开始 -> 块.结束 的样式。
                addSegment(parentBlock.startLine == this ? parentBlock.startColumn : 0, buffer.length, parentBlockType);
                break;

            }

            return parentBlock;

        }

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

    ///// <summary>
    ///// 表示一个行内的片段。
    ///// </summary>
    //[DebuggerStepThrough]
    //public struct Segment {

    //    /// <summary>
    //    /// 获取当前片段的类型。
    //    /// </summary>
    //    public SegmentType type;

    //    /// <summary>
    //    /// 获取当前片段在行内的开始位置。
    //    /// </summary>
    //    public int startIndex;

    //    /// <summary>
    //    /// 获取当前片段在行内的结束位置。
    //    /// </summary>
    //    public int endIndex;

    //    public Segment(SegmentType type, int startIndex, int endIndex) {
    //        this.startIndex = startIndex;
    //        this.endIndex = endIndex;
    //        this.type = type;
    //    }

    //    public override string ToString() {
    //        return $"{startIndex}-{endIndex}: {type}";
    //    }
    //}

}

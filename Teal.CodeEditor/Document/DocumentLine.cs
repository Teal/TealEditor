using System;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示文档中的一行。
    /// </summary>
    public sealed class DocumentLine {

        #region 字符和样式

        /// <summary>
        /// 存储当前行的实际字符串数组。
        /// </summary>
        private string _data;

        /// <summary>
        /// 获取当前行的所有字符数组。
        /// </summary>
        public char[] chars;

        /// <summary>
        /// 获取当前行的所有字符样式。
        /// </summary>
        public CharStylePoint[] styles;

        /// <summary>
        /// 存储当前行的字符长度。
        /// </summary>
        private int _length;

        /// <summary>
        /// 获取或设置当前行的字符长度。
        /// </summary>
        public int length {
            get {
                return _length;
            }
            set {
                _length = value;

                if (value < capacity) {
                    return;
                }

                if (value < 128) {
                    capacity = 128;
                    return;
                }

                if (value < 512) {
                    capacity = 512;
                    return;
                }

                if (value < 2048) {
                    capacity = 2048;
                    return;
                }

                capacity = value * 2 + 1;

            }
        }

        /// <summary>
        /// 获取或设置当前行最多可容纳的字符数。
        /// </summary>
        public int capacity {
            get {
                return chars.Length;
            }
            set {
                //var count = Math.Min(chars.Length, value);

                //var newData = new char[value];
                //Array.Copy(chars, newData, count);
                //chars = newData;

                //var newStyles = new CharStyles[value];
                //Array.Copy(styles, newStyles, count);
                //styles = newStyles;

            }
        }

        /// <summary>
        /// 初始化 <see cref="DocumentLine"/> 类的新实例。
        /// </summary>
        /// <param name="chars">当前行的所有字符。</param>
        /// <param name="styles">当前行的所有样式。</param>
        /// <param name="length">设置字符串长度。</param>
        public DocumentLine(char[] chars, CharStyles[] styles, int length) {
            this.chars = chars;
          //  this.styles = styles;
            _length = length;
        }

        /// <summary>
        /// 初始化 <see cref="DocumentLine"/> 类的新实例。
        /// </summary>
        /// <param name="value">初始化的值。</param>
        public DocumentLine(string value)
            : this(value.ToCharArray(), new CharStyles[value.Length], value.Length) { }

        /// <summary>
        /// 初始化 <see cref="DocumentLine" /> 类的新实例。
        /// </summary>
        /// <param name="capacity">初始化容量。</param>
        public DocumentLine(int capacity = 16)
            : this(new char[capacity], new CharStyles[capacity], 0) { }

        /// <summary>
        /// 确保当前行可以容纳指定的字符数。
        /// </summary>
        public void ensureCapacity(int capacity) {
            if (capacity > this.capacity) {
                this.capacity = capacity;
            }
        }

        #endregion

        //#region 追加

        ///// <summary>
        ///// 向当前行追加字符。
        ///// </summary>
        ///// <param name="value">要追加的字符。</param>
        //public void append(char value) {
        //    int currentLength = length;
        //    length++;
        //    chars[currentLength] = value;
        //}

        ///// <summary>
        ///// 向当前行追加字符。
        ///// </summary>
        ///// <param name="value">要追加的字符。</param>
        ///// <param name="repeatCount">字符重复的次数。</param>
        //public void append(char value, int repeatCount) {
        //    int currentLength = length;
        //    length += repeatCount;
        //    while (repeatCount > 0) {
        //        chars[currentLength++] = value;
        //    }
        //}

        ///// <summary>
        ///// 向当前行追加字符。
        ///// </summary>
        ///// <param name="value">要追加的字符数组。</param>
        ///// <param name="startIndex">数组的起始位置。</param>
        ///// <param name="charCount">要追加的字符数。</param>
        //public void append(char[] value, int startIndex, int charCount) {
        //    int currentLength = length;
        //    length += charCount;
        //    Array.Copy(value, startIndex, chars, currentLength, charCount);
        //}

        ///// <summary>
        ///// 向当前行追加字符。
        ///// </summary>
        ///// <param name="value">要追加的字符串。</param>
        //public void append(string value) {
        //    append(value, 0, value.Length);
        //}

        ///// <summary>
        ///// 向当前字符串缓存追加数据。
        ///// </summary>
        ///// <param name="value">要追加的字符串。</param>
        ///// <param name="startIndex">插入的起始位置。</param>
        ///// <param name="count">插入的个数。</param>
        //public void append(string value, int startIndex, int count) {
        //    int currentLength = length;
        //    length += count;
        //    value.CopyTo(startIndex, chars, currentLength, count);
        //}

        //#endregion

        //#region 插入

        ///// <summary>
        ///// 在指定位置插入一个字符。
        ///// </summary>
        ///// <param name="index">插入的位置。</param>
        ///// <param name="value">要插入的字符串。</param>
        ///// <param name="count">插入的长度。</param>
        //public unsafe void insert(int index, char* value, int count) {
        //    if (index >= _length) {
        //        index = _length;
        //    }
        //    length += count;
        //    fixed (char* c = chars) {
        //        fixed (CharStyles* s = styles) {

        //            char* cs = c + length;
        //            char* cd = cs + count;

        //            CharStyles* ss = s + length;
        //            CharStyles* sd = ss + count;

        //            char* ce = c + index;

        //            while (cs >= ce) {
        //                *--cd = *--cs;
        //                *--sd = *--ss;
        //            }

        //            Utility.wstrcpy(ce, value, count);

        //        }
        //    }
        //}

        ///// <summary>
        ///// 在指定位置插入一个字符。
        ///// </summary>
        ///// <param name="index">插入的位置。</param>
        ///// <param name="value">要插入的字符。</param>
        //public unsafe void insert(int index, char value) {
        //    if (index >= _length) {
        //        append(value);
        //        return;
        //    }
        //    int currentLength = length;
        //    length++;
        //    fixed (char* c = chars) {
        //        fixed (CharStyles* s = styles) {

        //            char* cs = c + length;
        //            char* cd = cs + 1;

        //            CharStyles* ss = s + length;
        //            CharStyles* sd = ss + 1;

        //            char* ce = c + index;

        //            while (cs >= ce) {
        //                *--cd = *--cs;
        //                *--sd = *--ss;
        //            }

        //            *ce = value;

        //        }
        //    }
        //}

        ///// <summary>
        ///// 在指定位置插入一个字符串。
        ///// </summary>
        ///// <param name="index">插入的位置。</param>
        ///// <param name="value">要插入的字符串。</param>
        ///// <param name="startIndex">开始的索引。</param>
        ///// <param name="count">插入的长度。</param>
        //public unsafe void insert(int index, string value, int startIndex, int count) {
        //    fixed (char* p = value) {
        //        insert(index, p + startIndex, count);
        //    }
        //}

        ///// <summary>
        ///// 在指定位置插入一个字符串。
        ///// </summary>
        ///// <param name="index">插入的位置。</param>
        ///// <param name="value">要插入的字符串。</param>
        //public void insert(int index, string value) {
        //    insert(index, value, 0, value.Length);
        //}

        ///// <summary>
        ///// 在指定位置插入一个字符数组。
        ///// </summary>
        ///// <param name="index">插入的位置。</param>
        ///// <param name="value">要插入的字符数组。</param>
        ///// <param name="startIndex">开始的索引。</param>
        ///// <param name="count">插入的长度。</param>
        //public unsafe void insert(int index, char[] value, int startIndex, int count) {
        //    fixed (char* p = value) {
        //        insert(index, p + startIndex, count);
        //    }
        //}

        ///// <summary>
        ///// 在指定位置插入一个字符数组。
        ///// </summary>
        ///// <param name="index">插入的位置。</param>
        ///// <param name="value">要插入的字符数组。</param>
        //public void insert(int index, char[] value) {
        //    insert(index, value, 0, value.Length);
        //}

        //#endregion

        //#region 删除

        ///// <summary>
        ///// 清空当前行内容。
        ///// </summary>
        //public void clear() {
        //    length = 0;
        //}

        ///// <summary>
        ///// 从当前缓存删除指定索引的字符串。
        ///// </summary>
        ///// <param name="startIndex">开始的索引。</param>
        ///// <param name="count">删除的长度。</param>
        //public unsafe void remove(int startIndex, int count) {
        //    fixed (char* p = chars) {
        //        fixed (CharStyles* s = styles) {

        //            char* ps = p + startIndex;
        //            CharStyles* ss = s + startIndex;

        //            int copyLength = length - startIndex - count;
        //            Utility.wstrcpy(ps, ps + count, copyLength);
        //            Utility.memcpy((byte*)ss, (byte*)(ss + count), copyLength * sizeof(CharStyles));

        //            length -= count;
        //        }
        //    }

        //}

        ///// <summary>
        ///// 从当前缓存删除指定索引的字符串。
        ///// </summary>
        ///// <param name="startIndex">开始的索引。</param>
        //public void remove(int startIndex) {
        //    remove(startIndex, length - startIndex);
        //}

        //#endregion

        //#region 获取

        ///// <summary>
        ///// 获取从指定位置开始指定长度的子字符串。
        ///// </summary>
        ///// <param name="startIndex">开始的索引。</param>
        ///// <param name="length">获取的字符串长度。</param>
        ///// <returns>返回子字符串。</returns>
        //public string substring(int startIndex, int length) {
        //    return new String(chars, startIndex, length);
        //}

        ///// <summary>
        ///// 获取从指定位置开始指定长度的子字符串。
        ///// </summary>
        ///// <param name="startIndex">开始的索引。</param>
        ///// <returns>返回子字符串。</returns>
        //public string substring(int startIndex) {
        //    return substring(startIndex, length - startIndex);
        //}

        ///// <summary>
        ///// 获取或设置当前行的文本。
        ///// </summary>
        //public string text {
        //    get {
        //        return _length == 0 ? String.Empty : new String(chars, 0, _length);
        //    }
        //    set {
        //        length = value.Length;
        //        value.CopyTo(0, chars, 0, value.Length);
        //        Array.Clear(styles, 0, value.Length);
        //    }
        //}

        ///// <summary>
        ///// 获取或设置指定索引的字符。
        ///// </summary>
        ///// <param name="index"></param>
        ///// <returns></returns>
        //public char this[int index] {
        //    get {
        //        return chars[index];
        //    }
        //    set {
        //        chars[index] = value;
        //    }
        //}

        ///// <summary>
        ///// 返回当前行的字符串形式。
        ///// </summary>
        ///// <returns>表示当前对象的字符串。</returns>
        //public override string ToString() {
        //    return text;
        //}

        //#endregion

        //#region 行的属性

        ///// <summary>
        ///// 获取当前行和上一行之间的换行符。
        ///// </summary>
        //public string newLine {
        //    get {
        //        return Utility.newlineTypeToNewLine(newLineType);
        //    }
        //    set {
        //        newLineType = Utility.newlineToNewLineType(value);
        //    }
        //}

        ///// <summary>
        ///// 获取当前行和上一行之间的换行符类型。
        ///// </summary>
        //public DocumentLineFlags newLineType {
        //    get {
        //        return flags & DocumentLineFlags.NEW_LINE_TYPE;
        //    }
        //    set {
        //        flags = (flags & ~DocumentLineFlags.NEW_LINE_TYPE) | value;
        //    }
        //}

        ///// <summary>
        ///// 获取当前行的缩进长度。
        ///// </summary>
        //public int indentCount {
        //    get {
        //        for (var i = 0; i < length; i++) {
        //            if (chars[i] != ' ' && chars[i] != '\t' && chars[i] != '\u3000') {
        //                return i;
        //            }
        //        }
        //        return 0;
        //    }
        //}

        ///// <summary>
        ///// 获取或设置当前行的书签。
        ///// </summary>
        //public DocumentLineFlags flags;

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

        //#endregion

        //#region 布局



        //#endregion

        #region 样式

        /// <summary>
        /// 获取属于当前行的所有片段。
        /// </summary>
        public ArrayList<Segment> segments {
            get;
        }

        /// <summary>
        /// 所有的片段列表。
        /// </summary>
        private Segment[] _segments;

        /// <summary>
        /// 所有的片段列表实际长度。
        /// </summary>
        private int _segmentLength;

        /// <summary>
        /// 获取属于当前行的最后一个语句块。
        /// </summary>
        public Block lastBlock {
            get {
                if (endBlock.startLine == this) {
                    return endBlock;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取当前行末尾的所属块。
        /// </summary>
        public Block endBlock {
            get;
            internal set;
        }

        /// <summary>
        /// 更新当前行的片段列表。
        /// </summary>
        /// <param name="parentBlock">当前行所在的块。</param>
        public void parseSegments(Block parentBlock) {

            //// 清空片段列表。
            //_segmentLength = 1;
            //_segments[0].type = parentBlock.type;
            //_segments[0].startIndex = -1;
            //_segments[0].endIndex = -1;

            parseSegments(ref parentBlock, 0, _length);

        }

        private int parseSegments(ref Block parentBlock, int startIndex, int endIndex) {
            while (startIndex < endIndex) {
                // 每次解析到当前块的结束。
                int lastStartIndex = startIndex;
                startIndex = parseSegment(ref parentBlock, parentBlock.type, startIndex, endIndex);
                if (startIndex < 0) {
                    return lastStartIndex;
                }
            }
            return startIndex;
        }

        /// <summary>
        /// 解析一个块级的子类型。
        /// </summary>
        /// <param name="parentBlock">上级块级。</param>
        /// <param name="parentSegmentType">上级片段类型。</param>
        /// <param name="startIndex">解析的开始位置。</param>
        /// <param name="endIndex">解析的结束位置。</param>
        /// <returns>返回当前块的关闭位置。如果当前块未关闭则返回 -1。</returns>
        private int parseSegment(ref Block parentBlock, SegmentType parentSegmentType, int startIndex, int endIndex) {
        redo:

            // 1. 查找父块的结束标志。
            int parentBlockEndLeft;
            int parentBlockEndRight;
            if (parentBlock != null) {
                parentBlock.type.end.match(_data, startIndex, endIndex, out parentBlockEndLeft, out parentBlockEndRight);
            } else {
                parentBlockEndLeft = parentBlockEndRight = -1;
            }

            // 2. 查找子块开始标志。
            Segment childSegment;
            childSegment.type = null;
            childSegment.startIndex = parentBlockEndLeft == -1 ? int.MaxValue : childSegment.startIndex;
            childSegment.endIndex = 0;

            // 遍历所有子片段类型，找到匹配的子片段类型。
            // 如果发现多个匹配项，则匹配位置最前的生效。
            // 子片段可能是完全匹配一个单词或仅仅匹配其开始标记。
            if (parentSegmentType.children != null) {
                for (var i = 0; i < parentSegmentType.children.Length; i++) {
                    var currentSegmentType = parentSegmentType.children[i];
                    int start, end;
                    currentSegmentType.start.match(_data, startIndex, endIndex, out start, out end);
                    if (start > 0 && childSegment.startIndex > start) {
                        childSegment.type = currentSegmentType;
                        childSegment.startIndex = start;
                        childSegment.endIndex = end;
                    }
                }
            }

            // 3. 处理子块开始。
            if (childSegment.type != null) {

                // 添加片段信息。
                int segmentIndex = _segmentLength;
                Utility.appendArrayList(ref _segments, ref _segmentLength);
                _segments[segmentIndex] = childSegment;

                // 区分是块级片段还是内联片段。
                if (childSegment.type.isBlock) {

                    // 创建一个子块。
                    var block = new Block();
                    block.parent = parentBlock;
                    block.startLine = this;
                    block.type = (BlockSegmentType)childSegment.type;

                    // 继续解析内部区块。
                    int childBlockEnd = parseSegment(ref block, childSegment.type, childSegment.endIndex, endIndex);
                    _segments[segmentIndex].endIndex = childBlockEnd;
                    if (childBlockEnd < 0) {
                        return childBlockEnd;
                    }

                    // 跳过当前内部块继续解析剩余内容。
                    startIndex = childBlockEnd;
                    goto redo;
                } else {

                    // 内联区块：childSegment.endIndex 表示当前区块的结束点。
                    Block block = null;

                    // 在内部继续查找子片段。
                    parseSegment(ref block, childSegment.type, childSegment.startIndex, childSegment.endIndex);

                    // 跳过当前内部块继续解析剩余内容。
                    startIndex = childSegment.endIndex;
                    goto redo;

                }

            }

            // 4. 处理父块结束。
            if (parentBlockEndLeft != -1) {

                // 如果当前行自之前的行开始则插入一个片段。
                if (parentBlock.startLine != this) {
                    int segmentIndex = _segmentLength;
                    Utility.prependArrayList(ref _segments, ref _segmentLength);
                    _segments[0].type = parentSegmentType;
                    _segments[0].startIndex = -1;
                    _segments[0].endIndex = parentBlockEndRight;
                }

                // 设置块的结束行。
                parentBlock.endLine = this;

                // 回溯到父级块。
                parentBlock = parentBlock.parent;

                // 返回结束行。
                return parentBlockEndRight;
            }

            // 5. 当前块未结束。
            return -1;

        }

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
        //        int start, end;
        //        currentSegmentType.start.match(_data, startIndex, endIndex, out start, out end);
        //        if (start > 0 && childSegment.startIndex > start) {
        //            childSegment.type = currentSegmentType;
        //            childSegment.startIndex = start;
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
        //        var block = new Block();
        //        block.startLine = this;
        //        block.type = (BlockSegmentType)childSegment.type;

        //        // 继续解析内部区块。
        //        int newEndIndex = parseSegment(block, childSegment.endIndex, endIndex);

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

    /// <summary>
    /// 表示一个字符样式点。用于声明指定位置之后的字符样式为新的样式。
    /// </summary>
    public struct CharStylePoint {

        /// <summary>
        /// 当前样式的开始位置。
        /// </summary>
        public int index;

        /// <summary>
        /// 当前应用的字符样式。
        /// </summary>
        public CharStyles styles;

    }

    /// <summary>
    /// 表示一个字符样式。
    /// </summary>
    [Flags]
    public enum CharStyles {

        /// <summary>
        /// 无样式。
        /// </summary>
        none = 0,

        /// <summary>
        /// 空格。
        /// </summary>
        space = 1,

        /// <summary>
        /// 存在错误的文本。
        /// </summary>
        errorText = 1 << 1,

        /// <summary>
        /// 存在警告的文本。
        /// </summary>
        warningText = 1 << 2,

        /// <summary>
        /// 存在警告的文本。
        /// </summary>
        hintText = 1 << 3,

        /// <summary>
        /// 存在超链接的文本。
        /// </summary>
        hyperText = 1 << 4,

    }

    public struct DocumentLayoutInfo {

        /// <summary>
        /// 获取当前布局行第一个字符的垂直坐标。如果当前布局行被隐藏，则返回 -1。
        /// </summary>
        public int top;


    }

    /// <summary>
    /// 表示一个布局信息元素。
    /// </summary>
    public abstract class DocumentLayoutElement {

        /// <summary>
        /// 获取同个布局行的下一个中断点。
        /// </summary>
        public LayoutBreakPoint next;

        /// <summary>
        /// 获取当前中断点的起始列。
        /// </summary>
        public abstract int startColumn {
            get;
        }

        /// <summary>
        /// 判断当前中断点是否是自动换行中断点。
        /// </summary>
        public abstract bool isWrapPoint {
            get;
        }

    }

    /// <summary>
    /// 表示一个文档行标记。
    /// </summary>
    public enum DocumentLineFlags {

        /// <summary>
        /// 无任何书签。
        /// </summary>
        none = 0,

        /// <summary>
        /// 表示当前行由 \r\n 换行。
        /// </summary>
        newLineTypeWin = 0,

        /// <summary>
        /// 表示当前行由 \r 换行。
        /// </summary>
        newLineTypeMac = 1 << 0,

        /// <summary>
        /// 表示当前行由 \n 换行。
        /// </summary>
        newLineTypeUnix = 1 << 1,

        /// <summary>
        /// 表示换行符类型。
        /// </summary>
        NEW_LINE_TYPE = newLineTypeWin | newLineTypeMac | newLineTypeUnix,

        ///// <summary>
        ///// 已修改。
        ///// </summary>
        //modified = 1 << 0,

        ///// <summary>
        ///// 已修改并保存。
        ///// </summary>
        //modifiedAndSaved = 1 << 1,

        ///// <summary>
        ///// 获取书签状态。
        ///// </summary>
        //MODIFIE_STATE = modified | modifiedAndSaved,

        ///// <summary>
        ///// 指示当前行需要重新布局。
        ///// </summary>
        //needRelayout,

        /// <summary>
        /// 当前行已解析。
        /// </summary>
        parsed = 1 << 2,

        /// <summary>
        /// 普通书签。
        /// </summary>
        bookmark = 1 << 3,

        /// <summary>
        /// 包含断点。
        /// </summary>
        breakpoint = 1 << 4,

        /// <summary>
        /// 正在激活。
        /// </summary>
        actived = 1 << 5,

        /// <summary>
        /// 表示当前行包含一个定义。
        /// </summary>
        definition = 1 << 6,

    }

    /// <summary>
    /// 表示一个行内的片段。
    /// </summary>
    public struct Segment {

        /// <summary>
        /// 获取当前片段在行内的开始位置。如果开始位置属于上一行，则返回 -1。
        /// </summary>
        public int startIndex { get; internal set; }

        /// <summary>
        /// 获取当前片段在行内的结束位置。如果结束位置属于下一行，则返回 -1。
        /// </summary>
        public int endIndex { get; internal set; }

        /// <summary>
        /// 获取当前片段的类型。
        /// </summary>
        public SegmentType type { get; internal set; }

        public Segment(int startIndex, int endIndex, SegmentType type) {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.type = type;
        }
    }

    /// <summary>
    /// 表示一个跨行的代码块。
    /// </summary>
    public sealed class Block {

        /// <summary>
        /// 获取同行的上一级代码块。
        /// </summary>
        public Block parent { get; internal set; }

        /// <summary>
        /// 获取当前代码块的起始行。
        /// </summary>
        public DocumentLine startLine { get; internal set; }

        /// <summary>
        /// 获取当前代码块的结束行。
        /// </summary>
        public DocumentLine endLine { get; internal set; }

        /// <summary>
        /// 获取或设置当前块的折叠状态。
        /// </summary>
        public bool collapsed { get; internal set; }

        /// <summary>
        /// 获取当前代码块的类型。
        /// </summary>
        public BlockSegmentType type { get; internal set; }
    }

}

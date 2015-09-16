using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed class Document {

        #region 行

        /// <summary>
        /// 获取当前文档的所有行。
        /// </summary>
        public List<DocumentLine> lines = new List<DocumentLine>();

        /// <summary>
        /// 获取指定索引的行。
        /// </summary>
        /// <param name="index">要获取的行号。行号从 0 开始。</param>
        /// <returns>返回指定的行。</returns>
        public DocumentLine this[int index] {
            get {
                return index >= 0 && index < lines.Count ? lines[index] : null;
            }
        }

        ///// <summary>
        ///// 创建用于读取当前文档内容的读取器。
        ///// </summary>
        ///// <returns></returns>
        //public TextReader createReader() {
        //    return new DocumentReader(this);
        //}

        ///// <summary>
        ///// 创建用于写入到当前文档的写入器。
        ///// </summary>
        ///// <returns></returns>
        //public TextWriter createWriter() {
        //    return new DocumentWriter(this);
        //}

        ///// <summary>
        ///// 从指定的读取器载入文档。
        ///// </summary>
        ///// <param name="reader">要载入的读取器。</param>
        //public void load(TextReader reader) {
        //    lines.Clear();

        //    DocumentLine currentLine = new DocumentLine();
        //    lines.Add(currentLine);
        //    int c;
        //    while ((c = reader.Read()) > 0) {
        //        if (c == '\r') {
        //            currentLine = new DocumentLine();
        //            if (reader.Peek() == '\n') {
        //                reader.Read();
        //            } else {
        //                currentLine.flags |= DocumentLineFlags.newLineTypeMac;
        //            }
        //            lines.Add(currentLine);
        //            continue;
        //        }

        //        if (c == '\n') {
        //            currentLine = new DocumentLine();
        //            currentLine.flags |= DocumentLineFlags.newLineTypeUnix;
        //            lines.Add(currentLine);
        //            continue;
        //        }

        //        currentLine.append((char)c);
        //    }
        //}

        ///// <summary>
        ///// 将当前文档的数据写到指定的输出器。
        ///// </summary>
        ///// <param name="writer">要写入的输出器。</param>
        //public void save(TextWriter writer) {
        //    for (var i = 0; i < lines.Count; i++) {
        //        var line = lines[i];
        //        if (line.newLine != null) {
        //            writer.Write(line.newLine);
        //        }
        //        writer.Write(line.chars, 0, line.length);
        //    }
        //}

        ///// <summary>
        ///// 获取或设置当前文档的全部文本。
        ///// </summary>
        //public string text {
        //    get {
        //        var writer = new StringWriter();
        //        save(writer);
        //        return writer.ToString();
        //    }
        //    set {
        //        load(new StringReader(value));
        //    }
        //}

        ///// <summary>
        ///// 获取指定区间的文本。
        ///// </summary>
        ///// <param name="startLine"></param>
        ///// <param name="startColumn"></param>
        ///// <param name="endLine"></param>
        ///// <param name="endColumn"></param>
        ///// <returns></returns>
        //public string getText(int startLine, int startColumn, int endLine, int endColumn) {
        //    StringBuilder sb = new StringBuilder();
        //    write(sb, startLine, startColumn, endLine, endColumn);
        //    return sb.ToString();
        //}

        ///// <summary>
        ///// 将指定区间的文本写入缓存器。
        ///// </summary>
        ///// <param name="sb"></param>
        ///// <param name="startLine"></param>
        ///// <param name="startColumn"></param>
        ///// <param name="endLine"></param>
        ///// <param name="endColumn"></param>
        //public void write(StringBuilder sb, int startLine, int startColumn, int endLine, int endColumn) {
        //    if (endLine == startLine) {
        //        sb.Append(lines[startLine].chars, startColumn, endColumn - startColumn);
        //    } else {
        //        sb.Append(lines[startLine].chars, startColumn, lines[startLine].length - startColumn);
        //        for (var i = startLine + 1; i < endLine; i++) {
        //            sb.Append(lines[i].newLine);
        //            sb.Append(lines[i].chars, 0, lines[i].length);
        //        }
        //        sb.Append(lines[endLine].newLine);
        //        sb.Append(lines[endLine].chars, 0, endColumn);
        //    }
        //}

        ///// <summary>
        ///// 返回表示当前对象的字符串。
        ///// </summary>
        ///// <returns>
        ///// 表示当前对象的字符串。
        ///// </returns>
        //public override string ToString() {
        //    return text;
        //}

        #endregion

        #region 区域

        ///// <summary>
        ///// 表示一个 Range 对象弱引用。
        ///// </summary>
        //private class RangeWeakReference : WeakReference {

        //    private RangeWeakReference prev;

        //    public RangeWeakReference(Range range)
        //        : base(range) {

        //    }

        //}

        ///// <summary>
        ///// 所有 Range 对象列，文档更新后需同时更新所有 Range 对象。
        ///// </summary>
        //RangeWeakReference _ranges;

        ///// <summary>
        ///// 创建一个文档区域对象。
        ///// </summary>
        ///// <returns></returns>
        //public Range createRange() {
        //    var range = new Range();

        //    // todo: 添加列表

        //    return range;
        //}

        #endregion

        #region 换行

        /// <summary>
        /// 当前文档的换行符。
        /// </summary>
        private DocumentLineFlags _newLineType = CodeEditorConfigs.defaultNewLineType;

        /// <summary>
        /// 判断当前文档是否包含混合的换行符。
        /// </summary>
        public bool hasMixedNewLine {
            get {
                //if (lines.Count == 0) {
                //    return false;
                //}

                //var lastLine = lines[lines.Count - 1].newLineType;
                //for (var i = 1; i < lines.Count; i++) {
                //    if (lines[i].newLineType != lastLine) {
                //        return false;
                //    }
                //}


                return true;
            }
        }

        /// <summary>
        /// 获取或设置当前文档的换行符。
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
        /// 获取或设置当前文档的换行符。
        /// </summary>
        public DocumentLineFlags newLineType {
            get {
                if (lines.Count == 0) {
                    return _newLineType;
                }

                return lines[lines.Count - 1].newLineType;

            }
            set {
                if (newLineType != value || hasMixedNewLine) {
                    _newLineType = value;
                    for (var i = 1; i < lines.Count; i++) {
                        lines[i].newLineType = value;
                    }
                    modifyState = ModifyState.modified;
                }
            }
        }

        #endregion

        #region 编辑

        /// <summary>
        /// 当前文档的修改状态。
        /// </summary>
        private ModifyState _modifyState;

        /// <summary>
        /// 当文档修改状态更改时触发。
        /// </summary>
        public event Action modifyStateChange;

        /// <summary>
        /// 判断或设置当前文档的修改状态。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModifyState modifyState {
            get {
                return _modifyState;
            }
            set {
                if (_modifyState != value) {
                    _modifyState = value;
                    if (modifyStateChange != null) {
                        modifyStateChange();
                    }
                }
            }
        }

        /// <summary>
        /// 当文档被更新时触发。
        /// </summary>
        public event Action<int, int, int, int, int, int> update;

        /// <summary>
        /// 触发文档被更新事件。
        /// </summary>
        /// <param name="line">发生改变的行。</param>
        /// <param name="column">发生改变的列。</param>
        /// <param name="deleteLineCount">删除的行数。</param>
        /// <param name="insertLineCount">插入的行数。</param>
        /// <param name="deleteColumnCount">删除的列数。</param>
        /// <param name="insertColumnCount">插入的列数。</param>
        private void onUpdate(int line, int column, int deleteLineCount, int insertLineCount, int deleteColumnCount, int insertColumnCount) {

            // 更新文档修改状态。
            modifyState = ModifyState.modified;

            if (update != null) {
                update(line, column, deleteLineCount, insertLineCount, deleteColumnCount, insertColumnCount);
            }

        }

        #endregion

        #region 修改

        /// <summary>
        /// 在指定位置插入一行。
        /// </summary>
        /// <param name="line">插入的行号。新插入的行将在指定行之后。</param>
        /// <param name="column">插入的列号。新插入的行将从指定列开始。</param>
        /// <param name="inheritIndents">指示新行是否继承原行的缩进。</param>
        /// <param name="newLineType">新行的换行符。如果为 null 则继承上一行。</param>
        /// <returns>返回插入的新行。</returns>
        public DocumentLine breakLine(int line, int column, bool inheritIndents = true, DocumentLineFlags newLineType = DocumentLineFlags.NEW_LINE_TYPE) {
            int indentCount;
            return breakLine(line, column, inheritIndents, newLineType, out indentCount);
        }

        /// <summary>
        /// 在指定位置插入一行。
        /// </summary>
        /// <param name="line">插入的行号。新插入的行将在指定行之后。</param>
        /// <param name="column">插入的列号。新插入的行将从指定列开始。</param>
        /// <param name="inheritIndents">指示新行是否继承原行的缩进。</param>
        /// <param name="newLineType">新行的换行符。如果为 null 则继承上一行。</param>
        /// <param name="indentCount">输出缩进的个数。</param>
        /// <returns>返回插入的新行。</returns>
        public DocumentLine breakLine(int line, int column, bool inheritIndents, DocumentLineFlags newLineType, out int indentCount) {

            var oldDocumentLine = lines[line];

            // 生成新行。
            var newDocumentLine = new DocumentLine();

            // 插入换行。
            if (newLineType == DocumentLineFlags.NEW_LINE_TYPE) {
                newLineType = oldDocumentLine.newLineType;
            }
            newDocumentLine.newLineType = newLineType;

            // 拷贝继承缩进。
            if (inheritIndents) {
                newDocumentLine.append(oldDocumentLine.data, 0, oldDocumentLine.indentCount);
            }

            // 生成新列。
            var newColumn = indentCount = newDocumentLine.length;

            // 将原行数据分成两行。
            var restCount = oldDocumentLine.length - column;
            if (restCount > 0) {
                newDocumentLine.append(oldDocumentLine.data, column, restCount);
                oldDocumentLine.remove(column);
            }

            // 保存新行。
            lines.Insert(line + 1, newDocumentLine);

            onUpdate(line, column, 0, 1, column, newColumn);

            return newDocumentLine;
        }

        /// <summary>
        /// 将指定的行将和上一行合并为一行。
        /// </summary>
        /// <param name="line">要删除的行号。</param>
        /// <param name="column">要删除的列号。</param>
        /// <returns>返回删除的行。</returns>
        public DocumentLine unbreakLine(int line, int column = 0) {
            if (line == 0) {
                return null;
            }

            var oldLine = lines[line];
            lines[line - 1].append(oldLine.data, column, oldLine.length - column);
            lines.RemoveAt(line);

            onUpdate(line, column, 1, 0, column, lines[line - 1].length - column);

            return oldLine;
        }

        /// <summary>
        /// 在指定位置插入一个非换行字符。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符。不允许插入换行符。</param>
        public void insert(int line, int column, char value) {
            lines[line].insert(column, value);
            onUpdate(line, column, 0, 0, 0, 1);
        }

        /// <summary>
        /// 替换指定位置的字符。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符。不允许插入换行符。</param>
        public void replace(int line, int column, char value) {
            lines[line][column] = value;
            onUpdate(line, column + 1, 0, 0, 1, 1);
        }

        /// <summary>
        /// 删除指定位置的字符。
        /// </summary>
        /// <param name="line">删除的行号。</param>
        /// <param name="column">删除的列号。</param>
        public void delete(int line, int column) {
            lines[line].remove(column, 1);
            onUpdate(line, column, 0, 0, 1, 0);
        }

        ///// <summary>
        ///// 在指定位置插入一个多行字符串。
        ///// </summary>
        ///// <param name="line">插入的行号。</param>
        ///// <param name="column">插入的列号。</param>
        ///// <param name="value">插入的字符串。</param>
        ///// <returns>返回插入后的新位置。</returns>
        //public Point insert(int line, int column, string value) {
        //    return insert(line, column, value, 0, value.Length);
        //}

        ///// <summary>
        ///// 在指定位置插入一个多行字符串。
        ///// </summary>
        ///// <param name="line">插入的行号。</param>
        ///// <param name="column">插入的列号。</param>
        ///// <param name="value">插入的字符串。</param>
        ///// <param name="startIndex">插入的字符串起始位置。</param>
        ///// <param name="length">插入的字符串长度。</param>
        ///// <returns>返回插入后的新位置。</returns>
        //public Point insert(int line, int column, string value, int startIndex, int length) {

        //    var currentLineNumber = line;
        //    var currentLine = lines[currentLineNumber];
        //    string restNewLine = currentLine.newLine;
        //    string rest = null;

        //    // 剪切被删除的行尾。
        //    var restCount = currentLine.length - column;
        //    if (restCount > 0) {
        //        rest = new String(currentLine.data, column, restCount);
        //        currentLine.remove(column);
        //    }

        //    // 插入行。
        //    int index = startIndex, count;
        //    string newLine;
        //    while ((count = Utility.readLine(value, length, ref index, out newLine)) > 0) {

        //        // 插入当前行字符串。
        //        currentLine.append(value, startIndex, count);
        //        startIndex = index;

        //        // 如果发现了换行符，则进行换行。
        //        if (newLine != null) {

        //            // 更新当前行的换行符。
        //            currentLine.newLine = newLine;

        //            // 创建新行。
        //            currentLine = new DocumentLine();
        //            lines.Insert(++currentLineNumber, currentLine);

        //        }

        //    }

        //    var newColumn = currentLine.length;

        //    // 重新复制被粘贴的末尾。
        //    if (rest != null) {
        //        currentLine.append(rest);
        //    }

        //    // 重新设置新换行符。
        //    currentLine.newLine = restNewLine;

        //    onUpdate(line, column, 0, currentLineNumber - line, 0, newColumn - column);

        //    return new Point(newColumn, currentLineNumber);
        //}

        ///// <summary>
        ///// 替换指定位置的多行字符串。
        ///// </summary>
        ///// <param name="startLine">替换的起始行。</param>
        ///// <param name="startColumn">替换的起始列。</param>
        ///// <param name="endLine">替换的结束行。</param>
        ///// <param name="endColumn">替换的结束列。</param>
        ///// <param name="value">插入的字符串。</param>
        ///// <returns>返回插入后的新位置。</returns>
        //public Point replace(int startLine, int startColumn, int endLine, int endColumn, string value) {
        //    return replace(startLine, startColumn, endLine, endColumn, value, 0, value.Length);
        //}

        ///// <summary>
        ///// 替换指定位置的多行字符串。
        ///// </summary>
        ///// <param name="startLine">替换的起始行。</param>
        ///// <param name="startColumn">替换的起始列。</param>
        ///// <param name="endLine">替换的结束行。</param>
        ///// <param name="endColumn">替换的结束列。</param>
        ///// <param name="value">替换的字符串。</param>
        ///// <param name="startIndex">替换的字符串起始位置。</param>
        ///// <param name="value">替换的字符串长度。</param>
        ///// <returns>返回插入后的新位置。</returns>
        //public Point replace(int startLine, int startColumn, int endLine, int endColumn, string value, int startIndex, int length) {

        //    var currentLineNumber = startLine;
        //    var currentLine = lines[currentLineNumber];
        //    string rest = null;

        //    // 剪切被删除的行尾。
        //    var restCount = lines[endLine].length - endColumn;
        //    if (restCount > 0) {
        //        rest = new String(lines[endLine].chars, endColumn, restCount);
        //    }

        //    // 删除当前行多余的内容。
        //    currentLine.remove(startColumn);

        //    // 插入行。
        //    int index = startIndex, count;
        //    string newLine;
        //    while ((count = Utility.readLine(value, length, ref index, out newLine)) > 0) {

        //        // 插入当前行字符串。
        //        currentLine.append(value, startIndex, count);
        //        startIndex = index;

        //        // 如果发现了换行符，则进行换行。
        //        if (newLine != null) {

        //            // 创建新行。
        //            if (++currentLineNumber <= endLine) {
        //                currentLine = lines[currentLineNumber];
        //                currentLine.clear();
        //            } else {
        //                currentLine = new DocumentLine();
        //                lines.Insert(currentLineNumber, currentLine);
        //            }

        //            // 更新当前行的换行符。
        //            currentLine.newLine = newLine;

        //        }

        //    }

        //    var newColumn = currentLine.length;

        //    // 粘贴被剪切的末尾。
        //    if (rest != null) {
        //        currentLine.append(rest);
        //    }

        //    // 去除多余的行。
        //    for (var i = endLine; i > currentLineNumber; i++) {
        //        lines.RemoveAt(i);
        //    }

        //    onUpdate(endLine, endColumn, endLine - startLine, currentLineNumber - startLine, endColumn - startColumn, newColumn - startColumn);

        //    return new Point(newColumn, currentLineNumber);
        //}

        ///// <summary>
        ///// 删除指定区间的字符串。
        ///// </summary>
        ///// <param name="startLine">删除的起始行。</param>
        ///// <param name="startColumn">删除的起始列。</param>
        ///// <param name="endLine">删除的结束行。</param>
        ///// <param name="endColumn">删除的结束列。</param>
        //public void delete(int startLine, int startColumn, int endLine, int endColumn) {

        //    if (startLine == endLine) {
        //        lines[startLine].remove(startColumn, endColumn - startColumn);
        //        onUpdate(endLine, endColumn, 0, 0, endColumn - startColumn, 0);
        //        return;
        //    }

        //    var oldLine = lines[startLine];

        //    oldLine.remove(startColumn);
        //    oldLine.append(lines[endLine].chars, endColumn, lines[endLine].length - endColumn);

        //    for (var i = endLine; i > startLine; i++) {
        //        lines.RemoveAt(i);
        //    }

        //    onUpdate(endLine, endColumn, endLine - startLine, 0, endColumn - startColumn, 0);

        //}

        #endregion

    }

    /// <summary>
    /// 表示文档的修改状态。
    /// </summary>
    public enum ModifyState {

        /// <summary>
        /// 文档未修改。
        /// </summary>
        unmodified,

        /// <summary>
        /// 文档已修改。
        /// </summary>
        modified,

        /// <summary>
        /// 文档已修改并保存。
        /// </summary>
        modifiedAndSaved,

    }

}
